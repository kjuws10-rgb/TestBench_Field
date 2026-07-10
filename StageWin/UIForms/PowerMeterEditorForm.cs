using Core.Config;
using NetCommon;
using StageWin.Core.Recipe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageWin.UI
{
    public sealed partial class PowerMeterEditorForm : Form
    {
        public enum PMInterface { Meas1_1010, Meas2_1011 }
        public Func<double, double, CancellationToken, Task> RequestMoveToPowerStartAsync { get; set; }
        public Func<NetCommon.ST_POWER_TABLE[], Task<NetCommon.ST_POWER_MEAS1_RSP>> RequestMeas1Async { get; set; }
        public Func<int, double, double, double, Task<NetCommon.ST_POWER_MEAS2_RSP>> RequestMeas2Async { get; set; }
        public event Action<double, double> RequestSetLaserParams;

        // 레시피 리스트/스토어 
        private bool _suppressListEvent;
        private double _initPowerW;
        private double _initAtt;

        private PowerRecipeStore _pmStore;
        private PowerTablePerFreq _pendingTable;      // Make Table(빌드/측정)으로 만든 아직 저장 전 작업본
        private string _pmCurrentName;
        private PowerOnlyDoc _pmCurrent;

        private string _lastSavedName;
        public string SelectedPMRecipeName { get; private set; }  // 폼 닫힌 뒤 부모가 읽을 수 있게

        public Func<int, Task> RequestAbortScanAsync { get; set; }   // Stage(Form1)에서 ScanStop(1006) 연결
        public Action<string> RequestLog { get; set; }               // Stage 로그로 연결

        private CancellationTokenSource _measCts;
        private int _measSeq = 0;
        private volatile bool _measuring = false;

        // 표시용 데이터 모델(기존) 
        private sealed class PowerRow
        {
            public int Index { get; set; }            // #
            public double Frequency { get; set; }     // kHz
            public double TargetW { get; set; }       // Target Power(W)
            public double ResultAttPos { get; set; }  // 결과 Attenuator Position
            public string RecipeName { get; set; }
        }
        private readonly BindingList<PowerRow> _powerRows = new BindingList<PowerRow>();

        private RecipeDoc _doc;

        public PowerMeterEditorForm()
        {
            InitializeComponent();
            Text = "Powermeter Editor";

            _pmStore = PowerRecipeStore.Open(AppConfig.Current.PowerRecipesPath);

            // 중복 연결 방지: 먼저 해제, 그 다음 연결
            if (lstPowerRecipeList != null)
            {
                lstPowerRecipeList.SelectedIndexChanged -= lstPowerRecipeList_SelectedIndexChanged;
                lstPowerRecipeList.SelectedIndexChanged += lstPowerRecipeList_SelectedIndexChanged;
                RefreshRecipeList();
            }

            _gridPower.AutoGenerateColumns = false;
            _gridPower.AllowUserToAddRows = false;
            _gridPower.AllowUserToDeleteRows = false;
            _gridPower.AllowUserToResizeColumns = false;
            _gridPower.AllowUserToResizeRows = false;
            _gridPower.RowHeadersVisible = false;
            _gridPower.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            _gridPower.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2; // 자연스러운 편집
            _gridPower.CellValidating -= _gridPower_CellValidating;
            _gridPower.CellValidating += _gridPower_CellValidating;
            _gridPower.CellEndEdit -= _gridPower_CellEndEdit;
            _gridPower.CellEndEdit += _gridPower_CellEndEdit;
            _gridPower.DataError += (s, e) => { e.ThrowException = false; }; // 숫자 포맷 오류 등 무시(메시지는 직접 띄움)
            _gridPower.CellFormatting += (s, e) =>
            {
                var grid = (DataGridView)s;
                var prop = grid.Columns[e.ColumnIndex].DataPropertyName;
                if (string.Equals(prop, "ResultAttPos", StringComparison.OrdinalIgnoreCase) && e.Value is double d && double.IsNaN(d))
                {
                    e.Value = "";      // NaN → 빈칸
                    e.FormattingApplied = true;
                }
            };

            EnsureGridColumns();
            _gridPower.DataSource = _powerRows;

            if (btnPT_Build != null) { btnPT_Build.Click -= btnPT_Build_Click; btnPT_Build.Click += btnPT_Build_Click; }
            if (btnPT_Measure != null) { btnPT_Measure.Click -= btnPT_Measure_Click; btnPT_Measure.Click += btnPT_Measure_Click; }
            if (btnAdd != null) { btnAdd.Click -= btnAdd_Click; btnAdd.Click += btnAdd_Click; }
            if (btnDelete != null) { btnDelete.Click -= btnDelete_Click; btnDelete.Click += btnDelete_Click; }
            if (btnOk != null) { btnOk.Click -= btnOk_Click; btnOk.Click += btnOk_Click; }
            if (btnCancel != null) { btnCancel.Click -= btnCancel_Click; btnCancel.Click += btnCancel_Click; }
            EnsureAbortButton();
        }
        public void SetInitial(double powerW, double atten)
        {
            _initPowerW = powerW;
            _initAtt = atten;

            // 컨트롤이 존재하면 대략적인 초기 표시만 해줍니다. (없으면 조용히 패스)
            try
            {
                // Min/Max W가 비어있으면 현재 Power 주변으로 2W 범위 제안
                if (numMinW_1 != null && numMaxW_1 != null)
                {
                    if ((double)numMinW_1.Value == 0 && (double)numMaxW_1.Value == 0 && powerW > 0)
                    {
                        SafeSet(numMinW_1, Math.Max(0, powerW - 1));
                        SafeSet(numMaxW_1, powerW + 1);
                    }
                }
                // Att 범위도 현재 atten 주변으로 간단히 제안
                if (numAttMin_1 != null && numAttMax_1 != null && atten > 0)
                {
                    if ((double)numAttMin_1.Value == 0 && (double)numAttMax_1.Value == 0)
                    {
                        SafeSet(numAttMin_1, Math.Max((double)numAttMin_1.Minimum, atten - 5));
                        SafeSet(numAttMax_1, Math.Min((double)numAttMax_1.Maximum, atten + 5));
                    }
                }
            }
            catch { /* 표시 실패해도 무시 */ }
        }
        public PowerMeterEditorForm(RecipeDoc doc) : this()
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            if (grpPT != null) grpPT.Enabled = true;
        }
        public PMInterface SelectedInterface
        {
            get
            {
                var radio = FindControlRecursive<RadioButton>("rbtnIf1");
                var txt = (radio?.Checked ?? false) ? radio?.Text : string.Empty;
                if (!string.IsNullOrEmpty(txt) && (txt.Contains("#1")))
                    return PMInterface.Meas1_1010;
                return PMInterface.Meas2_1011;
            }
        }
        public int GetScanNoOr(int fallback)
        {
            // 기존("numScanNo"/"txtScanNo") + PT 화면 컨트롤("numPT_ScanNo"/"txtPT_ScanNo") 모두 지원
            var nud = FindControlRecursive<NumericUpDown>("numScanNo") ?? FindControlRecursive<NumericUpDown>("numPT_ScanNo");
            if (nud != null) return (int)nud.Value;

            var tb = FindControlRecursive<TextBox>("txtScanNo") ?? FindControlRecursive<TextBox>("txtPT_ScanNo");
            if (tb != null && int.TryParse(tb.Text, out var v)) return v;

            return fallback;
        }
        // range 0으로 전달되는 문제 발생 -PGT
        public double GetRangePercentOr(double fallback)
        {
            var nud = FindControlRecursive<NumericUpDown>("numRangePct_1");
            if (nud != null) return (double)nud.Value;

            //var tb = FindControlRecursive<TextBox>("txtRangePct_1");
            //if (tb != null && double.TryParse(tb.Text, out var d)) return d;

            return fallback;
        }
        private PowerTablePerFreq CaptureTableSnapshotFromGrid()
        {
            if (_gridPower == null || _gridPower.Rows.Count == 0)
                return null;

            // freq는 그리드 첫 행의 Frequency 우선
            double freq = 0;
            for (int i = 0; i < _gridPower.Rows.Count; i++)
            {
                if (_gridPower.Rows[i].DataBoundItem is PowerRow pr && pr.Frequency > 0)
                {
                    freq = pr.Frequency;
                    break;
                }
            }
            if (freq <= 0) freq = _pendingTable?.Frequency ?? (double)numFreq_1.Value;

            double attMin = (double)numAttMin_1.Value;
            double attMax = (double)numAttMax_1.Value;
            var rows = new List<PowerTableRow>();
            foreach (DataGridViewRow gr in _gridPower.Rows)
            {
                if (gr?.DataBoundItem is PowerRow pr)
                {
                    rows.Add(new PowerTableRow
                    {
                        TargetW = Math.Round(pr.TargetW, 3),
                        AttPos = pr.ResultAttPos,
                        AttMin = attMin,
                        AttMax = attMax
                    });
                }
            }

            double minW = rows.Count > 0 ? rows.Min(r => r.TargetW) : 0.0;
            double maxW = rows.Count > 0 ? rows.Max(r => r.TargetW) : 0.0;

            // step은 UI값 우선(없으면 추정)
            double stepW = (numStepW_1 != null) ? (double)numStepW_1.Value : 0.0;
            if (stepW <= 0 && rows.Count >= 2)
            {
                var ordered = rows.OrderBy(r => r.TargetW).ToList();
                stepW = Math.Abs(ordered[1].TargetW - ordered[0].TargetW);
            }

            return new PowerTablePerFreq
            {
                Frequency = freq,
                MinW = minW,
                MaxW = maxW,
                StepW = stepW,
                Rows = rows,
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
        // Grid 컬럼(요구사항: 4개만) 
        private void EnsureGridColumns()
        {
            _gridPower.Columns.Clear();
            _gridPower.Columns.Add(MakeNumCol("#", "Index", 50, 0, readOnly: true, center: true));
            _gridPower.Columns.Add(MakeNumCol("Frequency", "Frequency", 110, 2, readOnly: true));
            _gridPower.Columns.Add(MakeNumCol("Target Power(W)", "TargetW", 140, 3, readOnly: true));
            _gridPower.Columns.Add(MakeNumCol("Result Attenuator Position", "ResultAttPos", 200, 3, readOnly: false));
        }
        private DataGridViewTextBoxColumn MakeNumCol(string header, string prop, int width, int dp, bool readOnly = false, bool center = false)
        {
            var col = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.False
            };
            col.DefaultCellStyle.Format = "N" + Math.Max(0, dp);
            col.DefaultCellStyle.Alignment = center ? DataGridViewContentAlignment.MiddleCenter : DataGridViewContentAlignment.MiddleRight;
            return col;
        }

        // 레시피 리스트 갱신
        private void RefreshRecipeList()
        {
            if (lstPowerRecipeList == null) return;
            var names = _pmStore?.ListNames() ?? new List<string>();

            _suppressListEvent = true;
            try
            {
                lstPowerRecipeList.BeginUpdate();
                lstPowerRecipeList.Items.Clear();
                foreach (var n in names) lstPowerRecipeList.Items.Add(n);
                lstPowerRecipeList.EndUpdate();

                if (lstPowerRecipeList.Items.Count > 0)
                    lstPowerRecipeList.SelectedIndex = 0;
            }
            finally { _suppressListEvent = false; }
        }
        private void _gridPower_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var grid = (DataGridView)sender;
            var prop = grid.Columns[e.ColumnIndex].DataPropertyName;

            if (!string.Equals(prop, "ResultAttPos", StringComparison.OrdinalIgnoreCase))
                return; // 편집 허용 컬럼만 검증

            var txt = (e.FormattedValue?.ToString() ?? "").Trim();

            // 빈 값 → NaN 허용 (지울 수 있도록)
            if (string.IsNullOrEmpty(txt)) return;

            if (!double.TryParse(txt, out var v))
            {
                e.Cancel = true;
                MessageBox.Show(this, "숫자를 입력하세요. (빈 값은 허용)", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // (선택) 범위 검증: 테이블의 AttMin/AttMax 참고
            try
            {
                var min = _pendingTable?.Rows?.FirstOrDefault()?.AttMin ?? double.NaN;
                var max = _pendingTable?.Rows?.FirstOrDefault()?.AttMax ?? double.NaN;
                if (!double.IsNaN(min) && !double.IsNaN(max) && (v < min || v > max))
                {
                    var r = MessageBox.Show(this,
                        $"권장 범위 [{min:F3} ~ {max:F3}] 를 벗어났습니다. 그대로 입력할까요?",
                        "범위 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                    if (r != DialogResult.Yes) e.Cancel = true;
                }
            }
            catch { /* 범위 정보 없으면 패스 */ }
        }

        private void _gridPower_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var prop = grid.Columns[e.ColumnIndex].DataPropertyName;
            if (!string.Equals(prop, "ResultAttPos", StringComparison.OrdinalIgnoreCase))
                return;

            var rowObj = grid.Rows[e.RowIndex].DataBoundItem as PowerRow;
            if (rowObj == null) return;

            var cellVal = (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ?? "").ToString().Trim();

            // 빈 값 → NaN 저장
            double val = double.NaN;
            if (!string.IsNullOrEmpty(cellVal))
                double.TryParse(cellVal, out val);

            // 1) 표시용 모델에 반영
            rowObj.ResultAttPos = val;

            // 2) 작업본(_pendingTable)에도 반영 (인덱스로 매핑)
            if (_pendingTable?.Rows != null && e.RowIndex >= 0 && e.RowIndex < _pendingTable.Rows.Count)
            {
                _pendingTable.Rows[e.RowIndex].AttPos = val;
                _pendingTable.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // 상태 표시
            if (lblStatus != null)
                lblStatus.Text = "[Edit] Result Att pos 수정됨 (아직 저장 전: OK 누르면 저장)";
        }

        // 리스트 선택 시 테이블/컨트롤 동기화
        private void lstPowerRecipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressListEvent) return;
            var name = lstPowerRecipeList?.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                _pmCurrentName = name;
                _pmCurrent = _pmStore.Load(name);  // 파일 문제 등으로 null일 수 있음
                if (_pmCurrent == null)
                {
                    _powerRows.Clear();
                    _gridPower.Refresh();
                    _pendingTable = null;
                    tbRecipeName.Text = _pmCurrentName;
                    lblStatus.Text = $"'{name}' 문서를 불러오지 못했습니다.";
                    return;
                }

                LoadRecipeToView(_pmCurrent);
                _pendingTable = PickPowerTable(_pmCurrent);
                tbRecipeName.Text = _pmCurrentName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"파워 레시피 로드 실패: {ex.Message}",
                    "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 현재 문서에서 표시(작업)할 테이블 하나 고르기
        private PowerTablePerFreq PickPowerTable(PowerOnlyDoc doc)
        {
            if (doc == null) return null;

            // 1순위: 공정 레시피(_doc)의 DefaultTooling.Freq
            double wantFreq = (_doc?.DefaultTooling?.Freq > 0) ? _doc.DefaultTooling.Freq : 0.0;

            // PowerOnlyDoc에 저장된 테이블 중 가장 가까운 것 선택
            return doc.PickTable(wantFreq);
        }

        // 선택된 레시피를 Grid/컨트롤에 반영
        private void LoadRecipeToView(PowerOnlyDoc pm)
        {
            if (pm == null) throw new ArgumentNullException(nameof(pm));
            if (grpPT != null) grpPT.Enabled = true;

            var table = PickPowerTable(pm);

            _powerRows.Clear();
            if (table?.Rows != null && table.Rows.Count > 0)
            {
                int idx = 1;
                foreach (var r in table.Rows.OrderBy(r => r.TargetW))
                {
                    _powerRows.Add(new PowerRow
                    {
                        Index = idx++,
                        Frequency = table.Frequency,
                        TargetW = Math.Round(r.TargetW, 3),
                        ResultAttPos = r.AttPos
                    });
                }
                _gridPower.Refresh();
            }

            PullTableToControls(table);
            if (lblStatus != null)
                lblStatus.Text = (table == null)
                    ? $"No power table in '{_pmCurrentName}'."
                    : $"Loaded '{_pmCurrentName}' @ {table.Frequency} ({table.Rows?.Count ?? 0} pts)";
        }

        // 테이블 → 컨트롤 동기화 
        private void PullTableToControls(PowerTablePerFreq table)
        {
            if (table == null)
            {
                SafeSet(numMinW_1, 0);
                SafeSet(numMaxW_1, 0);
                SafeSet(numStepW_1, 0);
                SafeSet(numAttMin_1, 0);
                SafeSet(numAttMax_1, 0);

                SafeSetOptional("numPT_ScanNo", "txtPT_ScanNo", 1);
                SafeSetOptional("numPT_Range", "txtPT_Range", 0);

                // 시작 위치도 '값이 없으면 표시하지 않음'
                SafeSetIfValid(numPowerMeasureX, double.NaN);
                SafeSetIfValid(numPowerMeasureY, double.NaN);

                if (lblStatus != null)
                    lblStatus.Text = $"No power table in '{_pmCurrentName}'.";
                return;
            }

            SafeSet(numMinW_1, table.MinW);
            SafeSet(numMaxW_1, table.MaxW);
            SafeSet(numStepW_1, table.StepW);

            double attMin = table.Rows?.Count > 0 ? table.Rows[0].AttMin : 0.0;
            double attMax = table.Rows?.Count > 0 ? table.Rows[0].AttMax : 0.0;
            SafeSet(numAttMin_1, attMin);
            SafeSet(numAttMax_1, attMax);

            // 스캐너 1대 고정
            SafeSetOptional("numPT_ScanNo", "txtPT_ScanNo", 1);

            double range = table.MaxW - table.MinW;
            SafeSetOptional("numPT_Range", "txtPT_Range", range);

            // 여기서 _pmCurrent가 null일 수 있음 → 안전한 fallback 사용
            double sx = _pmCurrent?.PowerMeterStartX ?? _doc?.PowerMeterStartX ?? double.NaN;
            double sy = _pmCurrent?.PowerMeterStartY ?? _doc?.PowerMeterStartY ?? double.NaN;
            SafeSetIfValid(numPowerMeasureX, sx);
            SafeSetIfValid(numPowerMeasureY, sy);

            if (lblStatus != null)
                lblStatus.Text = $"Loaded '{_pmCurrentName}' @ {table.Frequency} ({table.Rows?.Count ?? 0} pts)";
        }


        private PowerTablePerFreq BuildTableFromUi(double freq)
        {
            double minW = (double)numMinW_1.Value;
            double maxW = (double)numMaxW_1.Value;
            double stepW = (double)numStepW_1.Value;
            double attMin = (double)numAttMin_1.Value;
            double attMax = (double)numAttMax_1.Value;

            var rows = new List<PowerTableRow>();
            for (double w = minW; w <= maxW + 1e-9; w += Math.Max(1e-9, stepW))
            {
                rows.Add(new PowerTableRow
                {
                    TargetW = Math.Round(w, 3),
                    AttPos = double.NaN,
                    AttMin = attMin,
                    AttMax = attMax
                });
            }

            return new PowerTablePerFreq
            {
                Frequency = freq,
                MinW = minW,
                MaxW = maxW,
                StepW = stepW,
                Rows = rows,
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        //  Helpers: 컨트롤 안전 세팅 
        private static decimal ClampToRange(NumericUpDown nud, double val)
        {
            var v = (decimal)val;
            if (v < nud.Minimum) v = nud.Minimum;
            if (v > nud.Maximum) v = nud.Maximum;
            return v;
        }
        private static void SafeSet(NumericUpDown nud, double val)
        {
            if (nud == null) return;
            nud.Value = ClampToRange(nud, val);
        }
        private static void SafeSetIfValid(NumericUpDown nud, double val)
        {
            if (nud == null) return;
            if (double.IsNaN(val) || double.IsInfinity(val)) return; // 값이 없으면 건너뜀
            SafeSet(nud, val);
        }
        private bool CommitPendingToCurrentRecipe(out string savedName)
        {
            savedName = null;
            var tableToSave = CaptureTableSnapshotFromGrid();
            if (tableToSave == null) return false;

            var name = _pmCurrentName;
            if (string.IsNullOrWhiteSpace(name)) name = (tbRecipeName?.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "레시피 이름이 없습니다. 좌측 목록에서 선택하거나 이름을 입력하세요.",
                                "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var doc = _pmStore.Exists(name) ? _pmStore.Load(name) : (_pmCurrent ?? _pmStore.New());

            doc.SavePowerTable(tableToSave);

            var (sx, sy) = ReadStartXYFromUI();
            if (!double.IsNaN(sx)) doc.PowerMeterStartX = sx;
            if (!double.IsNaN(sy)) doc.PowerMeterStartY = sy;

            _pmStore.Save(name, doc);

            // 상태 갱신
            _pmCurrent = doc;
            _pmCurrentName = name;
            _lastSavedName = name;
            savedName = name;

            // 저장 기준 테이블도 현재 스냅샷으로 갱신
            _pendingTable = tableToSave;

            LogPm($"[PM][SAVE] '{name}' saved. freq={tableToSave.Frequency}, pts={tableToSave.Rows?.Count ?? 0}");
            return true;
        }
        private async void BtnAbort_Click(object sender, EventArgs e)
        {
            if (!_measuring) return;

            try
            {
                _measCts?.Cancel();

                int scanNo = GetScanNoOr(1);
                LogPm($"[PM][ABORT] user abort requested. scanNo={scanNo}");

                if (RequestAbortScanAsync != null)
                {
                    try { await RequestAbortScanAsync(scanNo); }
                    catch (Exception ex) { LogPm($"[PM][ABORT][W] ScanStop failed: {ex.Message}"); }
                }

                if (lblStatus != null) lblStatus.Text = "[Abort] 측정 중지 요청됨";
            }
            catch { }
        }
        private async Task<T> AwaitWithCancel<T>(Task<T> task, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (ct.Register(() => tcs.TrySetResult(true)))
            {
                var completed = await Task.WhenAny(task, tcs.Task);
                ct.ThrowIfCancellationRequested();
                return await task; // 예외/결과 전달
            }
        }
        private void SafeSetOptional(string nudName, string altTextName, double val)
        {
            // NumericUpDown 우선, 없으면 TextBox/Label 순서로 찾아서 표시
            var nud = FindControlRecursive<NumericUpDown>(nudName);
            if (nud != null) { SafeSet(nud, val); return; }

            var tb = FindControlRecursive<TextBox>(altTextName);
            if (tb != null) { tb.Text = val.ToString("G"); return; }

            var lb = FindControlRecursive<Label>(altTextName);
            if (lb != null) { lb.Text = val.ToString("G"); }
        }
        private T FindControlRecursive<T>(string name) where T : Control
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var arr = this.Controls.Find(name, true);
            return (arr != null && arr.Length > 0) ? arr[0] as T : null;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                // 측정 결과가(_pendingTable) 있으면 커밋(덮어쓰기 저장)
                if (CommitPendingToCurrentRecipe(out var saved))
                {
                    SelectedPMRecipeName = saved;
                    if (lblStatus != null)
                        lblStatus.Text = $"[OK] 저장 완료: '{saved}' ({_pendingTable.Rows?.Count ?? 0} pts)";
                }
                else
                {
                    // 커밋할 게 없으면 마지막 저장명 또는 현재 선택명만 반환
                    SelectedPMRecipeName = !string.IsNullOrWhiteSpace(_lastSavedName)
                        ? _lastSavedName
                        : (lstPowerRecipeList?.SelectedItem as string);
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "저장 실패: " + ex.Message,
                                "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 저장 실패 시 닫지 않음
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 저장 없이 종료 (SelectedPMRecipeName 설정하지 않음)
            DialogResult = DialogResult.Cancel;
        }
        private void EnsureAbortButton()
        {
            // 디자이너에 btnAbort가 있으면 그걸 쓰고, 없으면 런타임 생성
            btnAbort = FindControlRecursive<Button>("btnAbort");
            if (btnAbort == null)
            {
                btnAbort = new Button
                {
                    Name = "btnAbort",
                    Text = "Abort",
                    Width = 90,
                    Height = 28,
                    Enabled = false
                };
                // 폼 우하단 근처에 대충 배치(원하면 디자이너로 옮겨도 됨)
                btnAbort.Left = this.ClientSize.Width - btnAbort.Width - 12;
                btnAbort.Top = this.ClientSize.Height - btnAbort.Height - 12;
                btnAbort.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                this.Controls.Add(btnAbort);
            }

            btnAbort.Click -= BtnAbort_Click;
            btnAbort.Click += BtnAbort_Click;
        }
        private string GetCurrentPmRecipeNameSafe()
        {
            // 실제로 측정에 쓰였다고 볼 수 있는 우선순위
            var name =
                (_pmCurrentName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                name = (lstPowerRecipeList?.SelectedItem as string ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                name = (tbRecipeName?.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                name = (SelectedPMRecipeName ?? "").Trim();

            return string.IsNullOrWhiteSpace(name) ? "(None)" : name;
        }
        private void SetMeasuringUi(bool measuring, string status = null)
        {
            _measuring = measuring;
            if (btnPT_Measure != null) btnPT_Measure.Enabled = !measuring;
            if (btnPT_Build != null) btnPT_Build.Enabled = !measuring;
            if (btnAdd != null) btnAdd.Enabled = !measuring;
            if (btnDelete != null) btnDelete.Enabled = !measuring;
            if (btnOk != null) btnOk.Enabled = !measuring;          // 측정 중 OK 저장 방지(원하면 true로)
            if (btnCancel != null) btnCancel.Enabled = !measuring;  // 측정 중 닫기 방지(Abort로만 종료)
            if (btnAbort != null) btnAbort.Enabled = measuring;
            if (lblStatus != null && status != null) lblStatus.Text = status;
        }
        private void LogPm(string msg)
        {
            try { RequestLog?.Invoke(msg); } catch { }
        }
        // 표만 작성해서 보여주고 레시피에 테이블 정의 저장
        private void btnPT_Build_Click(object sender, EventArgs e)
        {
            try
            {
                double freq = (double)numFreq_1.Value;

                _pendingTable = BuildTableFromUi(freq);
                // 테이블 만들고도 위치 0안되게 변경
                decimal? keepStartX = (numPowerMeasureX != null) ? numPowerMeasureX.Value : (decimal?)null;
                decimal? keepStartY = (numPowerMeasureY != null) ? numPowerMeasureY.Value : (decimal?)null;

                _powerRows.Clear();
                int idx = 1;
                foreach (var r in _pendingTable.Rows.OrderBy(r => r.TargetW))
                {
                    _powerRows.Add(new PowerRow
                    {
                        Index = idx++,
                        Frequency = _pendingTable.Frequency,
                        TargetW = r.TargetW,
                        ResultAttPos = r.AttPos,
                        RecipeName = _pmCurrentName ?? (tbRecipeName?.Text ?? "")
                    });
                }
                _gridPower.Refresh();
                PullTableToControls(_pendingTable);
                // 테이블 만들고도 위치 0안되게 변경
                if (keepStartX.HasValue && numPowerMeasureX != null) numPowerMeasureX.Value = keepStartX.Value;
                if (keepStartY.HasValue && numPowerMeasureY != null) numPowerMeasureY.Value = keepStartY.Value;

                if (lblStatus != null) lblStatus.Text = $"[Make Table] {_powerRows.Count} pts @ {_pendingTable.Frequency} (저장 전)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "PT_Build 실패: " + ex.Message, "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // PT_Measure: 별도 버튼에서만 측정 수행 (1010/1011 분기) 
        private async void btnPT_Measure_Click(object sender, EventArgs e)
        {
            var mySeq = Interlocked.Increment(ref _measSeq);
            _measCts?.Cancel();
            _measCts = new CancellationTokenSource();
            CancellationToken ctMeas = _measCts.Token;
            var pmRecipeNameUsed = GetCurrentPmRecipeNameSafe();
            LogPm($"[PM][MEASURE] start seq={mySeq}, IF={(SelectedInterface == PMInterface.Meas1_1010 ? "1010" : "1011")}, recipe='{pmRecipeNameUsed}'");

            // ─────────────────────────────────────────────────────────────
            // #1) 1010: 테이블 전체 측정 (기존 흐름 유지)
            // ─────────────────────────────────────────────────────────────
            if (SelectedInterface == PMInterface.Meas1_1010)
            {
                if (_powerRows.Count == 0 && _pendingTable == null)
                {
                    MessageBox.Show(this, "먼저 Make Table 을 생성하세요.", "PowerTable",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (RequestMeas1Async == null)
                {
                    MessageBox.Show(this, "측정 콜백(RequestMeas1Async)이 연결되지 않았습니다.", "PowerTable",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SetMeasuringUi(true, "Measuring (1010)...");
                LogPm($"[PM][1010] start. seq={mySeq}");
                LogPm($"[PM][1010] recipe='{pmRecipeNameUsed}', pts={_powerRows?.Count ?? 0}, freq={(double)numFreq_1.Value:F3}");
                try
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.MarqueeAnimationSpeed = 50;
                    lblStatus.Text = "Measuring (1010)...";

                    var (uiX, uiY) = ReadStartXYFromUI();

                    // 기본값: 문서에 저장된 값
                    double startX = _pmCurrent?.PowerMeterStartX ?? _doc?.PowerMeterStartX ?? 0.0;
                    double startY = _pmCurrent?.PowerMeterStartY ?? _doc?.PowerMeterStartY ?? 0.0;

                    // UI 값이 둘 다 0이 아니면(=사용자가 지정) UI 값을 우선 사용
                    bool uiSpecified = Math.Abs(uiX) > 1e-9 || Math.Abs(uiY) > 1e-9;
                    if (uiSpecified)
                    {
                        if (!double.IsNaN(uiX)) startX = uiX;
                        if (!double.IsNaN(uiY)) startY = uiY;
                    }

                    if (Math.Abs(startX) < 1e-9 && Math.Abs(startY) < 1e-9)
                    {
                        startX = _doc?.Parameters?.FirstHoleX ?? 0;  // 최후 fallback
                        startY = _doc?.Parameters?.FirstHoleY ?? 0;
                    }

                    if (RequestMoveToPowerStartAsync != null)
                    {
                        using (var cts = new CancellationTokenSource())
                            await RequestMoveToPowerStartAsync(startX, startY, cts.Token);
                    }

                    // 측정 요청 payload
                    var tables = _powerRows.Select(r => new NetCommon.ST_POWER_TABLE
                    {
                        dFrequency = r.Frequency,
                        dTargetPower = r.TargetW,
                        dAttMinPos = (double)numAttMin_1.Value,
                        dAttMaxPos = (double)numAttMax_1.Value
                    }).ToArray();

                    var rsp = await AwaitWithCancel(RequestMeas1Async(tables), ctMeas);
                    if (mySeq != _measSeq || ctMeas.IsCancellationRequested) return;
                    // 결과 반영: 그리드 + 작업본(_pendingTable.Rows)
                    if (rsp?.dResultAttPos != null)
                    {
                        // 표시 순서대로 채우기
                        int i = 0;
                        foreach (DataGridViewRow gr in _gridPower.Rows)
                        {
                            if (i >= rsp.dResultAttPos.Length) break;
                            if (gr.DataBoundItem is PowerRow pr)
                            {
                                pr.ResultAttPos = rsp.dResultAttPos[i++];
                                LogPm($"[PM][1010] ResultAttPos = {pr.ResultAttPos:F3}");
                            }
                        }
                        _gridPower.Refresh();
                    }

                    lblStatus.Text = $"Measure OK (1010). ({_powerRows.Count} pts, 아직 저장 전)";
                    LogPm($"[PM][1010] done. seq={mySeq}, pts={_gridPower.Rows.Count}");
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = "[1010] 측정 취소됨";
                    LogPm($"[PM][1010] canceled. seq={mySeq}");
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Measure(1010) failed";
                    LogPm($"[PM][1010][E] {ex.Message}");
                    MessageBox.Show(this, "Power measure(1010) 실패: " + ex.Message, "PowerTable",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.MarqueeAnimationSpeed = 0;
                    SetMeasuringUi(false);
                    MessageBox.Show(this, "Power measure Interface#1(1010) 측정완료.", "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            // ─────────────────────────────────────────────────────────────
            // #2) 1011: 단일 조건 측정
            // ─────────────────────────────────────────────────────────────
            if (RequestMeas2Async == null)
            {
                MessageBox.Show(this, "측정 콜백(RequestMeas2Async)이 연결되지 않았습니다.",
                                "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 50;
                lblStatus.Text = "Measuring (1011)...";

                // 시작 위치 계산(1010과 동일 로직)
                var (uiX, uiY) = ReadStartXYFromUI();

                double startX2 = _pmCurrent?.PowerMeterStartX ?? _doc?.PowerMeterStartX ?? 0.0;
                double startY2 = _pmCurrent?.PowerMeterStartY ?? _doc?.PowerMeterStartY ?? 0.0;

                bool uiSpecified2 = Math.Abs(uiX) > 1e-9 || Math.Abs(uiY) > 1e-9;
                if (uiSpecified2)
                {
                    if (!double.IsNaN(uiX)) startX2 = uiX;
                    if (!double.IsNaN(uiY)) startY2 = uiY;
                }

                if (Math.Abs(startX2) < 1e-9 && Math.Abs(startY2) < 1e-9)
                {
                    startX2 = _doc?.Parameters?.FirstHoleX ?? 0;  // 최후 fallback
                    startY2 = _doc?.Parameters?.FirstHoleY ?? 0;
                }

                if (RequestMoveToPowerStartAsync != null)
                {
                    using (var cts = new CancellationTokenSource())
                        await RequestMoveToPowerStartAsync(startX2, startY2, cts.Token);
                }
                // ScanNo/Range%/Freq/AttPos 확보
                int scanNo = GetScanNoOr(1);
                double range = GetRangePercentOr(0); // 허용 편차(%) 또는 스펙상 range
                //double freq = (row.Frequency > 0) ? row.Frequency : (_pendingTable?.Frequency ?? 0.0);
                //입력한 Feq가져오기 -PGT
                double freq = (double)numFreq_2.Value;
                if (freq <= 0) throw new Exception("유효한 주파수를 찾을 수 없습니다. 테이블에 주파수가 포함되어야 합니다.");
                double att = (double)numAttPos_2.Value;
                LogPm($"[PM][1011] start. scanNo={scanNo}, range={range}");
                LogPm($"[PM][1011] recipe='{pmRecipeNameUsed}', scanNo={scanNo}, freq={freq:F3}, att={att:F3}, range={range}");
                // 1011 요청
                var rsp = await AwaitWithCancel(RequestMeas2Async(scanNo, range, freq, att), ctMeas);

                // 결과 출력(표 구조 변경 없이 상태 라벨로만 표시)
                lblStatus.Text =
                    $"[1011] ScanNo={rsp.nScanNo}, Result={(rsp.bResult == 1 ? "OK" : "NG")}, " +
                    $"LaserPower={rsp.dResultLaserPower:F3} W @ Freq={freq:F3}, Att={att:F3}";

                MessageBox.Show(this, "Power measure Interface#2 (1011) 측정완료.\r\n" +
                    $"LaserPower={rsp.dResultLaserPower:F3} W @ Freq={freq:F3}, Att={att:F3}",
                "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Measure(1011) failed";
                MessageBox.Show(this, "Power measure(1011) 실패: " + ex.Message,
                                "PowerTable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnimationSpeed = 0;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = (tbRecipeName?.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "레시피 이름을 입력하세요.", "Add",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 작업본 없으면 UI 값으로 즉석 생성 (주파수 선택 규칙 동일)
                double freq = 0.0;
                if (_doc?.DefaultTooling?.Freq > 0) freq = _doc.DefaultTooling.Freq;
                else if (_pmCurrent?.PowerTables?.Count > 0) freq = _pmCurrent.PowerTables.Keys.First();

                var table = _pendingTable ?? BuildTableFromUi(freq);

                bool exists = _pmStore.Exists(name);
                if (exists)
                {
                    // 한번만 묻기
                    var ans = MessageBox.Show(
                        this,
                        $"'{name}' 레시피가 이미 존재합니다.\r\n덮어쓰시겠습니까?",
                        "Overwrite?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                    if (ans != DialogResult.Yes)
                    {
                        lblStatus.Text = "[Add] 사용자가 덮어쓰기를 취소했습니다.";
                        return;
                    }
                }

                // 파워 전용 문서에만 저장
                var doc = exists ? _pmStore.Load(name) : _pmStore.New();
                doc.SavePowerTable(table);

                // 시작 위치는 기존 값 유지, 없으면 0
                // 필요 시 UI에서 StartX/Y를 입력받아 doc.PowerMeterStartX/Y에 넣으세요.
                var (sx, sy) = ReadStartXYFromUI();
                if (!double.IsNaN(sx)) doc.PowerMeterStartX = sx;
                if (!double.IsNaN(sy)) doc.PowerMeterStartY = sy;

                _pmStore.Save(name, doc);

                _pmCurrent = doc;
                _pmCurrentName = name;
                _pendingTable = PickPowerTable(doc);
                _lastSavedName = name;

                RefreshRecipeList();
                SelectListByName(lstPowerRecipeList, name);

                lblStatus.Text = exists
                    ? $"[Overwrite] 저장 완료: '{name}' ({table.Rows.Count} pts)"
                    : $"[Add] 저장 완료: '{name}' ({table.Rows.Count} pts)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Add/Overwrite 실패: " + ex.Message,
                    "PowerRecipe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var name = lstPowerRecipeList?.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "삭제할 레시피를 선택하세요.", "Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show(this, $"'{name}' 를 삭제할까요?", "Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            try
            {
                _pmStore.Delete(name);
                if (string.Equals(_lastSavedName, name, StringComparison.OrdinalIgnoreCase))
                    _lastSavedName = null;

                _pmCurrentName = null;
                _pmCurrent = null;
                _pendingTable = null;
                _powerRows.Clear();
                _gridPower.Refresh();

                RefreshRecipeList();
                lblStatus.Text = $"[Delete] '{name}' 삭제됨";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Delete 실패: " + ex.Message,
                    "PowerRecipe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectListByName(ListBox lb, string name)
        {
            if (lb == null || string.IsNullOrWhiteSpace(name)) return;
            for (int i = 0; i < lb.Items.Count; i++)
                if (string.Equals(lb.Items[i] as string, name, StringComparison.OrdinalIgnoreCase))
                { lb.SelectedIndex = i; return; }
        }

        private (double sx, double sy) ReadStartXYFromUI()
        {
            double sx = (numPowerMeasureX != null) ? (double)numPowerMeasureX.Value : double.NaN;
            double sy = (numPowerMeasureY != null) ? (double)numPowerMeasureY.Value : double.NaN;
            return (sx, sy);
        }
    }
}