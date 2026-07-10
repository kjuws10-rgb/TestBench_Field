using ACS.SPiiPlusNET;
using Core.Config;
using Core.Logging;
using NetCommon;
using StageWin.Alarm;
using StageWin.Core.Recipe;
using StageWin.Driver.LDS;
using StageWin.Driver.Motion;
using StageWin.Driver.Network.Packets;
using StageWin.Etc;
using StageWin.Safety;
using StageWin.UI;
using StageWin.WagoIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using Axis = StageWin.Driver.Motion.Axis;
using Timer = System.Windows.Forms.Timer;
using System.Globalization;

namespace StageWin
{
    public partial class Form1 : Form, ILogger
    {
        private int PORT
        {
            get
            {
                var c = AppConfig.Current;
                int p = c?.StagePort ?? 0;
                return (p > 0 && p <= 65535) ? p : 5000;
            }
        }
        private TcpListener _listener;
        ConcurrentDictionary<TcpClient, RpcSession> _sessions = new ConcurrentDictionary<TcpClient, RpcSession>();
        ConcurrentDictionary<RpcSession, PacketRouter> _routers = new ConcurrentDictionary<RpcSession, PacketRouter>();
        private volatile bool _scanCommOk = false;
        private volatile bool _visionCommOk = false;
        private readonly Dictionary<int, double> _posReachedLoggedTarget = new Dictionary<int, double>();

        private void SetLinkState(Role r, bool ok)
        {
            if (r == Role.Scan)
            {
                _scanCommOk = ok;
                LedLabel.Set(ledScan, ok ? LedState.On : LedState.Off);
            }
            else if (r == Role.Vision)
            {
                _visionCommOk = ok;
                LedLabel.Set(ledVision, ok ? LedState.On : LedState.Off);
            }
        }
        private static Role ParseRoleString(string s)
        {
            if (string.Equals(s, "Scan", StringComparison.OrdinalIgnoreCase)) return Role.Scan;
            if (string.Equals(s, "Vision", StringComparison.OrdinalIgnoreCase)) return Role.Vision;
            return Role.Unknown;
        }
        private static string ToV4(IPAddress ip)
        {
            try { return ip.MapToIPv4().ToString(); } catch { return ip?.ToString() ?? ""; }
        }
        // 고정 IP 매핑으로 Role 찾기
        private Role ResolveRoleByRemoteIp(TcpClient cli)
        {
            try
            {
                var remoteIp = ((IPEndPoint)cli.Client.RemoteEndPoint).Address;
                var ipStr = ToV4(remoteIp);
                var map = AppConfig.Current?.FixedRoleMap ?? Array.Empty<AppConfig.FixedRoleEntry>();
                foreach (var e in map)
                {
                    if (e == null) continue;
                    var target = (e.Ip ?? "").Trim();
                    if (string.IsNullOrEmpty(target)) continue;
                    if (string.Equals(ipStr, target, StringComparison.OrdinalIgnoreCase)) return ParseRoleString(e.Role);
                }
            }
            catch { }
            return Role.Unknown;
        }
        private sealed class RecipeListItem
        {
            public string Name { get; set; }
            public RecipeDoc Doc { get; set; }
            public string Source { get; set; } // "Local" | "Scan"
            public override string ToString() => Name;
        }
        private RecipeListItem MakeItem(string name, bool fromScan)
        {
            RecipeDoc doc = null;
            try
            {
                var store = fromScan ? _scanStore : _localStore;
                string fn = store.GetFullPath(name);
                if (File.Exists(fn)) doc = store.Load(name);
            }
            catch { /* ignore */ }
            return new RecipeListItem { Name = name, Doc = doc, Source = fromScan ? "Scan" : "Local" };
        }
        private RecipeStore _localStore;
        private RecipeStore _scanStore;
        private volatile bool _skipOverwriteConfirmOnce = false;
        private string _dirLocal => AppConfig.Current?.LocalRecipeDirPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
        private string _dirScan => AppConfig.Current?.ScanRecipeDirPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes.Scan");
        private string _currentRecipeName = string.Empty;
        // 세션 찾기용
        private RpcSession Find(Role r)
        {
            foreach (var s in _sessions.Values) if (s.Role == r) return s;
            return null;
        }
        Timer _tmrUi = new Timer();   // 위치/상태 폴링(시뮬)
        Random _rnd = new Random();
        FileSystemWatcher _fswLocal, _fswScan;
        Timer _fswDebounceLocal = new Timer { Interval = 250 };
        Timer _fswDebounceScan = new Timer { Interval = 250 };
        private const int LOAD_DEBOUNCE_MS = 2000;
        private long _lastLocalLoadAt;
        private string _lastLocalLoadName;
        private long _lastScanLoadAt;
        private string _lastScanLoadName;
        private int _loadReentry = 0;
        private int _abortReentry = 0;
        BindingList<AlarmRow> _alarms = new BindingList<AlarmRow>();
        OverviewForm _overviewForm;
        RecipeForm _recipeForm;
        OpticOperationForm _opticForm;
        ToolingEditorForm _toolingForm;
        PowerMeterEditorForm _powerForm;
        IOForm _ioForm;
        MotionManualForm _motionForm;
        private TowerLampForm _towerForm;
        private bool _autoSeqRunning = false; // 기존 사용 그대로 유지
        IMotionController _acsMotion;
        AjinMotionAdapter _ajinMotion = new AjinMotionAdapter();
        AxisCsvLogger _axisCsvLogger;
        private IOptexLds _lds;
        private Driver.LDS.LdsPollingHub _ldsHub;
        private const int LdsPollMs = 100;
        private ESCFlatnessForm _escFlatnessForm;
        private StageOffsetForm _stageOffsetForm;
        private readonly object _manualInspectionLock = new object();
        private TaskCompletionSource<ST_MANUAL_INSPECTION_REQ> _manualInspectionTcs;
        private StageWin.UI.RecipeForm.Cell _manualInspectionCell;
        private volatile bool _manualInspectionArmed = false;
        private const int MANUAL_INSPECTION_TIMEOUT_MS = 30_000;
        private volatile bool _fvAbortByError = false;

        // 알람 관리
        private Alarm.AlarmManager _alarmMgr;
        private bool _changingModeProgrammatically = false;
        private ProgramMode _lastConfirmedMode = ProgramMode.Manual;

        //  추가: 시퀀스 취소 토큰 
        private CancellationTokenSource _procCts;
        private CancellationTokenSource _serverCts = new CancellationTokenSource();

        private readonly ConcurrentQueue<(ListBox lb, string text)> _logQueue = new ConcurrentQueue<(ListBox lb, string text)>();
        private readonly Timer _logFlushTimer = new Timer { Interval = 50 };
        private const int MAX_LOG_LINES_DEBUG = 5000;
        private const int MAX_LOG_LINES_RELEASE = 1000;
        private int MaxLogLines =>
        #if DEBUG
            MAX_LOG_LINES_DEBUG;
        #else
            MAX_LOG_LINES_RELEASE;
        #endif

        //  추가: Scan Status(1008) 결과 enum(값은 장비 정의에 맞게 필요 시 조정) 
        private enum ScanRunState { Idle = 0, Run = 1, Manual = 2, PM = 3 }

        private IMotionController BuildMotion()
        {
            var c = AppConfig.Current ?? new AppConfig();
            var driver = (c.MotionDriver ?? "Sim").Trim();
            var ip = string.IsNullOrWhiteSpace(c.AcsIp) ? "192.168.0.10" : c.AcsIp;
            var port = (c.AcsPort > 0 && c.AcsPort <= 65535) ? c.AcsPort : 701;

            AddLog(lstSystem, $"[Motion] Driver='{driver}', UseSimulator={c.AcsUseSimulator}");

            if (!c.AcsUseSimulator)
                return new AcsMotionAdapter(ip, port, s => AddLog(lstSystem, "[ACS] " + s), false);
            else
                return new AcsMotionAdapter(ip, port, s => AddLog(lstSystem, "[ACS] " + s), true);
        }
        private static Axis AxisFromScanIndex(int nAxisNo)
        {
            switch (nAxisNo)
            {
                case 0: return Axis.Y;
                case 4: return Axis.X; // case 2: return Axis.T; // T축이 있다면 필요 시 활성화
                default: throw new ArgumentOutOfRangeException("nAxisNo", "지원하지 않는 축 번호");
            }
        }
        private static void ShowOrActivate(Form owner, Form form)
        {
            if (form.Visible)
            {
                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.Activate();
            }
            else
            {
                form.Show(owner);
                form.Activate();
            }
        }
        private IOptexLds BuildLds()
        {
            var cfg = AppConfig.Current ?? new AppConfig();
            string ip = string.IsNullOrWhiteSpace(cfg?.LdsIp) ? "192.168.0.30" : cfg.LdsIp;
            int port = (cfg?.LdsPort > 0 && cfg.LdsPort <= 65535) ? cfg.LdsPort : 49300;
            bool sim = cfg?.LdsUseSimulator ?? false;
            var lds = new OptexCdxEthernet(ip, port, useSimulator: sim)
            {
                Protocol = OptexCdxEthernet.LdsProtocol.OptexBinary,
            };
            lds.QueryCommand = string.IsNullOrWhiteSpace(cfg?.LdsQueryCommand) ? null : cfg.LdsQueryCommand;
            lds.NewLine = string.IsNullOrWhiteSpace(cfg?.LdsNewLine) ? "\n" : cfg.LdsNewLine;

            try
            {
                if (lds.Connect(1500))
                    AddLog(lstSystem, $"[LDS] Connected {ip}:{port} (Sim={sim})");
                else
                    AddLog(lstSystem, $"[LDS][W] Connect failed {ip}:{port} (Sim={sim})");
            }
            catch (Exception ex)
            {
                AddLog(lstSystem, "[E][LDS Connect] " + ex.Message);
            }
            _ldsHub = new Driver.LDS.LdsPollingHub(lds, LdsPollMs, s => AddLog(lstSystem, s));
            _ldsHub.Start();
            return lds;
        }
        private bool ShouldDebounceLoad(bool fromScan, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            long now = Environment.TickCount;
            if (fromScan)
            {
                if (string.Equals(_lastScanLoadName, name, StringComparison.OrdinalIgnoreCase) &&
                    (now - _lastScanLoadAt) <= LOAD_DEBOUNCE_MS)
                    return true;
                _lastScanLoadName = name; _lastScanLoadAt = now;
                return false;
            }
            else
            {
                if (string.Equals(_lastLocalLoadName, name, StringComparison.OrdinalIgnoreCase) &&
                    (now - _lastLocalLoadAt) <= LOAD_DEBOUNCE_MS)
                    return true;
                _lastLocalLoadName = name; _lastLocalLoadAt = now;
                return false;
            }
        }
        private readonly AutoSequenceRunner _autoSeqRunner;
        private AutoProcessForm _autoForm;
        private ParameterSettingDocument _paramSetting;
        private DateTime _paramSettingLoadedAt = DateTime.MinValue;
        private string ParameterSettingJsonPath => Path.Combine(AppConfig.ConfigRoot, "ParameterSetting.json");
        public Form1()
        {
            InitializeComponent();
            InitSequenceBanner();
            // 최대크기 창모드
            this.WindowState = FormWindowState.Maximized;
            this.FormClosed += OnFormClosedAsync;
            //실시간 AxisLogger
            this.Shown += (s, e) =>
            {
                try
                {
                    if (_axisCsvLogger != null) return;
                    _axisCsvLogger = new AxisCsvLogger();
                    _axisCsvLogger.Start(new AxisCsvLogger.Options
                    {
                        SampleProvider = () =>
                        {
                            var ts = DateTime.Now;

                            double zLoad = 0.000;
                            double tLoad = 0.000;

                            double xPos = 0.0, yPos = 0.0, zPos = 0.0, tPos = 0.0;
                            double xAcc = 0.0, yAcc = 0.0;
                            bool XServoOn = false, XHome = false, YServoOn = false, YHome = false;
                            double xRms = 0.0, yRms = 0.0, zRms = 0.0, tRms = 0.0;
                            double xVel = 0.0, yVel = 0.0, zCmdVel = 0.0, tCmdVel = 0.0;
                            bool ZServoOn = false, ZHome = false, TServoOn = false, THome = false;
                            //X,Y Data
                            try { xPos = _acsMotion?.GetAPosition(Axis.X) ?? 0.0; } catch { }
                            try { yPos = _acsMotion?.GetAPosition(Axis.Y) ?? 0.0; } catch { }

                            try { xVel = _acsMotion?.GetVelocity(Axis.X) ?? 0.0; } catch { }
                            try { yVel = _acsMotion?.GetVelocity(Axis.Y) ?? 0.0; } catch { }

                            try { xAcc = _acsMotion?.GetAcceleration(Axis.X) ?? 0.0; } catch { }
                            try { yAcc = _acsMotion?.GetAcceleration(Axis.Y) ?? 0.0; } catch { }

                            XServoOn = (_acsMotion as StageWin.Driver.Motion.IAcsStatus)?.IsServoOn(Axis.X) ?? false;
                            XHome = (_acsMotion as StageWin.Driver.Motion.IAcsStatus)?.IsHomeDone(Axis.X) ?? false;

                            YServoOn = (_acsMotion as StageWin.Driver.Motion.IAcsStatus)?.IsServoOn(Axis.Y) ?? false;
                            YHome = (_acsMotion as StageWin.Driver.Motion.IAcsStatus)?.IsHomeDone(Axis.Y) ?? false;

                            //RMS Data 
                            try
                            {
                                var acsStatus = _acsMotion as StageWin.Driver.Motion.IAcsStatus;
                                xRms = acsStatus?.GetRms(Axis.X) ?? 0.0;
                                yRms = acsStatus?.GetRms(Axis.Y) ?? 0.0;
                                zRms = CAXM.AxmStatusReadServoLoadRatio(0, ref zLoad);
                                tRms = CAXM.AxmStatusReadServoLoadRatio(1, ref tLoad);
                            }
                            catch { }

                            try { zPos = _ajinMotion?.GetActPos(AjinAxis.MaintZ1) ?? 0.0; } catch { }
                            try { tPos = _ajinMotion?.GetActPos(AjinAxis.T) ?? 0.0; } catch { }
                            try { zCmdVel = _ajinMotion?.GetCmdVel(AjinAxis.MaintZ1) ?? 0.0; } catch { }
                            try { tCmdVel = _ajinMotion?.GetCmdVel(AjinAxis.T) ?? 0.0; } catch { }

                            try
                            {
                                if (_ajinMotion?.IsConnected == true)
                                {
                                    ZServoOn = _ajinMotion.IsServoOn(AjinAxis.MaintZ1);
                                    ZHome = _ajinMotion.IsHomed(AjinAxis.MaintZ1);

                                    TServoOn = _ajinMotion.IsServoOn(AjinAxis.T);
                                    THome = _ajinMotion.IsHomed(AjinAxis.T);
                                }
                            }
                            catch { }

                            return new AxisCsvLogger.AxisRawData
                            {
                                Timestamp = ts,
                                XPos = xPos,
                                XVel = xVel,
                                XAcc = xAcc,
                                XRms = xRms,
                                XServoOn = XServoOn,
                                XHome = XHome,
                                YPos = yPos,
                                YVel = yVel,
                                YAcc = yAcc,
                                YRms = yRms,
                                YServoOn = YServoOn,
                                YHome = YHome,
                                ZPos = zPos,
                                ZCmdVel = zCmdVel,
                                ZServoOn = ZServoOn,
                                ZHome = ZHome,
                                ZRms = zRms,
                                TPos = tPos,
                                TCmdVel = tCmdVel,
                                TServoOn = TServoOn,
                                THome = THome,
                                TRms = tRms,
                            };
                        }
                    });
                }
                catch { }
            };

            if (ledServer != null) LedLabel.Init(ledServer); if (ledScan != null) LedLabel.Init(ledScan); if (ledVision != null) LedLabel.Init(ledVision);
            
            // 모드 기본
            rbtnModeManual.Checked = true;
            if (ledServer != null) LedLabel.Set(ledServer, LedState.On);

            // 로그/알람 바인딩
            _logFlushTimer.Tick += LogFlushTimer_Tick;
            _logFlushTimer.Start();

            if (gridAlarms != null)
            {
                SetupGridCommon(gridAlarms, defaultRowSelect: true);
                
                // 알람 컬럼 구성 및 포맷(시간 ms 포함)
                SetupAlarmGridColumns(gridAlarms);
                
                // 나머지 속성
                gridAlarms.ReadOnly = true;
                gridAlarms.AllowUserToAddRows = false;
                gridAlarms.AllowUserToDeleteRows = false;
                
                // 데이터 바인딩
                gridAlarms.DataSource = _alarms;
                
                // 새 행 추가 시 자동 스크롤
                gridAlarms.RowPrePaint += (s, e) =>
                {
                    if (e.RowIndex < 0) return;
                    var row = gridAlarms.Rows[e.RowIndex];
                    if (row?.DataBoundItem is StageWin.Alarm.AlarmRow ar)
                    {
                        bool isCleared = (ar.Message ?? "").StartsWith("CLEARED", StringComparison.OrdinalIgnoreCase);
                        if (!isCleared && string.Equals(ar.Level, "Alarm", StringComparison.OrdinalIgnoreCase))
                        {
                            row.DefaultCellStyle.BackColor = Color.MistyRose;        // 밝은 빨강 음영
                            row.DefaultCellStyle.SelectionBackColor = Color.IndianRed;
                            row.DefaultCellStyle.SelectionForeColor = Color.White;
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                            row.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                            row.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                        }
                    }
                };
                EnableDoubleBuffer(this);
                EnableDoubleBuffer(gridAlarms);
            }
            // 모션 드라이버 생성/연결
            _acsMotion = BuildMotion();
            _acsMotion = new StageWin.Driver.Motion.SafeMotionController(
            _acsMotion, alarm: new StageWin.UI.OverviewForm.MessageBoxAlarmSink(this)   // UI 팝업
            // ctxProvider: null이면 라이트 컨텍스트 사용(OUT_MC_ALLOK 기준)
            );
            try { _acsMotion.Connect(); AddLog(lstSystem, $"ACS Motion connected: {AppConfig.Current.MotionDriver}"); }
            catch (Exception ex) { AddLog(lstSystem, "[E][ACS Motion] " + ex.Message); }
            try { _ajinMotion.Connect(); AddLog(lstSystem, $"Ajin Motion connected"); }
            catch (Exception ex) { AddLog(lstSystem, "[E][Ajin Motion] " + ex.Message); }
            // LDS 연결
            _lds = BuildLds();
            // 서버 시작
            StartServer();
            // 저장소 준비
            _localStore = new RecipeStore(_dirLocal);
            _scanStore = new RecipeStore(_dirScan);
            EnsureAutoProcessForm();
            // 레시피 폼 임베딩
            CreateAndEmbedInnerForms();
            // Log Box 색변경
            EnableLogBoxColor(lstRecipeScanLog);
            EnableLogBoxColor(lstLog);
            EnableLogBoxColor(lstSystem);
            
            // 파일 변경 감시 시작
            SetupRecipeWatchers();
            // Scan I/F 버튼 이벤트 연결
            btnReqRecipeList.Click += btnReqRecipeList_Click;
            btnReqRecipeAdd.Click += btnReqRecipeAdd_Click;
            // Vision I/F 버튼 이벤트 연결
            btnReqAlign.Click += btnReqAlign_Click;
            btnReq2ndAlign.Click += btnReq2ndAlign_Click;
            // 기타 기능 이벤트 연결
            btnDownloadScanToLocal.Click += btnDownloadScanToLocal_Click;
            btnRefreshLocalList.Click += btnRefreshLocalList_Click;
            btnAutoParameters.Click += (s, e) => { using (var f = new StageWin.UI.ParameterSettingForm()) f.ShowDialog(this); };
            // 좌측 리스트 및 파일 감시 등
            WireRecipeListEvents();

            // Auto 시퀀스 연결
            _autoSeqRunner = new AutoSequenceRunner(this);
            EnsureAutoProcessForm();
        }

