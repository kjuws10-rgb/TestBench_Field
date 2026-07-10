using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace StageWin.Etc
{
    public enum LedState { Off, On, Warn, Err }

    public static class LedLabel
    {
        public static void Init(Label lb)
        {
            if (lb == null) return;
            lb.AutoSize = false;
            if (lb.Width < 18 || lb.Height < 18) lb.Size = new Size(18, 18);
            lb.Text = string.Empty;
            lb.BackColor = Color.Gray;
            lb.BorderStyle = BorderStyle.None;
            lb.Margin = Padding.Empty;
            lb.Padding = Padding.Empty;

            // 원형 Region
            lb.Resize -= OnResizeMakeRound;
            lb.Resize += OnResizeMakeRound;
            MakeRound(lb);

            // 테두리(외곽선)
            lb.Paint -= OnPaintBorder;
            lb.Paint += OnPaintBorder;
        }

        public static void Set(Label lb, LedState state)
        {
            if (lb == null) return;
            Color c = Color.Gray;
            switch (state)
            {
                case LedState.On: c = Color.LimeGreen; break;
                case LedState.Warn: c = Color.Gold; break;
                case LedState.Err: c = Color.Red; break;
                default: c = Color.Gray; break;
            }
            lb.BackColor = c;
            lb.Tag = state; // 필요 시 읽어올 수 있도록
            lb.Invalidate();
        }

        public static LedState Get(Label lb)
            => (lb?.Tag is LedState s) ? s : LedState.Off;

        // 내부: 원형 Region & 외곽선
        private static void OnResizeMakeRound(object sender, EventArgs e)
        {
            if (sender is Label lb) MakeRound(lb);
        }

        private static void MakeRound(Label lb)
        {
            var rect = new Rectangle(0, 0, lb.Width - 1, lb.Height - 1);
            using (var gp = new GraphicsPath())
            {
                gp.AddEllipse(rect);
                lb.Region?.Dispose();
                lb.Region = new Region(gp);
            }
        }

        private static void OnPaintBorder(object sender, PaintEventArgs e)
        {
            var lb = sender as Label;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.DimGray))
            {
                e.Graphics.DrawEllipse(pen, 0, 0, lb.Width - 1, lb.Height - 1);
            }
        }
    }
}
