using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetCommon;
using StageWin.Core.Recipe;
using StageWin.Etc;
using StageWin.Safety;
using XY = StageWin.UI.RecipeForm.XY;

namespace StageWin.UI
{
    public partial class OpticOperationForm : Form
    {
        #region ======= 변수, 이벤트 선언 =======

        public enum FlyPattern { OneWay, Snake }
        public enum TravelDirection { Forward, Backward }
        public enum ProcessMode { StepByStep, OnTheFly }

        private const bool SCAN_Y_MAJOR = true; // Main Y 방향 주행

        // StagePoint 하나당 벡터셀 개수 (vecRows * vecCols)
        private int _vecPerStage = 1;

        // UI용 벡터 레이아웃(표시/선택/하이라이트용)
        private int _procVecRows = 1;
        private int _procVecCols = 1;

        // Processing highlight
        private readonly List<(int r, int c)> _procHlCells = new List<(int r, int c)>();
        private static readonly Color PROC_HL = Color.Lavender;

        // (선택 유지) 마지막 선택 셀
        private Cell _lastSelectedCell = new Cell { Row = 1, Col = 1, VecR = 0, VecC = 0 };

        public Func<(int VecRows, int VecCols)> ProvideVectorLayout { get; set; }  // 선택: 벡터 셀 정보(없으면 1x1)

        public struct LaserCtrlParams
        {
            public int ScanNo;
            public long TimeMs;
            public double Power;
            public double Frequency;
            public int LaserOn;     // 1: ON, 0: OFF
            public double XOffset;
            public double YOffset;
        }

        public struct ProcessScanningParams
        {
            public int ScanNo;
            public double MotorX;
            public double MotorY;
            public ST_DRAW_DATA_LIST[] DrawList;
        }

        public sealed class ProcessRunPlan
        {
            public int ScanNo;
            public ST_DRAW_DATA_LIST[] All;        // 전체 드로우 리스트(레시피 기준)
            public int StartIndex;                 // gridProcess에서 시작 인덱스(0-based)
            public ProcessMode Mode;

            // Step-by-Step: 몇 개 홀을 진행할지
            public int StepCount;
            // Step-by-Step: 라인 끝까지
            public bool StepWholeLine;
            public bool StepAllWholeLine;   // Step-by-Step: 전체 라인 모두 가공

            // On-The-Fly: 몇 개 라인
            public int FlyLineCount;

            public int HolesPerLine;               // 레이아웃 정보

            public bool UseFlyingVision { get; set; } = false;
            public FlyPattern Pattern { get; set; } = FlyPattern.OneWay;
            public TravelDirection Direction { get; set; } = TravelDirection.Forward;
            public bool IsYMajor { get; set; } = SCAN_Y_MAJOR;

            public int Lines { get; set; }
            public (int Row, int Col)[] Order { get; set; } // index -> (Row, Col) 매핑

            // Vector
            public int VecPerStage { get; set; } = 1;       // StagePoint 하나당 DrawList 개수 (VectorRow * VectorCol)
            public int VecRows { get; set; } = 1;
            public int VecCols { get; set; } = 1;
            public double PitchX { get; set; } = 0.0;
            public double PitchY { get; set; } = 0.0;

            // Process Scan에서 사용할 축 속도 (Stage/Scanner feed 속도 개념)
            public double ProcessSpeedX { get; set; } = 0.0;
            public double ProcessSpeedY { get; set; } = 0.0;
        }

        public event Action<ProcessScanningParams> RequestProcessScanning;
        public event Func<ProcessRunPlan, Task> RequestRunProcess; // Form1에서 실행
        public event Action RequestStopProcess;                    // Form1에서 정지

        public Func<XY> GetAxesPos { get; set; }  // 현재 ReviewX/MainY 조회 델리게이트
        public Func<XY> GetAxesVel { get; set; }  // 현재 ReviewX/MainY 속도 조회 델리게이트
        public Func<double> GetZPos { get; set; }               // Ajin Z axis position
        public Func<double> GetThetaPos { get; set; }           // Ajin Theta axis position
        public Action<double, double> MoveAxes { get; set; }    // MoveAxes(reviewX, mainY)

        public Func<ST_DRAW_DATA_LIST[]> ProvideDrawList { get; set; }
        public Func<(int lines, int holesPerLine)> ProvideLayout { get; set; }   // 라인/홀 수 제공
        public Func<(int Row, int Col)[]> ProvideScanOrder { get; set; }
        public Func<RecipeDoc> ProvideRecipeDoc { get; set; }

        public event Action<double, double> RequestLaserPowerSet; // power, atten
        public event Action RequestLaserOn;
        public event Action RequestLaserOff;
        public event Action<double, double> RequestScannerMove;   // x, y
        public event Action RequestScannerHome;
        public event Action<LaserCtrlParams> RequestLaserControl;

        private volatile bool _laserInterlockLatched = false;
        private readonly Timer _tmrPoll = new Timer { Interval = 500 };
        private IOForm _io;

        public Func<ISafetyContext> GetSafetyContext { get; set; }
        private ISafetyContext _fallbackCtx;
        public Func<ProgramMode> ModeProvider { get; set; }

        private readonly BindingList<RowColXY> _process = new BindingList<RowColXY>();

        // Scan 레시피인지 여부(로컬이면 레이저 출사 금지)
        private bool _isScanOrigin = false;

        public Func<(double AlignX, double AlignY, double FirstHoleX, double FirstHoleY,
            double PitchX, double PitchY, double ScanOffsetX, double ScanOffsetY)> ProvideScanGeometry
        { get; set; }

        public double ProcessScanXSpeedValue => (double)ProcessScanXSpeed.Value;
        public double ProcessScanYSpeedValue => (double)ProcessScanYSpeed.Value;
        public int CurrentScanNo => (int)numScanNo.Value;

        private sealed class RowColXY
        {
            public int Index { get; set; }  // 1-based
            public int Row { get; set; }
            public int Col { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }

        // RecipeForm 개선판과 동일한 "선택 셀" 구조
        private struct Cell
        {
            public int Row;   // line1 (1-based)
            public int Col;   // hole1 (1-based)
            public int VecR;  // 0-based
            public int VecC;  // 0-based
        }
        #endregion

        #region ======= 성능 캐시(좌표 계산/드로우리스트) =======

        private sealed class ScanCalcCache
        {
            public bool Valid;

            public int Lines;
            public int Holes;
            public int VecRows;
            public int VecCols;

            public double PitchX;
            public double PitchY;

            public double VOrgR;
            public double VOrgC;

            public double XCenter;
            public double YCenter;