        #region ======= 초기화 및 강제종료 관련 함수 ======= 
        private void CreateAndEmbedInnerForms()
        {
            //  RecipeForm 생성/임베드 (Recipe Edit 탭) 
            _recipeForm = new RecipeForm();
            _recipeForm.RequestOpenToolingEditor += (doc) =>
            {
                try
                {
                    if (_toolingForm != null && !_toolingForm.IsDisposed)
                    {
                        _toolingForm.LoadFrom(doc);
                        ShowOrActivate(this, _toolingForm);
                        return;
                    }
                    _toolingForm = new ToolingEditorForm();
                    _toolingForm.StartPosition = FormStartPosition.Manual;
                    _toolingForm.Location = new Point(this.Right - 40 - _toolingForm.Width, this.Top + 120);
                    _toolingForm.RequestApply += () =>
                    {
                        var curDoc = _recipeForm?.CurrentDoc;
                        if (curDoc == null) return;
                        _toolingForm.SaveInto(curDoc);
                        _recipeForm?.RefreshUiFromDoc();
                        if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Scan)
                        {
                            _skipOverwriteConfirmOnce = true;
                            _recipeForm.ExternalCommit();
                        }
                        else
                        {
                            _recipeForm.ExternalSave(silent: true);
                        }
                    };
                    _toolingForm.FormClosing += (s, e) =>
                    {
                        if (e.CloseReason == CloseReason.UserClosing)
                        {
                            e.Cancel = true;
                            _toolingForm.Hide();
                        }
                    };
                    _toolingForm.LoadFrom(doc);
                    ShowOrActivate(this, _toolingForm);
                }
                catch (Exception ex)
                {
                    AddLog(lstRecipeScanLog, "[E][ToolingEditor Open] " + ex.Message);
                    MessageBox.Show(this, "Tooling Editor 열기 실패: " + ex.Message, "Tooling",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _recipeForm.RequestOpenPowerMeterEditor += (doc) =>
            {
                try
                {
                    if (_powerForm != null && !_powerForm.IsDisposed)
                    {
                        ShowOrActivate(this, _powerForm); return;
                    }
                    _powerForm = new PowerMeterEditorForm(_recipeForm.CurrentDoc);
                    _powerForm.StartPosition = FormStartPosition.Manual;
                    _powerForm.Location = new System.Drawing.Point(this.Left + 40, this.Top + 120);
                    _powerForm.RequestSetLaserParams += (power, atten) =>
                    {
                        if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Scan) _recipeForm.ExternalCommit();
                        else _recipeForm.ExternalSave(silent: true);
                    };
                    _powerForm.RequestMoveToPowerStartAsync = async (x, y, ct) =>
                    {
                        await _acsMotion.ServoOnAsync(Axis.X);
                        await _acsMotion.ServoOnAsync(Axis.Y);
                        var t1 = MoveAbsAndWait_SeqAsync(Axis.X, x, X_INPOS_TOL, 20000, ct);
                        var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, y, Y_INPOS_TOL, 20000, ct);
                        await Task.WhenAll(t1, t2);
                    };
                    _powerForm.RequestMeas1Async = async (tables) =>
                    {
                        var scanNo = _powerForm.GetScanNoOr(_opticForm?.CurrentScanNo ?? 1);
                        var range = _powerForm.GetRangePercentOr(0);
                        return await DoPowerMeterMeas1_1010_Async(
                            scanNo: scanNo,
                            rangePercent: range,
                            tables: tables,
                            timeoutMs: 1_200_000,
                            ct: CancellationToken.None);
                    };
                    _powerForm.RequestMeas2Async = async (scanNo, range, freq, att) =>
                    {
                        return await DoPowerMeterMeas2_1011_Async(
                            scanNo: scanNo,
                            rangePercent: range,
                            freq: freq,
                            att: att,
                            timeoutMs: 120_000,
                            ct: CancellationToken.None);
                    };
                    _powerForm.RequestLog = s => AddLog(lstRecipeScanLog, s);
                    _powerForm.RequestAbortScanAsync = async (scanNo) =>
                    {
                        var scan = GetScanSessionOrWarn();
                        if (scan == null) return;
                        AddLog(lstRecipeScanLog, $"[PM][ABORT] ScanStop(1006) → scanNo={scanNo}");
                        await ScanRpc.Request_ScanStopAsync(scan, scanNo, timeoutMs: 3000);
                    };
                    _powerForm.FormClosing += (s, e) =>
                    {
                        if (e.CloseReason == CloseReason.UserClosing)
                        {
                            e.Cancel = true;
                            _powerForm.Hide();
                        }
                    };
                    _powerForm.SetInitial(20, 100);
                    ShowOrActivate(this, _powerForm);
                }
                catch (Exception ex)
                {
                    AddLog(lstRecipeScanLog, "[E][PowerMeterEditor Open] " + ex.Message);
                    MessageBox.Show(this, "Power Meter Editor 열기 실패: " + ex.Message, "PowerMeter",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            // 현재 스테이지 위치: X=Review, Y=Main
            _recipeForm.GetAxesPos = () => new RecipeForm.XY
            {
                X = _acsMotion.GetAPosition(Axis.X), // Review X
                Y = _acsMotion.GetAPosition(Axis.Y)  // Main   Y
            };

            // Ajin 보드 Z / Theta 실시간 위치 표시용
            _recipeForm.GetZPos = () => _ajinMotion.GetActPos(AjinAxis.MaintZ1);
            _recipeForm.GetThetaPos = () => _ajinMotion.GetActPos(AjinAxis.T);

            // ABS 이동 실행
            _recipeForm.MoveAxes = (reviewX, mainY) => { _ = MoveAxesAbsAsync(reviewX, mainY); };
            // CreateAndEmbedInnerForms() 안, 기존 _recipeForm.MoveAxes 할당 바로 아래 추가
            _recipeForm.MoveAxesAsync = (reviewX, mainY) => MoveAxesAbsAsync(reviewX, mainY);
            // 부분 라인(선택 Col→마지막 Col) 러너 바인딩
            _recipeForm.RequestRunFlyingVisionPartial += async plan =>
            {
                var doc = _recipeForm?.CurrentDoc;
                var rf = doc?.Parameters?.ReviewFlying;
                bool yMajor = rf?.MajorIsY ?? true;
                bool forward = rf?.Forward ?? true; // 부분 라인은 snake 토글 영향 최소
                await RunFlyingVisionPartialApplyAsync(plan, yMajor, forward);
            };
            // 정착 여부 점검
            _recipeForm.IsAxesSettledAtAsync = (tx, ty) => AreAxesSettledAsync(tx, ty);
            _recipeForm.MeasureAtExAsync = async (tx, ty, nLine, globalR, globalC) =>
            {
                var vision = GetVisionSessionOrWarn();
                if (vision == null) throw new InvalidOperationException("Vision 연결되어 있지 않습니다.");

                AddLog(lstRecipeScanLog, $"[3002] Single Mark Find 요청... nLine={nLine}, gR={globalR}, gC={globalC}");
                var rsp = await VisionRpc.Request_MarkFindSingleAsync(
                    vision,
                    nLine,
                    globalR,
                    globalC,
                    timeoutMs: 15000);

                AddLog(lstRecipeScanLog, $"[3002] MarkFind" +
                            $"Find={rsp.nResult} " +
                            $"Tgt=({rsp.dTargetX:F3},{rsp.dTargetY:F3}) " +
                            $"Hole=({rsp.dMarkX:F3},{rsp.dMarkY:F3}) ");

                return new RecipeForm.MeasRaw
                {
                    FindResult = rsp.nResult,
                    TargetX = rsp.dTargetX,
                    TargetY = rsp.dTargetY,
                    HoleX = rsp.dMarkX,
                    HoleY = rsp.dMarkY
                };
            };

            // 폼에 저장소 전달
            _recipeForm.SetStores(_localStore, _scanStore);
            _recipeForm.RecipeSaved += name =>
            {
                // 저장된 출처에 따라 해당 목록만 갱신
                if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Local)
                {
                    RefreshLocalRecipeListKeepingSelection();
                    SelectByName(lstRecipeList, name);
                }
                else // Scan
                {
                    // 스캔 목록은 이름 리스트가 원격이므로 리스트는 그대로,
                    // 현재 선택 아이템 Doc 포인터만 갱신
                    var it = lstScanRecipeList.SelectedItem as RecipeListItem;
                    if (it != null && string.Equals(it.Name, name, StringComparison.OrdinalIgnoreCase)) it.Doc = _recipeForm.CurrentDoc;
                }
                bool fromScan = (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Scan);
                UpdateCurrentRecipeUi(name, fromScan);
            };
            _recipeForm.RecipeDeleted += removed =>
            {
                // 출처에 따라 삭제 반영
                if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Local) RefreshLocalRecipeListKeepingSelection();
            };
            _recipeForm.RecipeCommitted += OnRecipeCommitted;
            _recipeForm.RecipeDeleteRequested += OnRecipeDeleteRequestedFromScan;
            _recipeForm.RequestRunFlyingVisionPartial += p => RunFlyingVision_ByPlansAsync(new[] { p });
            _recipeForm.RequestRunFlyingVisionPlans += plans => RunFlyingVision_ByPlansAsync(plans);
            _recipeForm.WaitManualInspectionAsync = WaitManualInspectionFromVisionAsync;
            _recipeForm.RequestReselectCurrentRecipe += (origin, recipeName) =>
            {
                string originText = (origin == RecipeForm.RecipeOpenOrigin.Scan) ? "Scan" : "Local";
                _opticForm.SetRecipeContext(recipeName, originText);
                _opticForm.RefreshProcessGrid();
            };
            _recipeForm.GetScanSessionOrWarn = GetScanSessionOrWarn;
            _recipeForm.GetVisionSessionOrWarn = GetVisionSessionOrWarn;

            // 탭에 임베드
            _recipeForm.TopLevel = false;
            _recipeForm.FormBorderStyle = FormBorderStyle.None;
            _recipeForm.Dock = DockStyle.Fill;
            tabRecipeEdit.Controls.Clear();
            tabRecipeEdit.Controls.Add(_recipeForm);
            _recipeForm.Show();

            //  OpticOperationForm 생성/임베드 (Optic Control 탭) 
            _opticForm = new OpticOperationForm();
            _opticForm.TopLevel = false;
            _opticForm.FormBorderStyle = FormBorderStyle.None;
            _opticForm.Dock = DockStyle.Fill;
            tabOpticControl.Controls.Clear();
            tabOpticControl.Controls.Add(_opticForm);
            _opticForm.Show();

            // 레이아웃 정보 제공(라인/홀수)
            _opticForm.ProvideLayout = () =>
            {
                var d = _recipeForm.CurrentDoc;
                return (lines: Math.Max(1, d.Parameters.Lines),
                        holesPerLine: Math.Max(1, d.Parameters.HolesPerLine));
            };
            _opticForm.ProvideVectorLayout = () =>
            {
                var d = _recipeForm.CurrentDoc;
                return (VecRows: Math.Max(1, d.Parameters.VectorRows),
                        VecCols: Math.Max(1, d.Parameters.VectorCols));
            };
            _opticForm.ProvideScanGeometry = () =>
            {
                var d = _recipeForm.CurrentDoc;
                return (
                    AlignX: d.Offset.ReviewOffsetX,
                    AlignY: d.Offset.ReviewOffsetY,
                    d.Parameters.FirstHoleX,
                    d.Parameters.FirstHoleY, 
                    d.Parameters.PitchX,
                    d.Parameters.PitchY,
                    ScanOffsetX: d.Offset.ScanToReviewOffsetX,
                    ScanOffsetY: d.Offset.ScanToReviewOffsetY
                );
            };
            // 좌표/툴 리스트 제공(이미 있던 콜백 유지)
            _opticForm.ProvideDrawList = () => _recipeForm.BuildDrawListForScan();
            _opticForm.ProvideScanOrder = () => _recipeForm.GetScanOrderSequence();
            _opticForm.RequestRunProcess += async plan =>
            {
                var doc = _recipeForm?.CurrentDoc;
                var sf = doc?.Parameters?.ScanFlying;
                if (sf != null && doc != null)
                {
                    sf.MajorIsY = plan.IsYMajor;
                    sf.Forward = (plan.Direction == OpticOperationForm.TravelDirection.Forward);
                    sf.Serpentine = (plan.Pattern == OpticOperationForm.FlyPattern.Snake);

                    var (lines, holes) = _opticForm.ProvideLayout();
                    sf.FlyDirY = +1; sf.FlyDirX = +1;
                    if (sf.MajorIsY) sf.FlyDirY = sf.Forward ? +1 : -1;
                    else sf.FlyDirX = sf.Forward ? +1 : -1;

                    // 시작측: Forward면 반대 끝에서 시작(사용자 예시와 동일)
                    if (sf.MajorIsY) sf.StartCol = sf.Forward ? holes : 1;
                    else sf.StartRow = sf.Forward ? lines : 1;

                    // PGT Manual 모드일 때만 Process Scan 속도 적용
                    if (_lastConfirmedMode == ProgramMode.Manual)
                    {
                        sf.FlySpeedX = _opticForm.ProcessScanXSpeedValue;
                        sf.FlySpeedY = _opticForm.ProcessScanYSpeedValue;

                        // Stage 구동용 계획 속도에도 동일 값 반영
                        plan.ProcessSpeedX = sf.FlySpeedX;
                        plan.ProcessSpeedY = sf.FlySpeedY;
                    }

                    var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
                    rf.MajorIsY = sf.MajorIsY;
                    rf.Forward = sf.Forward;
                    rf.Serpentine = sf.Serpentine;
                    rf.FlyDirX = sf.FlyDirX;
                    rf.FlyDirY = sf.FlyDirY;
                    rf.StartRow = sf.StartRow;
                    rf.StartCol = sf.StartCol;
                    rf.UseFlyCompY = sf.UseFlyCompY;
                    rf.PerHoleTimeSec = sf.PerHoleTimeSec;
                    rf.PerHoleTimeSecX = sf.PerHoleTimeSecX;
                    rf.FlySpeedX = sf.FlySpeedX;
                    rf.FlySpeedY = sf.FlySpeedY;
                    // 좌표 재생성(엔코더 도메인)
                    _recipeForm.RebuildFromParameters();
                    // 장비 송신 리스트도 최신으로
                    plan.All = _recipeForm.BuildDrawListForScan();
                }
                await RunProcessSequenceAsync(plan);
            };
            _opticForm.ProvideRecipeDoc = () => _recipeForm.CurrentDoc;
            _opticForm.RequestStopProcess += async () =>
            {
                try
                {
                    await SendScanStopAsync(_opticForm.CurrentScanNo);
                }
                catch { /* ignore */ }
                _procCts?.Cancel();
            };

            //  OpticOperationForm 이벤트 바인딩 
            _opticForm.RequestLaserPowerSet += (power, atten) =>
            {
                if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Scan) _recipeForm.ExternalCommit();
                else _recipeForm.ExternalSave(silent: true); // 메시지 없이 저장
            };
            _opticForm.RequestLaserControl += async p => await SendLaserControlAsync(p);
            _opticForm.RequestProcessScanning += async p => await SendProcessScanningAsync(p);
            _opticForm.GetAxesPos = () => new RecipeForm.XY
            {
                X = _acsMotion.GetAPosition(Axis.X), // Review X
                Y = _acsMotion.GetAPosition(Axis.Y)  // Main   Y
            };
            // Ajin 보드 Z / Theta 실시간 위치 표시용
            _opticForm.GetZPos = () => _ajinMotion.GetActPos(AjinAxis.MaintZ1);
            _opticForm.GetThetaPos = () => _ajinMotion.GetActPos(AjinAxis.T);

            _opticForm.GetAxesVel = () => new RecipeForm.XY
            {
                X = _acsMotion.GetVelocity(Axis.X), // Review X
                Y = _acsMotion.GetVelocity(Axis.Y)  // Main   Y
            };

            // ABS 이동 실행
            _opticForm.MoveAxes = (reviewX, mainY) => { _ = MoveAxesAbsAsync(reviewX, mainY); };

            // 장비축 이동 요청 바인딩 갱신 (로그 + 동시 이동)
            _opticForm.RequestScannerMove += async (x, y) =>
            {
                AddLog(lstRecipeScanLog, $"[MoveScan] ReviewX→{x:F3}, MainY→{y:F3} 요청");
                try
                {
                    await _acsMotion.ServoOnAsync(Axis.X);
                    await _acsMotion.ServoOnAsync(Axis.Y);
                    var tx = _acsMotion.MoveAbsAsync(Axis.X, x); // Review X
                    var ty = _acsMotion.MoveAbsAsync(Axis.Y, y); // Main   Y
                    await Task.WhenAll(tx, ty);
                    AddLog(lstRecipeScanLog, "[MoveScan] 완료");
                }
                catch (Exception ex)
                {
                    AddLog(lstRecipeScanLog, "[E][MoveScan] " + ex.Message);
                    try { await _acsMotion.StopAsync(Axis.X); await _acsMotion.StopAsync(Axis.Y); } catch { }
                }
            };
            _opticForm.RequestLaserOn += async () => await LaserSwitchAsync(true);
            _opticForm.RequestLaserOff += async () => await LaserSwitchAsync(false);

            // Program Mdoe 연결
            _opticForm.ModeProvider = () => GetProgramModeFromUi();

            // 좌측 리스트 등 기존 준비
            WireRecipeListEvents();
            RefreshLocalRecipeList();

            // 첫 로딩 시 좌표 그리드 표시
            _opticForm.RefreshProcessGrid();

            // IO Form 임베딩
            _ioForm = new IOForm { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            AddLog(lstSystem, $"[WagoIO] Driver IP='{AppConfig.Current.WagoIp}', UseSimulator={AppConfig.Current.WagoUseSimulator}");
            tabIO.Controls.Clear();
            tabIO.Controls.Add(_ioForm);
            _ioForm.Show();

            // Manual Motion Form 임베딩
            _motionForm = new MotionManualForm()
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            _motionForm.AttachMotion(_acsMotion);
            _motionForm.AttachAjin(_ajinMotion);
            _motionForm.ModeProvider = () => GetProgramModeFromUi();

            tabMotion.Controls.Clear();
            tabMotion.Controls.Add(_motionForm);
            _motionForm.Show();

            // ESC Flatness 임베딩
            if (_lds == null) _lds = BuildLds();
            _escFlatnessForm = new ESCFlatnessForm(_acsMotion, _ldsHub)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            _escFlatnessForm.ModeProvider = () => GetProgramModeFromUi();

            // 탭 페이지에 임베드
            tabESC.Controls.Clear();
            tabESC.Controls.Add(_escFlatnessForm);
            _escFlatnessForm.Show();

            // Stage Offset 임베딩
            _stageOffsetForm = new StageOffsetForm()
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            _stageOffsetForm.AttachMotion(_acsMotion);
            tabStageOffset.Controls.Clear();
            tabStageOffset.Controls.Add(_stageOffsetForm);
            _stageOffsetForm.Show();

            _overviewForm = new OverviewForm(new OverviewForm.Options
            {
                AcsMotion = _acsMotion,
                IoForm = _ioForm,
                //Overview에서 실시간 위치 못가져오는 문제 해결.
                AjinMotion = _ajinMotion,
                LaserOn = () => StageWin.WagoIO.VirtualBus.DigitalInputs.TryGet("Laser ON / OFF Status", out var v) && v.RawBit,
                Setpoint = ax => 0.0,
                AcsOnline = () => _acsMotion?.IsConnected == true,
                AcsSim = () => (AppConfig.Current?.AcsUseSimulator ?? false),
                AjinOnline = () => _ajinMotion?.IsConnected == true,
                AjinSim = () => false,
                LdsOnline = () => _lds?.IsConnected == true,
                LdsSim = () => (AppConfig.Current?.LdsUseSimulator ?? false),
                // !!!중요: _overviewForm 생성 전에 _ioForm을 먼저 만들어야함
                WagoOnline = () => _ioForm?.IsConnected == true,
                WagoSim = () => (AppConfig.Current?.WagoUseSimulator ?? false),
                ModeProvider = () => GetProgramModeFromUi(),
            });
            _overviewForm.TopLevel = false;
            _overviewForm.FormBorderStyle = FormBorderStyle.None;
            _overviewForm.Dock = DockStyle.Fill;
            tabOverview.Controls.Add(_overviewForm);
            _overviewForm.Show();
            WireTabLockGuard();

            // Alarm Manager 초기화, AlarmConfig 로드 (D:\AppConfig\AlarmConfig.json 사용)
            var alarmCfgPath = AppConfig.GetConfigFile("AlarmConfig.json");
            var alarmCfg = Alarm.AlarmConfig.Load(alarmCfgPath);
            AddLog(lstSystem, $"[ALARM] Config: '{alarmCfgPath}', exists={System.IO.File.Exists(alarmCfgPath)}, count={alarmCfg?.Alarms?.Length ?? 0}");

            // gridAlarms 읽기 전용 보장
            if (gridAlarms != null)
            {
                gridAlarms.ReadOnly = true;
                gridAlarms.AllowUserToAddRows = false;
                gridAlarms.AllowUserToDeleteRows = false;
                gridAlarms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            // 모드 강제/확인 람다
            ProgramMode GetMode() => GetProgramModeFromUi();

            // 알람 매니저 시작
            _alarmMgr = new Alarm.AlarmManager(
                provider: new Alarm.WagoIoProvider(),
                cfg: alarmCfg,
                view: _alarms,                    // 기존 BindingList<AlarmRow>
                log: s => AddLog(lstSystem, s),
                modeProvider: GetMode,
                forceModeSetter: ForceMode);
            _alarmMgr.Start();
            UpdateAutoFormEnableState();

            // Reset 버튼 이벤트 바인딩
            btnAlarmReset.Click += (s, e2) =>
            {
                _alarmMgr.ResetLatched();              // 알람 Latch 해제 시도
                _towerForm?.ArmBuzzerAfterReset();     // 무음 Latch 해제
                _towerForm?.RefreshNow();              // 즉시 평가 → 알람이 남아있으면 즉시 BUZZER ON
                UpdateAutoFormEnableState();
            };
            this.btnForceAbort.Click += BtnForceAbort_Click;

            // 모드 바뀔 때 차단(사용자가 바꾸는 즉시 복귀)
            rbtnModeManual.CheckedChanged += ModeRadio_CheckedChanged;
            rbtnModeSemiAuto.CheckedChanged += ModeRadio_CheckedChanged;
            rbtnModeAuto.CheckedChanged += ModeRadio_CheckedChanged;

            // TowerLamp 임베딩 (pnlTowerLamp 내부에 Dock)
            _towerForm = new TowerLampForm(new TowerLampForm.Options
            {
                ModeProvider = () => GetProgramModeFromUi(),
                HasAlarmProvider = () => _alarmMgr?.HasActiveLevel(AlarmLevel.Alarm) ?? false,
                AutoSeqRunningProvider = () => _autoSeqRunning,
                WriteOutput = (name, val, src) => _ioForm?.TryWriteOutputByName(name, val, src)
            });
            _towerForm.TopLevel = false;
            _towerForm.Dock = DockStyle.Fill;
            pnlTowerLamp.Controls.Clear();
            pnlTowerLamp.Controls.Add(_towerForm);
            _towerForm.Show();
        }
        // RecipeForm에서 "Set Manual Inspection" 클릭 시 호출 -> Vision 3008을 10초 대기
        private Task<ST_MANUAL_INSPECTION_REQ> WaitManualInspectionFromVisionAsync(StageWin.UI.RecipeForm.Cell cell)
        {
            var tcs = new TaskCompletionSource<ST_MANUAL_INSPECTION_REQ>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_manualInspectionLock)
            {
                _manualInspectionArmed = true;
                _manualInspectionCell = cell;
                _manualInspectionTcs = tcs;
            }

            AddLog(lstRecipeScanLog,
                $"[Stage] ManualInspection ARM (cmd=3008) cell=R{cell.Row}C{cell.Col} V{cell.VecR},{cell.VecC} timeout={MANUAL_INSPECTION_TIMEOUT_MS}ms");

            // 10초 타임아웃 처리
            _ = Task.Run(async () =>
            {
                await Task.Delay(MANUAL_INSPECTION_TIMEOUT_MS).ConfigureAwait(false);

                TaskCompletionSource<ST_MANUAL_INSPECTION_REQ> timeoutTcs = null;
                lock (_manualInspectionLock)
                {
                    if (_manualInspectionArmed && ReferenceEquals(_manualInspectionTcs, tcs))
                    {
                        _manualInspectionArmed = false;
                        timeoutTcs = _manualInspectionTcs;
                        _manualInspectionTcs = null;
                    }
                }

                if (timeoutTcs != null)
                {
                    AddLog(lstRecipeScanLog, "[Stage][TIMEOUT] ManualInspection: Vision 요청(3008) 수신 없음");
                    timeoutTcs.TrySetException(new TimeoutException("ManualInspection timeout"));
                }
            });

            return tcs.Task;
        }
        // Vision 3008 수신 시 호출되는 쪽(라우터 핸들러에서 호출)
        private void OnManualInspectionReq(ST_MANUAL_INSPECTION_REQ req)
        {
            TaskCompletionSource<ST_MANUAL_INSPECTION_REQ> tcs = null;
            StageWin.UI.RecipeForm.Cell cell = default;

            lock (_manualInspectionLock)
            {
                if (_manualInspectionArmed && _manualInspectionTcs != null)
                {
                    _manualInspectionArmed = false;
                    tcs = _manualInspectionTcs;
                    cell = _manualInspectionCell;
                    _manualInspectionTcs = null;
                }
            }

            if (tcs != null)
            {
                AddLog(lstRecipeScanLog,
                    $"[Stage] ManualInspection HIT -> apply target cell=R{cell.Row}C{cell.Col} V{cell.VecR},{cell.VecC}");
                tcs.TrySetResult(req);
            }
            else
            {
                AddLog(lstRecipeScanLog, "[Stage] ManualInspection 수신(3008) but NOT ARMED -> ignored");
            }
        }
        private async void BtnForceAbort_Click(object sender, EventArgs e)
        {
            if (System.Threading.Interlocked.Exchange(ref _abortReentry, 1) == 1) return;
            btnForceAbort.Enabled = false;
            var prev = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                await ForceAbortAsync(TimeSpan.FromSeconds(5)); // 전체 5초 안팎으로 강제 정리
                MessageBox.Show(this, "강제 종료 완료.", "Force Abort", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][ABORT] " + ex.Message);
                MessageBox.Show(this, "강제 종료 중 오류: " + ex.Message, "Force Abort", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Cursor = prev;
                btnForceAbort.Enabled = true;
                System.Threading.Interlocked.Exchange(ref _abortReentry, 0);
            }
        }
        private async Task ForceAbortAsync(TimeSpan budget)
        {
            var sw = Stopwatch.StartNew();
            void ThrowIfBudget() { if (sw.Elapsed > budget) throw new TimeoutException("ForceAbort budget exceeded"); }
            AddLog(lstRecipeScanLog, "[ABORT] ==== FORCE ABORT BEGIN ====");
            // 내부 실행중 토큰 전부 취소(시퀀스/리뷰플라잉/프로세스 등)
            try { _procCts?.Cancel(); } catch { }
            // RecipeForm 내부의 Review 측정도 강제 취소
            try { _recipeForm?.AbortReviewMeasurement(); } catch { }
            ThrowIfBudget();
            // 1) 레이저/스캔 측에 STOP 통보 (1006)
            try
            {
                var scan = Find(Role.Scan);
                if (scan != null)
                {
                    int scanNo = _opticForm?.CurrentScanNo ?? 0;
                    AddLog(lstRecipeScanLog, $"[ABORT] Send ScanStop(1006) scanNo={scanNo}");
                    await ScanRpc.Request_ScanStopAsync(scan, scanNo, timeoutMs: 2000);
                    _opticForm?.SetLaserEmission(false);
                }
            }
            catch (Exception ex) { AddLog(lstRecipeScanLog, "[W][ABORT] ScanStop: " + ex.Message); }
            ThrowIfBudget();
            // 2) 축 정지 (ACS X/Y, Ajin T)
            try { await _acsMotion.StopAsync(Axis.X); } catch { }
            try { await _acsMotion.StopAsync(Axis.Y); } catch { }
            try { if (_ajinMotion?.IsConnected == true) _ajinMotion.Stop(StageWin.Etc.AjinAxis.T); } catch { }
            ThrowIfBudget();
            // 3) ACS 버퍼/트리거 정리
            try
            {
                await AbortAcsProgramsAndTriggersAsync();
            }
            catch (Exception ex) { AddLog(lstRecipeScanLog, "[W][ABORT] ACS cleanup: " + ex.Message); }
            ThrowIfBudget();
            // 4) Vision 쪽 진행 중 플로우가 있다면 MoveDone(3007)으로 상태 정리 시도
            try
            {
                var vision = Find(Role.Vision);
                if (vision != null)
                {
                    AddLog(lstRecipeScanLog, "[ABORT] Vision FLYING_MOVE_DONE(3007)");
                    // 3007은 bool ACK 반환. VisionRpc에 이미 랩퍼가 있으므로 재사용.
                    await StageWin.Driver.Network.Packets.VisionRpc.Request_FlyingMoveDoneAsync(vision, timeoutMs: 1500);
                }
            }
            catch (Exception ex) { AddLog(lstRecipeScanLog, "[W][ABORT] VisionMoveDone: " + ex.Message); }
            ThrowIfBudget();
            // 5) IO/릴레이 OFF (트리거 릴레이, 레이저 인터락 관련 출력들 정지)
            try
            {
                var cfg = AppConfig.Current ?? new AppConfig();
                _ioForm?.TryWriteOutputByName(cfg.ReviewTrigOutXName, false, "ForceAbort");
                _ioForm?.TryWriteOutputByName(cfg.ReviewTrigOutYName, false, "ForceAbort");
            }
            catch { /* ignore */ }
            ThrowIfBudget();
            // 6) 상태/UI 리셋
            try
            {
                _autoSeqRunning = false;
                _towerForm?.RefreshNow();
            }
            catch { }
            AddLog(lstRecipeScanLog, "[ABORT] ==== FORCE ABORT END ====");
        }
        private async Task AbortAcsProgramsAndTriggersAsync()
        {
            var cfg = AppConfig.Current ?? new AppConfig();
            string trigX = string.IsNullOrWhiteSpace(cfg.AcsTrigVarX) ? "POS_ARRAY_X1" : cfg.AcsTrigVarX;
            string trigY = string.IsNullOrWhiteSpace(cfg.AcsTrigVarY) ? "POS_ARRAY_Y1" : cfg.AcsTrigVarY;

            int bufX = (cfg.AcsBufferX > 0) ? cfg.AcsBufferX : 14;
            int bufY = (cfg.AcsBufferY > 0) ? cfg.AcsBufferY : 13;

            // 3-1) 버퍼 중지
            await AcsStopBufferIfPossibleAsync(bufX);
            await AcsStopBufferIfPossibleAsync(bufY);

            // 3-2) 트리거 배열 0으로
            await ClearAcsTriggerArrayAsync(trigX, 1024);
            await ClearAcsTriggerArrayAsync(trigY, 1024);
        }
        private async Task AcsStopBufferIfPossibleAsync(int bufferNo)
        {
            try
            {
                var acsProg = _acsMotion as StageWin.Driver.Motion.IAcsPrograms;
                if (acsProg != null)
                {
                    await acsProg.StopBufferAsync(bufferNo);
                    AddLog(lstRecipeScanLog, $"[ABORT][ACS] StopBuffer buf={bufferNo}");
                    return;
                }

                // 리플렉션 보조 (어댑터가 StopBufferAsync 없는 구버전인 경우)
                var mAsync = _acsMotion?.GetType().GetMethod("StopBufferAsync", new[] { typeof(int) });
                if (mAsync != null)
                {
                    await (Task)mAsync.Invoke(_acsMotion, new object[] { bufferNo });
                    AddLog(lstRecipeScanLog, $"[ABORT][ACS] StopBuffer(Reflect) buf={bufferNo}");
                    return;
                }

                var mSync = _acsMotion?.GetType().GetMethod("StopBuffer", new[] { typeof(int) });
                if (mSync != null)
                {
                    mSync.Invoke(_acsMotion, new object[] { bufferNo });
                    AddLog(lstRecipeScanLog, $"[ABORT][ACS] StopBuffer(Sync) buf={bufferNo}");
                    return;
                }

                AddLog(lstRecipeScanLog, $"[W][ABORT][ACS] StopBuffer 메서드 없음 buf={bufferNo}");
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, $"[W][ABORT][ACS] StopBuffer 실패 buf={bufferNo}: {ex.Message}");
            }
        }
        #endregion

