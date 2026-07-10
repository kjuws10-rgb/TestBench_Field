using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using StageWin.Driver.LDS;
using StageWin.Driver.Motion;
using StageWin.Etc;
using StageWin.Safety;

namespace StageWin.UI
{
    public partial class ESCFlatnessForm : Form
    {
        private struct Pt
        {
            public double X;      // 절대 X (mm)
            public double Y;      // 절대 Y (mm)
            public double Flat;   // 측정 평탄 (mm)
            public double Thick;  // 측정 두께 (mm)
            public bool Measured; // 측정 여부
        }

        private Pt[,] _pts;                 // 포인트 좌표/결과 테이블
        private double _baseFlat;           // 첫 측정 셀의 평탄 기준
        private double _baseThick;          // 첫 측정 셀의 두께 기준
        private bool _baseInited;           // 기준 초기화 여부

        private readonly IMotionController _motion;   // X=Review, Y=Main
        private readonly ILdsSampler _lds;
        private CancellationTokenSource _cts;
        private readonly System.Windows.Forms.Timer _tmrLive = new System.Windows.Forms.Timer { Interval = 100 };

        // Safety
        public Func<ISafetyContext> GetSafetyContext { get; set; }
        private readonly System.Windows.Forms.Timer _tmrSafety = new System.Windows.Forms.Timer { Interval = 100 };
        private volatile bool _safetyStopping = false;
        private ISafetyContext _fallbackCtx;
        public Func<ProgramMode> ModeProvider { get; set; }

        // 원점 및 포인트 간격
        private double _originX, _originY;