            public void BuildFromDoc(RecipeDoc doc)
            {
                Valid = false;
                if (doc == null) return;

                Lines = Math.Max(1, doc.Parameters.Lines);
                Holes = Math.Max(1, doc.Parameters.HolesPerLine);
                VecRows = Math.Max(1, doc.Parameters.VectorRows);
                VecCols = Math.Max(1, doc.Parameters.VectorCols);

                PitchX = doc.Parameters.PitchX;
                PitchY = doc.Parameters.PitchY;

                VOrgR = CenterOrigin(VecRows);
                VOrgC = CenterOrigin(VecCols);

                // gridScan(CalcScanXY)와 동일: ReviewCenter - ScanToReviewOffset
                XCenter = (doc.Offset?.ReviewOffsetX ?? 0.0) - (doc.Offset?.ScanToReviewOffsetX ?? 0.0);
                YCenter = (doc.Offset?.ReviewOffsetY ?? 0.0) - (doc.Offset?.ScanToReviewOffsetY ?? 0.0);

                Valid = true;
            }

            public bool TryCalc(int line1, int hole1, int vr, int vc, out double x, out double y)
            {
                x = y = 0.0;
                if (!Valid) return false;

                line1 = Math.Max(1, Math.Min(Lines, line1));
                hole1 = Math.Max(1, Math.Min(Holes, hole1));
                vr = Math.Max(0, Math.Min(VecRows - 1, vr));
                vc = Math.Max(0, Math.Min(VecCols - 1, vc));

                int globalR = (hole1 - 1) * VecRows + vr;
                int globalC = (line1 - 1) * VecCols + vc;

                x = XCenter - ((globalC - VOrgC) * PitchX);
                y = YCenter - ((globalR - VOrgR) * PitchY);
                return true;
            }
        }

        private ScanCalcCache _scanCache = new ScanCalcCache();

        // 그리드/Plan 생성에서 DrawList를 매번 ProvideDrawList()로 다시 만들지 않게 캐시
        private ST_DRAW_DATA_LIST[] _cachedDraw;
        private (int lines, int holesPerLine) _cachedLayout;
        #endregion

        #region ======= Safety, UI =======

