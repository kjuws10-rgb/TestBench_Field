using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using StageWin.Etc;
using StageWin.Core.Recipe;
using Core.Config;
using Core.Logging;
using StageWin.Safety;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using Timer = System.Windows.Forms.Timer;
using NetCommon;
using Core.Logging;


namespace StageWin.UI
{
    public partial class RecipeForm : Form
    {
        #region ======= 변수, 인터페이스, 플래그, 컬렉션 선언 =======

        public event Action<RecipeDoc> RequestOpenToolingEditor;
        public event Action<RecipeDoc> RequestOpenPowerMeterEditor;
        public event Action<RecipeOpenOrigin, string> RequestReselectCurrentRecipe;

        private RecipeOpenOrigin _lastReselectOrigin = (RecipeOpenOrigin)(-1);
        private string _lastReselectName = null;
        private long _lastReturnRefreshTick = 0;

        public event Action<string> RecipeSaved;
        public event Action<string> RecipeDeleted;
        public event Action<string> RecipeCommitted;
        public event Action<string> RecipeDeleteRequested;  // Scan 리스트 삭제 RPC 요청용
        public event Func<FlyingVisionPartialPlan[], Task> RequestRunFlyingVisionPlans;
        public event Func<FlyingVisionPartialPlan, Task> RequestRunFlyingVisionPartial;

        public Func<RpcSession> GetScanSessionOrWarn { get; set; }
        public Func<RpcSession> GetVisionSessionOrWarn { get; set; }
        public Func<double, double, Task> MoveAxesAsync { get; set; }  // 이동 후 완료까지 대기
        public Func<double, double, Task<bool>> IsAxesSettledAtAsync { get; set; }
        public Func<XY> GetAxesPos { get; set; }               // { X=ReviewX, Y=MainY }
        public Action<double, double> MoveAxes { get; set; }    // MoveAxes(reviewX, mainY)
        public Func<double> GetZPos { get; set; }               // Ajin Z axis position
        public Func<double> GetThetaPos { get; set; }           // Ajin Theta axis position
        public Func<Cell, Task<ST_MANUAL_INSPECTION_REQ>> WaitManualInspectionAsync { get; set; }
        public Func<double, double, int, int, int, Task<MeasRaw>> MeasureAtExAsync { get; set; }
        public Func<ISafetyContext> GetSafetyContext { get; set; }
        public Func<ProgramMode> ModeProvider { get; set; }
        public async void MeasureSelected() => await DoMeasureSelectedAsync();
        public RecipeDoc CurrentDoc => _doc;
        public enum RecipeOpenOrigin { Local, Scan }
        public RecipeOpenOrigin OpenOrigin { get; private set; } = RecipeOpenOrigin.Local;
        public RecipeOpenOrigin LastPickedOrigin => _lastPickedOrigin;
        public string LastPickedRecipeName => _lastPickedRecipeName;
        public void RefreshUiFromDoc()
        {
            if (_doc == null) return;

            PullDocToHeaderUI();
            PullDocToParamUI();
            RebuildFromParameters();
            ClearNeedRebuild();
            ApplySavedResultsToReview();
            ClearNeedApplyReview();
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
            FireReselectCurrentRecipe();
        }
        public string RecipeNameText
        {
            get => (txtRecipe.Text ?? "").Trim();
            set => txtRecipe.Text = value ?? "";
        }
        public void RememberPickedRecipe(RecipeOpenOrigin origin, string recipeNameWithoutExt)
        {
            _lastPickedOrigin = origin;
            _lastPickedRecipeName = recipeNameWithoutExt;
        }
        public void SetOpenOrigin(RecipeOpenOrigin origin)
        {
            OpenOrigin = origin;
            UpdateToolingButtonEnabled();
        }
        public void SetStores(RecipeStore local, RecipeStore scan)
        {
            _storeLocal = local ?? throw new ArgumentNullException(nameof(local));
            _storeScan = scan ?? throw new ArgumentNullException(nameof(scan));
        }
        public bool TryLoadRecipeByName(string recipeNameWithoutExt, bool quietMissing = false, bool createIfMissing = false)
        {
            if (string.IsNullOrWhiteSpace(recipeNameWithoutExt)) return false;
            try
            {
                var st = CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
                RecipeDoc loaded = null;
                try { loaded = st.Load(recipeNameWithoutExt); }
                catch (FileNotFoundException) { /* handled below */ }

                if (loaded == null)
                {
                    if (createIfMissing) loaded = st.New(recipeNameWithoutExt);
                    else
                    {
                        if (!quietMissing)
                            MessageBox.Show(this, "Recipe not found: " + recipeNameWithoutExt,
                                "Load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                var prevSel = _lastSelectedCell ?? new Cell { Row = 1, Col = 1, VecR = 0, VecC = 0 };
                _doc = loaded;
                RefreshUiFromDoc();
                RestoreReviewSelection(prevSel.Row, prevSel.Col, prevSel.VecR, prevSel.VecC);
                gridReviewDetail?.Refresh();
                RefreshReviewMap();
                Logger.Info("Recipe loaded: " + _doc.Header.Name);
                return true;
            }
            catch (Exception ex)
            {
                if (!quietMissing)
                    MessageBox.Show(this, "Load failed: " + ex.Message, "Load", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error("TryLoadRecipeByName", ex);
                return false;
            }
        }
        public void LoadRecipeByName(string recipeNameWithoutExt)
        {
            TryLoadRecipeByName(recipeNameWithoutExt, quietMissing: false, createIfMissing: true);
        }
        public void LoadRecipeDoc(RecipeDoc doc)
        {
            if (doc == null) return;
            _doc = doc;
            PullDocToHeaderUI();
            PullDocToParamUI();
            RebuildFromParameters();
            ClearNeedRebuild();
            ApplySavedResultsToReview();
            ClearNeedApplyReview();
        }
        // 외부(Form1)에서 ForceAbort 시 호출할 API
        public void AbortReviewMeasurement()
        {
            try { _reviewMeasCts?.Cancel(); } catch { }
            ClearRunningHighlights_AbortSafe();
        }
        // 외부 호출용 래퍼(필요 시)
        public void ExternalCommit() => CommitRecipe();
        public void ExternalSave(bool silent = true) => SaveRecipeInternal(silent: silent, confirmOverwrite: true);
        public sealed class FlyingVisionPartialPlan
        {
            public int Row;        // 라인(Row, 1-based)
            public int StartCol;   // 시작 Col(1-based)
            public int Count;      // 남은 홀 개수 (FLYING_READY의 nDataCount)
            public int VecC;       // flyingready에 넘길 "몇번째 열" (0-based)
            public double StartX;  // 시작 Review X
            public double StartY;  // 시작 Main   Y
            public double EndX;    // 라인의 마지막 홀 Review X
            public double EndY;    // 라인의 마지막 홀 Main   Y
            public bool YMajor;        // 메이저축(Y-주행이면 true)
            public bool ForwardThis;   // 이 라인의 실제 진행방향 (스네이크 반전 반영 후 확정)
        }
        public sealed class MarkResult
        {
            public int Result;
            public double TargetX;
            public double TargetY;
            public double MarkX;
            public double MarkY;
        }
        public struct XY { public double X; public double Y; }
        // 외부에서 읽는 선택 셀
        public struct Cell { public int Row; public int Col; public int VecR; public int VecC; }
        public struct MeasRaw
        {
            public int FindResult;
            public double TargetX;
            public double TargetY;
            public double HoleX;
            public double HoleY;
            public double MeasX => HoleX;
            public double MeasY => HoleY;
        }
        private struct ReviewMapTag
        {
            public int Line1;
            public int Hole1;
            public int Vr;
            public int Vc;
            public double X;
            public double Y;
            public int GlobalR;
        }
        public class RowColXY
        {
            public int Index { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public int VectorRow { get; set; }  // R0..R(N-1)
            public int VectorCol { get; set; }  // C0..C(M-1)
            public double X { get; set; }
            public double Y { get; set; }
        }
        private const double SensorPixelSizeUm = 3.45;  // 카메라 픽셀 크기(um)
        private const double ObjectiveMag = 20.0;       // 배율
        private const double MmPerPx = (SensorPixelSizeUm / ObjectiveMag) / 1000.0;  // mm/px
        public class ReviewRow
        {
            public int Index { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public int VectorRow { get; set; }
            public int VectorCol { get; set; }
            public double StageX { get; set; }   // mm
            public double StageY { get; set; }   // mm
            public double TargetX { get; set; }  // px
            public double TargetY { get; set; }  // px
            public double MarkX { get; set; }    // px
            public double MarkY { get; set; }    // px
            private double _errXmm;
            private double _errYmm;
            // 파일/전송/내부 저장값은 mm로 통일
            public double ErrX { get => _errXmm; set => _errXmm = value; } // mm
            public double ErrY { get => _errYmm; set => _errYmm = value; } // mm
            // 화면/판정용 px 값(계산은 mm<->px 변환)
            public double ErrXpx
            {
                get => _errXmm / MmPerPx;
                set => _errXmm = value * MmPerPx;
            }
            public double ErrYpx
            {
                get => _errYmm / MmPerPx;
                set => _errYmm = value * MmPerPx;
            }
            public string ErrXpxmm => $"{ErrXpx:0.###} ({ErrX:0.####})";
            public string ErrYpxmm => $"{ErrYpx:0.###} ({ErrY:0.####})";
            public int FindResult { get; set; }
            public string Grade { get; set; }    // "OK"/"NG"/"IDLE"
        }
        private static string Key2(int row, int col) => row + "-" + col;
        private static string Key4(int row, int col, int vr, int vc) => row + "-" + col + "-" + vr + "-" + vc;
        private static bool TryParseKey(string key, out int r, out int c, out int vr, out int vc, out bool isVecKey)
        {
            r = c = vr = vc = 0;
            isVecKey = false;
            if (string.IsNullOrWhiteSpace(key)) return false;

            var sp = key.Split('-');
            if (sp.Length == 2)
            {
                if (!int.TryParse(sp[0], out r)) return false;
                if (!int.TryParse(sp[1], out c)) return false;
                vr = 0; vc = 0;
                isVecKey = false;
                return true;
            }
            if (sp.Length == 4)
            {
                if (!int.TryParse(sp[0], out r)) return false;
                if (!int.TryParse(sp[1], out c)) return false;
                if (!int.TryParse(sp[2], out vr)) return false;
                if (!int.TryParse(sp[3], out vc)) return false;
                isVecKey = true;
                return true;
            }
            return false;
        }
        private static double CenterOrigin(int n)
        {
            // n=3 -> 1.0, n=2 -> 0.5, n=1 -> 0.0
            if (n <= 1) return 0.0;
            return (n - 1) * 0.5;
        }
        private static bool IsCenterIndex(int idx, int n)
        {
            if (n <= 1) return idx == 0;
            if ((n % 2) == 1)
            {
                // odd: single center
                return idx == (n / 2);
            }
            // even: two centers
            return (idx == (n / 2 - 1)) || (idx == (n / 2));
        }
        private readonly Dictionary<(int Row, int Col), XY> _scanMm = new Dictionary<(int Row, int Col), XY>();
        private readonly Dictionary<(int Row, int Col), XY> _reviewMm = new Dictionary<(int Row, int Col), XY>();

        private int _reviewVecRows = 0;
        private int _reviewVecCols = 0;
        private int _reviewTotalLines = 1;
        private int _reviewTotalHoles = 1;

        private Cell? _lastSelectedCell;
        private readonly HashSet<(int Row, int Col, int Vr, int Vc)> _runningVecHL = new HashSet<(int, int, int, int)>();
        private readonly HashSet<int> _runningLineHL = new HashSet<int>();

        private CancellationTokenSource _reviewMeasCts;
        private readonly Timer _tmrPoll = new Timer { Interval = 500 };

        private IOForm _io = new IOForm();
        private ISafetyContext _fallbackCtx;
        private volatile bool _laserInterlockLatched = false;

        private ContextMenuStrip _ctxGrade;
        private ToolStripMenuItem _miGradeIDLE;
        private ToolStripMenuItem _miManualInspection;
        private ToolStripMenuItem _miStartBatchOffsetApply;

        private bool _needRebuild = false;
        private Color _rebuildBtnBack;
        private string _rebuildBtnText;

        private readonly Color _clrIdleBack = Color.LightGray;
        private readonly Color _clrIdleFore = Color.Black;
        private readonly Color _clrOkBack = Color.LightGreen;
        private readonly Color _clrOkFore = Color.Black;
        private readonly Color _clrNgBack = Color.OrangeRed;
        private readonly Color _clrNgFore = Color.White;
        private readonly Color _clrRunBack = Color.Gold;        // 동작중 하이라이트
        private readonly Color _clrBatchTargetBack = Color.DeepSkyBlue;
        private readonly Color _clrBatchSourceBack = Color.Orange;
        private bool _needApplyReview = false;
        private Color _applyBtnBack;
        private string _applyBtnText;
        private bool _batchOffsetApplyMode = false;
        private Cell _batchOffsetSourceCell;
        private double _batchOffsetSourceX;
        private double _batchOffsetSourceY;
        private readonly HashSet<string> _batchOffsetTargetKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool _batchOffsetDragActive = false;
        private bool _batchOffsetDragSelect = true;
        private bool _batchOffsetSuppressNextClick = false;
        private readonly HashSet<string> _batchOffsetDragVisitedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private struct ReviewState
        {
            public double TargetX, TargetY;
            public double MarkX, MarkY;
            public double ErrX, ErrY;
            public int FindResult;
            public string Grade;
        }
        private BindingList<RowColXY> _design = new BindingList<RowColXY>();
        private BindingList<RowColXY> _scan = new BindingList<RowColXY>();
        private readonly BindingList<ReviewRow> _review = new BindingList<ReviewRow>();
        private readonly BindingList<ReviewRow> _reviewDetailView = new BindingList<ReviewRow>();
        private readonly BindingSource _bsReviewDetail = new BindingSource();
        private bool _isPullingFromDoc = false;
        private int _designVecCols = 0;      // 현재 VectorCol 개수
        private int _designVecRows = 0;      // StagePoint 하나당 VectorRow 개수
        private int _scanVecRows = 0;
        private int _scanVecCols = 0;
        private int _scanTotalLines = 1;
        private int _scanTotalHoles = 1;
        private RecipeStore _storeLocal;
        private RecipeStore _storeScan;
        private RecipeStore CurrentStore => (OpenOrigin == RecipeOpenOrigin.Scan) ? _storeScan : _storeLocal;
        private RecipeDoc _doc;
        private RecipeOpenOrigin _lastPickedOrigin = RecipeOpenOrigin.Local;
        private string _lastPickedRecipeName = null;
        private (int Row, int Col)[] _lastScanOrderCache;
        private bool _isCommitting = false;
        #endregion

        #region ======= 생성자, 이벤트 등록, Context Menu =======

        public RecipeForm()
        {
            InitializeComponent();

            EnableDoubleBuffer(gridScan);
            EnableDoubleBuffer(gridReview);
            EnableDoubleBuffer(gridReviewDetail);
            SetupGridCommon(gridScan, defaultRowSelect: false);
            SetupGridCommon(gridReview, defaultRowSelect: false);
            SetupGridCommon(gridReviewDetail, defaultRowSelect: true);
            EnsureScanColumns();
            EnsureReviewColumns();
            gridDesign.AutoGenerateColumns = false;
            gridScan.AutoGenerateColumns = false;
            gridReview.AutoGenerateColumns = false;
            gridReview.DataSource = null;
            gridReviewDetail.AutoGenerateColumns = false;
            bsDesign.DataSource = _design;
            bsScan.DataSource = _scan;
            bsReview.DataSource = _review;
            _bsReviewDetail.DataSource = _reviewDetailView;
            gridDesign.DataSource = bsDesign;
            gridScan.DataSource = bsScan;
            gridReviewDetail.DataSource = _bsReviewDetail;
            btnSave.Click += (s, e) => SaveRecipe();
            btnCommit.Click += (s, e) => CommitRecipe();
            btnNew.Click += (s, e) => NewRecipe();
            btnDelete.Click += (s, e) => DeleteRecipe();
            btnRebuild.Click += (s, e) =>
            {
                RebuildFromParametersWithProgress();
                ClearNeedRebuild();
            };
            btnMoveScan.Click += (s, e) => DoMoveAxesFromScanGrid();
            btnMoveReview.Click += (s, e) => DoMoveAxesFromReviewGrid();
            btnMeasureSingle.Click += async (s, e) =>
            {
                await RunManualMeasureWithApplyConfirmAsync("Measure Single", async (ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    var rsel = gridReviewDetail.CurrentRow?.DataBoundItem as ReviewRow;
                    if (rsel == null)
                    {
                        MessageBox.Show(this, "측정할 셀을 선택하세요.", "Single Measure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    bool ok = await WithCancellation(MeasureOneReviewRowAsync(rsel, "3002"), ct);
                    if (ok)
                    {
                        gridReviewDetail.Refresh();
                        RefreshReviewMap();
                    }
                });
            };
            btnMeasureLine.Click += async (s, e) =>
            {
                await RunManualMeasureWithApplyConfirmAsync("Measure Line (Step/Flying)", async (ct) =>
                {
                    await DoMeasureLineOptionAsync(ct);
                });
            };
            btnInitalReviewResults.Click += (s, e) =>
            {
                if (MessageBox.Show(this, "모든 Review 결과를 IDLE로 초기화하고 저장할까요?",
                    "Init Review Results", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                InitAllReviewResultsToIdle(clearMeasuredXY: true);      // 1) UI _review 초기화 + doc 결과 클리어
                ApplyAllReviewResultsToDocOffsets(saveToFile: true, resetAppliedOffsets: true);    // 2) 현재 _review 상태(IDLE/0)를 Doc에 저장
                ClearNeedApplyReview();                                 // 3) Apply 버튼 별표/상태도 정상화
                FireReselectCurrentRecipe(force: true);                 // 4) 리스트/선택 동기화
            };
            btnApplyReviewResult.Click += (s, e) =>
            {
                if (_batchOffsetApplyMode)
                {
                    ApplyBatchOffsetTargets();
                    return;
                }

                if (!_needApplyReview)
                {
                    MessageBox.Show(this, "적용할 변경사항이 없습니다.", "Apply Review Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (MessageBox.Show(this,
                    "현재 Review 측정/수정 결과를 레시피에 적용하고 저장할까요?",
                    "Apply Review Results", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                ApplyAllReviewResultsToDocOffsets(saveToFile: true);
                ClearNeedApplyReview();
                FireReselectCurrentRecipe();
            };
            gridDesign.CellFormatting += GridDesign_CellFormatting;
            gridScan.CellFormatting += GridScan_CellFormatting;
            gridReview.CellFormatting += GridReviewMap_CellFormatting;
            gridReview.CellClick += GridReviewMap_CellClick;
            gridReview.CellMouseDown += GridReviewMap_CellMouseDownForBatchOffset;
            gridReview.CellMouseEnter += GridReviewMap_CellMouseEnterForBatchOffset;
            gridReview.MouseUp += GridReviewBatchOffset_MouseUp;
            gridReviewDetail.CellFormatting += GridReview_CellFormatting;
            gridReviewDetail.CellClick += GridReviewDetail_CellClick;
            gridReviewDetail.CellMouseDown += GridReviewDetail_CellMouseDownForBatchOffset;
            gridReviewDetail.CellMouseEnter += GridReviewDetail_CellMouseEnterForBatchOffset;
            gridReviewDetail.MouseUp += GridReviewBatchOffset_MouseUp;
            gridReview.CellDoubleClick += GridReviewMap_CellDoubleClick;


            if (_btnToolingEditor != null)
            {
                _btnToolingEditor.Click += (s, e) =>
                {
                    PushHeaderUIToDoc();
                    PushParamUIToDoc();
                    RequestOpenToolingEditor?.Invoke(_doc);
                };
            }
            if (_btnPowerMeterEditor != null)
            {
                _btnPowerMeterEditor.Click += (s, e) =>
                {
                    PushHeaderUIToDoc();
                    PushParamUIToDoc();
                    RequestOpenPowerMeterEditor?.Invoke(_doc);
                };
            }
            _tmrPoll.Tick += (s, e) => PollUi();
            _tmrPoll.Start();
            UpdateToolingButtonEnabled();
            NewRecipe();

            if (numScanToRevOfsX != null) numScanToRevOfsX.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numScanToRevOfsY != null) numScanToRevOfsY.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numRevOfsX != null) numRevOfsX.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numRevOfsY != null) numRevOfsY.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };

            void Recalc() => UpdateVisionFlyLinesMax();
            gridReviewDetail.SelectionChanged += (s, e) => Recalc();
            gridReviewDetail.CurrentCellChanged += (s, e) => Recalc();
            gridReviewDetail.CellClick += (s, e) => Recalc();
            gridReviewDetail.DataBindingComplete += (s, e) => Recalc();
            gridReviewDetail.CurrentCellChanged += (s, e) => RememberSelectionFromGrid();
            gridReviewDetail.SelectionChanged += (s, e) => RememberSelectionFromGrid();
            gridReviewDetail.CellClick += (s, e) => RememberSelectionFromGrid();

            if (numLines != null) numLines.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numHoles != null) numHoles.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numPitchX != null) numPitchX.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numPitchY != null) numPitchY.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numFirstX != null) numFirstX.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numFirstY != null) numFirstY.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numVectorRow != null) numVectorRow.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (numVectorCol != null) numVectorCol.ValueChanged += (s, e) => { if (_isPullingFromDoc) return; MarkNeedRebuild(); };
            if (rbtnVisionStepAllHoles != null) rbtnVisionStepAllHoles.CheckedChanged += (s, e) => Recalc();
            if (rbtnVisionStepAllWholeLine != null) rbtnVisionStepAllWholeLine.CheckedChanged += (s, e) => UpdateVisionFlyLinesMax();
            if (rbtnVisionStepbyStep != null) rbtnVisionStepbyStep.CheckedChanged += (s, e) => UpdateVisionFlyLinesMax();
            if (rbtnVisionStepHoles != null) rbtnVisionStepHoles.CheckedChanged += (s, e) =>
            { if (rbtnVisionStepHoles.Checked && numVisionStepCount != null) numVisionStepCount.Enabled = true; UpdateVisionFlyLinesMax(); };
            UpdateVisionFlyLinesMax();
            SetupGradeContextMenu();
            this.VisibleChanged += (s, e) => { if (this.Visible) RefreshUiOnReturn(); };
            this.Activated += (s, e) => RefreshUiOnReturn();
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
            WireCellSelectionBehavior(gv, defaultRowSelect);
        }
        private static void EnableDoubleBuffer(DataGridView gv)
        {
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, gv, new object[] { true });
        }
        private void WireCellSelectionBehavior(DataGridView gv, bool defaultRowSelect = false)
        {
            bool shift = false;
            bool allowRowSelectWithShift = !ReferenceEquals(gv, gridDesign);
            gv.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    shift = true;
                    if (allowRowSelectWithShift) gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
            };
            gv.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    shift = false;
                    gv.SelectionMode = defaultRowSelect && allowRowSelectWithShift
                        ? DataGridViewSelectionMode.FullRowSelect
                        : DataGridViewSelectionMode.CellSelect;
                }
            };
            gv.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (!Control.ModifierKeys.HasFlag(Keys.Control) && !shift) gv.ClearSelection();
                gv.CurrentCell = gv[e.ColumnIndex, e.RowIndex];

                if (gv.SelectionMode == DataGridViewSelectionMode.FullRowSelect)
                    gv.Rows[e.RowIndex].Selected = true;
                else
                    gv[e.ColumnIndex, e.RowIndex].Selected = true;

