namespace StageWin.UI
{
    partial class PowerMeterEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._gridPower = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.flRight = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.grpPT = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numFreq_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_Freq = new System.Windows.Forms.Label();
            this.numMinW_1 = new System.Windows.Forms.NumericUpDown();
            this.numRangePct_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_Range = new System.Windows.Forms.Label();
            this.numAttMax_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_AttMax = new System.Windows.Forms.Label();
            this.numAttMin_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_AttMin = new System.Windows.Forms.Label();
            this.numStepW_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_MinW = new System.Windows.Forms.Label();
            this.lblPT_StepW = new System.Windows.Forms.Label();
            this.numMaxW_1 = new System.Windows.Forms.NumericUpDown();
            this.lblPT_MaxW = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.numFreq_2 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numAttPos_2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbRecipeName = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstPowerRecipeList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRecipe_PowerList = new System.Windows.Forms.Label();
            this.rbtnIf1 = new System.Windows.Forms.RadioButton();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.rbtnIf2 = new System.Windows.Forms.RadioButton();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnPT_Measure = new System.Windows.Forms.Button();
            this.lblScanNo = new System.Windows.Forms.Label();
            this.numScanNo = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numPowerMeasureY = new System.Windows.Forms.NumericUpDown();
            this.numPowerMeasureX = new System.Windows.Forms.NumericUpDown();
            this.btnPT_Build = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._gridPower)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.flRight.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.grpPT.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFreq_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinW_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangePct_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttMax_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttMin_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepW_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxW_1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFreq_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttPos_2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScanNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerMeasureY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerMeasureX)).BeginInit();
            this.SuspendLayout();
            // 
            // _gridPower
            // 
            this._gridPower.AllowUserToAddRows = false;
            this._gridPower.AllowUserToDeleteRows = false;
            this._gridPower.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._gridPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridPower.Location = new System.Drawing.Point(0, 90);
            this._gridPower.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._gridPower.Name = "_gridPower";
            this._gridPower.RowHeadersVisible = false;
            this._gridPower.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._gridPower.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridPower.Size = new System.Drawing.Size(553, 911);
            this._gridPower.TabIndex = 5;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.flRight);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 1001);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.panelBottom.Size = new System.Drawing.Size(1066, 60);
            this.panelBottom.TabIndex = 4;
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
            this.flRight.Location = new System.Drawing.Point(826, 10);
            this.flRight.Margin = new System.Windows.Forms.Padding(0);
            this.flRight.Name = "flRight";
            this.flRight.Size = new System.Drawing.Size(232, 40);
            this.flRight.TabIndex = 0;
            this.flRight.WrapContents = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(124, 10);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 29);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(8, 10);
            this.btnOk.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 29);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            // 
            // panelTop
            // 
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.panelTop.Size = new System.Drawing.Size(1066, 90);
            this.panelTop.TabIndex = 3;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.panelStatus);
            this.panelRight.Controls.Add(this.grpPT);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(553, 90);
            this.panelRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.panelRight.Size = new System.Drawing.Size(513, 911);
            this.panelRight.TabIndex = 6;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.lblStatus);
            this.panelStatus.Controls.Add(this.progressBar1);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.panelStatus.Location = new System.Drawing.Point(6, 805);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panelStatus.Size = new System.Drawing.Size(501, 98);
            this.panelStatus.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(10, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready.";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 34);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 29);
            this.progressBar1.TabIndex = 1;
            // 
            // grpPT
            // 
            this.grpPT.Controls.Add(this.tabControl1);
            this.grpPT.Controls.Add(this.panel1);
            this.grpPT.Controls.Add(this.rbtnIf1);
            this.grpPT.Controls.Add(this.btnExportCsv);
            this.grpPT.Controls.Add(this.rbtnIf2);
            this.grpPT.Controls.Add(this.btnAbort);
            this.grpPT.Controls.Add(this.btnPT_Measure);
            this.grpPT.Controls.Add(this.lblScanNo);
            this.grpPT.Controls.Add(this.numScanNo);
            this.grpPT.Controls.Add(this.label3);
            this.grpPT.Controls.Add(this.label2);
            this.grpPT.Controls.Add(this.numPowerMeasureY);
            this.grpPT.Controls.Add(this.numPowerMeasureX);
            this.grpPT.Controls.Add(this.btnPT_Build);
            this.grpPT.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPT.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.grpPT.Location = new System.Drawing.Point(6, 8);
            this.grpPT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpPT.Name = "grpPT";
            this.grpPT.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpPT.Size = new System.Drawing.Size(501, 790);
            this.grpPT.TabIndex = 2;
            this.grpPT.TabStop = false;
            this.grpPT.Text = "Power Table";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(14, 101);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(266, 266);
            this.tabControl1.TabIndex = 30;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.numFreq_1);
            this.tabPage1.Controls.Add(this.lblPT_Freq);
            this.tabPage1.Controls.Add(this.numMinW_1);
            this.tabPage1.Controls.Add(this.numRangePct_1);
            this.tabPage1.Controls.Add(this.lblPT_Range);
            this.tabPage1.Controls.Add(this.numAttMax_1);
            this.tabPage1.Controls.Add(this.lblPT_AttMax);
            this.tabPage1.Controls.Add(this.numAttMin_1);
            this.tabPage1.Controls.Add(this.lblPT_AttMin);
            this.tabPage1.Controls.Add(this.numStepW_1);
            this.tabPage1.Controls.Add(this.lblPT_MinW);
            this.tabPage1.Controls.Add(this.lblPT_StepW);
            this.tabPage1.Controls.Add(this.numMaxW_1);
            this.tabPage1.Controls.Add(this.lblPT_MaxW);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(258, 238);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "I/F #1 Param";
            // 
            // numFreq_1
            // 
            this.numFreq_1.DecimalPlaces = 2;
            this.numFreq_1.Location = new System.Drawing.Point(121, 15);
            this.numFreq_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numFreq_1.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numFreq_1.Name = "numFreq_1";
            this.numFreq_1.Size = new System.Drawing.Size(80, 23);
            this.numFreq_1.TabIndex = 21;
            this.numFreq_1.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // lblPT_Freq
            // 
            this.lblPT_Freq.Location = new System.Drawing.Point(30, 19);
            this.lblPT_Freq.Name = "lblPT_Freq";
            this.lblPT_Freq.Size = new System.Drawing.Size(77, 29);
            this.lblPT_Freq.TabIndex = 20;
            this.lblPT_Freq.Text = "Freq(kHz)";
            // 
            // numMinW_1
            // 
            this.numMinW_1.DecimalPlaces = 2;
            this.numMinW_1.Location = new System.Drawing.Point(121, 45);
            this.numMinW_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numMinW_1.Name = "numMinW_1";
            this.numMinW_1.Size = new System.Drawing.Size(80, 23);
            this.numMinW_1.TabIndex = 5;
            this.numMinW_1.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // numRangePct_1
            // 
            this.numRangePct_1.DecimalPlaces = 1;
            this.numRangePct_1.Location = new System.Drawing.Point(121, 195);
            this.numRangePct_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numRangePct_1.Name = "numRangePct_1";
            this.numRangePct_1.Size = new System.Drawing.Size(80, 23);
            this.numRangePct_1.TabIndex = 19;
            this.numRangePct_1.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            // 
            // lblPT_Range
            // 
            this.lblPT_Range.Location = new System.Drawing.Point(30, 199);
            this.lblPT_Range.Name = "lblPT_Range";
            this.lblPT_Range.Size = new System.Drawing.Size(77, 29);
            this.lblPT_Range.TabIndex = 18;
            this.lblPT_Range.Text = "Range(±W)";
            // 
            // numAttMax_1
            // 
            this.numAttMax_1.Location = new System.Drawing.Point(121, 165);
            this.numAttMax_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numAttMax_1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numAttMax_1.Name = "numAttMax_1";
            this.numAttMax_1.Size = new System.Drawing.Size(80, 23);
            this.numAttMax_1.TabIndex = 13;
            this.numAttMax_1.Value = new decimal(new int[] {
            450,
            0,
            0,
            0});
            // 
            // lblPT_AttMax
            // 
            this.lblPT_AttMax.Location = new System.Drawing.Point(30, 169);
            this.lblPT_AttMax.Name = "lblPT_AttMax";
            this.lblPT_AttMax.Size = new System.Drawing.Size(77, 29);
            this.lblPT_AttMax.TabIndex = 12;
            this.lblPT_AttMax.Text = "Att Max";
            // 
            // numAttMin_1
            // 
            this.numAttMin_1.Location = new System.Drawing.Point(121, 135);
            this.numAttMin_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numAttMin_1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numAttMin_1.Name = "numAttMin_1";
            this.numAttMin_1.Size = new System.Drawing.Size(80, 23);
            this.numAttMin_1.TabIndex = 11;
            // 
            // lblPT_AttMin
            // 
            this.lblPT_AttMin.Location = new System.Drawing.Point(30, 139);
            this.lblPT_AttMin.Name = "lblPT_AttMin";
            this.lblPT_AttMin.Size = new System.Drawing.Size(77, 29);
            this.lblPT_AttMin.TabIndex = 10;
            this.lblPT_AttMin.Text = "Att Min";
            // 
            // numStepW_1
            // 
            this.numStepW_1.DecimalPlaces = 2;
            this.numStepW_1.Location = new System.Drawing.Point(121, 105);
            this.numStepW_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numStepW_1.Name = "numStepW_1";
            this.numStepW_1.Size = new System.Drawing.Size(80, 23);
            this.numStepW_1.TabIndex = 9;
            this.numStepW_1.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // lblPT_MinW
            // 
            this.lblPT_MinW.Location = new System.Drawing.Point(30, 49);
            this.lblPT_MinW.Name = "lblPT_MinW";
            this.lblPT_MinW.Size = new System.Drawing.Size(77, 29);
            this.lblPT_MinW.TabIndex = 4;
            this.lblPT_MinW.Text = "Min W";
            // 
            // lblPT_StepW
            // 
            this.lblPT_StepW.Location = new System.Drawing.Point(30, 109);
            this.lblPT_StepW.Name = "lblPT_StepW";
            this.lblPT_StepW.Size = new System.Drawing.Size(77, 29);
            this.lblPT_StepW.TabIndex = 8;
            this.lblPT_StepW.Text = "Step W";
            // 
            // numMaxW_1
            // 
            this.numMaxW_1.DecimalPlaces = 2;
            this.numMaxW_1.Location = new System.Drawing.Point(121, 75);
            this.numMaxW_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numMaxW_1.Name = "numMaxW_1";
            this.numMaxW_1.Size = new System.Drawing.Size(80, 23);
            this.numMaxW_1.TabIndex = 7;
            this.numMaxW_1.Value = new decimal(new int[] {
            1200,
            0,
            0,
            131072});
            // 
            // lblPT_MaxW
            // 
            this.lblPT_MaxW.Location = new System.Drawing.Point(30, 79);
            this.lblPT_MaxW.Name = "lblPT_MaxW";
            this.lblPT_MaxW.Size = new System.Drawing.Size(77, 29);
            this.lblPT_MaxW.TabIndex = 6;
            this.lblPT_MaxW.Text = "Max W";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.numFreq_2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.numAttPos_2);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(258, 238);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "I/F #2 Param";
            // 
            // numFreq_2
            // 
            this.numFreq_2.DecimalPlaces = 2;
            this.numFreq_2.Location = new System.Drawing.Point(127, 15);
            this.numFreq_2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numFreq_2.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numFreq_2.Name = "numFreq_2";
            this.numFreq_2.Size = new System.Drawing.Size(80, 23);
            this.numFreq_2.TabIndex = 23;
            this.numFreq_2.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(32, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 29);
            this.label5.TabIndex = 22;
            this.label5.Text = "Freq(kHz)";
            // 
            // numAttPos_2
            // 
            this.numAttPos_2.Location = new System.Drawing.Point(127, 45);
            this.numAttPos_2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numAttPos_2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numAttPos_2.Name = "numAttPos_2";
            this.numAttPos_2.Size = new System.Drawing.Size(80, 23);
            this.numAttPos_2.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(32, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 29);
            this.label4.TabIndex = 12;
            this.label4.Text = "Att Position";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbRecipeName);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.lstPowerRecipeList);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblRecipe_PowerList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 435);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(495, 351);
            this.panel1.TabIndex = 29;
            // 
            // tbRecipeName
            // 
            this.tbRecipeName.Location = new System.Drawing.Point(11, 40);
            this.tbRecipeName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbRecipeName.Name = "tbRecipeName";
            this.tbRecipeName.Size = new System.Drawing.Size(242, 23);
            this.tbRecipeName.TabIndex = 36;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(264, 72);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 29);
            this.btnDelete.TabIndex = 34;
            this.btnDelete.Text = "DELETE";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(264, 38);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 29);
            this.btnAdd.TabIndex = 35;
            this.btnAdd.Text = "ADD";
            // 
            // lstPowerRecipeList
            // 
            this.lstPowerRecipeList.HorizontalScrollbar = true;
            this.lstPowerRecipeList.ItemHeight = 15;
            this.lstPowerRecipeList.Location = new System.Drawing.Point(7, 136);
            this.lstPowerRecipeList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstPowerRecipeList.Name = "lstPowerRecipeList";
            this.lstPowerRecipeList.Size = new System.Drawing.Size(481, 199);
            this.lstPowerRecipeList.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 29);
            this.label1.TabIndex = 32;
            this.label1.Text = "New Recipe Name";
            // 
            // lblRecipe_PowerList
            // 
            this.lblRecipe_PowerList.Location = new System.Drawing.Point(7, 104);
            this.lblRecipe_PowerList.Name = "lblRecipe_PowerList";
            this.lblRecipe_PowerList.Size = new System.Drawing.Size(168, 29);
            this.lblRecipe_PowerList.TabIndex = 32;
            this.lblRecipe_PowerList.Text = "PowerMeter Recipe List";
            // 
            // rbtnIf1
            // 
            this.rbtnIf1.Checked = true;
            this.rbtnIf1.Location = new System.Drawing.Point(13, 30);
            this.rbtnIf1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnIf1.Name = "rbtnIf1";
            this.rbtnIf1.Size = new System.Drawing.Size(104, 30);
            this.rbtnIf1.TabIndex = 27;
            this.rbtnIf1.TabStop = true;
            this.rbtnIf1.Text = "Interface #1";
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(297, 315);
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(137, 48);
            this.btnExportCsv.TabIndex = 4;
            this.btnExportCsv.Text = "Export CSV";
            // 
            // rbtnIf2
            // 
            this.rbtnIf2.Location = new System.Drawing.Point(148, 30);
            this.rbtnIf2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtnIf2.Name = "rbtnIf2";
            this.rbtnIf2.Size = new System.Drawing.Size(104, 30);
            this.rbtnIf2.TabIndex = 28;
            this.rbtnIf2.Text = "Interface #2";
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(297, 237);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(137, 48);
            this.btnAbort.TabIndex = 26;
            this.btnAbort.Text = "Measure Abort";
            // 
            // btnPT_Measure
            // 
            this.btnPT_Measure.Location = new System.Drawing.Point(297, 181);
            this.btnPT_Measure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPT_Measure.Name = "btnPT_Measure";
            this.btnPT_Measure.Size = new System.Drawing.Size(137, 48);
            this.btnPT_Measure.TabIndex = 26;
            this.btnPT_Measure.Text = "PowerMeter Measure";
            // 
            // lblScanNo
            // 
            this.lblScanNo.Location = new System.Drawing.Point(273, 38);
            this.lblScanNo.Name = "lblScanNo";
            this.lblScanNo.Size = new System.Drawing.Size(77, 29);
            this.lblScanNo.TabIndex = 0;
            this.lblScanNo.Text = "Scan No";
            // 
            // numScanNo
            // 
            this.numScanNo.Location = new System.Drawing.Point(354, 34);
            this.numScanNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numScanNo.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numScanNo.Name = "numScanNo";
            this.numScanNo.Size = new System.Drawing.Size(80, 23);
            this.numScanNo.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(264, 389);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 29);
            this.label3.TabIndex = 18;
            this.label3.Text = "Y";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(36, 390);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 29);
            this.label2.TabIndex = 18;
            this.label2.Text = "Power Measure X";
            // 
            // numPowerMeasureY
            // 
            this.numPowerMeasureY.DecimalPlaces = 1;
            this.numPowerMeasureY.Location = new System.Drawing.Point(288, 386);
            this.numPowerMeasureY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numPowerMeasureY.Maximum = new decimal(new int[] {
            2300,
            0,
            0,
            0});
            this.numPowerMeasureY.Name = "numPowerMeasureY";
            this.numPowerMeasureY.Size = new System.Drawing.Size(80, 23);
            this.numPowerMeasureY.TabIndex = 19;
            this.numPowerMeasureY.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            // 
            // numPowerMeasureX
            // 
            this.numPowerMeasureX.DecimalPlaces = 1;
            this.numPowerMeasureX.Location = new System.Drawing.Point(153, 386);
            this.numPowerMeasureX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numPowerMeasureX.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numPowerMeasureX.Name = "numPowerMeasureX";
            this.numPowerMeasureX.Size = new System.Drawing.Size(80, 23);
            this.numPowerMeasureX.TabIndex = 19;
            this.numPowerMeasureX.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            // 
            // btnPT_Build
            // 
            this.btnPT_Build.Location = new System.Drawing.Point(297, 125);
            this.btnPT_Build.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPT_Build.Name = "btnPT_Build";
            this.btnPT_Build.Size = new System.Drawing.Size(137, 48);
            this.btnPT_Build.TabIndex = 20;
            this.btnPT_Build.Text = "Make Table";
            // 
            // PowerMeterEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1066, 1061);
            this.Controls.Add(this._gridPower);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PowerMeterEditorForm";
            this.Text = "PowerMeterEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this._gridPower)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.flRight.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.grpPT.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numFreq_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinW_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangePct_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttMax_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttMin_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepW_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxW_1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numFreq_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttPos_2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScanNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerMeasureY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerMeasureX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _gridPower;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.FlowLayoutPanel flRight;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panelTop;

        // 우측 패널 + 내부 컨트롤
        private System.Windows.Forms.Panel panelRight;

        private System.Windows.Forms.GroupBox grpPT;
        private System.Windows.Forms.Label lblScanNo;
        private System.Windows.Forms.NumericUpDown numScanNo;
        private System.Windows.Forms.Label lblPT_MinW;
        private System.Windows.Forms.NumericUpDown numMinW_1;
        private System.Windows.Forms.Label lblPT_MaxW;
        private System.Windows.Forms.NumericUpDown numMaxW_1;
        private System.Windows.Forms.Label lblPT_StepW;
        private System.Windows.Forms.NumericUpDown numStepW_1;
        private System.Windows.Forms.Label lblPT_AttMin;
        private System.Windows.Forms.NumericUpDown numAttMin_1;
        private System.Windows.Forms.Label lblPT_AttMax;
        private System.Windows.Forms.NumericUpDown numAttMax_1;
        private System.Windows.Forms.Label lblPT_Range;
        private System.Windows.Forms.NumericUpDown numRangePct_1;
        private System.Windows.Forms.Button btnPT_Build;
        private System.Windows.Forms.Button btnExportCsv;

        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnPT_Measure;
        private System.Windows.Forms.RadioButton rbtnIf1;
        private System.Windows.Forms.RadioButton rbtnIf2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbRecipeName;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstPowerRecipeList;
        private System.Windows.Forms.Label lblRecipe_PowerList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPowerMeasureY;
        private System.Windows.Forms.NumericUpDown numPowerMeasureX;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown numAttPos_2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numFreq_1;
        private System.Windows.Forms.Label lblPT_Freq;
        private System.Windows.Forms.NumericUpDown numFreq_2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAbort;
    }
}