using System;
using System.Drawing;
using System.Windows.Forms;
using StageWin.Etc;

namespace StageWin.UIForms
{
    public sealed class ZHomeParamForm : Form
    {
        private ComboBox comboBox_Direction;
        private ComboBox comboBox_Signal;
        private ComboBox comboBox_ZPhase;

        private TextBox txt_ClearTime;
        private TextBox txt_Offset;
        private TextBox txt_Vel1st;
        private TextBox txt_Vel2nd;
        private TextBox txt_Vel3rd;
        private TextBox txt_VelLast;
        private TextBox txt_Acc1st;
        private TextBox txt_Acc2nd;

        private Button btnOk;
        private Button btnCancel;

        public AjinHomeParams Result { get; private set; }

        public ZHomeParamForm(AjinHomeParams initial)
        {
            Text = "Z Home Parameter";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(420, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            BuildUi();
            LoadFrom(initial ?? new AjinHomeParams());
        }

        private void BuildUi()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Padding = new Padding(10),
                AutoSize = true,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            comboBox_Direction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            comboBox_Direction.Items.AddRange(new object[] {"-1: DIR_CCW(-)", "1: DIR_CW(+)" });

            comboBox_Signal = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            comboBox_Signal.Items.AddRange(new object[] { "0: PosEndLimit", "1: NegEndLimt","4: HomeSensor", "5: EncodZPhase" });

            comboBox_ZPhase = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            comboBox_ZPhase.Items.AddRange(new object[] { "0: Disable", "1: Dir_RevHome", "2: Dir_Home" });

            txt_ClearTime = new TextBox();
            txt_Offset = new TextBox();
            txt_Vel1st = new TextBox();
            txt_Vel2nd = new TextBox();
            txt_Vel3rd = new TextBox();
            txt_VelLast = new TextBox();
            txt_Acc1st = new TextBox();
            txt_Acc2nd = new TextBox();

            AddRow(table, 0, "Direction", comboBox_Direction);
            AddRow(table, 1, "Home Signal", comboBox_Signal);
            AddRow(table, 2, "Z Phase Use", comboBox_ZPhase);
            AddRow(table, 3, "Clear Time [ms]", txt_ClearTime);
            AddRow(table, 4, "Offset", txt_Offset);
            AddRow(table, 5, "Vel 1st", txt_Vel1st);
            AddRow(table, 6, "Vel 2nd", txt_Vel2nd);
            AddRow(table, 7, "Vel 3rd", txt_Vel3rd);
            AddRow(table, 8, "Vel Last", txt_VelLast);
            AddRow(table, 9, "Acc 1st", txt_Acc1st);
            AddRow(table, 10, "Acc 2nd", txt_Acc2nd);

            var panelButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                Height = 45,
            };

            btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 80 };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 80 };

            btnOk.Click += BtnOk_Click;

            panelButtons.Controls.Add(btnOk);
            panelButtons.Controls.Add(btnCancel);

            Controls.Add(table);
            Controls.Add(panelButtons);
        }

        private void AddRow(TableLayoutPanel table, int row, string label, Control editor)
        {
            if (row >= table.RowCount)
                table.RowCount = row + 1;

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

            var lbl = new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
            };

            editor.Dock = DockStyle.Fill;

            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(editor, 1, row);
        }

        private void LoadFrom(AjinHomeParams p)
        {
            int dirIndex = 1; // default 0
            if (p.Direction < 0) dirIndex = 0;
            else if (p.Direction > 0) dirIndex = 1;
            comboBox_Direction.SelectedIndex = dirIndex;

            comboBox_Signal.SelectedIndex = (int)Math.Max(0, Math.Min(comboBox_Signal.Items.Count - 1, p.HomeSignal));
            comboBox_ZPhase.SelectedIndex = (int)Math.Max(0, Math.Min(comboBox_ZPhase.Items.Count - 1, p.UseZPhase));

            txt_ClearTime.Text = p.ClearTimeMs.ToString("0.###");
            txt_Offset.Text = p.Offset.ToString("0.###");
            txt_Vel1st.Text = p.Vel1st.ToString("0.###");
            txt_Vel2nd.Text = p.Vel2nd.ToString("0.###");
            txt_Vel3rd.Text = p.Vel3rd.ToString("0.###");
            txt_VelLast.Text = p.VelLast.ToString("0.###");
            txt_Acc1st.Text = p.Acc1st.ToString("0.###");
            txt_Acc2nd.Text = p.Acc2nd.ToString("0.###");
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
        {
                int dir = 0;
                if (comboBox_Direction.SelectedIndex == 0) dir = -1;
                else if (comboBox_Direction.SelectedIndex == 2) dir = +1;

                var p = new AjinHomeParams
                {
                    Direction = dir,
                    HomeSignal = (uint)Math.Max(0, comboBox_Signal.SelectedIndex),
                    UseZPhase = (uint)Math.Max(0, comboBox_ZPhase.SelectedIndex),
                    ClearTimeMs = ParseDouble(txt_ClearTime, 1000),
                    Offset = ParseDouble(txt_Offset, 0),
                    Vel1st = ParseDouble(txt_Vel1st, 0.1),
                    Vel2nd = ParseDouble(txt_Vel2nd, 0.05),
                    Vel3rd = ParseDouble(txt_Vel3rd, 0.01),
                    VelLast = ParseDouble(txt_VelLast, 1),
                    Acc1st = ParseDouble(txt_Acc1st, 5),
                    Acc2nd = ParseDouble(txt_Acc2nd, 2),
                };

                Result = p;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Z Home Param", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static double ParseDouble(TextBox tb, double fallback)
        {
            if (tb == null) return fallback;
            if (double.TryParse(tb.Text, out var v)) return v;
            return fallback;
        }
    }
}