        // 결과 버퍼
        private double[,] _flat;     // ch1
        private double[,] _thick;    // ch2 - ch1
        private ISafetyContext TryGetSafetyCtx()
        {
            // 1순위: 외부에서 주입된 컨텍스트 사용
            var ctx = GetSafetyContext?.Invoke();
            if (ctx != null) return ctx;

            // 2순위: 폴백 컨텍스트 — 반드시 실제 ProgramMode를 끌어오도록 수정
            if (_fallbackCtx == null)
            {
                _fallbackCtx = new BasicSafetyContext(
                    modeGetter: () => (ModeProvider != null ? ModeProvider() : ProgramMode.Manual)
                );
            }
            return _fallbackCtx;
        }
        public ESCFlatnessForm(IMotionController motion, ILdsSampler lds)
        {
            InitializeComponent();
            _motion = motion ?? throw new ArgumentNullException(nameof(motion));
            _lds = lds ?? throw new ArgumentNullException(nameof(lds));
            InitGrid();
            InitLivePanel(); // 라이브 패널 생성

            // Safety 인터락
            _tmrSafety.Tick += (s, e) =>
            {
                try
                {
                    var ctx = TryGetSafetyCtx();
                    if (ctx != null && !_safetyStopping)
                    {
                        if (SafetyPolicy.ShouldForceAxesStop(ctx, out var why))
                        {
                            _safetyStopping = true;
                            _cts?.Cancel(); // 측정 루프 취소
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    try { await _motion.JogStopAsync(Axis.X); } catch { }
                                    try { await _motion.JogStopAsync(Axis.Y); } catch { }
                                    try { await _motion.StopAsync(Axis.X); } catch { }
                                    try { await _motion.StopAsync(Axis.Y); } catch { }
                                }
                                finally { _safetyStopping = false; }
                            });
                        }
                    }
                }
                catch { }
            };
            _tmrSafety.Start();
        }

        private void InitGrid()
        {
            SetupGridCommon(grid, defaultRowSelect: false, profile: GridProfile.Card);

            grid.ReadOnly = true;                // 편집은 다이얼로그로만
            grid.RowHeadersVisible = true;
            grid.RowHeadersWidth = 70;
            grid.Font = new Font("Consolas", 9f);

            grid.CellMouseClick += grid_CellMouseClick;
        }

        // SetupGridCommon에서 호출하는 훅(우클릭 시 셀 선택 보장 등)
        private void WireCellSelectionBehavior(DataGridView gv, bool defaultRowSelect)
        {
            gv.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    gv.CurrentCell = gv[e.ColumnIndex, e.RowIndex];
            };
        }

        public enum GridProfile { Table, Card }

        private void SetupGridCommon(DataGridView gv, bool defaultRowSelect = false, GridProfile profile = GridProfile.Table)
        {
            gv.AllowUserToAddRows = false;
            gv.AllowUserToDeleteRows = false;
            gv.AllowUserToResizeColumns = false;
            gv.AllowUserToResizeRows = false;
            gv.MultiSelect = false;
            gv.RowHeadersVisible = false;

            gv.SelectionMode = defaultRowSelect
                ? DataGridViewSelectionMode.FullRowSelect
                : DataGridViewSelectionMode.CellSelect;

            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gv.ScrollBars = ScrollBars.Both;
            gv.BackgroundColor = Color.White;
            gv.EnableHeadersVisualStyles = false;

            var headerCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("맑은 고딕", 9F, FontStyle.Bold)
            };
            gv.ColumnHeadersDefaultCellStyle = headerCenter;

            // 🔸 프로필별 기본값 분기
            if (profile == GridProfile.Table)
            {
                gv.DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    WrapMode = DataGridViewTriState.False,
                    Padding = Padding.Empty
                };
                gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                gv.RowTemplate.Height = 35;
            }
            else // GridProfile.Card : 멀티라인 카드 텍스트용
            {
                gv.DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.TopLeft,
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(4, 2, 4, 6)
                };
                gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders; // 줄바꿈 높이 자동
                gv.RowTemplate.Height = 100; // 초기값(이후 AutoResizeRows가 실제 높이로 맞춤)
                                            // 선택 색상이 텍스트를 가리지 않도록
                gv.DefaultCellStyle.SelectionBackColor = gv.DefaultCellStyle.BackColor;
                gv.DefaultCellStyle.SelectionForeColor = gv.DefaultCellStyle.ForeColor;
            }

            WireCellSelectionBehavior(gv, defaultRowSelect);
        }


        private void InitLivePanel()
        {
            _tmrLive.Tick += (s, e) =>
            {
                // 1) 현재 ACS 위치 실시간 표시
                try
                {
                    txtOriginX.Text = _motion.GetPosition(Axis.X).ToString("F3"); // 현재 위치
                    txtOriginY.Text = _motion.GetPosition(Axis.Y).ToString("F3");
                }
                catch { /* ignore */ }

                // 2) LDS 라이브
                var cur = _lds.TryGetLatest();
                if (cur.HasValue)
                {
                    double ch1 = cur.Value.ch1;
                    double ch2 = cur.Value.ch2;
                    double flat = ch1;
                    double thick = ch2 - ch1;
                    lblLiveCh1.Text = $"Ch1: {ch1:F3} mm";
                    lblLiveCh2.Text = $"Ch2: {ch2:F3} mm";
                    lblLiveFlat.Text = $"Flat: {flat:F3} mm";
                    lblLiveThick.Text = $"Thick: {thick:F3} mm";
                }
                else
                {
                    lblLiveCh1.Text = "Ch1: -";
                    lblLiveCh2.Text = "Ch2: -";
                    lblLiveFlat.Text = "Flat: -";
                    lblLiveThick.Text = "Thick: -";
                }
            };
            _tmrLive.Start();

            _lds.OnSample += (a, b, ts) => { /* 필요 시 */ };
        }
        private bool GuardAllowAxisMove()
        {
            var ctx = GetSafetyContext?.Invoke();
            if (ctx == null) return true;

            var eval = SafetyPolicy.CheckGlobalMotionInterlockForAxis(ctx);
            if (!eval.Allowed)
            {
                MessageBox.Show(this, eval.Reason ?? "도어 인터락으로 축 동작이 차단되었습니다.",
                    "Safety", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // 현재 위치를 "원점"으로 설정
        private void btnPickOrigin_Click(object sender, EventArgs e)
        {
            try
            {
                _originX = _motion.GetPosition(Axis.X);
                _originY = _motion.GetPosition(Axis.Y);
                _baseInited = false; // 기준은 다시 캡처되도록
                lblStatus.Text = $"원점 설정: ({_originX:F3}, {_originY:F3})";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "원점 설정 실패: " + ex.Message, "Origin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool _isMeasuring;
        private (int r, int c)? _measuringCell;

        private void grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (_isMeasuring) return; // 측정 중 차단

            if (e.Button == MouseButtons.Left)
                _ = MoveScanWithConfirmAsync(e.RowIndex, e.ColumnIndex);
            else if (e.Button == MouseButtons.Right)
                EditPoint(e.RowIndex, e.ColumnIndex);
        }
        private void SetMeasuringUi(bool on)
        {
            _isMeasuring = on;

            // 폼의 모든 컨트롤 순회하며 잠금, 단 btnStop만 예외
            void Toggle(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    if (c == btnStop) { c.Enabled = true; continue; }
                    if (c == grid) { c.Enabled = true; continue; } // 그리드는 보이되 입력만 우리가 차단
                    c.Enabled = !on;
                    if (c.HasChildren) Toggle(c);
                }
            }
            Toggle(this);

            this.UseWaitCursor = on;
            grid.Cursor = on ? Cursors.No : Cursors.Default;
        }

        private void MarkMeasuring(int y, int x, bool on)
        {
            if (y < 0 || x < 0 || y >= grid.RowCount || x >= grid.ColumnCount) return;

            var cell = grid.Rows[y].Cells[x];
            if (on)
            {
                _measuringCell = (y, x);
                cell.Style.BackColor = Color.LightGray; // 음영
            }
            else
            {
                // 최종 상태로 복구
                ApplyCard(y, x);
                _measuringCell = null;
            }
            grid.Refresh();
        }

        private void ClearMeasuringMarker()
        {
            if (_measuringCell.HasValue)
            {
                var (ry, rx) = _measuringCell.Value;
                MarkMeasuring(ry, rx, false);
            }
        }

        private async Task MoveScanWithConfirmAsync(int y, int x)
        {
            if (_isMeasuring) return; // 측정 중 차단
            if (!GuardAllowAxisMove()) return;

            double tx = _pts[y, x].X, ty = _pts[y, x].Y;
            var ans = MessageBox.Show(this,
                $"선택 좌표로 이동할까요?\nX:{tx:F3}mm  Y:{ty:F3}mm",
                "Move to Selected", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (ans != DialogResult.Yes) return;

            try
            {
                await _motion.ServoOnAsync(Axis.X);
                await _motion.ServoOnAsync(Axis.Y);
                await Task.WhenAll(_motion.MoveAbsAsync(Axis.X, tx),
                                   _motion.MoveAbsAsync(Axis.Y, ty));
                await WaitAxisInPositionAsync(Axis.X, tx, (double)numTolX.Value, 20000, CancellationToken.None);
                await WaitAxisInPositionAsync(Axis.Y, ty, (double)numTolY.Value, 20000, CancellationToken.None);
                lblStatus.Text = $"Moved to (Y{y + 1}, X{x + 1}) → ({tx:F3}, {ty:F3})";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "이동 실패: " + ex.Message, "Move",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnBuildGrid_Click(object sender, EventArgs e)
        {
            BuildGrid();
        }
        private void BuildGrid()
        {
            int nx = (int)numCountX.Value;
            int ny = (int)numCountY.Value;
            double stepX = (double)numStepX.Value;
            double stepY = (double)numStepY.Value;

            grid.Columns.Clear();
            for (int x = 0; x < nx; x++)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    HeaderText = $"X{x + 1}",
                    Width = 160,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft; // 좌정렬
                grid.Columns.Add(col);
            }

            grid.Rows.Clear();
            for (int y = 0; y < ny; y++)
            {
                var idx = grid.Rows.Add();
                grid.Rows[idx].HeaderCell.Value = $"Y{y + 1}";
                grid.Rows[idx].Height = 92;
            }

            _pts = new Pt[ny, nx];
            _flat = new double[ny, nx];
            _thick = new double[ny, nx];

            for (int y = 0; y < ny; y++)
                for (int x = 0; x < nx; x++)
                {
                    _pts[y, x].X = _originX + x * stepX;  // 절대 좌표 저장
                    _pts[y, x].Y = _originY + y * stepY;
                    _pts[y, x].Measured = false;
                    ApplyCard(y, x);
                }

            _baseInited = false;
            lblStatus.Text = $"Grid 생성: {nx} x {ny}";

            grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
        }

        private void ApplyCard(int y, int x)
        {
            int idx = y * grid.ColumnCount + x + 1;
            string flatVal = _pts[y, x].Measured ? $"{_pts[y, x].Flat:F3}" : "----";
            string thickVal = _pts[y, x].Measured ? $"{_pts[y, x].Thick:F3}" : "----";

            string text =
                $"# {idx} Point\n" +
                $"X:{_pts[y, x].X:F3}mm\n" +
                $"Y:{_pts[y, x].Y:F3}mm\n" +
                $"Flat:{_pts[y, x].Flat - _baseFlat:F3}mm\n" +
                $"Thick:{_pts[y, x].Thick - _baseThick:F3}mm";

            var cell = grid.Rows[y].Cells[x];
            cell.Value = text;
            cell.Style.Alignment = DataGridViewContentAlignment.TopLeft;

            // 기준(첫 측정) 대비 Δ로 색상 판정만 수행
            cell.Style.BackColor = Color.White;
            cell.ToolTipText = null;
            if (_pts[y, x].Measured && _baseInited)
            {
                double dF = _pts[y, x].Flat - _baseFlat;
                double dT = _pts[y, x].Thick - _baseThick;
                double cF = (double)numCritFlat.Value;
                double cT = (double)numCritThick.Value;

                bool ngF = Math.Abs(dF) > cF;
                bool ngT = Math.Abs(dT) > cT;

                if (ngF && ngT) cell.Style.BackColor = Color.IndianRed;
                else if (ngF) cell.Style.BackColor = Color.Gold;
                else if (ngT) cell.Style.BackColor = Color.LightSalmon;

                cell.ToolTipText = $"ΔFlat={dF:+0.000;-0.000}mm, ΔThick={dT:+0.000;-0.000}mm";
            }
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_cts != null) return;
            if (!GuardAllowAxisMove()) return;
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;

            try
            {
                if (!_lds.IsConnected)
                {
                    MessageBox.Show(this, "LDS 센서가 연결되지 않았습니다.", "Flatness",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetMeasuringUi(true);              // UI 잠금 시작

                int nx = grid.ColumnCount, ny = grid.RowCount;
                int dwellMs = (int)numDwell.Value;

                await _motion.ServoOnAsync(Axis.X);
                await _motion.ServoOnAsync(Axis.Y);

                progressBar.Maximum = nx * ny;
                progressBar.Value = 0;

                bool snake = chkSnake.Checked;
                _baseInited = false;

                for (int y = 0; y < ny; y++)
                {
                    ct.ThrowIfCancellationRequested();
                    var xs = Enumerable.Range(0, nx).ToArray();
                    if (snake && (y % 2 == 1)) Array.Reverse(xs);

                    foreach (var xi in xs)
                    {
                        int x = xi;
                        ct.ThrowIfCancellationRequested();

                        // 현재 셀 음영 표시
                        MarkMeasuring(y, x, true);

                        double tx = _pts[y, x].X;
                        double ty = _pts[y, x].Y;

                        var mx = _motion.MoveAbsAsync(Axis.X, tx);
                        var my = _motion.MoveAbsAsync(Axis.Y, ty);
                        await Task.WhenAll(mx, my);

                        await WaitAxisInPositionAsync(Axis.X, tx, (double)numTolX.Value, 20000, ct);
                        await WaitAxisInPositionAsync(Axis.Y, ty, (double)numTolY.Value, 20000, ct);

                        await Task.Delay(dwellMs, ct);

                        var (ch1, ch2) = await _lds.AcquireOnceWhilePausedAsync(ct);
                        double flat = ch1, thick = ch2 - ch1;

                        if (!_baseInited) { _baseFlat = flat; _baseThick = thick; _baseInited = true; }

                        _pts[y, x].Flat = flat; _pts[y, x].Thick = thick; _pts[y, x].Measured = true;

                        // 결과 반영(색상 판정 포함) + 음영 해제
                        ApplyCard(y, x);
                        MarkMeasuring(y, x, false);

                        progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                        lblStatus.Text = $"Meas ({y + 1},{x + 1}) Flat:{flat:F3}  Thick:{thick:F3}";
                        Application.DoEvents();
                    }
                }

                lblStatus.Text = "측정 완료";
                MessageBox.Show(this, "평탄도 측정이 완료되었습니다.", "Flatness",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "사용자 중지";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "오류: " + ex.Message;
                MessageBox.Show(this, "측정 실패: " + ex.Message, "Flatness",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearMeasuringMarker();
                SetMeasuringUi(false);             // UI 잠금 해제
                _cts?.Dispose(); _cts = null;
            }
        }


        private void grid_CellDoubleClick_EditPoint(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            EditPoint(e.RowIndex, e.ColumnIndex);
        }

        private void EditPoint(int y, int x)
        {
            using (var dlg = new Form())
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.Text = $"Edit Point (Y{y + 1}, X{x + 1})";
                dlg.ClientSize = new Size(260, 130);
                dlg.MinimizeBox = dlg.MaximizeBox = false;

                var numX = new NumericUpDown
                {
                    Left = 80,
                    Top = 15,
                    Width = 150,
                    DecimalPlaces = 3,
                    Minimum = -100000,
                    Maximum = 100000,
                    Value = (decimal)_pts[y, x].X
                };
                var numY = new NumericUpDown
                {
                    Left = 80,
                    Top = 45,
                    Width = 150,
                    DecimalPlaces = 3,
                    Minimum = -100000,
                    Maximum = 100000,
                    Value = (decimal)_pts[y, x].Y
                };
                dlg.Controls.Add(new Label { Left = 20, Top = 18, Text = "X (mm)", AutoSize = true });
                dlg.Controls.Add(new Label { Left = 20, Top = 48, Text = "Y (mm)", AutoSize = true });
                dlg.Controls.Add(numX); dlg.Controls.Add(numY);

                var ok = new Button { Text = "OK", Left = 70, Width = 60, Top = 85, DialogResult = DialogResult.OK };
                var cancel = new Button { Text = "Cancel", Left = 140, Width = 60, Top = 85, DialogResult = DialogResult.Cancel };
                dlg.Controls.Add(ok); dlg.Controls.Add(cancel);
                dlg.AcceptButton = ok; dlg.CancelButton = cancel;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _pts[y, x].X = (double)numX.Value;   // 절대좌표로 저장
                    _pts[y, x].Y = (double)numY.Value;
                    _pts[y, x].Measured = false;         // 좌표 변경 → 측정값 무효화
                    ApplyCard(y, x);
                }
            }
        }


        private async void MoveScan()
        {
            if (grid.CurrentCell == null) return;
            if (!GuardAllowAxisMove()) return;
            int y = grid.CurrentCell.RowIndex;
            int x = grid.CurrentCell.ColumnIndex;

            double tx = _pts[y, x].X;
            double ty = _pts[y, x].Y;

            using (var dlg = new MoveConfirmForm(
                "Move to Review (Stage mm)",
                _motion.GetPosition(Axis.X), _motion.GetPosition(Axis.Y), tx, ty))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes)
                {
                    try
                    {
                        await _motion.ServoOnAsync(Axis.X);
                        await _motion.ServoOnAsync(Axis.Y);

                        var mx = _motion.MoveAbsAsync(Axis.X, tx);
                        var my = _motion.MoveAbsAsync(Axis.Y, ty);
                        await Task.WhenAll(mx, my);

                        await WaitAxisInPositionAsync(Axis.X, tx, (double)numTolX.Value, 20000, CancellationToken.None);
                        await WaitAxisInPositionAsync(Axis.Y, ty, (double)numTolY.Value, 20000, CancellationToken.None);

                        lblStatus.Text = $"Moved to (Y{y + 1}, X{x + 1})  →  ({tx:F3}, {ty:F3})";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "이동 실패: " + ex.Message, "Move", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else return;
            }
        }
        private void grid_CellOneClick_MoveScan(object sender, DataGridViewCellEventArgs e)
        {
            MoveScan();
        }

        private async Task WaitAxisInPositionAsync(Axis ax, double target, double tol, int timeoutMs, CancellationToken ct)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var acs = _motion as IAcsStatus;

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                ct.ThrowIfCancellationRequested();

                // 기존 IMotionController에 있는 동기 API 사용
                bool isBusy = false;
                try { isBusy = _motion.IsBusy(ax); } catch { /* ignore */ }

                double cur = double.NaN;
                try { cur = _motion.GetPosition(ax); } catch { /* ignore */ }

                bool inPosFlag = false;
                try { if (acs != null) inPosFlag = acs.IsInPosition(ax); } catch { /* ignore */ }

                // 판정: 플래그가 있으면 우선, 없으면 Busy+오차 기반으로 판정
                bool inpos = inPosFlag || (!isBusy && !double.IsNaN(cur) && Math.Abs(cur - target) <= tol);

                if (inpos)
                {
                    return;
                }

                await Task.Delay(10, ct);
            }
            throw new TimeoutException($"Axis {ax} in-position timeout (target={target:F3})");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try { _cts?.Cancel(); } catch { }
            try { _ = _motion.StopAsync(Axis.X); _ = _motion.StopAsync(Axis.Y); } catch { }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pts == null)
                {
                    MessageBox.Show(this, "데이터가 없습니다.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var sfd = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"Flatness_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath)
                })
                {
                    if (sfd.ShowDialog(this) != DialogResult.OK) return;

                    int ny = _pts.GetLength(0);
                    int nx = _pts.GetLength(1);

                    using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Row,Col,AbsX(mm),AbsY(mm),RelX(mm),RelY(mm),Flat(mm),Thick(mm),DeltaFlat(mm),DeltaThick(mm)");
                        for (int y = 0; y < ny; y++)
                        {
                            for (int x = 0; x < nx; x++)
                            {
                                double relX = _pts[y, x].X - _originX;
                                double relY = _pts[y, x].Y - _originY;
                                double df = (_pts[y, x].Measured && _baseInited) ? _pts[y, x].Flat - _baseFlat : 0;
                                double dt = (_pts[y, x].Measured && _baseInited) ? _pts[y, x].Thick - _baseThick : 0;

                                sw.WriteLine($"{y + 1},{x + 1},{_pts[y, x].X:F6},{_pts[y, x].Y:F6},{relX:F6},{relY:F6},{_pts[y, x].Flat:F6},{_pts[y, x].Thick:F6},{df:F6},{dt:F6}");
                            }
                        }
                    }
                    MessageBox.Show(this, "CSV 저장 완료", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "CSV 저장 실패: " + ex.Message, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
