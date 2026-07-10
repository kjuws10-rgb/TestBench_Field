namespace StageWin.UI
{
    partial class ESCFlatnessForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView grid;

        private System.Windows.Forms.NumericUpDown numCountX;
        private System.Windows.Forms.NumericUpDown numCountY;
        private System.Windows.Forms.NumericUpDown numStepX;
        private System.Windows.Forms.NumericUpDown numStepY;
        private System.Windows.Forms.NumericUpDown numVelX;
        private System.Windows.Forms.NumericUpDown numVelY;
        private System.Windows.Forms.NumericUpDown numAccX;
        private System.Windows.Forms.NumericUpDown numAccY;
        private System.Windows.Forms.NumericUpDown numDwell;
        private System.Windows.Forms.NumericUpDown numTolX;
        private System.Windows.Forms.NumericUpDown numTolY;
        private System.Windows.Forms.NumericUpDown numCritFlat;
        private System.Windows.Forms.NumericUpDown numCritThick;

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;

        private System.Windows.Forms.Button btnPickOrigin;
        private System.Windows.Forms.TextBox txtOriginX;
        private System.Windows.Forms.TextBox txtOriginY;
        private System.Windows.Forms.Button btnBuildGrid;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.CheckBox chkSnake;

        // 캡션 라벨들
        private System.Windows.Forms.Label lblCountX;
        private System.Windows.Forms.Label lblCountY;
        private System.Windows.Forms.Label lblStepX;
        private System.Windows.Forms.Label lblStepY;
        private System.Windows.Forms.Label lblVelX;
        private System.Windows.Forms.Label lblVelY;
        private System.Windows.Forms.Label lblAccX;
        private System.Windows.Forms.Label lblAccY;
        private System.Windows.Forms.Label lblDwell;
        private System.Windows.Forms.Label lblTolX;
        private System.Windows.Forms.Label lblTolY;
        private System.Windows.Forms.Label lblCritFlat;
        private System.Windows.Forms.Label lblCritThick;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grid = new System.Windows.Forms.DataGridView();
            this.numCountX = new System.Windows.Forms.NumericUpDown();
            this.numCountY = new System.Windows.Forms.NumericUpDown();
            this.numStepX = new System.Windows.Forms.NumericUpDown();
            this.numStepY = new System.Windows.Forms.NumericUpDown();
            this.numVelX = new System.Windows.Forms.NumericUpDown();
            this.numVelY = new System.Windows.Forms.NumericUpDown();
            this.numAccX = new System.Windows.Forms.NumericUpDown();
            this.numAccY = new System.Windows.Forms.NumericUpDown();
            this.numDwell = new System.Windows.Forms.NumericUpDown();
            this.numTolX = new System.Windows.Forms.NumericUpDown();
            this.numTolY = new System.Windows.Forms.NumericUpDown();
            this.numCritFlat = new System.Windows.Forms.NumericUpDown();
            this.numCritThick = new System.Windows.Forms.NumericUpDown();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnPickOrigin = new System.Windows.Forms.Button();
            this.txtOriginX = new System.Windows.Forms.TextBox();
            this.txtOriginY = new System.Windows.Forms.TextBox();
            this.btnBuildGrid = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.chkSnake = new System.Windows.Forms.CheckBox();
            this.lblCountX = new System.Windows.Forms.Label();
            this.lblCountY = new System.Windows.Forms.Label();
            this.lblStepX = new System.Windows.Forms.Label();
            this.lblStepY = new System.Windows.Forms.Label();
            this.lblVelX = new System.Windows.Forms.Label();
            this.lblVelY = new System.Windows.Forms.Label();
            this.lblAccX = new System.Windows.Forms.Label();
            this.lblAccY = new System.Windows.Forms.Label();
            this.lblDwell = new System.Windows.Forms.Label();
            this.lblTolX = new System.Windows.Forms.Label();
            this.lblTolY = new System.Windows.Forms.Label();
            this.lblCritFlat = new System.Windows.Forms.Label();
            this.lblCritThick = new System.Windows.Forms.Label();
            this.gpLdsValue = new System.Windows.Forms.GroupBox();
            this.lblLiveThick = new System.Windows.Forms.Label();
            this.lblLiveFlat = new System.Windows.Forms.Label();
            this.lblLiveCh2 = new System.Windows.Forms.Label();
            this.lblLiveCh1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbJudge1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDwell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCritFlat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCritThick)).BeginInit();
            this.gpLdsValue.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle1;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Font = new System.Drawing.Font("Consolas", 9F);
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersWidth = 70;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grid.Size = new System.Drawing.Size(1103, 901);
            this.grid.TabIndex = 100;
            // 
            // numCountX
            // 
            this.numCountX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numCountX.Location = new System.Drawing.Point(12, 35);
            this.numCountX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numCountX.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numCountX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCountX.Name = "numCountX";
            this.numCountX.Size = new System.Drawing.Size(80, 23);
            this.numCountX.TabIndex = 1;
            this.numCountX.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numCountY
            // 
            this.numCountY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numCountY.Location = new System.Drawing.Point(100, 35);
            this.numCountY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numCountY.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numCountY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCountY.Name = "numCountY";
            this.numCountY.Size = new System.Drawing.Size(80, 23);
            this.numCountY.TabIndex = 2;
            this.numCountY.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numStepX
            // 
            this.numStepX.DecimalPlaces = 3;
            this.numStepX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numStepX.Location = new System.Drawing.Point(188, 35);
            this.numStepX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numStepX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numStepX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numStepX.Name = "numStepX";
            this.numStepX.Size = new System.Drawing.Size(80, 23);
            this.numStepX.TabIndex = 3;
            this.numStepX.Value = new decimal(new int[] {
            1000,
            0,
            0,
            196608});
            // 
            // numStepY
            // 
            this.numStepY.DecimalPlaces = 3;
            this.numStepY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numStepY.Location = new System.Drawing.Point(276, 35);
            this.numStepY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numStepY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numStepY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numStepY.Name = "numStepY";
            this.numStepY.Size = new System.Drawing.Size(80, 23);
            this.numStepY.TabIndex = 4;
            this.numStepY.Value = new decimal(new int[] {
            1000,
            0,
            0,
            196608});
            // 
            // numVelX
            // 
            this.numVelX.DecimalPlaces = 3;
            this.numVelX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numVelX.Location = new System.Drawing.Point(364, 35);
            this.numVelX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numVelX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numVelX.Name = "numVelX";
            this.numVelX.Size = new System.Drawing.Size(80, 23);
            this.numVelX.TabIndex = 5;
            this.numVelX.Value = new decimal(new int[] {
            50000,
            0,
            0,
            196608});
            // 
            // numVelY
            // 
            this.numVelY.DecimalPlaces = 3;
            this.numVelY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numVelY.Location = new System.Drawing.Point(452, 35);
            this.numVelY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numVelY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numVelY.Name = "numVelY";
            this.numVelY.Size = new System.Drawing.Size(80, 23);
            this.numVelY.TabIndex = 6;
            this.numVelY.Value = new decimal(new int[] {
            50000,
            0,
            0,
            196608});
            // 
            // numAccX
            // 
            this.numAccX.DecimalPlaces = 3;
            this.numAccX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numAccX.Location = new System.Drawing.Point(540, 35);
            this.numAccX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numAccX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAccX.Name = "numAccX";
            this.numAccX.Size = new System.Drawing.Size(80, 23);
            this.numAccX.TabIndex = 7;
            this.numAccX.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            196608});
            // 
            // numAccY
            // 
            this.numAccY.DecimalPlaces = 3;
            this.numAccY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numAccY.Location = new System.Drawing.Point(628, 35);
            this.numAccY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numAccY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAccY.Name = "numAccY";
            this.numAccY.Size = new System.Drawing.Size(80, 23);
            this.numAccY.TabIndex = 8;
            this.numAccY.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            196608});
            // 
            // numDwell
            // 
            this.numDwell.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numDwell.Location = new System.Drawing.Point(716, 35);
            this.numDwell.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numDwell.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numDwell.Name = "numDwell";
            this.numDwell.Size = new System.Drawing.Size(80, 23);
            this.numDwell.TabIndex = 9;
            this.numDwell.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numTolX
            // 
            this.numTolX.DecimalPlaces = 3;
            this.numTolX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numTolX.Location = new System.Drawing.Point(809, 35);
            this.numTolX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numTolX.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTolX.Name = "numTolX";
            this.numTolX.Size = new System.Drawing.Size(80, 23);
            this.numTolX.TabIndex = 10;
            this.numTolX.Value = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            // 
            // numTolY
            // 
            this.numTolY.DecimalPlaces = 3;
            this.numTolY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numTolY.Location = new System.Drawing.Point(902, 35);
            this.numTolY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numTolY.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTolY.Name = "numTolY";
            this.numTolY.Size = new System.Drawing.Size(80, 23);
            this.numTolY.TabIndex = 11;
            this.numTolY.Value = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            // 
            // numCritFlat
            // 
            this.numCritFlat.DecimalPlaces = 3;
            this.numCritFlat.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numCritFlat.Location = new System.Drawing.Point(1135, 442);
            this.numCritFlat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numCritFlat.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCritFlat.Name = "numCritFlat";
            this.numCritFlat.Size = new System.Drawing.Size(80, 23);
            this.numCritFlat.TabIndex = 14;
            this.numCritFlat.Value = new decimal(new int[] {
            50,
            0,
            0,
            196608});
            // 
            // numCritThick
            // 
            this.numCritThick.DecimalPlaces = 3;
            this.numCritThick.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.numCritThick.Location = new System.Drawing.Point(1228, 442);
            this.numCritThick.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numCritThick.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCritThick.Name = "numCritThick";
            this.numCritThick.Size = new System.Drawing.Size(80, 23);
            this.numCritThick.TabIndex = 15;
            this.numCritThick.Value = new decimal(new int[] {
            100,
            0,
            0,
            196608});
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblStatus.Location = new System.Drawing.Point(840, 128);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(152, 28);
            this.lblStatus.TabIndex = 25;
            this.lblStatus.Text = "대기";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 125);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(820, 22);
            this.progressBar.TabIndex = 24;
            // 
            // btnPickOrigin
            // 
            this.btnPickOrigin.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnPickOrigin.Location = new System.Drawing.Point(12, 76);
            this.btnPickOrigin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPickOrigin.Name = "btnPickOrigin";
            this.btnPickOrigin.Size = new System.Drawing.Size(120, 35);
            this.btnPickOrigin.TabIndex = 16;
            this.btnPickOrigin.Text = "현재위치=원점";
            this.btnPickOrigin.UseVisualStyleBackColor = true;
            this.btnPickOrigin.Click += new System.EventHandler(this.btnPickOrigin_Click);
            // 
            // txtOriginX
            // 
            this.txtOriginX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.txtOriginX.Location = new System.Drawing.Point(140, 76);
            this.txtOriginX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtOriginX.Name = "txtOriginX";
            this.txtOriginX.ReadOnly = true;
            this.txtOriginX.Size = new System.Drawing.Size(90, 23);
            this.txtOriginX.TabIndex = 17;
            // 
            // txtOriginY
            // 
            this.txtOriginY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.txtOriginY.Location = new System.Drawing.Point(236, 76);
            this.txtOriginY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtOriginY.Name = "txtOriginY";
            this.txtOriginY.ReadOnly = true;
            this.txtOriginY.Size = new System.Drawing.Size(90, 23);
            this.txtOriginY.TabIndex = 18;
            // 
            // btnBuildGrid
            // 
            this.btnBuildGrid.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnBuildGrid.Location = new System.Drawing.Point(340, 76);
            this.btnBuildGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBuildGrid.Name = "btnBuildGrid";
            this.btnBuildGrid.Size = new System.Drawing.Size(100, 35);
            this.btnBuildGrid.TabIndex = 19;
            this.btnBuildGrid.Text = "Grid 생성";
            this.btnBuildGrid.UseVisualStyleBackColor = true;
            this.btnBuildGrid.Click += new System.EventHandler(this.btnBuildGrid_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnStart.Location = new System.Drawing.Point(560, 76);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 35);
            this.btnStart.TabIndex = 21;
            this.btnStart.Text = "측정 시작";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnStop.Location = new System.Drawing.Point(666, 76);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 35);
            this.btnStop.TabIndex = 22;
            this.btnStop.Text = "정지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnExportCsv.Location = new System.Drawing.Point(772, 76);
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(120, 35);
            this.btnExportCsv.TabIndex = 23;
            this.btnExportCsv.Text = "CSV 저장";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // chkSnake
            // 
            this.chkSnake.AutoSize = true;
            this.chkSnake.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.chkSnake.Location = new System.Drawing.Point(456, 81);
            this.chkSnake.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSnake.Name = "chkSnake";
            this.chkSnake.Size = new System.Drawing.Size(86, 19);
            this.chkSnake.TabIndex = 20;
            this.chkSnake.Text = "Snake 패턴";
            this.chkSnake.UseVisualStyleBackColor = true;
            // 
            // lblCountX
            // 
            this.lblCountX.AutoSize = true;
            this.lblCountX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblCountX.Location = new System.Drawing.Point(12, 14);
            this.lblCountX.Name = "lblCountX";
            this.lblCountX.Size = new System.Drawing.Size(47, 15);
            this.lblCountX.TabIndex = 101;
            this.lblCountX.Text = "CountX";
            // 
            // lblCountY
            // 
            this.lblCountY.AutoSize = true;
            this.lblCountY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblCountY.Location = new System.Drawing.Point(100, 14);
            this.lblCountY.Name = "lblCountY";
            this.lblCountY.Size = new System.Drawing.Size(47, 15);
            this.lblCountY.TabIndex = 102;
            this.lblCountY.Text = "CountY";
            // 
            // lblStepX
            // 
            this.lblStepX.AutoSize = true;
            this.lblStepX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblStepX.Location = new System.Drawing.Point(188, 14);
            this.lblStepX.Name = "lblStepX";
            this.lblStepX.Size = new System.Drawing.Size(68, 15);
            this.lblStepX.TabIndex = 103;
            this.lblStepX.Text = "StepX(mm)";
            // 
            // lblStepY
            // 
            this.lblStepY.AutoSize = true;
            this.lblStepY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblStepY.Location = new System.Drawing.Point(276, 14);
            this.lblStepY.Name = "lblStepY";
            this.lblStepY.Size = new System.Drawing.Size(68, 15);
            this.lblStepY.TabIndex = 104;
            this.lblStepY.Text = "StepY(mm)";
            // 
            // lblVelX
            // 
            this.lblVelX.AutoSize = true;
            this.lblVelX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblVelX.Location = new System.Drawing.Point(364, 14);
            this.lblVelX.Name = "lblVelX";
            this.lblVelX.Size = new System.Drawing.Size(31, 15);
            this.lblVelX.TabIndex = 105;
            this.lblVelX.Text = "VelX";
            // 
            // lblVelY
            // 
            this.lblVelY.AutoSize = true;
            this.lblVelY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblVelY.Location = new System.Drawing.Point(452, 14);
            this.lblVelY.Name = "lblVelY";
            this.lblVelY.Size = new System.Drawing.Size(31, 15);
            this.lblVelY.TabIndex = 106;
            this.lblVelY.Text = "VelY";
            // 
            // lblAccX
            // 
            this.lblAccX.AutoSize = true;
            this.lblAccX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblAccX.Location = new System.Drawing.Point(540, 14);
            this.lblAccX.Name = "lblAccX";
            this.lblAccX.Size = new System.Drawing.Size(34, 15);
            this.lblAccX.TabIndex = 107;
            this.lblAccX.Text = "AccX";
            // 
            // lblAccY
            // 
            this.lblAccY.AutoSize = true;
            this.lblAccY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblAccY.Location = new System.Drawing.Point(628, 14);
            this.lblAccY.Name = "lblAccY";
            this.lblAccY.Size = new System.Drawing.Size(34, 15);
            this.lblAccY.TabIndex = 108;
            this.lblAccY.Text = "AccY";
            // 
            // lblDwell
            // 
            this.lblDwell.AutoSize = true;
            this.lblDwell.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblDwell.Location = new System.Drawing.Point(716, 14);
            this.lblDwell.Name = "lblDwell";
            this.lblDwell.Size = new System.Drawing.Size(61, 15);
            this.lblDwell.TabIndex = 109;
            this.lblDwell.Text = "Dwell(ms)";
            // 
            // lblTolX
            // 
            this.lblTolX.AutoSize = true;
            this.lblTolX.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblTolX.Location = new System.Drawing.Point(809, 14);
            this.lblTolX.Name = "lblTolX";
            this.lblTolX.Size = new System.Drawing.Size(30, 15);
            this.lblTolX.TabIndex = 110;
            this.lblTolX.Text = "TolX";
            // 
            // lblTolY
            // 
            this.lblTolY.AutoSize = true;
            this.lblTolY.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblTolY.Location = new System.Drawing.Point(902, 14);
            this.lblTolY.Name = "lblTolY";
            this.lblTolY.Size = new System.Drawing.Size(30, 15);
            this.lblTolY.TabIndex = 111;
            this.lblTolY.Text = "TolY";
            // 
            // lblCritFlat
            // 
            this.lblCritFlat.AutoSize = true;
            this.lblCritFlat.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblCritFlat.Location = new System.Drawing.Point(1135, 421);
            this.lblCritFlat.Name = "lblCritFlat";
            this.lblCritFlat.Size = new System.Drawing.Size(45, 15);
            this.lblCritFlat.TabIndex = 114;
            this.lblCritFlat.Text = "CritFlat";
            // 
            // lblCritThick
            // 
            this.lblCritThick.AutoSize = true;
            this.lblCritThick.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblCritThick.Location = new System.Drawing.Point(1228, 421);
            this.lblCritThick.Name = "lblCritThick";
            this.lblCritThick.Size = new System.Drawing.Size(54, 15);
            this.lblCritThick.TabIndex = 115;
            this.lblCritThick.Text = "CritThick";
            // 
            // gpLdsValue
            // 
            this.gpLdsValue.Controls.Add(this.lblLiveThick);
            this.gpLdsValue.Controls.Add(this.lblLiveFlat);
            this.gpLdsValue.Controls.Add(this.lblLiveCh2);
            this.gpLdsValue.Controls.Add(this.lblLiveCh1);
            this.gpLdsValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpLdsValue.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gpLdsValue.Location = new System.Drawing.Point(0, 0);
            this.gpLdsValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gpLdsValue.Name = "gpLdsValue";
            this.gpLdsValue.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gpLdsValue.Size = new System.Drawing.Size(213, 251);
            this.gpLdsValue.TabIndex = 116;
            this.gpLdsValue.TabStop = false;
            this.gpLdsValue.Text = "LDS Monitoring";
            // 
            // lblLiveThick
            // 
            this.lblLiveThick.AutoSize = true;
            this.lblLiveThick.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblLiveThick.Location = new System.Drawing.Point(18, 188);
            this.lblLiveThick.Name = "lblLiveThick";
            this.lblLiveThick.Size = new System.Drawing.Size(54, 21);
            this.lblLiveThick.TabIndex = 1;
            this.lblLiveThick.Text = "label1";
            // 
            // lblLiveFlat
            // 
            this.lblLiveFlat.AutoSize = true;
            this.lblLiveFlat.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblLiveFlat.Location = new System.Drawing.Point(18, 145);
            this.lblLiveFlat.Name = "lblLiveFlat";
            this.lblLiveFlat.Size = new System.Drawing.Size(54, 21);
            this.lblLiveFlat.TabIndex = 0;
            this.lblLiveFlat.Text = "label1";
            // 
            // lblLiveCh2
            // 
            this.lblLiveCh2.AutoSize = true;
            this.lblLiveCh2.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblLiveCh2.Location = new System.Drawing.Point(18, 91);
            this.lblLiveCh2.Name = "lblLiveCh2";
            this.lblLiveCh2.Size = new System.Drawing.Size(54, 21);
            this.lblLiveCh2.TabIndex = 1;
            this.lblLiveCh2.Text = "label1";
            // 
            // lblLiveCh1
            // 
            this.lblLiveCh1.AutoSize = true;
            this.lblLiveCh1.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.lblLiveCh1.Location = new System.Drawing.Point(18, 46);
            this.lblLiveCh1.Name = "lblLiveCh1";
            this.lblLiveCh1.Size = new System.Drawing.Size(54, 21);
            this.lblLiveCh1.TabIndex = 0;
            this.lblLiveCh1.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.grid);
            this.panel1.Location = new System.Drawing.Point(12, 155);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1103, 901);
            this.panel1.TabIndex = 117;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gpLdsValue);
            this.panel2.Location = new System.Drawing.Point(1130, 155);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(213, 251);
            this.panel2.TabIndex = 118;
            // 
            // lbJudge1
            // 
            this.lbJudge1.BackColor = System.Drawing.Color.IndianRed;
            this.lbJudge1.Location = new System.Drawing.Point(1135, 495);
            this.lbJudge1.Name = "lbJudge1";
            this.lbJudge1.Size = new System.Drawing.Size(19, 30);
            this.lbJudge1.TabIndex = 119;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label1.Location = new System.Drawing.Point(1162, 499);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 21);
            this.label1.TabIndex = 120;
            this.label1.Text = "Flat && Thick NG";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label2.Location = new System.Drawing.Point(1162, 542);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 21);
            this.label2.TabIndex = 122;
            this.label2.Text = "Flatness NG";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Gold;
            this.label3.Location = new System.Drawing.Point(1135, 539);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 30);
            this.label3.TabIndex = 121;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.label4.Location = new System.Drawing.Point(1162, 586);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 21);
            this.label4.TabIndex = 124;
            this.label4.Text = "Thickness NG";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.LightSalmon;
            this.label5.Location = new System.Drawing.Point(1135, 582);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 30);
            this.label5.TabIndex = 123;
            // 
            // ESCFlatnessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1613, 1071);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbJudge1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblCountX);
            this.Controls.Add(this.lblCountY);
            this.Controls.Add(this.lblStepX);
            this.Controls.Add(this.lblStepY);
            this.Controls.Add(this.lblVelX);
            this.Controls.Add(this.lblVelY);
            this.Controls.Add(this.lblAccX);
            this.Controls.Add(this.lblAccY);
            this.Controls.Add(this.lblDwell);
            this.Controls.Add(this.lblTolX);
            this.Controls.Add(this.lblTolY);
            this.Controls.Add(this.lblCritFlat);
            this.Controls.Add(this.lblCritThick);
            this.Controls.Add(this.numCountX);
            this.Controls.Add(this.numCountY);
            this.Controls.Add(this.numStepX);
            this.Controls.Add(this.numStepY);
            this.Controls.Add(this.numVelX);
            this.Controls.Add(this.numVelY);
            this.Controls.Add(this.numAccX);
            this.Controls.Add(this.numAccY);
            this.Controls.Add(this.numDwell);
            this.Controls.Add(this.numTolX);
            this.Controls.Add(this.numTolY);
            this.Controls.Add(this.numCritFlat);
            this.Controls.Add(this.numCritThick);
            this.Controls.Add(this.btnPickOrigin);
            this.Controls.Add(this.txtOriginX);
            this.Controls.Add(this.txtOriginY);
            this.Controls.Add(this.btnBuildGrid);
            this.Controls.Add(this.chkSnake);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ESCFlatnessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ESC Flatness";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCountY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDwell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCritFlat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCritThick)).EndInit();
            this.gpLdsValue.ResumeLayout(false);
            this.gpLdsValue.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.GroupBox gpLdsValue;
        private System.Windows.Forms.Label lblLiveCh1;
        private System.Windows.Forms.Label lblLiveThick;
        private System.Windows.Forms.Label lblLiveFlat;
        private System.Windows.Forms.Label lblLiveCh2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbJudge1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
