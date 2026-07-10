using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using ModbusTCP;
using Core.Config;
using StageWin.WagoIO;
using StageWin.Etc;
using StageWin.Safety;
using System.Collections.Concurrent;

namespace StageWin.UI
{
    public partial class IOForm : Form
    {
        private ModbusTCP.Master MBmaster;
        private volatile bool _reconnecting = false;
        private byte[] data;
        private System.Windows.Forms.Timer _modbusReadTimer;
        private bool _isForceOutputMode = false; // Force Output Mode 상태
        private readonly object _modbusLock = new object(); // Modbus 통신 동기화 객체
        private bool _isReading = false; // 재진입 방지 플래그

        private List<IOData> allIOList; // cfg 파일 전체 데이터
        private List<string> statusList; // Status 값(X100, X101 등) 리스트

        private List<string> inputStatusList; // 'X'로 시작하는 Status 리스트
        private List<string> outputStatusList; // 'Y'로 시작하는 Status 리스트

        private int currentInputStatusIndex = 0;
        private int currentOutputStatusIndex = 0;

        private const byte fctReadCoil = 1;
        private const byte fctWriteSingleCoil = 5;
        private const byte fctReadInputRegister = 4;

        // IOForm 클래스 내부 필드 추가
        private bool _useSim = false;
        private string _ip = "192.168.1.2";
        private int _port = 502;
        private string _cfgPath = @"D:\AppConfig\IO.cfg";

        // 시뮬레이터일 때 사용할 메모리 상태(은행별 16비트)
        private readonly Dictionary<string, bool[]> _simInputs = new Dictionary<string, bool[]>();
        private readonly Dictionary<string, bool[]> _simOutputs = new Dictionary<string, bool[]>();

        // IOForm 필드들 사이에 추가
        private bool _isConnected = false;
        private ScanRequest? _lastIssuedScan = null;
        private bool _scanToggle = false;

        // Safety 인터락 관련 항목
        private readonly ISafetyContext _safetyCtx;
        private readonly IAlarmSink _safetyAlarm;
        public Func<ProgramMode> ProgramModeProvider { get; set; } = null;
        private volatile ProgramMode _lastKnownMode = ProgramMode.Manual;
        // 확인용(원하면 라벨 등에 바인딩해도 됨)
        public ProgramMode CurrentProgramMode => _lastKnownMode;
        private ProgramMode _prevMode = ProgramMode.Manual;
        public void BindProgramModeProvider(Func<ProgramMode> provider)
        {
            ProgramModeProvider = provider ?? (() => ProgramMode.Manual);
            // 주입 직후 한 번 평가해 캐시에 반영
            _lastKnownMode = SafeResolveProgramMode();
        }

        // 외부(Overview)에서 보기 위한 공개 속성
        public bool IsConnected => _useSim || _isConnected;

        private int _scanInputIdx = -1;
        private int _scanOutputIdx = -1;

        private struct ScanRequest
        {
            public string Bank;      // "X100", "X101", ...
            public bool IsInput;     // true=X, false=Y
            public ushort Start;     // 시작 어드레스(코일/레지스터)
        }

        private readonly ConcurrentDictionary<ushort, ScanRequest> _pendingScans =
            new ConcurrentDictionary<ushort, ScanRequest>();

        private ushort _nextScanId = 100;
        private ushort NextScanId()
        {
            var id = _nextScanId;
            _nextScanId = (ushort)((_nextScanId + 1) % 60000);
            if (_nextScanId < 100) _nextScanId = 100;
            return id;
        }

        public IOForm()
        {
            InitializeComponent();
            _safetyAlarm = new MsgBoxAlarmSink(this);
            _safetyCtx = new IoSafetyContext(() => ResolveProgramMode());
            Initialize();
        }

