using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace StageWin.UI
{
    partial class ToolingEditorForm
    {
        private IContainer components = null;

        private Panel panelTop;
        private Panel panelBottom;
        private Label lblHint;
        private Label lblTitle;

        private NumericUpDown numDefPower;
        private NumericUpDown numDefFreq;
        private NumericUpDown numDefProc;
        private NumericUpDown numDefShot;
        private NumericUpDown numDefJump;

        private Label lblPower;
        private Label lblFreq;
        private Label lblProc;
        private Label lblShot;
        private Label lblJump;
        private Button btnOk;
        private Button btnCancel;

        private DataGridView _grid;
        private DataGridViewTextBoxColumn colIdx;
        private DataGridViewTextBoxColumn colRow;
        private DataGridViewTextBoxColumn colCol;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colPwr;
        private DataGridViewTextBoxColumn colFreq;
        private DataGridViewTextBoxColumn colProc;
        private DataGridViewTextBoxColumn colShot;
        private DataGridViewTextBoxColumn colJump;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPower = new System.Windows.Forms.Label();
            this.numDefPower = new System.Windows.Forms.NumericUpDown();
            this.lblFreq = new System.Windows.Forms.Label();
            this.numDefFreq = new System.Windows.Forms.NumericUpDown();
            this.lblProc = new System.Windows.Forms.Label();
            this.numDefProc = new System.Windows.Forms.NumericUpDown();
            this.lblShot = new System.Windows.Forms.Label();
            this.numDefShot = new System.Windows.Forms.NumericUpDown();
            this.lblJump = new System.Windows.Forms.Label();
            this.numDefJump = new System.Windows.Forms.NumericUpDown();
            this.lblHint = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.flRight = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this._grid = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefProc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefShot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefJump)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.flRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblPower);
            this.panelTop.Controls.Add(this.numDefPower);
            this.panelTop.Controls.Add(this.lblFreq);
            this.panelTop.Controls.Add(this.numDefFreq);
            this.panelTop.Controls.Add(this.lblProc);
            this.panelTop.Controls.Add(this.numDefProc);
            this.panelTop.Controls.Add(this.lblShot);
            this.panelTop.Controls.Add(this.numDefShot);
            this.panelTop.Controls.Add(this.lblJump);
            this.panelTop.Controls.Add(this.numDefJump);
            this.panelTop.Controls.Add(this.lblHint);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(8);
            this.panelTop.Size = new System.Drawing.Size(1384, 72);
            this.panelTop.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(8, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(93, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Default Tooling:";
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(323, 23);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(40, 15);
            this.lblPower.TabIndex = 1;
            this.lblPower.Text = "Power";
            // 
            // numDefPower
            // 
            this.numDefPower.DecimalPlaces = 2;
            this.numDefPower.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDefPower.Location = new System.Drawing.Point(325, 38);
            this.numDefPower.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDefPower.Name = "numDefPower";
            this.numDefPower.Size = new System.Drawing.Size(100, 23);
            this.numDefPower.TabIndex = 2;
            // 
            // lblFreq
            // 
            this.lblFreq.AutoSize = true;
            this.lblFreq.Location = new System.Drawing.Point(443, 23);
            this.lblFreq.Name = "lblFreq";
            this.lblFreq.Size = new System.Drawing.Size(30, 15);
            this.lblFreq.TabIndex = 3;
            this.lblFreq.Text = "Freq";
            // 
            // numDefFreq
            // 
            this.numDefFreq.DecimalPlaces = 2;
            this.numDefFreq.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDefFreq.Location = new System.Drawing.Point(445, 38);
            this.numDefFreq.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numDefFreq.Name = "numDefFreq";
            this.numDefFreq.Size = new System.Drawing.Size(100, 23);
            this.numDefFreq.TabIndex = 4;
            // 
            // lblProc
            // 
            this.lblProc.AutoSize = true;
            this.lblProc.Location = new System.Drawing.Point(567, 23);
            this.lblProc.Name = "lblProc";
            this.lblProc.Size = new System.Drawing.Size(52, 15);
            this.lblProc.TabIndex = 5;
            this.lblProc.Text = "ProcSpd";
            // 
            // numDefProc
            // 
            this.numDefProc.DecimalPlaces = 1;
            this.numDefProc.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDefProc.Location = new System.Drawing.Point(569, 38);
            this.numDefProc.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numDefProc.Name = "numDefProc";
            this.numDefProc.Size = new System.Drawing.Size(90, 23);
            this.numDefProc.TabIndex = 6;
            // 
            // lblShot
            // 
            this.lblShot.AutoSize = true;
            this.lblShot.Location = new System.Drawing.Point(804, 23);
            this.lblShot.Name = "lblShot";
            this.lblShot.Size = new System.Drawing.Size(32, 15);
            this.lblShot.TabIndex = 7;
            this.lblShot.Text = "Shot";
            // 
            // numDefShot
            // 
            this.numDefShot.Location = new System.Drawing.Point(806, 38);
            this.numDefShot.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDefShot.Name = "numDefShot";
            this.numDefShot.Size = new System.Drawing.Size(83, 23);
            this.numDefShot.TabIndex = 8;
            // 
            // lblJump
            // 
            this.lblJump.AutoSize = true;
            this.lblJump.Location = new System.Drawing.Point(688, 23);
            this.lblJump.Name = "lblJump";
            this.lblJump.Size = new System.Drawing.Size(36, 15);
            this.lblJump.TabIndex = 9;
            this.lblJump.Text = "Jump";
            // 
            // numDefJump
            // 
            this.numDefJump.DecimalPlaces = 1;
            this.numDefJump.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDefJump.Location = new System.Drawing.Point(690, 38);
            this.numDefJump.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numDefJump.Name = "numDefJump";
            this.numDefJump.Size = new System.Drawing.Size(90, 23);
            this.numDefJump.TabIndex = 10;
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Location = new System.Drawing.Point(8, 40);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(292, 15);
            this.lblHint.TabIndex = 11;
            this.lblHint.Text = "※ Default는 새 홀/미지정 홀에 기본으로 적용됩니다.";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.flRight);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 613);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(8);
            this.panelBottom.Size = new System.Drawing.Size(1384, 48);
            this.panelBottom.TabIndex = 1;
            // 
            // flRight
            // 
            this.flRight.AutoSize = true;
            this.flRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flRight.Controls.Add(this.btnCancel);
            this.flRight.Controls.Add(this.btnOk);
            this.flRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.flRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flRight.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.flRight.Location = new System.Drawing.Point(1144, 8);
            this.flRight.Margin = new System.Windows.Forms.Padding(0);
            this.flRight.Name = "flRight";
            this.flRight.Size = new System.Drawing.Size(232, 32);
            this.flRight.TabIndex = 0;
            this.flRight.WrapContents = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(124, 8);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(8, 8);
            this.btnOk.Margin = new System.Windows.Forms.Padding(8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Location = new System.Drawing.Point(0, 72);
            this._grid.Name = "_grid";
            this._grid.RowHeadersVisible = false;
            this._grid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(1384, 541);
            this._grid.TabIndex = 0;
            // 
            // ToolingEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(1384, 661);
            this.Controls.Add(this._grid);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(820, 520);
            this.Name = "ToolingEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tooling Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolingEditorForm_FormClosing);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefProc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefShot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefJump)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.flRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);

        }

        private FlowLayoutPanel flRight;
    }
}