                if (ReferenceEquals(gv, gridReviewDetail)) UpdateVisionFlyLinesMax();
            };
        }
        private void EnsureScanColumns()
        {
            if (gridScan.Columns.Count > 0) return;
            gridScan.Columns.Clear();
            AddNumberCol(gridScan, "colSIdx", "#", "Index", 60, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gridScan, "colSRow", "Line", "Row", 50, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gridScan, "colSCol", "Holes", "Col", 60, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gridScan, "colSVecR", "VecR", "VectorRow", 60, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gridScan, "colSVecC", "VecC", "VectorCol", 60, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gridScan, "colSX", "Review X (mm)", "X", 110, 3);
            AddNumberCol(gridScan, "colSY", "Main Y (mm)", "Y", 110, 3);
        }
        private void EnsureReviewColumns()
        {
            if (gridReviewDetail.Columns.Count > 0) return;
            var gv = gridReviewDetail;
            gv.Columns.Clear();

            AddNumberCol(gv, "colRVecR", "VecR", "VectorRow", 50, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gv, "colRVecC", "VecC", "VectorCol", 50, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gv, "colRTgtX", "Target X (px)", "TargetX", 90, 3);
            AddNumberCol(gv, "colRTgtY", "Target Y (px)", "TargetY", 90, 3);
            AddNumberCol(gv, "colRMeasX", "Meas X (px)", "MarkX", 90, 3);
            AddNumberCol(gv, "colRMeasY", "Meas Y (px)", "MarkY", 90, 3);
            AddTextCol(gv, "colRErrX", "Err X (px/mm)", "ErrXpxmm", 120, DataGridViewContentAlignment.MiddleRight);
            AddTextCol(gv, "colRErrY", "Err Y (px/mm)", "ErrYpxmm", 120, DataGridViewContentAlignment.MiddleRight);
            AddTextCol(gv, "colRGrade", "Grade", "Grade", 60, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol(gv, "colRFind", "Find", "FindResult", 60, 0, DataGridViewContentAlignment.MiddleCenter);
        }
        private static DataGridViewTextBoxColumn AddNumberCol(
            DataGridView gv,
            string name, string header, string prop,
            int width, int decimalPlaces,
            DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleRight,
            bool readOnly = true)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            col.DefaultCellStyle.Alignment = align;
            col.DefaultCellStyle.Format = "N" + Math.Max(0, decimalPlaces);
            col.ValueType = (decimalPlaces == 0) ? typeof(int) : typeof(double);
            gv.Columns.Add(col);
            return col;
        }
        private static DataGridViewTextBoxColumn AddTextCol(
            DataGridView gv,
            string name, string header, string prop,
            int width,
            DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleCenter,
            bool readOnly = true)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            col.DefaultCellStyle.Alignment = align;
            col.ValueType = typeof(string);
            gv.Columns.Add(col);
            return col;
        }
        private void SetupGradeContextMenu()
        {
            _ctxGrade = new ContextMenuStrip();

            _miGradeIDLE = new ToolStripMenuItem("Set Grade = IDLE (Clear)");
            _miManualInspection = new ToolStripMenuItem("Set Manual Inspection");
            _miStartBatchOffsetApply = new ToolStripMenuItem("Batch Apply Offset From This Cell");
            _miGradeIDLE.Click += (s, e) => ForceSetIdleAndClearOnCurrent();
            _miManualInspection.Click += async (s, e) => { await DoManualInspectionFromVisionAsync(); };
            _miStartBatchOffsetApply.Click += (s, e) => StartBatchOffsetApplyFromCurrentCell();
            _ctxGrade.Items.AddRange(new ToolStripItem[] { _miGradeIDLE, new ToolStripSeparator(), _miManualInspection, new ToolStripSeparator(), _miStartBatchOffsetApply, });
            // 상세 그리드에서 우클릭
            gridReviewDetail.CellMouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0) return;
                gridReviewDetail.CurrentCell = gridReviewDetail[e.ColumnIndex, e.RowIndex];
                _ctxGrade.Show(Cursor.Position);
            };
            // 2D Map(gridReview)에서 우클릭
            gridReview.CellMouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 1) return;

                int hole1 = (e.RowIndex / _reviewVecRows) + 1;
                int line1 = ((e.ColumnIndex - 1) / _reviewVecCols) + 1;
                int vr = (e.RowIndex % _reviewVecRows);
                int vc = ((e.ColumnIndex - 1) % _reviewVecCols);
                _lastSelectedCell = new Cell { Row = line1, Col = hole1, VecR = vr, VecC = vc };
                RestoreReviewSelection(line1, hole1, vr, vc);
                UpdateVisionFlyLinesMax();
                _ctxGrade.Show(Cursor.Position);
            };
        }
        private void StartBatchOffsetApplyFromCurrentCell()
        {
            var src = CurrentSelectedReviewRow();
            if (src == null)
            {
                MessageBox.Show(this, "기준 offset 셀을 먼저 선택하세요.", "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryGetCurrentAppliedOffset(src, out _batchOffsetSourceX, out _batchOffsetSourceY))
            {
                MessageBox.Show(this, "선택한 셀에서 적용할 offset 값을 찾을 수 없습니다.", "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _batchOffsetSourceCell = new Cell { Row = src.Row, Col = src.Col, VecR = src.VectorRow, VecC = src.VectorCol };
            _batchOffsetTargetKeys.Clear();
            _batchOffsetApplyMode = true;
            UpdateBatchOffsetApplyButton();
            gridReviewDetail?.Refresh();
            gridReview?.Refresh();

            MessageBox.Show(this,
                "일괄 적용할 셀들을 클릭해서 선택하세요.\r\n선택된 셀은 파란색으로 표시됩니다.\r\n선택이 끝나면 Apply Review Results 버튼을 누르세요.",
                "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private ReviewRow CurrentSelectedReviewRow()
        {
            if (gridReviewDetail?.CurrentRow?.DataBoundItem is ReviewRow rr) return rr;

            var sel = this.SelectedCell;
            return _review.FirstOrDefault(x =>
                x.Row == sel.Row && x.Col == sel.Col &&
                x.VectorRow == sel.VecR && x.VectorCol == sel.VecC);
        }

        private bool TryGetCurrentAppliedOffset(ReviewRow rr, out double ox, out double oy)
        {
            ox = 0.0;
            oy = 0.0;
            if (rr == null) return false;

            if (_doc?.ResultsVec != null &&
                _doc.ResultsVec.TryGetValue(Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol), out var rv) &&
                MeasResult.TryGetAppliedOffset(rv, out ox, out oy))
                return true;

            if (rr.VectorRow == 0 && rr.VectorCol == 0 &&
                _doc?.Results != null &&
                _doc.Results.TryGetValue(Key2(rr.Row, rr.Col), out var r2) &&
                MeasResult.TryGetAppliedOffset(r2, out ox, out oy))
                return true;

            ox = rr.ErrX;
            oy = rr.ErrY;
            return rr.FindResult == 1 || rr.ErrX != 0.0 || rr.ErrY != 0.0;
        }

        private void ToggleBatchOffsetTarget(Cell cell)
        {
            if (!_batchOffsetApplyMode) return;

            string key = Key4(cell.Row, cell.Col, cell.VecR, cell.VecC);
            if (key == Key4(_batchOffsetSourceCell.Row, _batchOffsetSourceCell.Col, _batchOffsetSourceCell.VecR, _batchOffsetSourceCell.VecC))
                return;

            if (!_batchOffsetTargetKeys.Add(key)) _batchOffsetTargetKeys.Remove(key);
            UpdateBatchOffsetApplyButton();
            gridReviewDetail?.Refresh();
            gridReview?.Refresh();
        }

        private void SetBatchOffsetTarget(Cell cell, bool selected)
        {
            if (!_batchOffsetApplyMode) return;

            string key = Key4(cell.Row, cell.Col, cell.VecR, cell.VecC);
            if (key == Key4(_batchOffsetSourceCell.Row, _batchOffsetSourceCell.Col, _batchOffsetSourceCell.VecR, _batchOffsetSourceCell.VecC))
                return;

            if (selected) _batchOffsetTargetKeys.Add(key);
            else _batchOffsetTargetKeys.Remove(key);
        }

        private bool IsBatchOffsetTargetKey(Cell cell)
        {
            return _batchOffsetTargetKeys.Contains(Key4(cell.Row, cell.Col, cell.VecR, cell.VecC));
        }

        private void BeginBatchOffsetDrag(Cell cell)
        {
            if (!_batchOffsetApplyMode) return;

            _batchOffsetDragActive = true;
            _batchOffsetSuppressNextClick = true;
            _batchOffsetDragVisitedKeys.Clear();
            _batchOffsetDragSelect = !IsBatchOffsetTargetKey(cell);
            ApplyBatchOffsetDragCell(cell);
        }

        private void ApplyBatchOffsetDragCell(Cell cell)
        {
            if (!_batchOffsetApplyMode || !_batchOffsetDragActive) return;

            string key = Key4(cell.Row, cell.Col, cell.VecR, cell.VecC);
            if (!_batchOffsetDragVisitedKeys.Add(key)) return;

            SetBatchOffsetTarget(cell, _batchOffsetDragSelect);
            UpdateBatchOffsetApplyButton();
            gridReviewDetail?.Refresh();
            gridReview?.Refresh();
        }

        private void EndBatchOffsetDrag()
        {
            _batchOffsetDragActive = false;
            _batchOffsetDragVisitedKeys.Clear();
        }

        private void ApplyBatchOffsetTargets()
        {
            if (!_batchOffsetApplyMode) return;

            if (_batchOffsetTargetKeys.Count == 0)
            {
                MessageBox.Show(this, "일괄 적용할 대상 셀을 선택하세요.", "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this,
                $"선택한 {_batchOffsetTargetKeys.Count}개 셀에 기준 offset X={_batchOffsetSourceX:0.######}, Y={_batchOffsetSourceY:0.######} mm 를 적용하고 저장할까요?",
                "Batch Apply Offset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            EnsureRebuildIfNeeded(preserve: true, showProgress: false);
            if (_doc == null) return;

            int applied = 0;
            foreach (string key in _batchOffsetTargetKeys.ToArray())
            {
                if (!TryParseKey(key, out int r, out int c, out int vr, out int vc, out bool isVecKey)) continue;
                var rr = _review.FirstOrDefault(x => x.Row == r && x.Col == c && x.VectorRow == vr && x.VectorCol == vc);
                if (rr == null) continue;

                rr.ErrX = _batchOffsetSourceX;
                rr.ErrY = _batchOffsetSourceY;
                rr.FindResult = 1;
                rr.Grade = "OK";
                UpsertForcedAppliedOffsetToDoc(rr, _batchOffsetSourceX, _batchOffsetSourceY);
                applied++;
            }

            try
            {
                var st = CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
                st.Save(_doc);
                RecipeSaved?.Invoke(_doc.Header.Name);
                ClearNeedApplyReview();
                EndBatchOffsetApplyMode();
                FireReselectCurrentRecipe(force: true);
                gridReviewDetail?.Refresh();
                RefreshReviewMap();
                MessageBox.Show(this, $"{applied}개 셀에 offset을 일괄 적용했습니다.", "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Error("[BatchApplyOffset] Save failed", ex);
                MessageBox.Show(this, ex.Message, "Batch Apply Offset", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpsertForcedAppliedOffsetToDoc(ReviewRow rr, double offsetX, double offsetY)
        {
            if (_doc == null || rr == null) return;
            if (_doc.Results == null) _doc.Results = new Dictionary<string, MeasResult>();
            if (_doc.ResultsVec == null) _doc.ResultsVec = new Dictionary<string, MeasResult>();

            string k4 = Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            var resV = BuildForcedAppliedOffsetResult(rr, offsetX, offsetY);
            _doc.ResultsVec[k4] = resV;

            if (rr.VectorRow == 0 && rr.VectorCol == 0)
            {
                string k2 = Key2(rr.Row, rr.Col);
                _doc.Results[k2] = BuildForcedAppliedOffsetResult(rr, offsetX, offsetY);
            }
        }

        private static MeasResult BuildForcedAppliedOffsetResult(ReviewRow rr, double offsetX, double offsetY)
        {
            var result = new MeasResult
            {
                Row = rr.Row,
                Col = rr.Col,
                VectorRow = rr.VectorRow,
                VectorCol = rr.VectorCol,
                TargetX = rr.TargetX,
                TargetY = rr.TargetY,
                MeasX = rr.MarkX,
                MeasY = rr.MarkY,
                Grade = rr.Grade ?? "OK",
                FindResult = 1,
                HasMeasuredErr = false,
                MeasuredErrX = 0.0,
                MeasuredErrY = 0.0
            };
            result.SetAppliedOffset(offsetX, offsetY);
            return result;
        }

        private void EndBatchOffsetApplyMode()
        {
            _batchOffsetApplyMode = false;
            _batchOffsetTargetKeys.Clear();
            UpdateBatchOffsetApplyButton();
            gridReviewDetail?.Refresh();
            gridReview?.Refresh();
        }

        private void UpdateBatchOffsetApplyButton()
        {
            if (btnApplyReviewResult == null) return;
            if (_applyBtnText == null) _applyBtnText = btnApplyReviewResult.Text;
            if (_applyBtnBack == default) _applyBtnBack = btnApplyReviewResult.BackColor;

            if (_batchOffsetApplyMode)
            {
                btnApplyReviewResult.Text = $"Apply Batch Offset ({_batchOffsetTargetKeys.Count})";
                btnApplyReviewResult.BackColor = _clrBatchTargetBack;
            }
            else
            {
                if (!string.IsNullOrEmpty(_applyBtnText)) btnApplyReviewResult.Text = _applyBtnText;
                if (_applyBtnBack != default) btnApplyReviewResult.BackColor = _applyBtnBack;
            }
        }

        private bool IsBatchOffsetSource(ReviewRow rr)
        {
            if (!_batchOffsetApplyMode || rr == null) return false;
            return rr.Row == _batchOffsetSourceCell.Row && rr.Col == _batchOffsetSourceCell.Col &&
                   rr.VectorRow == _batchOffsetSourceCell.VecR && rr.VectorCol == _batchOffsetSourceCell.VecC;
        }

        private bool IsBatchOffsetTarget(ReviewRow rr)
        {
            if (!_batchOffsetApplyMode || rr == null) return false;
            return _batchOffsetTargetKeys.Contains(Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol));
        }

        private void UpdateToolingButtonEnabled()
        {
            if (_btnToolingEditor == null) return;
            _btnToolingEditor.Visible = true;
        }
        private void RefreshUiOnReturn()
        {
            long now = Environment.TickCount;
            if (now - _lastReturnRefreshTick < 200) return;
            _lastReturnRefreshTick = now;
            if (_doc == null) return;
            FireReselectCurrentRecipe(force: true);
            try
            {
                if (!_needApplyReview)
                {
                    ApplySavedResultsToReview();
                    gridReviewDetail?.Refresh();
                    RefreshReviewMap();
                    var sel = this.SelectedCell;
                    RestoreReviewSelection(sel.Row, sel.Col, sel.VecR, sel.VecC);
                }
            }
            catch { }
        }
        #endregion

        #region ======= 레시피, 좌표, 리빌드 (Doc <-> UI 포함) =======
        public (int Row, int Col)[] GetScanOrderSequence()
        {
            var doc = CurrentDoc;
            if (doc == null) return Array.Empty<(int, int)>();

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            var sf = doc.Parameters?.ScanFlying ?? new RecipeParameters.ScanFlyingParam();

            // 시작점 자동 결정(미지정 시 플라이 방향에 맞춰 자동 세팅)
            int startCol = ResolveStartColAuto(sf.StartCol, holes, sf.FlyDirY);
            int startRow = ResolveStartRowAuto(sf.StartRow, lines, sf.FlyDirX);
            var seq = ProcessOrder(lines, holes, sf.MajorIsY, sf.Forward, sf.Serpentine, startRow, startCol);
            _lastScanOrderCache = seq.Select(t => (t.r, t.c)).ToArray();
            return _lastScanOrderCache;
        }
        private static IEnumerable<(int r, int c)> ProcessOrder(
            int lines, int holes, bool majorIsY, bool forward, bool serpentine,
            int startRow, int startCol)
        {
            bool startLowCol = startCol <= 1;
            bool startHighCol = startCol >= holes;
            bool startLowRow = startRow <= 1;
            bool startHighRow = startRow >= lines;

            if (majorIsY)
            {
                // 라인 방문 순서는 시작측에 맞춤(일반적으로 1..lines)
                var rowSeq = startLowRow
                    ? Enumerable.Range(1, lines)
                    : startHighRow ? Enumerable.Range(1, lines).Reverse()
                                   : Enumerable.Range(1, lines);

                int i = 0;
                foreach (var r in rowSeq)
                {
                    // dir=true  : 주행축 +방향(=Forward)으로 진행
                    // dir=false : 주행축 -방향(=Backward)으로 진행
                    bool dir = forward ^ (serpentine && ((i++ % 2) == 1));
                    IEnumerable<int> cols;
                    if (startLowCol) cols = dir ? Enumerable.Range(1, holes) : Enumerable.Range(1, holes).Reverse();
                    else if (startHighCol) cols = dir ? Enumerable.Range(1, holes).Reverse() : Enumerable.Range(1, holes);
                    else cols = dir ? Enumerable.Range(1, holes) : Enumerable.Range(1, holes).Reverse();

                    foreach (var c in cols) yield return (r, c);
                }
            }
            else
            {
                // X-주행: 열 방문 순서를 시작측에 맞춤
                var colSeq = startLowCol
                    ? Enumerable.Range(1, holes)
                    : startHighCol ? Enumerable.Range(1, holes).Reverse() : Enumerable.Range(1, holes);
                int j = 0;
                foreach (var c in colSeq)
                {
                    bool dir = forward ^ (serpentine && ((j++ % 2) == 1));
                    IEnumerable<int> rows;
                    if (startLowRow) rows = dir ? Enumerable.Range(1, lines) : Enumerable.Range(1, lines).Reverse();
                    else if (startHighRow) rows = dir ? Enumerable.Range(1, lines).Reverse() : Enumerable.Range(1, lines);
                    else rows = dir ? Enumerable.Range(1, lines) : Enumerable.Range(1, lines).Reverse();
                    foreach (var r in rows) yield return (r, c);
                }
            }
        }
        // 시작 Col 자동결정 (미지정 또는 0이면 FlyDirY 기준)
        private static int ResolveStartColAuto(int startCol, int holes, int flyDirY)
        {
            if (startCol >= 1 && startCol <= holes) return startCol;
            // Y-주행일 때: 진행방향이 +면 1에서 시작, -면 끝에서 시작
            return (flyDirY >= 0) ? 1 : holes;
        }
        // 시작 Row 자동결정 (미지정 또는 0이면 FlyDirX 기준)
        private static int ResolveStartRowAuto(int startRow, int lines, int flyDirX)
        {
            if (startRow >= 1 && startRow <= lines) return startRow;
            // X-주행일 때: 진행방향이 +면 1에서 시작, -면 끝에서 시작
            return (flyDirX >= 0) ? 1 : lines;
        }
        // 주행축: Y-주행일 때, 한 Row 안에서 Col 진행이 StartCol로부터 몇 번째인지(스네이크+Forward/Backward 반영)
        private static int ColIndexFromStart_WithSnakeAndDirection(
            int r, int c, int lines, int holes,
            bool serpentine, bool forward, int startCol)
        {
            // Row 진행 순서에서의 0-based 인덱스(Forward면 0→lines-1, Backward면 반대)
            int rowOrderIdx = forward ? (r - 1) : (lines - r);

            // 짝수/홀수 Row(진행순서 기준)마다 스네이크 반전
            bool reverse = serpentine && ((rowOrderIdx % 2) == 1);

            // 반전되면 좌우를 뒤집은 좌표계에서 인덱스 계산
            int effStart = reverse ? (holes - startCol + 1) : startCol;
            int effCol = reverse ? (holes - c + 1) : c;

            // 이 Row에서 '진행방향'이 증가(+1)인지 감소(-1)인지
            // startCol==1 이면 왼→오 진행, 그런데 reverse면 오→왼으로 뒤집힘
            bool increasing = (startCol == 1) ^ reverse;
            int k = increasing ? (effCol - effStart) : (effStart - effCol); // 0..holes-1
            // 안전망(숫자 깔끔히)
            if (k < 0) k = -k;                // 이 줄은 이론상 필요 없지만 방어차원
            if (k > holes - 1) k = holes - 1; // 방어차원
            return k;
        }
        // 주행축: X-주행일 때, 한 Col 안에서 Row 진행이 StartRow로부터 몇 번째인지(스네이크+Forward/Backward 반영)
        private static int RowIndexFromStart_WithSnakeAndDirection(
            int r, int c, int lines, int holes,
            bool serpentine, bool forward, int startRow, int startCol)
        {
            // Col 진행 순서에서의 0-based 인덱스(Forward면 0→holes-1, Backward면 반대)
            int colOrderIdx = forward ? (c - 1) : (holes - c);

            // 짝수/홀수 Col(진행순서 기준)마다 스네이크 반전
            bool reverse = serpentine && ((colOrderIdx % 2) == 1);
            int effStart = reverse ? (lines - startRow + 1) : startRow;
            int effRow = reverse ? (lines - r + 1) : r;
            bool increasing = (startRow == 1) ^ reverse;
            int k = increasing ? (effRow - effStart) : (effStart - effRow); // 0..lines-1
            if (k < 0) k = -k;
            if (k > lines - 1) k = lines - 1;
            return k;
        }
        private static bool ResolveForwardForLine(
            bool yMajor,
            int targetRowOrCol,
            int lines,
            int holes,
            bool serpentine,
            bool forward,
            int startRow,
            int startCol)
        {
            bool startLowRow = startRow <= 1;
            bool startHighRow = startRow >= lines;
            bool startLowCol = startCol <= 1;
            bool startHighCol = startCol >= holes;

            if (yMajor)
            {
                // Row 방문 순서
                IEnumerable<int> rowSeq = startLowRow ? Enumerable.Range(1, lines)
                    : startHighRow ? Enumerable.Range(1, lines).Reverse()
                                   : Enumerable.Range(1, lines);
                int rowOrderIdx = 0;
                foreach (var r in rowSeq)
                {
                    if (r == targetRowOrCol) break;
                    rowOrderIdx++;
                }
                bool reverse = serpentine && ((rowOrderIdx % 2) == 1);
                return forward ^ reverse;
            }
            else
            {
                // Col 방문 순서
                IEnumerable<int> colSeq = startLowCol ? Enumerable.Range(1, holes)
                    : startHighCol ? Enumerable.Range(1, holes).Reverse()
                                   : Enumerable.Range(1, holes);
                int colOrderIdx = 0;
                foreach (var c in colSeq)
                {
                    if (c == targetRowOrCol) break;
                    colOrderIdx++;
                }
                bool reverse = serpentine && ((colOrderIdx % 2) == 1);
                return forward ^ reverse;
            }
        }
        private void NewRecipe()
        {
            var st = CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
            _doc = st.New();
            PullDocToHeaderUI();
            PullDocToParamUI();
            RebuildFromParameters();
            ClearNeedRebuild();
            ClearNeedApplyReview();
        }
        private void PullDocToHeaderUI()
        {
            if (_doc == null) return;
            _isPullingFromDoc = true;
            try
            {
                txtRecipe.Text = _doc.Header.Name ?? "";
                numScanToRevOfsX.Value = (decimal)_doc.Offset.ScanToReviewOffsetX;
                numScanToRevOfsY.Value = (decimal)_doc.Offset.ScanToReviewOffsetY;
                numRevOfsX.Value = (decimal)_doc.Offset.ReviewOffsetX;
                numRevOfsY.Value = (decimal)_doc.Offset.ReviewOffsetY;
                numTolX.Value = (decimal)_doc.Crit.TolX;
                numTolY.Value = (decimal)_doc.Crit.TolY;
            }
            finally
            {
                _isPullingFromDoc = false;
            }
        }
        private void PushHeaderUIToDoc()
        {
            _doc.Header.Name = txtRecipe.Text ?? "NEW_RECIPE";
            _doc.Offset.ScanToReviewOffsetX = (double)numScanToRevOfsX.Value;
            _doc.Offset.ScanToReviewOffsetY = (double)numScanToRevOfsY.Value;
            _doc.Offset.ReviewOffsetX = (double)numRevOfsX.Value;
            _doc.Offset.ReviewOffsetY = (double)numRevOfsY.Value;
            _doc.Crit.TolX = (double)numTolX.Value;
            _doc.Crit.TolY = (double)numTolY.Value;
        }
        private void PullDocToParamUI()
        {
            if (_doc == null) return;
            _isPullingFromDoc = true;
            try
            {
                numLines.Value = _doc.Parameters.Lines <= 0 ? 1 : _doc.Parameters.Lines;
                numHoles.Value = _doc.Parameters.HolesPerLine <= 0 ? 1 : _doc.Parameters.HolesPerLine;
                numPitchX.Value = (decimal)_doc.Parameters.PitchX;
                numPitchY.Value = (decimal)_doc.Parameters.PitchY;
                numFirstX.Value = (decimal)_doc.Parameters.FirstHoleX;
                numFirstY.Value = (decimal)_doc.Parameters.FirstHoleY;
                if (numVectorRow != null) numVectorRow.Value = _doc.Parameters.VectorRows <= 0 ? 1 : _doc.Parameters.VectorRows;
                if (numVectorCol != null) numVectorCol.Value = _doc.Parameters.VectorCols <= 0 ? 1 : _doc.Parameters.VectorCols;
            }
            finally
            {
                _isPullingFromDoc = false;
            }
        }
        private void PushParamUIToDoc()
        {
            _doc.Parameters.Lines = (int)numLines.Value;
            _doc.Parameters.HolesPerLine = (int)numHoles.Value;
            _doc.Parameters.PitchX = (double)numPitchX.Value;
            _doc.Parameters.PitchY = (double)numPitchY.Value;
            _doc.Parameters.FirstHoleX = (double)numFirstX.Value;
            _doc.Parameters.FirstHoleY = (double)numFirstY.Value;
            if (numVectorRow != null) _doc.Parameters.VectorRows = (int)numVectorRow.Value;
            if (numVectorCol != null) _doc.Parameters.VectorCols = (int)numVectorCol.Value;
        }
        public void RebuildFromParameters(bool preserve = false)
        {
            RebuildFromParametersInternal(preserve, showProgress: false);
        }
        public void RebuildFromParametersWithProgress(bool preserve = false)
        {
            RebuildFromParametersInternal(preserve, showProgress: true);
        }
        private void RebuildFromParametersInternal(bool preserve, bool showProgress)
        {
            StageWin.Etc.BlinkNotifier.Session blink = null;
            if (showProgress)
                blink = StageWin.Etc.BlinkNotifier.Show(this, "RECIPE REBUILD\r\n좌표/맵 생성 중...", 450, Color.DodgerBlue);
            try
            {
                blink?.Update("RECIPE REBUILD\r\nUI → Doc 반영...");
                if (_doc != null) PushParamUIToDoc();
                var prevSel = this.SelectedCell;

                // (preserve keep 구성은 기존 그대로)
                Dictionary<(int r, int c), (double Tpx, double Tpy, double Mpx, double Mpy, int FindResult, string Grade)> keep = null;
                if (preserve)
                {
                    blink?.Update("RECIPE REBUILD\r\n기존 결과 보존(keep) 구성...");
                    keep = new Dictionary<(int, int), (double, double, double, double, int, string)>();
                    foreach (var rr in _review)
                    {
                        if (rr.VectorRow == 0 && rr.VectorCol == 0) keep[(rr.Row, rr.Col)] = (rr.TargetX, rr.TargetY, rr.MarkX, rr.MarkY, rr.FindResult, rr.Grade);

                    }
                    if (_doc != null && _doc.Results != null)
                    {
                        foreach (var kv in _doc.Results)
                        {
                            var sp = kv.Key.Split('-');
                            if (sp.Length == 2 &&
                                int.TryParse(sp[0], out var r) &&
                                int.TryParse(sp[1], out var c))
                            {
                                if (!keep.ContainsKey((r, c)))
                                    keep[(r, c)] = (kv.Value.TargetX, kv.Value.TargetY, kv.Value.MeasX, kv.Value.MeasY, kv.Value.FindResult, kv.Value.Grade);
                            }
                        }
                    }
                }
                // 파라미터 읽기
                blink?.Update("RECIPE REBUILD\r\n파라미터 읽는 중...");
                int lines = Math.Max(1, (int)numLines.Value);
                int holes = Math.Max(1, (int)numHoles.Value);
                double pitchX = (double)numPitchX.Value;
                double pitchY = (double)numPitchY.Value;
                int vecRows = Math.Max(1, _doc?.Parameters?.VectorRows ?? 1);
                int vecCols = Math.Max(1, _doc?.Parameters?.VectorCols ?? 1);
                double vOrgR = CenterOrigin(vecRows);
                double vOrgC = CenterOrigin(vecCols);
                double revOfsX = (double)numRevOfsX.Value;
                double revOfsY = (double)numRevOfsY.Value;
                var sf = _doc?.Parameters?.ScanFlying ?? new RecipeParameters.ScanFlyingParam();
                var rf = _doc?.Parameters?.ReviewFlying ?? new RecipeParameters.ReviewFlyingParam();

                blink?.Update("RECIPE REBUILD\r\n리스트 초기화...");
                _scan.Clear();
                _review.Clear();
                _scanMm.Clear();
                _reviewMm.Clear();

                // Scan(mm) 생성
                blink?.Update("RECIPE REBUILD\r\nScan 좌표 생성 중...");
                int startColScan = ResolveStartColAuto(sf.StartCol, holes, sf.FlyDirY);
                int startRowScan = ResolveStartRowAuto(sf.StartRow, lines, sf.FlyDirX);
                int sidx = 1;

                foreach (var (r, c) in ProcessOrder(lines, holes, sf.MajorIsY, sf.Forward, sf.Serpentine, startRowScan, startColScan))
                {
                    // 1) 물리 좌표(벡터 셀 기준)는 "Review + 상수"
                    //    StagePoint center(=vec center)가 필요하면 vr/vc에 vOrg를 반영해야 하지만,
                    //    여기서는 _scanMm에 stage-point 기준(=vec 0,0이 아니라 center 개념)을 저장하는 게 목적이므로
                    //    vec(0,0) 대신 center에 해당하는 위치를 저장하려면 별도 계산이 필요.
                    //    기존 동작 호환을 위해 vec(0,0) 기준 좌표를 저장하되, 실제 셀 좌표는 아래에서 각각 계산.
                    CalcScanStagePointCenterXY(r, c, out double sx0, out double sy0);

                    // 플라잉 보정 적용된 StagePoint(0,0) 좌표
                    double sxBase = sx0;
                    double syBase = sy0;

                    int k = sf.MajorIsY
                        ? ColIndexFromStart_WithSnakeAndDirection(r, c, lines, holes, sf.Serpentine, sf.Forward, startColScan)
                        : RowIndexFromStart_WithSnakeAndDirection(r, c, lines, holes, sf.Serpentine, sf.Forward, startRowScan, startColScan);

                    if (sf.MajorIsY)
                    {
                        if ((sf.UseFlyCompY || sf.PerHoleTimeSec > 0) && sf.FlySpeedY != 0.0 && sf.PerHoleTimeSec > 0.0)
                            syBase += (sf.FlyDirY >= 0 ? +1.0 : -1.0) * sf.FlySpeedY * sf.PerHoleTimeSec * k;
                    }
                    else
                    {
                        if (sf.FlySpeedX != 0.0 && sf.PerHoleTimeSecX > 0.0)
                            sxBase += (sf.FlyDirX >= 0 ? +1.0 : -1.0) * sf.FlySpeedX * sf.PerHoleTimeSecX * k;
                    }

                    // 플라잉 보정으로 생긴 delta
                    double dxFly = sxBase - sx0;
                    double dyFly = syBase - sy0;

                    // vec 확장 셀에도 동일 delta 적용
                    for (int vR = 0; vR < vecRows; vR++)
                        for (int vC = 0; vC < vecCols; vC++)
                        {
                            CalcScanXY(r, c, vR, vC, out double sxCell, out double syCell);
                            sxCell += dxFly;
                            syCell += dyFly;

                            _scan.Add(new RowColXY
                            {
                                Index = sidx++,
                                Row = r,
                                Col = c,
                                VectorRow = vR,
                                VectorCol = vC,
                                X = sxCell,
                                Y = syCell
                            });
                        }
                    _scanMm[(r, c)] = new XY { X = sxBase, Y = syBase };
                }
                // Review(mm) 생성
                blink?.Update("RECIPE REBUILD\r\nReview 좌표 생성 중...");
                int startColRev = ResolveStartColAuto(rf.StartCol, holes, rf.FlyDirY);
                int startRowRev = ResolveStartRowAuto(rf.StartRow, lines, rf.FlyDirX);
                int ridx = 1;

                foreach (var (r, c) in ProcessOrder(lines, holes, rf.MajorIsY, rf.Forward, rf.Serpentine, startRowRev, startColRev))
                {
                    // Review는 보통 Step move이므로 flycomp은 적용 안 하는 걸 추천(필요 시 별도로)
                    for (int vR = 0; vR < vecRows; vR++)
                        for (int vC = 0; vC < vecCols; vC++)
                        {
                            CalcReviewXY(r, c, vR, vC, out double rx, out double ry);
                            _review.Add(new ReviewRow { Index = ridx++, Row = r, Col = c, VectorRow = vR, VectorCol = vC, StageX = rx, StageY = ry });
                        }
                    CalcReviewStagePointCenterXY(r, c, out double r0x, out double r0y);
                    _reviewMm[(r, c)] = new XY { X = r0x, Y = r0y };
                }
                if (preserve && keep != null)
                {
                    blink?.Update("RECIPE REBUILD\r\n보존 결과 복원 중...");
                    foreach (var rr in _review)
                    {
                        if (keep.TryGetValue((rr.Row, rr.Col), out var v))
                        {
                            rr.TargetX = v.Tpx; rr.TargetY = v.Tpy;
                            rr.MarkX = v.Mpx; rr.MarkY = v.Mpy;
                            rr.FindResult = v.FindResult;
                            RecalcErrByFindResult(rr);
                            rr.Grade = v.Grade;
                        }
                    }
                }
                blink?.Update("RECIPE REBUILD\r\n2D 매트릭스/그리드 갱신 중...");
                BuildScanStageMatrix();
                BuildReviewStageMatrix();
                gridScan.Refresh();
                gridReview.Refresh();
                gridReviewDetail.Refresh();
                UpdateVisionFlyLinesMax();
                RestoreReviewSelection(prevSel.Row, prevSel.Col, prevSel.VecR, prevSel.VecC);
                FireReselectCurrentRecipe();
                RebuildReviewIndex();
                blink?.Update("RECIPE REBUILD\r\n완료");
            }
            catch
            {
                // 기존 catch {} 유지하되, 필요하면 Logger 찍는 걸 추천
            }
            finally
            {
                blink?.Dispose();
            }
        }
        private Dictionary<(int row, int col, int vr, int vc), ReviewRow> _reviewIndex
            = new Dictionary<(int, int, int, int), ReviewRow>();
        private void RebuildReviewIndex()
        {
            _reviewIndex.Clear();
            foreach (var rr in _review)
                _reviewIndex[(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol)] = rr;
        }
        private void BuildScanStageMatrix()
        {
            if (_doc == null) return;

            int lines = Math.Max(1, _doc.Parameters.Lines);
            int holes = Math.Max(1, _doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, _doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, _doc.Parameters.VectorCols);
            double vOrgR = CenterOrigin(vecRows);
            double vOrgC = CenterOrigin(vecCols);
            double pitchX = _doc.Parameters.PitchX;
            double pitchY = _doc.Parameters.PitchY;
            double firstX = _doc.Parameters.FirstHoleX;
            double firstY = _doc.Parameters.FirstHoleY;

            _scanTotalLines = lines;
            _scanTotalHoles = holes;
            _scanVecRows = vecRows;
            _scanVecCols = vecCols;

            var gv = gridScan;
            if (gv == null) return;
            gv.DataSource = null;
            gv.Columns.Clear();
            gv.Rows.Clear();
            gv.Font = new Font("맑은 고딕", 8.0f);
            gv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            gv.SuspendLayout();
            gv.Visible = false;
            try
            {
                gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                gv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                gv.RowTemplate.Height = 60;
                gv.RowHeadersVisible = false;
                gv.AllowUserToAddRows = false;
                // 0번 컬럼: Stage Point 라벨
                var colStage = new DataGridViewTextBoxColumn
                {
                    Name = "colSStagePoint",
                    HeaderText = "Stage Point",
                    Width = 90,
                    ReadOnly = true,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                colStage.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colStage.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                gv.Columns.Add(colStage);

                // Line 헤더 컬럼들 (Line#1, Line#2, ...)
                for (int line = 1; line <= lines; line++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = $"colSL{line}_C{vc}",
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
                for (int hole = 1; hole <= holes; hole++)   // Holes(=StagePoint) × vecRows → 전역 R index
                {
                    for (int vR = 0; vR < vecRows; vR++)
                    {
                        int rowIndex = gv.Rows.Add();
                        var row = gv.Rows[rowIndex];
                        row.Cells[0].Value = (vR == 0) ? $"{hole} ({vecRows}x{lines * vecCols})" : string.Empty;
                        int globalR = (hole - 1) * vecRows + vR;
                        for (int line = 1; line <= lines; line++)
                        {
                            int baseColIndex = 1 + (line - 1) * vecCols;
                            for (int vC = 0; vC < vecCols; vC++)
                            {
                                int colIndex = baseColIndex + vC;
                                // Scan 좌표는 CalcScanXY로만 만든다 (Review + ScanToReviewOffset)
                                CalcScanXY(line, hole, vR, vC, out double cellX, out double cellY);
                                string txt =
                                    $"R{globalR}C{vC}{Environment.NewLine}" +
                                    $"{cellX:F3}{Environment.NewLine}" +
                                    $"{cellY:F3}";
                                row.Cells[colIndex].Value = txt;
                            }
                        }
                    }
                }
                gv.ClearSelection();
            }
            finally
            {
                gv.Visible = true;
                gv.ResumeLayout();
            }
        }
        private void BuildReviewStageMatrix()
        {
            if (_doc == null) return;

            int lines = Math.Max(1, _doc.Parameters.Lines);
            int holes = Math.Max(1, _doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, _doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, _doc.Parameters.VectorCols);

            _reviewTotalLines = lines;
            _reviewTotalHoles = holes;
            _reviewVecRows = vecRows;
            _reviewVecCols = vecCols;

            var gv = gridReview;   // 2D Map
            if (gv == null) return;

            gv.DataSource = null;
            gv.Columns.Clear();
            gv.Rows.Clear();

            gv.Font = new Font("맑은 고딕", 8.0f);
            gv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gv.SuspendLayout();
            gv.Visible = false;

            try
            {
                gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                gv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                gv.RowTemplate.Height = 60;
                gv.RowHeadersVisible = false;
                gv.AllowUserToResizeRows = false;

                // 0번 컬럼: Stage Point 라벨
                var colStage = new DataGridViewTextBoxColumn
                {
                    Name = "colRStagePoint",
                    HeaderText = "Stage Point",
                    Width = 90,
                    ReadOnly = true,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                colStage.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colStage.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                gv.Columns.Add(colStage);

                // Line 헤더 컬럼들 (Line#1, Line#2, ...), vecCols 만큼 확장
                for (int line1 = 1; line1 <= lines; line1++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = $"colRL{line1}_C{vc}",
                            HeaderText = (vc == 0) ? $"Line#{line1}" : string.Empty,
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
                for (int hole1 = 1; hole1 <= holes; hole1++)    // Rows: hole(=StagePoint) 블록 × vecRows
                {
                    int stagePointOffsetR = (hole1 - 1) * vecRows; // 이 Hole 블록의 시작 globalR
                    for (int vr = 0; vr < vecRows; vr++)
                    {
                        int rowIndex = gv.Rows.Add();
                        var row = gv.Rows[rowIndex];
                        row.Cells[0].Value = (vr == 0) ? $"{hole1} ({vecRows}x{lines * vecCols})" : string.Empty;
                        int globalR = stagePointOffsetR + vr; // 0..(holes*vecRows-1)
                        for (int line1 = 1; line1 <= lines; line1++)
                        {
                            int baseColIndex = 1 + (line1 - 1) * vecCols;
                            for (int vc = 0; vc < vecCols; vc++)
                            {
                                int colIndex = baseColIndex + vc;
                                // Review 좌표는 반드시 CalcReviewXY로만 계산
                                CalcReviewXY(line1, hole1, vr, vc, out double stgX, out double stgY);

                                // 해당 셀의 grade는 _review 결과 기반
                                var rrCell = _review.FirstOrDefault(x =>
                                    x.Row == line1 && x.Col == hole1 &&
                                    x.VectorRow == vr && x.VectorCol == vc);
                                _reviewIndex.TryGetValue((line1, hole1, vr, vc), out rrCell);
                                string gradeShort = GetDisplayGrade(rrCell);

                                // MoveReview에서 파싱할 수 있도록 Value를 항상 채워 둔다
                                string txt =
                                    $"R{globalR}C{vc}{Environment.NewLine}" +
                                    $"{stgX:F3}{Environment.NewLine}" +
                                    $"{stgY:F3}{Environment.NewLine}" +
                                    $"{gradeShort}";
                                var cell = row.Cells[colIndex];
                                cell.Value = txt;
                                // CellFormatting에서 좌표/grade 재표시 및 하이라이트에 활용
                                cell.Tag = new ReviewMapTag
                                {
                                    Line1 = line1,
                                    Hole1 = hole1,
                                    Vr = vr,
                                    Vc = vc,
                                    X = stgX,
                                    Y = stgY,
                                    GlobalR = globalR
                                };
                            }
                        }
                    }
                }
                gv.ClearSelection();
            }
            finally
            {
                gv.Visible = true;
                gv.ResumeLayout();
            }
        }
        private void CalcReviewXY(int line1, int hole1, int vr, int vc, out double x, out double y)
        {
            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            line1 = Math.Max(1, Math.Min(lines, line1));
            hole1 = Math.Max(1, Math.Min(holes, hole1));
            vr = Math.Max(0, Math.Min(vecRows - 1, vr));
            vc = Math.Max(0, Math.Min(vecCols - 1, vc));

            double pitchX = doc.Parameters.PitchX;
            double pitchY = doc.Parameters.PitchY;

            double xCenter = doc.Offset?.ReviewOffsetX ?? 0.0;
            double yCenter = doc.Offset?.ReviewOffsetY ?? 0.0;

            double vOrgR = CenterOrigin(vecRows);
            double vOrgC = CenterOrigin(vecCols);

            int globalR = (hole1 - 1) * vecRows + vr;
            int globalC = (line1 - 1) * vecCols + vc;

            x = xCenter - ((globalC - vOrgC) * pitchX);
            y = yCenter - ((globalR - vOrgR) * pitchY);
        }
        private void CalcScanXY(int line1, int hole1, int vr, int vc, out double x, out double y)
        {
            CalcReviewXY(line1, hole1, vr, vc, out x, out y);
            var doc = CurrentDoc;
            // Scan = Review - (ScanToReview)
            x -= doc.Offset?.ScanToReviewOffsetX ?? 0.0;
            y -= doc.Offset?.ScanToReviewOffsetY ?? 0.0;
        }
        private void CalcReviewStagePointCenterXY(int line1, int hole1, out double x, out double y)
        {
            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);
            line1 = Math.Max(1, Math.Min(lines, line1));
            hole1 = Math.Max(1, Math.Min(holes, hole1));
            double pitchX = doc.Parameters.PitchX;
            double pitchY = doc.Parameters.PitchY;
            double xCenter = doc.Offset?.ReviewOffsetX ?? 0.0;
            double yCenter = doc.Offset?.ReviewOffsetY ?? 0.0;

            // StagePoint center는 vec 블록 단위로만 이동
            x = xCenter - ((line1 - 1) * vecCols * pitchX);
            y = yCenter - ((hole1 - 1) * vecRows * pitchY);
        }
        private void CalcScanStagePointCenterXY(int line1, int hole1, out double x, out double y)
        {
            CalcReviewStagePointCenterXY(line1, hole1, out x, out y);

            var doc = CurrentDoc;
            // 그림 기준: Scan = Review - (ScanToReview)
            x -= doc.Offset?.ScanToReviewOffsetX ?? 0.0;
            y -= doc.Offset?.ScanToReviewOffsetY ?? 0.0;
        }
        // VecC를 지정해서 라인(=Row 고정) 좌표를 가져오기
        public ReviewRow[] GetLineReviewRows_ByVecC(int row, int vecC, int startCol, int count)
        {
            return _review
                .Where(r =>
                    r.Row == row &&
                    r.Col >= startCol &&
                    r.Col < startCol + count &&
                    r.VectorRow == 0 &&
                    r.VectorCol == vecC)
                .OrderBy(r => r.Col)
                .ToArray();
        }
        public ReviewRow[] GetLineReviewRows_AllVecR_ByVecC(int row, int vecC, int startCol, int count)
        {
            return _review
                .Where(r =>
                    r.Row == row &&
                    r.Col >= startCol &&
                    r.Col < startCol + count &&
                    r.VectorCol == vecC)                 // vecC 고정
                .OrderBy(r => r.Col)
                .ThenBy(r => r.VectorRow)               // hole 안에서 vecR 순서
                .ToArray();
        }
        // VecC를 지정해서 컬럼(=Col 고정) 좌표를 가져오기 (X-major일 때)
        public ReviewRow[] GetColumnReviewRows_ByVecC(int col, int vecC, int startRow, int count)
        {
            return _review
                .Where(r =>
                    r.Col == col &&
                    r.Row >= startRow &&
                    r.Row < startRow + count &&
                    r.VectorRow == 0 &&
                    r.VectorCol == vecC)
                .OrderBy(r => r.Row)
                .ToArray();
        }
        private void SaveRecipe()
        {
            this.ValidateChildren();
            EnsureRebuildIfNeeded(preserve: false, showProgress: true);
            SaveRecipeInternal(silent: false, confirmOverwrite: true);
        }
        private void SaveRecipeInternal(bool silent, bool confirmOverwrite, RecipeStore targetStore)
        {
            EnsureRebuildIfNeeded(preserve: false, showProgress: false);

            using (StageWin.Etc.BlinkNotifier.Show(this, "RECIPE SAVE\r\n저장 준비 중...", 450, Color.SeaGreen))
            {
                var st = targetStore ?? CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
                try
                {
                    StageWin.Etc.BlinkNotifier.Update("RECIPE SAVE\r\nUI → Doc 반영...");
                    double oldTolX = _doc?.Crit?.TolX ?? 0.0;
                    double oldTolY = _doc?.Crit?.TolY ?? 0.0;
                    PushHeaderUIToDoc();
                    PushParamUIToDoc();
                    double newTolX = _doc?.Crit?.TolX ?? 0.0;
                    double newTolY = _doc?.Crit?.TolY ?? 0.0;

                    bool tolChanged = Math.Abs(newTolX - oldTolX) > 1e-12 ||
                                      Math.Abs(newTolY - oldTolY) > 1e-12;

                    if (tolChanged) RecalcReviewGradesByCurrentTolerance_Once();
                    StageWin.Etc.BlinkNotifier.Update("RECIPE SAVE\r\n파일 저장 중...");
                    st.Save(_doc);
                    StageWin.Etc.BlinkNotifier.Update("RECIPE SAVE\r\n완료");
                    RecipeSaved?.Invoke(_doc.Header.Name);
                    FireReselectCurrentRecipe();
                }
                catch
                {
                    // 기존 catch 유지
                }
            }
        }
        private void SaveRecipeInternal(bool silent, bool confirmOverwrite = true)
        {
            SaveRecipeInternal(silent, confirmOverwrite, targetStore: null);
        }
        private void DeleteRecipe()
        {
            if (string.IsNullOrWhiteSpace(_doc.Header.Name))
            {
                MessageBox.Show(this, "Recipe name is empty.", "Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(this, $"Delete recipe '{_doc.Header.Name}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var removed = _doc.Header.Name;
            if (OpenOrigin == RecipeOpenOrigin.Scan)
            {
                //  원격 삭제 요청은 Form1에서 수행하도록 이벤트만 올림
                NewRecipe();
                RecipeDeleteRequested?.Invoke(removed);
                return;
            }
            // Local 삭제
            var st = _storeLocal ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
            st.Delete(removed);
            Logger.Warn("Local Recipe deleted: " + removed);
            NewRecipe();
            RecipeDeleted?.Invoke(removed);
        }
        private void CommitRecipe()
        {
            if (_isCommitting) return;

            this.ValidateChildren();
            EnsureRebuildIfNeeded(preserve: false, showProgress: true);
            var name = (_doc?.Header?.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "Recipe name is empty.", "Commit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _isCommitting = true;
            using (StageWin.Etc.BlinkNotifier.Show(this, "RECIPE COMMIT\r\n반영 중...", 450, Color.DarkOrange))
            {
                try
                {
                    // (권장) Commit 시 항상 Local 백업 저장
                    StageWin.Etc.BlinkNotifier.Update("RECIPE COMMIT\r\nLocal 저장 중...");
                    SaveRecipeInternal(silent: true, confirmOverwrite: false, targetStore: _storeLocal);

                    // Scan 반영(원격/공유 저장) 요청은 기존 이벤트로 위임
                    StageWin.Etc.BlinkNotifier.Update("RECIPE COMMIT\r\nScan 반영 요청 중...");
                    RecipeCommitted?.Invoke(name);
                    FireReselectCurrentRecipe();
                    StageWin.Etc.BlinkNotifier.Update("RECIPE COMMIT\r\n완료");
                }
                finally
                {
                    _isCommitting = false;
                }
            }
        }
        // 가공(1009)과 리뷰(Flying Vision)에서 동일하게 사용할 드로우리스트.
        // 내부 좌표 취득은 기존 구현을 그대로 사용하되 "순서"만 위 생성기에 맞춥니다.
        public NetCommon.ST_DRAW_DATA_LIST[] BuildDrawListForScan()
        {
            EnsureRebuildIfNeeded(preserve: false, showProgress: false);

            var doc = CurrentDoc;
            if (doc == null) return Array.Empty<NetCommon.ST_DRAW_DATA_LIST>();
            var order = GetScanOrderSequence();
            if (order == null || order.Length == 0) return Array.Empty<NetCommon.ST_DRAW_DATA_LIST>();
            var list = new List<NetCommon.ST_DRAW_DATA_LIST>(order.Length);

            foreach (var (row, col) in order)
            {
                if (!_scanMm.TryGetValue((row, col), out var mm))
                {
                    if (!_reviewMm.TryGetValue((row, col), out mm))
                        throw new InvalidOperationException($"Scan mm 좌표 누락: Row={row}, Col={col}");
                }
                int line1 = row;
                int hole1 = col;
                double ofsX = 0.0, ofsY = 0.0;
                TryGetOffsetFromDoc(line1, hole1, 0, 0, out ofsX, out ofsY);

                var d = new NetCommon.ST_DRAW_DATA_LIST
                {
                    nRowNo = row,
                    nColNo = col,
                    dMarkX = mm.X,
                    dMarkY = mm.Y,
                    dOffsetX = ofsX,
                    dOffsetY = ofsY,
                };
                var t = d.toolParam;
                d.toolParam = t;
                list.Add(d);
            }
            return list.ToArray();
        }
        #endregion

        #region ======= 모션 및 검사, 리뷰관련 기능들 =======
        public Cell SelectedCell
        {
            get
            {
                if (_lastSelectedCell.HasValue)
                {
                    var c = _lastSelectedCell.Value;
                    bool existsExact = _review.Any(r => r.Row == c.Row && r.Col == c.Col && r.VectorRow == c.VecR && r.VectorCol == c.VecC);
                    if (existsExact) return c;

                    bool exists00 = _review.Any(r => r.Row == c.Row && r.Col == c.Col && r.VectorRow == 0 && r.VectorCol == 0);
                    if (exists00) return new Cell { Row = c.Row, Col = c.Col, VecR = 0, VecC = 0 };
                }

                if (gridReviewDetail?.CurrentRow?.DataBoundItem is ReviewRow rr1)
                    return new Cell { Row = rr1.Row, Col = rr1.Col, VecR = rr1.VectorRow, VecC = rr1.VectorCol };

                var first00 = _review.FirstOrDefault(r => r.VectorRow == 0 && r.VectorCol == 0);
                if (first00 != null) return new Cell { Row = first00.Row, Col = first00.Col, VecR = 0, VecC = 0 };

                var first = _review.FirstOrDefault();
                return (first != null)
                    ? new Cell { Row = first.Row, Col = first.Col, VecR = first.VectorRow, VecC = first.VectorCol }
                    : new Cell { Row = 1, Col = 1, VecR = 0, VecC = 0 };
            }
        }
        private ReviewRow FindReviewRow(int row, int col)
            => _review.FirstOrDefault(x => x.Row == row && x.Col == col && x.VectorRow == 0 && x.VectorCol == 0);

        // Vec 포함 정확 매칭(우선) + (0,0) fallback
        private ReviewRow FindReviewRow(int row, int col, int vecR, int vecC)
            => _review.FirstOrDefault(x => x.Row == row && x.Col == col && x.VectorRow == vecR && x.VectorCol == vecC)
               ?? _review.FirstOrDefault(x => x.Row == row && x.Col == col && x.VectorRow == 0 && x.VectorCol == 0);
        public ReviewRow[] GetLineReviewRows(int row, int startCol, int count)
        {
            return _review
                .Where(r =>
                    r.Row == row &&
                    r.Col >= startCol &&
                    r.Col < startCol + count &&
                    r.VectorRow == 0 && r.VectorCol == 0)
                .OrderBy(r => r.Col)
                .ToArray();
        }
        public ReviewRow[] GetColumnReviewRows(int col, int startRow, int count)
        {
            return _review
                .Where(r =>
                    r.Col == col &&
                    r.Row >= startRow &&
                    r.Row < startRow + count &&
                    r.VectorRow == 0 && r.VectorCol == 0)
                .OrderBy(r => r.Row)
                .ToArray();
        }
        public bool TryGetTargetXY(int row, int col, out double tx, out double ty)
        {
            var r = FindReviewRow(row, col);
            if (r != null)
            {
                tx = r.TargetX; ty = r.TargetY;
                return true;
            }
            tx = ty = 0;
            return false;
        }
        private bool TryCalcReviewCellStageXY(int line1, int hole1, int vr, int vc, out double x, out double y)
        {
            x = y = 0.0;
            if (CurrentDoc == null) return false;
            CalcReviewXY(line1, hole1, vr, vc, out x, out y);
            return true;
        }
        private bool TryGetOffsetFromDoc(int line1, int hole1, int vecR, int vecC, out double ox, out double oy)
        {
            ox = 0.0; oy = 0.0;
            var doc = _doc;
            if (doc == null) return false;

            try
            {
                // 1) vec 결과 우선
                if (doc.ResultsVec != null)
                {
                    string k4 = Key4(line1, hole1, vecR, vecC);
                    if (doc.ResultsVec.TryGetValue(k4, out var rv) && rv != null)
                    {
                        GetAppliedOffsetOrMeasuredErr(rv, out ox, out oy);
                        return true;
                    }
                    string k00 = Key4(line1, hole1, 0, 0);
                    if (doc.ResultsVec.TryGetValue(k00, out var r00) && r00 != null)
                    {
                        GetAppliedOffsetOrMeasuredErr(r00, out ox, out oy);
                        return true;
                    }
                }
                // 2) StagePoint 결과 fallback
                if (doc.Results != null)
                {
                    string k2 = Key2(line1, hole1);
                    if (doc.Results.TryGetValue(k2, out var r2) && r2 != null)
                    {
                        GetAppliedOffsetOrMeasuredErr(r2, out ox, out oy);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private static bool GetAppliedOffsetOrMeasuredErr(MeasResult r, out double ox, out double oy)
        {
            return MeasResult.TryGetAppliedOffset(r, out ox, out oy);
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
                if (GetZPos != null && txtZPos != null)
                {
                    var z = GetZPos().ToString("F3");
                    if (txtZPos.Text != z) txtZPos.Text = z;
                }
                if (GetThetaPos != null && txtTPos != null)
                {
                    var t = GetThetaPos().ToString("F3");
                    if (txtTPos.Text != t) txtTPos.Text = t;
                }
            }
            catch { }
        }
        private void FireReselectCurrentRecipe(bool force = false)
        {
            try
            {
                var name = (_doc?.Header?.Name ?? _lastPickedRecipeName ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name)) return;
                if (!force && _lastReselectOrigin == OpenOrigin &&
                    string.Equals(_lastReselectName, name, StringComparison.OrdinalIgnoreCase))
                    return;
                _lastReselectOrigin = OpenOrigin;
                _lastReselectName = name;
                RequestReselectCurrentRecipe?.Invoke(OpenOrigin, name);
            }
            catch { /* silent */ }
        }
        // ForceAbort 시 하이라이트/상태 정리(Invoke-safe)
        private void ClearRunningHighlights_AbortSafe()
        {
            try
            {
                _runningVecHL.Clear();
                _runningLineHL.Clear();

                if (gridReview == null) return;
                if (gridReview.InvokeRequired) gridReview.BeginInvoke(new Action(() => gridReview.Invalidate()));
                else gridReview.Invalidate();
            }
            catch { }
        }
        private void MarkNeedRebuild()
        {
            _needRebuild = true;
            if (btnRebuild != null)
            {
                if (_rebuildBtnText == null) _rebuildBtnText = btnRebuild.Text;
                if (_rebuildBtnBack == default) _rebuildBtnBack = btnRebuild.BackColor;
                btnRebuild.Text = (_rebuildBtnText ?? "Rebuild") + " *";
                btnRebuild.BackColor = Color.Gold;
            }
        }
        private void ClearNeedRebuild()
        {
            _needRebuild = false;
            if (btnRebuild != null)
            {
                if (!string.IsNullOrEmpty(_rebuildBtnText)) btnRebuild.Text = _rebuildBtnText;
                if (_rebuildBtnBack != default) btnRebuild.BackColor = _rebuildBtnBack;
            }
        }
        private void EnsureRebuildIfNeeded(bool preserve = false, bool showProgress = true)
        {
            if (!_needRebuild) return;

            // UI값 → Doc 반영 + 좌표/맵 재생성
            RebuildFromParametersInternal(preserve, showProgress);
            ClearNeedRebuild();
        }
        private void RememberSelectionFromGrid()
        {
            var rr = gridReviewDetail?.CurrentRow?.DataBoundItem as ReviewRow;
            if (rr != null)
            {
                _lastSelectedCell = new Cell
                {
                    Row = rr.Row,
                    Col = rr.Col,
                    VecR = rr.VectorRow,
                    VecC = rr.VectorCol
                };
            }
        }
        private ISafetyContext TryGetSafetyCtx()
        {
            var ctx = GetSafetyContext?.Invoke();
            if (ctx != null) return ctx;

            if (_fallbackCtx == null)
            {
                _fallbackCtx = new BasicSafetyContext(modeGetter: () => (ModeProvider != null ? ModeProvider() : ProgramMode.Manual));
            }
            return _fallbackCtx;
        }
        private bool GuardAllowAxisMove()
        {
            var ctx = TryGetSafetyCtx();
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
        private void HighlightCell(int row, int col, int vr, int vc, bool on)
        {
            if (gridReview == null) return;

            // WinForms UI thread 보장
            if (gridReview.InvokeRequired)
            {
                gridReview.BeginInvoke(new Action(() => HighlightCell(row, col, vr, vc, on)));
                return;
            }
            // "현재 측정 중 셀 1개"만 유지(요구사항)
            if (on)
            {
                _runningVecHL.Clear();
                _runningVecHL.Add((row, col, vr, vc));
            }
            else
            {
                _runningVecHL.Remove((row, col, vr, vc));
            }
            gridReview.Invalidate(); // CellFormatting 다시 타게
        }
        private void HighlightFlyingCells(int line1, int startCol, int count, int vecC, bool on)
        {
            for (int i = 0; i < count; i++)
            {
                int col = startCol + i;
                _runningVecHL.Add((line1, col, 0, vecC));
            }
            gridReview?.Invalidate();
        }
        private void HighlightFlyLine(int lineIndex1, bool on)
        {
            if (gridReview == null) return;
            if (gridReview.InvokeRequired)
            {
                gridReview.BeginInvoke(new Action(() => HighlightFlyLine(lineIndex1, on)));
                return;
            }
            if (on) _runningLineHL.Add(lineIndex1);
            else _runningLineHL.Remove(lineIndex1);
            gridReview.Invalidate();
        }
        private void HighlightFlyingSegment(FlyingVisionPartialPlan p, bool on)
        {
            if (p == null || gridReview == null) return;

            void Do()
            {
                var doc = CurrentDoc;
                int vecRows = Math.Max(1, doc?.Parameters?.VectorRows ?? 1);

                if (on)
                {
                    // Step처럼: "이번 plan"만 노랗게 유지
                    _runningVecHL.Clear();
                    _runningLineHL.Clear(); // (가능하면 앞으로 _runningLineHL 자체를 안 쓰는 게 제일 깔끔)

                    for (int i = 0; i < p.Count; i++)
                    {
                        if (p.YMajor)
                        {
                            int row1 = p.Row;
                            int col1 = p.StartCol + i;
                            for (int vr = 0; vr < vecRows; vr++)
                                _runningVecHL.Add((row1, col1, vr, p.VecC));
                        }
                        else
                        {
                            int row1 = p.StartCol + i; // StartCol이 startRow 역할
                            int col1 = p.Row;          // Row가 col 고정
                            for (int vr = 0; vr < vecRows; vr++)
                                _runningVecHL.Add((row1, col1, vr, p.VecC));
                        }
                    }
                }
                else
                {
                    // OFF면 이번 plan 범위 제거 + 혹시 남아있으면 정리
                    for (int i = 0; i < p.Count; i++)
                    {
                        if (p.YMajor)
                        {
                            int row1 = p.Row;
                            int col1 = p.StartCol + i;
                            for (int vr = 0; vr < vecRows; vr++)
                                _runningVecHL.Remove((row1, col1, vr, p.VecC));
                        }
                        else
                        {
                            int row1 = p.StartCol + i;
                            int col1 = p.Row;
                            for (int vr = 0; vr < vecRows; vr++)
                                _runningVecHL.Remove((row1, col1, vr, p.VecC));
                        }
                    }
                }
                gridReview.Invalidate();
            }
            if (gridReview.InvokeRequired) gridReview.BeginInvoke(new Action(Do));
            else Do();
        }
        private void ApplyLineSummaryUi_AfterPlanDone(FlyingVisionPartialPlan p)
        {
            if (p == null || _doc == null) return;

            // yMajor일 때만 "Line#n"이 gridReview의 컬럼 그룹과 1:1 대응됨
            if (!p.YMajor) return;

            // plan 범위의 셀들 grade를 모아서 라인 결과 결정
            // 기준: 하나라도 NG면 NG, 전부 OK면 OK, 그 외(미측정 섞임)는 IDLE
            string lineGrade = EvalPlanGrade(p);

            // 헤더 색/텍스트 반영
            SetReviewLineHeaderGrade(line1: p.Row, grade: lineGrade);
        }
        private string EvalPlanGrade(FlyingVisionPartialPlan p)
        {
            bool anyMeasured = false;
            bool anyNg = false;
            bool allOk = true;
            int vecRows = Math.Max(1, _doc?.Parameters?.VectorRows ?? 1);

            for (int i = 0; i < p.Count; i++)
            {
                int row1 = p.Row;
                int col1 = p.StartCol + i;

                for (int vr = 0; vr < vecRows; vr++)
                {
                    var rr = _review.FirstOrDefault(x =>
                        x.Row == row1 && x.Col == col1 && x.VectorRow == vr && x.VectorCol == p.VecC);
                    if (rr == null) continue;
                    var g = GetDisplayGrade(rr); // OK/NG/IDLE
                    if (g != "IDLE") anyMeasured = true;
                    if (g == "NG") anyNg = true;
                    if (g != "OK") allOk = false;
                }
            }
            if (!anyMeasured) return "IDLE";
            if (anyNg) return "NG";
            if (allOk) return "OK";
            return "NG"; // 섞여 있으면 보수적으로 NG(원하면 "PARTIAL" 같은 표기도 가능)
        }
        private void SetReviewLineHeaderGrade(int line1, string grade)
        {
            if (gridReview == null) return;

            // gridReview 컬럼 구조: [0]=StagePoint, 이후 (line1-1)*vecCols + vc
            int vecCols = Math.Max(1, _doc?.Parameters?.VectorCols ?? 1);
            int baseCol = 1 + (line1 - 1) * vecCols;

            Color back;
            Color fore;

            switch ((grade ?? "").ToUpperInvariant())
            {
                case "OK": back = _clrOkBack; fore = _clrOkFore; break;
                case "NG": back = _clrNgBack; fore = _clrNgFore; break;
                default: back = _clrIdleBack; fore = _clrIdleFore; break;
            }
            // vecCols 그룹 전체 헤더에 색 적용
            for (int vc = 0; vc < vecCols; vc++)
            {
                int colIndex = baseCol + vc;
                if (colIndex < 0 || colIndex >= gridReview.Columns.Count) continue;
                var hc = gridReview.Columns[colIndex].HeaderCell;
                hc.Style.BackColor = back;
                hc.Style.ForeColor = fore;
                if (vc == 0) gridReview.Columns[colIndex].HeaderText = $"Line#{line1} [{grade}]";
            }
            gridReview.Invalidate();
        }
        private void ApplyPlanResultsToDocOffsets(FlyingVisionPartialPlan p)
        {
            ApplyOneLinePlanResultsToDocOffsets(p);
        }
        private void ApplyOneLinePlanResultsToDocOffsets(FlyingVisionPartialPlan p)
        {
            if (p == null || _doc == null) return;

            // Results / ResultsVec "초기화/클리어" 절대 하지 말 것!
            // (OneLine에서 다른 라인 결과를 유지해야 함)
            if (_doc.Results == null) _doc.Results = new Dictionary<string, MeasResult>();
            if (_doc.ResultsVec == null) _doc.ResultsVec = new Dictionary<string, MeasResult>();

            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);

            for (int i = 0; i < p.Count; i++)
            {
                // yMajor: row=라인, col=홀
                // xMajor: col=라인, row=홀(=startMajor 진행)
                int row1, col1;

                if (p.YMajor)
                {
                    row1 = p.Row;
                    col1 = p.StartCol + i;
                }
                else
                {
                    row1 = p.StartCol + i;   // StartCol이 startRow 역할
                    col1 = p.Row;            // Row가 col 고정(라인)
                }
                // 이 plan은 vecC 고정, vecR은 전체 반영
                for (int vr = 0; vr < vecRows; vr++)
                {
                    var rr = _review.FirstOrDefault(x =>
                        x.Row == row1 && x.Col == col1 &&
                        x.VectorRow == vr && x.VectorCol == p.VecC);

                    if (rr == null) continue;

                    // "측정된 것만" 반영 (IDLE/미측정이면 기존 doc 유지)
                    // - FindResult==1 이거나
                    // - 측정 흔적이 있는 경우(실패 NG 포함)
                    if (rr.FindResult == 1 || HasMeasuredTrace(rr))
                        UpsertMeasResultToDoc(rr);
                }
            }
        }
        private void SaveCurrentDocSafely()
        {
            try
            {
                var st = CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
                st.Save(_doc);
                RecipeSaved?.Invoke(_doc.Header.Name);
            }
            catch (Exception ex)
            {
                Logger.Error("[AutoInspection] Save failed", ex);
                MessageBox.Show(this, ex.Message, "Auto Inspection Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RestoreReviewSelection(int wantRow, int wantCol, int wantVr, int wantVc)
        {
            if (gridReviewDetail == null || _review.Count == 0) return;

            var rr = _review.FirstOrDefault(x =>
                        x.Row == wantRow && x.Col == wantCol &&
                        x.VectorRow == wantVr && x.VectorCol == wantVc)
                    ?? _review.FirstOrDefault(x => x.Row == wantRow && x.Col == wantCol && x.VectorRow == 0 && x.VectorCol == 0)
                    ?? _review.FirstOrDefault(x => x.VectorRow == 0 && x.VectorCol == 0);
            if (rr == null) return;
            _lastSelectedCell = new Cell { Row = rr.Row, Col = rr.Col, VecR = rr.VectorRow, VecC = rr.VectorCol };
            UpdateDetailViewForCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
        }
        private void RestoreReviewSelection(int wantRow, int wantCol)
        {
            RestoreReviewSelection(wantRow, wantCol, 0, 0);
        }
        // 취소를 강제로 반영시키기 위한 공용 래퍼 (Task)
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
        // 취소를 강제로 반영시키기 위한 공용 래퍼 (Task<T>)
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
        // Review 측정 시작 시마다 "새 CTS 발급" (이전 측정이 있으면 즉시 취소)
        private CancellationToken BeginReviewMeasurementScope()
        {
            return BeginReviewMeasurementScope(CancellationToken.None);
        }

        private CancellationToken BeginReviewMeasurementScope(CancellationToken externalToken)
        {
            try { _reviewMeasCts?.Cancel(); } catch { }
            try { _reviewMeasCts?.Dispose(); } catch { }

            if (externalToken.CanBeCanceled)
                _reviewMeasCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            else
                _reviewMeasCts = new CancellationTokenSource();

            return _reviewMeasCts.Token;
        }
        private void DoMoveAxesFromScanGrid()
        {
            var cell = gridScan.CurrentCell;    // 선택 셀 확인
            if (cell == null || cell.RowIndex < 0 || cell.ColumnIndex < 0)
            {
                MessageBox.Show(this, "이동할 스캔 좌표 셀을 선택하세요.", "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cell.ColumnIndex == 0)
            {
                MessageBox.Show(this, "Stage Point 라벨이 아닌 좌표 셀을 선택하세요.", "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!GuardAllowAxisMove()) return;

            // 1) 셀 값에서 X,Y 파싱
            // BuildScanStageMatrix 에서 저장한 형식:
            // "R{rIndexInLine}C{cIndexInLine}\r\n{cellX:F3}\r\n{cellY:F3}"
            var text = cell.Value?.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(this, "선택한 셀에 유효한 좌표가 없습니다.", "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 줄 구분 (CRLF / LF 모두 허용)
            var parts = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (parts.Length < 3)
            {
                MessageBox.Show(this, "좌표 형식을 인식할 수 없습니다.\r\n" + "예상 형식: R..C.. / X / Y", "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double targetX, targetY;
            if (!double.TryParse(parts[1], out targetX) ||
                !double.TryParse(parts[2], out targetY))
            {
                MessageBox.Show(this, "좌표 숫자 파싱에 실패했습니다.", "Move Scan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var cur = GetAxesPos != null ? GetAxesPos() : new XY();
            using (var dlg = new MoveConfirmForm("Pass under Scanner (ReviewX/MainY)", cur.X, cur.Y, targetX, targetY))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes) MoveAxes?.Invoke(targetX, targetY);   // 실제 축 이동: ReviewX/MainY 좌표로 이동
            }
        }
        private void DoMoveAxesFromReviewGrid()
        {
            var gv = gridReview;
            if (gv == null)
            {
                MessageBox.Show(this, "Review 그리드가 초기화되지 않았습니다.",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 1) 선택된 셀 확인
            var cell = gv.CurrentCell;
            if (cell == null || cell.RowIndex < 0 || cell.ColumnIndex < 0)
            {
                MessageBox.Show(this, "이동할 Review 좌표 셀을 선택하세요.",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // 0번 컬럼은 Stage Point 라벨이므로, 좌표 셀이 아님
            if (cell.ColumnIndex == 0)
            {
                MessageBox.Show(this, "Stage Point 라벨이 아닌 좌표 셀을 선택하세요.",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // 2) 축 안전 인터락 체크
            if (!GuardAllowAxisMove()) return;
            // 3) 셀 텍스트에서 X,Y 파싱
            var text = cell.Value?.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(this, "선택한 셀에 유효한 좌표가 없습니다.",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 줄 구분 (CRLF / LF 모두 허용)
            var parts = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();
            if (parts.Length < 3)
            {
                MessageBox.Show(this,
                    "좌표 형식을 인식할 수 없습니다.\r\n" +
                    "예상 형식: R..C.. / X / Y",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double targetX, targetY;
            // parts[1] = X, parts[2] = Y
            if (!double.TryParse(parts[1], out targetX) ||
                !double.TryParse(parts[2], out targetY))
            {
                MessageBox.Show(this, "좌표 숫자 파싱에 실패했습니다.",
                    "Move Review", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 4) 현재 위치 읽기 + 이동 확인 팝업
            var cur = GetAxesPos != null ? GetAxesPos() : new XY();

            using (var dlg = new MoveConfirmForm(
                "Move to Review (Stage mm)",
                cur.X, cur.Y,     // 현재 위치
                targetX, targetY  // 선택 셀의 X/Y
            ))
            {
                // 실제 축 이동: ReviewX/MainY(mm) 로 이동
                if (dlg.ShowDialog(this) == DialogResult.Yes) MoveAxes?.Invoke(targetX, targetY);
            }
        }
        private async Task DoMeasureSelectedAsync()
        {
            var ct = BeginReviewMeasurementScope();
            ct.ThrowIfCancellationRequested();

            var rsel = gridReviewDetail.CurrentRow?.DataBoundItem as ReviewRow;
            if (rsel == null)
            {
                MessageBox.Show(this, "측정할 셀을 선택하세요.", "Single Measure",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (BeginReviewMeasureBatch(
                "Single Selected Review",
                "3002_SINGLE",
                string.Format("Selected Row={0}, Col={1}, VecR={2}, VecC={3}",
                    rsel.Row, rsel.Col, rsel.VectorRow, rsel.VectorCol)))
            {
                bool ok = await WithCancellation(MeasureOneReviewRowAsync(rsel, "3002"), ct);

                if (ok)
                {
                    gridReviewDetail.Refresh();
                    RefreshReviewMap();
                }
            }
        }
        private FlyingVisionPartialPlan BuildLinePlanForLineIndex1_AndVecC(int lineIndex1, int vecC)
        {
            if (_doc == null || _review.Count == 0) return null;

            int lines = Math.Max(1, _doc.Parameters.Lines);
            int holes = Math.Max(1, _doc.Parameters.HolesPerLine);
            int vecCols = Math.Max(1, _doc.Parameters.VectorCols);

            vecC = Math.Max(0, Math.Min(vecCols - 1, vecC));

            var rf = _doc.Parameters.ReviewFlying ?? new RecipeParameters.ReviewFlyingParam();
            bool yMajor = rf.MajorIsY;

            int startRow = ResolveStartRowAuto(rf.StartRow, lines, rf.FlyDirX);
            int startCol = ResolveStartColAuto(rf.StartCol, holes, rf.FlyDirY);

            int totalLines = yMajor ? lines : holes;
            lineIndex1 = Math.Max(1, Math.Min(totalLines, lineIndex1));

            bool forwardThis = ResolveForwardForLine(
                yMajor: yMajor,
                targetRowOrCol: lineIndex1,
                lines: lines,
                holes: holes,
                serpentine: rf.Serpentine,
                forward: rf.Forward,
                startRow: startRow,
                startCol: startCol);

            ReviewRow[] fullSegment = yMajor
                ? GetLineReviewRows_ByVecC(lineIndex1, vecC, 1, holes)
                : GetColumnReviewRows_ByVecC(lineIndex1, vecC, 1, lines);

            if (fullSegment == null || fullSegment.Length == 0) return null;

            ReviewRow[] ordered = yMajor
                ? (forwardThis ? fullSegment.OrderBy(r => r.StageY).ToArray()
                               : fullSegment.OrderByDescending(r => r.StageY).ToArray())
                : (forwardThis ? fullSegment.OrderBy(r => r.StageX).ToArray()
                               : fullSegment.OrderByDescending(r => r.StageX).ToArray());

            var first = ordered.First();
            var last = ordered.Last();
            int countOnMajor = yMajor ? holes : lines;

            return new FlyingVisionPartialPlan
            {
                Row = lineIndex1,
                StartCol = 1,
                Count = countOnMajor,
                VecC = vecC,
                StartX = first.StageX,
                StartY = first.StageY,
                EndX = last.StageX,
                EndY = last.StageY,
                YMajor = yMajor,
                ForwardThis = forwardThis
            };
        }
        
        private FlyingVisionPartialPlan[] BuildFlyingPlans_ByColumn()
        {
            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool yMajor = rf.MajorIsY;

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            var sel = this.SelectedCell;

            int totalLines = yMajor ? lines : holes;
            int startLineIndex1 = yMajor ? sel.Row : sel.Col;     // 1-based
            startLineIndex1 = Math.Max(1, Math.Min(totalLines, startLineIndex1));

            int startVecC = Math.Max(0, Math.Min(vecCols - 1, sel.VecC));

            int max = CalcMaxFlyVisionColumns();
            int want = (int)(numFlyVisionLines?.Value ?? 1);
            want = Math.Max(1, Math.Min(max, want));

            var plans = new List<FlyingVisionPartialPlan>(want);

            int li = startLineIndex1;
            int vc = startVecC;

            for (int i = 0; i < want; i++)
            {
                var p = BuildLinePlanForLineIndex1_AndVecC(li, vc);
                if (p != null) plans.Add(p);

                vc++;
                if (vc >= vecCols)
                {
                    vc = 0;
                    li++;
                    if (li > totalLines) break;
                }
            }
            return plans.ToArray();
        }
        internal async Task DoMeasureFlyingLinesByCountAsync()
        {
            var vs = GetVisionSessionOrWarn();
            if (vs == null) { MessageBox.Show("Inspection failed, Check your Vision Client Connection"); return; }

            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            SyncReviewFlyingParamsFromUi_NoRebuild();

            var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool yMajor = rf.MajorIsY;

            var plans = BuildFlyingPlans_ByColumn();
            if (plans.Length == 0) return;

            if (RequestRunFlyingVisionPartial == null && RequestRunFlyingVisionPlans == null)
                throw new InvalidOperationException("RequestRunFlyingVisionPartial/Plans 연결이 없습니다.");
            ClearRunningHighlights_AbortSafe();

            foreach (var p in plans)
            {
                // 지금 진행하는 라인(=plan)만 노랗게
                HighlightFlyingSegment(p, true);
                try
                {
                    if (RequestRunFlyingVisionPartial != null)
                        await RequestRunFlyingVisionPartial(p);
                    else
                        await RequestRunFlyingVisionPlans(new[] { p });
                    // 라인(=plan) 하나 끝났으면 그 라인 결과를 즉시 계산해서 표시
                    ApplyLineSummaryUi_AfterPlanDone(p);
                }
                finally
                {
                    HighlightFlyingSegment(p, false);
                }
                // UI가 “다음 라인으로 넘어가는 느낌” 나게 한번 양보
                await Task.Yield();
            }
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
        }
        // Step + Line 측정용: 특정 Line#(1-based)에 대해 "모든 셀"을 R-major 순서로 나열
        // 순서: globalR(= (hole-1)*vecRows + vecR) 증가 -> vecC 증가
        private IEnumerable<(int Line1, int Hole1, int VecR, int VecC, double X, double Y)>
            EnumerateReviewLineAllCells_RowMajor(int line1, int startHole1, int holeCount, bool wholeHoles)
        {
            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            line1 = Math.Max(1, Math.Min(lines, line1));
            startHole1 = Math.Max(1, Math.Min(holes, startHole1));

            int takeHoles = wholeHoles ? holes : Math.Max(1, holeCount);
            int endHole1 = wholeHoles ? holes : Math.Min(holes, startHole1 + takeHoles - 1);

            for (int hole1 = startHole1; hole1 <= endHole1; hole1++)
            {
                for (int vr = 0; vr < vecRows; vr++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        if (!TryCalcReviewCellStageXY(line1, hole1, vr, vc, out double stgX, out double stgY)) continue;
                        yield return (line1, hole1, vr, vc, stgX, stgY);
                    }
                }
            }
        }
        private IEnumerable<(int Line1, int Hole1, int VecR, int VecC, double X, double Y)>
            EnumerateReviewColumnAllCells_RowMajor(int col1, int startRow1, int rowCount, bool wholeRows)
        {
            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            col1 = Math.Max(1, Math.Min(holes, col1));
            startRow1 = Math.Max(1, Math.Min(lines, startRow1));

            int takeRows = wholeRows ? lines : Math.Max(1, rowCount);
            int endRow1 = wholeRows ? lines : Math.Min(lines, startRow1 + takeRows - 1);

            // col1 고정, row1 진행 (RowMajor 요구 그대로 유지)
            for (int row1 = startRow1; row1 <= endRow1; row1++)
            {
                for (int vr = 0; vr < vecRows; vr++)
                {
                    for (int vc = 0; vc < vecCols; vc++)
                    {
                        // (line1=row1, hole1=col1) 로 호출
                        if (!TryCalcReviewCellStageXY(row1, col1, vr, vc, out double stgX, out double stgY)) continue;
                        yield return (row1, col1, vr, vc, stgX, stgY);
                    }
                }
            }
        }
        private async Task<bool> MeasureOneReviewRowAsync(ReviewRow rr, string logOpTag)
        {
            //var vs = GetVisionSessionOrWarn();
            //if (vs == null) return false;

            if (rr == null) return false;
            HighlightCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol, true);

            try
            {
                if (!GuardAllowAxisMove()) return false;
                if (MoveAxesAsync != null) await MoveAxesAsync(rr.StageX, rr.StageY);
                if (IsAxesSettledAtAsync != null)
                {
                    bool ok = await IsAxesSettledAtAsync(rr.StageX, rr.StageY);
                    if (!ok) throw new TimeoutException("Axis settle timeout");
                }
                var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
                var rf = doc.Parameters?.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
                bool yMajor = rf.MajorIsY;

                int nLine = yMajor ? rr.Row : rr.Col;
                CalcGlobalRC(yMajor: yMajor, line1: rr.Row, hole1: rr.Col, vecR: rr.VectorRow, vecC: rr.VectorCol,
                    out int gR, out int gC);

                var raw = await MeasureAtExAsync(rr.StageX, rr.StageY, nLine, gR, gC);
                rr.FindResult = raw.FindResult;
                if (raw.TargetX != 0 || raw.TargetY != 0)
                {
                    rr.TargetX = raw.TargetX;
                    rr.TargetY = raw.TargetY;
                }
                rr.MarkX = raw.HoleX;
                rr.MarkY = raw.HoleY;
                RecalcErrByFindResult(rr);
                rr.Grade = (rr.FindResult == 1) ? GradeByTol(rr.ErrXpx, rr.ErrYpx) : "NG";
                Logger.Info($"[{logOpTag}] MarkFind " +
                    $"R{rr.Row}C{rr.Col} V({rr.VectorRow},{rr.VectorCol}) " +
                    $"Find={rr.FindResult} " +
                    $"Tgt=({rr.TargetX:F3},{rr.TargetY:F3}) " +
                    $"Hole=({rr.MarkX:F3},{rr.MarkY:F3}) " +
                    $"ErrPx=({rr.ErrXpx:F3},{rr.ErrYpx:F3}) " +
                    $"ErrMm=({rr.ErrX:F6},{rr.ErrY:F6}) Grade={rr.Grade}");
                WriteReviewMeasureTextLog(rr, logOpTag, null, "MeasureOneReviewRowAsync");
                return true;
            }
            catch (OperationCanceledException)
            {
                Logger.Warn($"[{logOpTag}] CANCELED");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error($"[{logOpTag}] MeasureOneReviewRowAsync", ex);
                return false;
            }
            finally
            {
                HighlightCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol, false);
            }
        }
        private async Task DoMeasureLineOptionAsync(CancellationToken ct)
        {
            var vs = GetVisionSessionOrWarn();
            if (vs == null) { MessageBox.Show("Inspection failed, Check your Vision Client Connection"); return; }

            EnsureRebuildIfNeeded(preserve: true, showProgress: true);

            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            SyncReviewFlyingParamsFromUi_NoRebuild();

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            var rf = doc.Parameters?.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool isStepMode = (rbtnVisionStepbyStep?.Checked ?? false);

            using (BeginReviewMeasureBatch(
            isStepMode ? "StepByStep Line Review" : "Flying Vision Review",
            isStepMode ? "3002_STEP_LINE" : "3003_FLYING_LINES",
            "DoMeasureLineOptionAsync"))
            {
                if (!isStepMode)
                {
                    // ApplyLineMarkResults()에서 이미 MarkNeedApplyReview()를 올릴 수도 있지만,
                    // 안전하게 한 번 더 pending 표시
                    await WithCancellation(DoMeasureFlyingLinesByCountAsync(), ct);
                    MarkNeedApplyReview();
                    return;
                }
                if (MoveAxesAsync == null || MeasureAtExAsync == null)
                    throw new InvalidOperationException("MoveAxesAsync / MeasureAtAsync 연결이 없습니다.");
                bool yMajor = rf.MajorIsY;
                bool wholeMajor = (rbtnVisionStepAllHoles?.Checked ?? false);
                bool allWholeLines = (rbtnVisionStepAllWholeLine?.Checked ?? false);
                int stepCount = Math.Max(1, (int)(numVisionStepCount?.Value ?? 1));
                int done = 0;
                int tried = 0;

                if (allWholeLines)
                {
                    await DoMeasureStepByStepAllWholeLineCoreAsync(ct, "3002_STEP_ALL_WHOLELINE");
                    return;
                }

                // ====== 기존 “선택 라인만” Step 측정 로직은 그대로 유지 ======
                var sel = this.SelectedCell;
                int selRow1 = Math.Max(1, Math.Min(lines, sel.Row));
                int selCol1 = Math.Max(1, Math.Min(holes, sel.Col));
                IEnumerable<(int Line1, int Hole1, int VecR, int VecC, double X, double Y)> seq;
                if (yMajor)
                {
                    int line1 = selRow1;
                    int startHole1 = wholeMajor ? 1 : selCol1;
                    seq = EnumerateReviewLineAllCells_RowMajor(line1, startHole1, stepCount, wholeHoles: wholeMajor);
                }
                else
                {
                    int col1 = selCol1;
                    int startRow1 = wholeMajor ? 1 : selRow1;
                    seq = EnumerateReviewColumnAllCells_RowMajor(col1, startRow1, stepCount, wholeRows: wholeMajor);
                }
                foreach (var it in seq)
                {
                    ct.ThrowIfCancellationRequested();
                    tried++;
                    _lastSelectedCell = new Cell { Row = it.Line1, Col = it.Hole1, VecR = it.VecR, VecC = it.VecC };
                    UpdateDetailViewForCell(it.Line1, it.Hole1, it.VecR, it.VecC);

                    var rr = _review.FirstOrDefault(r =>
                        r.Row == it.Line1 && r.Col == it.Hole1 &&
                        r.VectorRow == it.VecR && r.VectorCol == it.VecC);
                    if (rr == null) continue;
                    rr.StageX = it.X;
                    rr.StageY = it.Y;
                    bool ok = await MeasureOneReviewRowAsync(rr, "3002_STEP_LINE");
                    if (ok) done++;
                    gridReviewDetail?.Invalidate();
                    gridReview?.Invalidate();
                    await Task.Yield();
                }
                gridReviewDetail?.Refresh();
                RefreshReviewMap();
                Logger.Info($"[3002_STEP_LINE] Done={done} Tried={tried} StepMode=TRUE MajorIsY={yMajor} " +
                    $"WholeMajor={wholeMajor} StepCount={stepCount} Selected(Row,Col)=({selRow1},{selCol1})");
            }
        }
        private async Task DoMeasureStepByStepAllWholeLineCoreAsync(CancellationToken ct, string logOpTag)
        {
            using (BeginReviewMeasureBatch(
            "StepByStep All WholeLine",
            logOpTag,
            "DoMeasureStepByStepAllWholeLineCoreAsync"))
            {
                var vs = GetVisionSessionOrWarn();
                if (vs == null)
                    throw new InvalidOperationException("Vision 연결이 없습니다.");

                EnsureRebuildIfNeeded(preserve: true, showProgress: true);

                var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
                SyncReviewFlyingParamsFromUi_NoRebuild();

                if (MoveAxesAsync == null || MeasureAtExAsync == null)
                    throw new InvalidOperationException("MoveAxesAsync / MeasureAtAsync 연결이 없습니다.");

                int lines = Math.Max(1, doc.Parameters.Lines);
                int holes = Math.Max(1, doc.Parameters.HolesPerLine);

                var rf = doc.Parameters?.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
                bool yMajor = rf.MajorIsY;

                int totalLineCount = yMajor ? lines : holes;
                int done = 0;
                int tried = 0;

                ClearRunningHighlights_AbortSafe();

                for (int li = 1; li <= totalLineCount; li++)
                {
                    ct.ThrowIfCancellationRequested();

                    IEnumerable<(int Line1, int Hole1, int VecR, int VecC, double X, double Y)> seqAll;

                    if (yMajor)
                    {
                        seqAll = EnumerateReviewLineAllCells_RowMajor(
                            line1: li,
                            startHole1: 1,
                            holeCount: holes,
                            wholeHoles: true);
                    }
                    else
                    {
                        seqAll = EnumerateReviewColumnAllCells_RowMajor(
                            col1: li,
                            startRow1: 1,
                            rowCount: lines,
                            wholeRows: true);
                    }

                    foreach (var it in seqAll)
                    {
                        ct.ThrowIfCancellationRequested();

                        tried++;

                        _lastSelectedCell = new Cell
                        {
                            Row = it.Line1,
                            Col = it.Hole1,
                            VecR = it.VecR,
                            VecC = it.VecC
                        };

                        UpdateDetailViewForCell(it.Line1, it.Hole1, it.VecR, it.VecC);

                        var rr = _review.FirstOrDefault(r =>
                            r.Row == it.Line1 &&
                            r.Col == it.Hole1 &&
                            r.VectorRow == it.VecR &&
                            r.VectorCol == it.VecC);

                        if (rr == null) continue;

                        rr.StageX = it.X;
                        rr.StageY = it.Y;

                        bool ok = await MeasureOneReviewRowAsync(rr, logOpTag);
                        if (ok) done++;

                        gridReviewDetail?.Invalidate();
                        gridReview?.Invalidate();

                        await Task.Yield();
                    }
                }
                gridReviewDetail?.Refresh();
                RefreshReviewMap();

                Logger.Info($"[{logOpTag}] Done={done} Tried={tried} MajorIsY={yMajor} StepByStepAllInspection=TRUE");
            }
            
        }
        public async Task DoSequenceStepByStepAllInspectionAsync(bool confirmApply, CancellationToken externalToken = default(CancellationToken))
        {
            string opName = confirmApply
                ? "Semi-Auto StepByStep All Inspection"
                : "Auto StepByStep All Inspection";

            if (confirmApply)
            {
                await RunManualMeasureWithApplyConfirmAsync(
                    opName,
                    async ct => await DoMeasureStepByStepAllWholeLineCoreAsync(ct, "3002_SEMI_STEP_ALL_WHOLELINE"),
                    externalToken);

                return;
            }

            var snap = CaptureReviewSnapshot();
            var ctAuto = BeginReviewMeasurementScope(externalToken);

            try
            {
                await DoMeasureStepByStepAllWholeLineCoreAsync(ctAuto, "3002_AUTO_STEP_ALL_WHOLELINE");

                if (HasReviewChanged(snap))
                {
                    ApplyAllReviewResultsToDocOffsets(saveToFile: true);
                    ClearNeedApplyReview();
                    FireReselectCurrentRecipe(force: true);

                    Logger.Info("[Auto StepByStep All Inspection] Applied review results automatically.");
                }
            }
            catch (OperationCanceledException)
            {
                RestoreReviewSnapshot(snap);
                ClearNeedApplyReview();
                ClearRunningHighlights_AbortSafe();
                throw;
            }
            catch (Exception ex)
            {
                RestoreReviewSnapshot(snap);
                ClearNeedApplyReview();
                ClearRunningHighlights_AbortSafe();

                Logger.Error("[Auto StepByStep All Inspection] failed", ex);
                throw;
            }
        }
        public async Task<int> DoAutoInspectionAsync(AutoInspectionMode mode, int nextLineIndex1, int vecC = 0)
        {
            EnsureRebuildIfNeeded(preserve: true, showProgress: false);

            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            SyncReviewFlyingParamsFromUi_NoRebuild();

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool yMajor = rf.MajorIsY;

            // base "라인 개수"(기존 정의): yMajor면 lines, xMajor면 holes
            int baseLineCount = yMajor ? lines : holes;
            if (baseLineCount <= 0) baseLineCount = 1;

            // Auto에서의 "검사 라인"은 (lineIndex1, vecC) 조합으로 확장
            int totalPlanCount = baseLineCount * vecCols;
            if (totalPlanCount <= 0) totalPlanCount = 1;

            if (RequestRunFlyingVisionPartial == null && RequestRunFlyingVisionPlans == null)
                throw new InvalidOperationException("RequestRunFlyingVisionPartial/Plans 연결이 없습니다.");
            ClearRunningHighlights_AbortSafe();

            // 1) OneLine: 확장 planIndex(=nextLineIndex1) 기준으로 1개만 수행
            if (mode == AutoInspectionMode.OneLine)
            {
                int planIndex1 = nextLineIndex1;
                if (planIndex1 < 1 || planIndex1 > totalPlanCount) planIndex1 = 1;
                // planIndex1 -> (lineIndex1, vecC)로 변환
                int li = ((planIndex1 - 1) / vecCols) + 1;   // 1-based
                int vc = (planIndex1 - 1) % vecCols;         // 0-based
                var p = BuildLinePlanForLineIndex1_AndVecC(li, vc);
                if (p != null)
                {
                    HighlightFlyingSegment(p, true);
                    try
                    {
                        if (RequestRunFlyingVisionPartial != null)
                            await RequestRunFlyingVisionPartial(p);
                        else
                            await RequestRunFlyingVisionPlans(new[] { p });

                        ApplyLineSummaryUi_AfterPlanDone(p);
                    }
                    finally
                    {
                        HighlightFlyingSegment(p, false);
                    }
                    // "해당 plan(=line+vecC)만" Doc에 반영
                    ApplyOneLinePlanResultsToDocOffsets(p);
                    SaveCurrentDocSafely();
                    ClearNeedApplyReview();
                    FireReselectCurrentRecipe(force: true);
                }
                // 다음 planIndex로 순환
                int nextPlan = (planIndex1 >= totalPlanCount) ? 1 : (planIndex1 + 1);
                gridReviewDetail?.Refresh();
                RefreshReviewMap();
                return nextPlan;
            }
            // 2) AllLines: baseLine 전체 + vecC 전체 전수 검사
            for (int li = 1; li <= baseLineCount; li++)
            {
                for (int vc = 0; vc < vecCols; vc++)
                {
                    var p = BuildLinePlanForLineIndex1_AndVecC(li, vc);
                    if (p == null) continue;

                    HighlightFlyingSegment(p, true);
                    try
                    {
                        if (RequestRunFlyingVisionPartial != null)
                            await RequestRunFlyingVisionPartial(p);
                        else
                            await RequestRunFlyingVisionPlans(new[] { p });

                        ApplyLineSummaryUi_AfterPlanDone(p);
                    }
                    finally
                    {
                        HighlightFlyingSegment(p, false);
                    }
                    ApplyPlanResultsToDocOffsets(p);
                    await Task.Yield();
                }
            }
            SaveCurrentDocSafely();
            ClearNeedApplyReview();
            FireReselectCurrentRecipe(force: true);
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
            return 1;
        }
        public void SyncReviewFlyingParamsFromUi_NoRebuild()
        {
            var doc = CurrentDoc;
            if (doc == null) return;

            var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool yMajor = (rbtnYInspection?.Checked ?? true);
            // backward 라디오가 있으면 그걸 우선 반영
            bool forward = (rbtnBackwardInspection?.Checked ?? false) ? false : (rbtnForwardInspection?.Checked ?? true);
            // one-way 라디오가 있으면 snake 강제 off
            bool snake = (rbtnOneWayInspection?.Checked ?? false) ? false : (rbtnSnakeInspection?.Checked ?? false);
            rf.MajorIsY = yMajor;
            rf.Forward = forward;
            rf.Serpentine = snake;
            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            rf.FlyDirY = yMajor ? (forward ? +1 : -1) : 0;
            rf.FlyDirX = yMajor ? 0 : (forward ? +1 : -1);
        }
        public void ApplyLineMarkResults(bool yMajor, int lineIndex1, int startCol1, int vecC, bool forwardThis, MarkResult[] results)
        {
            if (results == null || results.Length == 0) return;
            UI(() =>
            {
                var doc = CurrentDoc;
                int vecRows = Math.Max(1, doc?.Parameters?.VectorRows ?? 1);
                bool useVecR = yMajor && vecRows > 1 && (results.Length % vecRows == 0);

                for (int i = 0; i < results.Length; i++)
                {
                    int row, col, vr = 0;
                    if (yMajor)
                    {
                        row = lineIndex1;
                        if (useVecR)
                        {
                            int j = i;
                            int holeOfs = j / vecRows;
                            vr = j % vecRows;
                            col = startCol1 + holeOfs;
                        }
                        else
                        {
                            col = startCol1 + i;
                            vr = 0;
                        }
                    }
                    else
                    {
                        col = lineIndex1;
                        row = startCol1 + i;
                        vr = 0;
                    }
                    var rr = _review.FirstOrDefault(x => x.Row == row && x.Col == col && x.VectorRow == vr && x.VectorCol == vecC);
                    if (rr == null) continue;

                    rr.FindResult = results[i].Result;
                    rr.MarkX = results[i].MarkX;
                    rr.MarkY = results[i].MarkY;
                    rr.TargetX = results[i].TargetX;
                    rr.TargetY = results[i].TargetY;
                    RecalcErrByFindResult(rr);
                    rr.Grade = (rr.FindResult == 1) ? GradeByTol(rr.ErrXpx, rr.ErrYpx) : (HasMeasuredTrace(rr) ? "NG" : "IDLE");
                    string flyingMethod =
                        "FlyingVision_" +
                        (yMajor ? "YMajor" : "XMajor") + "_" +
                        (forwardThis ? "FWD" : "BWD");
                    string flyingNote = string.Format(
                        "LineIndex1={0}, StartMajor1={1}, VecC={2}, ResultIndex={3}/{4}",
                        lineIndex1,
                        startCol1,
                        vecC,
                        i + 1,
                        results.Length);
                    WriteReviewMeasureTextLog(
                        rr,
                        "3003_FLYING_LINE",
                        flyingMethod,
                        flyingNote);
                    MarkNeedApplyReview();
                    InvalidateReviewMapCell(rr);
                    var sel = this.SelectedCell;
                    if (gridReviewDetail != null && sel.Row == rr.Row && sel.Col == rr.Col && sel.VecR == rr.VectorRow && sel.VecC == rr.VectorCol)
                        gridReviewDetail.Invalidate();
                }
                gridReview?.Invalidate();
            });
        }
        private void ApplySavedResultsToReview()
        {
            if (_doc == null) return;

            if (_doc.ResultsVec != null && _doc.ResultsVec.Count > 0)
            {
                foreach (var kv in _doc.ResultsVec)
                {
                    if (!TryParseKey(kv.Key, out int r, out int c, out int vr, out int vc, out bool isVecKey)) continue;
                    var rr = _review.FirstOrDefault(x => x.Row == r && x.Col == c && x.VectorRow == vr && x.VectorCol == vc);
                    if (rr == null) continue;

                    var v = kv.Value;
                    rr.TargetX = v.TargetX;
                    rr.TargetY = v.TargetY;
                    rr.MarkX = v.MeasX;
                    rr.MarkY = v.MeasY;
                    rr.FindResult = v.FindResult;
                    RecalcErrByFindResult(rr);
                    rr.Grade = string.IsNullOrWhiteSpace(v.Grade) ? "IDLE" : v.Grade;
                }
                gridReview?.Refresh();
                gridReviewDetail?.Refresh();
                return;
            }

            if (_doc.Results == null || _doc.Results.Count == 0) return;

            foreach (var kv in _doc.Results)
            {
                if (!TryParseKey(kv.Key, out int r, out int c, out int vr, out int vc, out bool isVecKey)) continue;
                var rr = _review.FirstOrDefault(x => x.Row == r && x.Col == c && x.VectorRow == 0 && x.VectorCol == 0);
                if (rr == null) continue;

                var v = kv.Value;
                rr.TargetX = v.TargetX;
                rr.TargetY = v.TargetY;
                rr.MarkX = v.MeasX;
                rr.MarkY = v.MeasY;
                rr.FindResult = v.FindResult;
                RecalcErrByFindResult(rr);
                rr.Grade = string.IsNullOrWhiteSpace(v.Grade) ? "IDLE" : v.Grade;
            }
            gridReview?.Refresh();
            gridReviewDetail?.Refresh();
        }
        private static MeasResult BuildMeasResultWithAppliedOffset(ReviewRow r, MeasResult previous, bool resetAppliedOffsets)
        {
            var result = new MeasResult
            {
                Row = r.Row,
                Col = r.Col,
                VectorRow = r.VectorRow,
                VectorCol = r.VectorCol,
                TargetX = r.TargetX,
                TargetY = r.TargetY,
                MeasX = r.MarkX,
                MeasY = r.MarkY,
                ErrX = r.ErrX,
                ErrY = r.ErrY,
                MeasuredErrX = r.ErrX,
                MeasuredErrY = r.ErrY,
                HasMeasuredErr = r.FindResult == 1,
                Grade = r.Grade ?? "",
                FindResult = r.FindResult
            };

            result.AccumulateAppliedOffset(previous, resetAppliedOffsets);
            return result;
        }

        private static MeasResult SnapshotAppliedOffsetSource(MeasResult source)
        {
            if (source == null) return null;
            return new MeasResult
            {
                TargetX = source.TargetX,
                TargetY = source.TargetY,
                MeasX = source.MeasX,
                MeasY = source.MeasY,
                ErrX = source.ErrX,
                ErrY = source.ErrY,
                AppliedOffsetX = source.AppliedOffsetX,
                AppliedOffsetY = source.AppliedOffsetY,
                HasAppliedOffset = source.HasAppliedOffset,
                MeasuredErrX = source.MeasuredErrX,
                MeasuredErrY = source.MeasuredErrY,
                HasMeasuredErr = source.HasMeasuredErr,
                FindResult = source.FindResult
            };
        }

        private void ApplyAllReviewResultsToDocOffsets(bool saveToFile, bool resetAppliedOffsets = false)
        {
            EnsureRebuildIfNeeded(preserve: true, showProgress: false);
            if (_doc == null) return;

            var prevResults = _doc.Results != null
                ? new Dictionary<string, MeasResult>(_doc.Results, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, MeasResult>(StringComparer.OrdinalIgnoreCase);
            var prevResultsVec = _doc.ResultsVec != null
                ? new Dictionary<string, MeasResult>(_doc.ResultsVec, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, MeasResult>(StringComparer.OrdinalIgnoreCase);

            if (_doc.Results == null) _doc.Results = new Dictionary<string, MeasResult>();
            else _doc.Results.Clear();

            if (_doc.ResultsVec == null) _doc.ResultsVec = new Dictionary<string, MeasResult>();
            else _doc.ResultsVec.Clear();

            int total = _review.Count;
            for (int i = 0; i < total; i++)
            {
                var r = _review[i];
                string k4 = Key4(r.Row, r.Col, r.VectorRow, r.VectorCol);
                prevResultsVec.TryGetValue(k4, out var prevVec);
                _doc.ResultsVec[k4] = BuildMeasResultWithAppliedOffset(r, prevVec, resetAppliedOffsets);

                if (r.VectorRow == 0 && r.VectorCol == 0)
                {
                    string k2 = Key2(r.Row, r.Col);
                    prevResults.TryGetValue(k2, out var prevPoint);
                    _doc.Results[k2] = BuildMeasResultWithAppliedOffset(r, prevPoint, resetAppliedOffsets);
                }
            }
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
            if (saveToFile)
            {
                try
                {
                    var st = CurrentStore ?? new RecipeStore(AppConfig.Current.LocalRecipeDirPath);
                    st.Save(_doc);
                    RecipeSaved?.Invoke(_doc.Header.Name);
                }
                catch (Exception ex)
                {
                    Logger.Error("[ApplyReviewResult] Save failed", ex);
                    MessageBox.Show(this, ex.Message, "Apply Review Results", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Logger.Info($"[ApplyReviewResult] Applied review results into Doc + Save={saveToFile}. Total={total}");
        }
        private Dictionary<string, ReviewState> CaptureReviewSnapshot()
        {
            var snap = new Dictionary<string, ReviewState>(_review.Count);
            foreach (var rr in _review)
            {
                snap[Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol)] = new ReviewState
                {
                    TargetX = rr.TargetX,
                    TargetY = rr.TargetY,
                    MarkX = rr.MarkX,
                    MarkY = rr.MarkY,
                    ErrX = rr.ErrX,
                    ErrY = rr.ErrY,
                    FindResult = rr.FindResult,
                    Grade = rr.Grade ?? "IDLE"
                };
            }
            return snap;
        }
        private void RestoreReviewSnapshot(Dictionary<string, ReviewState> snap)
        {
            if (snap == null) return;

            foreach (var rr in _review)
            {
                if (snap.TryGetValue(Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol), out var s))
                {
                    rr.TargetX = s.TargetX; rr.TargetY = s.TargetY;
                    rr.MarkX = s.MarkX; rr.MarkY = s.MarkY;
                    rr.ErrX = s.ErrX; rr.ErrY = s.ErrY;
                    rr.FindResult = s.FindResult;
                    rr.Grade = string.IsNullOrWhiteSpace(s.Grade) ? "IDLE" : s.Grade;
                }
                else
                {
                    // 스냅샷에 없던 신규 셀(파라미터 변경 등)은 기본값으로
                    rr.TargetX = rr.TargetY = rr.MarkX = rr.MarkY = rr.ErrX = rr.ErrY = 0;
                    rr.FindResult = 0;
                    rr.Grade = "IDLE";
                }
            }
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
        }
        private static bool ReviewStateEquals(ReviewState a, ReviewState b)
        {
            return a.TargetX == b.TargetX
                && a.TargetY == b.TargetY
                && a.MarkX == b.MarkX
                && a.MarkY == b.MarkY
                && a.ErrX == b.ErrX
                && a.ErrY == b.ErrY
                && a.FindResult == b.FindResult
                && string.Equals(a.Grade ?? "IDLE", b.Grade ?? "IDLE", StringComparison.OrdinalIgnoreCase);
        }
        private bool HasReviewChanged(Dictionary<string, ReviewState> snap)
        {
            if (snap == null) return true;

            foreach (var rr in _review)
            {
                var k = Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
                var cur = new ReviewState
                {
                    TargetX = rr.TargetX,
                    TargetY = rr.TargetY,
                    MarkX = rr.MarkX,
                    MarkY = rr.MarkY,
                    ErrX = rr.ErrX,
                    ErrY = rr.ErrY,
                    FindResult = rr.FindResult,
                    Grade = rr.Grade ?? "IDLE"
                };

                if (!snap.TryGetValue(k, out var prev)) return true;
                if (!ReviewStateEquals(cur, prev)) return true;
            }
            return false;
        }
        private void MarkNeedApplyReview()
        {
            _needApplyReview = true;

            if (btnApplyReviewResult != null)
            {
                if (_applyBtnText == null) _applyBtnText = btnApplyReviewResult.Text;
                if (_applyBtnBack == default) _applyBtnBack = btnApplyReviewResult.BackColor;

                btnApplyReviewResult.Text = (_applyBtnText ?? "Apply") + " *";
                btnApplyReviewResult.BackColor = Color.Gold;
            }
        }
        private void ClearNeedApplyReview()
        {
            _needApplyReview = false;

            if (btnApplyReviewResult != null)
            {
                if (!string.IsNullOrEmpty(_applyBtnText)) btnApplyReviewResult.Text = _applyBtnText;
                if (_applyBtnBack != default) btnApplyReviewResult.BackColor = _applyBtnBack;
            }
        }
        // Manual 측정(싱글/스텝/플라잉) 끝난 뒤 Apply 여부를 확인하고
        // Yes면 적용+저장, No면 스냅샷 롤백.
        private async Task RunManualMeasureWithApplyConfirmAsync(
    string opName,
    Func<CancellationToken, Task> runner,
    CancellationToken externalToken = default(CancellationToken))
        {
            var snap = CaptureReviewSnapshot();
            var ct = BeginReviewMeasurementScope(externalToken);

            try
            {
                await runner(ct);

                if (!HasReviewChanged(snap))
                {
                    gridReviewDetail?.Refresh();
                    RefreshReviewMap();
                    return;
                }

                MarkNeedApplyReview();
                gridReviewDetail?.Refresh();
                RefreshReviewMap();

                var msg =
                    "측정 결과가 생성되었습니다.\r\n\r\n" +
                    "지금 바로 레시피에 적용하고 저장할까요?\r\n\r\n" +
                    "Yes  : 적용 + 저장\r\n" +
                    "No   : 저장하지 않음(화면 유지, Apply 버튼으로 저장 가능)\r\n" +
                    "Cancel : 취소(원상복구)";

                var dr = MessageBox.Show(this, msg, opName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    ApplyAllReviewResultsToDocOffsets(saveToFile: true);
                    ClearNeedApplyReview();
                    FireReselectCurrentRecipe(force: true);
                }
                else if (dr == DialogResult.No)
                {
                    MarkNeedApplyReview();
                }
                else
                {
                    RestoreReviewSnapshot(snap);
                    ClearNeedApplyReview();
                    ClearRunningHighlights_AbortSafe();
                }
            }
            catch (OperationCanceledException)
            {
                RestoreReviewSnapshot(snap);
                ClearNeedApplyReview();
                ClearRunningHighlights_AbortSafe();
            }
            catch (Exception ex)
            {
                RestoreReviewSnapshot(snap);
                ClearNeedApplyReview();
                Logger.Error($"[{opName}] failed", ex);
                MessageBox.Show(this, ex.Message, opName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private const double ManualInspectPosTolMm = 0.001;
        private bool NeedMoveTo(double targetX, double targetY)
        {
            if (GetAxesPos == null) return false;
            var cur = GetAxesPos();
            return Math.Abs(cur.X - targetX) > ManualInspectPosTolMm
                || Math.Abs(cur.Y - targetY) > ManualInspectPosTolMm;
        }
        private async Task DoManualInspectionFromVisionAsync()
        {
            var vs = GetVisionSessionOrWarn();
            if (vs == null) { MessageBox.Show("Inspection failed, Check your Vision Client Connection"); return; }

            var cell = this.SelectedCell;
            if (WaitManualInspectionAsync == null)
            {
                MessageBox.Show(this, "Manual Inspection I/F가 연결되어 있지 않습니다.",
                    "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!TryCalcReviewCellStageXY(cell.Row, cell.Col, cell.VecR, cell.VecC, out double targetX, out double targetY))
            {
                MessageBox.Show(this, "선택 셀의 Stage 좌표 계산 실패",
                    "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (NeedMoveTo(targetX, targetY))
            {
                if (!GuardAllowAxisMove()) return;
                try
                {
                    if (MoveAxesAsync != null) await MoveAxesAsync(targetX, targetY);
                    if (IsAxesSettledAtAsync != null)
                    {
                        bool ok = await IsAxesSettledAtAsync(targetX, targetY);
                        if (!ok) throw new TimeoutException("Axis settle timeout (ManualInspection move)");
                    }
                    else
                    {
                        var cur2 = GetAxesPos != null ? GetAxesPos() : new XY();
                        if (Math.Abs(cur2.X - targetX) > ManualInspectPosTolMm || Math.Abs(cur2.Y - targetY) > ManualInspectPosTolMm)
                            throw new TimeoutException("Axis not at target (ManualInspection move)");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Manual Inspection 이동 실패: " + ex.Message,
                        "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            ST_MANUAL_INSPECTION_REQ req;
            try
            {
                req = await WaitManualInspectionAsync(cell);
            }
            catch (TimeoutException)
            {
                MessageBox.Show(this, "Vision ManualInspection(3008) 요청이 없어 타임아웃 처리되었습니다.",
                    "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Manual Inspection 대기 실패: " + ex.Message,
                    "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!ApplyManualInspectionToCell(cell, req))
            {
                MessageBox.Show(this, "선택 셀을 찾을 수 없어 Manual Inspection 결과를 반영하지 못했습니다.",
                    "Manual Inspection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MarkNeedApplyReview();
            gridReviewDetail.Refresh();
            RefreshReviewMap();
        }
        private bool ApplyManualInspectionToCell(Cell cell, ST_MANUAL_INSPECTION_REQ req)
        {
            // vec 포함 정확매칭 우선, 없으면 vec(0,0) fallback
            var rr = _review.FirstOrDefault(x =>
                x.Row == cell.Row && x.Col == cell.Col &&
                x.VectorRow == cell.VecR && x.VectorCol == cell.VecC) ?? _review.FirstOrDefault(x =>
                    x.Row == cell.Row && x.Col == cell.Col &&
                    x.VectorRow == 0 && x.VectorCol == 0);

            if (rr == null) return false;

            rr.FindResult = req.nResult;
            rr.TargetX = req.dTargetX;
            rr.TargetY = req.dTargetY;
            rr.MarkX = req.dMarkX;
            rr.MarkY = req.dMarkY;
            // Err는 px 기준으로 즉시 갱신 (내부적으로 um 저장 구조 유지)
            RecalcErrByFindResult(rr);
            // Grade는 ManualInspection 결과 기반으로 자동 세팅(원치 않으면 이 줄 제거)
            // rr.Grade = (req.nResult == 1) ? "OK" : "NG";
            // 선택/화면 갱신 일관성 유지
            if (req.nResult == 1) rr.Grade = GradeByTol(rr.ErrXpx, rr.ErrYpx);
            else rr.Grade = HasMeasuredTrace(rr) ? "NG" : "IDLE";
            WriteReviewMeasureTextLog(
                rr,
                "3008_MANUAL_INSPECTION",
                "ManualInspectionFromVision",
                "Vision -> Stage ManualInspection result");
            RestoreReviewSelection(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            UpdateDetailViewForCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            return true;
        }
        private void UpsertMeasResultToDoc(ReviewRow rr)
        {
            if (_doc == null || rr == null) return;
            // StagePoint Results
            if (_doc.Results == null) _doc.Results = new Dictionary<string, MeasResult>();
            // vec 전체 ResultsVec
            if (_doc.ResultsVec == null) _doc.ResultsVec = new Dictionary<string, MeasResult>();
            // vec 결과는 ResultsVec에 항상 저장
            string k4 = Key4(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            if (!_doc.ResultsVec.TryGetValue(k4, out var resV) || resV == null) resV = new MeasResult();
            var prevVec = SnapshotAppliedOffsetSource(resV);
            resV.Row = rr.Row;
            resV.Col = rr.Col;
            resV.VectorRow = rr.VectorRow;
            resV.VectorCol = rr.VectorCol;
            resV.TargetX = rr.TargetX;
            resV.TargetY = rr.TargetY;
            resV.MeasX = rr.MarkX;
            resV.MeasY = rr.MarkY;
            resV.ErrX = rr.ErrX;
            resV.ErrY = rr.ErrY;
            resV.MeasuredErrX = rr.ErrX;
            resV.MeasuredErrY = rr.ErrY;
            resV.HasMeasuredErr = rr.FindResult == 1;
            resV.Grade = rr.Grade ?? "";
            resV.FindResult = rr.FindResult;
            resV.AccumulateAppliedOffset(prevVec, resetAppliedOffsets: false);

            _doc.ResultsVec[k4] = resV;

            if (rr.VectorRow == 0 && rr.VectorCol == 0)
            {
                string k2 = Key2(rr.Row, rr.Col);

                if (!_doc.Results.TryGetValue(k2, out var res2) || res2 == null) res2 = new MeasResult();
                var prevPoint = SnapshotAppliedOffsetSource(res2);
                res2.Row = rr.Row;
                res2.Col = rr.Col;
                res2.VectorRow = 0;
                res2.VectorCol = 0;
                res2.TargetX = rr.TargetX;
                res2.TargetY = rr.TargetY;
                res2.MeasX = rr.MarkX;
                res2.MeasY = rr.MarkY;
                res2.ErrX = rr.ErrX;
                res2.ErrY = rr.ErrY;
                res2.MeasuredErrX = rr.ErrX;
                res2.MeasuredErrY = rr.ErrY;
                res2.HasMeasuredErr = rr.FindResult == 1;
                res2.Grade = rr.Grade ?? "";
                res2.FindResult = rr.FindResult;
                res2.AccumulateAppliedOffset(prevPoint, resetAppliedOffsets: false);
                _doc.Results[k2] = res2;
            }
        }
        private void RefreshReviewMap()
        {
            gridReview?.Invalidate();
            gridReview?.Refresh();
        }
        private string GradeByTol(double ex_px, double ey_px)
        {
            var tolx = _doc?.Crit?.TolX ?? 0;
            var toly = _doc?.Crit?.TolY ?? 0;
            bool ok = (Math.Abs(ex_px) <= tolx) && (Math.Abs(ey_px) <= toly);
            return ok ? "OK" : "NG";
        }
        private static bool IsIdleGradeString(string g)
        {
            return string.IsNullOrWhiteSpace(g)
                || g.Equals("NONE", StringComparison.OrdinalIgnoreCase)
                || g.Equals("IDLE", StringComparison.OrdinalIgnoreCase);
        }
        private static bool HasMeasuredTrace(ReviewRow rr)
        {
            if (rr == null) return false;
            return (rr.TargetX != 0) || (rr.TargetY != 0) ||
                   (rr.MarkX != 0) || (rr.MarkY != 0) ||
                   (rr.ErrX != 0) || (rr.ErrY != 0);
        }
        private static string GetDisplayGrade(ReviewRow rr)
        {
            if (rr == null) return "IDLE";

            // 측정 흔적이 있고 FindResult==0 인 경우는 NG
            if (IsIdleGradeString(rr.Grade))
            {
                if (rr.FindResult == 0 && HasMeasuredTrace(rr))
                    return "NG";
                return "IDLE";
            }
            return rr.Grade.ToUpperInvariant();
        }
        private void RecalcReviewGradesByCurrentTolerance_Once()
        {
            if (_doc == null) return;

            foreach (var rr in _review)
            {
                if (rr == null) continue;
                RecalcErrByFindResult(rr);
                if (rr.FindResult == 1) rr.Grade = GradeByTol(rr.ErrXpx, rr.ErrYpx);
                else rr.Grade = HasMeasuredTrace(rr) ? "NG" : "IDLE";
                if (HasMeasuredTrace(rr)) UpsertMeasResultToDoc(rr);
            }
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
        }
        public static (double ex, double ey, double exMm, double eyMm, string grade) Eval(double tx, double ty, double mx, double my, RecipeDoc doc)
        {
            double ex = ReviewErrSignX * (tx - mx);
            double ey = ReviewErrSignY * (ty - my);     // Err = Target - Mark
            var crit = doc?.Crit ?? new StageWin.Core.Recipe.Criteria();
            double exMm = ex * MmPerPx;
            double eyMm = ey * MmPerPx;
            bool ok = Math.Abs(ex) <= crit.TolX && Math.Abs(ey) <= crit.TolY;
            return (ex, ey, exMm, eyMm, ok ? "OK" : "NG");
        }
        private void UpdateDetailViewForCell(int row, int col, int preferVecR = 0, int preferVecC = 0)
        {
            if (gridReviewDetail == null) return;
            if (_doc == null) return;

            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, _doc.Parameters?.VectorCols ?? 1);

            // 범위 방어
            row = Math.Max(1, row);
            col = Math.Max(1, col);
            preferVecR = Math.Max(0, Math.Min(vecRows - 1, preferVecR));
            preferVecC = Math.Max(0, Math.Min(vecCols - 1, preferVecC));

            // 상세뷰는 "해당 StagePoint(row,col)의 vec 전체"를 보여주는 용도
            _reviewDetailView.RaiseListChangedEvents = false;
            try
            {
                _reviewDetailView.Clear();

                // 1) preferVec 우선, 그 다음 나머지 vec를 채우는 방식(사용자 UX가 좋음)
                //    - 먼저 prefer cell을 찾고
                //    - 전체를 VectorRow/VectorCol 순으로 넣되 prefer는 맨 위로 올림
                var all = _review
                    .Where(r => r.Row == row && r.Col == col)
                    .OrderBy(r => r.VectorRow)
                    .ThenBy(r => r.VectorCol)
                    .ToList();

                if (all.Count == 0)
                {
                    // 혹시 _review가 아직 구성 전이면 안전하게 종료
                    return;
                }

                // prefer를 맨 위로 올리고 싶으면 reorder
                var prefer = all.FirstOrDefault(r => r.VectorRow == preferVecR && r.VectorCol == preferVecC)
                          ?? all.FirstOrDefault(r => r.VectorRow == 0 && r.VectorCol == 0)
                          ?? all[0];

                // prefer 먼저 추가
                _reviewDetailView.Add(prefer);

                // 나머지 추가(중복 방지)
                foreach (var rr in all)
                {
                    if (ReferenceEquals(rr, prefer)) continue;
                    _reviewDetailView.Add(rr);
                }
            }
            finally
            {
                _reviewDetailView.RaiseListChangedEvents = true;
                _reviewDetailView.ResetBindings(); // BindingList 갱신
            }

            // 현재 선택 셀 기억 (Auto/Semi 연계에 중요)
            _lastSelectedCell = new Cell
            {
                Row = row,
                Col = col,
                VecR = preferVecR,
                VecC = preferVecC
            };

            // 상세 그리드에서 prefer row를 CurrentRow로 잡아줌
            try
            {
                if (gridReviewDetail.Rows.Count > 0)
                {
                    // prefer를 맨 위에 올렸으니 보통 0번이 prefer
                    int idx = 0;

                    // 혹시 prefer가 0번이 아닐 수 있는 경우를 대비해 탐색
                    for (int i = 0; i < gridReviewDetail.Rows.Count; i++)
                    {
                        if (gridReviewDetail.Rows[i].DataBoundItem is ReviewRow rr &&
                            rr.Row == row && rr.Col == col &&
                            rr.VectorRow == preferVecR && rr.VectorCol == preferVecC)
                        {
                            idx = i;
                            break;
                        }
                    }
                    gridReviewDetail.ClearSelection();
                    gridReviewDetail.CurrentCell = gridReviewDetail.Rows[idx].Cells[0];
                    gridReviewDetail.Rows[idx].Selected = true;
                    gridReviewDetail.FirstDisplayedScrollingRowIndex = Math.Max(0, idx);
                }
            }
            catch
            {
            }
            // 라벨/상단 표시 등 추가 UI가 있다면 여기서 갱신
            gridReviewDetail.Refresh();
        }
        private void InitAllReviewResultsToIdle(bool clearMeasuredXY = true)
        {
            EnsureRebuildIfNeeded(preserve: true, showProgress: false);
            _runningVecHL.Clear();
            _runningLineHL.Clear();

            foreach (var rr in _review)
            {
                rr.Grade = "IDLE";
                rr.FindResult = 0;

                if (clearMeasuredXY)
                {
                    rr.TargetX = 0;
                    rr.TargetY = 0;
                    rr.MarkX = 0;
                    rr.MarkY = 0;
                    rr.ErrX = 0;
                    rr.ErrY = 0;
                }
            }
            if (_doc != null)
            {
                _doc.Results?.Clear();
                _doc.ResultsVec?.Clear();

            }
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
        }

        private const double ReviewErrSignX = 1.0;
        private const double ReviewErrSignY = 1.0;
        private void RecalcErrByFindResult(ReviewRow rr)
        {
            if (rr == null) return;

            // Find 실패면 Err는 0으로 정리(판정은 Grade 로직에서 처리)
            if (rr.FindResult != 1)
            {
                rr.ErrX = 0.0;   // mm
                rr.ErrY = 0.0;   // mm
                return;
            }
            // Vision 측정 좌표는 px 기준(현재 구조상 Target/Mark가 px)
            double ex_px = ReviewErrSignX * (rr.TargetX - rr.MarkX);
            double ey_px = ReviewErrSignY * (rr.TargetY - rr.MarkY);
            // ErrXpx setter를 통해 내부(mm) 저장되도록 해야 함 (이중 변환 방지)
            rr.ErrXpx = ex_px;
            rr.ErrYpx = ey_px;
        }
        private void ForceSetIdleAndClearOnCurrent()
        {
            var c = this.SelectedCell;
            var rr = _review.FirstOrDefault(x => x.Row == c.Row && x.Col == c.Col &&
                x.VectorRow == c.VecR && x.VectorCol == c.VecC) ?? _review.FirstOrDefault(x =>
                    x.Row == c.Row && x.Col == c.Col && x.VectorRow == 0 && x.VectorCol == 0);
            if (rr == null) return;
            rr.Grade = "IDLE";
            rr.FindResult = 0;
            rr.TargetX = 0;
            rr.TargetY = 0;
            rr.MarkX = 0;
            rr.MarkY = 0;
            rr.ErrX = 0;   // mm
            rr.ErrY = 0;   // mm
            RestoreReviewSelection(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            UpdateDetailViewForCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
            gridReviewDetail?.Refresh();
            RefreshReviewMap();
            MarkNeedApplyReview();
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
        private void UpdateVisionFlyLinesMax(bool clampValue = true)
        {
            var doc = CurrentDoc;
            if (doc == null) return;

            SyncReviewFlyingParamsFromUi_NoRebuild();

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            var rf = doc.Parameters?.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());

            bool yMajor = rf.MajorIsY;
            bool forward = rf.Forward;
            bool snake = rf.Serpentine;

            int startRow = ResolveStartRowAuto(rf.StartRow, lines, rf.FlyDirX);
            int startCol = ResolveStartColAuto(rf.StartCol, holes, rf.FlyDirY);

            var sel = this.SelectedCell;
            int row = Math.Max(1, Math.Min(lines, sel.Row));
            int col = Math.Max(1, Math.Min(holes, sel.Col));

            bool isStepMode = (rbtnVisionStepbyStep?.Checked ?? false);
            bool stepAllHoles = (rbtnVisionStepAllHoles?.Checked ?? false);
            bool allWholeLines = (rbtnVisionStepAllWholeLine?.Checked ?? false);

            int prevStep = (int)(numVisionStepCount?.Value ?? 1);
            if (prevStep < 1) prevStep = 1;

            // forwardThis는 "비-Step(플라잉)"에서만 stepCount 최대치 계산에 필요 (기존 유지)
            bool forwardThis = true;
            if (!isStepMode)
            {
                int max = CalcMaxFlyVisionColumns();
                max = Math.Max(1, max);
                numFlyVisionLines.Enabled = true;
                UpdateNumericLimit(numFlyVisionLines, min: 1, max: max, clampValue: true);
                return;
            }

            // StepCount 최대치 산정
            int maxStep;
            if (isStepMode)
            {
                if (allWholeLines)
                {
                    maxStep = yMajor ? holes : lines;
                }
                else if (stepAllHoles)
                {
                    maxStep = yMajor ? holes : lines;
                }
                else
                {
                    if (yMajor)
                    {
                        int cnt = _review.Count(r => r.Row == row && r.Col >= col && r.VectorRow == 0 && r.VectorCol == 0);
                        maxStep = Math.Max(1, cnt);
                    }
                    else
                    {
                        int cnt = _review.Count(r => r.Col == col && r.Row >= row && r.VectorRow == 0 && r.VectorCol == 0);
                        maxStep = Math.Max(1, cnt);
                    }
                }
            }
            else
            {
                if (yMajor)
                {
                    int remainF = holes - col + 1;
                    int remainB = col;
                    maxStep = forwardThis ? remainF : remainB;
                }
                else
                {
                    int remainF = lines - row + 1;
                    int remainB = row;
                    maxStep = forwardThis ? remainF : remainB;
                }
                maxStep = Math.Max(1, maxStep);
            }

            maxStep = Math.Max(1, maxStep);
            UpdateNumericLimit(numVisionStepCount, 1, maxStep, clampValue: false);

            if (stepAllHoles || allWholeLines)
            {
                if (numVisionStepCount != null)
                {
                    numVisionStepCount.Value = maxStep;
                    numVisionStepCount.Enabled = false;
                }
            }
            else
            {
                if (numVisionStepCount != null)
                {
                    numVisionStepCount.Enabled = true;
                    if (prevStep < numVisionStepCount.Minimum || prevStep > numVisionStepCount.Maximum) numVisionStepCount.Value = numVisionStepCount.Maximum;
                    else numVisionStepCount.Value = prevStep;
                }
            }
            // numFlyVisionLines 최대치 산정
            if (numFlyVisionLines == null) return;

            if (isStepMode)
            {
                // Step 모드에서는 numFlyVisionLines로 "동작"하면 안 됨 ==> 1로 고정 (기존 유지)
                UpdateNumericLimit(numFlyVisionLines, 1, 1, clampValue: true);
                numFlyVisionLines.Value = 1;
                numFlyVisionLines.Enabled = false;
                return;
            }
            numFlyVisionLines.Enabled = true;

            int remain;
            if (yMajor)
            {
                // (row,col)부터 row-major로 남은 셀 수
                remain = (holes - col + 1) + (lines - row) * holes;
            }
            else
            {
                // xMajor: (col 고정에서 row 진행) 후 다음 col로
                remain = (lines - row + 1) + (holes - col) * lines;
            }
            remain = Math.Max(1, remain);
            UpdateNumericLimit(numFlyVisionLines, 1, remain, clampValue);
            if (numFlyVisionLines.Value < numFlyVisionLines.Minimum) numFlyVisionLines.Value = numFlyVisionLines.Minimum;
            else if (numFlyVisionLines.Value > numFlyVisionLines.Maximum) numFlyVisionLines.Value = numFlyVisionLines.Maximum;
        }
        private int CalcMaxFlyVisionColumns()
        {
            var doc = CurrentDoc;
            if (doc == null) return 1;

            var rf = doc.Parameters.ReviewFlying ?? (doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam());
            bool yMajor = rf.MajorIsY;

            int lines = Math.Max(1, doc.Parameters.Lines);
            int holes = Math.Max(1, doc.Parameters.HolesPerLine);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            var sel = this.SelectedCell;

            int totalLines = yMajor ? lines : holes;          // yMajor면 lineIndex1=Row, xMajor면 lineIndex1=Col
            int selLineIndex1 = yMajor ? sel.Row : sel.Col;   // 1-based
            selLineIndex1 = Math.Max(1, Math.Min(totalLines, selLineIndex1));

            int selVecC = Math.Max(0, Math.Min(vecCols - 1, sel.VecC));

            int remainInThisLine = vecCols - selVecC;                 // 선택 VecC 포함해서 남은 개수
            int remainLinesRight = totalLines - selLineIndex1;        // 오른쪽(다음) 라인 개수
            int max = remainInThisLine + remainLinesRight * vecCols;

            return Math.Max(1, max);
        }
        private void CalcGlobalRC(bool yMajor, int line1, int hole1, int vecR, int vecC, out int gR, out int gC)
        {
            // 주의:
            // - 여기서 "line1/hole1"은 항상 (Row,Col) 의미로 고정
            // - globalR/globalC는 2D 맵에서 표시하던 방식과 동일하게 유지하는 게 중요
            // - yMajor 여부는 nLine 결정에는 쓰지만, global index 자체는 보통 고정 정의로 두는 게 혼란이 적음

            var doc = CurrentDoc ?? throw new InvalidOperationException("레시피 미로딩");
            int vecRows = Math.Max(1, doc.Parameters.VectorRows);
            int vecCols = Math.Max(1, doc.Parameters.VectorCols);

            gR = (hole1 - 1) * vecRows + vecR;
            gC = (line1 - 1) * vecCols + vecC;
        }
        public static void CalcGlobalRC(RecipeDoc doc, bool yMajor, int line1, int hole1, int vecR, int vecC, out int gR, out int gC)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            int vecRows = Math.Max(1, doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, doc.Parameters?.VectorCols ?? 1);

            vecR = Math.Max(0, Math.Min(vecRows - 1, vecR));
            vecC = Math.Max(0, Math.Min(vecCols - 1, vecC));

            int majorIndex0 = (yMajor ? hole1 : line1) - 1;
            if (majorIndex0 < 0) majorIndex0 = 0;

            gR = majorIndex0 * vecRows + vecR;
            gC = vecC;
        }
        private void GridDesign_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var gv = gridDesign;
            var colName = gv.Columns[e.ColumnIndex].Name;
            // 기본 정렬: 가운데
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // Stage Point 컬럼은 따로 색 안 칠함
            if (colName == "colDStagePoint") return;
            // 기준점(각 StagePoint 의 R0C0, R0C{vecCols}, R0C{2*vecCols} …) 음영 처리
            if (_designVecRows > 0 && e.ColumnIndex >= 1)
            {
                int localVecRow = e.RowIndex % _designVecRows;               // 0..vecRows-1
                int colWithinLineGrp = (e.ColumnIndex - 1) % _designVecCols; // 0..vecCols-1

                bool isStageR = IsCenterIndex(localVecRow, _designVecRows);
                bool isStageC = IsCenterIndex(colWithinLineGrp, _designVecCols);

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
            else
            {
                e.CellStyle.BackColor = gv.DefaultCellStyle.BackColor;
                e.CellStyle.SelectionBackColor = gv.DefaultCellStyle.SelectionBackColor;
            }
        }
        private void GridScan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var gv = gridScan;
            if (gv == null) return;
            var colName = gv.Columns[e.ColumnIndex].Name;
            if (colName == "colSStagePoint" || colName.StartsWith("colSL"))
            {
                // 기본 가운데 정렬
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // StagePoint 컬럼은 색만 기본 유지
                if (colName == "colSStagePoint") return;

                // VecRow / VecCol 정보 기반으로 "기준 셀(R0C0)" 음영 처리
                if (_scanVecRows > 0 && _scanVecCols > 0 && e.ColumnIndex >= 1)
                {
                    int localVecRow = e.RowIndex % _scanVecRows;
                    int colWithinLineGroup = (e.ColumnIndex - 1) % _scanVecCols;

                    bool isStageR = IsCenterIndex(localVecRow, _scanVecRows);
                    bool isStageC = IsCenterIndex(colWithinLineGroup, _scanVecCols);

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
                return;
            }
        }
        private void GridReviewMap_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.ColumnIndex == 0) return;

            var gv = gridReview;
            if (gv == null) return;

            if (gv.Columns[e.ColumnIndex].Name == "colRStagePoint") return;
            if (_reviewVecRows <= 0 || _reviewVecCols <= 0 || e.ColumnIndex < 1) return;

            var cell = gv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell == null) return;

            // 기본 인덱스(태그 없어도 계산 가능)
            int hole1 = (e.RowIndex / _reviewVecRows) + 1;
            int line1 = ((e.ColumnIndex - 1) / _reviewVecCols) + 1;
            int vr = (e.RowIndex % _reviewVecRows);
            int vc = ((e.ColumnIndex - 1) % _reviewVecCols);

            double x = 0, y = 0;
            int globalR = (hole1 - 1) * _reviewVecRows + vr;

            // Tag가 있으면 Tag 값으로 덮어쓰기
            bool hasTag = false;
            if (cell.Tag is ReviewMapTag t)
            {
                hasTag = true;

                line1 = t.Line1;
                hole1 = t.Hole1;
                vr = t.Vr;
                vc = t.Vc;

                x = t.X;
                y = t.Y;
                globalR = t.GlobalR;
            }

            // ---- 값 표시 ----
            var rr = _review.FirstOrDefault(z => z.Row == line1 && z.Col == hole1 && z.VectorRow == vr && z.VectorCol == vc);
            string gradeShort = GetDisplayGrade(rr);

            e.Value =
                $"R{globalR}C{vc}{Environment.NewLine}" +
                $"{x:F3}{Environment.NewLine}" +
                $"{y:F3}{Environment.NewLine}" +
                $"{gradeShort}";

            if (IsBatchOffsetSource(rr))
            {
                e.CellStyle.BackColor = _clrBatchSourceBack;
                e.CellStyle.ForeColor = Color.Black;
                return;
            }

            if (IsBatchOffsetTarget(rr))
            {
                e.CellStyle.BackColor = _clrBatchTargetBack;
                e.CellStyle.ForeColor = Color.White;
                return;
            }

            if (hasTag)
            {
                var rf = _doc?.Parameters?.ReviewFlying;
                bool yMajor = rf?.MajorIsY ?? true;
                bool isRunningCell = _runningVecHL.Contains((line1, hole1, vr, vc));
                int lineKey = yMajor ? line1 : hole1;
                bool isRunningLine = _runningLineHL.Contains(lineKey);

                if (isRunningCell || isRunningLine)
                {
                    e.CellStyle.BackColor = _clrRunBack;
                    e.CellStyle.ForeColor = Color.Black;
                    return;
                }
            }
            if (gradeShort == "IDLE")
            {
                e.CellStyle.BackColor = _clrIdleBack;
                e.CellStyle.ForeColor = _clrIdleFore;
            }
            else if (gradeShort == "OK")
            {
                e.CellStyle.BackColor = _clrOkBack;
                e.CellStyle.ForeColor = _clrOkFore;
            }
            else
            {
                e.CellStyle.BackColor = _clrNgBack;
                e.CellStyle.ForeColor = _clrNgFore;
            }
        }
        private void GridReview_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rr = gridReviewDetail.Rows[e.RowIndex].DataBoundItem as ReviewRow;
            if (rr == null) return;
            var colName = gridReviewDetail.Columns[e.ColumnIndex].Name;
            string gradeShort = GetDisplayGrade(rr);
            if (IsBatchOffsetSource(rr))
            {
                if (colName == "colRGrade") e.Value = gradeShort;
                e.CellStyle.BackColor = _clrBatchSourceBack;
                e.CellStyle.ForeColor = Color.Black;
                return;
            }
            else if (IsBatchOffsetTarget(rr))
            {
                if (colName == "colRGrade") e.Value = gradeShort;
                e.CellStyle.BackColor = _clrBatchTargetBack;
                e.CellStyle.ForeColor = Color.White;
                return;
            }

            if (colName != "colRGrade") return;
            bool isIdle = (gradeShort == "IDLE");
            if (isIdle)
            {
                e.Value = "IDLE";
                e.CellStyle.BackColor = _clrIdleBack;
                e.CellStyle.ForeColor = _clrIdleFore;
            }
            else if (gradeShort == "OK")
            {
                e.Value = "OK";
                e.CellStyle.BackColor = _clrOkBack;
                e.CellStyle.ForeColor = _clrOkFore;
            }
            else
            {
                e.Value = "NG";
                e.CellStyle.BackColor = _clrNgBack;
                e.CellStyle.ForeColor = _clrNgFore;
            }
        }
        private void GridReviewDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!_batchOffsetApplyMode || e.RowIndex < 0) return;
            if (_batchOffsetSuppressNextClick)
            {
                _batchOffsetSuppressNextClick = false;
                return;
            }

            var rr = gridReviewDetail.Rows[e.RowIndex].DataBoundItem as ReviewRow;
            if (rr == null) return;
            ToggleBatchOffsetTarget(new Cell { Row = rr.Row, Col = rr.Col, VecR = rr.VectorRow, VecC = rr.VectorCol });
        }

        private void GridReviewDetail_CellMouseDownForBatchOffset(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!_batchOffsetApplyMode || e.Button != MouseButtons.Left || e.RowIndex < 0) return;
            var rr = gridReviewDetail.Rows[e.RowIndex].DataBoundItem as ReviewRow;
            if (rr == null) return;
            BeginBatchOffsetDrag(new Cell { Row = rr.Row, Col = rr.Col, VecR = rr.VectorRow, VecC = rr.VectorCol });
        }

        private void GridReviewDetail_CellMouseEnterForBatchOffset(object sender, DataGridViewCellEventArgs e)
        {
            if (!_batchOffsetApplyMode || !_batchOffsetDragActive || e.RowIndex < 0) return;
            if ((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left)
            {
                EndBatchOffsetDrag();
                return;
            }

            var rr = gridReviewDetail.Rows[e.RowIndex].DataBoundItem as ReviewRow;
            if (rr == null) return;
            ApplyBatchOffsetDragCell(new Cell { Row = rr.Row, Col = rr.Col, VecR = rr.VectorRow, VecC = rr.VectorCol });
        }

        private void GridReviewBatchOffset_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) EndBatchOffsetDrag();
        }

        private void GridReviewMap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;   // 0번 컬럼(Stage Point 라벨) 제외
            if (gridReview == null) return;
            int line1, hole1, vr, vc;
            var cell = gridReview[e.ColumnIndex, e.RowIndex];
            if (cell?.Tag is ReviewMapTag tag)
            {
                line1 = tag.Line1;
                hole1 = tag.Hole1;
                vr = tag.Vr;
                vc = tag.Vc;
            }
            else
            {
                if (_reviewVecRows <= 0 || _reviewVecCols <= 0) return;

                hole1 = (e.RowIndex / _reviewVecRows) + 1;
                line1 = ((e.ColumnIndex - 1) / _reviewVecCols) + 1;
                vr = (e.RowIndex % _reviewVecRows);
                vc = ((e.ColumnIndex - 1) % _reviewVecCols);
            }
            if (_batchOffsetApplyMode)
            {
                if (_batchOffsetSuppressNextClick)
                {
                    _batchOffsetSuppressNextClick = false;
                    return;
                }

                ToggleBatchOffsetTarget(new Cell { Row = line1, Col = hole1, VecR = vr, VecC = vc });
                return;
            }

            _lastSelectedCell = new Cell { Row = line1, Col = hole1, VecR = vr, VecC = vc };
            RestoreReviewSelection(line1, hole1, vr, vc);
            UpdateVisionFlyLinesMax();
        }

        private void GridReviewMap_CellMouseDownForBatchOffset(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!_batchOffsetApplyMode || e.Button != MouseButtons.Left || e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if (!TryGetReviewMapCell(e.RowIndex, e.ColumnIndex, out var cell)) return;
            BeginBatchOffsetDrag(cell);
        }

        private void GridReviewMap_CellMouseEnterForBatchOffset(object sender, DataGridViewCellEventArgs e)
        {
            if (!_batchOffsetApplyMode || !_batchOffsetDragActive || e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if ((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left)
            {
                EndBatchOffsetDrag();
                return;
            }
            if (!TryGetReviewMapCell(e.RowIndex, e.ColumnIndex, out var cell)) return;
            ApplyBatchOffsetDragCell(cell);
        }

        private bool TryGetReviewMapCell(int rowIndex, int columnIndex, out Cell result)
        {
            result = new Cell();
            if (gridReview == null || rowIndex < 0 || columnIndex < 1) return false;

            var gridCell = gridReview[columnIndex, rowIndex];
            if (gridCell?.Tag is ReviewMapTag tag)
            {
                result = new Cell { Row = tag.Line1, Col = tag.Hole1, VecR = tag.Vr, VecC = tag.Vc };
                return true;
            }

            if (_reviewVecRows <= 0 || _reviewVecCols <= 0) return false;

            result = new Cell
            {
                Row = ((columnIndex - 1) / _reviewVecCols) + 1,
                Col = (rowIndex / _reviewVecRows) + 1,
                VecR = rowIndex % _reviewVecRows,
                VecC = (columnIndex - 1) % _reviewVecCols
            };
            return true;
        }

        private void GridReviewMap_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;
            if (gridReview == null) return;

            int line1, hole1, vr, vc;
            var cell = gridReview[e.ColumnIndex, e.RowIndex];

            if (cell?.Tag is ReviewMapTag tag)
            {
                line1 = tag.Line1;
                hole1 = tag.Hole1;
                vr = tag.Vr;
                vc = tag.Vc;
            }
            else
            {
                // (CalcGlobalRC)에서는 column만으로 line/hole 역산 불가 → (gR,gC)로 역매칭
                bool yMajor = CurrentDoc?.Parameters?.ReviewFlying?.MajorIsY
                              ?? CurrentDoc?.Parameters?.ScanFlying?.MajorIsY
                              ?? true;
                int clickedGR = e.RowIndex;
                int clickedGC = e.ColumnIndex - 1;
                ReviewRow hit = null;
                foreach (var rr0 in _review)
                {
                    CalcGlobalRC(yMajor, rr0.Row, rr0.Col, rr0.VectorRow, rr0.VectorCol, out int gR, out int gC);
                    if (gR == clickedGR && gC == clickedGC) { hit = rr0; break; }
                }
                if (hit == null)
                {
                    MessageBox.Show(this, "해당 셀의 Review 데이터를 찾을 수 없습니다.", "Edit Review",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                line1 = hit.Row; hole1 = hit.Col; vr = hit.VectorRow; vc = hit.VectorCol;
            }
            _lastSelectedCell = new Cell { Row = line1, Col = hole1, VecR = vr, VecC = vc };
            RestoreReviewSelection(line1, hole1, vr, vc);
            UpdateVisionFlyLinesMax();
            var rr = _review.FirstOrDefault(z => z.Row == line1 && z.Col == hole1 && z.VectorRow == vr && z.VectorCol == vc);
            if (rr == null)
            {
                MessageBox.Show(this, "해당 셀의 Review 데이터를 찾을 수 없습니다.", "Edit Review",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ShowReviewEditDialog(rr))
            {
                MarkNeedApplyReview();
                RestoreReviewSelection(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
                UpdateDetailViewForCell(rr.Row, rr.Col, rr.VectorRow, rr.VectorCol);
                gridReviewDetail?.Refresh();
                RefreshReviewMap();
            }
        }
        private bool ShowReviewEditDialog(ReviewRow rr)
        {
            // 현재값 스냅샷 (Cancel 시 복원용)
            var prev = new ReviewState
            {
                TargetX = rr.TargetX,
                TargetY = rr.TargetY,
                MarkX = rr.MarkX,
                MarkY = rr.MarkY,
                ErrX = rr.ErrX,
                ErrY = rr.ErrY,
                FindResult = rr.FindResult,
                Grade = rr.Grade ?? "IDLE"
            };
            using (var dlg = new ReviewEditDialog(this, rr))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    rr.TargetX = prev.TargetX; rr.TargetY = prev.TargetY;
                    rr.MarkX = prev.MarkX; rr.MarkY = prev.MarkY;
                    rr.ErrX = prev.ErrX; rr.ErrY = prev.ErrY;
                    rr.FindResult = prev.FindResult;
                    rr.Grade = prev.Grade;
                    return false;
                }
                // 강제 Offset 모드면: dlg에서 Err/Find/Grade를 이미 강제 세팅했으므로
                if (dlg.ForceOffset)
                {
                    // FindResult=1, Grade=OK는 dlg에서 강제됨 (요구사항)
                    return true;
                }
                // 기존 동작(체크 해제) : Target/Mark/Find 기반 Err 계산 + Grade 처리
                RecalcErrByFindResult(rr);

                if (dlg.GradeMode == "AUTO")
                {
                    if (rr.FindResult == 1) rr.Grade = GradeByTol(rr.ErrXpx, rr.ErrYpx);
                    else rr.Grade = HasMeasuredTrace(rr) ? "NG" : "IDLE";
                }
                else
                {
                    rr.Grade = dlg.GradeMode; // "IDLE"/"OK"/"NG"
                }
                return true;
            }
        }
        private void UI(Action a)
        {
            if (IsDisposed) return;
            if (InvokeRequired) BeginInvoke(a);
            else a();
        }
        private void InvalidateReviewMapCell(ReviewRow rr)
        {
            if (rr == null || gridReview == null || _doc == null) return;

            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, _doc.Parameters?.VectorCols ?? 1);

            // gridReview 구조:
            // rowIndex = (hole1-1)*vecRows + vr
            // colIndex = 1 + (line1-1)*vecCols + vc   (0번 col은 StagePoint 라벨)
            int rowIndex = (rr.Col - 1) * vecRows + rr.VectorRow;
            int colIndex = 1 + (rr.Row - 1) * vecCols + rr.VectorCol;

            if (rowIndex < 0 || rowIndex >= gridReview.Rows.Count) return;
            if (colIndex < 1 || colIndex >= gridReview.Columns.Count) return;

            if (gridReview.InvokeRequired)
            {
                gridReview.BeginInvoke(new Action(() =>
                {
                    try { gridReview.InvalidateCell(colIndex, rowIndex); } catch { }
                }));
            }
            else
            {
                try { gridReview.InvalidateCell(colIndex, rowIndex); } catch { }
            }
        }

        #region ======= Review 측정 Text Log =======

        private string _reviewMeasureBatchId = "";
        private string _reviewMeasureBatchMethod = "";
        private string _reviewMeasureBatchOpTag = "";
        private int _reviewMeasureBatchDepth = 0;

        private sealed class ReviewMeasureScope : IDisposable
        {
            private Action _close;

            public ReviewMeasureScope(Action close)
            {
                _close = close;
            }

            public void Dispose()
            {
                var a = Interlocked.Exchange(ref _close, null);
                if (a != null) a();
            }
        }

        private IDisposable BeginReviewMeasureBatch(string methodName, string opTag, string note)
        {
            bool isRoot = string.IsNullOrWhiteSpace(_reviewMeasureBatchId);

            if (isRoot)
            {
                _reviewMeasureBatchId = ReviewMeasureTextLogger.MakeSessionId(opTag);
                _reviewMeasureBatchMethod = methodName ?? "";
                _reviewMeasureBatchOpTag = opTag ?? "";

                ReviewMeasureTextLogger.WriteSessionState(
                    _reviewMeasureBatchId,
                    GetCurrentModeNameSafe(),
                    GetCurrentRecipeNameSafe(),
                    _reviewMeasureBatchMethod,
                    _reviewMeasureBatchOpTag,
                    "BEGIN",
                    note);
            }

            _reviewMeasureBatchDepth++;

            return new ReviewMeasureScope(() =>
            {
                try
                {
                    _reviewMeasureBatchDepth--;

                    if (isRoot && _reviewMeasureBatchDepth <= 0)
                    {
                        ReviewMeasureTextLogger.WriteSessionState(
                            _reviewMeasureBatchId,
                            GetCurrentModeNameSafe(),
                            GetCurrentRecipeNameSafe(),
                            _reviewMeasureBatchMethod,
                            _reviewMeasureBatchOpTag,
                            "END",
                            note);

                        _reviewMeasureBatchId = "";
                        _reviewMeasureBatchMethod = "";
                        _reviewMeasureBatchOpTag = "";
                        _reviewMeasureBatchDepth = 0;
                    }
                }
                catch
                {
                    _reviewMeasureBatchId = "";
                    _reviewMeasureBatchMethod = "";
                    _reviewMeasureBatchOpTag = "";
                    _reviewMeasureBatchDepth = 0;
                }
            });
        }

        private string GetCurrentModeNameSafe()
        {
            try
            {
                if (ModeProvider != null)
                    return ModeProvider().ToString();

                return ProgramMode.Manual.ToString();
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetCurrentRecipeNameSafe()
        {
            try
            {
                string name = _doc?.Header?.Name ?? "";
                if (!string.IsNullOrWhiteSpace(name)) return name.Trim();

                name = CurrentDoc?.Header?.Name ?? "";
                return string.IsNullOrWhiteSpace(name) ? "NoRecipe" : name.Trim();
            }
            catch
            {
                return "NoRecipe";
            }
        }

        private string ResolveReviewMeasureMethodName(string opTag, string methodName)
        {
            if (!string.IsNullOrWhiteSpace(methodName))
                return methodName;

            string tag = (opTag ?? "").ToUpperInvariant();

            if (tag.Contains("3008") || tag.Contains("MANUAL_INSPECTION"))
                return "ManualInspectionFromVision";

            if (tag.Contains("3003") || tag.Contains("FLYING"))
                return "FlyingVision";

            if (tag.Contains("STEP_ALL_WHOLELINE"))
                return "StepByStepAllWholeLine";

            if (tag.Contains("STEP_LINE"))
                return "StepByStepLine";

            if (tag.Contains("3002"))
                return "SingleReviewMeasure";

            return "ReviewMeasure";
        }

        private void WriteReviewMeasureTextLog(
            ReviewRow rr,
            string opTag,
            string methodName,
            string note,
            string sessionIdOverride = null)
        {
            if (rr == null) return;

            try
            {
                var doc = CurrentDoc;
                bool yMajor = true;

                try
                {
                    yMajor = doc?.Parameters?.ReviewFlying?.MajorIsY ?? true;
                }
                catch
                {
                    yMajor = true;
                }

                int gR = 0;
                int gC = 0;

                try
                {
                    CalcGlobalRC(
                        yMajor: yMajor,
                        line1: rr.Row,
                        hole1: rr.Col,
                        vecR: rr.VectorRow,
                        vecC: rr.VectorCol,
                        gR: out gR,
                        gC: out gC);
                }
                catch
                {
                    gR = (rr.Col - 1) + rr.VectorRow;
                    gC = (rr.Row - 1) + rr.VectorCol;
                }

                string sessionId = sessionIdOverride;

                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    if (!string.IsNullOrWhiteSpace(_reviewMeasureBatchId))
                        sessionId = _reviewMeasureBatchId;
                    else
                        sessionId = ReviewMeasureTextLogger.MakeSessionId(opTag);
                }

                var rec = new ReviewMeasureTextLogger.ReviewMeasureRecord
                {
                    Time = DateTime.Now,

                    SessionId = sessionId,
                    ProgramMode = GetCurrentModeNameSafe(),
                    RecipeName = GetCurrentRecipeNameSafe(),
                    MethodName = ResolveReviewMeasureMethodName(opTag, methodName),
                    OpTag = opTag ?? "",

                    Row = rr.Row,
                    Col = rr.Col,
                    VectorRow = rr.VectorRow,
                    VectorCol = rr.VectorCol,

                    GlobalR = gR,
                    GlobalC = gC,

                    StageX = rr.StageX,
                    StageY = rr.StageY,

                    TargetX = rr.TargetX,
                    TargetY = rr.TargetY,
                    MarkX = rr.MarkX,
                    MarkY = rr.MarkY,

                    ErrXpx = rr.ErrXpx,
                    ErrYpx = rr.ErrYpx,
                    ErrXmm = rr.ErrX,
                    ErrYmm = rr.ErrY,

                    TolXpx = doc?.Crit?.TolX ?? 0.0,
                    TolYpx = doc?.Crit?.TolY ?? 0.0,

                    FindResult = rr.FindResult,
                    Grade = GetDisplayGrade(rr),
                    Note = note ?? ""
                };

                ReviewMeasureTextLogger.Write(rec);
            }
            catch
            {
                // Review 측정 로그 실패가 검사 동작을 막으면 안 되므로 무시
            }
        }
        #endregion

        #endregion

        #region ReviewEdit다이얼로그 박스
        // gridReview 셀(한 점)의 Target/Mark/Find/Grade를 수정하는 최소 팝업
        // - 저장은 하지 않음
        // - OK 누르면 rr에 값만 반영
        private sealed class ReviewEditDialog : Form
        {
            private readonly RecipeForm _owner;
            private readonly ReviewRow _rr;
            private CheckBox _chkForceOffset;
            private ComboBox _cmbUnit;
            private TextBox _txtErrX, _txtErrY;
            private TextBox _txtTgtX, _txtTgtY;
            private TextBox _txtMarkX, _txtMarkY;
            private NumericUpDown _numFind;
            private ComboBox _cmbGrade;
            private Label _lblErr;

            public string GradeMode { get; private set; } = "AUTO";  // "AUTO"/"IDLE"/"OK"/"NG"
            public bool ForceOffset => (_chkForceOffset?.Checked ?? false);
            private const double UmPerMm = 1000.0;

            public ReviewEditDialog(RecipeForm owner, ReviewRow rr)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _rr = rr ?? throw new ArgumentNullException(nameof(rr));
                bool yMajor = _owner.CurrentDoc?.Parameters?.ReviewFlying?.MajorIsY
                              ?? _owner.CurrentDoc?.Parameters?.ScanFlying?.MajorIsY
                              ?? true;
                _owner.CalcGlobalRC(yMajor, rr.Row, rr.Col, rr.VectorRow, rr.VectorCol, out var gR, out var gC);

                Text = $"Edit Review: Line#{rr.Row} Hole#{rr.Col}  (Map R{gR} C{gC})  V({rr.VectorRow},{rr.VectorCol})";
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;

                var tlp = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    ColumnCount = 2,
                    Padding = new Padding(12)
                };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
                _chkForceOffset = new CheckBox
                {
                    Text = "Force Offset (use ErrX/ErrY input)  →  Save as OK + Find=1",
                    AutoSize = true,
                    Checked = true // 기본 checked
                };
                tlp.Controls.Add(_chkForceOffset, 0, tlp.RowCount);
                tlp.SetColumnSpan(_chkForceOffset, 2);
                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp.RowCount++;
                _cmbUnit = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                _cmbUnit.Items.AddRange(new object[] { "mm", "um", "px" });
                _cmbUnit.SelectedIndex = 0; // 기본 mm
                _txtErrX = NewBox(0.0);
                _txtErrY = NewBox(0.0);
                AddRow(tlp, "Err Unit", _cmbUnit);
                AddRow(tlp, "Err X", _txtErrX);
                AddRow(tlp, "Err Y", _txtErrY);
                _txtTgtX = NewBox(rr.TargetX);
                _txtTgtY = NewBox(rr.TargetY);
                _txtMarkX = NewBox(rr.MarkX);
                _txtMarkY = NewBox(rr.MarkY);

                AddRow(tlp, "Target X (px)", _txtTgtX);
                AddRow(tlp, "Target Y (px)", _txtTgtY);
                AddRow(tlp, "Mark X (px)", _txtMarkX);
                AddRow(tlp, "Mark Y (px)", _txtMarkY);
                _numFind = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 1,
                    DecimalPlaces = 0,
                    Value = Math.Max(0, Math.Min(1, rr.FindResult)),
                    Width = 120
                };
                AddRow(tlp, "FindResult", _numFind);
                _cmbGrade = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 160
                };
                _cmbGrade.Items.AddRange(new object[] { "AUTO", "IDLE", "OK", "NG" });
                _cmbGrade.SelectedIndex = 0;
                AddRow(tlp, "Grade", _cmbGrade);
                _lblErr = new Label
                {
                    AutoSize = true,
                    Text = "Err(px / mm) = (0,0) / (0,0)"
                };
                tlp.Controls.Add(_lblErr, 0, tlp.RowCount);
                tlp.SetColumnSpan(_lblErr, 2);
                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp.RowCount++;
                var pnlBtn = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    Dock = DockStyle.Fill,
                    AutoSize = true
                };
                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
                pnlBtn.Controls.Add(btnOk);
                pnlBtn.Controls.Add(btnCancel);
                tlp.Controls.Add(pnlBtn, 0, tlp.RowCount);
                tlp.SetColumnSpan(pnlBtn, 2);
                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp.RowCount++;
                Controls.Add(tlp);
                AcceptButton = btnOk;
                CancelButton = btnCancel;
                SetErrBoxesFromMm(rr.ErrX, rr.ErrY, unit: "mm"); // 콤보 기본 mm라서 일단 mm로 표시
                _chkForceOffset.CheckedChanged += (s, e) =>
                {
                    ApplyEnableStates();
                    UpdatePreview();
                };
                _cmbUnit.SelectedIndexChanged += (s, e) =>
                {
                    // unit 변경 시: 현재 표시값을 mm로 해석해서 유지한 뒤 다시 표시 단위를 바꿔준다
                    if (TryGetErrMm(out double exMm, out double eyMm))
                        SetErrBoxesFromMm(exMm, eyMm, CurrentUnit());
                    UpdatePreview();
                };
                _txtErrX.TextChanged += (s, e) => UpdatePreview();
                _txtErrY.TextChanged += (s, e) => UpdatePreview();
                _txtTgtX.TextChanged += (s, e) => UpdatePreview();
                _txtTgtY.TextChanged += (s, e) => UpdatePreview();
                _txtMarkX.TextChanged += (s, e) => UpdatePreview();
                _txtMarkY.TextChanged += (s, e) => UpdatePreview();
                _numFind.ValueChanged += (s, e) => UpdatePreview();
                _cmbGrade.SelectedIndexChanged += (s, e) =>
                {
                    GradeMode = (_cmbGrade.SelectedItem?.ToString() ?? "AUTO").ToUpperInvariant();
                };
                btnOk.Click += (s, e) =>
                {
                    if (!ApplyToRowOrBlockClose())
                    {
                        this.DialogResult = DialogResult.None; // close 방지
                        return;
                    }
                };
                // 초기 상태 반영
                GradeMode = "AUTO";
                ApplyEnableStates();
                UpdatePreview();
            }
            private void ApplyEnableStates()
            {
                bool force = ForceOffset;
                // ForceOffset 체크 시: Err 입력 활성화
                _cmbUnit.Enabled = force;
                _txtErrX.Enabled = force;
                _txtErrY.Enabled = force;
                _txtTgtX.Enabled = !force;
                _txtTgtY.Enabled = !force;
                _txtMarkX.Enabled = !force;
                _txtMarkY.Enabled = !force;
                _numFind.Enabled = !force;
                _cmbGrade.Enabled = !force;
            }
            private bool ApplyToRowOrBlockClose()
            {
                bool force = ForceOffset;

                if (force)
                {
                    // Err 입력 파싱 + 단위 변환(mm 저장)
                    if (!TryGetErrMm(out double exMm, out double eyMm))
                    {
                        MessageBox.Show(this, "ErrX/ErrY 값이 숫자가 아닙니다.", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // 무조건 OK + Find=1 저장
                    _rr.ErrX = exMm;   // mm
                    _rr.ErrY = eyMm;   // mm
                    _rr.FindResult = 1;
                    _rr.Grade = "OK";
                    // Target/Mark는 “그대로 유지”(강제 Offset 목적이 offset 저장이므로)
                    return true;
                }
                else
                {
                    // 기존 동작: Target/Mark/Find/Grade 입력 허용
                    if (!TryParseBox(_txtTgtX, out double tx) ||
                        !TryParseBox(_txtTgtY, out double ty) ||
                        !TryParseBox(_txtMarkX, out double mx) ||
                        !TryParseBox(_txtMarkY, out double my))
                    {
                        MessageBox.Show(this, "Target/Mark 값이 숫자가 아닙니다.", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    _rr.TargetX = tx;
                    _rr.TargetY = ty;
                    _rr.MarkX = mx;
                    _rr.MarkY = my;
                    _rr.FindResult = (int)_numFind.Value;
                    GradeMode = (_cmbGrade.SelectedItem?.ToString() ?? "AUTO").ToUpperInvariant();
                    return true;
                }
            }
            private void UpdatePreview()
            {
                // Preview Err는 항상 "Err(px / mm) = (0,0) / (0,0)" 형태 유지
                double exPx = 0, eyPx = 0;
                double exMm = 0, eyMm = 0;

                if (ForceOffset)
                {
                    if (TryGetErrMm(out exMm, out eyMm))
                    {
                        exPx = exMm / MmPerPx;
                        eyPx = eyMm / MmPerPx;
                    }
                }
                else
                {
                    int find = (int)_numFind.Value;
                    if (find == 1)
                    {
                        if (TryParseBox(_txtTgtX, out double tx) &&
                            TryParseBox(_txtTgtY, out double ty) &&
                            TryParseBox(_txtMarkX, out double mx) &&
                            TryParseBox(_txtMarkY, out double my))
                        {
                            exPx = tx - mx;
                            eyPx = ty - my;
                            exMm = exPx * MmPerPx;
                            eyMm = eyPx * MmPerPx;
                        }
                    }
                }
                _lblErr.Text = $"Err(px / mm) = ({Fmt(exPx)},{Fmt(eyPx)}) / ({Fmt(exMm)},{Fmt(eyMm)})";
            }
            private static string Fmt(double v)
            {
                // (0,0) 느낌을 살리면서도 소수는 적당히
                // -0.000 같은 것도 정리
                if (Math.Abs(v) < 0.0000005) v = 0;
                return v.ToString("0.###", CultureInfo.InvariantCulture);
            }
            private string CurrentUnit() { return (_cmbUnit?.SelectedItem?.ToString() ?? "mm").ToLowerInvariant(); }
            private void SetErrBoxesFromMm(double exMm, double eyMm, string unit)
            {
                double ux = MmToUnit(exMm, unit);
                double uy = MmToUnit(eyMm, unit);
                _txtErrX.Text = ux.ToString("0.###", CultureInfo.InvariantCulture);
                _txtErrY.Text = uy.ToString("0.###", CultureInfo.InvariantCulture);
            }
            private bool TryGetErrMm(out double exMm, out double eyMm)
            {
                exMm = eyMm = 0.0;
                if (!TryParseBox(_txtErrX, out double ux)) return false;
                if (!TryParseBox(_txtErrY, out double uy)) return false;

                string u = CurrentUnit();
                exMm = UnitToMm(ux, u);
                eyMm = UnitToMm(uy, u);
                return true;
            }
            private static double UnitToMm(double v, string unit)
            {
                switch ((unit ?? "mm").ToLowerInvariant())
                {
                    case "mm": return v;
                    case "um": return v / UmPerMm;
                    case "px": return v * MmPerPx;
                    default: return v;
                }
            }
            private static double MmToUnit(double mm, string unit)
            {
                switch ((unit ?? "mm").ToLowerInvariant())
                {
                    case "mm": return mm;
                    case "um": return mm * UmPerMm;
                    case "px": return mm / MmPerPx;
                    default: return mm;
                }
            }
            private static void AddRow(TableLayoutPanel tlp, string label, Control editor)
            {
                var lbl = new Label { Text = label, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 6, 8, 0) };

                tlp.Controls.Add(lbl, 0, tlp.RowCount);
                tlp.Controls.Add(editor, 1, tlp.RowCount);

                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp.RowCount++;
            }
            private static TextBox NewBox(double v)
            {
                return new TextBox
                {
                    Width = 160,
                    Text = v.ToString("0.###", CultureInfo.InvariantCulture),
                    TextAlign = HorizontalAlignment.Right
                };
            }
            private static bool TryParseBox(TextBox tb, out double v)
            {
                v = 0.0;
                if (tb == null) return false;

                var s = (tb.Text ?? "").Trim();
                if (string.IsNullOrEmpty(s))
                {
                    v = 0.0;
                    return true;
                }
                // InvariantCulture 기준 (점 '.' 입력 기준)
                return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
            }
        }
        #endregion
    }
}
