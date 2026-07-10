using System.Windows.Forms;
using System.Drawing;
using StageWin.Etc;

namespace StageWin.UI
{
    partial class OpticOperationForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlLaser = new System.Windows.Forms.Panel();
            this.lblLaserTitle = new System.Windows.Forms.Label();
            this.lblPower = new System.Windows.Forms.Label();
            this.numPower = new System.Windows.Forms.NumericUpDown();
            this.lblAtt = new System.Windows.Forms.Label();
            this.numAtt = new System.Windows.Forms.NumericUpDown();
            this.btnApplyLaser = new System.Windows.Forms.Button();
            this.btnLaserOn = new System.Windows.Forms.Button();
            this.btnLaserOff = new System.Windows.Forms.Button();
            this.lblRecipeCaption = new System.Windows.Forms.Label();
            this.lblRecipeValue = new System.Windows.Forms.Label();
            this.lblOriginCaption = new System.Windows.Forms.Label();
            this.lblOriginValue = new System.Windows.Forms.Label();
            this.lblEmissionCaption = new System.Windows.Forms.Label();
            this.ledEmission = new System.Windows.Forms.Label();
            this.lblEmissionState = new System.Windows.Forms.Label();
            this.lblScanNo = new System.Windows.Forms.Label();
            this.numScanNo = new System.Windows.Forms.NumericUpDown();
            this.lblTime = new System.Windows.Forms.Label();
            this.numTimeMs = new System.Windows.Forms.NumericUpDown();
            this.lblFreq = new System.Windows.Forms.Label();
            this.numFreq = new System.Windows.Forms.NumericUpDown();
            this.lblXOfs = new System.Windows.Forms.Label();
            this.numXOfs = new System.Windows.Forms.NumericUpDown();
            this.lblYOfs = new System.Windows.Forms.Label();
            this.numYOfs = new System.Windows.Forms.NumericUpDown();
            this._btnProcessScanning = new System.Windows.Forms.Button();
            this.btnMoveScan = new System.Windows.Forms.Button();
            this.gridProcess = new System.Windows.Forms.DataGridView();
            this._btnProcessStop = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.ProcessScanXSpeed = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.ProcessScanYSpeed = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbtnXProcess = new System.Windows.Forms.RadioButton();
            this.rbtnYProcess = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnForwardProcess = new System.Windows.Forms.RadioButton();
            this.rbtnBackwardProcess = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtnOneWayProcess = new System.Windows.Forms.RadioButton();
            this.rbtnSnakeProcess = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.numFlyProcessLines = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnProcessStepAllWholeLine = new System.Windows.Forms.RadioButton();
            this.rbtnProcessStepWholeLine = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.numProcessStepCount = new System.Windows.Forms.NumericUpDown();
            this.gbProcessType = new System.Windows.Forms.GroupBox();
            this.rbtnProcessStepbyStep = new System.Windows.Forms.RadioButton();
            this.rbtnProcessOntheFly = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.grpMotionData = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStageXSpeed = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtStageYSpeed = new System.Windows.Forms.TextBox();
            this.txtTPos = new System.Windows.Forms.TextBox();
            this.txtZPos = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblRPos = new System.Windows.Forms.Label();
            this.txtRPos = new System.Windows.Forms.TextBox();
            this.lblMPos = new System.Windows.Forms.Label();
            this.txtMPos = new System.Windows.Forms.TextBox();
            this.rbtnProcessStepHoles = new System.Windows.Forms.RadioButton();
            this.pnlLaser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAtt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXOfs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYOfs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessScanXSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessScanYSpeed)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlyProcessLines)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessStepCount)).BeginInit();
            this.gbProcessType.SuspendLayout();
            this.grpMotionData.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLaser
            // 
            this.pnlLaser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLaser.Controls.Add(this.lblLaserTitle);
            this.pnlLaser.Controls.Add(this.lblPower);
            this.pnlLaser.Controls.Add(this.numPower);
            this.pnlLaser.Controls.Add(this.lblAtt);
            this.pnlLaser.Controls.Add(this.numAtt);
            this.pnlLaser.Controls.Add(this.btnApplyLaser);
            this.pnlLaser.Controls.Add(this.btnLaserOn);
            this.pnlLaser.Controls.Add(this.btnLaserOff);
            this.pnlLaser.Controls.Add(this.lblRecipeCaption);
            this.pnlLaser.Controls.Add(this.lblRecipeValue);
            this.pnlLaser.Controls.Add(this.lblOriginCaption);
            this.pnlLaser.Controls.Add(this.lblOriginValue);
            this.pnlLaser.Controls.Add(this.lblEmissionCaption);
            this.pnlLaser.Controls.Add(this.ledEmission);
            this.pnlLaser.Controls.Add(this.lblEmissionState);
            this.pnlLaser.Controls.Add(this.lblScanNo);
            this.pnlLaser.Controls.Add(this.numScanNo);
            this.pnlLaser.Controls.Add(this.lblTime);
            this.pnlLaser.Controls.Add(this.numTimeMs);
            this.pnlLaser.Controls.Add(this.lblFreq);
            this.pnlLaser.Controls.Add(this.numFreq);
            this.pnlLaser.Controls.Add(this.lblXOfs);
            this.pnlLaser.Controls.Add(this.numXOfs);
            this.pnlLaser.Controls.Add(this.lblYOfs);
            this.pnlLaser.Controls.Add(this.numYOfs);
            this.pnlLaser.Location = new System.Drawing.Point(10, 10);
            this.pnlLaser.Name = "pnlLaser";
            this.pnlLaser.Size = new System.Drawing.Size(545, 294);
            this.pnlLaser.TabIndex = 0;
            // 
            // lblLaserTitle
            // 
            this.lblLaserTitle.AutoSize = true;
            this.lblLaserTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblLaserTitle.Location = new System.Drawing.Point(10, 2);
            this.lblLaserTitle.Name = "lblLaserTitle";
            this.lblLaserTitle.Size = new System.Drawing.Size(84, 15);
            this.lblLaserTitle.TabIndex = 0;
            this.lblLaserTitle.Text = "Laser Control";
            // 
            // lblPower
            // 
            this.lblPower.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblPower.Location = new System.Drawing.Point(10, 63);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(48, 23);
            this.lblPower.TabIndex = 1;
            this.lblPower.Text = "Power";
            // 
            // numPower
            // 
            this.numPower.DecimalPlaces = 2;
            this.numPower.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            this.numPower.Location = new System.Drawing.Point(70, 61);
            this.numPower.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numPower.Name = "numPower";
            this.numPower.Size = new System.Drawing.Size(90, 23);
            this.numPower.TabIndex = 2;
            // 
            // lblAtt
            // 
            this.lblAtt.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblAtt.Location = new System.Drawing.Point(180, 63);
            this.lblAtt.Name = "lblAtt";
            this.lblAtt.Size = new System.Drawing.Size(68, 23);
            this.lblAtt.TabIndex = 3;
            this.lblAtt.Text = "Attenuator";
            // 
            // numAtt
            // 
            this.numAtt.DecimalPlaces = 2;
            this.numAtt.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numAtt.Increment = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            this.numAtt.Location = new System.Drawing.Point(256, 61);
            this.numAtt.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAtt.Name = "numAtt";
            this.numAtt.Size = new System.Drawing.Size(90, 23);
            this.numAtt.TabIndex = 4;
            // 
            // btnApplyLaser
            // 
            this.btnApplyLaser.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnApplyLaser.Location = new System.Drawing.Point(360, 59);
            this.btnApplyLaser.Name = "btnApplyLaser";
            this.btnApplyLaser.Size = new System.Drawing.Size(80, 26);
            this.btnApplyLaser.TabIndex = 5;
            this.btnApplyLaser.Text = "Apply";
            // 
            // btnLaserOn
            // 
            this.btnLaserOn.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnLaserOn.Location = new System.Drawing.Point(80, 195);
            this.btnLaserOn.Name = "btnLaserOn";
            this.btnLaserOn.Size = new System.Drawing.Size(150, 28);
            this.btnLaserOn.TabIndex = 6;
            this.btnLaserOn.Text = "Laser ON";
            // 
            // btnLaserOff
            // 
            this.btnLaserOff.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnLaserOff.Location = new System.Drawing.Point(240, 195);
            this.btnLaserOff.Name = "btnLaserOff";
            this.btnLaserOff.Size = new System.Drawing.Size(150, 28);
            this.btnLaserOff.TabIndex = 7;
            this.btnLaserOff.Text = "Laser OFF";
            // 
            // lblRecipeCaption
            // 
            this.lblRecipeCaption.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblRecipeCaption.Location = new System.Drawing.Point(11, 34);
            this.lblRecipeCaption.Name = "lblRecipeCaption";
            this.lblRecipeCaption.Size = new System.Drawing.Size(56, 21);
            this.lblRecipeCaption.TabIndex = 8;
            this.lblRecipeCaption.Text = "Recipe";
            // 
            // lblRecipeValue
            // 
            this.lblRecipeValue.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblRecipeValue.Location = new System.Drawing.Point(73, 34);
            this.lblRecipeValue.Name = "lblRecipeValue";
            this.lblRecipeValue.Size = new System.Drawing.Size(220, 21);
            this.lblRecipeValue.TabIndex = 9;
            this.lblRecipeValue.Text = "-";
            // 
            // lblOriginCaption
            // 
            this.lblOriginCaption.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblOriginCaption.Location = new System.Drawing.Point(320, 34);
            this.lblOriginCaption.Name = "lblOriginCaption";
            this.lblOriginCaption.Size = new System.Drawing.Size(44, 21);
            this.lblOriginCaption.TabIndex = 10;
            this.lblOriginCaption.Text = "Origin";
            // 
            // lblOriginValue
            // 
            this.lblOriginValue.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblOriginValue.Location = new System.Drawing.Point(370, 34);
            this.lblOriginValue.Name = "lblOriginValue";
            this.lblOriginValue.Size = new System.Drawing.Size(80, 21);
            this.lblOriginValue.TabIndex = 11;
            this.lblOriginValue.Text = "Local";
            // 
            // lblEmissionCaption
            // 
            this.lblEmissionCaption.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblEmissionCaption.Location = new System.Drawing.Point(20, 245);
            this.lblEmissionCaption.Name = "lblEmissionCaption";
            this.lblEmissionCaption.Size = new System.Drawing.Size(64, 21);
            this.lblEmissionCaption.TabIndex = 12;
            this.lblEmissionCaption.Text = "Emission";
            // 
            // ledEmission
            // 
            this.ledEmission.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledEmission.Location = new System.Drawing.Point(92, 240);
            this.ledEmission.Name = "ledEmission";
            this.ledEmission.Size = new System.Drawing.Size(18, 18);
            this.ledEmission.TabIndex = 13;
            // 
            // lblEmissionState
            // 
            this.lblEmissionState.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblEmissionState.Location = new System.Drawing.Point(116, 244);
            this.lblEmissionState.Name = "lblEmissionState";
            this.lblEmissionState.Size = new System.Drawing.Size(64, 21);
            this.lblEmissionState.TabIndex = 14;
            this.lblEmissionState.Text = "OFF";
            // 
            // lblScanNo
            // 
            this.lblScanNo.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblScanNo.Location = new System.Drawing.Point(10, 128);
            this.lblScanNo.Name = "lblScanNo";
            this.lblScanNo.Size = new System.Drawing.Size(60, 23);
            this.lblScanNo.TabIndex = 15;
            this.lblScanNo.Text = "ScanNo";
            // 
            // numScanNo
            // 
            this.numScanNo.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numScanNo.Location = new System.Drawing.Point(70, 126);
            this.numScanNo.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numScanNo.Name = "numScanNo";
            this.numScanNo.Size = new System.Drawing.Size(80, 23);
            this.numScanNo.TabIndex = 16;
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblTime.Location = new System.Drawing.Point(160, 128);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(70, 23);
            this.lblTime.TabIndex = 17;
            this.lblTime.Text = "Time(ms)";
            // 
            // numTimeMs
            // 
            this.numTimeMs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numTimeMs.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numTimeMs.Location = new System.Drawing.Point(230, 126);
            this.numTimeMs.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.numTimeMs.Name = "numTimeMs";
            this.numTimeMs.Size = new System.Drawing.Size(90, 23);
            this.numTimeMs.TabIndex = 18;
            // 
            // lblFreq
            // 
            this.lblFreq.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblFreq.Location = new System.Drawing.Point(335, 128);
            this.lblFreq.Name = "lblFreq";
            this.lblFreq.Size = new System.Drawing.Size(60, 23);
            this.lblFreq.TabIndex = 19;
            this.lblFreq.Text = "Freq(Hz)";
            // 
            // numFreq
            // 
            this.numFreq.DecimalPlaces = 1;
            this.numFreq.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numFreq.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numFreq.Location = new System.Drawing.Point(395, 126);
            this.numFreq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numFreq.Name = "numFreq";
            this.numFreq.Size = new System.Drawing.Size(90, 23);
            this.numFreq.TabIndex = 20;
            // 
            // lblXOfs
            // 
            this.lblXOfs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblXOfs.Location = new System.Drawing.Point(30, 155);
            this.lblXOfs.Name = "lblXOfs";
            this.lblXOfs.Size = new System.Drawing.Size(32, 23);
            this.lblXOfs.TabIndex = 21;
            this.lblXOfs.Text = "dX";
            // 
            // numXOfs
            // 
            this.numXOfs.DecimalPlaces = 3;
            this.numXOfs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numXOfs.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numXOfs.Location = new System.Drawing.Point(70, 153);
            this.numXOfs.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numXOfs.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numXOfs.Name = "numXOfs";
            this.numXOfs.Size = new System.Drawing.Size(100, 23);
            this.numXOfs.TabIndex = 22;
            // 
            // lblYOfs
            // 
            this.lblYOfs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblYOfs.Location = new System.Drawing.Point(190, 155);
            this.lblYOfs.Name = "lblYOfs";
            this.lblYOfs.Size = new System.Drawing.Size(32, 23);
            this.lblYOfs.TabIndex = 23;
            this.lblYOfs.Text = "dY";
            // 
            // numYOfs
            // 
            this.numYOfs.DecimalPlaces = 3;
            this.numYOfs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numYOfs.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numYOfs.Location = new System.Drawing.Point(230, 153);
            this.numYOfs.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numYOfs.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numYOfs.Name = "numYOfs";
            this.numYOfs.Size = new System.Drawing.Size(100, 23);
            this.numYOfs.TabIndex = 24;
            // 
            // _btnProcessScanning
            // 
            this._btnProcessScanning.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this._btnProcessScanning.Location = new System.Drawing.Point(499, 586);
            this._btnProcessScanning.Name = "_btnProcessScanning";
            this._btnProcessScanning.Size = new System.Drawing.Size(204, 49);
            this._btnProcessScanning.TabIndex = 2;
            this._btnProcessScanning.Text = "ProcessScanning";
            this._btnProcessScanning.UseVisualStyleBackColor = true;
            // 
            // btnMoveScan
            // 
            this.btnMoveScan.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnMoveScan.Location = new System.Drawing.Point(4, 25);
            this.btnMoveScan.Name = "btnMoveScan";
            this.btnMoveScan.Size = new System.Drawing.Size(920, 24);
            this.btnMoveScan.TabIndex = 5;
            this.btnMoveScan.Text = "Move To Selected";
            // 
            // gridProcess
            // 
            this.gridProcess.AllowUserToAddRows = false;
            this.gridProcess.AllowUserToDeleteRows = false;
            this.gridProcess.AllowUserToResizeColumns = false;
            this.gridProcess.AllowUserToResizeRows = false;
            this.gridProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridProcess.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridProcess.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridProcess.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Format = "N3";
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridProcess.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridProcess.Location = new System.Drawing.Point(4, 55);
            this.gridProcess.MultiSelect = false;
            this.gridProcess.Name = "gridProcess";
            this.gridProcess.ReadOnly = true;
            this.gridProcess.RowHeadersVisible = false;
            this.gridProcess.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridProcess.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridProcess.Size = new System.Drawing.Size(920, 434);
            this.gridProcess.TabIndex = 6;
            // 
            // _btnProcessStop
            // 
            this._btnProcessStop.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this._btnProcessStop.Location = new System.Drawing.Point(709, 586);
            this._btnProcessStop.Name = "_btnProcessStop";
            this._btnProcessStop.Size = new System.Drawing.Size(204, 49);
            this._btnProcessStop.TabIndex = 7;
            this._btnProcessStop.Text = "Process Stop";
            this._btnProcessStop.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox6);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.gbProcessType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnMoveScan);
            this.panel1.Controls.Add(this._btnProcessStop);
            this.panel1.Controls.Add(this._btnProcessScanning);
            this.panel1.Controls.Add(this.gridProcess);
            this.panel1.Location = new System.Drawing.Point(561, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(942, 677);
            this.panel1.TabIndex = 8;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.ProcessScanXSpeed);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.ProcessScanYSpeed);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.groupBox6.Location = new System.Drawing.Point(499, 511);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(426, 60);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Scan Speed";
            // 
            // ProcessScanXSpeed
            // 
            this.ProcessScanXSpeed.Location = new System.Drawing.Point(261, 21);
            this.ProcessScanXSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ProcessScanXSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ProcessScanXSpeed.Name = "ProcessScanXSpeed";
            this.ProcessScanXSpeed.Size = new System.Drawing.Size(63, 23);
            this.ProcessScanXSpeed.TabIndex = 14;
            this.ProcessScanXSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(33, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Y Speed";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ProcessScanYSpeed
            // 
            this.ProcessScanYSpeed.Location = new System.Drawing.Point(104, 22);
            this.ProcessScanYSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ProcessScanYSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ProcessScanYSpeed.Name = "ProcessScanYSpeed";
            this.ProcessScanYSpeed.Size = new System.Drawing.Size(63, 23);
            this.ProcessScanYSpeed.TabIndex = 4;
            this.ProcessScanYSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(190, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "X Speed";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numFlyProcessLines);
            this.groupBox2.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.groupBox2.Location = new System.Drawing.Point(232, 511);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(261, 158);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "On the Fly Condition";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbtnXProcess);
            this.groupBox5.Controls.Add(this.rbtnYProcess);
            this.groupBox5.Location = new System.Drawing.Point(22, 50);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(213, 32);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            // 
            // rbtnXProcess
            // 
            this.rbtnXProcess.AutoSize = true;
            this.rbtnXProcess.Location = new System.Drawing.Point(14, 11);
            this.rbtnXProcess.Name = "rbtnXProcess";
            this.rbtnXProcess.Size = new System.Drawing.Size(32, 19);
            this.rbtnXProcess.TabIndex = 11;
            this.rbtnXProcess.Text = "X";
            this.rbtnXProcess.UseVisualStyleBackColor = true;
            // 
            // rbtnYProcess
            // 
            this.rbtnYProcess.AutoSize = true;
            this.rbtnYProcess.Checked = true;
            this.rbtnYProcess.Location = new System.Drawing.Point(98, 11);
            this.rbtnYProcess.Name = "rbtnYProcess";
            this.rbtnYProcess.Size = new System.Drawing.Size(32, 19);
            this.rbtnYProcess.TabIndex = 12;
            this.rbtnYProcess.TabStop = true;
            this.rbtnYProcess.Text = "Y";
            this.rbtnYProcess.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbtnForwardProcess);
            this.groupBox4.Controls.Add(this.rbtnBackwardProcess);
            this.groupBox4.Location = new System.Drawing.Point(22, 86);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(213, 32);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            // 
            // rbtnForwardProcess
            // 
            this.rbtnForwardProcess.AutoSize = true;
            this.rbtnForwardProcess.Location = new System.Drawing.Point(14, 11);
            this.rbtnForwardProcess.Name = "rbtnForwardProcess";
            this.rbtnForwardProcess.Size = new System.Drawing.Size(68, 19);
            this.rbtnForwardProcess.TabIndex = 11;
            this.rbtnForwardProcess.Text = "Forward";
            this.rbtnForwardProcess.UseVisualStyleBackColor = true;
            // 
            // rbtnBackwardProcess
            // 
            this.rbtnBackwardProcess.AutoSize = true;
            this.rbtnBackwardProcess.Checked = true;
            this.rbtnBackwardProcess.Location = new System.Drawing.Point(98, 11);
            this.rbtnBackwardProcess.Name = "rbtnBackwardProcess";
            this.rbtnBackwardProcess.Size = new System.Drawing.Size(76, 19);
            this.rbtnBackwardProcess.TabIndex = 12;
            this.rbtnBackwardProcess.TabStop = true;
            this.rbtnBackwardProcess.Text = "Backward";
            this.rbtnBackwardProcess.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbtnOneWayProcess);
            this.groupBox3.Controls.Add(this.rbtnSnakeProcess);
            this.groupBox3.Location = new System.Drawing.Point(22, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 32);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            // 
            // rbtnOneWayProcess
            // 
            this.rbtnOneWayProcess.AutoSize = true;
            this.rbtnOneWayProcess.Checked = true;
            this.rbtnOneWayProcess.Location = new System.Drawing.Point(14, 11);
            this.rbtnOneWayProcess.Name = "rbtnOneWayProcess";
            this.rbtnOneWayProcess.Size = new System.Drawing.Size(70, 19);
            this.rbtnOneWayProcess.TabIndex = 11;
            this.rbtnOneWayProcess.TabStop = true;
            this.rbtnOneWayProcess.Text = "OneWay";
            this.rbtnOneWayProcess.UseVisualStyleBackColor = true;
            // 
            // rbtnSnakeProcess
            // 
            this.rbtnSnakeProcess.AutoSize = true;
            this.rbtnSnakeProcess.Location = new System.Drawing.Point(98, 11);
            this.rbtnSnakeProcess.Name = "rbtnSnakeProcess";
            this.rbtnSnakeProcess.Size = new System.Drawing.Size(57, 19);
            this.rbtnSnakeProcess.TabIndex = 12;
            this.rbtnSnakeProcess.Text = "Snake";
            this.rbtnSnakeProcess.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Line Count";
            // 
            // numFlyProcessLines
            // 
            this.numFlyProcessLines.Location = new System.Drawing.Point(90, 25);
            this.numFlyProcessLines.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFlyProcessLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFlyProcessLines.Name = "numFlyProcessLines";
            this.numFlyProcessLines.Size = new System.Drawing.Size(63, 23);
            this.numFlyProcessLines.TabIndex = 3;
            this.numFlyProcessLines.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnProcessStepHoles);
            this.groupBox1.Controls.Add(this.rbtnProcessStepAllWholeLine);
            this.groupBox1.Controls.Add(this.rbtnProcessStepWholeLine);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numProcessStepCount);
            this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.groupBox1.Location = new System.Drawing.Point(5, 574);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 95);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Step by Step Condition";
            // 
            // rbtnProcessStepAllWholeLine
            // 
            this.rbtnProcessStepAllWholeLine.AutoSize = true;
            this.rbtnProcessStepAllWholeLine.Location = new System.Drawing.Point(111, 66);
            this.rbtnProcessStepAllWholeLine.Name = "rbtnProcessStepAllWholeLine";
            this.rbtnProcessStepAllWholeLine.Size = new System.Drawing.Size(94, 19);
            this.rbtnProcessStepAllWholeLine.TabIndex = 11;
            this.rbtnProcessStepAllWholeLine.Text = "All Cell Lines";
            this.rbtnProcessStepAllWholeLine.UseVisualStyleBackColor = true;
            // 
            // rbtnProcessStepWholeLine
            // 
            this.rbtnProcessStepWholeLine.AutoSize = true;
            this.rbtnProcessStepWholeLine.Location = new System.Drawing.Point(22, 66);
            this.rbtnProcessStepWholeLine.Name = "rbtnProcessStepWholeLine";
            this.rbtnProcessStepWholeLine.Size = new System.Drawing.Size(73, 19);
            this.rbtnProcessStepWholeLine.TabIndex = 11;
            this.rbtnProcessStepWholeLine.Text = "All Holes";
            this.rbtnProcessStepWholeLine.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Step Count";
            // 
            // numProcessStepCount
            // 
            this.numProcessStepCount.Location = new System.Drawing.Point(88, 36);
            this.numProcessStepCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numProcessStepCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numProcessStepCount.Name = "numProcessStepCount";
            this.numProcessStepCount.Size = new System.Drawing.Size(63, 23);
            this.numProcessStepCount.TabIndex = 0;
            this.numProcessStepCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbProcessType
            // 
            this.gbProcessType.Controls.Add(this.rbtnProcessStepbyStep);
            this.gbProcessType.Controls.Add(this.rbtnProcessOntheFly);
            this.gbProcessType.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gbProcessType.Location = new System.Drawing.Point(5, 511);
            this.gbProcessType.Name = "gbProcessType";
            this.gbProcessType.Size = new System.Drawing.Size(221, 60);
            this.gbProcessType.TabIndex = 11;
            this.gbProcessType.TabStop = false;
            this.gbProcessType.Text = "Process Type";
            // 
            // rbtnProcessStepbyStep
            // 
            this.rbtnProcessStepbyStep.AutoSize = true;
            this.rbtnProcessStepbyStep.Location = new System.Drawing.Point(18, 26);
            this.rbtnProcessStepbyStep.Name = "rbtnProcessStepbyStep";
            this.rbtnProcessStepbyStep.Size = new System.Drawing.Size(49, 19);
            this.rbtnProcessStepbyStep.TabIndex = 9;
            this.rbtnProcessStepbyStep.Text = "Step";
            this.rbtnProcessStepbyStep.UseVisualStyleBackColor = true;
            // 
            // rbtnProcessOntheFly
            // 
            this.rbtnProcessOntheFly.AutoSize = true;
            this.rbtnProcessOntheFly.Checked = true;
            this.rbtnProcessOntheFly.Location = new System.Drawing.Point(97, 26);
            this.rbtnProcessOntheFly.Name = "rbtnProcessOntheFly";
            this.rbtnProcessOntheFly.Size = new System.Drawing.Size(81, 19);
            this.rbtnProcessOntheFly.TabIndex = 10;
            this.rbtnProcessOntheFly.TabStop = true;
            this.rbtnProcessOntheFly.Text = "On the Fly";
            this.rbtnProcessOntheFly.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(14, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Manual Process Scanning";
            // 
            // grpMotionData
            // 
            this.grpMotionData.Controls.Add(this.label8);
            this.grpMotionData.Controls.Add(this.txtStageXSpeed);
            this.grpMotionData.Controls.Add(this.label9);
            this.grpMotionData.Controls.Add(this.txtStageYSpeed);
            this.grpMotionData.Controls.Add(this.txtTPos);
            this.grpMotionData.Controls.Add(this.txtZPos);
            this.grpMotionData.Controls.Add(this.label5);
            this.grpMotionData.Controls.Add(this.label4);
            this.grpMotionData.Controls.Add(this.lblRPos);
            this.grpMotionData.Controls.Add(this.txtRPos);
            this.grpMotionData.Controls.Add(this.lblMPos);
            this.grpMotionData.Controls.Add(this.txtMPos);
            this.grpMotionData.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.grpMotionData.Location = new System.Drawing.Point(12, 310);
            this.grpMotionData.Name = "grpMotionData";
            this.grpMotionData.Size = new System.Drawing.Size(405, 146);
            this.grpMotionData.TabIndex = 30;
            this.grpMotionData.TabStop = false;
            this.grpMotionData.Text = "Motion Display";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(208, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 23);
            this.label8.TabIndex = 21;
            this.label8.Text = "Stage X Speed";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStageXSpeed
            // 
            this.txtStageXSpeed.Location = new System.Drawing.Point(304, 57);
            this.txtStageXSpeed.Name = "txtStageXSpeed";
            this.txtStageXSpeed.ReadOnly = true;
            this.txtStageXSpeed.Size = new System.Drawing.Size(85, 23);
            this.txtStageXSpeed.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(208, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 23);
            this.label9.TabIndex = 19;
            this.label9.Text = "Stage Y Speed";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStageYSpeed
            // 
            this.txtStageYSpeed.Location = new System.Drawing.Point(304, 28);
            this.txtStageYSpeed.Name = "txtStageYSpeed";
            this.txtStageYSpeed.ReadOnly = true;
            this.txtStageYSpeed.Size = new System.Drawing.Size(85, 23);
            this.txtStageYSpeed.TabIndex = 20;
            // 
            // txtTPos
            // 
            this.txtTPos.Location = new System.Drawing.Point(103, 116);
            this.txtTPos.Name = "txtTPos";
            this.txtTPos.ReadOnly = true;
            this.txtTPos.Size = new System.Drawing.Size(85, 23);
            this.txtTPos.TabIndex = 18;
            // 
            // txtZPos
            // 
            this.txtZPos.Location = new System.Drawing.Point(103, 89);
            this.txtZPos.Name = "txtZPos";
            this.txtZPos.ReadOnly = true;
            this.txtZPos.Size = new System.Drawing.Size(85, 23);
            this.txtZPos.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 23);
            this.label5.TabIndex = 16;
            this.label5.Text = "Theta Pos";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 23);
            this.label4.TabIndex = 15;
            this.label4.Text = "Z Pos";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRPos
            // 
            this.lblRPos.Location = new System.Drawing.Point(13, 56);
            this.lblRPos.Name = "lblRPos";
            this.lblRPos.Size = new System.Drawing.Size(84, 23);
            this.lblRPos.TabIndex = 13;
            this.lblRPos.Text = "Stage X Pos";
            this.lblRPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRPos
            // 
            this.txtRPos.Location = new System.Drawing.Point(103, 57);
            this.txtRPos.Name = "txtRPos";
            this.txtRPos.ReadOnly = true;
            this.txtRPos.Size = new System.Drawing.Size(85, 23);
            this.txtRPos.TabIndex = 14;
            // 
            // lblMPos
            // 
            this.lblMPos.Location = new System.Drawing.Point(13, 27);
            this.lblMPos.Name = "lblMPos";
            this.lblMPos.Size = new System.Drawing.Size(84, 23);
            this.lblMPos.TabIndex = 11;
            this.lblMPos.Text = "Stage Y Pos";
            this.lblMPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMPos
            // 
            this.txtMPos.Location = new System.Drawing.Point(103, 28);
            this.txtMPos.Name = "txtMPos";
            this.txtMPos.ReadOnly = true;
            this.txtMPos.Size = new System.Drawing.Size(85, 23);
            this.txtMPos.TabIndex = 12;
            // 
            // rbtnProcessStepHoles
            // 
            this.rbtnProcessStepHoles.AutoSize = true;
            this.rbtnProcessStepHoles.Location = new System.Drawing.Point(162, 38);
            this.rbtnProcessStepHoles.Name = "rbtnProcessStepHoles";
            this.rbtnProcessStepHoles.Size = new System.Drawing.Size(49, 19);
            this.rbtnProcessStepHoles.TabIndex = 12;
            this.rbtnProcessStepHoles.TabStop = true;
            this.rbtnProcessStepHoles.Text = "Step";
            this.rbtnProcessStepHoles.UseVisualStyleBackColor = true;
            // 
            // OpticOperationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1530, 699);
            this.Controls.Add(this.grpMotionData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlLaser);
            this.Name = "OpticOperationForm";
            this.Text = "Optic Operation";
            this.pnlLaser.ResumeLayout(false);
            this.pnlLaser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAtt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXOfs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYOfs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ProcessScanXSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessScanYSpeed)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlyProcessLines)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessStepCount)).EndInit();
            this.gbProcessType.ResumeLayout(false);
            this.gbProcessType.PerformLayout();
            this.grpMotionData.ResumeLayout(false);
            this.grpMotionData.PerformLayout();
            this.ResumeLayout(false);

        }

        private Button _btnProcessScanning;
        private Button btnMoveScan;
        private DataGridView gridProcess;
        private Button _btnProcessStop;
        private Panel panel1;
        private Label label1;
        private GroupBox gbProcessType;
        private RadioButton rbtnProcessStepbyStep;
        private RadioButton rbtnProcessOntheFly;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private NumericUpDown numProcessStepCount;
        private Label label3;
        private NumericUpDown numFlyProcessLines;
        private Label label2;
        private GroupBox grpMotionData;
        private Label lblRPos;
        private TextBox txtRPos;
        private Label lblMPos;
        private TextBox txtMPos;
        private GroupBox groupBox3;
        private RadioButton rbtnOneWayProcess;
        private RadioButton rbtnSnakeProcess;
        private GroupBox groupBox4;
        private RadioButton rbtnForwardProcess;
        private RadioButton rbtnBackwardProcess;
        private GroupBox groupBox5;
        private RadioButton rbtnXProcess;
        private RadioButton rbtnYProcess;
        private TextBox txtTPos;
        private TextBox txtZPos;
        private Label label5;
        private Label label4;
        private GroupBox groupBox6;
        private NumericUpDown ProcessScanXSpeed;
        private Label label6;
        private NumericUpDown ProcessScanYSpeed;
        private Label label7;
        private Label label8;
        private TextBox txtStageXSpeed;
        private Label label9;
        private TextBox txtStageYSpeed;
        private Panel pnlLaser;
        private Label lblLaserTitle;
        private Label lblPower;
        private Label lblAtt;
        private NumericUpDown numPower;
        private NumericUpDown numAtt;
        private Button btnApplyLaser;
        private Button btnLaserOn;
        private Button btnLaserOff;

        private Label lblRecipeCaption;
        private Label lblRecipeValue;
        private Label lblOriginCaption;
        private Label lblOriginValue;

        private Label lblEmissionCaption;
        private Label lblEmissionState;
        private Label ledEmission;

        private Label lblScanNo;
        private NumericUpDown numScanNo;
        private Label lblTime;
        private NumericUpDown numTimeMs;
        private Label lblFreq;
        private NumericUpDown numFreq;
        private Label lblXOfs;
        private Label lblYOfs;
        private NumericUpDown numXOfs;
        private NumericUpDown numYOfs;
        private RadioButton rbtnProcessStepAllWholeLine;
        private RadioButton rbtnProcessStepWholeLine;
        private RadioButton rbtnProcessStepHoles;
    }
}