        private void Initialize()
        {
            // 1) appsettings.json 로드값 사용
            var c = AppConfig.Current ?? new AppConfig();
            _useSim = c.WagoUseSimulator;
            _ip = string.IsNullOrWhiteSpace(c.WagoIp) ? "192.168.1.2" : c.WagoIp;
            _port = (c.WagoPort > 0 && c.WagoPort <= 65535) ? c.WagoPort : 502;
            _cfgPath = string.IsNullOrWhiteSpace(c.WagoCfgPath) ? @"D:\AppConfig\IO.cfg" : c.WagoCfgPath;

            // 2) cfg 파싱
            allIOList = CfgParser.Parse(_cfgPath);
            IoCatalog.BuildFrom(allIOList);

            // Status 리스트 구성
            statusList = allIOList.Select(io => io.StatusCode.Trim()).Distinct().ToList();
            inputStatusList = statusList.Where(s => s.StartsWith("X", StringComparison.OrdinalIgnoreCase)).ToList();
            outputStatusList = statusList.Where(s => s.StartsWith("Y", StringComparison.OrdinalIgnoreCase)).ToList();

            // 3) 시뮬레이터
            if (_useSim)
            {
                foreach (var s in inputStatusList) _simInputs[s] = new bool[16];
                foreach (var s in outputStatusList) _simOutputs[s] = new bool[16];
                pbConnectionStatus.BackColor = System.Drawing.Color.SteelBlue;
            }
            else
            {
                bool isConnected = ConnectModbusTcp(_ip, _port);
                if (!isConnected) { UpdateConnectionStatus(false); return; }
                UpdateConnectionStatus(true);
            }

            // 4) DGV 초기화 + 초기 데이터
            InitializeDataGridView(dgvInput);
            InitializeDataGridView(dgvOutput);
            UpdateUI();

            // 5) (삭제) ModbusDataMapper 초기화는 더 이상 사용하지 않음
            // ModbusDataMapper.Instance.InitializeStatusMappings();  // ← 제거

            // 6) 폴링 타이머
            _modbusReadTimer = new System.Windows.Forms.Timer();
            _modbusReadTimer.Interval = 300;
            _modbusReadTimer.Tick += (s2, ev2) =>
            {
                if (_useSim)
                {
                    ScanSimOneStep();
                }
                else
                {
                    if (inputStatusList.Count + outputStatusList.Count > 0)
                    {
                        if (_scanToggle && inputStatusList.Count > 0)
                        {
                            _scanInputIdx = (_scanInputIdx + 1) % inputStatusList.Count;
                            RequestReadInputBank(inputStatusList[_scanInputIdx]);
                        }
                        //else if (outputStatusList.Count > 0)    // Test KJW
                        //{
                        //    _scanOutputIdx = (_scanOutputIdx + 1) % outputStatusList.Count;
                        //    RequestReadOutputBank(outputStatusList[_scanOutputIdx]);
                        //}
                        _scanToggle = !_scanToggle;
                    }
                }

                var nowMode = SafeResolveProgramMode();
                if (_isForceOutputMode && _prevMode == ProgramMode.Manual && nowMode != ProgramMode.Manual)
                {
                    _isForceOutputMode = false;
                    UiSafeInvoke(() =>
                    {
                        btnForceOutputMode.BackColor = Color.LightGray;
                        MessageBox.Show("Manual 해제되어 Force Output Mode가 자동 종료되었습니다.",
                            "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                _lastKnownMode = nowMode;
                _prevMode = nowMode;
            };
            _modbusReadTimer.Start();
        }
        private void ScanSimOneStep()
        {
            // 입력(X)
            if (inputStatusList.Count > 0)
            {
                _scanInputIdx = (_scanInputIdx + 1) % inputStatusList.Count;
                string bank = inputStatusList[_scanInputIdx];

                if (_simInputs.TryGetValue(bank, out bool[] bits))
                {
                    IOData first = allIOList.Where(io => io.StatusCode == bank && io.IsInput)
                                            .OrderBy(io => io.SubAddressInt)
                                            .FirstOrDefault();
                    if (first != null)
                    {
                        byte b0 = 0;
                        byte b1 = 0;

                        for (int i = 0; i < 8; i++)
                        {
                            if (i < bits.Length && bits[i])
                                b0 |= (byte)(1 << i);
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            if ((8 + i) < bits.Length && bits[8 + i])
                                b1 |= (byte)(1 << i);
                        }

                        byte[] coilData = new byte[] { b0, b1 };
                        ProcessInputBankResponse(bank, (ushort)first.SubAddressInt, coilData);
                    }
                }
            }

            // 출력(Y) - 기존 Register read 흉내가 아니라 Coil read와 동일하게 처리
            if (outputStatusList.Count > 0)
            {
                _scanOutputIdx = (_scanOutputIdx + 1) % outputStatusList.Count;
                string bank = outputStatusList[_scanOutputIdx];

                if (_simOutputs.TryGetValue(bank, out bool[] bits))
                {
                    IOData first = allIOList.Where(io => io.StatusCode == bank && io.IsOutput)
                                            .OrderBy(io => io.SubAddressInt)
                                            .FirstOrDefault();
                    if (first != null)
                    {
                        byte b0 = 0;
                        byte b1 = 0;

                        for (int i = 0; i < 8; i++)
                        {
                            if (i < bits.Length && bits[i])
                                b0 |= (byte)(1 << i);
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            if ((8 + i) < bits.Length && bits[8 + i])
                                b1 |= (byte)(1 << i);
                        }

                        byte[] coilData = new byte[] { b0, b1 };
                        ProcessOutputBankResponse(bank, (ushort)first.SubAddressInt, coilData);
                    }
                }
            }
        }
        public void HintRefreshInputs(params string[] ioNames)
        {
            if (ioNames == null || ioNames.Length == 0) return;

            var banks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var nm in ioNames)
            {
                var io = allIOList?.Find(x => x.IsInput &&
                                  string.Equals(NormalizeName(x.Name), NormalizeName(nm), StringComparison.OrdinalIgnoreCase));
                if (io != null && !string.IsNullOrWhiteSpace(io.StatusCode))
                    banks.Add(io.StatusCode.Trim());
            }

            // 각 은행을 1회 즉시 요청 — _isReading 보호로 동시에 여러 건 쏘지는 않지만,
            // 사용자가 Unlock 버튼 누른 순간 ‘그 은행’은 바로 다음 응답에 포함될 확률이 확 올라갑니다.
            foreach (var bank in banks)
                RequestReadInputBank(bank);
        }
        private ProgramMode SafeResolveProgramMode()
        {
            // 1) 외부 Provider 최우선
            try
            {
                var provider = ProgramModeProvider;
                if (provider != null)
                {
                    var mode = provider(); // Func<ProgramMode>
                                           // ProgramMode가 enum이므로 통상 유효. 그래도 방어 로직 유지
                    if (Enum.IsDefined(typeof(ProgramMode), mode))
                        return mode;
                }
            }
            catch
            {
                // provider에서 예외가 나도 폼이 죽지 않도록 무시하고 폴백
            }

            // 2) IO 기반 추정
            var guessed = GuessModeFromIo();
            if (guessed.HasValue)
                return guessed.Value;

            // 3) Unknown이면 '마지막 확인값' 유지 (이게 핵심)
            return _lastKnownMode;
        }

        private ProgramMode? GuessModeFromIo()
        {
            var st = ReadModeKeyStatus();
            // 여기서도 Unknown은 null 반환해서 호출측에서 lastKnown 유지
            switch (st)
            {
                case DigitalStatus.Auto:
                    return ProgramMode.Auto;
                case DigitalStatus.Teach:
                    return ProgramMode.Manual; // Teach/Hand를 Manual로 묶는 정책 유지
                                               // 필요하다면 DigitalStatus에 따른 다른 매핑 추가
                case DigitalStatus.Unknown:
                default:
                    return null;
            }
        }

        // 기존 ResolveProgramMode를 아래처럼 교체
        private ProgramMode ResolveProgramMode()
        {
            var now = SafeResolveProgramMode();
            _lastKnownMode = now; // 캐시 갱신
            return now;
        }

        private static readonly string[] _modeKeyAliases = new[]
        {
            "Mode Key Status", "Mode Key", "Mode Select Status", "Mode Selector",
            "Mode Switch Status", "Mode Switch", "Mode Key Switch Status"
        };

        private DigitalStatus ReadModeKeyStatus()
        {
            // 1) 미리 정의한 alias 우선
            foreach (var name in _modeKeyAliases)
            {
                if (StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(name, out var v))
                    return v.ReadValue != DigitalStatus.Unknown ? v.ReadValue
                                                                : (v.RawBit ? DigitalStatus.Auto : DigitalStatus.Teach);
            }

            // 2) cfg 기반 휴리스틱 (이름에 'mode' 포함된 input)
            var modeLike = allIOList?.FirstOrDefault(io =>
                io.IsInput && io.Name?.IndexOf("mode", StringComparison.OrdinalIgnoreCase) >= 0);

            if (modeLike != null &&
                StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(modeLike.Name, out var vv))
            {
                return vv.ReadValue != DigitalStatus.Unknown ? vv.ReadValue
                                                             : (vv.RawBit ? DigitalStatus.Auto : DigitalStatus.Teach);
            }

            return DigitalStatus.Unknown;
        }
        private bool ConnectModbusTcp(string ipAddress, int port)
        {
            try
            {
                MBmaster = new Master(ipAddress, (ushort)port, true);

                MBmaster.OnException += new ModbusTCP.Master.ExceptionData(MBmaster_OnException);
                MBmaster.OnResponseData += OnModbusResponseData;

                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                MessageBox.Show($"Modbus 연결 중 오류 발생: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void TryReconnectModbus()
        {
            if (_useSim) return;          // 시뮬레이터면 무시
            if (_reconnecting) return;    // 중복 재접속 방지

            _reconnecting = true;
            try
            {
                bool ok = false;
                try { ok = ConnectModbusTcp(_ip, _port); }
                catch { ok = false; }

                UpdateConnectionStatus(ok);

                // 연결 성공 시 현재 선택된 Bank를 즉시 한 번 읽어와 UI 갱신(선택)
                if (ok)
                {
                    if (inputStatusList != null && inputStatusList.Count > 0)
                    {
                        int idx = currentInputStatusIndex;
                        if (idx < 0 || idx >= inputStatusList.Count) idx = 0;
                        RequestReadInputBank(inputStatusList[idx]);
                    }
                    if (outputStatusList != null && outputStatusList.Count > 0)
                    {
                        int idx = currentOutputStatusIndex;
                        if (idx < 0 || idx >= outputStatusList.Count) idx = 0;
                        RequestReadOutputBank(outputStatusList[idx]);
                    }
                }
            }
            finally
            {
                _reconnecting = false;
            }
        }

        private void MBmaster_OnResponseData(ushort ID, byte unit, byte function, byte[] values)
        {
            // ------------------------------------------------------------------
            // Seperate calling threads
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Master.ResponseData(MBmaster_OnResponseData), new object[] { ID, unit, function, values });
                return;
            }
        }
        private void MBmaster_OnException(ushort id, byte unit, byte function, byte exception)
        {
            string exc = "Modbus says error: ";
            switch (exception)
            {
                case Master.excIllegalFunction: exc += "Illegal function!"; break;
                case Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case Master.excAck: exc += "Acknoledge!"; break;
                case Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }

            // UI 스레드에서 메시지 표시 및 OK 후 재접속
            UiSafeInvoke(() =>
            {
                var dr = MessageBox.Show(this, exc, "Modbus slave exception",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (dr == DialogResult.OK)
                    TryReconnectModbus();
            });
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            _isConnected = isConnected;
            pbConnectionStatus.BackColor = isConnected ? Color.LimeGreen : Color.Gray;
        }

        private void pbConnectionStatus_Click(object sender, EventArgs e)
        {
            if (_useSim)
            {
                MessageBox.Show("WAGO 시뮬레이터 모드입니다. 실제 통신 연결은 수행하지 않습니다.",
                                "WAGO IO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool isConnected = ConnectModbusTcp(_ip, _port);
            MessageBox.Show(isConnected ? "ModbusTCP 연결 성공" : "ModbusTCP 연결 실패",
                            "Connection", MessageBoxButtons.OK, isConnected ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        private void InitializeDataGridView(DataGridView gv)
        {
            gv.AutoGenerateColumns = false;
            gv.Columns.Clear();

            // 숨김 내부용 nibble 인덱스
            var colIdx = new DataGridViewTextBoxColumn { Name = "IDX", HeaderText = "IDX", Width = 1, ReadOnly = true, Visible = false };

            var colName = new DataGridViewTextBoxColumn { Name = "NAME", HeaderText = "NAME", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true };
            var colAddr = new DataGridViewTextBoxColumn { Name = "ADDRESS", HeaderText = "ADDRESS", Width = 100, ReadOnly = true };
            var colStat = new DataGridViewTextBoxColumn { Name = "STATUS", HeaderText = "STATUS", Width = 120, ReadOnly = true };
            var colVal = new DataGridViewTextBoxColumn { Name = "VALUE", HeaderText = "VALUE", Width = 70, ReadOnly = true };

            gv.Columns.AddRange(new DataGridViewColumn[] { colIdx, colName, colAddr, colStat, colVal });

            gv.ReadOnly = true;
            gv.AllowUserToAddRows = false;
            gv.AllowUserToDeleteRows = false;
            gv.AllowUserToResizeColumns = false;
            gv.AllowUserToResizeRows = false;
            gv.MultiSelect = false;
            gv.RowHeadersVisible = false;
            gv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gv.RowTemplate.Height = 24;
            gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gv.BackgroundColor = Color.White;

            var headerCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("맑은 고딕", 9F, FontStyle.Bold)
            };
            gv.ColumnHeadersDefaultCellStyle = headerCenter;

            var cell = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            gv.DefaultCellStyle = cell;

            // 0~F 행 추가 (IDX는 숨김)
            for (int i = 0; i < 16; i++)
                gv.Rows.Add(i.ToString("X"), "", "", "", "");
        }


        // ==
        //   UI 업데이트 이벤트
        // ==
        private void UpdateUI()
        {
            UpdateInputList();
            UpdateOutputList();
        }

        private void UpdateInputList()
        {
            if (inputStatusList.Count == 0) return;

            string currentStatus = inputStatusList[currentInputStatusIndex];
            btnInputStatus.Text = currentStatus;

            var currentInputData = allIOList
                .Where(io => io.StatusCode == currentStatus && io.IsInput)
                .ToList();

            if (currentInputData.Count == 0) return;

            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                string idxHex = dgvInput.Rows[i].Cells["IDX"].Value as string ?? "0";
                var item = currentInputData.FirstOrDefault(io =>
                    io.SubAddressHex.EndsWith(idxHex, StringComparison.OrdinalIgnoreCase));

                dgvInput.Rows[i].Cells["NAME"].Value = item?.Name ?? "";
                dgvInput.Rows[i].Cells["ADDRESS"].Value = item != null ? BuildDisplayAddress(item) : "";
                dgvInput.Rows[i].Cells["STATUS"].Value = "";
                dgvInput.Rows[i].Cells["VALUE"].Value = "";
                dgvInput.Rows[i].Cells["VALUE"].Style.BackColor = Color.LightGray;
            }
        }

        private void UpdateOutputList()
        {
            if (outputStatusList.Count == 0) return;

            string currentStatus = outputStatusList[currentOutputStatusIndex];
            btnOutputStatus.Text = currentStatus;

            var currentOutputData = allIOList
                .Where(io => io.StatusCode == currentStatus && io.IsOutput)
                .ToList();

            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                dgvOutput.Rows[i].Cells["NAME"].Value = "";
                dgvOutput.Rows[i].Cells["ADDRESS"].Value = "";
                dgvOutput.Rows[i].Cells["STATUS"].Value = "";
                dgvOutput.Rows[i].Cells["VALUE"].Value = "";
                dgvOutput.Rows[i].Cells["VALUE"].Style.BackColor = Color.LightGray;
            }

            if (currentOutputData.Count == 0) return;

            for (int i = 0; i < dgvOutput.Rows.Count; i++)
            {
                string idxHex = dgvOutput.Rows[i].Cells["IDX"].Value as string ?? "0";
                var item = currentOutputData.FirstOrDefault(io =>
                    io.SubAddressHex.EndsWith(idxHex, StringComparison.OrdinalIgnoreCase));

                dgvOutput.Rows[i].Cells["NAME"].Value = item?.Name ?? "";
                dgvOutput.Rows[i].Cells["ADDRESS"].Value = item != null ? BuildDisplayAddress(item) : "";
            }
        }

        // ==
        //   Input 버튼 이벤트
        // ==
        private void btnInputPrevAll_Click(object sender, EventArgs e)
        {
            currentInputStatusIndex = 0;
            UpdateInputList();
            if (inputStatusList.Count > 0) RequestReadInputBank(inputStatusList[currentInputStatusIndex]);
        }

        private void btnInputPrev_Click(object sender, EventArgs e)
        {
            if (currentInputStatusIndex > 0)
            {
                currentInputStatusIndex--;
                UpdateInputList();
                RequestReadInputBank(inputStatusList[currentInputStatusIndex]);
            }
        }

        private void btnInputNext_Click(object sender, EventArgs e)
        {
            // Digital input 테스트
            //ushort address = 0;// 0, 16, 32, 48, 64
            //string hex = "3";
            //GetReadCoil(address, hex);

            if (currentInputStatusIndex < inputStatusList.Count - 1)
            {
                currentInputStatusIndex++;
                UpdateInputList();
                RequestReadInputBank(inputStatusList[currentInputStatusIndex]);
            }
        }

        private void btnInputNextAll_Click(object sender, EventArgs e)
        {
            currentInputStatusIndex = inputStatusList.Count - 1;
            UpdateInputList();
            if (inputStatusList.Count > 0) RequestReadInputBank(inputStatusList[currentInputStatusIndex]);
        }


        // ==
        //   OUTPUT 버튼 이벤트
        // ==
        private void btnOutputPrevAll_Click(object sender, EventArgs e)
        {
            currentOutputStatusIndex = 0;
            UpdateOutputList();
            if (outputStatusList.Count > 0) RequestReadOutputBank(outputStatusList[currentOutputStatusIndex]);
        }

        private void btnOutputPrev_Click(object sender, EventArgs e)
        {
            if (currentOutputStatusIndex > 0)
            {
                currentOutputStatusIndex--;
                UpdateOutputList();
                RequestReadOutputBank(outputStatusList[currentOutputStatusIndex]);
            }
        }

        private void btnOutputNext_Click(object sender, EventArgs e)
        {
            if (currentOutputStatusIndex < outputStatusList.Count - 1)
            {
                currentOutputStatusIndex++;
                UpdateOutputList();
                RequestReadOutputBank(outputStatusList[currentOutputStatusIndex]);
            }
        }

        private void btnOutputNextAll_Click(object sender, EventArgs e)
        {
            currentOutputStatusIndex = outputStatusList.Count - 1;
            UpdateOutputList();
            if (outputStatusList.Count > 0) RequestReadOutputBank(outputStatusList[currentOutputStatusIndex]);
        }


        // 
        //   Status(X,Y) 버튼 이벤트
        // 
        private void btnInputStatus_Click(object sender, EventArgs e)
        {
            // 'X'로 시작하는 Status 리스트를 필터링하여 전달
            var varinputStatusList = inputStatusList.Where(s => s.StartsWith("X")).ToList();
            ShowStatusSelectionDialog(varinputStatusList);
        }

        private void btnOutputStatus_Click(object sender, EventArgs e)
        {
            // 'Y'로 시작하는 Status 리스트를 필터링하여 전달
            var varoutputStatusList = outputStatusList.Where(s => s.StartsWith("Y")).ToList();
            ShowStatusSelectionDialog(varoutputStatusList);
        }

        private void ShowStatusSelectionDialog(List<string> filteredStatusList)
        {
            using (StatusSelectionForm statusForm = new StatusSelectionForm(filteredStatusList))
            {
                if (statusForm.ShowDialog() == DialogResult.OK)
                {
                    string selectedStatus = statusForm.SelectedStatus;

                    if (selectedStatus.StartsWith("X", StringComparison.OrdinalIgnoreCase))
                    {
                        currentInputStatusIndex = inputStatusList.IndexOf(selectedStatus);
                        UpdateInputList();
                        RequestReadInputBank(selectedStatus);
                    }
                    else if (selectedStatus.StartsWith("Y", StringComparison.OrdinalIgnoreCase))
                    {
                        currentOutputStatusIndex = outputStatusList.IndexOf(selectedStatus);
                        UpdateOutputList();
                        RequestReadOutputBank(selectedStatus);
                    }
                }
            }
        }

        private void btnForceOutputMode_Click(object sender, EventArgs e)
        {
            var modeNow = ResolveProgramMode();
            if (modeNow != ProgramMode.Manual)
            {
                MessageBox.Show("Manual Mode에서만 Direct IO 동작을 할 수 있습니다.", "경고",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isForceOutputMode = !_isForceOutputMode;
            btnForceOutputMode.BackColor = _isForceOutputMode ? Color.LimeGreen : Color.LightGray;
            MessageBox.Show(_isForceOutputMode ? "Force Output Mode가 활성화되었습니다." : "Force Output Mode가 비활성화되었습니다.",
                "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvOutput_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isForceOutputMode)
            {
                MessageBox.Show("Force Output Mode가 비활성화되어 동작할 수 없습니다.", "경고",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ResolveProgramMode() != ProgramMode.Manual)
            {
                MessageBox.Show("Manual Mode에서만 Direct IO 동작을 할 수 있습니다.", "경고",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _isForceOutputMode = false;
                btnForceOutputMode.BackColor = Color.LightGray;
                return;
            }

            if (e.RowIndex < 0 || e.ColumnIndex != dgvOutput.Columns["VALUE"].Index) return;

            var row = dgvOutput.Rows[e.RowIndex];
            string status = btnOutputStatus.Text;
            string dataName = row.Cells["NAME"].Value?.ToString();
            string idxHex = row.Cells["IDX"].Value?.ToString();
            if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(idxHex)) return;

            // 현재 셀 상태 기준 토글 결정
            bool desiredOn = !(row.Cells["VALUE"].Style.BackColor == Color.LimeGreen);

            // cfg에서 ioData 찾기 (← 먼저 찾고 이름/주소 기반으로 차단)
            var ioData = allIOList.FirstOrDefault(io => io.StatusCode == status &&
                                                        io.IsOutput &&
                                                        string.Equals(NormalizeName(io.Name), NormalizeName(dataName), StringComparison.OrdinalIgnoreCase));
            if (ioData == null) return;

            // === IOForm에서 금지된 출력이면 (이름/주소 기준) 바로 차단 ===
            if (IsIoFormWriteBlocked(ioData))
            {
                MessageBox.Show("이 출력은 IOForm에서 조작할 수 없습니다. Overview 화면에서 제어하세요.",
                    "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // source를 명시적으로 "IOForm" 으로 전달
            if (TryWriteOutputByName(ioData.Name, desiredOn, source: "IOForm"))
            {
                var eval = IoCatalog.Evaluate(ioData, desiredOn);
                row.Cells["ADDRESS"].Value = BuildDisplayAddress(ioData);
                row.Cells["STATUS"].Value = eval.Label;
                row.Cells["VALUE"].Style.BackColor = desiredOn ? Color.LimeGreen : Color.LightGray;
                row.Cells["VALUE"].Value = desiredOn ? "ON" : "OFF";
            }
            dgvOutput.ClearSelection();
        }



        // Set Read/Write 이벤트
        private bool SetWriteSingleCoil(string address_hex, int index, int value)
        {
            if (_useSim)
            {
                // address_hex의 마지막 nibble(0~F) → 행 인덱스로 사용
                if (outputStatusList.Count == 0) return true;
                string status = outputStatusList[currentOutputStatusIndex];
                if (!_simOutputs.TryGetValue(status, out var bits)) return true;

                // 주소의 끝 1자리(16진)로 행 찾기 (은행 내 비트)
                int rowIdx = 0;
                try { rowIdx = int.Parse(address_hex.Substring(address_hex.Length - 1), System.Globalization.NumberStyles.HexNumber); }
                catch { rowIdx = 0; }

                bits[rowIdx] = (value == 1);
                return true;
            }

            // --- 실기 기존 코드 ---
            lock (_modbusLock)
            {
                try
                {
                    ushort ID = 5; byte unit = 0;
                    ushort StartAddress = Convert.ToUInt16(int.Parse(address_hex, System.Globalization.NumberStyles.HexNumber));
                    data = GetDataBits(index, value);
                    MBmaster.WriteSingleCoils(ID, unit, StartAddress, Convert.ToBoolean(data[0]));
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Modbus Write 실패: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private byte[] GetDataBits(int index, int value)
        {
            bool[] bits = new bool[index]; // index : 16

            //0, 1
            bits[0] = Convert.ToBoolean(Convert.ToByte(value));

            // 비트 > 바이트 배열로 변환
            int numBytes = (index / 8 + (index % 8 > 0 ? 1 : 0));
            data = new Byte[numBytes];
            BitArray bitArray = new BitArray(bits);
            bitArray.CopyTo(data, 0);

            return data;
        }

        private void InternalReadCoilsWithId(ushort id, ushort address)
        {
            if (_useSim) return;
            if (_isReading) return;
            lock (_modbusLock)
            {
                _isReading = true;
                try
                {
                    byte unit = 0; UInt16 length = 16;
                    MBmaster.ReadCoils(id, unit, address, length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Modbus ReadCoils 실패: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { _isReading = false; }
            }
        }

        private void RequestReadInputBank(string bank)
        {
            var first = allIOList.Where(io => io.StatusCode == bank && io.IsInput)
                                 .OrderBy(io => io.SubAddressInt).FirstOrDefault();
            if (first == null) return;
            ushort start = (ushort)first.SubAddressInt;

            var id = NextScanId();
            var req = new ScanRequest { Bank = bank, IsInput = true, Start = start };
            _pendingScans[id] = req;
            _lastIssuedScan = req;
            InternalReadCoilsWithId(id, start);
        }

        private void RequestReadOutputBank(string bank)
        {
            IOData first = allIOList.Where(io => io.StatusCode == bank && io.IsOutput)
                                    .OrderBy(io => io.SubAddressInt)
                                    .FirstOrDefault();
            if (first == null)
                return;

            ushort start = (ushort)first.SubAddressInt;

            ushort id = NextScanId();

            ScanRequest req = new ScanRequest();
            req.Bank = bank;
            req.IsInput = false;
            req.Start = start;

            _pendingScans[id] = req;
            _lastIssuedScan = req;

            // 출력도 입력과 동일하게 Coil 영역을 읽는다.
            InternalReadCoilsWithId(id, start);
        }

        private void OnModbusResponseData(ushort id, byte unit, byte function, byte[] data)
        {
            UiSafeInvoke(() =>
            {
                if (_pendingScans.TryRemove(id, out ScanRequest req))
                {
                    if (function == fctReadCoil)
                    {
                        if (req.IsInput)
                            ProcessInputBankResponse(req.Bank, req.Start, data);
                        else
                            ProcessOutputBankResponse(req.Bank, req.Start, data);

                        _lastIssuedScan = null;
                        return;
                    }
                }
                // ID 매칭 실패 시 마지막 요청으로 보정
                if (_lastIssuedScan.HasValue)
                {
                    ScanRequest last = _lastIssuedScan.Value;

                    if (function == fctReadCoil)
                    {
                        if (last.IsInput)
                            ProcessInputBankResponse(last.Bank, last.Start, data);
                        else
                            ProcessOutputBankResponse(last.Bank, last.Start, data);
                        
                        _lastIssuedScan = null;
                        return;
                    }
                }
            });
        }

        private void ProcessInputBankResponse(string bank, ushort startAddress, byte[] data)
        {
            // bank의 16비트를 VirtualBus에 모두 반영, 화면에 해당 bank가 선택된 경우 그리드도 갱신
            string currentBank = btnInputStatus.Text;

            for (int i = 0; i < 16; i++)
            {
                // coil bit 추출
                bool bit = ((data[i / 8] >> (i % 8)) & 0x01) == 0x01;

                // cfg 매핑 (주소가 정확한 경우 SubAddressInt 바로 매칭)
                var rowIo = allIOList.FirstOrDefault(io =>
                    io.StatusCode == bank && io.IsInput && io.SubAddressInt == (startAddress + i));

                if (rowIo == null) continue;

                var eval = IoCatalog.Evaluate(rowIo, bit);
                StageWin.WagoIO.VirtualBus.DigitalInputs.Upsert(rowIo.Name, eval);

                // 화면에 이 bank가 선택되어 있으면 그리드 갱신
                if (string.Equals(bank, currentBank, StringComparison.OrdinalIgnoreCase))
                {
                    int nibble = rowIo.SubAddressInt & 0xF; // 0~15
                    if (nibble >= 0 && nibble < dgvInput.Rows.Count)
                    {
                        dgvInput.Rows[nibble].Cells["NAME"].Value = rowIo.Name;
                        dgvInput.Rows[nibble].Cells["ADDRESS"].Value = BuildDisplayAddress(rowIo);
                        dgvInput.Rows[nibble].Cells["STATUS"].Value = eval.Label;
                        dgvInput.Rows[nibble].Cells["VALUE"].Style.BackColor = bit ? Color.LimeGreen : Color.LightGray;
                        dgvInput.Rows[nibble].Cells["VALUE"].Value = bit ? "ON" : "OFF";
                    }
                }
            }
        }

        private void ProcessOutputBankResponse(string bank, ushort startAddress, byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            string currentBank = btnOutputStatus.Text;

            List<IOData> bankOutputs = allIOList.Where(io => io.StatusCode == bank && io.IsOutput)
                                                .OrderBy(io => io.SubAddressInt)
                                                .ToList();

            if (bankOutputs.Count == 0)
                return;

            foreach (IOData rowIo in bankOutputs)
            {
                int coilIndex = rowIo.SubAddressInt - startAddress;
                if (coilIndex < 0 || coilIndex >= 16)
                    continue;

                int byteIndex = coilIndex / 8;
                int bitIndex = coilIndex % 8;

                if (byteIndex >= data.Length)
                    continue;

                bool bit = ((data[byteIndex] >> bitIndex) & 0x01) == 0x01;

                IoEvaluatedState eval = IoCatalog.Evaluate(rowIo, bit);
                StageWin.WagoIO.VirtualBus.DigitalOutputs.Upsert(rowIo.Name, eval);

                if (string.Equals(bank, currentBank, StringComparison.OrdinalIgnoreCase))
                {
                    int nibble = rowIo.SubAddressInt & 0xF;

                    if (nibble >= 0 && nibble < dgvOutput.Rows.Count)
                    {
                        dgvOutput.Rows[nibble].Cells["NAME"].Value = rowIo.Name;
                        dgvOutput.Rows[nibble].Cells["ADDRESS"].Value = BuildDisplayAddress(rowIo);
                        dgvOutput.Rows[nibble].Cells["STATUS"].Value = eval.Label;
                        dgvOutput.Rows[nibble].Cells["VALUE"].Style.BackColor = bit ? Color.LimeGreen : Color.LightGray;
                        dgvOutput.Rows[nibble].Cells["VALUE"].Value = bit ? "ON" : "OFF";
                    }
                }
            }
        }

        private static string BuildDisplayAddress(IOData io)
        {
            if (io == null) return "";
            var baseCode = io.StatusCode ?? "";
            var sub = io.SubAddressHex ?? "";
            var lastNibble = (sub.Length > 0) ? sub[sub.Length - 1] : '0';
            return baseCode + char.ToUpperInvariant(lastNibble);
        }
        private bool UiSafeInvoke(Action action)
        {
            if (IsDisposed || Disposing) return false;
            if (!IsHandleCreated) return false;

            try
            {
                if (InvokeRequired) BeginInvoke(action);
                else action();
                return true;
            }
            catch (ObjectDisposedException) { return false; }
            catch (InvalidOperationException) { return false; }
        }
        private void TraceOutputWrite(string source, string ioName, bool on, string note = null)
        {
            string msg = string.Format(
                "[WAGO-DO] {0:HH:mm:ss.fff} SRC={1}, IO={2}, VAL={3}, NOTE={4}",
                DateTime.Now,
                source ?? "-",
                ioName ?? "-",
                on ? "ON" : "OFF",
                note ?? "-");

            System.Diagnostics.Debug.WriteLine(msg);
        }
        public bool TryWriteOutputByName(string ioName, bool on, string source = null)
        {
            if (string.IsNullOrWhiteSpace(ioName)) return false;

            var io = allIOList?.FirstOrDefault(x =>
                x.IsOutput &&
                string.Equals(NormalizeName(x.Name), NormalizeName(ioName), StringComparison.OrdinalIgnoreCase));

            if (io == null) return false;

            // IOForm 발신이면 차단 목록에 대해 즉시 반환
            if (IsIoFormWriteBlocked(io) &&
                (string.IsNullOrEmpty(source) || source.StartsWith("IOForm", StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("이 출력은 IOForm에서 조작할 수 없습니다. Overview 화면에서 제어하세요.",
                    "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // (이하 동일: 안전 인터락 + Write)
            if (IsMcOutputName(io.Name))
            {
                SafetyIntent intent =
                    (NameEqN(io.Name, SafetyPolicy.OUT_AJIN_MC2_T) || NameEqN(io.Name, SafetyPolicy.OUT_AJIN_MC2_Z))
                    ? SafetyIntent.ForAjinMcOutput(io.Name, on, source ?? "IOForm")
                    : SafetyIntent.ForAcsMcOutput(io.Name, on, source ?? "IOForm");

                if (!SafetyPolicy.Evaluate(_safetyCtx, intent, out var why, _safetyAlarm))
                    return false;
            }

            int index = io.Index <= 0 ? 16 : io.Index;
            int value = on ? 1 : 0;

            TraceOutputWrite(source, io.Name, on, "WRITE_REQUEST"); // Test KJW
            bool ok = SetWriteSingleCoil(io.SubAddressHex, index, value);
            TraceOutputWrite(source, io.Name, on, ok ? "WRITE_OK" : "WRITE_FAIL");
            if (ok)
            {
                var eval = IoCatalog.Evaluate(io, on);
                StageWin.WagoIO.VirtualBus.DigitalOutputs.Upsert(io.Name, eval);
            }
            return ok;
        }

        private void IOForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { _modbusReadTimer?.Stop(); } catch { }

            if (MBmaster != null)
            {
                try
                {
                    MBmaster.OnResponseData -= MBmaster_OnResponseData;
                    MBmaster.OnResponseData -= OnModbusResponseData;
                    MBmaster.OnException -= MBmaster_OnException;
                }
                catch { }

                // 라이브러리가 지원한다면 정리 호출 (하나만 선택)
                // try { MBmaster.Dispose(); } catch { }
                // try { MBmaster.Disconnect(); } catch { }
            }

            base.OnFormClosing(e);
        }

        // IOForm 화면에서 ‘사용자 조작 금지’ 대상으로 묶음
        private static readonly HashSet<string> _ioformBlockedOutputs
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            SafetyPolicy.OUT_ACS_MC1,      // ACS Controller 1'st MC On / Off
            SafetyPolicy.OUT_ACS_MC2_Y,    // ACS Controller 2'nd MC - Stage Y
            SafetyPolicy.OUT_ACS_MC2_X,    // ACS Controller 2'nd MC - Stage X
            SafetyPolicy.OUT_AJIN_MC2_T,   // Stage Theta  Motor Drive MC
            SafetyPolicy.OUT_AJIN_MC2_Z    // Maint Z (1,2) Motor Drive MC
        };

        // IOForm 화면에서 ‘사용자 조작 금지’ 대상으로 묶음 (정규화 사용)
        private static string NormalizeName(string s)
            => Regex.Replace((s ?? string.Empty), @"\s+", " ").Trim();

        private static readonly HashSet<string> _ioformBlockedOutputsNorm
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            NormalizeName(SafetyPolicy.OUT_ACS_MC1),
            NormalizeName(SafetyPolicy.OUT_ACS_MC2_Y),
            NormalizeName(SafetyPolicy.OUT_ACS_MC2_X),
            NormalizeName(SafetyPolicy.OUT_AJIN_MC2_T),
            NormalizeName(SafetyPolicy.OUT_AJIN_MC2_Z),
        };

        private static bool NameEqN(string a, string b)
            => string.Equals(NormalizeName(a), NormalizeName(b), StringComparison.OrdinalIgnoreCase);

        private static bool IsIoFormWriteBlocked(string name)
            => _ioformBlockedOutputsNorm.Contains(NormalizeName(name));

        // IOData 기반 차단(이름 불일치 대비: Y1000~Y1004 주소면 차단)
        private static bool IsIoFormWriteBlocked(IOData io)
        {
            if (io == null) return false;
            if (IsIoFormWriteBlocked(io.Name)) return true;

            // 보정: 대상 5개는 Y100 bank, nibble 0~4
            if (io.StatusCode?.Equals("Y100", StringComparison.OrdinalIgnoreCase) == true)
            {
                var hex = io.SubAddressHex ?? "";
                char last = hex.Length > 0 ? hex[hex.Length - 1] : '\0';
                if (last == '0' || last == '1' || last == '2' || last == '3' || last == '4')
                    return true;
            }
            return false;
        }

        // MC 출력 이름 판정도 정규화 사용
        private static bool IsMcOutputName(string name)
        {
            return
                NameEqN(name, SafetyPolicy.OUT_ACS_MC1) ||
                NameEqN(name, SafetyPolicy.OUT_ACS_MC2_Y) ||
                NameEqN(name, SafetyPolicy.OUT_ACS_MC2_X) ||
                NameEqN(name, SafetyPolicy.OUT_ACS_MC_ALLOK) ||
                NameEqN(name, SafetyPolicy.OUT_AJIN_MC2_T) ||
                NameEqN(name, SafetyPolicy.OUT_AJIN_MC2_Z);
        }

        // MC 인터락만 쓰므로 최소 구현
        // 기존 IoSafetyContext 대체
        private sealed class IoSafetyContext : ISafetyContext
        {
            private readonly Func<ProgramMode> _modeProvider;

            public IoSafetyContext(Func<ProgramMode> modeProvider)
            {
                _modeProvider = modeProvider ?? (() => ProgramMode.Manual);
            }

            public ProgramMode Mode => _modeProvider();
            public string CurrentProgram => Mode.ToString().ToUpperInvariant();

            public double GetXActualVelocity() => 0.0;
            public double GetYActualVelocity() => 0.0;
            public double GetMaintZActualVelocity() => 0.0;
            public double GetThetaActualVelocity() => 0.0;

            public bool GetInput(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(ioName, out var v) && v.RawBit;
            public bool GetOutput(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalOutputs.TryGet(ioName, out var v) && v.RawBit;

            public DigitalStatus GetInputStatus(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(ioName, out var v) ? v.ReadValue : DigitalStatus.Unknown;
            public DigitalStatus GetOutputStatus(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalOutputs.TryGet(ioName, out var v) ? v.ReadValue : DigitalStatus.Unknown;

            public string GetInputLabel(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet(ioName, out var v) ? v.Label : "N/A";
            public string GetOutputLabel(string ioName)
                => StageWin.WagoIO.VirtualBus.DigitalOutputs.TryGet(ioName, out var v) ? v.Label : "N/A";

            public bool IsLaserOn()
                => StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet("Laser ON / OFF Status", out var v) && v.RawBit;
        }


        private sealed class MsgBoxAlarmSink : IAlarmSink
        {
            private readonly IWin32Window _owner;
            public MsgBoxAlarmSink(IWin32Window owner) { _owner = owner; }
            public void Notify(string title, string message)
                => MessageBox.Show(_owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
