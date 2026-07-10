using System;
using System.Drawing;
using System.Windows.Forms;

namespace StageWin.Etc
{
    public partial class NotificationBlinkForm : Form
    {
        private readonly Label _lbl;
        private readonly Timer _tmr;
        private bool _dimmed;
        private readonly int _intervalMs;
        private double _phase;       // 0.0 ~ 1.0
        private readonly double _step;
        private readonly Color _baseColor;
        private readonly Color _highlightColor;

        public NotificationBlinkForm(string message, int intervalMs = 450, Color? baseColor = null)
        {
            _intervalMs = Math.Max(120, intervalMs);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint, true);
            UpdateStyles();
            _baseColor = baseColor ?? Color.FromArgb(20, 120, 220);
            _highlightColor = ControlPaint.Light(_baseColor);

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = _baseColor;
            ForeColor = Color.White;
            Opacity = 0.95;
            Size = new Size(520, 120);
            Padding = new Padding(16);

            _lbl = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("맑은 고딕", 20f, FontStyle.Bold),
                Text = message
            };
            Controls.Add(_lbl);

            // 살짝 라운드 사각형 느낌
            var rc = Region;
            try
            {
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                int r = 14;
                var rect = new Rectangle(0, 0, Width, Height);
                path.AddArc(rect.Left, rect.Top, r, r, 180, 90);
                path.AddArc(rect.Right - r, rect.Top, r, r, 270, 90);
                path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - r, r, r, 90, 90);
                path.CloseAllFigures();
                rc = new Region(path);
                Region = rc;
            }
            catch { if (rc != null) rc.Dispose(); }

            _tmr = new Timer { Interval = 30 };
            _phase = 0.0;
            _step = 30.0 / _intervalMs; // intervalMs 동안 0→1

            _tmr.Tick += (s, e) =>
            {
                if (IsDisposed || Disposing) { _tmr.Stop(); return; }
                StepFade();
            };
        }

        private static Color LerpColor(Color a, Color b, double t)
        {
            t = Math.Max(0.0, Math.Min(1.0, t));
            int r = (int)(a.R + (b.R - a.R) * t);
            int g = (int)(a.G + (b.G - a.G) * t);
            int b2 = (int)(a.B + (b.B - a.B) * t);
            return Color.FromArgb(r, g, b2);
        }

        private void StepFade()
        {
            _phase += _step;
            if (_phase > 1.0) _phase -= 1.0;

            // 0~1 구간을 sin으로 변환해서 부드러운 곡선
            double x = Math.Sin(_phase * Math.PI);      // 0→1→0
            double t = 0.5 + 0.5 * x;                   // 0.5 ~ 1.0

            try
            {
                BackColor = LerpColor(_baseColor, _highlightColor, t);
                Opacity = 0.80 + 0.15 * t;              // 0.80 ~ 0.95
            }
            catch
            {
                // 일부 환경에서 Opacity 예외 방어
            }
        }

        public void UpdateMessage(string msg)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired) BeginInvoke(new Action<string>(UpdateMessage), msg);
            else _lbl.Text = msg;
        }

        public void StartBlink(IWin32Window owner = null)
        {
            CenterToOwner(owner as Form);
            _tmr.Start();
            Show(owner);
            Activate();
        }

        public void StopBlinkAndCloseSafe()
        {
            try { _tmr.Stop(); } catch { }
            try { if (!IsDisposed) Close(); } catch { }
            try { Dispose(); } catch { }
        }

        private void CenterToOwner(Form owner)
        {
            Rectangle bounds;
            if (owner != null && owner.Visible)
            {
                var w = owner.Width; var h = owner.Height;
                var x = owner.Left + (w - Width) / 2;
                var y = owner.Top + (h - Height) / 2;
                bounds = new Rectangle(x, y, Width, Height);
            }
            else
            {
                var scr = Screen.PrimaryScreen.WorkingArea;
                bounds = new Rectangle(scr.Left + (scr.Width - Width) / 2,
                                       scr.Top + (scr.Height - Height) / 2,
                                       Width, Height);
            }
            Location = new Point(bounds.Left, bounds.Top);
        }
    }
    public static class BlinkNotifier
    {
        private static readonly object _sync = new object();
        private static NotificationBlinkForm _form;
        private static int _refCount = 0;
        private static Form _owner;

        public sealed class Session : IDisposable
        {
            private bool _disposed;
            public void Update(string msg) => BlinkNotifier.Update(msg);
            public void Dispose() { BlinkNotifier.Hide(); _disposed = true; }
        }

        /// <summary>
        /// 팝업을 띄우고 참조 카운트를 +1 합니다. Dispose시 자동 닫힘(0 될 때).
        /// </summary>
        public static Session Show(Form owner, string msg,
                                   int blinkIntervalMs = 450,
                                   Color? theme = null)
        {
            EnsureOnUi(owner, () =>
            {
                lock (_sync)
                {
                    _owner = owner ?? _owner;
                    if (_form == null || _form.IsDisposed)
                        _form = new NotificationBlinkForm(msg, blinkIntervalMs, theme);
                    else
                        _form.UpdateMessage(msg);

                    if (!_form.Visible) _form.StartBlink(_owner);
                    _refCount++;
                }
            });
            return new Session();
        }

        /// <summary>현재 떠있는 팝업 메시지만 갱신.</summary>
        public static void Update(string msg)
        {
            lock (_sync)
            {
                if (_form != null && !_form.IsDisposed) _form.UpdateMessage(msg);
                System.Threading.Thread.Sleep(200);
            }
        }

        /// <summary>참조 카운트 -1, 0이면 닫기.</summary>
        public static void Hide()
        {
            EnsureOnUi(_owner, () =>
            {
                lock (_sync)
                {
                    _refCount = Math.Max(0, _refCount - 1);
                    if (_refCount == 0 && _form != null && !_form.IsDisposed)
                    {
                        _form.StopBlinkAndCloseSafe();
                        _form = null;
                    }
                }
            });
        }

        /// <summary>작업을 실행하는 동안 자동 표시/종료.</summary>
        public static async System.Threading.Tasks.Task RunAsync(
            Form owner, string msg, Func<System.Threading.CancellationToken,
            System.Threading.Tasks.Task> work,
            int blinkIntervalMs = 450,
            Color? theme = null,
            System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            using (Show(owner, msg, blinkIntervalMs, theme))
                await work(ct).ConfigureAwait(true); // UI 컨텍스트 유지
        }

        private static void EnsureOnUi(Form owner, Action a)
        {
            if (a == null) return;

            // 우선순위 명확화: owner가 null이면 OpenForms[0] 또는 null
            var target = owner ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);

            // target이 없거나(콘솔/초기화 단계) 핸들이 없으면 그냥 실행
            if (target == null || target.IsDisposed || !target.IsHandleCreated)
            {
                a();
                return;
            }

            if (target.InvokeRequired)
                target.BeginInvoke(new MethodInvoker(() => a()));
            else
                a();
        }

    }
}