        #region ======= 레시피 관련 함수 =======
        // 1000: Recipe List 요청 후 결과를 lstScan에 표시
        private async Task RefreshScanRecipeListAsync()
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            try
            {
                var rsp = await ScanRpc.Request_RecipeListAsync(scan, 5000);
                var names = ScanUtil.ToStrings(rsp);
                UI(() =>
                {
                    lstScanRecipeList.BeginUpdate();
                    lstScanRecipeList.Items.Clear();
                    foreach (var n in names)
                        lstScanRecipeList.Items.Add(MakeItem(n, fromScan: true));
                    lstScanRecipeList.EndUpdate();
                });
                AddLog(lstRecipeScanLog, $"[1000] Recipe list 수신: {names.Length}개");
            }
            catch (Exception ex) { AddLog(lstRecipeScanLog, "[E][1000] " + ex.Message); }
        }
        private async void btnReqRecipeList_Click(object sender, EventArgs e)
        {
            await RefreshScanRecipeListAsync();
        }
        private async void btnReqRecipeAdd_Click(object sender, EventArgs e)
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            var name = (txtNewRecipe.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "추가할 레시피 이름을 입력하세요.", "Recipe Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Scan 프로토콜은 ASCII 40바이트 고정명을 사용
            if (name.Length > NetCommon.ScanConst.RECIPE_NAME_BYTES)
                name = name.Substring(0, NetCommon.ScanConst.RECIPE_NAME_BYTES);

            try
            {
                var echoed = await ScanRpc.Request_RecipeAddAsync(scan, name, 5000);
                AddLog(lstRecipeScanLog, $"[1001] RecipeAdd OK: '{echoed}'");

                // 추가 직후 리스트 갱신
                await RefreshScanRecipeListAsync();
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][1001] " + ex.Message);
            }
        }
        private void SelectByName(ListBox lb, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            for (int i = 0; i < lb.Items.Count; i++)
            {
                var it = lb.Items[i] as RecipeListItem;
                var s = it?.Name ?? lb.Items[i] as string;
                if (string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) // 대소문자 무시
                {
                    lb.SelectedIndex = i;
                    return;
                }
            }
        }
        private void btnDownloadScanToLocal_Click(object sender, EventArgs e)
        {
            var it = lstScanRecipeList.SelectedItem as RecipeListItem;
            var name = it?.Name ?? (lstScanRecipeList.SelectedItem as string);
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "스캔 레시피를 선택하세요.", "내려받기", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                // 1) 스캔 캐시에서 로드(없으면 대체 경로)
                RecipeDoc doc = null;
                var scanPath = _scanStore.GetFullPath(name);

                if (File.Exists(scanPath))
                {
                    doc = _scanStore.Load(name);
                }
                else if (it?.Doc != null && string.Equals(it.Doc.Header.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    //  리스트 아이템이 들고있는 최신 Doc 사용(선택→1004 직후 등)
                    doc = it.Doc;
                }
                else if (_recipeForm?.CurrentDoc != null && string.Equals(_recipeForm.CurrentDoc.Header.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    //  에디터에 로드된 동일명 문서가 있으면 사용
                    doc = _recipeForm.CurrentDoc;
                }
                else
                {
                    // 마지막 안전망: 새로 생성(값은 0일 수 있음)
                    doc = _localStore.New(name);
                }

                // 2) 로컬 저장소에 저장
                _localStore.Save(doc);

                // 3) 로컬 리스트 갱신 + 선택
                RefreshLocalRecipeList();
                SelectByName(lstRecipeList, name);

                // 4) 에디터를 "로컬"로 전환해서 바로 편집 가능하게
                _recipeForm.SetOpenOrigin(RecipeForm.RecipeOpenOrigin.Local);
                _recipeForm.LoadRecipeDoc(doc);
                AddLog(lstRecipeScanLog, $"[Download] '{name}' → Local 저장 및 목록 갱신");
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][Download] " + ex.Message);
                MessageBox.Show(this, "내려받기 실패: " + ex.Message, "내려받기", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRefreshLocalList_Click(object sender, EventArgs e)
        {
            try
            {
                // 선택 유지 + 목록만 다시 읽기
                var keepName = (lstRecipeList.SelectedItem as RecipeListItem)?.Name
                               ?? (lstRecipeList.SelectedItem as string);

                RefreshLocalRecipeList();              // 폴더의 json 목록을 다시 스캔
                if (!string.IsNullOrWhiteSpace(keepName))
                    SelectByName(lstRecipeList, keepName);

                // 에디터가 '로컬'을 열람 중이면, 같은 파일을 디스크에서 재로딩
                if (!string.IsNullOrWhiteSpace(keepName)
                    && _recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Local
                    && File.Exists(_localStore.GetFullPath(keepName)))
                {
                    _recipeForm.LoadRecipeByName(keepName);
                }
                AddLog(lstRecipeScanLog, "[Local] 레시피 목록 새로고침 완료");
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][Local Refresh] " + ex.Message);
                MessageBox.Show(this, "로컬 레시피 새로고침 실패: " + ex.Message,
                    "새로고침", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //  이름 절단: Scan은 ASCII 40바이트 고정 
        private static string TruncateForScanName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            int max = NetCommon.ScanConst.RECIPE_NAME_BYTES; // 40
                                                             // 바이트 기준 절단이 필요하면 Encoding.ASCII.GetBytes 후 잘라 재조립해도 됨.
            return name.Length > max ? name.Substring(0, max) : name;
        }
        // Commit 후 Scan 리스트 갱신 + 해당 이름 선택 고정
        private async Task PostCommitUpdateScanListAsync(string scanName)
        {
            await RefreshScanRecipeListAsync();
            UI(() => SelectByName(lstScanRecipeList, scanName));

            // 선택된 항목의 Doc 포인터도 최신 문서로 연결(스테일 방지)
            var it = lstScanRecipeList.SelectedItem as RecipeListItem;
            if (it != null && string.Equals(it.Name, scanName, StringComparison.OrdinalIgnoreCase))
                it.Doc = _recipeForm?.CurrentDoc;
        }
        private void RefreshLocalRecipeList()
        {
            try
            {
                var names = _localStore.ListNames();
                lstRecipeList.BeginUpdate();
                lstRecipeList.Items.Clear();
                lstRecipeList.Items.AddRange(names.Select(n => MakeItem(n, fromScan: false)).ToArray());
                lstRecipeList.EndUpdate();
            }
            catch (Exception ex) { AddLog(lstRecipeScanLog, "Local recipe list failed: " + ex.Message); }
        }
        private void RefreshLocalRecipeListKeepingSelection()
        {
            var keepName = (lstRecipeList.SelectedItem as RecipeListItem)?.Name ?? (lstRecipeList.SelectedItem as string);
            // 리프레시 중 SelectedIndexChanged로 Load가 튀는 것 방지
            lstRecipeList.SelectedIndexChanged -= LstRecipeSelected;
            try
            {
                RefreshLocalRecipeList();
                if (!string.IsNullOrWhiteSpace(keepName)) SelectByName(lstRecipeList, keepName);
            }
            finally
            {
                lstRecipeList.SelectedIndexChanged += LstRecipeSelected;
            }
        }
        private async void OnRecipeCommitted(string name)
        {
            try
            {
                // 커밋 시작 시점 스냅샷 (중간에 Doc 바뀌는 것 방지)
                var origin = _recipeForm?.LastPickedOrigin ?? _recipeForm?.OpenOrigin ?? RecipeForm.RecipeOpenOrigin.Local;
                var docSnapshot = _recipeForm?.CurrentDoc; // 중요: 이후 절대 CurrentDoc 다시 의존하지 않기
                var recipeName = name;
                // Scan에서 연 레시피 커밋이면 Local 리스트/파일 건드리지 않음
                if (origin == RecipeForm.RecipeOpenOrigin.Local)
                {
                    RefreshLocalRecipeListKeepingSelection();
                }
                else
                {
                    // Scan 커밋이면 로컬 리스트 갱신 자체를 하지 않는다
                    AddLog(lstRecipeScanLog, $"[Commit] origin=Scan → Local list refresh SKIP: '{recipeName}'");
                }
                var scan = Find(Role.Scan);
                if (scan == null)
                {
                    AddLog(lstRecipeScanLog, "[Scan] 연결 없음 → Commit 원격 반영 생략");
                    return;
                }
                // Scan 이름 40바이트 제한
                var scanName = TruncateForScanName(recipeName);
                var skipConfirm = _skipOverwriteConfirmOnce;
                _skipOverwriteConfirmOnce = false;
                // Scan에서 마지막으로 클릭된 항목이면 덮어쓰기 질문 금지
                if (!skipConfirm)
                {
                    if (origin == RecipeForm.RecipeOpenOrigin.Scan)
                    {
                        skipConfirm = true;
                        AddLog(lstRecipeScanLog, $"[Commit] origin=Scan → overwrite confirm 생략: '{scanName}'");
                    }
                }
                // Local에서 Scan으로 올릴 때만 Scan에 동일명 존재 여부 질문
                if (!skipConfirm && origin == RecipeForm.RecipeOpenOrigin.Local)
                {
                    if (!await ConfirmOverwriteIfScanHasSameNameAsync(scanName))
                    {
                        AddLog(lstRecipeScanLog, $"[Commit] 사용자가 Scan 덮어쓰기를 취소: '{scanName}'");
                        return;
                    }
                }
                // 1001: 존재 보장(없으면 생성)
                var echoed = await ScanRpc.Request_RecipeAddAsync(scan, scanName, 5000);
                AddLog(lstRecipeScanLog, $"[1001] Commit → Scan Add OK: '{echoed}'");
                // 1003: 선택 고정
                var sel = await ScanRpc.Request_RecipeSelectAsync(scan, scanName, 3000);
                AddLog(lstRecipeScanLog, $"[1003] RecipeSelect OK: '{sel}'");
                // Scan 캐시 동기화: Scan 커밋이면 ScanStore만 저장 (LocalStore는 절대 저장 금지)
                try
                {
                    if (docSnapshot != null) _scanStore.Save(docSnapshot);
                }
                catch { }
                // Commit 직후 스캔 리스트 갱신 + 선택 고정
                await PostCommitUpdateScanListAsync(scanName);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][Commit→1001/1003/1005] " + ex.Message);
            }
        }
        private async Task<bool> ConfirmOverwriteIfScanHasSameNameAsync(string name)
        {
            var scan = Find(Role.Scan);
            if (scan == null) return true;
            try
            {
                var rsp = await ScanRpc.Request_RecipeListAsync(scan, 5000);
                var names = ScanUtil.ToStrings(rsp);
                var truncated = TruncateForScanName(name);
                bool exists = names.Any(n =>
                    string.Equals(n, truncated, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
                if (!exists) return true;
                var ans = MessageBox.Show(this,
                    $"Scan 목록에 같은 이름의 레시피가 이미 존재합니다.\r\n'{truncated}'을(를) 덮어쓸까요?", "Scan 레시피 덮어쓰기",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                return ans == DialogResult.Yes;
            }
            catch { return true; }
        }
        private async void OnRecipeDeleteRequestedFromScan(string name)
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            try
            {
                var echoed = await ScanRpc.Request_RecipeDeleteAsync(scan, name, 5000);
                AddLog(lstRecipeScanLog, $"[1002] RecipeDelete OK: '{echoed}'");

                // 스캔 캐시 JSON 파일도 삭제 (분리 보관)
                try { _scanStore.Delete(name); } catch { }

                // 이름 리스트는 원격이므로 다시 받아와 반영
                await RefreshScanRecipeListAsync();
                if (lstScanRecipeList.Items.Count > 0) lstScanRecipeList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][1002] " + ex.Message);
                MessageBox.Show(this, "Scan Recipe Delete 실패: " + ex.Message,
                    "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void WireRecipeListEvents()
        {
            // 중복 방지: 먼저 떼고 다시 연결
            lstRecipeList.SelectedIndexChanged -= LstRecipeSelected;
            lstScanRecipeList.SelectedIndexChanged -= LstScanSelected;
            lstRecipeList.SelectedIndexChanged += LstRecipeSelected;
            lstScanRecipeList.SelectedIndexChanged += LstScanSelected;

            // MouseDown도 명명 핸들러로 교체하고 idempotent하게 연결
            lstRecipeList.MouseDown -= LstRecipeList_MouseDown;
            lstRecipeList.MouseDown += LstRecipeList_MouseDown;
            lstScanRecipeList.MouseDown -= LstScanRecipeList_MouseDown;
            lstScanRecipeList.MouseDown += LstScanRecipeList_MouseDown;
        }
        private void LstRecipeList_MouseDown(object sender, MouseEventArgs e)
        {
            int idx = lstRecipeList.IndexFromPoint(e.Location);
            if (idx >= 0 && idx == lstRecipeList.SelectedIndex)
            {
                LoadFromListBoxSelection(lstRecipeList, fromScan: false);
            }
        }
        private void LstScanRecipeList_MouseDown(object sender, MouseEventArgs e)
        {
            int idx = lstScanRecipeList.IndexFromPoint(e.Location);
            if (idx >= 0 && idx == lstScanRecipeList.SelectedIndex)
            {
                LoadFromListBoxSelection(lstScanRecipeList, fromScan: true);
            }
        }
        private void LstRecipeSelected(object s, EventArgs e)
        {
            LoadFromListBoxSelection(lstRecipeList, fromScan: false);
        }
        private async void LstScanSelected(object s, EventArgs e)
        {
            LoadFromListBoxSelection(lstScanRecipeList, fromScan: true);
            var it = lstScanRecipeList.SelectedItem as RecipeListItem;
            var name = it?.Name ?? (lstScanRecipeList.SelectedItem as string);
            var scan = Find(Role.Scan);
            if (scan != null && !string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    // 1003: 선택 동기화 (유지)
                    var echoed = await ScanRpc.Request_RecipeSelectAsync(scan, name, 3000);
                    AddLog(lstRecipeScanLog, $"[1003] RecipeSelect OK: '{echoed}'");

                    // 문서 확보: 캐시 → 에디터 동일명 → 새 생성
                    RecipeDoc doc =
                           it?.Doc
                        ?? (File.Exists(_scanStore.GetFullPath(name)) ? _scanStore.Load(name)
                         : (_recipeForm?.CurrentDoc != null
                            && string.Equals(_recipeForm.CurrentDoc.Header.Name, name, StringComparison.OrdinalIgnoreCase)
                            ? _recipeForm.CurrentDoc
                            : _scanStore.New(name)));

                    // UI 반영 + 캐시 저장
                    _recipeForm.SetOpenOrigin(RecipeForm.RecipeOpenOrigin.Scan);
                    _recipeForm.RecipeNameText = name;
                    _opticForm?.SetRecipeContext(name, "Scan");
                    _scanStore.Save(doc);
                    if (it != null) it.Doc = doc;

                    AddLog(lstRecipeScanLog, $"[Scan 캐시] '{name}' 공유 파라미터 동기화 및 저장 완료");
                }
                catch (Exception ex)
                {
                    AddLog(lstRecipeScanLog, "[E][1003/PullShared] " + ex.Message);
                }
            }
        }
        private void LoadFromListBoxSelection(ListBox lb, bool fromScan)
        {
            var sel = lb.SelectedItem;
            if (sel == null || _recipeForm == null) return;

            var item = sel as RecipeListItem;
            string name = item?.Name ?? (sel as string);
            if (string.IsNullOrWhiteSpace(name)) return;
            _recipeForm.RememberPickedRecipe(fromScan ? RecipeForm.RecipeOpenOrigin.Scan : RecipeForm.RecipeOpenOrigin.Local, name);
            // 중복 호출 억제
            if (ShouldDebounceLoad(fromScan, name)) return;
            if (System.Threading.Interlocked.Exchange(ref _loadReentry, 1) == 1) return;
            try
            {
                _recipeForm.SetOpenOrigin(fromScan ? RecipeForm.RecipeOpenOrigin.Scan : RecipeForm.RecipeOpenOrigin.Local);
                bool ok = false;
                if (fromScan)
                {
                    RecipeDoc doc = null;
                    var path = _scanStore.GetFullPath(name);
                    if (File.Exists(path)) doc = _scanStore.Load(name);
                    else if (item?.Doc != null) doc = item.Doc;
                    else if (_recipeForm?.CurrentDoc != null && string.Equals(_recipeForm.CurrentDoc.Header.Name, name, StringComparison.OrdinalIgnoreCase))
                        doc = _recipeForm.CurrentDoc;
                    else
                        doc = _scanStore.New(name);
                    _recipeForm.LoadRecipeDoc(doc);
                    if (item != null) item.Doc = doc;
                    ok = true;
                }
                else
                {
                    if (item?.Doc != null)
                    {
                        _recipeForm.LoadRecipeDoc(item.Doc);
                        ok = true;
                    }
                    else
                    {
                        ok = _recipeForm.TryLoadRecipeByName(name, quietMissing: false, createIfMissing: false);
                        if (ok && item != null) item.Doc = _recipeForm.CurrentDoc;
                    }
                }
                if (ok)
                {
                    AddLog(lstRecipeScanLog, $"Recipe loaded → {name}");
                    if (_recipeForm?.CurrentDoc != null)
                    {
                        _opticForm?.SetRecipeContext(name, fromScan ? "Scan" : "Local");
                    }
                    UpdateCurrentRecipeUi(name, fromScan);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Load failed: " + ex.Message, "Recipe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref _loadReentry, 0);
            }
        }
        private void SetupRecipeWatchers()
        {
            try
            {
                // Local
                _fswLocal?.Dispose();
                Directory.CreateDirectory(_dirLocal);
                _fswLocal = new FileSystemWatcher(_dirLocal, "*.json")
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false
                };
                _fswLocal.Created += (s, e) => Debounce(_fswDebounceLocal);
                _fswLocal.Changed += (s, e) => Debounce(_fswDebounceLocal);
                _fswLocal.Deleted += (s, e) => Debounce(_fswDebounceLocal);
                _fswLocal.Renamed += (s, e) => Debounce(_fswDebounceLocal);

                // Scan
                _fswScan?.Dispose();
                Directory.CreateDirectory(_dirScan);
                _fswScan = new FileSystemWatcher(_dirScan, "*.json")
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false
                };
                _fswScan.Created += (s, e) => Debounce(_fswDebounceScan);
                _fswScan.Changed += (s, e) => Debounce(_fswDebounceScan);
                _fswScan.Deleted += (s, e) => Debounce(_fswDebounceScan);
                _fswScan.Renamed += (s, e) => Debounce(_fswDebounceScan);

                _fswDebounceLocal.Tick -= FswDebounceLocal_Tick;
                _fswDebounceLocal.Tick += FswDebounceLocal_Tick;
                _fswDebounceScan.Tick -= FswDebounceScan_Tick;
                _fswDebounceScan.Tick += FswDebounceScan_Tick;
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "FSW init fail: " + ex.Message);
            }
        }
        private void FswDebounceLocal_Tick(object sender, EventArgs e)
        {
            _fswDebounceLocal.Stop();
            var cur = (lstRecipeList.SelectedItem as RecipeListItem)?.Name
                      ?? (lstRecipeList.SelectedItem as string);
            RefreshLocalRecipeListKeepingSelection();
            if (!string.IsNullOrWhiteSpace(cur) && File.Exists(_localStore.GetFullPath(cur)))
            {
                if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Local) _recipeForm?.LoadRecipeByName(cur);
            }
        }
        private void FswDebounceScan_Tick(object sender, EventArgs e)
        {
            _fswDebounceScan.Stop();
            var curScan = (lstScanRecipeList.SelectedItem as RecipeListItem)?.Name
                          ?? (lstScanRecipeList.SelectedItem as string);
            if (!string.IsNullOrWhiteSpace(curScan) && File.Exists(_scanStore.GetFullPath(curScan)))
            {
                if (_recipeForm.OpenOrigin == RecipeForm.RecipeOpenOrigin.Scan) _recipeForm?.LoadRecipeByName(curScan);
            }
        }
        void Debounce(Timer t) { t.Stop(); t.Start(); }
        #endregion

        #region ======= Scan/Vision 소켓통신관련 함수 =======
        private async void btnReqAlign_Click(object sender, EventArgs e)
        {
            //if (!EnsureSemiOrAuto("Glass Align")) return;         // Test KJW

            var vision = GetVisionSessionOrWarn();
            if (vision == null) return;
            try
            {
                AddLog(lstRecipeScanLog, "[3000] Glass Align 요청...");
                int timeoutMs = 15000;
                AddLog(lstRecipeScanLog, "[3000][SEQ] Glass Align 요청...");
                var rsp = await Logger.AutoRpcAsync(
                    name: "VisionRpc.GlassAlign(3000)",
                    timeoutMs: timeoutMs,
                    call: () => VisionRpc.Request_GlassAlignAsync(vision, timeoutMs: timeoutMs)
                );
                var ok = (rsp.nResult == 1);
                AddLog(lstRecipeScanLog, ok ? "[3000] 결과: OK" : "[3000] 결과: NG");
                if (!ok) MessageBox.Show(this, "Glass Align 실패 응답(NG).", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, "[W][3000] Glass Align 타임아웃");
                MessageBox.Show(this, "Glass Align 타임아웃.", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][3000] " + ex.Message);
                MessageBox.Show(this, "Glass Align 요청 실패: " + ex.Message, "Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 2nd Align (3001) 요청
        private async void btnReq2ndAlign_Click(object sender, EventArgs e)
        {
            //if (!EnsureSemiOrAuto("2nd Align")) return;

            var vision = GetVisionSessionOrWarn();
            if (vision == null) return;
            try
            {
                AddLog(lstRecipeScanLog, "[3001] 2nd Align 요청...");
                int timeoutMs = 15000;
                AddLog(lstRecipeScanLog, "[3001][SEQ] 2nd Align 요청...");
                var rsp = await Logger.AutoRpcAsync(
                    name: "VisionRpc.2ndAlign(3001)",
                    timeoutMs: timeoutMs,
                    call: () => VisionRpc.Request_SecondAlignAsync(vision, timeoutMs: timeoutMs)
                );
                var ok = (rsp.nResult == 1);
                AddLog(lstRecipeScanLog, ok ? "[3001] 결과: OK" : "[3001] 결과: NG");
                if (!ok) MessageBox.Show(this, "2nd Align 실패 응답(NG).", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, "[W][3001] 2nd Align 타임아웃");
                MessageBox.Show(this, "2nd Align 타임아웃.", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][3001] " + ex.Message);
                MessageBox.Show(this, "2nd Align 요청 실패: " + ex.Message, "Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 1006: Scan Stop 요청/응답 처리
        private async Task SendScanStopAsync(int scanNo)
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            AddLog(lstRecipeScanLog, $"[1006] ScanStop → scanNo={scanNo}");
            var rsp = await ScanRpc.Request_ScanStopAsync(scan, scanNo, timeoutMs: 3000);

            // echo 확인
            if (rsp.nScanNo == scanNo)
            {
                _opticForm?.SetLaserEmission(false);
                AddLog(lstRecipeScanLog, "[1006] ScanStop OK");
            }
            else
            {
                AddLog(lstRecipeScanLog, $"[1006][W] echo mismatch (rsp={rsp.nScanNo}, req={scanNo})");
                MessageBox.Show(this, "ScanStop 응답의 scanNo가 일치하지 않습니다.",
                    "Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async Task LaserSwitchAsync(bool turnOn)
        {
            //if (!EnsureSemiOrAuto("Laser ON")) return;

            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            try
            {
                if (turnOn)
                {
                    // Optic 폼의 현재 UI 값으로 1007 전송
                    var p = _opticForm?.GetLaserCtrlParams(true)
                            ?? new OpticOperationForm.LaserCtrlParams
                            {
                                ScanNo = 0,
                                TimeMs = 0,
                                Power = 0,
                                Frequency = 0,
                                LaserOn = 1,
                                XOffset = 0,
                                YOffset = 0
                            };
                    await SendLaserControlAsync(p); // 아래 기존 함수 재사용(1007)
                }
                else
                {
                    // OFF는 Scan Stop(1006)
                    int scanNo = _opticForm?.CurrentScanNo ?? 0;
                    await SendScanStopAsync(scanNo);
                }
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, turnOn ? "[W][1007] 타임아웃" : "[W][1006] 타임아웃");
                MessageBox.Show(this, turnOn ? "Laser control 타임아웃" : "Scan stop 타임아웃",
                    "Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, (turnOn ? "[E][1007] " : "[E][1006] ") + ex.Message);
                MessageBox.Show(this, (turnOn ? "Laser control 실패: " : "Scan stop 실패: ") + ex.Message,
                    "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task SendLaserControlAsync(OpticOperationForm.LaserCtrlParams p)
        {
            //if (!EnsureSemiOrAuto("Laser Control Async")) return;

            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            try
            {
                AddLog(lstRecipeScanLog, $"[1007] LaserCtrl → on={p.LaserOn}, t={p.TimeMs}ms, pow={p.Power:F2}, " +
                                  $"freq={p.Frequency:F1}Hz, ofs=({p.XOffset:F5},{p.YOffset:F5}), scanNo={p.ScanNo}");

                // 1007 송신
                var ack = await ScanRpc.Request_LaserControlAsync(
                    scan,
                    p.ScanNo,
                    p.TimeMs,
                    p.Power,
                    p.Frequency,
                    p.LaserOn,
                    p.XOffset,
                    p.YOffset,
                    timeoutMs: 5000);

                // 예외 없이 여기까지 왔으면 성공(헤더 ErrorCode == 0)
                // echo 검증만 수행
                bool ok = (ack.nScanNo == p.ScanNo);

                if (ok)
                {
                    _opticForm?.SetLaserEmission(p.LaserOn == 1);
                    AddLog(lstRecipeScanLog, "[1007] LaserCtrl OK (echo scanNo match)");
                }
                else
                {
                    AddLog(lstRecipeScanLog, $"[1007][W] echo mismatch (rsp={ack.nScanNo}, req={p.ScanNo})");
                    MessageBox.Show(this, "Laser control 응답의 scanNo가 일치하지 않습니다.",
                        "Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, "[W][1007] 타임아웃");
                MessageBox.Show(this, "Laser control 타임아웃", "Scan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][1007] " + ex.Message);
                MessageBox.Show(this, "Laser control 실패: " + ex.Message,
                    "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ======= 모션관련 함수 =======
        private static (double sx, double sy, double ex, double ey) ApplyReadyOffsetToSpan(bool yMajor, bool forward,
                       double startX, double startY, double endX, double endY,
                       double readyMm)
        {
            if (double.IsNaN(readyMm) || double.IsInfinity(readyMm)) readyMm = 0;
            if (readyMm < 0) readyMm = Math.Abs(readyMm);
            if (readyMm == 0) return (startX, startY, endX, endY);
            double sx = startX, sy = startY, ex = endX, ey = endY;
            double startShift = forward ? -readyMm : +readyMm; // 시작은 진행 반대
            double endShift = forward ? +readyMm : -readyMm; // 종료는 진행 동일
            if (yMajor) { sy += startShift; ey += endShift; }
            else { sx += startShift; ex += endShift; }
            return (sx, sy, ex, ey);
        }

        const double X_INPOS_TOL = 0.001;   // mm: 필요에 맞게 조정
        const double Y_INPOS_TOL = 0.001;   // mm
        const int INPOS_SETTLE_MS = 50;  // in-position 유지 확인 시간
        const int POLL_MS = 10;
        private async Task WaitAxisInPositionAsync(
            Axis axis, double target, double tol, int timeoutMs, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            long firstInPosAt = -1;

            // ACS 어댑터면 In-Position 플래그 확인 가능
            var acs = _acsMotion as IAcsStatus;

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                ct.ThrowIfCancellationRequested();

                // 기존 IMotionController에 있는 동기 API 사용
                bool isBusy = false;
                try { isBusy = _acsMotion.IsBusy(axis); } catch { /* ignore */ }

                double cur = double.NaN;
                try { cur = _acsMotion.GetAPosition(axis); } catch { /* ignore */ }

                bool inPosFlag = false;
                try { if (acs != null) inPosFlag = acs.IsInPosition(axis); } catch { /* ignore */ }

                // 판정: 플래그가 있으면 우선, 없으면 Busy+오차 기반으로 판정
                bool inpos = inPosFlag || (!isBusy && !double.IsNaN(cur) && Math.Abs(cur - target) <= tol);

                if (inpos)
                {
                    if (firstInPosAt < 0) firstInPosAt = sw.ElapsedMilliseconds;
                    if (sw.ElapsedMilliseconds - firstInPosAt >= INPOS_SETTLE_MS)
                        return; // 정착 확인 후 종료
                }
                else
                {
                    firstInPosAt = -1; // 다시 측정
                }

                await Task.Delay(POLL_MS, ct);
            }
            // 타임아웃 시 마지막 위치를 메시지에 포함(디버깅 편의)
            double last = double.NaN;
            try { last = _acsMotion.GetAPosition(axis); } catch { }
            throw new TimeoutException($"Axis {axis} move timeout. target={target:F3}, last={last:F3}");
        }
        private async Task MoveAbsAndWaitAsync(Axis axis, double targetPos, double inposTol, int timeoutMs, CancellationToken ct,
            double? velOverride = null,
            double? accOverride = null,
            double? decOverride = null)
        {
            // 1) ACS에 절대 이동 명령 (vel/acc/dec override 전달)
            await _acsMotion.MoveAbsAsync(axis, targetPos, velOverride, accOverride, decOverride);
            // 2) INPOS 비슷하게 현재 위치 기준으로 도달 여부 체크
            var t0 = Environment.TickCount;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                double cur = _acsMotion.GetAPosition(axis);
                if (!double.IsNaN(cur) && Math.Abs(cur - targetPos) <= inposTol) break;
                if (timeoutMs > 0 && Environment.TickCount - t0 > timeoutMs)
                    throw new TimeoutException($"Axis {axis} MoveAbs timeout (Target={targetPos}, Pos={cur}).");
                await Task.Delay(10, ct);
            }
        }
        private MotionProfile GetMotionProfileFromParam(Axis axis, string prefix)
        {
            var cur = _acsMotion.GetProfile(axis);   // 기본값: 현재 설정된 프로파일
            string axisStr = (axis == Axis.X) ? "X" : "Y";
            string velKey = $"{prefix}{axisStr}Speed";
            string accKey = $"{prefix}{axisStr}Acc";
            double vel = GetProcessParamDouble(velKey, cur.Velocity);
            double acc = GetProcessParamDouble(accKey, cur.Acceleration);
            double dec = acc;
            // 0 이하가 들어오면 기존 값 유지 (미설정/오타 방지)
            if (vel <= 0) vel = cur.Velocity;
            if (acc <= 0) acc = cur.Acceleration;
            if (dec <= 0) dec = cur.Deceleration;
            return new MotionProfile(vel, acc, dec);
        }
        private void LogMotionProfile(string prefix, MotionProfile px, MotionProfile py)
        {
            AddLog(lstSystem,
                $"[{prefix}][Profile] " +
                $"X(V={px.Velocity:F1}, A={px.Acceleration:F1}, D={px.Deceleration:F1}), " +
                $"Y(V={py.Velocity:F1}, A={py.Acceleration:F1}, D={py.Deceleration:F1})");
        }
        private async Task<bool> AreAxesSettledAsync(
            double targetX, double targetY,
            int quickCheckTimeoutMs = 2000,               // “정착 여부 확인”만 위한 짧은 타임아웃
            CancellationToken ct = default)
        {
            async Task<bool> CheckAsync(Axis ax, double target, double tol)
            {
                try
                {
                    await WaitAxisInPositionAsync(ax, target, tol, quickCheckTimeoutMs, ct);
                    return true;
                }
                catch { return false; }
            }

            var ok = await Task.WhenAll(
                CheckAsync(Axis.X, targetX, X_INPOS_TOL),
                CheckAsync(Axis.Y, targetY, Y_INPOS_TOL)
            );
            return ok[0] && ok[1];
        }
        private async Task MoveAxesAbsAsync(double reviewX, double mainY)
        {
            try
            {
                await _acsMotion.ServoOnAsync(Axis.X);
                await _acsMotion.ServoOnAsync(Axis.Y);
                var tx = _acsMotion.MoveAbsAsync(Axis.X, reviewX); // Review X
                var ty = _acsMotion.MoveAbsAsync(Axis.Y, mainY);   // Main   Y
                await Task.WhenAll(tx, ty);

                AddLog(lstRecipeScanLog, $"[ACS] MoveAbs → ReviewX:{reviewX:F3}, MainY:{mainY:F3}");
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][ACS MoveAbs] " + ex.Message);
                try { await _acsMotion.StopAsync(Axis.X); await _acsMotion.StopAsync(Axis.Y); } catch { }
            }
        }
        #endregion

        #region ======= Flying Scan Process 동작 관련 함수 =======
        private async Task<ScanRunState> QueryScanStatusAsync(int scanNo, int timeoutMs = 5000)
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) return ScanRunState.Idle;

            var ack = await StageWin.Driver.Network.Packets.ScanRpc.Request_ScanStatusAsync(
                scan, timeoutMs: timeoutMs);
            var key = (ScanRunState)ack.nArrScanStatus[scanNo];
            return key;
        }
        private async Task SendProcessScanningAsync(OpticOperationForm.ProcessScanningParams p)
        {
            //if (!EnsureSemiOrAuto("Process Scanning")) return;         // Test KJW

            var scan = GetScanSessionOrWarn();
            if (scan == null) return;

            // ===== 로그/성능 옵션 =====
            const int LOG_HEAD_N = 30;     // 앞쪽 샘플
            const int LOG_TAIL_N = 10;     // 뒤쪽 샘플
            const int LOG_EVERY_N = 0;     // (옵션) 중간 샘플링: 0이면 비활성, 예: 50이면 50개마다 1개 찍기
            const int LOG_MAX_CHARS = 120_000; // 로그 폭주 방지(문자수 제한). 넘으면 잘라서 출력.
            const bool LOG_TOOL_PARAMS = true; // Tool Param까지 찍을지
            const bool LOG_OFFSETS = true;     // Offset까지 찍을지

            try
            {
                int cnt = p.DrawList?.Length ?? 0;

                // 헤더 로그 (1회)
                AddLog(lstRecipeScanLog,
                    $"[1009] ProcessScanning → scanNo={p.ScanNo}, count={cnt}, motor=({p.MotorX:F3},{p.MotorY:F3})");

                // DrawList 로깅(샘플링) - UI 느려지는 주범 제거
                if (p.DrawList != null && p.DrawList.Length > 0)
                {
                    var sb = new System.Text.StringBuilder(capacity: Math.Min(LOG_MAX_CHARS, 16_384));

                    // 간단 통계(빠르게)
                    double minX = double.PositiveInfinity, minY = double.PositiveInfinity;
                    double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity;
                    int nonZeroOfs = 0;

                    // 통계 계산도 너무 비싸면(수백만) 샘플로 바꿀 수 있지만,
                    // 보통 DrawList가 수천 단위면 이 정도는 OK.
                    for (int i = 0; i < p.DrawList.Length; i++)
                    {
                        var d = p.DrawList[i];
                        if (d.dMarkX < minX) minX = d.dMarkX;
                        if (d.dMarkY < minY) minY = d.dMarkY;
                        if (d.dMarkX > maxX) maxX = d.dMarkX;
                        if (d.dMarkY > maxY) maxY = d.dMarkY;

                        if (LOG_OFFSETS && (Math.Abs(d.dOffsetX) > 1e-12 || Math.Abs(d.dOffsetY) > 1e-12))
                            nonZeroOfs++;
                    }

                    sb.AppendLine("[1009] ---- DrawList SUMMARY ----");
                    sb.AppendLine($"[1009] MarkRange X=[{minX:F3}..{maxX:F3}]  Y=[{minY:F3}..{maxY:F3}]");
                    if (LOG_OFFSETS)
                        sb.AppendLine($"[1009] OffsetNonZero={nonZeroOfs}/{cnt}");
                    sb.AppendLine($"[1009] LogSample: head={LOG_HEAD_N}, tail={LOG_TAIL_N}" +
                                  (LOG_EVERY_N > 0 ? $", every={LOG_EVERY_N}" : ""));
                    sb.AppendLine("[1009] ---- DrawList BEGIN (SAMPLED) ----");

                    int headN = Math.Min(LOG_HEAD_N, cnt);
                    int tailN = Math.Min(LOG_TAIL_N, Math.Max(0, cnt - headN));
                    int tailStart = Math.Max(headN, cnt - tailN);

                    // 1) HEAD
                    for (int i = 0; i < headN; i++)
                    {
                        AppendDrawLogLine(sb, p.DrawList[i], i, LOG_TOOL_PARAMS, LOG_OFFSETS);
                        if (sb.Length >= LOG_MAX_CHARS) break;
                    }

                    // 2) MIDDLE (optional every N)
                    if (LOG_EVERY_N > 0 && sb.Length < LOG_MAX_CHARS && tailStart > headN)
                    {
                        int midCount = 0;
                        for (int i = headN; i < tailStart; i++)
                        {
                            if ((i % LOG_EVERY_N) != 0) continue;
                            AppendDrawLogLine(sb, p.DrawList[i], i, LOG_TOOL_PARAMS, LOG_OFFSETS);
                            midCount++;
                            if (sb.Length >= LOG_MAX_CHARS) break;
                        }

                        if (midCount > 0 && sb.Length < LOG_MAX_CHARS)
                            sb.AppendLine($"[1009] ... (middle sampled {midCount} items) ...");
                    }
                    else if (tailStart > headN && sb.Length < LOG_MAX_CHARS)
                    {
                        sb.AppendLine($"[1009] ... (middle skipped {tailStart - headN} items) ...");
                    }

                    // 3) TAIL
                    if (sb.Length < LOG_MAX_CHARS)
                    {
                        for (int i = tailStart; i < cnt; i++)
                        {
                            AppendDrawLogLine(sb, p.DrawList[i], i, LOG_TOOL_PARAMS, LOG_OFFSETS);
                            if (sb.Length >= LOG_MAX_CHARS) break;
                        }
                    }

                    if (sb.Length >= LOG_MAX_CHARS)
                        sb.AppendLine("[1009] ... (log truncated: too many chars) ...");

                    sb.AppendLine("[1009] ---- DrawList END ----");

                    // AddLog를 '한 번만' 호출 (UI 업데이트 비용 극소화)
                    AddLog(lstRecipeScanLog, sb.ToString());
                }

                // 실제 RPC 호출
                var ack = await StageWin.Driver.Network.Packets.ScanRpc.Request_ProcessScanningAsync(
                    scan,
                    p.ScanNo,
                    p.DrawList,
                    p.MotorX,
                    p.MotorY,
                    timeoutMs: 120000);

                bool ok = (ack.nScanNo == p.ScanNo);
                if (ok)
                    AddLog(lstRecipeScanLog, "[1009] ProcessScanning OK (echo scanNo match)");
                else
                    AddLog(lstRecipeScanLog, $"[1009][W] echo mismatch (rsp={ack.nScanNo}, req={p.ScanNo})");
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, "[W][1009] 타임아웃");
                MessageBox.Show(this, "Process Scanning 타임아웃", "Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                try { await SendScanStopAsync(p.ScanNo); } catch { }
                // 폴링 중단. -PGT
                try { _procCts?.Cancel(); } catch { }
                throw;
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][1009] " + ex.Message);
                MessageBox.Show(this, "Process Scanning 실패: " + ex.Message, "Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try { await SendScanStopAsync(p.ScanNo); } catch { }
                //폴링 중단. -PGT
                try { _procCts?.Cancel(); } catch { }
                throw;
            }
        }

        // 로그 한 줄 생성: StringBuilder에 붙임 (AddLog 여러 번 호출 방지)
        private static void AppendDrawLogLine(
            System.Text.StringBuilder sb,
            NetCommon.ST_DRAW_DATA_LIST d,
            int idx0,
            bool logToolParams,
            bool logOffsets)
        {
            sb.Append("[1009] #");
            sb.Append(idx0.ToString("D4"));
            sb.Append(" ");
            sb.Append("R"); sb.Append(d.nRowNo);
            sb.Append("C"); sb.Append(d.nColNo);
            sb.Append(" ");
            sb.Append("Mark=(");
            sb.Append(d.dMarkX.ToString("F3"));
            sb.Append(",");
            sb.Append(d.dMarkY.ToString("F3"));
            sb.Append(")");

            if (logOffsets)
            {
                sb.Append(" Ofs=(");
                sb.Append(d.dOffsetX.ToString("F5"));
                sb.Append(",");
                sb.Append(d.dOffsetY.ToString("F5"));
                sb.Append(")");
            }

            if (logToolParams)
            {
                sb.Append(" Tool(");
                sb.Append("Pwr=");
                sb.Append(d.toolParam.dPower.ToString("F2"));
                sb.Append(", Freq=");
                sb.Append(d.toolParam.dFreq.ToString("F1"));
                sb.Append(", Vel=");
                sb.Append(d.toolParam.dProcessSpeed.ToString("F3"));
                sb.Append(", Att=");
                sb.Append(d.toolParam.dAttPos.ToString("F3"));
                sb.Append(")");
            }

            sb.AppendLine();
        }
        private async Task<bool> WaitForScanStateAsync(int scanNo, ScanRunState desired,
                                               int timeoutMs, int pollMs = 150,
                                               CancellationToken ct = default)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int perReqTimeout = Math.Max(200, Math.Min(1000, pollMs * 3)); // 200~1000ms
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                ct.ThrowIfCancellationRequested();
                var st = await QueryScanStatusAsync(scanNo, timeoutMs: perReqTimeout);
                if (st == desired) return true;
                await Task.Delay(pollMs, ct);
            }
            return false;
        }
        private static double GetScanReadyOffset()
        {
            var v = AppConfig.Current?.ScanFlyingReadyOffset ?? 0;
            if (double.IsNaN(v) || double.IsInfinity(v)) return 0;
            return Math.Abs(v);
        }
        private static double GetLineConstCoordFromStagePoints(NetCommon.ST_DRAW_DATA_LIST[] baseAll, int[] stageIndices, bool yMajor, Action<string> log = null)
        {
            var vals = stageIndices
                .Select(i => yMajor ? baseAll[i].dMarkX : baseAll[i].dMarkY)
                .ToArray();

            if (vals.Length == 0) return 0;

            double avg = vals.Average();
            double min = vals.Min();
            double max = vals.Max();
            double span = max - min;

            // 라인인데 값이 퍼져있으면 데이터 생성/그룹핑이 잘못된 신호라 로그 남겨두는게 좋음
            if (log != null && span > 1e-6)   // 필요하면 tol을 0.001 같은 실장비 수준으로 키워도 됨
                log($"[SEQ][WARN] LineConstCoord spread detected: span={span:F6} (min={min:F6}, max={max:F6}, avg={avg:F6})");
            return avg;
        }
        // 한 번의 배치(개별 홀 or 여러 홀 묶음) 실행
        private async Task RunSingleBatchAsync(int scanNo, NetCommon.ST_DRAW_DATA_LIST[] payload,
            double startX, double startY, double endX, double endY, CancellationToken ct,
            double? velXOverride = null, double? velYOverride = null)
        {
            // [입력 주의] startX/startY, endX/endY는 호출부에서 ReadyOffset까지 반영된 좌표입니다.
            // 1) 시작 위치로 이동
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, startX, X_INPOS_TOL, 20000, ct,
                velOverride:velXOverride);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, startY, Y_INPOS_TOL, 20000, ct,
                velOverride: velYOverride);
            await Task.WhenAll(t1, t2);
            AddLog(lstRecipeScanLog, $"[SEQ] Move to start ({startX:F3},{startY:F3})");

            // 2) Idle 확인
            bool idle = await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 1200000, ct: ct);
            if (!idle) throw new TimeoutException("ScanStatus(IDLE) 대기 타임아웃");

            // 3) 1009 송신 (가공 준비)
            await SendProcessScanningAsync(new OpticOperationForm.ProcessScanningParams
            {
                ScanNo = scanNo,
                MotorX = startX,
                MotorY = startY,
                DrawList = payload
            });

            // 4) Run 진입 확인
            //await Task.Delay(1000, ct);
            bool run = await WaitForScanStateAsync(scanNo, ScanRunState.Run, timeoutMs: 120000000, ct: ct);
            if (!run) throw new TimeoutException("ScanStatus(RUN) 진입 타임아웃");

            // 5) 마지막 좌표까지 이동(동시)
            var mx = MoveAbsAndWait_SeqAsync(Axis.X, endX, X_INPOS_TOL, 60000, ct,
                velOverride: velXOverride);
            var my = MoveAbsAndWait_SeqAsync(Axis.Y, endY, Y_INPOS_TOL, 60000, ct,
                velOverride: velYOverride);
            await Task.WhenAll(mx, my);
            AddLog(lstRecipeScanLog, $"[SEQ] Move to end   ({endX:F3},{endY:F3})");

            // 6) Idle 복귀 확인(가공 완료)
            bool idle2 = await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 1800000, ct: ct);
            if (!idle2) throw new TimeoutException("Scan 완료(IDLE 복귀) 타임아웃");
            AddLog(lstRecipeScanLog, $"[MOF] On the flying Process Complete!!");
        }

        // 시퀀스 전체 실행 (Step-by-Step / On-the-Fly)
        private async Task RunProcessSequenceAsync(OpticOperationForm.ProcessRunPlan plan)
        {
            //if (!EnsureSemiOrAuto("Process Sequence")) return;        // Test KJW

            _procCts?.Cancel();
            _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;

            try
            {
                var all = plan.All;
                int start = Math.Max(0, plan.StartIndex);
                int holesPerLine = Math.Max(1, plan.HolesPerLine);
                int vecPerStage = Math.Max(1, plan.VecPerStage);
                int totalItems = all?.Length ?? 0;

                var order = plan.Order ?? Array.Empty<(int Row, int Col)>();
                bool yMajor = plan.IsYMajor;
                double ready = GetScanReadyOffset();

                int stageCount = (vecPerStage > 0) ? totalItems / vecPerStage : totalItems;
                if (stageCount <= 0 && order.Length > 0) stageCount = order.Length;

                Func<int, int> groupKeyOf = (idx) =>
                {
                    // 1) Draw 인덱스 → Stage 인덱스
                    int stageIdx = (vecPerStage > 0) ? (idx / vecPerStage) : idx;  // 0-based

                    // 2) ScanOrder 가 Stage 기준으로 들어온 경우
                    if (order.Length > stageIdx && stageIdx >= 0)
                    {
                        return yMajor ? order[stageIdx].Row : order[stageIdx].Col;
                    }

                    // 3) Fallback: order 가 없을 때 단순 (Line, Hole) 계산
                    if (yMajor)
                    {
                        return (stageIdx / holesPerLine) + 1;   // Row = StageIdx / HolesPerLine + 1
                    }
                    else
                    {
                        // Col = StageIdx / Lines + 1
                        int lines = Math.Max(1, plan.Lines > 0 ? plan.Lines : (stageCount / Math.Max(1, holesPerLine)));
                        return (stageIdx / Math.Max(1, lines)) + 1;
                    }
                };

                var groups = new Dictionary<int, List<int>>();
                for (int i = 0; i < totalItems; i++)
                {
                    int key = groupKeyOf(i);
                    if (!groups.TryGetValue(key, out var list))
                    {
                        list = new List<int>();
                        groups[key] = list;
                    }
                    list.Add(i);
                }

                var groupKeys = groups.Keys.OrderBy(k => k).ToList();

                int startGroupKey = groupKeyOf(start);
                int startGroupIndex = groupKeys.IndexOf(startGroupKey);
                if (startGroupIndex < 0) startGroupIndex = 0;

                if (plan.Mode == OpticOperationForm.ProcessMode.StepByStep)
                {
                    // === Step-by-Step ===
                    // - Backward: 1,2,3,4 (오름차순)
                    // - Forward : 4,3,2,1 (내림차순)
                    // - StepAllWholeLine이면 Line#1 -> Line#2 -> ... 전체 라인

                    bool isForward = (plan.Direction == OpticOperationForm.TravelDirection.Forward);

                    Func<NetCommon.ST_DRAW_DATA_LIST, double> majorCoord = yMajor ?
                        (Func<NetCommon.ST_DRAW_DATA_LIST, double>)(p => p.dMarkY) : (p => p.dMarkX);

                    // 전체 라인 루프 범위 결정
                    int gStart = plan.StepAllWholeLine ? 0 : startGroupIndex;
                    int gEndExclusive = plan.StepAllWholeLine ? groupKeys.Count : (startGroupIndex + 1);

                    for (int gi = gStart; gi < gEndExclusive; gi++)
                    {
                        ct.ThrowIfCancellationRequested();

                        int gKey = groupKeys[gi];
                        if (!groups.TryGetValue(gKey, out var curGroup) || curGroup == null || curGroup.Count == 0)
                            continue;

                        // 라인 내부 정렬(요구사항 반영)
                        // Forward : 4->1 (내림차순)
                        // Backward: 1->4 (오름차순)
                        var sortedIdx = isForward
                            ? curGroup.OrderBy(i => majorCoord(all[i])).ToList()
                            : curGroup.OrderByDescending(i => majorCoord(all[i])).ToList();

                        // 시작점/개수
                        int posInSorted;

                        if (plan.StepAllWholeLine)
                        {
                            // 전체라인 모드: 클릭셀 무시하고 라인 처음부터
                            posInSorted = 0;
                        }
                        else
                        {
                            // 기존 동작 유지:
                            // - StepWholeLine이면 클릭셀 무시하고 라인 처음부터
                            // - 아니면 클릭셀 위치부터 StepCount만
                            posInSorted = sortedIdx.IndexOf(start);
                            if (posInSorted < 0) posInSorted = 0;
                            if (plan.StepWholeLine) posInSorted = 0;
                        }

                        int remainInSorted = Math.Max(1, sortedIdx.Count - posInSorted);

                        int countToDo;
                        if (plan.StepAllWholeLine || plan.StepWholeLine)
                            countToDo = remainInSorted;  // 라인 전체
                        else
                            countToDo = Math.Min(plan.StepCount, remainInSorted);

                        AddLog(lstRecipeScanLog,
                            $"[STEP] Line={gKey} StartPos={posInSorted}, Count={countToDo}, Dir={(isForward ? "Forward(4->1)" : "Backward(1->4)")}, AllWhole={plan.StepAllWholeLine}");

                        // 기존과 동일: 홀 단위 RunSingleBatchAsync 반복
                        for (int k = 0; k < countToDo; k++)
                        {
                            ct.ThrowIfCancellationRequested();

                            int idx = sortedIdx[posInSorted + k];
                            var p = all[idx];

                            await RunSingleBatchAsync(
                                plan.ScanNo,
                                new[] { p },
                                startX: p.dMarkX,
                                startY: p.dMarkY,
                                endX: p.dMarkX,
                                endY: p.dMarkY,
                                ct: ct,
                                velXOverride: (plan.ProcessSpeedX > 0) ? plan.ProcessSpeedX : (double?)null,
                                velYOverride: (plan.ProcessSpeedY > 0) ? plan.ProcessSpeedY : (double?)null);

                            AddLog(lstRecipeScanLog,
                                $"[STEP] Line={gKey} {k + 1}/{countToDo} done @({p.dMarkX + p.dOffsetX:F3},{p.dMarkY + p.dOffsetY:F3})");
                        }
                    }

                    return; // StepByStep -> OnTheFly로 내려가지 않게
                }
                else
                {
                    // ============ On-The-Fly 모드 ============
                    yMajor = plan.IsYMajor;
                    ready = GetScanReadyOffset();

                    // 라인 내 진행 방향 (Y 또는 X 방향)만 결정
                    bool forwardDir = (plan.Direction == OpticOperationForm.TravelDirection.Forward);
                    int totalGroups = groupKeys.Count;

                    // 라인 순서는 항상 groupKeys 오름차순으로만 진행
                    // 현재 선택한 라인 인덱스부터 오른쪽(아래)으로만 확장
                    int maxLinesFromHere = totalGroups - startGroupIndex;
                    if (maxLinesFromHere <= 0)
                    {
                        AddLog(lstRecipeScanLog, $"[SEQ][OnTheFly] No lines to run from StartGroupIndex={startGroupIndex} (TotalGroups={totalGroups})");
                        return;
                    }

                    // 사용자가 요청한 FlyLineCount 와 실제 가능한 라인 수 중 작은 값 사용
                    int linesToRun = Math.Max(1, Math.Min(plan.FlyLineCount, maxLinesFromHere));

                    // 라인 진행 방향은 항상 +1 (라인 번호 증가)
                    const int dirStep = +1;
                    int gIndex = startGroupIndex;
                    int linesDone = 0;

                    AddLog(lstRecipeScanLog,
                        $"[SEQ][OnTheFly] StartGroupKey={groupKeys[startGroupIndex]}, " +
                        $"StartGroupIndex={startGroupIndex}, Direction={plan.Direction}, " +
                        $"RequestFlyLines={plan.FlyLineCount}, ActualLinesToRun={linesToRun}, " +
                        $"TotalGroups={totalGroups}");

                    while (gIndex >= 0 && gIndex < totalGroups && linesDone < linesToRun)
                    {
                        int gKey = groupKeys[gIndex];
                        var idxList = groups[gKey];     // 이 라인에 속한 Draw index 목록
                        var stageIndices = idxList.ToArray();

                        if (stageIndices.Length == 0)
                        {
                            AddLog(lstRecipeScanLog, $"[SEQ][Line={gKey}] stageIndices empty → skip");
                            gIndex += dirStep;
                            continue;
                        }

                        // 이 라인의 payload (Vector 확장 포함한 최종 리스트)
                        int dummyVecRow = 0;
                        var payload = _opticForm.ExpandVectorsForLine(plan, stageIndices, ref dummyVecRow);
                        if (payload == null || payload.Length == 0)
                        {
                            AddLog(lstRecipeScanLog, $"[SEQ][Line={gKey}] payload empty → skip");
                            gIndex += dirStep;
                            continue;
                        }

                        // =========================================================
                        // OneWay 최소 수정
                        // - Auto/Semi에서 plan.Pattern = OneWay로 강제되어 있음
                        // - 현재 라인의 payload 방향이 Stage 주행 방향과 반대이면 Reverse만 수행
                        // - 좌표 재계산 / 재정렬 / RowCol 재부여 없음
                        // =========================================================
                        if (plan.Pattern == OpticOperationForm.FlyPattern.OneWay && payload.Length > 1)
                        {
                            bool reversePayload = false;

                            if (yMajor)
                            {
                                double firstY = payload[0].dMarkY;
                                double lastY = payload[payload.Length - 1].dMarkY;

                                if (plan.Direction == OpticOperationForm.TravelDirection.Backward)
                                {
                                    // Backward는 Y 큰 값 → 작은 값이어야 함
                                    if (firstY < lastY) reversePayload = true;
                                }
                                else
                                {
                                    // Forward는 Y 작은 값 → 큰 값이어야 함
                                    if (firstY > lastY) reversePayload = true;
                                }
                            }
                            else
                            {
                                double firstX = payload[0].dMarkX;
                                double lastX = payload[payload.Length - 1].dMarkX;

                                if (plan.Direction == OpticOperationForm.TravelDirection.Backward)
                                {
                                    // Backward는 X 큰 값 → 작은 값이어야 함
                                    if (firstX < lastX) reversePayload = true;
                                }
                                else
                                {
                                    // Forward는 X 작은 값 → 큰 값이어야 함
                                    if (firstX > lastX) reversePayload = true;
                                }
                            }

                            if (reversePayload)
                            {
                                Array.Reverse(payload);
                                AddLog(lstRecipeScanLog, $"[SEQ][Line={gKey}] OneWay payload reverse applied.");
                            }
                        }

                        // ReadyOffset / startX,startY,endX,endY 계산
                        double startX, startY, endX, endY;
                        if (yMajor)
                        {
                            // X는 "Stage Point" 기준으로 산출 (payload가 아니라 base all)
                            double lineX = GetLineConstCoordFromStagePoints(
                                all,
                                stageIndices,
                                yMajor: true,
                                log: s => AddLog(lstRecipeScanLog, s));

                            double minY = payload.Min(p => p.dMarkY);
                            double maxY = payload.Max(p => p.dMarkY);

                            startX = lineX;
                            endX = lineX;

                            if (forwardDir)
                            {
                                startY = minY - ready;
                                endY = maxY + ready;
                            }
                            else
                            {
                                startY = maxY + ready;
                                endY = minY - ready;
                            }
                        }
                        else
                        {
                            // Y는 "Stage Point" 기준으로 산출
                            double lineY = GetLineConstCoordFromStagePoints(
                                all,
                                stageIndices,
                                yMajor: false,
                                log: s => AddLog(lstRecipeScanLog, s));

                            double minX = payload.Min(p => p.dMarkX);
                            double maxX = payload.Max(p => p.dMarkX);

                            startY = lineY;
                            endY = lineY;

                            if (forwardDir)
                            {
                                startX = minX - ready;
                                endX = maxX + ready;
                            }
                            else
                            {
                                startX = maxX + ready;
                                endX = minX - ready;
                            }
                        }

                        AddLog(lstRecipeScanLog,
                            $"[SEQ][Line={gKey}] OnTheFly: Start=({startX:F3},{startY:F3}) → End=({endX:F3},{endY:F3}), " +
                            $"LineIndex={gIndex}, Done={linesDone + 1}/{linesToRun}");

                        await RunSingleBatchAsync(
                            plan.ScanNo,
                            payload,
                            startX,
                            startY,
                            endX,
                            endY,
                            ct,
                            velXOverride: (plan.ProcessSpeedX > 0) ? plan.ProcessSpeedX : (double?)null,
                            velYOverride: (plan.ProcessSpeedY > 0) ? plan.ProcessSpeedY : (double?)null);

                        linesDone++;
                        gIndex += dirStep;

                        AddLog(lstRecipeScanLog, $"[SEQ][OnTheFly] Finished. LinesDone={linesDone}/{linesToRun}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AddLog(lstRecipeScanLog, "[SEQ] 사용자 정지");
            }
            catch (TimeoutException ex)
            {
                AddLog(lstRecipeScanLog, "[W][SEQ] " + ex.Message);
                MessageBox.Show(this, ex.Message, "Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][SEQ] " + ex.Message);
                MessageBox.Show(this, "Process 실패: " + ex.Message, "Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ======= Flying Vision 동작 관련 함수 =======
        private async Task RunFlyingVision_ByPlansAsync(RecipeForm.FlyingVisionPartialPlan[] plans)
        {
            //if (!EnsureSemiOrAuto("Flying Vision")) return;       // Test KJW

            _procCts?.Cancel();
            _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            _fvAbortByError = false;

            try
            {
                if (plans == null || plans.Length == 0)
                {
                    MessageBox.Show(this, "실행할 계획이 없습니다.", "Flying Vision",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int okLines = 0;
                for (int i = 0; i < plans.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var p = plans[i];
                    AddLog(lstRecipeScanLog,
                        $"[FV] LinePlan {i + 1}/{plans.Length} → Row={p.Row}, Start={p.StartCol}, Cnt={p.Count}, " +
                        $"Start=({p.StartX:F3},{p.StartY:F3}), End=({p.EndX:F3},{p.EndY:F3}), " +
                        $"Major={(p.YMajor ? "Y" : "X")}, Dir={(p.ForwardThis ? "FWD" : "BWD")}");

                    await RunFlyingVisionPartialApplyAsync(p, p.YMajor, p.ForwardThis);
                    ct.ThrowIfCancellationRequested();
                    okLines++;
                }
            }
            catch (OperationCanceledException)
            {
                AddLog(lstRecipeScanLog, _fvAbortByError ? "[FV] 오류로 중단" : "[FV] 사용자 정지");
                if (_fvAbortByError) return;
            }
            catch (TimeoutException ex)
            {
                AddLog(lstRecipeScanLog, "[W][FV] " + ex.Message);
                MessageBox.Show(this, ex.Message, "Flying Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][FV] " + ex.Message);
                MessageBox.Show(this, "Flying Vision 실패: " + ex.Message,
                    "Flying Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Task RunFlyingVisionPartialApplyAsync(RecipeForm.FlyingVisionPartialPlan plan, bool? yMajorOverride = null, bool? forwardOverride = null)
            => RunFlyingVisionPartialApplyInternalAsync(plan, yMajorOverride, forwardOverride, _procCts?.Token ?? CancellationToken.None);
        private async Task RunFlyingVisionPartialApplyInternalAsync(RecipeForm.FlyingVisionPartialPlan plan,
            bool? yMajorOverride, bool? forwardOverride, CancellationToken ct)
        {
            var doc = _recipeForm?.CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            var rf = doc.Parameters?.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            var cfg = AppConfig.Current ?? new AppConfig();

            try
            {
                // 0) 트리거 릴레이 초기화 & 축 Servo ON
                await TriggerPulseOutputAsync(cfg.ReviewTrigOutXName, cfg.ReviewTrigOutYName, cfg.ReviewTrigPulseMs, ct, false, "ReviewTrigger");
                await _acsMotion.ServoOnAsync(Axis.X);
                await _acsMotion.ServoOnAsync(Axis.Y);

                // 1) 진행축/방향(override 우선)
                bool yMajor = yMajorOverride ?? plan.YMajor;
                bool forward = forwardOverride ?? plan.ForwardThis;

                // 2) "선택셀 무시"하고 라인 전체 구성
                int totalLines = Math.Max(1, doc.Parameters.Lines);
                int holesPerLine = Math.Max(1, doc.Parameters.HolesPerLine);

                RecipeForm.ReviewRow[] segment;
                int lineIndex;  // yMajor=true => Row, yMajor=false => Col
                int vecC = Math.Max(0, plan.VecC);
                int vecRows = Math.Max(1, doc.Parameters.VectorRows);
                if (yMajor)
                {
                    lineIndex = Math.Max(1, Math.Min(totalLines, plan.Row));
                    // VectorRow 전체
                    segment = _recipeForm.GetLineReviewRows_AllVecR_ByVecC(
                        lineIndex, vecC,
                        startCol: 1,
                        count: holesPerLine);
                }
                else
                {
                    lineIndex = Math.Max(1, Math.Min(holesPerLine, plan.Row));
                    segment = _recipeForm.GetColumnReviewRows_ByVecC(lineIndex, vecC, 1, totalLines);
                }
                if (segment.Length == 0) throw new InvalidOperationException($"라인 좌표가 없습니다. (Line={lineIndex}, VecC={vecC})");

                // 3) 라인 내 정렬 (주행축 기준 + 방향 반영)
                RecipeForm.ReviewRow[] ordered =
                    yMajor
                    ? (forward ? segment.OrderBy(r => r.StageY).ToArray() : segment.OrderByDescending(r => r.StageY).ToArray())
                    : (forward ? segment.OrderBy(r => r.StageX).ToArray() : segment.OrderByDescending(r => r.StageX).ToArray());
                var first = ordered.First();
                var last = ordered.Last();

                // 4) ReadyOffset 적용
                double rawStartX = first.StageX, rawStartY = first.StageY;
                double rawEndX = last.StageX, rawEndY = last.StageY;

                double ready = Math.Max(0, Math.Abs(cfg.ReviewFlyingReadyOffset));
                var (startX, startY, endX, endY) =
                    ApplyReadyOffsetToSpan(yMajor, forward, rawStartX, rawStartY, rawEndX, rawEndY, ready);

                // 5) 시작점으로 진입
                await RunFlyingVision_MoveStartAsync(startX, startY, ct);
                AddLog(lstRecipeScanLog,
                    $"[FV] PreMove → ({startX:F3},{startY:F3})  ready:{ready:F3}  raw=({rawStartX:F3},{rawStartY:F3})  axis={(yMajor ? "Y" : "X")} dir={(forward ? "+" : "-")}");

                // 6) 트리거 등록 (라인 전체) + Vision Ready
                int nTrig = await StartPegLineTriggerAsync(yMajor, ordered, forward, ct);
                await RunFlyingVision_ReadyAsync(nTrig, plan.Row, plan.VecC, ct);
                AddLog(lstRecipeScanLog, $"[FV] Trigger Assigned(LineFull), Count={nTrig}");
                await Task.Delay(100, ct);

                // 7) 끝점까지 통과
                await RunFlyingVision_MoveEndAsync(endX, endY, ct);
                AddLog(lstRecipeScanLog, $"[FV] PostMove → ({endX:F3},{endY:F3})  ready:{ready:F3}  raw=({rawEndX:F3},{rawEndY:F3})");

                // 8) 종료 정리
                await RunFlyingVision_MoveDoneAsync(ct);

                // 9) Vision 결과 수집/매핑
                var vision = GetVisionSessionOrWarn() ?? throw new InvalidOperationException("Vision 연결 없음");
                AddLog(lstRecipeScanLog, $"[3003] LINE MARK FIND 요청 (expect={nTrig})");
                var list = await StageWin.Driver.Network.Packets.VisionRpc.Request_MarkFindLineAsync(vision, nTrig, timeoutMs: 150000);

                if (list == null || list.Length == 0) throw new Exception("라인 마크 결과가 비어 있습니다.");

                int expected = nTrig;                 // 라인 트리거 개수
                int actual = list.Length;

                if (actual != expected)
                    AddLog(lstRecipeScanLog, $"[W][FV] Vision 결과 개수 불일치: expected={expected}, actual={actual} → 부족분은 NG로 채움");

                // 로그/셀 매칭의 기준은 "실제 모션/트리거 순서 = ordered"
                if (ordered == null || ordered.Length == 0)
                    throw new InvalidOperationException("ordered 라인 배열이 비어 있습니다.");

                if (ordered.Length != expected)
                    AddLog(lstRecipeScanLog, $"[W][FV] ordered.Length({ordered.Length}) != expected({expected}) (트리거/정렬 구성 확인 필요)");

                // mapped 배열 구성 (ApplyLineMarkResults에 그대로 전달)
                var mapped = new RecipeForm.MarkResult[expected];
                int vecCols = Math.Max(1, doc.Parameters.VectorCols);

                for (int i = 0; i < expected; i++)
                {
                    // 이 셀 정보가 곧 "지나간 순서"의 셀
                    var cell = (i < ordered.Length) ? ordered[i] : ordered.Last(); // 방어

                    int cellRow = cell.Row;
                    int cellCol = cell.Col;
                    int cellVecR = cell.VectorRow;
                    int cellVecC = cell.VectorCol;

                    // 로그에서 사용하고 싶은 R0C0 스타일(GlobalR/GlobalC)
                    int globalR = (cellCol - 1) * vecRows + cellVecR;              // 0-based
                    int globalC = (cellRow - 1) * vecCols + cellVecC;              // 0-based

                    RecipeForm.MarkResult m;
                    if (i < actual)
                    {
                        m = new RecipeForm.MarkResult
                        {
                            Result = list[i].nResult,
                            TargetX = list[i].dTargetX,
                            TargetY = list[i].dTargetY,
                            MarkX = list[i].dMarkX,
                            MarkY = list[i].dMarkY
                        };
                    }
                    else
                    {
                        // actual < expected 인 경우 list[i] 접근 금지 (OutOfRange 방지)
                        // Target은 기존 셀에 저장된 값이 있으면 쓰고(없으면 0), Mark는 NaN으로
                        m = new RecipeForm.MarkResult
                        {
                            Result = 0,
                            TargetX = cell.TargetX,
                            TargetY = cell.TargetY,
                            MarkX = double.NaN,
                            MarkY = double.NaN
                        };
                    }
                    mapped[i] = m;
                    // Eval() 호출해서 TolX/TolY 기준 Grade, Err 로그 출력
                    var (ex, ey, exMm, eyMm, gradeByTol) = RecipeForm.Eval(m.TargetX, m.TargetY, m.MarkX, m.MarkY, doc);
                    // "지나간 순서(i)" 기준으로 Cell을 찍는다
                    AddLog(lstRecipeScanLog,
                        $"[FV-RES] idx={i + 1}/{expected} " +
                        $"Cell(Row={cellRow},Col={cellCol},VecR={cellVecR},VecC={cellVecC},R{globalR}C{cellVecC}) " +
                        $"VisionResult={m.Result} " +
                        $"Target=({m.TargetX:F3},{m.TargetY:F3}) " +
                        $"Mark=({m.MarkX:F3},{m.MarkY:F3}) " +
                        $"Err=({ex:F3}/{exMm:F5},{ey:F3}/{eyMm:F5}) " +
                        $"Tol=({doc.Crit.TolX:F3},{doc.Crit.TolY:F3}) " +
                        $"Grade={gradeByTol}");
                }
                _recipeForm.ApplyLineMarkResults(plan.YMajor, plan.Row, plan.StartCol, plan.VecC, plan.ForwardThis, results: mapped);
                int okCnt = mapped.Count(x => x.Result == 1);
                AddLog(lstRecipeScanLog, $"[FV] Line 결과 {(yMajor ? "Y Dir" : "X Dir")}={plan.Row} : VisionOK={okCnt}, VisionNG={mapped.Length - okCnt}");
            }
            catch (OperationCanceledException) { AddLog(lstRecipeScanLog, "[FV] 사용자 정지"); }
            catch (TimeoutException ex)
            {
                AddLog(lstRecipeScanLog, "[W][FV] " + ex.Message);
                MessageBox.Show(this, ex.Message, "Flying Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _fvAbortByError = true;    
                _procCts?.Cancel();        
                return;
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][FV] " + ex.Message);
                MessageBox.Show(this, "Flying Vision 실패: " + ex.Message,
                    "Flying Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _fvAbortByError = true;  
                _procCts?.Cancel();      
                return;
            }
        }
        private async Task ClearAcsTriggerArrayAsync(string varName, int length)
        {
            if (length <= 0) length = 1;
            var zeros = new double[length];
            await AcsWriteGlobalVectorAsync(varName, zeros, startIndex: 0);
        }
        private Task AcsRunBufferAsync(int bufferNo)
        {
            var acsProg = _acsMotion as StageWin.Driver.Motion.IAcsPrograms;
            if (acsProg != null)
                return acsProg.RunBufferAsync(bufferNo);
            try
            {
                var mAsync = _acsMotion?.GetType().GetMethod("RunBufferAsync", new[] { typeof(int) })
                         ?? _acsMotion?.GetType().GetMethod("ExecBufferAsync", new[] { typeof(int) });
                if (mAsync != null) return (Task)mAsync.Invoke(_acsMotion, new object[] { bufferNo });

                var mSync = _acsMotion?.GetType().GetMethod("RunBuffer", new[] { typeof(int) })
                         ?? _acsMotion?.GetType().GetMethod("ExecBuffer", new[] { typeof(int) });
                if (mSync != null) { mSync.Invoke(_acsMotion, new object[] { bufferNo }); return Task.CompletedTask; }

                AddLog(lstRecipeScanLog, $"[W][ACS] RunBuffer 메서드를 찾을 수 없습니다. buf={bufferNo}");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][ACS RunBuffer] " + ex.Message);
                throw;
            }
        }
        private async Task TriggerPulseOutputAsync(string name1, string name2, int ms, CancellationToken ct, bool ready = true, string src = "Auto")
        {
            if (_ioForm == null) return;
            try
            {
                _ioForm.TryWriteOutputByName(name1, ready, src);
                _ioForm.TryWriteOutputByName(name2, false, src);
                await Task.Delay(ms, ct);
            }
            catch { }
        }
        private async Task AcsWriteVarAnyAsync(object value, string varName, int? bufNo = null, int? from1 = null, int? to1 = null, int? from2 = null, int? to2 = null)
        {
            var acsVars = _acsMotion as StageWin.Driver.Motion.IAcsVariables;
            if (acsVars == null)
                throw new NotSupportedException("ACS variable write is not supported by the current motion adapter.");

            var nBuf = bufNo.HasValue ? (ProgramBuffer)bufNo.Value : ProgramBuffer.ACSC_NONE;
            int f1 = from1 ?? Api.ACSC_NONE;
            int t1 = to1 ?? Api.ACSC_NONE;
            int f2 = from2 ?? Api.ACSC_NONE;
            int t2 = to2 ?? Api.ACSC_NONE;

            await acsVars.WriteVariableAsync(value, varName, nBuf, f1, t1, f2, t2);
        }
        // 편의 래퍼: 글로벌 벡터(0..N-1) 한방쓰기
        private Task AcsWriteGlobalVectorAsync(string varName, double[] vec, int startIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(varName))
                throw new ArgumentException("ACS 글로벌 변수명이 비어 있습니다.", nameof(varName));

            return AcsWriteVarAnyAsync(vec, varName,
                bufNo: null,
                from1: startIndex,
                to1: startIndex + vec.Length - 1,
                from2: Api.ACSC_NONE,
                to2: Api.ACSC_NONE);
        }
        private async Task<int> StartPegLineTriggerAsync(
            bool yMajor,
            RecipeForm.ReviewRow[] orderedLine,   // 이미 진행방향 반영된 주행 순서
            bool forwardThisLine,
            CancellationToken ct)
        {
            var cfg = AppConfig.Current ?? new AppConfig();

            // 1) 주행축 포지션 배열 생성 (주행 순서 그대로)
            double[] pos = yMajor
                ? orderedLine.Select(r => r.StageY).ToArray()
                : orderedLine.Select(r => r.StageX).ToArray();
            if (pos.Length == 0) throw new InvalidOperationException("라인 좌표가 없습니다.");

            // 2) 단조성 보강 (동일값 간 미세 델타)
            const double EPS = 1e-7;
            for (int i = 1; i < pos.Length; i++)
                if (Math.Abs(pos[i] - pos[i - 1]) < EPS) pos[i] = pos[i - 1] + (forwardThisLine ? +EPS : -EPS); // ← 역방향일 땐 음의 델타
            // 3) 변수명/버퍼 추출
            string trigVar = yMajor ? cfg.AcsTrigVarY : cfg.AcsTrigVarX;
            if (string.IsNullOrWhiteSpace(trigVar))
            {
                trigVar = yMajor ? "POS_ARRAY_Y1" : "POS_ARRAY_X1";
                if (yMajor) cfg.AcsTrigVarY = trigVar; else cfg.AcsTrigVarX = trigVar;
                AddLog(lstRecipeScanLog, $"[PEG] {(yMajor ? "Y" : "X")} 트리거 변수명 미설정 → '{trigVar}' 기본 사용");
                AppConfig.Save();
            }
            int bufNo = yMajor ? cfg.AcsBufferY : cfg.AcsBufferX;
            if (bufNo <= 0) bufNo = yMajor ? 13 : 14;

            // 4) 0으로 초기화 후 기록
            await ClearAcsTriggerArrayAsync(trigVar, 1024);
            await AcsWriteGlobalVectorAsync(trigVar, pos, startIndex: 0);

            // 5) 버퍼 실행(PEG/PSO Arm)
            await AcsRunBufferWithRetryAsync(bufNo, ct);

            AddLog(lstRecipeScanLog,
                $"[PEG][{(yMajor ? "Y" : "X")}] N={pos.Length}, Dir={(forwardThisLine ? "FWD(증가)" : "BWD(감소)")}, " +
                $"First={pos.First():F6}, Last={pos.Last():F6}, Buf={bufNo}, Var={trigVar}");

            // 6) 릴레이 펄스 (선택 축 채널 On)
            await TriggerPulseOutputAsync(
                yMajor ? cfg.ReviewTrigOutYName : cfg.ReviewTrigOutXName,
                yMajor ? cfg.ReviewTrigOutXName : cfg.ReviewTrigOutYName,
                cfg.ReviewTrigPulseMs, ct, true, "ReviewTrigger");

            return pos.Length;
        }
        // 아래 3개 보조함수는 RunFlyingVisionAsync의 각 블록을 그대로 쪼갠 버전
        private Task RunFlyingVision_MoveStartAsync(double x, double y, CancellationToken ct)
            => Task.WhenAll(
                MoveAbsAndWait_SeqAsync(Axis.X, x, X_INPOS_TOL, 20000, ct),
                MoveAbsAndWait_SeqAsync(Axis.Y, y, Y_INPOS_TOL, 20000, ct));
        private async Task RunFlyingVision_ReadyAsync(int count, int nLine, int nCol, CancellationToken ct)
        {
            var vision = GetVisionSessionOrWarn() ?? throw new InvalidOperationException("Vision 연결 없음");
            AddLog(lstRecipeScanLog, $"[3004] FLYING_READY → nDataCount={count}, nLine={nLine}, nCol={nCol}");

            bool ack = await StageWin.Driver.Network.Packets.VisionRpc
                           .Request_FlyingReadyAsync(vision, count, nLine, nCol, timeoutMs: 15000);

            if (!ack) throw new Exception("FLYING_READY ACK 실패");
            AddLog(lstRecipeScanLog, "[3004] FLYING_READY ACK OK");
        }
        private async Task RunFlyingVision_MoveDoneAsync(CancellationToken ct)
        {
            var vision = GetVisionSessionOrWarn();
            if (vision == null) throw new InvalidOperationException("Vision 연결이 없습니다.");

            AddLog(lstRecipeScanLog, "[3007] FLYING_MOVE_DONE ACK");
            bool ack = await StageWin.Driver.Network.Packets.VisionRpc
                            .Request_FlyingMoveDoneAsync(vision, timeoutMs: 15000);
            if (!ack) throw new Exception("FLYING_MOVE_DONE ACK 실패");
            AddLog(lstRecipeScanLog, "[3007] FLYING_MOVE_DONE ACK OK");
        }
        private Task RunFlyingVision_MoveEndAsync(double x, double y, CancellationToken ct)
            => Task.WhenAll(
                MoveAbsAndWait_SeqAsync(Axis.X, x, X_INPOS_TOL, 30000, ct),
                MoveAbsAndWait_SeqAsync(Axis.Y, y, Y_INPOS_TOL, 30000, ct));

        // ACS RunBuffer "첫 1회만" Busy 예외 방어용
        private readonly SemaphoreSlim _acsRunBufLock = new SemaphoreSlim(1, 1);
        private static bool IsAcsRunWhileProgramRunning(Exception ex)
        {
            while (ex != null)
            {
                if ((ex.Source ?? "").IndexOf("ACS.SPiiPlusNET", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    (ex.Message ?? "").IndexOf("Command cannot be executed while the program is running",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                if (ex is AggregateException ae && ae.InnerExceptions != null)
                {
                    foreach (var ie in ae.InnerExceptions)
                        if (IsAcsRunWhileProgramRunning(ie)) return true;
                }
                ex = ex.InnerException;
            }
            return false;
        }
        private async Task TryStopAcsBufferAsync(int bufNo)
        {
            try
            {
                object motion = _acsMotion;
                if (motion == null) return;
                
                var mAsync =     // Async 우선
                    motion.GetType().GetMethod("StopBufferAsync", new[] { typeof(int) }) ??
                    motion.GetType().GetMethod("HaltBufferAsync", new[] { typeof(int) }) ??
                    motion.GetType().GetMethod("KillBufferAsync", new[] { typeof(int) });
                if (mAsync != null)
                {
                    var t = mAsync.Invoke(motion, new object[] { bufNo }) as Task;
                    if (t != null) await t.ConfigureAwait(false);
                    return;
                }
                // Sync fallback
                var mSync =
                    motion.GetType().GetMethod("StopBuffer", new[] { typeof(int) }) ??
                    motion.GetType().GetMethod("HaltBuffer", new[] { typeof(int) }) ??
                    motion.GetType().GetMethod("KillBuffer", new[] { typeof(int) });
                mSync?.Invoke(motion, new object[] { bufNo });
            }
            catch { }
        }
        private async Task AcsRunBufferWithRetryAsync(int bufNo, CancellationToken ct)
        {
            // 동시에 여러 곳에서 RunBuffer를 치면 더 잘 터지므로, 최소 락 1개로 직렬화
            await _acsRunBufLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // 첫 실행 직후만 race가 걸리는 경우가 많아서 2~3회만 가볍게 재시도
                int[] delaysMs = { 100, 500, 1000 };
                Exception last = null;

                for (int i = 0; i < delaysMs.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    if (delaysMs[i] > 0) await Task.Delay(delaysMs[i], ct).ConfigureAwait(false);

                    try
                    {
                        await AcsRunBufferAsync(bufNo).ConfigureAwait(false);
                        return;
                    }
                    catch (Exception ex) when (IsAcsRunWhileProgramRunning(ex))
                    {
                        last = ex;
                        AddLog(lstRecipeScanLog,
                            $"[W][ACS] RunBuffer busy(Program running). buf={bufNo}, retry={i + 1}/{delaysMs.Length}, msg={ex.Message}");
                        // 가능하면 한번 정지/홀드 시도 (메서드 있으면만)
                        await TryStopAcsBufferAsync(bufNo).ConfigureAwait(false);
                    }
                }
                throw last ?? new Exception("ACS RunBuffer failed (unknown)");
            }
            finally { _acsRunBufLock.Release(); }
        }

        #endregion

        #region ======= 통신 및 로그 관련 함수 =======
        private void UI(Action a)
        {
            try
            {
                if (IsDisposed) return;
                if (!IsHandleCreated) return;
                if (InvokeRequired) BeginInvoke(new MethodInvoker(() => { if (!IsDisposed) a?.Invoke(); }));
                else a?.Invoke();
            }
            catch { /* ignore */ }
        }
        public void Info(string msg) { AddLog(lstLog, "[I] " + msg); }
        public void Warn(string msg) { AddLog(lstLog, "[W] " + msg); }
        public void Error(string msg, Exception ex = null) { AddLog(lstLog, "[E] " + msg + (ex != null ? " :: " + ex.Message : "")); }
        private const int MAX_LOG_LINES = 5000;
        private void AddLog(ListBox lb, string s)
        {
            // 1) UI 큐에 쌓기
            if (lb != null)
                _logQueue.Enqueue((lb, s));

            // 2) 파일/전역 Logger는 그대로 (UI와 독립)
            try
            {
                string line = $"[{lb?.Name ?? "Log"}] {s}";
                if (!string.IsNullOrEmpty(s) && s.Contains("[E]")) Logger.Error(line);
                else if (!string.IsNullOrEmpty(s) && (s.Contains("[W]") || s.Contains("TIMEOUT"))) Logger.Warn(line);
                else Logger.Info(line);
            }
            catch { /* logging 실패는 무시 */ }
        }
        private void LogFlushTimer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) return;

            // 한 번에 너무 많이 처리하면 또 버벅이므로 제한
            int maxPerTick = 50;
            int processed = 0;

            while (processed < maxPerTick && _logQueue.TryDequeue(out var item))
            {
                var lb = item.lb;
                var text = item.text;

                if (lb == null || lb.IsDisposed) continue;

                // 이미 UI스레드이므로 Invoke 불필요
                lb.Items.Add(text);
                lb.TopIndex = lb.Items.Count - 1;

                if (lb.Items.Count > MaxLogLines)
                {
                    int toRemove = lb.Items.Count - MaxLogLines;
                    // 너무 자주 1개씩 지우지 말고 한 번에 블럭으로
                    for (int i = 0; i < toRemove; i++)
                        lb.Items.RemoveAt(0);
                }

                if (lb == lstRecipeScanLog && lstTcpLog != null && !lstTcpLog.IsDisposed)
                {
                    lstTcpLog.Items.Add(text);
                    lstTcpLog.TopIndex = lstTcpLog.Items.Count - 1;
                }
                processed++;
            }
        }
        private void StartServer()
        {
            try
            {
                if (_listener != null) { Info("Already listening"); return; }

                // 기존 토큰 정리 후 새 토큰
                try { _serverCts?.Cancel(); _serverCts?.Dispose(); } catch { }
                _serverCts = new CancellationTokenSource();
                var token = _serverCts.Token;
                _listener = new TcpListener(IPAddress.Any, PORT);
                _listener.Start();
                Info("Listening " + PORT);
                _ = Task.Run(async () => {
                    try
                    {
                        while (!token.IsCancellationRequested)
                        {
                            // 취소 대응: Accept와 취소를 경합시켜 안전 종료
                            var acceptTask = _listener.AcceptTcpClientAsync();
                            var done = await Task.WhenAny(acceptTask, Task.Delay(Timeout.Infinite, token));
                            if (done != acceptTask) break; // 취소됨
                            var cli = acceptTask.Result;
                            if (token.IsCancellationRequested)
                            {
                                try { cli.Close(); } catch { } break;
                            }
                            var role = ResolveRoleByRemoteIp(cli);
                            var sess = new RpcSession(cli) { Logger = this };
                            sess.Options.EnableHeartbeat = false;
                            if (role == Role.Unknown)
                            {
                                Info($"Reject: no FixedRoleMap for {sess.Remote}");
                                try { cli.Close(); } catch { } continue;
                            }
                            sess.Role = role;
                            sess.Error += (s2, ex) =>
                            {
                                Error("Session error", ex);
                                UI(() => SetLinkState(role, false));
                            };
                            sess.Disconnected += s2 =>
                            {
                                OnDisconnected(s2);
                                UI(() => SetLinkState(role, false));
                            };
                            AttachRouterFor(sess);
                            _sessions[cli] = sess;
                            sess.NetLog = s => AddLog(lstRecipeScanLog, s);
                            #if DEBUG
                                sess.NetDumpBodies = true;
                                sess.NetHexPreviewBytes = 64;
                            #else
                                sess.NetDumpBodies = false;
                                sess.NetHexPreviewBytes = 0;
                            #endif
                            sess.Start();
                            UI(() => SetLinkState(role, true));
                            Info($"+ Connected {sess.Remote} (Role={role})");
                        }
                    }
                    catch (ObjectDisposedException) { /* stopped */ }
                    // Client가 종료되서 끊기는건지 아니면 Stage에서 끊는건지 확인
                    catch (IOException ioEx) { Info("Socket Closed: " + ioEx.Message); }

                    catch (Exception ex)
                    {
                        Error("AcceptLoop", ex);
                        Info("Recv header failed: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Error("Start", ex);
                MessageBox.Show(this, ex.Message, "Server Start Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void StopServer()
        {
            try { _serverCts?.Cancel(); } catch { }
            try { _listener?.Stop(); }
            catch { }
            finally { _listener = null; }
            try
            {
                foreach (var s in _sessions.Values)
                {
                    try { s.Stop(); } catch { }
                    try { s.Dispose(); } catch { }
                }
            }
            catch { }
            finally
            {
                _sessions.Clear();
                _routers.Clear();
            }
            try { _serverCts?.Dispose(); } catch { } _serverCts = null;
        }
        void OnDisconnected(RpcSession s)
        {
            Info("- Disconnected " + s.Remote);
            try
            {
                foreach (var kv in _sessions)
                    if (kv.Value == s) { _sessions.TryRemove(kv.Key, out _); break; }
                _routers.TryRemove(s, out _);
            }
            catch { }
        }
        private void EnableLogBoxColor(ListBox lb)
        {
            try { lb.Font = new System.Drawing.Font("Consolas", lb.Font.Size); } catch { /* 폰트 없으면 무시 */ }
            lb.DrawMode = DrawMode.OwnerDrawFixed;
            lb.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index < 0) return;
                var text = lb.Items[e.Index]?.ToString() ?? string.Empty;
                var col = System.Drawing.Color.Black;
                if (text.Contains(" err=") || text.Contains("[E]")) col = System.Drawing.Color.Firebrick;
                else if (text.Contains("TIMEOUT") || text.Contains("[W]")) col = System.Drawing.Color.DarkOrange;
                else if (text.StartsWith("[NET] ▶")) col = System.Drawing.Color.RoyalBlue;   // 송신
                else if (text.StartsWith("[NET] ◀")) col = System.Drawing.Color.ForestGreen; // 수신
                TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, col, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                e.DrawFocusRectangle();
            };
        }
        private void AttachRouterFor(RpcSession s)
        {
            var router = new PacketRouter();
            if (s.Role == Role.Vision)
            {
                VisionRpc.RegisterHandlers( router, (t) => AddLog(lstRecipeScanLog, t), motorMoveHandler: (mv) =>
                    {
                        try
                        {
                            // nMoveType: 0=REL, 1=ABS  //  X/Y: ACS, T: AjinMotion (도 단위 가정)
                            double curX = _acsMotion.GetAPosition(Axis.X);
                            double curY = _acsMotion.GetAPosition(Axis.Y);
                            double curT = _ajinMotion.GetActPos(AjinAxis.T);

                            // 목표 위치 계산
                            double targetX, targetY, targetT;
                            targetX = mv.dMoveX; targetY = mv.dMoveY; targetT = mv.dMoveT;
                            AddLog(lstRecipeScanLog, $"[VISION->Stage 3005] MOVE Type={(mv.nMoveType == 1 ? "ABS" : "REL")} " +
                                $"X:{targetX:F3}  Y:{targetY:F3}  T:{targetT:F3}");
                            // 서보 ON + 동기 이동 (ACK는 이동완료 후 회신)
                            try
                            {
                                if (mv.nMoveType == 1)
                                {
                                    var tx = _acsMotion.MoveAbsAsync(Axis.X, targetX);
                                    var ty = _acsMotion.MoveAbsAsync(Axis.Y, targetY);
                                    var tt = _ajinMotion.MoveAbsAsync(AjinAxis.T, targetT, 0.5, 1, 1);  // test KJW 임시설정값 향후 파라미터창에서 가져올것
                                    // 완료 후 보내기 = PGT 
                                    Task.WhenAll(tx, ty, tt).GetAwaiter().GetResult();
                                }
                                else
                                {
                                    var tx = _acsMotion.MoveRelAsync(Axis.X, targetX);
                                    var ty = _acsMotion.MoveRelAsync(Axis.Y, targetY);
                                    var tt = _ajinMotion.MoveRelAsync(AjinAxis.T, targetT, 0.5, 1, 1);  // test KJW 임시설정값 향후 파라미터창에서 가져올것
                                    // 완료 후 보내기 = PGT 
                                    Task.WhenAll(tx, ty, tt).GetAwaiter().GetResult();
                                }
                                AddLog(lstRecipeScanLog, $"[VISION->Stage 3005] MOVE 완료 X:{targetX:F3}, Y:{targetY:F3}, T:{targetT:F3}");
                            }
                            catch (Exception mex)
                            {
                                AddLog(lstRecipeScanLog, "[E][3005 MOVE] " + mex.Message);
                                try { _acsMotion.StopAsync(Axis.X); _acsMotion.StopAsync(Axis.Y); } catch { }
                                if (_ajinMotion?.IsConnected == true) { try { _ajinMotion.Stop(AjinAxis.T); } catch { } }
                            }
                            return Err.OK;
                        }
                        catch (Exception ex)
                        {
                            AddLog(lstRecipeScanLog, "[E][3005 MOVE] " + ex.Message);
                            return Err.IOERROR;
                        }
                    },
                    alignRespHandler: (res) => { AddLog(lstRecipeScanLog, "AlignResp: " + (res.nResult == 0 ? "OK" : "NG")); },
                    motorCurPosProvider: () =>
                    {
                        try
                        {
                            var x = _acsMotion.GetAPosition(Axis.X);   // Review X
                            var y = _acsMotion.GetAPosition(Axis.Y);   // Main   Y
                            var t = _ajinMotion.GetActPos(AjinAxis.T); // Ajin T축(도)
                            return new[] { x, y, t };
                        }
                        catch { return new[] { 0.0, 0.0, 0.0 }; }
                    },
                    secondAlignRespHandler: null, markFindSingleRespHandler: null, markFindLineRespHandler: null,
                    flyingReadyReqHandler: req => { AddLog(lstRecipeScanLog, $"[SEQ] FlyingReady 수신 (nDataCount={req.nDataCount})"); },
                    manualInspectionReqHandler: req =>
                    {
                        AddLog(lstRecipeScanLog, "[VISION->Stage 3008] ManualInspection Check.");
                        OnManualInspectionReq(req);
                    }
                );
            }
            else if (s.Role == Role.Scan)
            {
                ScanRpc.RegisterHandlers(router, (t) => AddLog(lstRecipeScanLog, t),
                    motorMoveHandler: (mv) => // 1021: 실제 ACS 축 구동
                    {
                        try
                        {
                            var ax = AxisFromScanIndex(mv.nAxisNo);
                            // ABS/REL 대상 위치 계산
                            double target;
                            if (mv.nMoveType == 1) // ABS
                            {
                                target = mv.dMoveValue;
                            }
                            else if (mv.nMoveType == 0) // REL
                            {
                                var cur = _acsMotion.GetAPosition(ax);
                                target = cur + mv.dMoveValue;
                            }
                            else
                            {
                                AddLog(lstRecipeScanLog, $"[SCAN->Stage 1021] 지원하지 않는 MoveType={mv.nMoveType}");
                                return Err.INVALID_ARG;
                            }
                            AddLog(lstRecipeScanLog, $"[SCAN->Stage 1021] MOVE Ax={mv.nAxisNo}({ax}) Type={(mv.nMoveType == 1 ? "ABS" : "REL")} Target={target:F3}");
                            // 서보 ON 후 동기 이동(ACK는 이동 완료 회신)
                            try
                            {
                                _acsMotion.ServoOnAsync(ax).GetAwaiter().GetResult();
                                _acsMotion.MoveAbsAsync(ax, target).GetAwaiter().GetResult();
                                AddLog(lstRecipeScanLog, $"[SCAN->Stage 1021] MOVE 완료 Ax={ax} Target={target:F3}");
                            }
                            catch (Exception mex)
                            {
                                AddLog(lstRecipeScanLog, "[E][1021 MOVE] " + mex.Message);
                                try { _acsMotion.StopAsync(ax); } catch { }
                            }
                            return Err.OK;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return Err.INVALID_ARG;
                        }
                        catch (Exception ex)
                        {
                            AddLog(lstRecipeScanLog, "[E][1021 MOVE] " + ex.Message);
                            return Err.IOERROR;
                        }
                    },
                    recipeRespHandler: (rl) => { },
                    motorPosProvider: (axisNo) =>
                    {
                        try
                        {
                            var ax = AxisFromScanIndex(axisNo);  // 0:X, 1:Y
                            var pos = _acsMotion.GetAPosition(ax); // 목표 위치(Target)
                            var actual = _acsMotion.GetPosition(ax);       // 실제 위치(Feedback)
                            double tol = (ax == Axis.X ? X_INPOS_TOL : Y_INPOS_TOL);
                            // 현재 축에 대한 마지막 '로그 완료 대상' 조회(없으면 NaN 취급)
                            double loggedFor;
                            if (!_posReachedLoggedTarget.TryGetValue(axisNo, out loggedFor)) loggedFor = double.NaN;
                            // 목표가 바뀌었으면(이전 기록과 현 목표가 충분히 다르면) 미로그 상태로 리셋
                            if (!double.IsNaN(loggedFor) && Math.Abs(loggedFor - pos) > tol) loggedFor = double.NaN;
                            // 목표에 도달했고(허용오차 내), 아직 이 목표에 대한 로그를 하지 않았다면 1회 로그
                            if (Math.Abs(actual - pos) <= tol && double.IsNaN(loggedFor))
                            {
                                _posReachedLoggedTarget[axisNo] = pos; // 이번 목표는 로그 완료 처리
                                AddLog(lstRecipeScanLog, $"[SCAN->Stage 1022] POS ax={axisNo}({ax}) = {pos:F3} mm (IN-POS)");
                            }
                            else _posReachedLoggedTarget[axisNo] = loggedFor; // 상태 갱신(키 보장)만 수행
                            return pos; // 프로토콜 요구대로 Target(mm) 반환
                        }
                        catch (Exception ex)
                        {
                            AddLog(lstRecipeScanLog, "[E][1022 POS] " + ex.Message);
                            throw; // 예외를 던지면 RegisterHandlers 쪽에서 IOERROR로 응답
                        }
                    }
                );
            }
            _routers[s] = router;
            s.OnRequestAsync = (c, b) => router.DispatchAsync(s, c, b);
        }
        private RpcSession EnsureRoleForOutgoing(Role desired)
        {
            foreach (var s in _sessions.Values) if (s.Role == desired) return s;
            return null;
        }
        private void ledServer_Click(object sender, EventArgs e)
        {
            try { StopServer(); } catch { }
            StartServer();
            AddLog(lstSystem, "[Server] Restarted");
        }
        private RpcSession GetScanSessionOrWarn()
        {
            var scan = EnsureRoleForOutgoing(Role.Scan);
            if (scan == null) AddLog(lstSystem, "[Scan] 연결되어 있지 않거나 역할을 결정할 수 없습니다.");
            return scan;
        }
        private RpcSession GetVisionSessionOrWarn()
        {
            var vision = EnsureRoleForOutgoing(Role.Vision);
            if (vision == null) AddLog(lstSystem, "[Vision] 연결되어 있지 않거나 역할을 결정할 수 없습니다.");
            return vision;
        }
        #endregion

        #region ======= 오토시퀀스 관련항목 =======
        // 시퀀스 실행 상태
        private bool _seqRunning = false;
        private bool _seqIsFullAuto = false;
        private AutoSequenceId _seqCurrent;

        // 탭 제한에 사용할 TabControl (디자이너에서 사용 중인 이름으로 교체)
        private TabControl _mainTab;   // 실제 이름 다르면 교체

        // 상단 배너
        private Panel _seqBanner;
        private Label _seqBannerLabel;
        private readonly Timer _tmrSeqBlink = new Timer { Interval = 400 };
        private bool _seqBlinkState = false;
        private Color _seqBannerBack0;
        private int _autoNextInspectLine1 = 1;
        private int _autoNextProcessLine1 = 1;
        private sealed class AutoSequenceRunner
        {
            private readonly Form1 _owner;

            public AutoSequenceRunner(Form1 owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            // Semi Auto: 단일 시퀀스 실행
            public async Task RunSemiAutoAsync(AutoSequenceId seq)
            {
                // 이미 다른 시퀀스 동작 중이면 차단
                if (_owner._seqRunning)
                {
                    MessageBox.Show(_owner,
                        "다른 시퀀스가 실행 중입니다.\r\n현재 시퀀스가 종료된 후 다시 시도하세요.",
                        "Sequence",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                if (!_owner.EnsureClientLinksForSequence(seq, "Semi-Auto"))
                    return;

                if (SeqNeedsScanRecipeOrigin(seq))
                {
                    var origin = _owner._recipeForm?.OpenOrigin ?? RecipeForm.RecipeOpenOrigin.Local;
                    if (origin != RecipeForm.RecipeOpenOrigin.Scan)
                    {
                        MessageBox.Show(_owner,
                            "가공(Process) 동작은 Scan 레시피에서만 실행할 수 있습니다.\r\n" +
                            "Scan 탭에서 레시피를 선택한 후 다시 시도하세요.",
                            "Recipe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (!_owner.EnsureRecipeSelectedForAuto("Semi-Auto")) return;
                _owner.EnsureParameterSettingLoaded();
                _owner.AddLog(_owner.lstLog, $"[SemiAuto] {seq}");
                _owner.OnSequenceRunStart(isFullAuto: false, seq: seq);
                _owner._autoForm?.SetSequenceBlink(isFullAuto: false, seq: seq, running: true);
                _owner._overviewForm?.AddOverviewLog($"[SemiAuto] {seq} 시작");

                if (seq == AutoSequenceId.PowerMeter)
                {
                    if (!_owner.EnsurePmRecipeMatchesToolingAttSource("Semi-Auto(PowerMeter)"))
                    {
                        _owner._autoForm?.SetSequenceBlink(isFullAuto: false, seq: seq, running: false);
                        _owner.OnSequenceRunEnd();
                        return;
                    }
                }
                try
                {
                    switch (seq)
                    {
                        case AutoSequenceId.Load:             await RunSeq_WithDryRunAsync(_owner.RunSeq_LoadAsync,             false); break;
                        case AutoSequenceId.MoveToAlign:      await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToAlignAsync,      false); break;
                        case AutoSequenceId.Align:            await RunSeq_WithDryRunAsync(_owner.RunSeq_AlignAsync,            false); break;
                        case AutoSequenceId.PowerMeter:       await RunSeq_WithDryRunAsync(_owner.RunSeq_PowerMeterAsync,       false); break;
                        case AutoSequenceId.ProcessReady:     await RunSeq_WithDryRunAsync(_owner.RunSeq_ProcessReadyAsync,     false); break;
                        case AutoSequenceId.Process:          await RunSeq_WithDryRunAsync(_owner.RunSeq_ProcessAsync,          false); break;
                        case AutoSequenceId.Inspection:       await RunSeq_WithDryRunAsync(_owner.RunSeq_InspectionAsync,       false); break;
                        case AutoSequenceId.MoveToAlignCheck: await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToAlignCheckAsync, false); break;
                        case AutoSequenceId.AlignCheck:       await RunSeq_WithDryRunAsync(_owner.RunSeq_AlignCheckAsync,       false); break;
                        case AutoSequenceId.MoveToUnload:     await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToUnloadAsync,     false); break;
                        case AutoSequenceId.Unload:           await RunSeq_WithDryRunAsync(_owner.RunSeq_UnloadAsync,           false); break;
                    }
                }
                finally
                {
                    _owner._overviewForm?.AddOverviewLog($"[SemiAuto] {seq} 완료");
                    _owner._autoForm?.SetSequenceBlink(isFullAuto: false, seq: seq, running: false);
                    _owner.OnSequenceRunEnd();
                }
            }
            // Auto: 여러 시퀀스를 순차 실행
            public async Task RunAutoAsync(AutoProcessRequest req)
            {
                // 이미 시퀀스 동작 중이면 차단
                if (_owner._seqRunning)
                {
                    MessageBox.Show(_owner,
                        "다른 시퀀스가 실행 중입니다.\r\n현재 시퀀스가 종료된 후 다시 시도하세요.",
                        "Sequence",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                if (!_owner.EnsureClientLinksForSequences(req?.EnabledSequences ?? Array.Empty<AutoSequenceId>(), "Auto"))
                    return;
                if (req != null && (req.EnabledSequences?.Any(SeqNeedsScan) ?? false))
                {
                    var origin = _owner._recipeForm?.OpenOrigin ?? RecipeForm.RecipeOpenOrigin.Local;
                    if (origin != RecipeForm.RecipeOpenOrigin.Scan)
                    {
                        MessageBox.Show(_owner,
                            "Auto 가공(Process) 동작은 Scan 레시피에서만 실행할 수 있습니다.\r\n" +
                            "Scan 탭에서 레시피를 선택한 후 다시 시도하세요.",
                            "Recipe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (!_owner.EnsureRecipeSelectedForAuto("Full Auto")) return;
                _owner.EnsureParameterSettingLoaded();
                if (req != null && (req.EnabledSequences?.Contains(AutoSequenceId.PowerMeter) ?? false))
                {
                    if (!_owner.EnsurePmRecipeMatchesToolingAttSource("AutoStart")) return;
                }

                // 첫 시퀀스 기준으로 Start
                var firstSeq = (req.EnabledSequences.Length > 0) ? req.EnabledSequences[0] : AutoSequenceId.Load;
                _owner.OnSequenceRunStart(isFullAuto: true, seq: firstSeq);
                _owner._overviewForm?.AddOverviewLog($"[Auto] {req.EnabledSequences.Length}개 시퀀스 Start (DryRun={req.DryRun})");

                // Full Auto 상태 라벨 초기화
                _owner._autoForm?.ResetAutoSeqStates();
                foreach (var s in req.EnabledSequences)
                    _owner._autoForm?.SetAutoSeqState(s, AutoProcessForm.AutoSeqVisualState.Wait);
                string boardId = $"B{DateTime.Now:yyyyMMdd_HHmmss}";
                string recipeName = (_owner._currentRecipeName ?? "").Trim();
                var enabledSeqNames = (req?.EnabledSequences ?? Array.Empty<AutoSequenceId>())
                    .Select(s => s.ToString())
                    .ToArray();

                Logger.AutoBeginBoard(boardId, recipeName, req.DryRun, enabledSeqNames);
                Logger.AutoInfo($"[AUTO] Start | SeqCount={enabledSeqNames.Length}");
                // Full Auto 시 NotificationBlinkForm 사용
                using (StageWin.Etc.BlinkNotifier.Show(_owner,
                    "FULL AUTO 시퀀스 동작 중", 450, Color.DarkOrange))
                {
                    try
                    {
                        foreach (var seq in req.EnabledSequences)
                        {
                            if (!_owner.EnsureClientLinksForSequence(seq, "Auto"))
                            {
                                _owner._autoForm?.SetAutoSeqState(seq, AutoProcessForm.AutoSeqVisualState.Error);
                                Logger.AutoWarn($"[AUTO] Link check failed at seq={seq}");
                                break;
                            }

                            _owner.OnSequenceRunStart(isFullAuto: true, seq: seq);
                            StageWin.Etc.BlinkNotifier.Update($"FULL AUTO - {seq}" +"\r\n" + "동작 중");
                            _owner._autoForm?.SetAutoSeqState(seq, AutoProcessForm.AutoSeqVisualState.Running);
                            _owner._overviewForm?.AddOverviewLog($"[Auto] {seq} Start (DryRun={req.DryRun})");

                            using (Logger.AutoStepScope(seq.ToString(), note: $"DryRun={req.DryRun}"))
                            {
                                switch (seq)
                                {
                                    case AutoSequenceId.Load:               await RunSeq_WithDryRunAsync(_owner.RunSeq_LoadAsync, req.DryRun);              break;
                                    case AutoSequenceId.MoveToAlign:        await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToAlignAsync, req.DryRun);       break;
                                    case AutoSequenceId.Align:              await RunSeq_WithDryRunAsync(_owner.RunSeq_AlignAsync, req.DryRun);             break;
                                    case AutoSequenceId.PowerMeter:         await RunSeq_WithDryRunAsync(_owner.RunSeq_PowerMeterAsync, req.DryRun);        break;
                                    case AutoSequenceId.ProcessReady:       await RunSeq_WithDryRunAsync(_owner.RunSeq_ProcessReadyAsync, req.DryRun);      break;
                                    case AutoSequenceId.Process:            await RunSeq_WithDryRunAsync(_owner.RunSeq_ProcessAsync, req.DryRun);           break;
                                    case AutoSequenceId.Inspection:         await RunSeq_WithDryRunAsync(_owner.RunSeq_InspectionAsync, req.DryRun);        break;
                                    case AutoSequenceId.MoveToAlignCheck:   await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToAlignCheckAsync, req.DryRun);  break;
                                    case AutoSequenceId.AlignCheck:         await RunSeq_WithDryRunAsync(_owner.RunSeq_AlignCheckAsync, req.DryRun);        break;
                                    case AutoSequenceId.MoveToUnload:       await RunSeq_WithDryRunAsync(_owner.RunSeq_MoveToUnloadAsync, req.DryRun);      break;
                                    case AutoSequenceId.Unload:             await RunSeq_WithDryRunAsync(_owner.RunSeq_UnloadAsync, req.DryRun);            break;
                                }
                            }
                            _owner._autoForm?.SetAutoSeqState(seq, AutoProcessForm.AutoSeqVisualState.Done);
                            _owner._overviewForm?.AddOverviewLog($"[Auto] {seq} Done");
                        }
                    }
                    finally
                    {
                        _owner._overviewForm?.AddOverviewLog("[Auto] All Sequence Done");
                        _owner.OnSequenceRunEnd();
                        Logger.AutoEndBoard(ok: true, endReason: "All sequences done");
                    }
                }
            }
            // 내부: DryRun + 공통 토큰/타워 램프/로그 처리
            private async Task RunSeq_WithDryRunAsync(Func<bool, Task> runner, bool dryRun)
            {
                if (!_owner.EnsureSemiOrAuto("Sequence")) return;
                try { _owner._procCts?.Cancel(); } catch { }
                try { _owner._procCts?.Dispose(); } catch { }
                _owner._procCts = new CancellationTokenSource();
                var ct = _owner._procCts.Token;
                _owner._autoSeqRunning = true;
                _owner._towerForm?.RefreshNow();
                bool runnerDone = false;

                try
                {
                    _owner.AddLog(_owner.lstSystem, $"[SEQ] Start (DryRun={dryRun})");
                    await runner(dryRun);          // runner는 _procCts를 건드리지 않는 전제
                    runnerDone = true;
                    _owner.AddLog(_owner.lstSystem, "[SEQ] Done");
                    // AfterSeq는 "후처리 딜레이" → 취소되면 시퀀스 Cancel로 몰지 말고 스킵
                    var delayMs = _owner.GetAutoDelayAfterSeqMs();
                    try
                    {
                        await _owner.ApplyDelayAsync(delayMs, ct, "AfterSeq");
                    }
                    catch (OperationCanceledException)
                    {
                        _owner.AddLog(_owner.lstSystem, "[SEQ] AfterSeq delay skipped (Canceled)");
                        // 여기서 throw 하지 않음
                    }
                }
                catch (OperationCanceledException)
                {
                    // runner 수행 중 취소된 경우만 "Canceled"로 로그
                    if (!runnerDone) _owner.AddLog(_owner.lstSystem, "[SEQ] Canceled");
                    else _owner.AddLog(_owner.lstSystem, "[SEQ] Done (but token canceled after runner)");
                }
                catch (Exception ex)
                {
                    _owner.AddLog(_owner.lstSystem, "[E][SEQ] " + ex.Message);
                    MessageBox.Show(_owner,
                        "시퀀스 실행 중 오류: " + ex.Message,
                        "Sequence", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _owner._autoSeqRunning = false;
                    _owner._towerForm?.RefreshNow();
                }
            }
        }
        private void EnsureAutoProcessForm()
        {
            if (_autoForm != null && !_autoForm.IsDisposed) return;
            _autoForm = new AutoProcessForm();
            // 팝업 창 형태
            _autoForm.StartPosition = FormStartPosition.Manual;
            _autoForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            // 현재 모드 제공
            _autoForm.ModeProvider = () => GetProgramModeFromUi();
            // 이벤트 연결
            _autoForm.RequestRunSemiAuto += async seq => await _autoSeqRunner.RunSemiAutoAsync(seq);
            _autoForm.RequestRunAuto += async req => await _autoSeqRunner.RunAutoAsync(req);
            _autoForm.RequestStopAll += async () => await StopAllSequencesAsync();
            _autoForm.FormClosing += (s, e) =>  // 사용자가 X 눌렀을 때 완전히 Dispose 되지 않고 Hide만 되도록
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    _autoForm.Hide();
                    ForceMode(ProgramMode.Manual);
                    UpdateAutoFormEnableState();
                }
            };
        }
        private void ShowAutoProcessPopup()
        {
            EnsureAutoProcessForm();
            // 위치: 메인 폼 오른쪽 상단 근처 (원하면 조정 가능)
            int x = this.Right - _autoForm.Width - 20;
            int y = this.Top + 120;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            _autoForm.Location = new Point(x, y);
            // 처음이면 Show, 이미 떠있으면 Activate
            if (!_autoForm.Visible) _autoForm.Show(this);
            else _autoForm.Activate();
        }
        private void HideAutoProcessPopup()
        {
            if (_autoForm != null && !_autoForm.IsDisposed) _autoForm.Hide();
        }
        private void InitSequenceBanner()
        {
            if (_seqBanner != null) return;
            Control parent = tabOverview ?? (Control)this;
            _seqBanner = new Panel
            {
                Dock = DockStyle.Fill, // 탭 내용 전체를 덮는 오버레이
                BackColor = Color.FromArgb(80, 0, 0, 0), // 반투명 느낌 (완전한 알파는 아니지만 어두운 오버레이)
                Visible = false
            };
            _seqBannerLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Yellow,
                Font = new Font("맑은 고딕", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(160, 0, 0, 0), // 안쪽 박스는 조금 더 진하게
                Width = 420,
                Height = 60
            };

            // 가운데 정렬
            _seqBanner.Resize += (s, e) =>
            {
                if (_seqBannerLabel != null)
                {
                    _seqBannerLabel.Left = (_seqBanner.ClientSize.Width - _seqBannerLabel.Width) / 2;
                    _seqBannerLabel.Top = (_seqBanner.ClientSize.Height - _seqBannerLabel.Height) / 2;
                }
            };
            _seqBanner.Controls.Add(_seqBannerLabel);
            parent.Controls.Add(_seqBanner);
            _seqBanner.BringToFront();

            _seqBannerBack0 = _seqBannerLabel.BackColor;  // 깜빡임은 Label 배경으로만

            _tmrSeqBlink.Tick -= TmrSeqBlink_Tick;
            _tmrSeqBlink.Tick += TmrSeqBlink_Tick;
        }
        private void TmrSeqBlink_Tick(object sender, EventArgs e)
        {
            if (!_seqRunning || _seqBanner == null || _seqBannerLabel == null)
            {
                _tmrSeqBlink.Stop();
                return;
            }

            _seqBlinkState = !_seqBlinkState;
            _seqBannerLabel.BackColor = _seqBlinkState
                ? Color.FromArgb(200, 220, 20, 60)   // 밝은 빨강 계열
                : _seqBannerBack0;
        }
        private int GetProcessParamInt(string key, int fallback)
        {
            EnsureParameterSettingLoaded();
            if (_paramSetting?.ProcessParameter == null) return fallback;
            if (!_paramSetting.ProcessParameter.TryGetValue(key, out var s)) return fallback;
            if (string.IsNullOrWhiteSpace(s)) return fallback;

            if (int.TryParse(s, out var v)) return v;
            if (double.TryParse(s, out var dv)) return (int)Math.Round(dv);
            return fallback;
        }
        private int GetAutoDelayAfterSeqMs() => Math.Max(0, GetProcessParamInt("AutoDelayAfterSeqMs", 0));
        private int GetAutoDelayAfterMoveMs() => Math.Max(0, GetProcessParamInt("AutoDelayAfterMoveMs", 0));
        private async Task ApplyDelayAsync(int ms, CancellationToken ct, string tagForLog)
        {
            if (ms <= 0) return;
            if (ct.IsCancellationRequested) return;

            try
            {
                await Task.Delay(ms, ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        internal void OnSequenceRunStart(bool isFullAuto, AutoSequenceId seq)
        {
            _seqIsFullAuto = isFullAuto;
            _seqCurrent = seq;
            _seqRunning = true;
            // Full Auto면 Overview로, Semi는 현 탭 유지
            if (isFullAuto && tabOverview != null)
            {
                var tc = GetMainTabControl();
                if (tc != null) tc.SelectedTab = tabOverview;
            }
            ApplyAutoTabRestriction(running: true, fullAuto: isFullAuto);
        }
        internal void OnSequenceRunEnd()
        {
            _seqRunning = false;
            ApplyAutoTabRestriction(running: false, fullAuto: false);
        }
        private bool IsAllowedTabWhileRunning(TabPage page)
        {
            if (page == null) return false;
            var text = (page.Text ?? string.Empty).ToUpperInvariant();

            // Overview, I/O, Log & Alarm 만 허용
            if (text.Contains("OVERVIEW")) return true;
            if (text.Contains("I/O") || text.Contains("IO")) return true;
            if (text.Contains("LOG") || text.Contains("ALARM")) return true;

            return false;
        }
        private void ApplyAutoTabRestriction(bool running, bool fullAuto)
        {
            var tc = GetMainTabControl();
            if (tc == null) return;
            if (!running)
            {
                // 제한 해제
                foreach (TabPage tp in tc.TabPages) tp.Enabled = true;
                return;
            }
            // Semi-Auto일 때는 탭 제한 없음
            if (!fullAuto)
            {
                foreach (TabPage tp in tc.TabPages) tp.Enabled = true;
                return;
            }
            // Full Auto일 때만 제한
            foreach (TabPage tp in tc.TabPages) tp.Enabled = IsAllowedTabWhileRunning(tp);

            // Full Auto 시에는 Overview 탭으로 강제 전환
            if (tabOverview != null) tc.SelectedTab = tabOverview;
        }
        private void UpdateCurrentRecipeUi(string name, bool fromScan)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _currentRecipeName = name;
            string src = fromScan ? "Scan" : "Local";
            try
            {
                this.Text = $"StageWin - {name} [{src}]";       // 윈도우 타이틀
            }
            catch { }

            try
            {
                // 상단 라벨이 있다면 같이 표시 (디자이너에서 lblRecipeName 하나 만들어 두면 됨)
                if (lblRecipeName != null) lblRecipeName.Text = $"Recipe : {name} ({src})";
            }
            catch { /* 라벨 없으면 무시 */ }
        }
        private void UpdateAutoFormEnableState()
        {
            EnsureAutoProcessForm();
            var mode = GetProgramModeFromUi();
            // 인터락 조건: Alarm 레벨 Alarm 이상 없을 때만 허용
            bool noAlarm = !(_alarmMgr?.HasActiveLevel(AlarmLevel.Alarm) ?? false);
            //bool enable = noAlarm && (mode == ProgramMode.SemiAuto || mode == ProgramMode.Auto);
            bool enable = (mode == ProgramMode.SemiAuto || mode == ProgramMode.Auto);    // 260704 KJW 강제 Auto모드 활성화
            _autoForm.UpdateModeState(mode, enable);
            // 모드에 따라 탭 선택
            if (mode == ProgramMode.SemiAuto) _autoForm.SelectSemiAutoTab();
            else if (mode == ProgramMode.Auto) _autoForm.SelectAutoTab();
            // 팝업 Show/Hide
            if (enable) ShowAutoProcessPopup();
            else HideAutoProcessPopup();
        }
        private (double? vel, double? acc, double? dec) GetProcessOverride(Axis axis)
        {
            // 현재 모션 프로파일(기본값)
            var cur = _acsMotion.GetProfile(axis);

            // ParameterSetting(ProcessParameter)에서 읽기
            // 키 예: ProcessYSpeed, ProcessYAcc, (선택) ProcessYDec
            string axisStr = (axis == Axis.X) ? "X" : "Y";

            double vel = GetProcessParamDouble($"Process{axisStr}Speed", cur.Velocity);
            double acc = GetProcessParamDouble($"Process{axisStr}Acc", cur.Acceleration);
            double dec = GetProcessParamDouble($"Process{axisStr}Dec", acc); // Dec 키 없으면 Acc로

            // 0 이하/비정상 값이면 override 하지 않고 기존 사용
            if (vel <= 0) vel = cur.Velocity;
            if (acc <= 0) acc = cur.Acceleration;
            if (dec <= 0) dec = cur.Deceleration;

            return (vel, acc, dec);
        }
        private async Task MoveAbsAndWait_SeqAsync(Axis axis, double targetPos, double tol, int timeoutMs, CancellationToken ct)
        {
            await MoveAbsAndWait_SeqAsync(axis, targetPos, tol, timeoutMs, ct,
                velOverride: null, accOverride: null, decOverride: null);
        }
        private async Task MoveAbsAndWait_SeqAsync(Axis axis, double targetPos, double tol, int timeoutMs, CancellationToken ct,
            double? velOverride = null,
            double? accOverride = null,
            double? decOverride = null)
        {
            double cur = double.NaN;
            try { cur = _acsMotion.GetPosition(axis); } catch { }

            // profile 계산(네 기존 코드 유지)
            var (v, a, d) = GetProcessOverride(axis);
            double v2 = (double)(velOverride ?? v);
            double a2 = (double)(accOverride ?? a);
            double d2 = (double)(decOverride ?? d);

            if (Logger.IsAutoBoardActive)
            {
                using (Logger.AutoStepScope(
                    stepName: $"MOVE-{axis}",
                    note: "MoveAbsAndWait_SeqAsync",
                    ("StartPos", cur),
                    ("TargetPos", targetPos),
                    ("Tol", tol),
                    ("Vel", v2),
                    ("Acc", a2),
                    ("Dec", d2),
                    ("TimeoutMs", timeoutMs)))
                {
                    await MoveAbsAndWaitAsync(axis, targetPos, tol, timeoutMs, ct,
                        velOverride: v2,
                        accOverride: a2,
                        decOverride: d2);
                    await WaitAxisInPositionAsync(axis, targetPos, tol, timeoutMs: 2000, ct);
                    var moveDelayMs = GetAutoDelayAfterMoveMs();
                    await ApplyDelayAsync(moveDelayMs, ct, $"AfterMove {axis}");
                }
                return;
            }
            await MoveAbsAndWaitAsync(axis, targetPos, tol, timeoutMs, ct,
                velOverride: v2,
                accOverride: a2,
                decOverride: d2);
            await WaitAxisInPositionAsync(axis, targetPos, tol, timeoutMs: 2000, ct);
            var moveDelayMs2 = GetAutoDelayAfterMoveMs();
            await ApplyDelayAsync(moveDelayMs2, ct, $"AfterMove {axis}");
        }
        private string GetSelectedPmRecipeNameFromParameterSetting()
        {
            EnsureParameterSettingLoaded();
            if (_paramSetting == null) return "";

            // 1) 가장 확실한 키: ProcessParameter의 PowerMeterRecipeName (PowerMeter 시퀀스도 이걸 씀)
            if (_paramSetting.ProcessParameter != null &&
                _paramSetting.ProcessParameter.TryGetValue("PowerMeterRecipeName", out var ppv))
            {
                var s = (ppv ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }
            // 2) ParameterValue 후보 키들
            if (_paramSetting.ParameterValue != null)
            {
                string[] keyCandidates =
                {
                    "SelectedPmRecipeName",
                    "PmRecipeName",
                    "PowermeterRecipeName",
                    "PowerMeterRecipeName",
                    "txtSelectedPmRecipeName"
                };
                foreach (var k in keyCandidates)
                {
                    if (_paramSetting.ParameterValue.TryGetValue(k, out var v))
                    {
                        var s = (v ?? "").Trim();
                        if (!string.IsNullOrWhiteSpace(s)) return s;
                    }
                }
            }
            // 3) fallback: ParameterValue/ProcessParameter 전체를 실제 Store의 레시피명과 매칭
            try
            {
                var store = PowerRecipeStore.Open(AppConfig.Current?.PowerRecipesPath);
                var names = store?.ListNames();
                if (names != null && names.Count > 0)
                {
                    var set = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
                    if (_paramSetting.ProcessParameter != null)
                    {
                        foreach (var kv in _paramSetting.ProcessParameter)
                        {
                            var val = (kv.Value ?? "").Trim();
                            if (!string.IsNullOrWhiteSpace(val) && set.Contains(val))
                                return val;
                        }
                    }
                    if (_paramSetting.ParameterValue != null)
                    {
                        foreach (var kv in _paramSetting.ParameterValue)
                        {
                            var val = (kv.Value ?? "").Trim();
                            if (!string.IsNullOrWhiteSpace(val) && set.Contains(val))
                                return val;
                        }
                    }
                }
            }
            catch { /* ignore */ }
            return "";
        }
        private HashSet<string> CollectToolingAttSrcNamesFromCurrentRecipe()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var doc = _recipeForm?.CurrentDoc;
            if (doc == null) return set;

            string norm(string s)
            {
                s = (s ?? "").Trim();
                if (string.IsNullOrWhiteSpace(s)) return "User";
                return s;
            }

            // DefaultTooling
            set.Add(norm(doc.DefaultTooling?.AttSrcName));

            // Per-cell / per-stage toolings
            if (doc.Toolings != null)
            {
                foreach (var tp in doc.Toolings.Values)
                    set.Add(norm(tp?.AttSrcName));
            }

            return set;
        }
        private static async Task WithCancellation(Task task, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (ct.Register(() => tcs.TrySetCanceled(ct)))
            {
                var done = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                await done.ConfigureAwait(false);     // canceled면 여기서 throw
                await task.ConfigureAwait(false);     // 원 Task 예외 전달
            }
        }
        private static async Task<T> WithCancellation<T>(Task<T> task, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (ct.Register(() => tcs.TrySetCanceled(ct)))
            {
                var done = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                if (done == tcs.Task) return await tcs.Task.ConfigureAwait(false);
                return await task.ConfigureAwait(false);
            }
        }
        internal bool EnsurePmRecipeMatchesToolingAttSource(string caller)
        {
            // 1) ParameterSetting에서 선택된 PM 레시피
            var selectedPm = (GetSelectedPmRecipeNameFromParameterSetting() ?? "").Trim();

            // 2) 현재 레시피(RecipeDoc)에서 사용 중인 Att Source 이름들
            var attSrcAll = CollectToolingAttSrcNamesFromCurrentRecipe();
            var nonUser = attSrcAll.Where(x => !string.Equals(x, "User", StringComparison.OrdinalIgnoreCase)).ToArray();

            // Tooling쪽에서 Att Source가 여러 개 섞이면 불가
            if (nonUser.Length > 1)
            {
                var msg =
                    $"{caller} 인터락\r\n\r\n" +
                    "Tooling Att Source에 서로 다른 Powermeter 레시피가 혼용되어 있습니다.\r\n" +
                    $"(현재: {string.Join(", ", nonUser)})\r\n\r\n" +
                    "Att Source를 하나로 통일한 후 다시 시도하세요.";
                AddLog(lstSystem, "[INTERLOCK][PM] " + msg.Replace("\r\n", " | "));
                MessageBox.Show(this, msg, "PM Recipe Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Tooling쪽에서 레시피를 아예 지정 안 한(User) 상태면 불가 (정책)
            string toolingPm = (nonUser.Length == 1) ? nonUser[0] : "User";
            if (string.Equals(toolingPm, "User", StringComparison.OrdinalIgnoreCase))
            {
                var msg =
                    $"{caller} 인터락\r\n\r\n" +
                    "Tooling Att Source가 'User'(레시피 미지정) 상태입니다.\r\n" +
                    "Powermeter 측정/적용을 위해 Tooling Att Source에 Powermeter 레시피를 지정하세요.\r\n\r\n" +
                    $"(ParameterSetting 선택: '{selectedPm}')";
                AddLog(lstSystem, "[INTERLOCK][PM] " + msg.Replace("\r\n", " | "));
                MessageBox.Show(this, msg, "PM Recipe Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ParameterSetting에서 선택된 레시피가 비어있으면 불가
            if (string.IsNullOrWhiteSpace(selectedPm))
            {
                var msg =
                    $"{caller} 인터락\r\n\r\n" +
                    "ParameterSettingForm에서 Powermeter 레시피가 선택되어 있지 않습니다.\r\n" +
                    $"Tooling Att Source는 '{toolingPm}' 입니다.\r\n\r\n" +
                    "ParameterSettingForm에서 동일한 레시피를 선택 후 다시 시도하세요.";
                AddLog(lstSystem, "[INTERLOCK][PM] " + msg.Replace("\r\n", " | "));
                MessageBox.Show(this, msg, "PM Recipe Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 선택 레시피가 실제 Store에 존재하는지 확인(없으면 불가)
            try
            {
                var store = PowerRecipeStore.Open(AppConfig.Current?.PowerRecipesPath);
                var names = store?.ListNames();
                if (names == null || !names.Any(n => string.Equals(n, selectedPm, StringComparison.OrdinalIgnoreCase)))
                {
                    var msg =
                        $"{caller} 인터락\r\n\r\n" +
                        $"선택된 Powermeter 레시피 '{selectedPm}' 을(를) Store에서 찾을 수 없습니다.\r\n" +
                        "레시피 파일/경로(AppConfig.PowerRecipesPath) 또는 선택값을 확인하세요.";
                    AddLog(lstSystem, "[INTERLOCK][PM] " + msg.Replace("\r\n", " | "));
                    MessageBox.Show(this, msg, "PM Recipe Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch { /* store 검증 실패는 일단 무시(원하면 fail로 바꿀 수 있음) */ }

            // 최종 매칭 체크
            if (!string.Equals(selectedPm, toolingPm, StringComparison.OrdinalIgnoreCase))
            {
                var msg =
                    $"{caller} 인터락\r\n\r\n" +
                    "Powermeter 레시피가 서로 일치하지 않습니다.\r\n\r\n" +
                    $"ParameterSetting 선택: '{selectedPm}'\r\n" +
                    $"Tooling Att Source:    '{toolingPm}'\r\n\r\n" +
                    "두 값을 동일하게 맞춘 후 다시 시도하세요.";
                AddLog(lstSystem, "[INTERLOCK][PM] " + msg.Replace("\r\n", " | "));
                MessageBox.Show(this, msg, "PM Recipe Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void EnsureParameterSettingLoaded(bool force = false)
        {
            try
            {
                if (!force)
                {
                    // 이미 로드되어 있고, 너무 자주 로드할 필요 없으면 캐시 사용
                    if (_paramSetting != null && (DateTime.Now - _paramSettingLoadedAt).TotalSeconds < 1.0)
                        return;
                }

                if (!File.Exists(ParameterSettingJsonPath))
                {
                    _paramSetting = null;
                    _paramSettingLoadedAt = DateTime.Now;
                    return;
                }

                var ser = new DataContractJsonSerializer(typeof(ParameterSettingDocument));
                using (var fs = new FileStream(ParameterSettingJsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    _paramSetting = (ParameterSettingDocument)ser.ReadObject(fs);
                }
                _paramSettingLoadedAt = DateTime.Now;
            }
            catch
            {
                // 로드 실패 시에도 시퀀스가 죽지 않도록 null 처리
                _paramSetting = null;
                _paramSettingLoadedAt = DateTime.Now;
            }
        }
        private double GetProcessParamDouble(string key, double fallback)
        {
            EnsureParameterSettingLoaded();
            if (_paramSetting?.ProcessParameter == null) return fallback;
            if (!_paramSetting.ProcessParameter.TryGetValue(key, out var s)) return fallback;
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out v)) return v;
            return fallback;
        }
        private (double x, double y) GetLoadPosition()
        {
            double x = GetProcessParamDouble("MoveToLoadXPos", 0.0);
            double y = GetProcessParamDouble("MoveToLoadYPos", 0.0);
            return (x, y);
        }
        private (double x, double y) GetAlignPosition()
        {
            double x = GetProcessParamDouble("MoveToAlignXPos", 0.0);
            double y = GetProcessParamDouble("MoveToAlignYPos", 0.0);
            return (x, y);
        }
        private (double x, double y) GetPowerMeterPosition(double fallbackX, double fallbackY)
        {
            double x = GetProcessParamDouble("MoveToPWMXPos", fallbackX);
            double y = GetProcessParamDouble("MoveToPWMYPos", fallbackY);
            return (x, y);
        }
        private (double x, double y) GetProcessReadyPosition()
        {
            double x = GetProcessParamDouble("MoveToProcReadyXPos", 0.0);
            double y = GetProcessParamDouble("MoveToProcReadyYPos", 0.0);
            return (x, y);
        }
        private (double x, double y) GetAlignCheckPosition()
        {
            double x = GetProcessParamDouble("MoveToAlignCheckXPos", 0.0);
            double y = GetProcessParamDouble("MoveToAlignCheckYPos", 0.0);
            return (x, y);
        }
        private (double x, double y) GetUnloadPosition()
        {
            double x = GetProcessParamDouble("MoveToUnloadXPos", 0.0);
            double y = GetProcessParamDouble("MoveToUnloadYPos", 0.0);
            return (x, y);
        }
        private bool _stopAllRunning = false;
        private async Task StopAllSequencesAsync()
        {
            if (_stopAllRunning) return;
            _stopAllRunning = true;
            try
            {
                // 1) 현재 어떤 스텝인지 확인
                bool isProcessStep = (_seqCurrent == AutoSequenceId.Process);
                // 2) 공통: 시퀀스 토큰 취소
                try { _procCts?.Cancel(); } catch { }
                // 3) 타워램프 상태 갱신
                _autoSeqRunning = false;
                _towerForm?.RefreshNow();
                if (!isProcessStep)
                {
                    // Process 이외: 강제 Abort로 즉시 정지
                    try
                    {
                        await ForceAbortAsync(TimeSpan.FromSeconds(5));
                    }
                    catch (Exception ex)
                    {
                        AddLog(lstSystem, "[W][StopAll] ForceAbort 실패: " + ex.Message);
                    }
                }
                else
                {
                    // Process 스텝인 경우:
                    // _procCts.Cancel() 으로 내부 루프에서 안전하게 빠져 나오도록만 처리
                    // 나머지 모션/레이저 Stop은 RunProcessSequenceAsync 내부 로직을 따라감
                    AddLog(lstSystem, "[StopAll] Process 스텝 중 Stop 요청 → Soft Stop (토큰 취소만 수행)");
                }
                // 4) UI/탭/수동구동 복구
                OnSequenceRunEnd();
            }
            finally
            {
                _stopAllRunning = false;
            }
        }
        private async Task EnsureAtAlignPositionAsync(bool dryRun)
        {
            var (targetX, targetY) = GetAlignPosition();
            AddLog(lstSystem, $"[SEQ][Align] Check MoveToAlign Pos=({targetX:F3}, {targetY:F3})  DryRun={dryRun}");
            if (dryRun) return;
            double curX = double.NaN;
            double curY = double.NaN;
            try
            {
                curX = _acsMotion.GetPosition(Axis.X);
                curY = _acsMotion.GetPosition(Axis.Y);
            }
            catch
            {
                // 위치 읽기 실패 시에는 무조건 이동하도록 NaN 그대로 둠
            }
            bool needMove =
                double.IsNaN(curX) || double.IsNaN(curY) ||
                Math.Abs(curX - targetX) > X_INPOS_TOL ||
                Math.Abs(curY - targetY) > Y_INPOS_TOL;
            if (!needMove)
            {
                AddLog(lstSystem, "[SEQ][Align] 이미 MoveToAlign 위치에 있음 → 이동 생략");
                return;
            }
            AddLog(lstSystem, $"[SEQ][Align] MoveToAlign 위치로 이동: Cur=({curX:F3}, {curY:F3}) → Target=({targetX:F3}, {targetY:F3})");

            // 토큰 없으면 새로 생성 (Semi-Auto에서 직접 호출될 때 대비)
            if (_procCts == null || !_procCts.Token.CanBeCanceled)
            {
                _procCts?.Cancel();
                _procCts = new CancellationTokenSource();
            }
            var ct = _procCts.Token;

            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);

            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, "[SEQ][Align] MoveToAlign 위치 도달");
        }
        private bool EnsureRecipeSelectedForAuto(string caller)
        {
            var name = _currentRecipeName ?? string.Empty;
            // 필요하면 "NO NAME", "NoName" 등 변형도 여기서 막아준다.
            if (string.IsNullOrWhiteSpace(name) ||
                name.Equals("NO NAME", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("NONAME", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    $"{caller} 실행 전에 유효한 Recipe를 먼저 선택하세요.\r\n(현재: \"No Name\")",
                    "Recipe",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private async Task RunSeq_LoadAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("Load")) return;
            EnsureParameterSettingLoaded();

            var (targetX, targetY) = GetLoadPosition();
            AddLog(lstSystem, $"[SEQ-LOAD] Target=({targetX:F3}, {targetX:F3})  DryRun={dryRun}");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToLoad");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToLoad");
            LogMotionProfile("MoveToLoad", profX, profY);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, $"[SEQ-LOAD] 도착 완료 @({targetX:F3}, {targetY:F3})");
        }
        private async Task RunSeq_MoveToAlignAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("MoveToAlign")) return;
            EnsureParameterSettingLoaded();

            var (targetX, targetY) = GetAlignPosition();
            AddLog(lstSystem, $"[SEQ][MoveToAlign] Target=({targetX:F3}, {targetY:F3})  DryRun={dryRun}");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToAlign");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToAlign");
            LogMotionProfile("MoveToAlign", profX, profY);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, $"[SEQ][MoveToAlign] 도착 완료 @({targetX:F3}, {targetY:F3})");
        }
        private async Task RunSeq_AlignAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("Align")) return;
            EnsureParameterSettingLoaded();

            if (_procCts == null || !_procCts.Token.CanBeCanceled)
            {
                _procCts?.Cancel();
                _procCts = new CancellationTokenSource();
            }
            var ct = _procCts.Token;
            try
            {
                AddLog(lstSystem, $"[SEQ][Align] Start (DryRun={dryRun})");

                // 1) MoveToAlign 위치 보장
                await EnsureAtAlignPositionAsync(dryRun);
                if (dryRun)
                {
                    AddLog(lstSystem, "[SEQ][Align] DryRun → Vision Align 호출 생략");
                    return;
                }

                // 2) Vision Align 요청
                var vision = GetVisionSessionOrWarn();
                if (vision == null) throw new InvalidOperationException("Vision 연결이 없습니다.");
                AddLog(lstRecipeScanLog, "[3000][SEQ] Glass Align 요청...");
                var rsp = await VisionRpc.Request_GlassAlignAsync(vision, timeoutMs: 15000);
                bool ok = (rsp.nResult == 1);
                AddLog(lstRecipeScanLog, ok ? "[3000][SEQ] 결과: OK" : "[3000][SEQ] 결과: NG");
                if (!ok) throw new Exception("Glass Align 실패 응답(NG)");
            }
            catch (OperationCanceledException)
            {
                AddLog(lstSystem, "[SEQ][Align] Canceled");
            }
            catch (TimeoutException ex)
            {
                AddLog(lstSystem, "[W][SEQ][Align] " + ex.Message);
                MessageBox.Show(this, ex.Message, "Align", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstSystem, "[E][SEQ][Align] " + ex.Message);
                MessageBox.Show(this, "Align 시퀀스 실패: " + ex.Message, "Align", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task RunSeq_PowerMeterAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("PowerMeter")) return;
            EnsureParameterSettingLoaded();
            if (_procCts == null || !_procCts.Token.CanBeCanceled)
            {
                _procCts?.Cancel();
                _procCts = new CancellationTokenSource();
            }
            var ct = _procCts.Token;
            AddLog(lstSystem, $"[SEQ-PM] Start (DryRun={dryRun})");

            // DryRun이면 실제 1010 요청/이동을 생략
            if (dryRun)
            {
                AddLog(lstSystem, "[SEQ-PM] DryRun → Move/1010 생략");
                return;
            }

            // 1) ParameterSetting에서 "사용할 Powermeter 레시피 이름"만 가져온다.
            var pmRecipeName = GetPowerMeterRecipeNameFromParam();
            if (string.IsNullOrWhiteSpace(pmRecipeName))
            {
                MessageBox.Show(this,
                    "PowerMeter 시퀀스를 실행할 레시피가 선택되어 있지 않습니다.\r\n" +
                    "ParameterSettingForm에서 PowerMeterRecipeName을 선택하세요.",
                    "PowerMeter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Powermeter 레시피 로드 (PowerMeterEditorForm에서 만든 데이터)
            PowerOnlyDoc pmDoc;
            try
            {
                var store = PowerRecipeStore.Open(null); // 기본: ConfigRoot/PowerRecipes.json
                pmDoc = store.Load(pmRecipeName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Powermeter 레시피 로드 실패: {pmRecipeName}\r\n{ex.Message}",
                    "PowerMeter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) 주파수 기준으로 테이블 선택 (현재 Recipe의 DefaultTooling.Freq를 기준)
            var curRecipe = _recipeForm?.CurrentDoc;
            double preferredFreq = curRecipe?.DefaultTooling?.Freq ?? 0.0;

            var table = pmDoc?.PickTable(preferredFreq);
            if (table == null || table.Rows == null || table.Rows.Count == 0)
            {
                MessageBox.Show(this,
                    $"Powermeter 레시피에 PowerTable이 없습니다.\r\nRecipe={pmRecipeName}, Freq={preferredFreq}",
                    "PowerMeter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4) Stage를 Powermeter Start 위치로 이동
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);

            // 어떤 Powermeter 레시피로 측정했는지 로그(요구사항)
            AddLog(lstRecipeScanLog, $"[SEQ-PM] Using PM Recipe='{pmRecipeName}', preferredFreq={preferredFreq:F3}, " +
                $"pickedTableFreq={table.Frequency:F3}, pts={table.Rows.Count}");

            // 위치/속도/가속: ParameterSetting 기준
            var (targetX, targetY) = GetPowerMeterPosition(pmDoc.PowerMeterStartX, pmDoc.PowerMeterStartY);

            // 속도/가속(Dec는 없으면 Acc로 대체): 기존에 이미 쓰고 있는 GetMotionProfileFromParam 재사용
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToPWM");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToPWM");
            LogMotionProfile("MoveToPWM", profX, profY);
            AddLog(lstSystem, $"[SEQ-PM] MoveToPWM Target=({targetX:F3}, {targetY:F3})");
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 20000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 20000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);

            // 5) 1010에 넣을 테이블 생성
            var st = new NetCommon.ST_POWER_TABLE[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var r = table.Rows[i];
                st[i] = new NetCommon.ST_POWER_TABLE
                {
                    dFrequency = table.Frequency,   // ferredFreq
                    dTargetPower = r.TargetW,       // W
                    dAttMinPos = r.AttMin,          // Min Att Pos
                    dAttMaxPos = r.AttMax           // Max Att Pos
                };
            }

            // 6) ScanNo / Range는 기존 규칙 그대로
            int scanNo = _opticForm?.CurrentScanNo ?? 1;
            double rangePercent = GetPowerMeterRangePercentFromParam(fallback: 0.0);

            // 7) 1010 실행 (버튼과 동일 공통 함수 사용)
            var rsp = await DoPowerMeterMeas1_1010_Async(
                scanNo: scanNo,
                rangePercent: rangePercent,
                tables: st,
                timeoutMs: 1_200_000,
                ct: ct);

            AddLog(lstSystem, $"[SEQ-PM] Done. nResult={rsp.bResult}");
        }
        private async Task RunSeq_ProcessReadyAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("ProcessReady")) return;
            EnsureParameterSettingLoaded();

            var (targetX, targetY) = GetProcessReadyPosition();
            AddLog(lstSystem, $"[SEQ-PROC-READY] Target=({targetX:F3}, {targetY:F3})  DryRun={dryRun}");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToProcReady");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToProcReady");
            LogMotionProfile("MoveToProcReady", profX, profY);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, $"[SEQ-PROC-READY] Real 시퀀스 완료 (가공 대기 위치 도달 @({targetX:F3}, {targetY:F3}))");
        }
        private async Task RunSeq_ProcessAsync(bool dryRun)
        {
            var origin = _recipeForm?.OpenOrigin ?? RecipeForm.RecipeOpenOrigin.Local;
            if (origin != RecipeForm.RecipeOpenOrigin.Scan)
            {
                MessageBox.Show(this,
                    "가공(Process) 동작은 Scan 레시피에서만 실행할 수 있습니다.\r\n" +
                    "Scan 탭에서 레시피를 선택한 후 다시 시도하세요.",
                    "Recipe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!EnsureSemiOrAuto("Process")) return;
            AddLog(lstSystem, $"[SEQ-PROC] Start (DryRun={dryRun})");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            if (_opticForm == null)
            {
                AddLog(lstSystem, "[SEQ-PROC][E] OpticForm가 없습니다.");
                return;
            }

            bool oneStep = GetAutoOneStepProcessFromParameterSetting();

            // AllStep(=전체 순차 가공)일 때는 "1Step 로직을 라인별 반복"으로 구현
            if (!oneStep)
            {
                // 기준 plan 1회 생성 (전체 라인수/방향성 등 추출용)
                var basePlan = _opticForm.BuildRunPlanFromUI();
                if (basePlan == null)
                {
                    MessageBox.Show(this, "유효한 ProcessRunPlan이 없습니다.\r\n옵션/레이아웃을 먼저 설정하세요.",
                        "Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int totalLines = basePlan.IsYMajor ? Math.Max(1, basePlan.Lines) : Math.Max(1, basePlan.HolesPerLine);
                AddLog(lstSystem, $"[SEQ-PROC] AllStep → TotalLines={totalLines}");

                // 1라인~N라인 순차 가공
                for (int line1 = 1; line1 <= totalLines; line1++)
                {
                    _procCts.Token.ThrowIfCancellationRequested();

                    // 매 라인마다 fresh plan (StartIndex/StepCount 등 오염 방지)
                    var plan = _opticForm.BuildRunPlanFromUI();
                    if (plan == null) throw new InvalidOperationException("ProcessRunPlan build failed");

                    // 라인 지정 = 기존 1Step 방식 재사용
                    ApplyOneStepProcessPlan(plan, line1);

                    // Semi/Auto 속도 override 적용
                    ApplyAutoSemiProcessSpeeds(plan);

                    // UI: 현재 가공 라인 표시(연보라)
                    try { _opticForm.HighlightProcessingLine(line1); } catch { }

                    AddLog(lstSystem, $"[SEQ-PROC] AllStep → Run Line {line1}/{totalLines}");
                    await RunProcessSequenceAsync(plan);
                }
                AddLog(lstSystem, "[SEQ-PROC] AllStep → Done (All lines processed)");
                return;
            }
            // OneStep 동작 유지
            var plan1 = _opticForm.BuildRunPlanFromUI();
            if (plan1 == null)
            {
                MessageBox.Show(this, "유효한 ProcessRunPlan이 없습니다.\r\n옵션/레이아웃을 먼저 설정하세요.",
                    "Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ApplyOneStepProcessPlan(plan1, _autoNextProcessLine1);
            ApplyAutoSemiProcessSpeeds(plan1);
            // UI: 현재 가공 라인 표시(연보라)
            try { _opticForm.HighlightProcessingLine(_autoNextProcessLine1); } catch { }
            await RunProcessSequenceAsync(plan1);
            // 성공적으로 실행 후 다음 라인으로 증가(1Step일 때만)
            int totalLines1 = plan1.IsYMajor ? Math.Max(1, plan1.Lines) : Math.Max(1, plan1.HolesPerLine);
            _autoNextProcessLine1++;
            if (_autoNextProcessLine1 > totalLines1) _autoNextProcessLine1 = 1;
            AddLog(lstSystem, $"[SEQ-PROC] OneStep → NextProcessLine={_autoNextProcessLine1}/{totalLines1}");
        }
        private async Task RunSeq_InspectionAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("Inspection")) return;
            EnsureParameterSettingLoaded();

            AddLog(lstSystem, $"[SEQ][Inspection] Start (DryRun={dryRun})");

            if (dryRun)
            {
                AddLog(lstSystem, "[SEQ][Inspection] DryRun → Inspection 생략");
                return;
            }

            if (_recipeForm == null)
            {
                MessageBox.Show(this,
                    "RecipeForm이 초기화되지 않았습니다.",
                    "Inspection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_procCts == null || !_procCts.Token.CanBeCanceled)
            {
                try { _procCts?.Cancel(); } catch { }
                try { _procCts?.Dispose(); } catch { }
                _procCts = new CancellationTokenSource();
            }

            var ct = _procCts.Token;

            bool stepByStepAllInspection = GetStepByStepAllInspectionFromParameterSetting();

            if (stepByStepAllInspection)
            {
                bool isFullAuto = _seqIsFullAuto || GetProgramModeFromUi() == ProgramMode.Auto;

                AddLog(lstSystem,
                    $"[SEQ][Inspection] ParameterSetting StepByStepAllInspection=TRUE → " +
                    $"전셀 Step-by-Step Review 검사 실행, ApplyConfirm={(isFullAuto ? "AUTO_APPLY" : "ASK_USER")}");

                await _recipeForm.DoSequenceStepByStepAllInspectionAsync(
                    confirmApply: !isFullAuto,
                    externalToken: ct);

                AddLog(lstSystem, "[SEQ][Inspection] StepByStepAllInspection Done");
                return;
            }

            // 기존 동작 유지
            var mode = GetAutoInspectionModeFromParameterSetting();

            int vecC = 0;

            _autoNextInspectLine1 = await _recipeForm.DoAutoInspectionAsync(
                mode,
                _autoNextInspectLine1,
                vecC);

            AddLog(lstSystem, $"[SEQ][Inspection] Done (Mode={mode}, NextLine={_autoNextInspectLine1})");
        }
        private async Task RunSeq_MoveToAlignCheckAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("MoveToAlignCheck")) return;
            EnsureParameterSettingLoaded();

            var (targetX, targetY) = GetAlignCheckPosition();
            AddLog(lstSystem, $"[SEQ-MOVE-ALIGN-CHK] Target=({targetX:F3}, {targetY:F3})  DryRun={dryRun}");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToAlignCheck");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToAlignCheck");
            LogMotionProfile("MoveToAlignCheck", profX, profY);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, $"[SEQ-MOVE-ALIGN-CHK] Real 시퀀스 완료 @({targetX:F3}, {targetY:F3})");
        }
        private async Task RunSeq_AlignCheckAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("2nd Align")) return;
            AddLog(lstSystem, $"[SEQ-ALIGN-CHK] Start (DryRun={dryRun})");
            if (dryRun) return;

            _procCts?.Cancel();
            _procCts = new CancellationTokenSource();
            var vision = GetVisionSessionOrWarn();
            if (vision == null) return;

            try
            {
                AddLog(lstRecipeScanLog, "[3001][SEQ] 2nd Align 요청...");
                var rsp = await VisionRpc.Request_SecondAlignAsync(vision, timeoutMs: 15000);
                var ok = (rsp.nResult == 1);
                AddLog(lstRecipeScanLog, ok ? "[3001][SEQ] 결과: OK" : "[3001][SEQ] 결과: NG");
                if (!ok) MessageBox.Show(this, "2nd Align 실패 응답(NG).", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AddLog(lstSystem, "[SEQ-ALIGN-CHK] Real 시퀀스 완료");
            }
            catch (TimeoutException)
            {
                AddLog(lstRecipeScanLog, "[W][3001][SEQ] 2nd Align 타임아웃");
                MessageBox.Show(this, "2nd Align 타임아웃.", "Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog(lstRecipeScanLog, "[E][3001][SEQ] " + ex.Message);
                MessageBox.Show(this, "2nd Align 요청 실패: " + ex.Message, "Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task RunSeq_MoveToUnloadAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("MoveToUnload")) return;
            EnsureParameterSettingLoaded();

            var (targetX, targetY) = GetUnloadPosition();
            AddLog(lstSystem, $"[SEQ-MOVE-UNLOAD] Target=({targetX:F3}, {targetY:F3})  DryRun={dryRun}");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await _acsMotion.ServoOnAsync(Axis.X);
            await _acsMotion.ServoOnAsync(Axis.Y);
            var profX = GetMotionProfileFromParam(Axis.X, "MoveToUnload");
            var profY = GetMotionProfileFromParam(Axis.Y, "MoveToUnload");
            LogMotionProfile("MoveToUnload", profX, profY);
            var t1 = MoveAbsAndWait_SeqAsync(Axis.X, targetX, X_INPOS_TOL, 30000, ct, profX.Velocity, profX.Acceleration, profX.Deceleration);
            var t2 = MoveAbsAndWait_SeqAsync(Axis.Y, targetY, Y_INPOS_TOL, 30000, ct, profY.Velocity, profY.Acceleration, profY.Deceleration);
            await Task.WhenAll(t1, t2);
            AddLog(lstSystem, $"[SEQ-MOVE-UNLOAD] Real 시퀀스 완료 @({targetX:F3}, {targetY:F3})");
        }
        private async Task RunSeq_UnloadAsync(bool dryRun)
        {
            if (!EnsureSemiOrAuto("Unload")) return;
            AddLog(lstSystem, $"[SEQ-UNLOAD] Start (DryRun={dryRun})");
            if (dryRun) return;

            if (_procCts == null) _procCts = new CancellationTokenSource();
            var ct = _procCts.Token;
            await Task.Delay(200, ct);
            AddLog(lstSystem, "[SEQ-UNLOAD] Real 시퀀스 완료 (Unload 완료)");
        }
        private static bool SeqNeedsScan(AutoSequenceId seq)
        {
            // 현재 코드 기준: Process에서 Scan과 통신(가공 실행/레시피 전송 등)할 가능성이 큼
            // 필요하면 ProcessReady/Inspection 등도 확장
            return seq == AutoSequenceId.Process || seq == AutoSequenceId.PowerMeter;
        }
        private static bool SeqNeedsScanRecipeOrigin(AutoSequenceId seq)
        {
            // "Scan 탭 레시피" 여야만 가능한 시퀀스
            return seq == AutoSequenceId.Process;
        }
        private static bool SeqNeedsVision(AutoSequenceId seq)
        {
            // 현재 코드 기준: VisionRpc 호출이 들어가는 시퀀스만 Vision 필요
            return seq == AutoSequenceId.Align
                || seq == AutoSequenceId.AlignCheck
                || seq == AutoSequenceId.Inspection;
        }
        // 여러 시퀀스(Full Auto) 기준으로 필요한 링크를 미리 점검
        private bool EnsureClientLinksForSequences(IEnumerable<AutoSequenceId> seqs, string caller)
        {
            if (seqs == null) seqs = Array.Empty<AutoSequenceId>();

            bool needScan = seqs.Any(SeqNeedsScan);
            bool needVision = seqs.Any(SeqNeedsVision);

            // 네가 이미 관리 중인 LED/상태 플래그 그대로 사용
            if (needScan && !_scanCommOk)
            {
                MessageBox.Show(this,
                    $"{caller} 실행 불가: Scan 클라이언트 통신 연결이 없습니다.\r\n" +
                    $"Scan PC 프로그램/네트워크/포트를 확인하세요.",
                    caller, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (needVision && !_visionCommOk)
            {
                MessageBox.Show(this,
                    $"{caller} 실행 불가: Vision 클라이언트 통신 연결이 없습니다.\r\n" +
                    $"Vision PC 프로그램/네트워크/포트를 확인하세요.",
                    caller, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        // 단일 시퀀스(Semi-Auto) 기준 점검
        private bool EnsureClientLinksForSequence(AutoSequenceId seq, string caller)
        {
            return EnsureClientLinksForSequences(new[] { seq }, caller);
        }
        private void ApplyAutoSemiProcessSpeeds(OpticOperationForm.ProcessRunPlan plan)
        {
            if (plan == null) return;

            plan.Pattern = OpticOperationForm.FlyPattern.OneWay;
            plan.ProcessSpeedX = 0.0;
            plan.ProcessSpeedY = 0.0;

            double y = GetProcessParamDouble("ProcessYSpeed", 0.0);
            double x = GetProcessParamDouble("MoveToProcReadyXSpeed", 0.0);
            if (y > 0) plan.ProcessSpeedY = y;
            if (x > 0) plan.ProcessSpeedX = x;
        }
        private string GetPowerMeterRecipeNameFromParam()
        {
            // ParameterSettingForm은 여기 값만 바꾼다.
            // (나머지 Power table/StartXY 등은 PowerMeterEditorForm에서만 편집)
            EnsureParameterSettingLoaded();
            if (_paramSetting?.ProcessParameter == null) return null;
            if (_paramSetting.ProcessParameter.TryGetValue("PowerMeterRecipeName", out var s))
                return (s ?? "").Trim();
            return null;
        }

        private double GetPowerMeterRangePercentFromParam(double fallback = 0.0)
        {
            // 필요하면 ParameterSettingForm에서 제공. 없으면 0으로 운용(기존과 동일)
            EnsureParameterSettingLoaded();
            if (_paramSetting?.ProcessParameter == null) return fallback;
            if (!_paramSetting.ProcessParameter.TryGetValue("PowerMeterRangePercent", out var s)) return fallback;
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out v)) return v;
            return fallback;
        }
        private async Task<NetCommon.ST_POWER_MEAS1_RSP> DoPowerMeterMeas1_1010_Async(
            int scanNo,
            double rangePercent,
            NetCommon.ST_POWER_TABLE[] tables,
            int timeoutMs,
            CancellationToken ct)
        {
            // 시퀀스/버튼 공용이므로 여기서 모드 강제 체크는 "호출자"가 하도록 두는게 가장 깔끔함.
            // (기존 1010 람다와 동일 패턴 유지)

            var scan = GetScanSessionOrWarn();
            if (scan == null) throw new InvalidOperationException("Scan 연결이 없습니다.");

            AddLog(lstRecipeScanLog, $"[PM][1010] req → scanNo={scanNo}, range={rangePercent}, pts={tables?.Length ?? 0}");

            // 1) Idle 대기
            await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 5000, ct: ct);

            // 2) 1010 요청
            // (ct 취소를 강하게 반영하고 싶으면 WhenAny로 감싸도 되지만, 기존 스타일과 동일하게 유지)
            var rsp = await StageWin.Driver.Network.Packets.ScanRpc
                .Request_PowerMeter_Measurement1Async(scan, scanNo, rangePercent, tables, timeoutMs: timeoutMs);

            // 3) Idle 복귀 대기(예외는 무시 가능 - 기존 코드 동일)
            try { await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 10000, ct: ct); } catch { }

            return rsp;
        }
        private async Task<NetCommon.ST_POWER_MEAS2_RSP> DoPowerMeterMeas2_1011_Async(
            int scanNo,
            double rangePercent,
            double freq,
            double att,
            int timeoutMs,
            CancellationToken ct)
        {
            var scan = GetScanSessionOrWarn();
            if (scan == null) throw new InvalidOperationException("Scan 연결이 없습니다.");

            AddLog(lstRecipeScanLog, $"[PM][1011] req → scanNo={scanNo}, range={rangePercent}, freq={freq}, att={att}");

            await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 5000, ct: ct);

            var rsp = await StageWin.Driver.Network.Packets.ScanRpc
                .Request_PowerMeter_Measurement2Async(scan, scanNo, rangePercent, freq, att, timeoutMs: timeoutMs);

            try { await WaitForScanStateAsync(scanNo, ScanRunState.Idle, timeoutMs: 10000, ct: ct); } catch { }

            return rsp;
        }
        private AutoInspectionMode GetAutoInspectionModeFromParameterSetting()
        {
            EnsureParameterSettingLoaded();
            var pv = _paramSetting?.ParameterValue;
            var pp = _paramSetting?.ProcessParameter;

            bool ReadBool(string key)
            {
                string s = null;
                if (pv != null && pv.TryGetValue(key, out var v1)) s = v1;
                else if (pp != null && pp.TryGetValue(key, out var v2)) s = v2;
                s = (s ?? "").Trim();
                if (string.IsNullOrWhiteSpace(s)) return false;
                // "true/false", "1/0", "True" 등 허용
                if (bool.TryParse(s, out var b)) return b;
                if (int.TryParse(s, out var i)) return (i != 0);
                return false;
            }
            string ReadStr(string key)
            {
                if (pv != null && pv.TryGetValue(key, out var v1)) return (v1 ?? "").Trim();
                if (pp != null && pp.TryGetValue(key, out var v2)) return (v2 ?? "").Trim();
                return "";
            }
            // 1) 라디오 버튼 저장 키(가장 가능성 높은 케이스)
            // ParameterSettingForm에서 저장할 때 어떤 키로 저장했는지에 따라 후보를 넉넉히 잡음(안전)
            bool oneLine =
                ReadBool("rbtn1LineInspection") ||
                ReadBool("OneLineInspection") ||
                ReadBool("Inspection_OneLine");
            bool allLine =
                ReadBool("rbtnAllInspection") ||
                ReadBool("AllInspection") ||
                ReadBool("Inspection_All");

            if (oneLine && !allLine) return AutoInspectionMode.OneLine;
            if (allLine && !oneLine) return AutoInspectionMode.AllLines;
            // 2) 문자열 모드 키가 있는 경우
            var mode = ReadStr("InspectionMode");
            if (!string.IsNullOrWhiteSpace(mode))
            {
                mode = mode.ToUpperInvariant();
                if (mode.Contains("ONE") || mode.Contains("1LINE")) return AutoInspectionMode.OneLine;
                if (mode.Contains("ALL")) return AutoInspectionMode.AllLines;
            }
            // 3) fallback: 안전하게 OneLine (전체 덮어쓰기/시간 과다 방지)
            return AutoInspectionMode.OneLine;
        }
        private bool GetStepByStepAllInspectionFromParameterSetting()
        {
            EnsureParameterSettingLoaded();

            var pv = _paramSetting?.ParameterValue;
            var pp = _paramSetting?.ProcessParameter;

            string ReadStr(string key)
            {
                if (pp != null && pp.TryGetValue(key, out var v2)) return (v2 ?? "").Trim();
                if (pv != null && pv.TryGetValue(key, out var v1)) return (v1 ?? "").Trim();
                return "";
            }

            bool ReadBool(string key)
            {
                string s = ReadStr(key);
                if (string.IsNullOrWhiteSpace(s)) return false;

                if (bool.TryParse(s, out var b)) return b;
                if (int.TryParse(s, out var i)) return i != 0;

                s = s.Trim().ToUpperInvariant();

                return s == "Y"
                    || s == "YES"
                    || s == "ON"
                    || s == "CHECKED"
                    || s == "ENABLE"
                    || s == "ENABLED"
                    || s == "STEP"
                    || s == "STEP_ALL"
                    || s == "STEP_ALL_WHOLELINE"
                    || s == "STEPBYSTEPALLINSPECTION";
            }

            if (ReadBool("StepByStepAllInspection")) return true;
            if (ReadBool("rbtnStepbyStepAllInspection")) return true;
            if (ReadBool("InspectionStepByStepAll")) return true;
            if (ReadBool("StepAllWholeLineInspection")) return true;

            string mode = ReadStr("InspectionMode").ToUpperInvariant();

            if (mode.Contains("STEP") && (mode.Contains("ALL") || mode.Contains("WHOLE")))
                return true;

            return false;
        }
        private bool GetAutoOneStepProcessFromParameterSetting()
        {
            EnsureParameterSettingLoaded();
            var pv = _paramSetting?.ParameterValue;
            var pp = _paramSetting?.ProcessParameter;

            bool ReadBool(string key)
            {
                string s = null;
                if (pv != null && pv.TryGetValue(key, out var v1)) s = v1;
                else if (pp != null && pp.TryGetValue(key, out var v2)) s = v2;

                s = (s ?? "").Trim();
                if (string.IsNullOrWhiteSpace(s)) return false;

                if (bool.TryParse(s, out var b)) return b;
                if (int.TryParse(s, out var i)) return (i != 0);
                return false;
            }
            string ReadStr(string key)
            {
                if (pv != null && pv.TryGetValue(key, out var v1)) return (v1 ?? "").Trim();
                if (pp != null && pp.TryGetValue(key, out var v2)) return (v2 ?? "").Trim();
                return "";
            }
            // 1) 라디오 bool 저장 케이스 대응
            bool one =
                ReadBool("rbtn1StepProcess") ||
                ReadBool("OneStepProcess") ||
                ReadBool("Process_OneStep");
            bool all =
                ReadBool("rbtnAllStepProcess") ||
                ReadBool("AllStepProcess") ||
                ReadBool("Process_AllStep");

            if (one && !all) return true;
            if (all && !one) return false;
            // 2) 문자열 모드 키
            var mode = ReadStr("StepProcessMode"); // ParameterSettingForm에서 ProcessParameter:StepProcessMode로 저장됨
            if (!string.IsNullOrWhiteSpace(mode))
            {
                mode = mode.ToUpperInvariant();
                if (mode.Contains("ONE") || mode.Contains("1STEP") || mode.Contains("1LINE")) return true;
                if (mode.Contains("ALL")) return false;
            }
            // 3) fallback: 안전하게 1Step (시간/리스크 최소화)
            return true;
        }
        private void ApplyOneStepProcessPlan(OpticOperationForm.ProcessRunPlan plan, int line1)
        {
            if (plan == null) return;

            int totalLines = plan.IsYMajor ? Math.Max(1, plan.Lines) : Math.Max(1, plan.HolesPerLine);
            line1 = Math.Max(1, Math.Min(totalLines, line1));

            // Order: index -> (Row, Col)  (Row/Col은 1-based로 쓰고 있는 전제)
            var order = plan.Order ?? Array.Empty<(int Row, int Col)>();
            if (order.Length == 0)
            {
                // Order가 없으면 기존 plan 유지(최소 변경)
                plan.FlyLineCount = 1;
                plan.StepWholeLine = true;
                plan.StepCount = plan.IsYMajor ? Math.Max(1, plan.HolesPerLine) : Math.Max(1, plan.Lines);
                return;
            }
            // 해당 라인의 첫 StageIndex 찾기
            int stageIdx = -1;
            for (int i = 0; i < order.Length; i++)
            {
                int lineOfI = plan.IsYMajor ? order[i].Row : order[i].Col;
                if (lineOfI == line1) { stageIdx = i; break; }
            }
            if (stageIdx < 0) stageIdx = 0;
            // StartIndex 재설정(벡터 포함)
            int vecPerStage = Math.Max(1, plan.VecPerStage);
            plan.StartIndex = stageIdx * vecPerStage;
            // “1 라인만 가공”
            plan.FlyLineCount = 1;
            // StepByStep 모드에서도 “라인 끝까지”로 강제
            plan.StepWholeLine = true;
            plan.StepCount = plan.IsYMajor ? Math.Max(1, plan.HolesPerLine) : Math.Max(1, plan.Lines);
        }
        #endregion

        #region ======= 기타 함수들... =======
        private async void OnFormClosedAsync(object sender, FormClosedEventArgs e)
        {
            try { _procCts?.Cancel(); } catch { }
            try
            {
                var scan = Find(Role.Scan);
                if (scan != null)
                {
                    int scanNo = _opticForm?.CurrentScanNo ?? 0;
                    await SendScanStopAsync(scanNo);
                }
            }
            catch { /* ignore */ }
            try { _ldsHub?.Stop(); } catch { }
            StopServer();
            try
            {
                _fswLocal?.Dispose();
                _fswScan?.Dispose();
                _fswDebounceLocal?.Stop();
                _fswDebounceScan?.Stop();
            }
            catch { }
        }
        private TabControl GetMainTabControl()
        {
            if (_mainTab != null && !_mainTab.IsDisposed)
                return _mainTab;

            // tabOverview 가 올라가 있는 부모가 실제 메인 TabControl 이라고 가정
            if (tabOverview != null)
                _mainTab = tabOverview.Parent as TabControl;

            return _mainTab;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var tc = GetMainTabControl();
            if (tc != null)
            {
                tc.Selecting -= TabMain_Selecting;
                tc.Selecting += TabMain_Selecting;
            }
        }
        private void TabMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var tc = GetMainTabControl();
            if (tc == null) return;

            // Full Auto일 때만 탭 이동 제한
            if (_seqRunning && _seqIsFullAuto && !IsAllowedTabWhileRunning(e.TabPage))
            {
                e.Cancel = true;
            }
        }
        private static void EnableDoubleBuffer(Control c)
        {
            if (c == null) return;

            c.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(c, true, null);
        }
        private void ForceMode(ProgramMode m)
        {
            _lastConfirmedMode = m;   // 상태 갱신
            SetModeUI(m);             // 라디오 + 캡션 동기화
        }
        private ProgramMode GetProgramModeFromUi()
        {
            try
            {
                if (rbtnModeManual?.Checked == true) return ProgramMode.Manual;
                if (rbtnModeSemiAuto?.Checked == true) return ProgramMode.SemiAuto;
                if (rbtnModeAuto?.Checked == true) return ProgramMode.Auto;
                return ProgramMode.Manual; // 안전하게
            }
            catch { return ProgramMode.Manual; }
        }
        private bool TryApplyProgramMode(ProgramMode want, out string reason)
        {
            try
            {
                // TODO: 실제 장비/시퀀스/인터락에 모드를 통지해야 한다면 여기서 처리
                // ex) Scan/Vision RPC, Safety Gate 등
                reason = null;
                return true; // 처리 OK
            }
            catch (Exception ex)
            {
                reason = ex.Message;
                return false; // 처리 실패
            }
        }
        private void SetModeUI(ProgramMode m)
        {
            _changingModeProgrammatically = true;
            try
            {
                if (rbtnModeManual != null) rbtnModeManual.Checked = (m == ProgramMode.Manual);
                if (rbtnModeSemiAuto != null) rbtnModeSemiAuto.Checked = (m == ProgramMode.SemiAuto);
                if (rbtnModeAuto != null) rbtnModeAuto.Checked = (m == ProgramMode.Auto);
            }
            finally { _changingModeProgrammatically = false; }
            try { if (gbMode != null) gbMode.Text = $"OperationMode : {m}"; } catch { /* ignore */ }
        }
        private bool EnsureSemiOrAuto(string caption)
        {
            var m = GetProgramModeFromUi();
            if (m == ProgramMode.Manual)
            {
                MessageBox.Show(this, $"{caption} 기능은 Semi/Auto에서만 동작합니다.", "Mode 제한", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private void WireTabLockGuard()
        {
            var tc = tabOverview?.Parent as TabControl;
            if (tc == null || _overviewForm == null) return;
            // Selecting 핸들러 중복 방지
            tc.Selecting -= Tc_Selecting_LockGuard;
            tc.Selecting += Tc_Selecting_LockGuard;
            _overviewForm.UiLockChanged += locked => ApplyTabLock(tc, locked);
            // 초기 1회 적용
            ApplyTabLock(tc, _overviewForm.UiLocked);
        }
        private void ApplyTabLock(TabControl tc, bool locked)
        {
            if (tc == null) return;
            // 허용 탭: Overview + (이름/텍스트에 "Log" 포함 탭 하나)
            var allowed = new HashSet<TabPage>();
            if (tabOverview != null) allowed.Add(tabOverview);
            var logTab = tc.TabPages.Cast<TabPage>().FirstOrDefault(p =>
                    (p.Name ?? "").IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Text ?? "").IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0);
            if (logTab != null) allowed.Add(logTab);
            if (tabIO != null) allowed.Add(tabIO);
            foreach (TabPage p in tc.TabPages) p.Enabled = !locked || allowed.Contains(p);
            // 이미 잠금인데 허용되지 않은 탭이 선택돼 있으면 Overview로 강제 전환
            if (locked && tc.SelectedTab != null && !allowed.Contains(tc.SelectedTab) && tabOverview != null) tc.SelectedTab = tabOverview;
        }
        private void Tc_Selecting_LockGuard(object sender, TabControlCancelEventArgs e)
        {
            try
            {
                if (_overviewForm?.UiLocked == true)
                {
                    bool isAllowed = e.TabPage == tabOverview ||
                        (e.TabPage.Name ?? "").IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (e.TabPage.Text ?? "").IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!isAllowed) e.Cancel = true;
                }
            }
            catch { /* ignore */ }
        }
        private void SetupGridCommon(DataGridView gv, bool defaultRowSelect = false)
        {
            gv.AllowUserToAddRows = false;
            gv.AllowUserToDeleteRows = false;
            gv.AllowUserToResizeColumns = false;
            gv.AllowUserToResizeRows = false;
            gv.MultiSelect = false;
            gv.RowHeadersVisible = false;
            gv.SelectionMode = defaultRowSelect ? DataGridViewSelectionMode.FullRowSelect : DataGridViewSelectionMode.CellSelect;
            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gv.RowTemplate.Height = 24;
            gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gv.ScrollBars = ScrollBars.Both;
            gv.BackgroundColor = Color.White;

            var headerCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("맑은 고딕", 9F, FontStyle.Bold)
            };
            gv.ColumnHeadersDefaultCellStyle = headerCenter;
            var cell = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };
            gv.DefaultCellStyle = cell;
        }
        private void SetupAlarmGridColumns(DataGridView gv)
        {
            gv.AutoGenerateColumns = false;
            gv.Columns.Clear();
            var colTime = new DataGridViewTextBoxColumn
            {
                Name = "Time",
                HeaderText = "Time",
                DataPropertyName = "Time",
                Width = 200,
                ValueType = typeof(DateTime),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm:ss.fff" // ← 올바른 포맷
                }
            };
            var colCode = new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "Code",
                DataPropertyName = "Code",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            var colLevel = new DataGridViewTextBoxColumn
            {
                Name = "Level",
                HeaderText = "Level",
                DataPropertyName = "Level",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            var colMsg = new DataGridViewTextBoxColumn
            {
                Name = "Message",
                HeaderText = "Message",
                DataPropertyName = "Message",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            };
            gv.Columns.AddRange(colTime, colCode, colLevel, colMsg);
        }

        private void ModeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_changingModeProgrammatically || _alarmMgr == null) return;

            var rb = sender as RadioButton;
            if (rb != null && rb.Checked == false) return;

            var want = GetProgramModeFromUi();

            // Mode Key Status IO 때문에 Semi / Auto 전환이 막히므로
            // 모드 전환 시점에서는 AlarmManager 체크 제외
            /*
            if (!_alarmMgr.IsModeAllowed(want))
            {
                var fallback = _alarmMgr.GetClosestAllowedMode(want);
                ForceMode(fallback);

                MessageBox.Show(
                    "현재 알람 상태에서는 선택한 운전 모드가 허용되지 않습니다.\r\n" +
                    " - Alarm/Fatal Latch 시: Manual 고정\r\n" +
                    " - Warning Latch + ForceManual: Allow 설정에 따라 허용",
                    "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            */

            if (!TryApplyProgramMode(want, out var reason))
            {
                ForceMode(_lastConfirmedMode);

                if (!string.IsNullOrWhiteSpace(reason))
                {
                    AddLog(lstSystem, "[Mode] change failed: " + reason);

                    MessageBox.Show(
                        reason,
                        "Mode Change Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return;
            }

            _lastConfirmedMode = want;

            try
            {
                if (gbMode != null)
                    gbMode.Text = $"OperationMode : {want}";
            }
            catch { }

            _towerForm?.RefreshNow();
            UpdateAutoFormEnableState();
        }
        #endregion
    }
}