        public OpticOperationForm()
        {
            InitializeComponent();

            _io = new IOForm();
            LedLabel.Init(ledEmission);

            // Apply: 파라미터만 전달(Scan이면 Commit, Local이면 Save는 Form1에서 처리)
            btnApplyLaser.Click += (s, e) =>
                RequestLaserPowerSet?.Invoke((double)numPower.Value, (double)numAtt.Value);

            // 레이저 출사 제어: Local이면 안내 후 즉시 반환
            btnLaserOn.Click += (s, e) =>
            {
                var ctx = TryGetSafetyCtx();
                if (ctx == null) { ShowLocalLaserBlocked(); return; }

                var r = SafetyPolicy.CheckLaserOnPreconditions(ctx);
                if (!r.Allowed)
                {
                    MessageBox.Show(this, r.Reason ?? "안전 조건 불만족으로 Laser ON 금지.",
                        "Laser", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_isScanOrigin) { ShowLocalLaserBlocked(); return; }
                RequestLaserOn?.Invoke();
            };

            btnLaserOff.Click += (s, e) =>
            {
                if (!_isScanOrigin) { ShowLocalLaserBlocked(); return; }
                RequestLaserOff?.Invoke();
            };

            // UI 갱신
            _tmrPoll.Tick += (s, e) => PollUi();
            _tmrPoll.Start();

            // gridProcess 구성
            SetupGridProcess();

            // 선택 변경 시: "선택셀 저장 + Max 갱신"
            gridProcess.SelectionChanged += (s, e) =>
            {
                _lastSelectedCell = SelectedCell;
                UpdateProcessLimits(clampValue: true);
            };

            // 모드 토글 변경 시에도 Max 갱신
            if (rbtnProcessStepbyStep != null) rbtnProcessStepbyStep.CheckedChanged += (s, e) => UpdateProcessLimits(true);
            if (rbtnProcessOntheFly != null) rbtnProcessOntheFly.CheckedChanged += (s, e) => UpdateProcessLimits(true);
            if (rbtnProcessStepWholeLine != null) rbtnProcessStepWholeLine.CheckedChanged += (s, e) => UpdateProcessLimits(true);
            if (rbtnProcessStepHoles != null) rbtnProcessStepHoles.CheckedChanged += (s, e) =>
            { if (rbtnProcessStepHoles.Checked && numProcessStepCount != null) numProcessStepCount.Enabled = true; UpdateProcessLimits(true); };
            if (rbtnYProcess != null) rbtnYProcess.CheckedChanged += (s, e) => UpdateProcessLimits(true);
            if (rbtnXProcess != null) rbtnXProcess.CheckedChanged += (s, e) => UpdateProcessLimits(true);

            // 처음 로딩 시 1회
            UpdateProcessLimits(clampValue: true);

            // 스캔 좌표로 이동(btnMoveScan)
            btnMoveScan.Click += (s, e) => DoMoveAxesFromScanGrid();

            // 시퀀스 시작/정지 버튼
            _btnProcessScanning.Click += async (s, e) =>
            {
                var result = MessageBox.Show(
                    this,
                    "Process Scan을 시작하겠습니까?",
                    "Process Scan",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );
                if (result != DialogResult.OK) return;

                if (!_isScanOrigin) { ShowLocalLaserBlocked(); return; }

                // UI freeze 방지(버튼 연타 방지)
                _btnProcessScanning.Enabled = false;
                try
                {
                    var plan = BuildRunPlanFromUI(); // 여기서 캐시 사용
                    if (plan == null) return;

                    if (RequestRunProcess != null)
                    {
                        await RequestRunProcess(plan);
                        MessageBox.Show(this, "Process Scan이 완료되었습니다.", "Process",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this, "실행 콜백이 연결되어 있지 않습니다(Form1).", "Process",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                finally
                {
                    _btnProcessScanning.Enabled = true;
                }
            };

            _btnProcessStop.Click += (s, e) =>
            {
                RequestStopProcess?.Invoke();
                MessageBox.Show(this, "Process Scan이 취소되었습니다.", "Process",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            SetRecipeContext("-", "Local");  // 처음에는 레이저 출사 불가 상태로 표시
        }

        private ISafetyContext TryGetSafetyCtx()
        {
            var ctx = GetSafetyContext?.Invoke();
            if (ctx != null) return ctx;

            if (_fallbackCtx == null)
                _fallbackCtx = new BasicSafetyContext(modeGetter: () => (ModeProvider != null ? ModeProvider() : ProgramMode.Manual));
            return _fallbackCtx;
        }

        private void PollUi()
        {
            try
            {
                var getdata = GetAxesPos;
                if (getdata != null)
                {
                    XY pos = getdata();
                    var rx = pos.X.ToString("F3");
                    var my = pos.Y.ToString("F3");
                    if (txtRPos != null && txtRPos.Text != rx) txtRPos.Text = rx;
                    if (txtMPos != null && txtMPos.Text != my) txtMPos.Text = my;
                }

                var getvel = GetAxesVel;
                if (getvel != null)
                {
                    XY vel = getvel();
                    if (txtStageXSpeed != null) txtStageXSpeed.Text = vel.X.ToString("F3");
                    if (txtStageYSpeed != null) txtStageYSpeed.Text = vel.Y.ToString("F3");
                }

                if (GetZPos != null && txtZPos != null)
                {
                    var z = GetZPos().ToString("F3");
                    if (txtZPos.Text != z) txtZPos.Text = z;
                }

                if (GetThetaPos != null && txtTPos != null)
                {
                    var t = GetThetaPos().ToString("F5");
                    if (txtTPos.Text != t) txtTPos.Text = t;
                }
            }
            catch { }

            EnforceLaserInterlock();
        }

        private bool GuardAllowAxisMove()
        {
            var ctx = GetSafetyContext?.Invoke();
            if (ctx == null) return true;

            var r = SafetyPolicy.CheckGlobalMotionInterlockForAxis(ctx);
            if (!r.Allowed)
            {
                MessageBox.Show(this, r.Reason ?? "도어 인터락으로 축 동작이 차단되었습니다.",
                    "Safety", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void EnforceLaserInterlock()
        {
            var ctx = TryGetSafetyCtx();
            if (ctx == null) return;

            if (SafetyPolicy.ShouldForceLaserOff(ctx, out var why))
            {
                if (!_laserInterlockLatched)
                {
                    _laserInterlockLatched = true;
                    try { RequestLaserOff?.Invoke(); } catch { }
                    try { _io.TryWriteOutputByName(SafetyPolicy.OUT_LASER_INTERLOCK_OK, false, source: "Auto"); } catch { }
                }
            }
            else
            {
                _laserInterlockLatched = false;
            }
        }

        private void ShowLocalLaserBlocked()
        {
            MessageBox.Show(this,
                "로컬 레시피는 레이저 출사 동작을 수행할 수 없습니다.\r\n" +
                "Scan 레시피를 선택한 뒤 시도해 주세요.",
                "Laser",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void SetLaserEmission(bool isOn)
        {
            LedLabel.Set(ledEmission, isOn ? LedState.On : LedState.Off);
            lblEmissionState.Text = isOn ? "ON" : "OFF";
        }

        public void RefreshProcessGrid()
        {
            RebuildProcessGrid(preserve: true, showProgress: false);
        }

        public void SetRecipeContext(string recipeName, string originText)
        {
            lblRecipeValue.Text = string.IsNullOrWhiteSpace(recipeName) ? "-" : recipeName;
            lblOriginValue.Text = string.IsNullOrWhiteSpace(originText) ? "-" : originText;

            _isScanOrigin = string.Equals(originText, "Scan", StringComparison.OrdinalIgnoreCase);

            btnLaserOn.Enabled = _isScanOrigin;
            btnLaserOff.Enabled = _isScanOrigin;

            if (!_isScanOrigin) SetLaserEmission(false);

            // 레시피 바뀌면 캐시 무효화
            _cachedDraw = null;
            _scanCache.Valid = false;

            // 레시피 바뀔 때 좌표 그리드 갱신(+ 선택 유지)
            RebuildProcessGrid(preserve: true, showProgress: true);
        }
        #endregion

        #region ===== Grid 모션, 가공 Plan Build =======

        private void DoMoveAxesFromScanGrid()
        {
            var cell = gridProcess.CurrentCell;
            if (cell == null || cell.RowIndex < 0 || cell.ColumnIndex < 0)
            {
                MessageBox.Show(this, "이동할 스캔 좌표 셀을 선택하세요.", "Move Scan",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cell.ColumnIndex == 0)
            {
                MessageBox.Show(this, "Stage Point 라벨이 아닌 좌표 셀을 선택하세요.", "Move Scan",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!GuardAllowAxisMove()) return;

            var text = cell.Value?.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(this, "선택한 셀에 유효한 좌표가 없습니다.", "Move Scan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var parts = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (parts.Length < 3)
            {
                MessageBox.Show(this,
                    "좌표 형식을 인식할 수 없습니다.\r\n예상 형식: R..C.. / X / Y",
                    "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(parts[1], out double targetX) ||
                !double.TryParse(parts[2], out double targetY))
            {
                MessageBox.Show(this, "좌표 숫자 파싱에 실패했습니다.", "Move Scan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cur = GetAxesPos != null ? GetAxesPos() : new XY();
            using (var dlg = new MoveConfirmForm(
                "Pass under Scanner (ReviewX/MainY)",
                cur.X, cur.Y, targetX, targetY))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes)
                    MoveAxes?.Invoke(targetX, targetY);
            }
        }

        private void SetupGridProcess()
        {
            gridProcess.AllowUserToAddRows = false;
            gridProcess.AllowUserToDeleteRows = false;
            gridProcess.AllowUserToResizeColumns = false;
            gridProcess.AllowUserToResizeRows = false;
            gridProcess.RowHeadersVisible = false;
            gridProcess.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridProcess.MultiSelect = false;
            gridProcess.AutoGenerateColumns = false;
            gridProcess.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gridProcess.BackgroundColor = Color.White;
            gridProcess.DataSource = null;
            gridProcess.CellFormatting += GridProcess_CellFormatting;

            TryEnableDoubleBuffer(gridProcess, true);
        }

        private static void TryEnableDoubleBuffer(DataGridView gv, bool on)
        {
            try
            {
                // DataGridView.DoubleBuffered 는 protected라 reflection 필요
                typeof(DataGridView).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, gv, new object[] { on });
            }
            catch { }
        }

        private static double CenterOrigin(int n)
        {
            if (n <= 1) return 0.0;
            return (n - 1) * 0.5;
        }

        private static bool IsCenterIndex(int idx, int n)
        {
            if (n <= 1) return idx == 0;
            if ((n % 2) == 1) return idx == (n / 2);            // odd
            return (idx == (n / 2 - 1)) || (idx == (n / 2));    // even
        }

        private static void UpdateNumericLimit(NumericUpDown num, int min, int max, bool clampValue = true)
        {
            if (num == null) return;
            if (max < min) max = min;
            num.Minimum = min;
            num.Maximum = max;
            if (clampValue)
            {
                if (num.Value < num.Minimum) num.Value = num.Minimum;
                else if (num.Value > num.Maximum) num.Value = num.Maximum;
            }
            num.Enabled = (max > 0);
        }

        private void BuildProcessStageMatrixCached(ST_DRAW_DATA_LIST[] draw, int lines, int holesPerLine, ScanCalcCache cache)
        {
            var gv = gridProcess;
            if (gv == null) return;
            bool restoreFormatting = false;
            try
            {
                gv.CellFormatting -= GridProcess_CellFormatting;
                restoreFormatting = true;
            }
            catch { restoreFormatting = false; }

            gv.SuspendLayout();
            try
            {
                gv.DataSource = null;
                gv.Columns.Clear();
                gv.Rows.Clear();
                gv.Font = new Font("맑은 고딕", 8.0f);
                gv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                gv.RowTemplate.Height = 60;
                gv.AllowUserToAddRows = false;
                int vecRows = Math.Max(1, _procVecRows);
                int vecCols = Math.Max(1, _procVecCols);

                var colStage = new DataGridViewTextBoxColumn
                {
                    Name = "colPStagePoint",
                    HeaderText = "Stage Point",
                    Width = 90,
                    ReadOnly = true,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                colStage.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colStage.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                gv.Columns.Add(colStage);

                for (int line = 1; line <= lines; line++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = $"colPL{line}_C{vc}",
                            HeaderText = (vc == 0) ? $"Line#{line}" : string.Empty,
                            Width = 120,
                            ReadOnly = true,
                            Resizable = DataGridViewTriState.False,
                            SortMode = DataGridViewColumnSortMode.NotSortable
                        };
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        gv.Columns.Add(col);
                    }
                }
                for (int hole = 1; hole <= holesPerLine; hole++)
                {
                    int stagePointOffset = (hole - 1) * vecRows;
                    for (int vR = 0; vR < vecRows; vR++)
                    {
                        int rowIndex = gv.Rows.Add();
                        var row = gv.Rows[rowIndex];
                        string stageLabel = $"{hole} ({vecRows}x{lines * vecCols})";
                        row.Cells[0].Value = (vR == 0) ? stageLabel : string.Empty;
                        int globalR = stagePointOffset + vR;
                        for (int line = 1; line <= lines; line++)
                        {
                            int baseColIndex = 1 + (line - 1) * vecCols;
                            for (int vC = 0; vC < vecCols; vC++)
                            {
                                int colIndex = baseColIndex + vC;
                                double cellX, cellY;
                                if (!cache.TryCalc(line, hole, vR, vC, out cellX, out cellY))
                                {
                                    cellX = 0; cellY = 0;
                                }
                                row.Cells[colIndex].Value =
                                    "R" + globalR.ToString() + "C" + vC.ToString() + Environment.NewLine +
                                    cellX.ToString("F3") + Environment.NewLine +
                                    cellY.ToString("F3");
                            }
                        }
                    }
                }
                gv.ClearSelection();
            }
            finally
            {
                gv.ResumeLayout();
                if (restoreFormatting)
                {
                    try
                    {
                        gv.CellFormatting -= GridProcess_CellFormatting; // 중복 구독 방지
                        gv.CellFormatting += GridProcess_CellFormatting;
                    }
                    catch { }
                }
            }
        }

        private void RebuildProcessGrid(bool preserve, bool showProgress)
        {
            BlinkNotifier.Session blink = null;
            if (showProgress)
                blink = BlinkNotifier.Show(this, "PROCESS REBUILD\r\n좌표/맵 생성 중...", 450, Color.DodgerBlue);

            Cell prevSel = preserve ? SelectedCell : _lastSelectedCell;

            try
            {
                blink?.Update("PROCESS REBUILD\r\n드로우/레이아웃 읽는 중...");

                _process.Clear();

                // 1) draw/layout/doc를 “한 번만” 가져오고 캐시
                var draw = ProvideDrawList?.Invoke();
                var layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);

                _cachedDraw = draw;
                _cachedLayout = layout;

                var doc = ProvideRecipeDoc?.Invoke();
                _scanCache.BuildFromDoc(doc);

                int lines = Math.Max(1, layout.lines);
                int holesPerLine = Math.Max(1, layout.holesPerLine);
                int stageCount = lines * holesPerLine;

                if (draw == null || draw.Length == 0)
                {
                    _vecPerStage = 1;

                    // UI 표시용 vec layout: 레시피 우선
                    if (doc != null)
                    {
                        _procVecRows = Math.Max(1, doc.Parameters.VectorRows);
                        _procVecCols = Math.Max(1, doc.Parameters.VectorCols);
                    }
                    else if (ProvideVectorLayout != null)
                    {
                        var vec = ProvideVectorLayout.Invoke();
                        _procVecRows = Math.Max(1, vec.VecRows);
                        _procVecCols = Math.Max(1, vec.VecCols);
                    }
                    else
                    {
                        _procVecRows = 1;
                        _procVecCols = 1;
                    }

                    gridProcess.DataSource = null;
                    gridProcess.Columns.Clear();
                    gridProcess.Rows.Clear();

                    UpdateProcessLimits(clampValue: true);
                    RestoreProcessSelection(prevSel.Row, prevSel.Col, prevSel.VecR, prevSel.VecC);
                    return;
                }

                blink?.Update("PROCESS REBUILD\r\n벡터/인덱스 계산 중...");

                // 2) StagePoint 하나당 Draw 개수(_vecPerStage)
                _vecPerStage = 1;
                if (stageCount > 0 && (draw.Length % stageCount) == 0)
                    _vecPerStage = Math.Max(1, draw.Length / stageCount);

                // 3) UI용 vec layout은 ProvideVectorLayout 우선, 없으면 1 x _vecPerStage
                if (ProvideVectorLayout != null)
                {
                    var v = ProvideVectorLayout.Invoke();
                    _procVecRows = Math.Max(1, v.VecRows);
                    _procVecCols = Math.Max(1, v.VecCols);
                }
                else if (doc != null)
                {
                    // doc가 있으면 doc 값 우선(그리드 좌표/벡터와 일치)
                    _procVecRows = Math.Max(1, doc.Parameters.VectorRows);
                    _procVecCols = Math.Max(1, doc.Parameters.VectorCols);
                }
                else
                {
                    _procVecRows = 1;
                    _procVecCols = Math.Max(1, _vecPerStage);
                }

                // 4) _process 리스트는 "StagePoint 당 1개" 로만 유지
                // GroupBy 대신 Dictionary로 1-pass (GC/할당 감소)
                var map = new Dictionary<string, ST_DRAW_DATA_LIST>(Math.Max(16, draw.Length));
                for (int i = 0; i < draw.Length; i++)
                {
                    var d = draw[i];
                    var k = d.nRowNo.ToString() + "-" + d.nColNo.ToString();
                    if (!map.ContainsKey(k)) map[k] = d;
                }

                // 정렬
                var keys = map.Keys
                    .Select(k =>
                    {
                        var sp = k.Split('-');
                        int r = 0, c = 0;
                        if (sp.Length >= 2) { int.TryParse(sp[0], out r); int.TryParse(sp[1], out c); }
                        return new { k, r, c };
                    })
                    .OrderBy(x => x.r).ThenBy(x => x.c)
                    .ToList();

                int idx = 1;
                foreach (var kk in keys)
                {
                    var d = map[kk.k];
                    _process.Add(new RowColXY
                    {
                        Index = idx++,
                        Row = d.nRowNo,
                        Col = d.nColNo,
                        X = d.dMarkX,
                        Y = d.dMarkY
                    });
                }

                blink?.Update("PROCESS REBUILD\r\n2D 매트릭스 그리드 생성 중...");
                BuildProcessStageMatrixCached(draw, lines, holesPerLine, _scanCache);

                blink?.Update("PROCESS REBUILD\r\n최대치/선택 복원 중...");
                UpdateProcessLimits(clampValue: true);

                RestoreProcessSelection(prevSel.Row, prevSel.Col, prevSel.VecR, prevSel.VecC);

                blink?.Update("PROCESS REBUILD\r\n완료");
            }
            catch
            {
                // 기존 스타일 유지(무음)
            }
            finally
            {
                blink?.Dispose();
            }
        }

        private Cell SelectedCell
        {
            get
            {
                try
                {
                    var layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);
                    int lines = Math.Max(1, layout.lines);
                    int holes = Math.Max(1, layout.holesPerLine);

                    int vecRows = Math.Max(1, _procVecRows);
                    int vecCols = Math.Max(1, _procVecCols);

                    var cell = gridProcess?.CurrentCell;
                    if (cell == null || cell.RowIndex < 0 || cell.ColumnIndex < 0)
                        return _lastSelectedCell;

                    int rowIdx = cell.RowIndex;
                    int colIdx = cell.ColumnIndex;

                    int hole1 = (rowIdx / vecRows) + 1;
                    hole1 = Math.Max(1, Math.Min(holes, hole1));
                    int vecR = (rowIdx % vecRows);
                    vecR = Math.Max(0, Math.Min(vecRows - 1, vecR));

                    int line1, vecC;
                    if (colIdx <= 0)
                    {
                        line1 = 1;
                        vecC = 0;
                    }
                    else
                    {
                        int colOffset = colIdx - 1;
                        line1 = (colOffset / vecCols) + 1;
                        line1 = Math.Max(1, Math.Min(lines, line1));
                        vecC = (colOffset % vecCols);
                        vecC = Math.Max(0, Math.Min(vecCols - 1, vecC));
                    }

                    return new Cell { Row = line1, Col = hole1, VecR = vecR, VecC = vecC };
                }
                catch
                {
                    return _lastSelectedCell;
                }
            }
        }

        private void RestoreProcessSelection(int line1, int hole1, int vecR, int vecC)
        {
            if (gridProcess == null) return;

            var layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);
            int lines = Math.Max(1, layout.lines);
            int holes = Math.Max(1, layout.holesPerLine);

            int vecRows = Math.Max(1, _procVecRows);
            int vecCols = Math.Max(1, _procVecCols);

            line1 = Math.Max(1, Math.Min(lines, line1));
            hole1 = Math.Max(1, Math.Min(holes, hole1));
            vecR = Math.Max(0, Math.Min(vecRows - 1, vecR));
            vecC = Math.Max(0, Math.Min(vecCols - 1, vecC));

            int r = (hole1 - 1) * vecRows + vecR;
            int c = 1 + (line1 - 1) * vecCols + vecC;

            if (r < 0 || r >= gridProcess.Rows.Count) return;
            if (c < 1 || c >= gridProcess.Columns.Count) return;

            try
            {
                gridProcess.ClearSelection();
                gridProcess.CurrentCell = gridProcess.Rows[r].Cells[c];
                gridProcess.Rows[r].Cells[c].Selected = true;

                _lastSelectedCell = new Cell { Row = line1, Col = hole1, VecR = vecR, VecC = vecC };
            }
            catch { }
        }

        private void UpdateProcessLimits(bool clampValue)
        {
            var layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);
            int lines = Math.Max(1, layout.lines);
            int holesPerLine = Math.Max(1, layout.holesPerLine);

            int vecRows = Math.Max(1, _procVecRows);
            int vecCols = Math.Max(1, _procVecCols);

            bool yMajor = (rbtnYProcess?.Checked ?? true);

            var sel = SelectedCell;
            int line1 = Math.Max(1, Math.Min(lines, sel.Row));
            int hole1 = Math.Max(1, Math.Min(holesPerLine, sel.Col));

            bool isStepMode = (rbtnProcessStepbyStep?.Checked ?? false);
            bool stepWholeLine = (rbtnProcessStepWholeLine?.Checked ?? false);
            bool stepAllWholeLine = (rbtnProcessStepAllWholeLine?.Checked ?? false);

            int prevStep = (int)(numProcessStepCount?.Value ?? 1);
            if (prevStep < 1) prevStep = 1;

            int maxStep;
            if (isStepMode)
            {
                int lineLen = yMajor ? holesPerLine : lines;
                int posInLine = yMajor ? hole1 : line1;
                bool forward = (rbtnForwardProcess?.Checked ?? true);
                // Backward: pos -> lineLen 방향(증가)  => 남은 개수 = lineLen - pos + 1
                // Forward : pos -> 1 방향(감소)      => 남은 개수 = pos
                maxStep = forward
                    ? Math.Max(1, posInLine)
                    : Math.Max(1, lineLen - posInLine + 1);
            }
            else
            {
                maxStep = 1;
            }
            UpdateNumericLimit(numProcessStepCount, 1, Math.Max(1, maxStep), clampValue: false);
            if (isStepMode && stepWholeLine || isStepMode && stepAllWholeLine)
            {
                if (numProcessStepCount != null)
                {
                    numProcessStepCount.Value = numProcessStepCount.Maximum;
                    numProcessStepCount.Enabled = false;
                }
            }
            else
            {
                if (numProcessStepCount != null)
                {
                    numProcessStepCount.Enabled = isStepMode;
                    if (!isStepMode)
                        numProcessStepCount.Value = 1;
                    else
                    {
                        if (prevStep < numProcessStepCount.Minimum || prevStep > numProcessStepCount.Maximum)
                            numProcessStepCount.Value = numProcessStepCount.Maximum;
                        else
                            numProcessStepCount.Value = prevStep;
                    }
                }
            }

            if (numFlyProcessLines == null) return;

            if (isStepMode)
            {
                UpdateNumericLimit(numFlyProcessLines, 1, 1, clampValue: true);
                numFlyProcessLines.Value = 1;
                numFlyProcessLines.Enabled = false;
                return;
            }

            numFlyProcessLines.Enabled = true;

            int totalLines = yMajor ? lines : holesPerLine;
            int startLine = yMajor ? line1 : hole1;

            int remainLines = Math.Max(1, totalLines - (startLine - 1));
            UpdateNumericLimit(numFlyProcessLines, 1, remainLines, clampValue);

            if (numFlyProcessLines.Value < numFlyProcessLines.Minimum) numFlyProcessLines.Value = numFlyProcessLines.Minimum;
            else if (numFlyProcessLines.Value > numFlyProcessLines.Maximum) numFlyProcessLines.Value = numFlyProcessLines.Maximum;
        }

        public ProcessRunPlan BuildRunPlanFromUI()
        {
            var draw = _cachedDraw ?? ProvideDrawList?.Invoke();
            if (draw == null || draw.Length == 0)
            {
                MessageBox.Show(this, "가공할 좌표가 없습니다.", "Process",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            var layout = _cachedLayout;
            if (layout.lines <= 0 || layout.holesPerLine <= 0)
                layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);

            int holes = Math.Max(1, layout.holesPerLine);
            int vecPerStage = Math.Max(1, _vecPerStage);

            int stageIdx = GetSelectedStageIndex();
            int startIdx = stageIdx * vecPerStage;

            var mode = (rbtnProcessStepbyStep?.Checked ?? false) ? ProcessMode.StepByStep : ProcessMode.OnTheFly;

            int vecRows = Math.Max(1, _procVecRows);
            int vecCols = Math.Max(1, _procVecCols);

            double pitchX = 0.0, pitchY = 0.0;
            if (ProvideScanGeometry != null)
            {
                var g = ProvideScanGeometry();
                pitchX = g.PitchX;
                pitchY = g.PitchY;
            }

            UpdateProcessLimits(clampValue: true);

            return new ProcessRunPlan
            {
                ScanNo = (int)numScanNo.Value,
                All = draw,
                StartIndex = startIdx,
                Mode = mode,

                StepCount = (int)numProcessStepCount.Value,
                StepWholeLine = (rbtnProcessStepWholeLine?.Checked ?? false),
                StepAllWholeLine = (rbtnProcessStepAllWholeLine?.Checked ?? false),

                FlyLineCount = (int)numFlyProcessLines.Value,
                HolesPerLine = holes,

                Pattern = (rbtnSnakeProcess?.Checked ?? true) ? FlyPattern.Snake : FlyPattern.OneWay,
                Direction = (rbtnForwardProcess?.Checked ?? true) ? TravelDirection.Forward : TravelDirection.Backward,
                IsYMajor = (rbtnYProcess?.Checked ?? true),

                Lines = Math.Max(1, layout.lines),
                Order = ProvideScanOrder?.Invoke() ?? Array.Empty<(int Row, int Col)>(),

                VecPerStage = vecPerStage,
                VecRows = vecRows,
                VecCols = vecCols,
                PitchX = pitchX,
                PitchY = pitchY,

                ProcessSpeedX = ProcessScanXSpeedValue,
                ProcessSpeedY = ProcessScanYSpeedValue,
            };
        }

        private int GetSelectedStageIndex()
        {
            var layout = ProvideLayout?.Invoke() ?? (lines: 1, holesPerLine: 1);
            int lines = Math.Max(1, layout.lines);
            int holes = Math.Max(1, layout.holesPerLine);
            int vecRows = Math.Max(1, _procVecRows);
            int vecCols = Math.Max(1, _procVecCols);

            var cell = gridProcess.CurrentCell;
            if (cell == null || cell.RowIndex < 0) return 0;

            int rowIdx = cell.RowIndex;
            int colIdx = cell.ColumnIndex;

            int holeIdx0 = rowIdx / vecRows;
            if (holeIdx0 < 0) holeIdx0 = 0;
            if (holeIdx0 >= holes) holeIdx0 = holes - 1;
            int hole = holeIdx0 + 1;

            int line;
            if (colIdx <= 0) line = 1;
            else
            {
                int colOffset = colIdx - 1;
                int lineIdx0 = colOffset / vecCols;
                if (lineIdx0 < 0) lineIdx0 = 0;
                if (lineIdx0 >= lines) lineIdx0 = lines - 1;
                line = lineIdx0 + 1;
            }

            var order = ProvideScanOrder?.Invoke();
            if (order != null && order.Length > 0)
            {
                for (int i = 0; i < order.Length; i++)
                    if (order[i].Row == line && order[i].Col == hole) return i;
            }

            return (line - 1) * holes + (hole - 1);
        }
        #endregion

        #region ======= Vector Expansion, Doc Mapping, Tooling & Offset Reflection Helpers =======

        public ST_DRAW_DATA_LIST[] ExpandVectorsForLine(
            ProcessRunPlan plan,
            int[] drawIndicesInLine,
            ref int globalVecRow)
        {
            int vecRows = Math.Max(1, plan.VecRows);
            int vecCols = Math.Max(1, plan.VecCols);
            var allDraw = plan.All ?? Array.Empty<ST_DRAW_DATA_LIST>();

            var doc = ProvideRecipeDoc?.Invoke();

            // 좌표 계산 캐시 (doc 기준)
            var cache = new ScanCalcCache();
            cache.BuildFromDoc(doc);

            // tooling dict 캐시
            IDictionary toolDict = null;
            try
            {
                object toolingsObj = GetMemberValue(doc, "Toolings") ?? GetMemberValue(doc, "toolings");
                toolDict = toolingsObj as IDictionary;
            }
            catch { }

            // 1D 모드(벡터 없음)
            if (vecRows == 1 && vecCols == 1)
            {
                var list1D = new List<ST_DRAW_DATA_LIST>(drawIndicesInLine.Length);

                foreach (int idx in drawIndicesInLine)
                {
                    if (idx < 0 || idx >= allDraw.Length) continue;
                    var src = allDraw[idx];
                    int line1 = src.nRowNo;
                    int hole1 = src.nColNo;

                    var cell = src;

                    cell.nRowNo = globalVecRow++;
                    cell.nColNo = 0;

                    if (doc != null) TryApplyToolingFromDocCached(toolDict, doc, ref cell, line1, hole1, 0, 0);

                    if (doc != null && TryGetOffsetFromDoc(doc, line1, hole1, 0, 0, out var ox, out var oy))
                    {
                        cell.dOffsetX = ox;
                        cell.dOffsetY = oy;
                    }
                    else
                    {
                        cell.dOffsetX = src.dOffsetX;
                        cell.dOffsetY = src.dOffsetY;
                    }

                    list1D.Add(cell);
                }
                return list1D.ToArray();
            }

            // 2D 벡터 모드
            var result = new List<ST_DRAW_DATA_LIST>(drawIndicesInLine.Length * vecRows * vecCols);

            // drawIndicesInLine 안의 StagePoint를 (line,hole) 기준 정렬
            var stages = new List<(int lineIdx0, int holeIdx0, ST_DRAW_DATA_LIST sp)>();
            foreach (int drawIdx in drawIndicesInLine)
            {
                if (drawIdx < 0 || drawIdx >= allDraw.Length) continue;
                var sp = allDraw[drawIdx];

                int lineIdx0 = Math.Max(0, sp.nRowNo - 1);
                int holeIdx0 = Math.Max(0, sp.nColNo - 1);
                stages.Add((lineIdx0, holeIdx0, sp));
            }

            stages.Sort((a, b) =>
            {
                int cmp = a.lineIdx0.CompareTo(b.lineIdx0);
                if (cmp != 0) return cmp;
                return a.holeIdx0.CompareTo(b.holeIdx0);
            });

            foreach (var s in stages)
            {
                int line1 = s.lineIdx0 + 1;
                int hole1 = s.holeIdx0 + 1;
                var sp = s.sp;

                for (int vr = 0; vr < vecRows; vr++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        int globalR = s.holeIdx0 * vecRows + vr;
                        int globalC = vc;

                        var cell = sp;
                        cell.nRowNo = globalR;
                        cell.nColNo = globalC;

                        double sx, sy;
                        if (cache.TryCalc(line1, hole1, vr, vc, out sx, out sy))
                        {
                            cell.dMarkX = sx;
                            cell.dMarkY = sy;
                        }

                        if (doc != null) TryApplyToolingFromDocCached(toolDict, doc, ref cell, line1, hole1, vr, vc);

                        if (doc != null && TryGetOffsetFromDoc(doc, line1, hole1, vr, vc, out var ox, out var oy))
                        {
                            cell.dOffsetX = ox;
                            cell.dOffsetY = oy;
                        }
                        else
                        {
                            cell.dOffsetX = 0;
                            cell.dOffsetY = 0;
                        }

                        result.Add(cell);
                    }
                }
            }

            if (stages.Count > 0)
            {
                int maxHoleIdx0 = stages.Max(s => s.holeIdx0);
                int usedRows = (maxHoleIdx0 + 1) * vecRows;
                if (globalVecRow < usedRows) globalVecRow = usedRows;
            }

            return result.ToArray();
        }

        private static string Key2(int line1, int hole1) => line1.ToString() + "-" + hole1.ToString();
        private static string Key4(int line1, int hole1, int vr, int vc) =>
            line1.ToString() + "-" + hole1.ToString() + "-" + vr.ToString() + "-" + vc.ToString();

        // 기존 함수는 유지(외부에서 쓰면 호환)
        private bool TryCalcScanXY_FromDoc(int line1, int hole1, int vr, int vc, out double x, out double y)
        {
            x = y = 0.0;
            var doc = ProvideRecipeDoc?.Invoke();
            if (doc == null) return false;

            var c = new ScanCalcCache();
            c.BuildFromDoc(doc);
            return c.TryCalc(line1, hole1, vr, vc, out x, out y);
        }

        private static bool TryGetOffsetFromDoc(RecipeDoc doc, int line1, int hole1, int vr, int vc, out double ox, out double oy)
        {
            ox = 0.0; oy = 0.0;
            if (doc == null) return false;

            try
            {
                if (doc.ResultsVec != null)
                {
                    var k4 = Key4(line1, hole1, vr, vc);
                    if (doc.ResultsVec.TryGetValue(k4, out var rv) && rv != null)
                    {
                        ox = rv.ErrX; oy = rv.ErrY;
                        return true;
                    }

                    var k00 = Key4(line1, hole1, 0, 0);
                    if (doc.ResultsVec.TryGetValue(k00, out var r00) && r00 != null)
                    {
                        ox = r00.ErrX; oy = r00.ErrY;
                        return true;
                    }
                }

                if (doc.Results != null)
                {
                    var k2 = Key2(line1, hole1);
                    if (doc.Results.TryGetValue(k2, out var r2) && r2 != null)
                    {
                        ox = r2.ErrX; oy = r2.ErrY;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private static void TryApplyToolingFromDocCached(IDictionary dict, RecipeDoc doc, ref ST_DRAW_DATA_LIST cell, int line1, int hole1, int vecR, int vecC)
        {
            try
            {
                if (doc == null) return;
                if (dict == null) return;

                string key = BuildToolingKey(line1, hole1, vecR, vecC);
                if (!dict.Contains(key)) return;

                var toolingObj = dict[key];
                if (toolingObj == null) return;

                if (toolingObj is ST_TOOL_LASER_PARAM p)
                {
                    cell.toolParam = p;
                    return;
                }

                cell.toolParam = UpdateToolParamByReflection(cell.toolParam, toolingObj);
            }
            catch { }
        }

        private static string BuildToolingKey(int line1, int hole1, int vecR, int vecC) =>
            line1.ToString() + "-" + hole1.ToString() + "-" + vecR.ToString() + "-" + vecC.ToString();

        private static TToolParam UpdateToolParamByReflection<TToolParam>(TToolParam toolParam, object toolingObj)
            where TToolParam : struct
        {
            object boxed = toolParam;

            double power = GetDouble(toolingObj, "Power", "Pwr", "dPower") ?? 0.0;
            double freq = GetDouble(toolingObj, "Freq", "Frequency", "dFreq") ?? 0.0;
            double vel = GetDouble(toolingObj, "Vel", "Velocity", "ProcessSpeed", "dProcessSpeed") ?? 0.0;
            double att = GetDouble(toolingObj, "Att", "AttPos", "Attenuator", "dAttPos") ?? 0.0;
            int shot = GetInt(toolingObj, "ShotCount", "iLaserShotCount", "LaserShotCount") ?? 0;
            double jump = GetDouble(toolingObj, "JumpSpeed", "dJumpSpeed") ?? 0.0;

            SetMemberValue(boxed, "dPower", power);
            SetMemberValue(boxed, "Power", power);

            SetMemberValue(boxed, "dFreq", freq);
            SetMemberValue(boxed, "Freq", freq);
            SetMemberValue(boxed, "Frequency", freq);

            SetMemberValue(boxed, "dProcessSpeed", vel);
            SetMemberValue(boxed, "ProcessSpeed", vel);
            SetMemberValue(boxed, "Vel", vel);
            SetMemberValue(boxed, "Velocity", vel);

            SetMemberValue(boxed, "dAttPos", att);
            SetMemberValue(boxed, "AttPos", att);
            SetMemberValue(boxed, "Att", att);
            SetMemberValue(boxed, "Attenuator", att);

            SetMemberValue(boxed, "iLaserShotCount", shot);
            SetMemberValue(boxed, "ShotCount", shot);

            SetMemberValue(boxed, "dJumpSpeed", jump);
            SetMemberValue(boxed, "JumpSpeed", jump);

            return (TToolParam)boxed;
        }

        private static object GetMemberValue(object obj, string name)
        {
            if (obj == null || string.IsNullOrWhiteSpace(name)) return null;
            var t = obj.GetType();
            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null) return p.GetValue(obj);
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) return f.GetValue(obj);
            return null;
        }

        private static int? GetInt(object obj, params string[] names)
        {
            foreach (var n in names)
            {
                var v = GetMemberValue(obj, n);
                if (v == null) continue;
                if (v is int i) return i;
                int r;
                if (int.TryParse(v.ToString(), out r)) return r;
            }
            return null;
        }

        private static double? GetDouble(object obj, params string[] names)
        {
            foreach (var n in names)
            {
                var v = GetMemberValue(obj, n);
                if (v == null) continue;
                if (v is double d) return d;
                if (v is float f) return (double)f;
                if (v is decimal m) return (double)m;
                double r;
                if (double.TryParse(v.ToString(), out r)) return r;
            }
            return null;
        }

        private static void SetMemberValue(object boxedStruct, string name, object value)
        {
            if (boxedStruct == null || string.IsNullOrWhiteSpace(name)) return;
            var t = boxedStruct.GetType();

            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null && p.CanWrite)
            {
                try
                {
                    var cv = Convert.ChangeType(value, p.PropertyType);
                    p.SetValue(boxedStruct, cv);
                }
                catch { }
                return;
            }

            var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
            {
                try
                {
                    var cv = Convert.ChangeType(value, f.FieldType);
                    f.SetValue(boxedStruct, cv);
                }
                catch { }
            }
        }
        #endregion

        #region ===== Formatting / Simple Accessors =====

        private void GridProcess_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var gv = gridProcess;
            if (gv == null) return;

            var colName = gv.Columns[e.ColumnIndex].Name;
            if (colName == "colPStagePoint") return;

            if (colName.StartsWith("colPL") && _procVecRows > 0 && _procVecCols > 0 && e.ColumnIndex >= 1)
            {
                int localVecRow = e.RowIndex % _procVecRows;
                int colWithinLineGrp = (e.ColumnIndex - 1) % _procVecCols;

                bool isStageR = IsCenterIndex(localVecRow, _procVecRows);
                bool isStageC = IsCenterIndex(colWithinLineGrp, _procVecCols);

                if (isStageR && isStageC)
                {
                    e.CellStyle.BackColor = Color.Cyan;
                    e.CellStyle.SelectionBackColor = Color.Cyan;
                }
                else
                {
                    e.CellStyle.BackColor = gv.DefaultCellStyle.BackColor;
                    e.CellStyle.SelectionBackColor = gv.DefaultCellStyle.SelectionBackColor;
                }
            }
        }

        public LaserCtrlParams GetLaserCtrlParams(bool on) => BuildLaserParams(on);

        private LaserCtrlParams BuildLaserParams(bool on)
        {
            return new LaserCtrlParams
            {
                ScanNo = (int)numScanNo.Value,
                TimeMs = (long)numTimeMs.Value,
                Power = (double)numPower.Value,
                Frequency = (double)numFreq.Value,
                LaserOn = on ? 1 : 0,
                XOffset = (double)numXOfs.Value,
                YOffset = (double)numYOfs.Value,
            };
        }

        private void ClearProcessHighlight()
        {
            if (gridProcess == null) return;

            foreach (var (r, c) in _procHlCells)
            {
                if (r < 0 || r >= gridProcess.Rows.Count) continue;
                if (c < 0 || c >= gridProcess.Columns.Count) continue;

                var cell = gridProcess.Rows[r].Cells[c];
                cell.Style.BackColor = Color.Empty;
                cell.Style.SelectionBackColor = Color.Empty;
                gridProcess.InvalidateCell(c, r);
            }
            _procHlCells.Clear();
        }

        public void HighlightProcessingLine(int line1)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HighlightProcessingLine(line1)));
                return;
            }

            if (gridProcess == null || gridProcess.Columns.Count == 0 || gridProcess.Rows.Count == 0)
                return;

            int vecCols = Math.Max(1, _procVecCols);

            int baseCol = 1 + (Math.Max(1, line1) - 1) * vecCols;
            if (baseCol < 1) baseCol = 1;
            if (baseCol >= gridProcess.Columns.Count) baseCol = gridProcess.Columns.Count - 1;

            ClearProcessHighlight();

            for (int r = 0; r < gridProcess.Rows.Count; r++)
            {
                for (int vc = 0; vc < vecCols; vc++)
                {
                    int c = baseCol + vc;
                    if (c <= 0 || c >= gridProcess.Columns.Count) continue;

                    var cell = gridProcess.Rows[r].Cells[c];
                    cell.Style.BackColor = PROC_HL;
                    cell.Style.SelectionBackColor = PROC_HL;

                    _procHlCells.Add((r, c));
                }
            }

            gridProcess.Invalidate();
        }

        public void HighlightProcessingCell(int line1, int hole1, int vecR, int vecC)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HighlightProcessingCell(line1, hole1, vecR, vecC)));
                return;
            }

            if (gridProcess == null || gridProcess.Columns.Count == 0 || gridProcess.Rows.Count == 0)
                return;

            int vecRows = Math.Max(1, _procVecRows);
            int vecCols = Math.Max(1, _procVecCols);

            int r = (Math.Max(1, hole1) - 1) * vecRows + Math.Max(0, vecR);
            int c = 1 + (Math.Max(1, line1) - 1) * vecCols + Math.Max(0, vecC);

            if (r < 0 || r >= gridProcess.Rows.Count) return;
            if (c < 1 || c >= gridProcess.Columns.Count) return;

            ClearProcessHighlight();

            var cell = gridProcess.Rows[r].Cells[c];
            cell.Style.BackColor = PROC_HL;
            cell.Style.SelectionBackColor = PROC_HL;
            _procHlCells.Add((r, c));
            gridProcess.InvalidateCell(c, r);
        }
        #endregion
    }
}
