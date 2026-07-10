using System.Windows.Forms;
using System.Drawing;

namespace StageWin.UI
{
    partial class ParameterSettingForm
    {
        private System.ComponentModel.IContainer components = null;

        // 상단 버튼
        private Button btnSave;
        private Button btnLoad;
        private Button btnClose;

        private TabControl tabMain;
        private TabPage tabProcessParams;

        // ----- Left column labels & textboxes -----
        private Label lblMoveToAlignXPos;
        private TextBox txtMoveToAlignXPos;
        private Label lblMoveToAlignXSpeed;
        private TextBox txtMoveToAlignXSpeed;
        private Label lblMoveToAlignXAcc;
        private TextBox txtMoveToAlignXAcc;

        private Label lblMoveToAlignYPos;
        private TextBox txtMoveToAlignYPos;
        private Label lblMoveToAlignYSpeed;
        private TextBox txtMoveToAlignYSpeed;
        private Label lblMoveToAlignYAcc;
        private TextBox txtMoveToAlignYAcc;

        private Label lblMoveToPWMXPos;
        private TextBox txtMoveToPWMXPos;
        private Label lblMoveToPWMXSpeed;
        private TextBox txtMoveToPWMXSpeed;
        private Label lblMoveToPWMXAcc;
        private TextBox txtMoveToPWMXAcc;

        private Label lblMoveToPWMYPos;
        private TextBox txtMoveToPWMYPos;
        private Label lblMoveToPWMYSpeed;
        private TextBox txtMoveToPWMYSpeed;
        private Label lblMoveToPWMYAcc;
        private TextBox txtMoveToPWMYAcc;

        private Label lblMoveToProcReadyXPos;
        private TextBox txtMoveToProcReadyXPos;
        private Label lblMoveToProcReadyXSpeed;
        private TextBox txtMoveToProcReadyXSpeed;
        private Label lblMoveToProcReadyXAcc;
        private TextBox txtMoveToProcReadyXAcc;

        private Label lblMoveToProcReadyYPos;
        private TextBox txtMoveToProcReadyYPos;
        private Label lblMoveToProcReadyYSpeed;
        private TextBox txtMoveToProcReadyYSpeed;
        private Label lblMoveToProcReadyYAcc;
        private TextBox txtMoveToProcReadyYAcc;

        private Label lblProcessYPos;
        private TextBox txtProcessYPos;
        private Label lblProcessYSpeed;
        private TextBox txtProcessYSpeed;
        private Label lblProcessYAcc;
        private TextBox txtProcessYAcc;

        // ----- Right column labels & textboxes -----
        private Label lblMoveToProcessEndYPos;
        private TextBox txtMoveToProcessEndYPos;
        private Label lblMoveToProcessEndYSpeed;
        private TextBox txtMoveToProcessEndYSpeed;
        private Label lblMoveToProcessEndYAcc;
        private TextBox txtMoveToProcessEndYAcc;

        private Label lblMoveToInspectionYPos;
        private TextBox txtMoveToInspectionYPos;
        private Label lblMoveToInspectionYSpeed;
        private TextBox txtMoveToInspectionYSpeed;
        private Label lblMoveToInspectionYAcc;
        private TextBox txtMoveToInspectionYAcc;

        private Label lblInspectionXPos;
        private TextBox txtInspectionXPos;
        private Label lblInspectionXSpeed;
        private TextBox txtInspectionXSpeed;
        private Label lblInspectionXAcc;
        private TextBox txtInspectionXAcc;

        private Label lblInspectionYPos;
        private TextBox txtInspectionYPos;
        private Label lblInspectionYSpeed;
        private TextBox txtInspectionYSpeed;
        private Label lblInspectionYAcc;
        private TextBox txtInspectionYAcc;

        private Label lblMoveToAlignCheckXPos;
        private TextBox txtMoveToAlignCheckXPos;
        private Label lblMoveToAlignCheckXSpeed;
        private TextBox txtMoveToAlignCheckXSpeed;
        private Label lblMoveToAlignCheckXAcc;
        private TextBox txtMoveToAlignCheckXAcc;

        private Label lblMoveToAlignCheckYPos;
        private TextBox txtMoveToAlignCheckYPos;
        private Label lblMoveToAlignCheckYSpeed;
        private TextBox txtMoveToAlignCheckYSpeed;
        private Label lblMoveToAlignCheckYAcc;
        private TextBox txtMoveToAlignCheckYAcc;

        private Label lblMoveToUnloadXPos;
        private TextBox txtMoveToUnloadXPos;
        private Label lblMoveToUnloadXSpeed;
        private TextBox txtMoveToUnloadXSpeed;
        private Label lblMoveToUnloadXAcc;
        private TextBox txtMoveToUnloadXAcc;

        private Label lblMoveToUnloadYPos;
        private TextBox txtMoveToUnloadYPos;
        private Label lblMoveToUnloadYSpeed;
        private TextBox txtMoveToUnloadYSpeed;
        private Label lblMoveToUnloadYAcc;
        private TextBox txtMoveToUnloadYAcc;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabProcessParams = new System.Windows.Forms.TabPage();
            this.grpProcessOption = new System.Windows.Forms.GroupBox();
            this.rbtnAllStepProcess = new System.Windows.Forms.RadioButton();
            this.rbtn1StepProcess = new System.Windows.Forms.RadioButton();
            this.lalAfterSeqMs = new System.Windows.Forms.Label();
            this.grpSettlingTime = new System.Windows.Forms.GroupBox();
            this.txtAutoDelayAfterMoveMs = new System.Windows.Forms.TextBox();
            this.lalAfterMoveMs = new System.Windows.Forms.Label();
            this.txtAutoDelayAfterSeqMs = new System.Windows.Forms.TextBox();
            this.grpInspectionOption = new System.Windows.Forms.GroupBox();
            this.rbtnStepbyStepAllInspection = new System.Windows.Forms.RadioButton();
            this.rbtnAllInspection = new System.Windows.Forms.RadioButton();
            this.rbtn1LineInspection = new System.Windows.Forms.RadioButton();
            this.lblMoveToLoadYPos = new System.Windows.Forms.Label();
            this.txtMoveToLoadYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToLoadYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToLoadYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToLoadYAcc = new System.Windows.Forms.Label();
            this.txtMoveToLoadYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToLoadXPos = new System.Windows.Forms.Label();
            this.txtMoveToLoadXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToLoadXPosSpeed = new System.Windows.Forms.Label();
            this.txtMoveToLoadXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToLoadXAcc = new System.Windows.Forms.Label();
            this.txtMoveToLoadXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignXPos = new System.Windows.Forms.Label();
            this.txtMoveToAlignXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignXSpeed = new System.Windows.Forms.Label();
            this.txtMoveToAlignXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignXAcc = new System.Windows.Forms.Label();
            this.txtMoveToAlignXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignYPos = new System.Windows.Forms.Label();
            this.txtMoveToAlignYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToAlignYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignYAcc = new System.Windows.Forms.Label();
            this.txtMoveToAlignYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMXPos = new System.Windows.Forms.Label();
            this.txtMoveToPWMXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMXSpeed = new System.Windows.Forms.Label();
            this.txtMoveToPWMXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMXAcc = new System.Windows.Forms.Label();
            this.txtMoveToPWMXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMYPos = new System.Windows.Forms.Label();
            this.txtMoveToPWMYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToPWMYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToPWMYAcc = new System.Windows.Forms.Label();
            this.txtMoveToPWMYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyXPos = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyXSpeed = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyXAcc = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyYPos = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToProcReadyYAcc = new System.Windows.Forms.Label();
            this.txtMoveToProcReadyYAcc = new System.Windows.Forms.TextBox();
            this.lblProcessYSpeed = new System.Windows.Forms.Label();
            this.txtProcessYSpeed = new System.Windows.Forms.TextBox();
            this.lblProcessYAcc = new System.Windows.Forms.Label();
            this.txtProcessYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckXPos = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckXSpeed = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckXAcc = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckYPos = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToAlignCheckYAcc = new System.Windows.Forms.Label();
            this.txtMoveToAlignCheckYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadXPos = new System.Windows.Forms.Label();
            this.txtMoveToUnloadXPos = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadXSpeed = new System.Windows.Forms.Label();
            this.txtMoveToUnloadXSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadXAcc = new System.Windows.Forms.Label();
            this.txtMoveToUnloadXAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadYPos = new System.Windows.Forms.Label();
            this.txtMoveToUnloadYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToUnloadYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToUnloadYAcc = new System.Windows.Forms.Label();
            this.txtMoveToUnloadYAcc = new System.Windows.Forms.TextBox();
            this.grpPowerMeterRecipe = new System.Windows.Forms.GroupBox();
            this.lstPmRecipeList = new System.Windows.Forms.ListBox();
            this.btnPmRefresh = new System.Windows.Forms.Button();
            this.lblPmSelectedDisplay = new System.Windows.Forms.Label();
            this.txtSelectedPmRecipeName = new System.Windows.Forms.TextBox();
            this.lblProcessYPos = new System.Windows.Forms.Label();
            this.txtProcessYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToProcessEndYPos = new System.Windows.Forms.Label();
            this.txtMoveToProcessEndYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToProcessEndYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToProcessEndYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToProcessEndYAcc = new System.Windows.Forms.Label();
            this.txtMoveToProcessEndYAcc = new System.Windows.Forms.TextBox();
            this.lblMoveToInspectionYPos = new System.Windows.Forms.Label();
            this.txtMoveToInspectionYPos = new System.Windows.Forms.TextBox();
            this.lblMoveToInspectionYSpeed = new System.Windows.Forms.Label();
            this.txtMoveToInspectionYSpeed = new System.Windows.Forms.TextBox();
            this.lblMoveToInspectionYAcc = new System.Windows.Forms.Label();
            this.txtMoveToInspectionYAcc = new System.Windows.Forms.TextBox();
            this.lblInspectionXPos = new System.Windows.Forms.Label();
            this.txtInspectionXPos = new System.Windows.Forms.TextBox();
            this.lblInspectionXSpeed = new System.Windows.Forms.Label();
            this.txtInspectionXSpeed = new System.Windows.Forms.TextBox();
            this.lblInspectionXAcc = new System.Windows.Forms.Label();
            this.txtInspectionXAcc = new System.Windows.Forms.TextBox();
            this.lblInspectionYPos = new System.Windows.Forms.Label();
            this.txtInspectionYPos = new System.Windows.Forms.TextBox();
            this.lblInspectionYSpeed = new System.Windows.Forms.Label();
            this.txtInspectionYSpeed = new System.Windows.Forms.TextBox();
            this.lblInspectionYAcc = new System.Windows.Forms.Label();
            this.txtInspectionYAcc = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabMain.SuspendLayout();
            this.tabProcessParams.SuspendLayout();
            this.grpProcessOption.SuspendLayout();
            this.grpSettlingTime.SuspendLayout();
            this.grpInspectionOption.SuspendLayout();
            this.grpPowerMeterRecipe.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(12, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 28);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoad.Location = new System.Drawing.Point(108, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(90, 28);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(204, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 28);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabProcessParams);
            this.tabMain.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.tabMain.Location = new System.Drawing.Point(12, 48);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1096, 635);
            this.tabMain.TabIndex = 4;
            // 
            // tabProcessParams
            // 
            this.tabProcessParams.Controls.Add(this.grpProcessOption);
            this.tabProcessParams.Controls.Add(this.lalAfterSeqMs);
            this.tabProcessParams.Controls.Add(this.grpSettlingTime);
            this.tabProcessParams.Controls.Add(this.grpInspectionOption);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadYAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadXPosSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToLoadXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToLoadXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignXSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignYAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMXSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToPWMYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToPWMYAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyXSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToProcReadyYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToProcReadyYAcc);
            this.tabProcessParams.Controls.Add(this.lblProcessYSpeed);
            this.tabProcessParams.Controls.Add(this.txtProcessYSpeed);
            this.tabProcessParams.Controls.Add(this.lblProcessYAcc);
            this.tabProcessParams.Controls.Add(this.txtProcessYAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckXSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToAlignCheckYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToAlignCheckYAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadXPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadXPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadXSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadXSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadXAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadXAcc);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadYPos);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadYPos);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadYSpeed);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadYSpeed);
            this.tabProcessParams.Controls.Add(this.lblMoveToUnloadYAcc);
            this.tabProcessParams.Controls.Add(this.txtMoveToUnloadYAcc);
            this.tabProcessParams.Controls.Add(this.grpPowerMeterRecipe);
            this.tabProcessParams.Location = new System.Drawing.Point(4, 24);
            this.tabProcessParams.Name = "tabProcessParams";
            this.tabProcessParams.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcessParams.Size = new System.Drawing.Size(1088, 607);
            this.tabProcessParams.TabIndex = 0;
            this.tabProcessParams.Text = "Parameters";
            this.tabProcessParams.UseVisualStyleBackColor = true;
            // 
            // grpProcessOption
            // 
            this.grpProcessOption.Controls.Add(this.rbtnAllStepProcess);
            this.grpProcessOption.Controls.Add(this.rbtn1StepProcess);
            this.grpProcessOption.Location = new System.Drawing.Point(477, 419);
            this.grpProcessOption.Name = "grpProcessOption";
            this.grpProcessOption.Size = new System.Drawing.Size(153, 84);
            this.grpProcessOption.TabIndex = 2003;
            this.grpProcessOption.TabStop = false;
            this.grpProcessOption.Text = "Process Option";
            // 
            // rbtnAllStepProcess
            // 
            this.rbtnAllStepProcess.AutoSize = true;
            this.rbtnAllStepProcess.Location = new System.Drawing.Point(23, 53);
            this.rbtnAllStepProcess.Name = "rbtnAllStepProcess";
            this.rbtnAllStepProcess.Size = new System.Drawing.Size(111, 19);
            this.rbtnAllStepProcess.TabIndex = 0;
            this.rbtnAllStepProcess.Text = "All Step Process";
            this.rbtnAllStepProcess.UseVisualStyleBackColor = true;
            // 
            // rbtn1StepProcess
            // 
            this.rbtn1StepProcess.AutoSize = true;
            this.rbtn1StepProcess.Checked = true;
            this.rbtn1StepProcess.Location = new System.Drawing.Point(23, 26);
            this.rbtn1StepProcess.Name = "rbtn1StepProcess";
            this.rbtn1StepProcess.Size = new System.Drawing.Size(105, 19);
            this.rbtn1StepProcess.TabIndex = 0;
            this.rbtn1StepProcess.TabStop = true;
            this.rbtn1StepProcess.Text = "1-Step Process";
            this.rbtn1StepProcess.UseVisualStyleBackColor = true;
            // 
            // lalAfterSeqMs
            // 
            this.lalAfterSeqMs.AutoSize = true;
            this.lalAfterSeqMs.Location = new System.Drawing.Point(657, 449);
            this.lalAfterSeqMs.Name = "lalAfterSeqMs";
            this.lalAfterSeqMs.Size = new System.Drawing.Size(77, 15);
            this.lalAfterSeqMs.TabIndex = 97;
            this.lalAfterSeqMs.Text = "AfterSeq(ms)";
            // 
            // grpSettlingTime
            // 
            this.grpSettlingTime.Controls.Add(this.txtAutoDelayAfterMoveMs);
            this.grpSettlingTime.Controls.Add(this.lalAfterMoveMs);
            this.grpSettlingTime.Controls.Add(this.txtAutoDelayAfterSeqMs);
            this.grpSettlingTime.Location = new System.Drawing.Point(636, 419);
            this.grpSettlingTime.Name = "grpSettlingTime";
            this.grpSettlingTime.Size = new System.Drawing.Size(229, 84);
            this.grpSettlingTime.TabIndex = 2002;
            this.grpSettlingTime.TabStop = false;
            this.grpSettlingTime.Text = "Settling Time";
            // 
            // txtAutoDelayAfterMoveMs
            // 
            this.txtAutoDelayAfterMoveMs.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtAutoDelayAfterMoveMs.Location = new System.Drawing.Point(154, 55);
            this.txtAutoDelayAfterMoveMs.Name = "txtAutoDelayAfterMoveMs";
            this.txtAutoDelayAfterMoveMs.Size = new System.Drawing.Size(60, 23);
            this.txtAutoDelayAfterMoveMs.TabIndex = 2004;
            this.txtAutoDelayAfterMoveMs.Tag = "ProcessParameter:AutoDelayAfterMoveMs";
            this.txtAutoDelayAfterMoveMs.Text = "0";
            // 
            // lalAfterMoveMs
            // 
            this.lalAfterMoveMs.AutoSize = true;
            this.lalAfterMoveMs.Location = new System.Drawing.Point(21, 57);
            this.lalAfterMoveMs.Name = "lalAfterMoveMs";
            this.lalAfterMoveMs.Size = new System.Drawing.Size(87, 15);
            this.lalAfterMoveMs.TabIndex = 97;
            this.lalAfterMoveMs.Text = "AfterMove(ms)";
            // 
            // txtAutoDelayAfterSeqMs
            // 
            this.txtAutoDelayAfterSeqMs.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtAutoDelayAfterSeqMs.Location = new System.Drawing.Point(154, 25);
            this.txtAutoDelayAfterSeqMs.Name = "txtAutoDelayAfterSeqMs";
            this.txtAutoDelayAfterSeqMs.Size = new System.Drawing.Size(60, 23);
            this.txtAutoDelayAfterSeqMs.TabIndex = 2003;
            this.txtAutoDelayAfterSeqMs.Tag = "ProcessParameter:AutoDelayAfterSeqMs";
            this.txtAutoDelayAfterSeqMs.Text = "0";
            // 
            // grpInspectionOption
            // 
            this.grpInspectionOption.Controls.Add(this.rbtnStepbyStepAllInspection);
            this.grpInspectionOption.Controls.Add(this.rbtnAllInspection);
            this.grpInspectionOption.Controls.Add(this.rbtn1LineInspection);
            this.grpInspectionOption.Location = new System.Drawing.Point(477, 509);
            this.grpInspectionOption.Name = "grpInspectionOption";
            this.grpInspectionOption.Size = new System.Drawing.Size(388, 84);
            this.grpInspectionOption.TabIndex = 2001;
            this.grpInspectionOption.TabStop = false;
            this.grpInspectionOption.Text = "Review Option";
            // 
            // rbtnStepbyStepAllInspection
            // 
            this.rbtnStepbyStepAllInspection.AutoSize = true;
            this.rbtnStepbyStepAllInspection.Location = new System.Drawing.Point(159, 26);
            this.rbtnStepbyStepAllInspection.Name = "rbtnStepbyStepAllInspection";
            this.rbtnStepbyStepAllInspection.Size = new System.Drawing.Size(126, 19);
            this.rbtnStepbyStepAllInspection.TabIndex = 1;
            this.rbtnStepbyStepAllInspection.Text = "Step All Inspection";
            this.rbtnStepbyStepAllInspection.UseVisualStyleBackColor = true;
            // 
            // rbtnAllInspection
            // 
            this.rbtnAllInspection.AutoSize = true;
            this.rbtnAllInspection.Location = new System.Drawing.Point(23, 53);
            this.rbtnAllInspection.Name = "rbtnAllInspection";
            this.rbtnAllInspection.Size = new System.Drawing.Size(98, 19);
            this.rbtnAllInspection.TabIndex = 0;
            this.rbtnAllInspection.Text = "All Inspection";
            this.rbtnAllInspection.UseVisualStyleBackColor = true;
            // 
            // rbtn1LineInspection
            // 
            this.rbtn1LineInspection.AutoSize = true;
            this.rbtn1LineInspection.Checked = true;
            this.rbtn1LineInspection.Location = new System.Drawing.Point(23, 26);
            this.rbtn1LineInspection.Name = "rbtn1LineInspection";
            this.rbtn1LineInspection.Size = new System.Drawing.Size(59, 19);
            this.rbtn1LineInspection.TabIndex = 0;
            this.rbtn1LineInspection.Text = "1-Line";
            this.rbtn1LineInspection.UseVisualStyleBackColor = true;
            // 
            // lblMoveToLoadYPos
            // 
            this.lblMoveToLoadYPos.AutoSize = true;
            this.lblMoveToLoadYPos.Location = new System.Drawing.Point(10, 86);
            this.lblMoveToLoadYPos.Name = "lblMoveToLoadYPos";
            this.lblMoveToLoadYPos.Size = new System.Drawing.Size(172, 15);
            this.lblMoveToLoadYPos.TabIndex = 102;
            this.lblMoveToLoadYPos.Text = "Move To Load Y Position(mm)";
            // 
            // txtMoveToLoadYPos
            // 
            this.txtMoveToLoadYPos.Location = new System.Drawing.Point(260, 84);
            this.txtMoveToLoadYPos.Name = "txtMoveToLoadYPos";
            this.txtMoveToLoadYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadYPos.TabIndex = 103;
            this.txtMoveToLoadYPos.Tag = "ProcessParameter:MoveToLoadYPos";
            this.txtMoveToLoadYPos.Text = "0";
            // 
            // lblMoveToLoadYSpeed
            // 
            this.lblMoveToLoadYSpeed.AutoSize = true;
            this.lblMoveToLoadYSpeed.Location = new System.Drawing.Point(10, 108);
            this.lblMoveToLoadYSpeed.Name = "lblMoveToLoadYSpeed";
            this.lblMoveToLoadYSpeed.Size = new System.Drawing.Size(184, 15);
            this.lblMoveToLoadYSpeed.TabIndex = 104;
            this.lblMoveToLoadYSpeed.Text = "Move To Load Y Speed(mm/sec)";
            // 
            // txtMoveToLoadYSpeed
            // 
            this.txtMoveToLoadYSpeed.Location = new System.Drawing.Point(260, 106);
            this.txtMoveToLoadYSpeed.Name = "txtMoveToLoadYSpeed";
            this.txtMoveToLoadYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadYSpeed.TabIndex = 105;
            this.txtMoveToLoadYSpeed.Tag = "ProcessParameter:MoveToLoadYSpeed";
            this.txtMoveToLoadYSpeed.Text = "0";
            // 
            // lblMoveToLoadYAcc
            // 
            this.lblMoveToLoadYAcc.AutoSize = true;
            this.lblMoveToLoadYAcc.Location = new System.Drawing.Point(10, 130);
            this.lblMoveToLoadYAcc.Name = "lblMoveToLoadYAcc";
            this.lblMoveToLoadYAcc.Size = new System.Drawing.Size(199, 15);
            this.lblMoveToLoadYAcc.TabIndex = 106;
            this.lblMoveToLoadYAcc.Text = "Move To Load Y Accelation(mm/s²)";
            // 
            // txtMoveToLoadYAcc
            // 
            this.txtMoveToLoadYAcc.Location = new System.Drawing.Point(260, 128);
            this.txtMoveToLoadYAcc.Name = "txtMoveToLoadYAcc";
            this.txtMoveToLoadYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadYAcc.TabIndex = 107;
            this.txtMoveToLoadYAcc.Tag = "ProcessParameter:MoveToLoadYAcc";
            this.txtMoveToLoadYAcc.Text = "0";
            // 
            // lblMoveToLoadXPos
            // 
            this.lblMoveToLoadXPos.AutoSize = true;
            this.lblMoveToLoadXPos.Location = new System.Drawing.Point(10, 20);
            this.lblMoveToLoadXPos.Name = "lblMoveToLoadXPos";
            this.lblMoveToLoadXPos.Size = new System.Drawing.Size(172, 15);
            this.lblMoveToLoadXPos.TabIndex = 96;
            this.lblMoveToLoadXPos.Text = "Move To Load X Position(mm)";
            // 
            // txtMoveToLoadXPos
            // 
            this.txtMoveToLoadXPos.Location = new System.Drawing.Point(260, 18);
            this.txtMoveToLoadXPos.Name = "txtMoveToLoadXPos";
            this.txtMoveToLoadXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadXPos.TabIndex = 97;
            this.txtMoveToLoadXPos.Tag = "ProcessParameter:MoveToLoadXPos";
            this.txtMoveToLoadXPos.Text = "0";
            // 
            // lblMoveToLoadXPosSpeed
            // 
            this.lblMoveToLoadXPosSpeed.AutoSize = true;
            this.lblMoveToLoadXPosSpeed.Location = new System.Drawing.Point(10, 42);
            this.lblMoveToLoadXPosSpeed.Name = "lblMoveToLoadXPosSpeed";
            this.lblMoveToLoadXPosSpeed.Size = new System.Drawing.Size(184, 15);
            this.lblMoveToLoadXPosSpeed.TabIndex = 98;
            this.lblMoveToLoadXPosSpeed.Text = "Move To Load X Speed(mm/sec)";
            // 
            // txtMoveToLoadXSpeed
            // 
            this.txtMoveToLoadXSpeed.Location = new System.Drawing.Point(260, 40);
            this.txtMoveToLoadXSpeed.Name = "txtMoveToLoadXSpeed";
            this.txtMoveToLoadXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadXSpeed.TabIndex = 99;
            this.txtMoveToLoadXSpeed.Tag = "ProcessParameter:MoveToLoadXSpeed";
            this.txtMoveToLoadXSpeed.Text = "0";
            // 
            // lblMoveToLoadXAcc
            // 
            this.lblMoveToLoadXAcc.AutoSize = true;
            this.lblMoveToLoadXAcc.Location = new System.Drawing.Point(10, 64);
            this.lblMoveToLoadXAcc.Name = "lblMoveToLoadXAcc";
            this.lblMoveToLoadXAcc.Size = new System.Drawing.Size(199, 15);
            this.lblMoveToLoadXAcc.TabIndex = 100;
            this.lblMoveToLoadXAcc.Text = "Move To Load X Accelation(mm/s²)";
            // 
            // txtMoveToLoadXAcc
            // 
            this.txtMoveToLoadXAcc.Location = new System.Drawing.Point(260, 62);
            this.txtMoveToLoadXAcc.Name = "txtMoveToLoadXAcc";
            this.txtMoveToLoadXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToLoadXAcc.TabIndex = 101;
            this.txtMoveToLoadXAcc.Tag = "ProcessParameter:MoveToLoadXAcc";
            this.txtMoveToLoadXAcc.Text = "0";
            // 
            // lblMoveToAlignXPos
            // 
            this.lblMoveToAlignXPos.AutoSize = true;
            this.lblMoveToAlignXPos.Location = new System.Drawing.Point(10, 152);
            this.lblMoveToAlignXPos.Name = "lblMoveToAlignXPos";
            this.lblMoveToAlignXPos.Size = new System.Drawing.Size(174, 15);
            this.lblMoveToAlignXPos.TabIndex = 0;
            this.lblMoveToAlignXPos.Text = "Move To Align X Position(mm)";
            // 
            // txtMoveToAlignXPos
            // 
            this.txtMoveToAlignXPos.Location = new System.Drawing.Point(260, 150);
            this.txtMoveToAlignXPos.Name = "txtMoveToAlignXPos";
            this.txtMoveToAlignXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignXPos.TabIndex = 1;
            this.txtMoveToAlignXPos.Tag = "ProcessParameter:MoveToAlignXPos";
            this.txtMoveToAlignXPos.Text = "0";
            // 
            // lblMoveToAlignXSpeed
            // 
            this.lblMoveToAlignXSpeed.AutoSize = true;
            this.lblMoveToAlignXSpeed.Location = new System.Drawing.Point(10, 174);
            this.lblMoveToAlignXSpeed.Name = "lblMoveToAlignXSpeed";
            this.lblMoveToAlignXSpeed.Size = new System.Drawing.Size(186, 15);
            this.lblMoveToAlignXSpeed.TabIndex = 2;
            this.lblMoveToAlignXSpeed.Text = "Move To Align X Speed(mm/sec)";
            // 
            // txtMoveToAlignXSpeed
            // 
            this.txtMoveToAlignXSpeed.Location = new System.Drawing.Point(260, 172);
            this.txtMoveToAlignXSpeed.Name = "txtMoveToAlignXSpeed";
            this.txtMoveToAlignXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignXSpeed.TabIndex = 3;
            this.txtMoveToAlignXSpeed.Tag = "ProcessParameter:MoveToAlignXSpeed";
            this.txtMoveToAlignXSpeed.Text = "0";
            // 
            // lblMoveToAlignXAcc
            // 
            this.lblMoveToAlignXAcc.AutoSize = true;
            this.lblMoveToAlignXAcc.Location = new System.Drawing.Point(10, 196);
            this.lblMoveToAlignXAcc.Name = "lblMoveToAlignXAcc";
            this.lblMoveToAlignXAcc.Size = new System.Drawing.Size(201, 15);
            this.lblMoveToAlignXAcc.TabIndex = 4;
            this.lblMoveToAlignXAcc.Text = "Move To Align X Accelation(mm/s²)";
            // 
            // txtMoveToAlignXAcc
            // 
            this.txtMoveToAlignXAcc.Location = new System.Drawing.Point(260, 194);
            this.txtMoveToAlignXAcc.Name = "txtMoveToAlignXAcc";
            this.txtMoveToAlignXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignXAcc.TabIndex = 5;
            this.txtMoveToAlignXAcc.Tag = "ProcessParameter:MoveToAlignXAcc";
            this.txtMoveToAlignXAcc.Text = "0";
            // 
            // lblMoveToAlignYPos
            // 
            this.lblMoveToAlignYPos.AutoSize = true;
            this.lblMoveToAlignYPos.Location = new System.Drawing.Point(10, 218);
            this.lblMoveToAlignYPos.Name = "lblMoveToAlignYPos";
            this.lblMoveToAlignYPos.Size = new System.Drawing.Size(174, 15);
            this.lblMoveToAlignYPos.TabIndex = 6;
            this.lblMoveToAlignYPos.Text = "Move To Align Y Position(mm)";
            // 
            // txtMoveToAlignYPos
            // 
            this.txtMoveToAlignYPos.Location = new System.Drawing.Point(260, 216);
            this.txtMoveToAlignYPos.Name = "txtMoveToAlignYPos";
            this.txtMoveToAlignYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignYPos.TabIndex = 7;
            this.txtMoveToAlignYPos.Tag = "ProcessParameter:MoveToAlignYPos";
            this.txtMoveToAlignYPos.Text = "0";
            // 
            // lblMoveToAlignYSpeed
            // 
            this.lblMoveToAlignYSpeed.AutoSize = true;
            this.lblMoveToAlignYSpeed.Location = new System.Drawing.Point(10, 240);
            this.lblMoveToAlignYSpeed.Name = "lblMoveToAlignYSpeed";
            this.lblMoveToAlignYSpeed.Size = new System.Drawing.Size(186, 15);
            this.lblMoveToAlignYSpeed.TabIndex = 8;
            this.lblMoveToAlignYSpeed.Text = "Move To Align Y Speed(mm/sec)";
            // 
            // txtMoveToAlignYSpeed
            // 
            this.txtMoveToAlignYSpeed.Location = new System.Drawing.Point(260, 238);
            this.txtMoveToAlignYSpeed.Name = "txtMoveToAlignYSpeed";
            this.txtMoveToAlignYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignYSpeed.TabIndex = 9;
            this.txtMoveToAlignYSpeed.Tag = "ProcessParameter:MoveToAlignYSpeed";
            this.txtMoveToAlignYSpeed.Text = "0";
            // 
            // lblMoveToAlignYAcc
            // 
            this.lblMoveToAlignYAcc.AutoSize = true;
            this.lblMoveToAlignYAcc.Location = new System.Drawing.Point(10, 262);
            this.lblMoveToAlignYAcc.Name = "lblMoveToAlignYAcc";
            this.lblMoveToAlignYAcc.Size = new System.Drawing.Size(201, 15);
            this.lblMoveToAlignYAcc.TabIndex = 10;
            this.lblMoveToAlignYAcc.Text = "Move To Align Y Accelation(mm/s²)";
            // 
            // txtMoveToAlignYAcc
            // 
            this.txtMoveToAlignYAcc.Location = new System.Drawing.Point(260, 260);
            this.txtMoveToAlignYAcc.Name = "txtMoveToAlignYAcc";
            this.txtMoveToAlignYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignYAcc.TabIndex = 11;
            this.txtMoveToAlignYAcc.Tag = "ProcessParameter:MoveToAlignYAcc";
            this.txtMoveToAlignYAcc.Text = "0";
            // 
            // lblMoveToPWMXPos
            // 
            this.lblMoveToPWMXPos.AutoSize = true;
            this.lblMoveToPWMXPos.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMXPos.Location = new System.Drawing.Point(10, 284);
            this.lblMoveToPWMXPos.Name = "lblMoveToPWMXPos";
            this.lblMoveToPWMXPos.Size = new System.Drawing.Size(175, 15);
            this.lblMoveToPWMXPos.TabIndex = 12;
            this.lblMoveToPWMXPos.Text = "Move To PWM X Position(mm)";
            // 
            // txtMoveToPWMXPos
            // 
            this.txtMoveToPWMXPos.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMXPos.Location = new System.Drawing.Point(260, 282);
            this.txtMoveToPWMXPos.Name = "txtMoveToPWMXPos";
            this.txtMoveToPWMXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMXPos.TabIndex = 13;
            this.txtMoveToPWMXPos.Tag = "ProcessParameter:MoveToPWMXPos";
            this.txtMoveToPWMXPos.Text = "0";
            // 
            // lblMoveToPWMXSpeed
            // 
            this.lblMoveToPWMXSpeed.AutoSize = true;
            this.lblMoveToPWMXSpeed.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMXSpeed.Location = new System.Drawing.Point(10, 306);
            this.lblMoveToPWMXSpeed.Name = "lblMoveToPWMXSpeed";
            this.lblMoveToPWMXSpeed.Size = new System.Drawing.Size(187, 15);
            this.lblMoveToPWMXSpeed.TabIndex = 14;
            this.lblMoveToPWMXSpeed.Text = "Move To PWM X Speed(mm/sec)";
            // 
            // txtMoveToPWMXSpeed
            // 
            this.txtMoveToPWMXSpeed.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMXSpeed.Location = new System.Drawing.Point(260, 304);
            this.txtMoveToPWMXSpeed.Name = "txtMoveToPWMXSpeed";
            this.txtMoveToPWMXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMXSpeed.TabIndex = 15;
            this.txtMoveToPWMXSpeed.Tag = "ProcessParameter:MoveToPWMXSpeed";
            this.txtMoveToPWMXSpeed.Text = "0";
            // 
            // lblMoveToPWMXAcc
            // 
            this.lblMoveToPWMXAcc.AutoSize = true;
            this.lblMoveToPWMXAcc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMXAcc.Location = new System.Drawing.Point(10, 328);
            this.lblMoveToPWMXAcc.Name = "lblMoveToPWMXAcc";
            this.lblMoveToPWMXAcc.Size = new System.Drawing.Size(202, 15);
            this.lblMoveToPWMXAcc.TabIndex = 16;
            this.lblMoveToPWMXAcc.Text = "Move To PWM X Accelation(mm/s²)";
            // 
            // txtMoveToPWMXAcc
            // 
            this.txtMoveToPWMXAcc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMXAcc.Location = new System.Drawing.Point(260, 326);
            this.txtMoveToPWMXAcc.Name = "txtMoveToPWMXAcc";
            this.txtMoveToPWMXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMXAcc.TabIndex = 17;
            this.txtMoveToPWMXAcc.Tag = "ProcessParameter:MoveToPWMXAcc";
            this.txtMoveToPWMXAcc.Text = "0";
            // 
            // lblMoveToPWMYPos
            // 
            this.lblMoveToPWMYPos.AutoSize = true;
            this.lblMoveToPWMYPos.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMYPos.Location = new System.Drawing.Point(10, 350);
            this.lblMoveToPWMYPos.Name = "lblMoveToPWMYPos";
            this.lblMoveToPWMYPos.Size = new System.Drawing.Size(175, 15);
            this.lblMoveToPWMYPos.TabIndex = 18;
            this.lblMoveToPWMYPos.Text = "Move To PWM Y Position(mm)";
            // 
            // txtMoveToPWMYPos
            // 
            this.txtMoveToPWMYPos.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMYPos.Location = new System.Drawing.Point(260, 348);
            this.txtMoveToPWMYPos.Name = "txtMoveToPWMYPos";
            this.txtMoveToPWMYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMYPos.TabIndex = 19;
            this.txtMoveToPWMYPos.Tag = "ProcessParameter:MoveToPWMYPos";
            this.txtMoveToPWMYPos.Text = "0";
            // 
            // lblMoveToPWMYSpeed
            // 
            this.lblMoveToPWMYSpeed.AutoSize = true;
            this.lblMoveToPWMYSpeed.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMYSpeed.Location = new System.Drawing.Point(10, 372);
            this.lblMoveToPWMYSpeed.Name = "lblMoveToPWMYSpeed";
            this.lblMoveToPWMYSpeed.Size = new System.Drawing.Size(187, 15);
            this.lblMoveToPWMYSpeed.TabIndex = 20;
            this.lblMoveToPWMYSpeed.Text = "Move To PWM Y Speed(mm/sec)";
            // 
            // txtMoveToPWMYSpeed
            // 
            this.txtMoveToPWMYSpeed.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMYSpeed.Location = new System.Drawing.Point(260, 370);
            this.txtMoveToPWMYSpeed.Name = "txtMoveToPWMYSpeed";
            this.txtMoveToPWMYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMYSpeed.TabIndex = 21;
            this.txtMoveToPWMYSpeed.Tag = "ProcessParameter:MoveToPWMYSpeed";
            this.txtMoveToPWMYSpeed.Text = "0";
            // 
            // lblMoveToPWMYAcc
            // 
            this.lblMoveToPWMYAcc.AutoSize = true;
            this.lblMoveToPWMYAcc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblMoveToPWMYAcc.Location = new System.Drawing.Point(10, 394);
            this.lblMoveToPWMYAcc.Name = "lblMoveToPWMYAcc";
            this.lblMoveToPWMYAcc.Size = new System.Drawing.Size(202, 15);
            this.lblMoveToPWMYAcc.TabIndex = 22;
            this.lblMoveToPWMYAcc.Text = "Move To PWM Y Accelation(mm/s²)";
            // 
            // txtMoveToPWMYAcc
            // 
            this.txtMoveToPWMYAcc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMoveToPWMYAcc.Location = new System.Drawing.Point(260, 392);
            this.txtMoveToPWMYAcc.Name = "txtMoveToPWMYAcc";
            this.txtMoveToPWMYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToPWMYAcc.TabIndex = 23;
            this.txtMoveToPWMYAcc.Tag = "ProcessParameter:MoveToPWMYAcc";
            this.txtMoveToPWMYAcc.Text = "0";
            // 
            // lblMoveToProcReadyXPos
            // 
            this.lblMoveToProcReadyXPos.AutoSize = true;
            this.lblMoveToProcReadyXPos.Location = new System.Drawing.Point(10, 416);
            this.lblMoveToProcReadyXPos.Name = "lblMoveToProcReadyXPos";
            this.lblMoveToProcReadyXPos.Size = new System.Drawing.Size(206, 15);
            this.lblMoveToProcReadyXPos.TabIndex = 24;
            this.lblMoveToProcReadyXPos.Text = "Move To Proc Ready X Position(mm)";
            // 
            // txtMoveToProcReadyXPos
            // 
            this.txtMoveToProcReadyXPos.Location = new System.Drawing.Point(260, 414);
            this.txtMoveToProcReadyXPos.Name = "txtMoveToProcReadyXPos";
            this.txtMoveToProcReadyXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyXPos.TabIndex = 25;
            this.txtMoveToProcReadyXPos.Tag = "ProcessParameter:MoveToProcReadyXPos";
            this.txtMoveToProcReadyXPos.Text = "0";
            // 
            // lblMoveToProcReadyXSpeed
            // 
            this.lblMoveToProcReadyXSpeed.AutoSize = true;
            this.lblMoveToProcReadyXSpeed.Location = new System.Drawing.Point(10, 438);
            this.lblMoveToProcReadyXSpeed.Name = "lblMoveToProcReadyXSpeed";
            this.lblMoveToProcReadyXSpeed.Size = new System.Drawing.Size(218, 15);
            this.lblMoveToProcReadyXSpeed.TabIndex = 26;
            this.lblMoveToProcReadyXSpeed.Text = "Move To Proc Ready X Speed(mm/sec)";
            // 
            // txtMoveToProcReadyXSpeed
            // 
            this.txtMoveToProcReadyXSpeed.Location = new System.Drawing.Point(260, 436);
            this.txtMoveToProcReadyXSpeed.Name = "txtMoveToProcReadyXSpeed";
            this.txtMoveToProcReadyXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyXSpeed.TabIndex = 27;
            this.txtMoveToProcReadyXSpeed.Tag = "ProcessParameter:MoveToProcReadyXSpeed";
            this.txtMoveToProcReadyXSpeed.Text = "0";
            // 
            // lblMoveToProcReadyXAcc
            // 
            this.lblMoveToProcReadyXAcc.AutoSize = true;
            this.lblMoveToProcReadyXAcc.Location = new System.Drawing.Point(10, 460);
            this.lblMoveToProcReadyXAcc.Name = "lblMoveToProcReadyXAcc";
            this.lblMoveToProcReadyXAcc.Size = new System.Drawing.Size(233, 15);
            this.lblMoveToProcReadyXAcc.TabIndex = 28;
            this.lblMoveToProcReadyXAcc.Text = "Move To Proc Ready X Accelation(mm/s²)";
            // 
            // txtMoveToProcReadyXAcc
            // 
            this.txtMoveToProcReadyXAcc.Location = new System.Drawing.Point(260, 458);
            this.txtMoveToProcReadyXAcc.Name = "txtMoveToProcReadyXAcc";
            this.txtMoveToProcReadyXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyXAcc.TabIndex = 29;
            this.txtMoveToProcReadyXAcc.Tag = "ProcessParameter:MoveToProcReadyXAcc";
            this.txtMoveToProcReadyXAcc.Text = "0";
            // 
            // lblMoveToProcReadyYPos
            // 
            this.lblMoveToProcReadyYPos.AutoSize = true;
            this.lblMoveToProcReadyYPos.Location = new System.Drawing.Point(10, 482);
            this.lblMoveToProcReadyYPos.Name = "lblMoveToProcReadyYPos";
            this.lblMoveToProcReadyYPos.Size = new System.Drawing.Size(206, 15);
            this.lblMoveToProcReadyYPos.TabIndex = 30;
            this.lblMoveToProcReadyYPos.Text = "Move To Proc Ready Y Position(mm)";
            // 
            // txtMoveToProcReadyYPos
            // 
            this.txtMoveToProcReadyYPos.Location = new System.Drawing.Point(260, 480);
            this.txtMoveToProcReadyYPos.Name = "txtMoveToProcReadyYPos";
            this.txtMoveToProcReadyYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyYPos.TabIndex = 31;
            this.txtMoveToProcReadyYPos.Tag = "ProcessParameter:MoveToProcReadyYPos";
            this.txtMoveToProcReadyYPos.Text = "0";
            // 
            // lblMoveToProcReadyYSpeed
            // 
            this.lblMoveToProcReadyYSpeed.AutoSize = true;
            this.lblMoveToProcReadyYSpeed.Location = new System.Drawing.Point(10, 504);
            this.lblMoveToProcReadyYSpeed.Name = "lblMoveToProcReadyYSpeed";
            this.lblMoveToProcReadyYSpeed.Size = new System.Drawing.Size(218, 15);
            this.lblMoveToProcReadyYSpeed.TabIndex = 32;
            this.lblMoveToProcReadyYSpeed.Text = "Move To Proc Ready Y Speed(mm/sec)";
            // 
            // txtMoveToProcReadyYSpeed
            // 
            this.txtMoveToProcReadyYSpeed.Location = new System.Drawing.Point(260, 502);
            this.txtMoveToProcReadyYSpeed.Name = "txtMoveToProcReadyYSpeed";
            this.txtMoveToProcReadyYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyYSpeed.TabIndex = 33;
            this.txtMoveToProcReadyYSpeed.Tag = "ProcessParameter:MoveToProcReadyYSpeed";
            this.txtMoveToProcReadyYSpeed.Text = "0";
            // 
            // lblMoveToProcReadyYAcc
            // 
            this.lblMoveToProcReadyYAcc.AutoSize = true;
            this.lblMoveToProcReadyYAcc.Location = new System.Drawing.Point(10, 526);
            this.lblMoveToProcReadyYAcc.Name = "lblMoveToProcReadyYAcc";
            this.lblMoveToProcReadyYAcc.Size = new System.Drawing.Size(233, 15);
            this.lblMoveToProcReadyYAcc.TabIndex = 34;
            this.lblMoveToProcReadyYAcc.Text = "Move To Proc Ready Y Accelation(mm/s²)";
            // 
            // txtMoveToProcReadyYAcc
            // 
            this.txtMoveToProcReadyYAcc.Location = new System.Drawing.Point(260, 524);
            this.txtMoveToProcReadyYAcc.Name = "txtMoveToProcReadyYAcc";
            this.txtMoveToProcReadyYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcReadyYAcc.TabIndex = 35;
            this.txtMoveToProcReadyYAcc.Tag = "ProcessParameter:MoveToProcReadyYAcc";
            this.txtMoveToProcReadyYAcc.Text = "0";
            // 
            // lblProcessYSpeed
            // 
            this.lblProcessYSpeed.AutoSize = true;
            this.lblProcessYSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProcessYSpeed.Location = new System.Drawing.Point(10, 548);
            this.lblProcessYSpeed.Name = "lblProcessYSpeed";
            this.lblProcessYSpeed.Size = new System.Drawing.Size(147, 15);
            this.lblProcessYSpeed.TabIndex = 44;
            this.lblProcessYSpeed.Text = "Process Y Speed(mm/sec)";
            // 
            // txtProcessYSpeed
            // 
            this.txtProcessYSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtProcessYSpeed.Location = new System.Drawing.Point(260, 546);
            this.txtProcessYSpeed.Name = "txtProcessYSpeed";
            this.txtProcessYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtProcessYSpeed.TabIndex = 45;
            this.txtProcessYSpeed.Tag = "ProcessParameter:ProcessYSpeed";
            this.txtProcessYSpeed.Text = "0";
            // 
            // lblProcessYAcc
            // 
            this.lblProcessYAcc.AutoSize = true;
            this.lblProcessYAcc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProcessYAcc.Location = new System.Drawing.Point(10, 570);
            this.lblProcessYAcc.Name = "lblProcessYAcc";
            this.lblProcessYAcc.Size = new System.Drawing.Size(162, 15);
            this.lblProcessYAcc.TabIndex = 46;
            this.lblProcessYAcc.Text = "Process Y Accelation(mm/s²)";
            // 
            // txtProcessYAcc
            // 
            this.txtProcessYAcc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtProcessYAcc.Location = new System.Drawing.Point(260, 568);
            this.txtProcessYAcc.Name = "txtProcessYAcc";
            this.txtProcessYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtProcessYAcc.TabIndex = 47;
            this.txtProcessYAcc.Tag = "ProcessParameter:ProcessYAcc";
            this.txtProcessYAcc.Text = "0";
            // 
            // lblMoveToAlignCheckXPos
            // 
            this.lblMoveToAlignCheckXPos.AutoSize = true;
            this.lblMoveToAlignCheckXPos.Location = new System.Drawing.Point(485, 18);
            this.lblMoveToAlignCheckXPos.Name = "lblMoveToAlignCheckXPos";
            this.lblMoveToAlignCheckXPos.Size = new System.Drawing.Size(211, 15);
            this.lblMoveToAlignCheckXPos.TabIndex = 72;
            this.lblMoveToAlignCheckXPos.Text = "Move To Align Check X Position(mm)";
            // 
            // txtMoveToAlignCheckXPos
            // 
            this.txtMoveToAlignCheckXPos.Location = new System.Drawing.Point(805, 16);
            this.txtMoveToAlignCheckXPos.Name = "txtMoveToAlignCheckXPos";
            this.txtMoveToAlignCheckXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckXPos.TabIndex = 73;
            this.txtMoveToAlignCheckXPos.Tag = "ProcessParameter:MoveToAlignCheckXPos";
            this.txtMoveToAlignCheckXPos.Text = "0";
            // 
            // lblMoveToAlignCheckXSpeed
            // 
            this.lblMoveToAlignCheckXSpeed.AutoSize = true;
            this.lblMoveToAlignCheckXSpeed.Location = new System.Drawing.Point(485, 40);
            this.lblMoveToAlignCheckXSpeed.Name = "lblMoveToAlignCheckXSpeed";
            this.lblMoveToAlignCheckXSpeed.Size = new System.Drawing.Size(223, 15);
            this.lblMoveToAlignCheckXSpeed.TabIndex = 74;
            this.lblMoveToAlignCheckXSpeed.Text = "Move To Align Check X Speed(mm/sec)";
            // 
            // txtMoveToAlignCheckXSpeed
            // 
            this.txtMoveToAlignCheckXSpeed.Location = new System.Drawing.Point(805, 38);
            this.txtMoveToAlignCheckXSpeed.Name = "txtMoveToAlignCheckXSpeed";
            this.txtMoveToAlignCheckXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckXSpeed.TabIndex = 75;
            this.txtMoveToAlignCheckXSpeed.Tag = "ProcessParameter:MoveToAlignCheckXSpeed";
            this.txtMoveToAlignCheckXSpeed.Text = "0";
            // 
            // lblMoveToAlignCheckXAcc
            // 
            this.lblMoveToAlignCheckXAcc.AutoSize = true;
            this.lblMoveToAlignCheckXAcc.Location = new System.Drawing.Point(485, 62);
            this.lblMoveToAlignCheckXAcc.Name = "lblMoveToAlignCheckXAcc";
            this.lblMoveToAlignCheckXAcc.Size = new System.Drawing.Size(238, 15);
            this.lblMoveToAlignCheckXAcc.TabIndex = 76;
            this.lblMoveToAlignCheckXAcc.Text = "Move To Align Check X Accelation(mm/s²)";
            // 
            // txtMoveToAlignCheckXAcc
            // 
            this.txtMoveToAlignCheckXAcc.Location = new System.Drawing.Point(805, 60);
            this.txtMoveToAlignCheckXAcc.Name = "txtMoveToAlignCheckXAcc";
            this.txtMoveToAlignCheckXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckXAcc.TabIndex = 77;
            this.txtMoveToAlignCheckXAcc.Tag = "ProcessParameter:MoveToAlignCheckXAcc";
            this.txtMoveToAlignCheckXAcc.Text = "0";
            // 
            // lblMoveToAlignCheckYPos
            // 
            this.lblMoveToAlignCheckYPos.AutoSize = true;
            this.lblMoveToAlignCheckYPos.Location = new System.Drawing.Point(485, 84);
            this.lblMoveToAlignCheckYPos.Name = "lblMoveToAlignCheckYPos";
            this.lblMoveToAlignCheckYPos.Size = new System.Drawing.Size(211, 15);
            this.lblMoveToAlignCheckYPos.TabIndex = 78;
            this.lblMoveToAlignCheckYPos.Text = "Move To Align Check Y Position(mm)";
            // 
            // txtMoveToAlignCheckYPos
            // 
            this.txtMoveToAlignCheckYPos.Location = new System.Drawing.Point(805, 82);
            this.txtMoveToAlignCheckYPos.Name = "txtMoveToAlignCheckYPos";
            this.txtMoveToAlignCheckYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckYPos.TabIndex = 79;
            this.txtMoveToAlignCheckYPos.Tag = "ProcessParameter:MoveToAlignCheckYPos";
            this.txtMoveToAlignCheckYPos.Text = "0";
            // 
            // lblMoveToAlignCheckYSpeed
            // 
            this.lblMoveToAlignCheckYSpeed.AutoSize = true;
            this.lblMoveToAlignCheckYSpeed.Location = new System.Drawing.Point(485, 106);
            this.lblMoveToAlignCheckYSpeed.Name = "lblMoveToAlignCheckYSpeed";
            this.lblMoveToAlignCheckYSpeed.Size = new System.Drawing.Size(223, 15);
            this.lblMoveToAlignCheckYSpeed.TabIndex = 80;
            this.lblMoveToAlignCheckYSpeed.Text = "Move To Align Check Y Speed(mm/sec)";
            // 
            // txtMoveToAlignCheckYSpeed
            // 
            this.txtMoveToAlignCheckYSpeed.Location = new System.Drawing.Point(805, 104);
            this.txtMoveToAlignCheckYSpeed.Name = "txtMoveToAlignCheckYSpeed";
            this.txtMoveToAlignCheckYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckYSpeed.TabIndex = 81;
            this.txtMoveToAlignCheckYSpeed.Tag = "ProcessParameter:MoveToAlignCheckYSpeed";
            this.txtMoveToAlignCheckYSpeed.Text = "0";
            // 
            // lblMoveToAlignCheckYAcc
            // 
            this.lblMoveToAlignCheckYAcc.AutoSize = true;
            this.lblMoveToAlignCheckYAcc.Location = new System.Drawing.Point(485, 128);
            this.lblMoveToAlignCheckYAcc.Name = "lblMoveToAlignCheckYAcc";
            this.lblMoveToAlignCheckYAcc.Size = new System.Drawing.Size(238, 15);
            this.lblMoveToAlignCheckYAcc.TabIndex = 82;
            this.lblMoveToAlignCheckYAcc.Text = "Move To Align Check Y Accelation(mm/s²)";
            // 
            // txtMoveToAlignCheckYAcc
            // 
            this.txtMoveToAlignCheckYAcc.Location = new System.Drawing.Point(805, 126);
            this.txtMoveToAlignCheckYAcc.Name = "txtMoveToAlignCheckYAcc";
            this.txtMoveToAlignCheckYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToAlignCheckYAcc.TabIndex = 83;
            this.txtMoveToAlignCheckYAcc.Tag = "ProcessParameter:MoveToAlignCheckYAcc";
            this.txtMoveToAlignCheckYAcc.Text = "0";
            // 
            // lblMoveToUnloadXPos
            // 
            this.lblMoveToUnloadXPos.AutoSize = true;
            this.lblMoveToUnloadXPos.Location = new System.Drawing.Point(485, 150);
            this.lblMoveToUnloadXPos.Name = "lblMoveToUnloadXPos";
            this.lblMoveToUnloadXPos.Size = new System.Drawing.Size(184, 15);
            this.lblMoveToUnloadXPos.TabIndex = 84;
            this.lblMoveToUnloadXPos.Text = "Move To Unload X Position(mm)";
            // 
            // txtMoveToUnloadXPos
            // 
            this.txtMoveToUnloadXPos.Location = new System.Drawing.Point(805, 148);
            this.txtMoveToUnloadXPos.Name = "txtMoveToUnloadXPos";
            this.txtMoveToUnloadXPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadXPos.TabIndex = 85;
            this.txtMoveToUnloadXPos.Tag = "ProcessParameter:MoveToUnloadXPos";
            this.txtMoveToUnloadXPos.Text = "0";
            // 
            // lblMoveToUnloadXSpeed
            // 
            this.lblMoveToUnloadXSpeed.AutoSize = true;
            this.lblMoveToUnloadXSpeed.Location = new System.Drawing.Point(485, 172);
            this.lblMoveToUnloadXSpeed.Name = "lblMoveToUnloadXSpeed";
            this.lblMoveToUnloadXSpeed.Size = new System.Drawing.Size(196, 15);
            this.lblMoveToUnloadXSpeed.TabIndex = 86;
            this.lblMoveToUnloadXSpeed.Text = "Move To Unload X Speed(mm/sec)";
            // 
            // txtMoveToUnloadXSpeed
            // 
            this.txtMoveToUnloadXSpeed.Location = new System.Drawing.Point(805, 170);
            this.txtMoveToUnloadXSpeed.Name = "txtMoveToUnloadXSpeed";
            this.txtMoveToUnloadXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadXSpeed.TabIndex = 87;
            this.txtMoveToUnloadXSpeed.Tag = "ProcessParameter:MoveToUnloadXSpeed";
            this.txtMoveToUnloadXSpeed.Text = "0";
            // 
            // lblMoveToUnloadXAcc
            // 
            this.lblMoveToUnloadXAcc.AutoSize = true;
            this.lblMoveToUnloadXAcc.Location = new System.Drawing.Point(485, 194);
            this.lblMoveToUnloadXAcc.Name = "lblMoveToUnloadXAcc";
            this.lblMoveToUnloadXAcc.Size = new System.Drawing.Size(211, 15);
            this.lblMoveToUnloadXAcc.TabIndex = 88;
            this.lblMoveToUnloadXAcc.Text = "Move To Unload X Accelation(mm/s²)";
            // 
            // txtMoveToUnloadXAcc
            // 
            this.txtMoveToUnloadXAcc.Location = new System.Drawing.Point(805, 192);
            this.txtMoveToUnloadXAcc.Name = "txtMoveToUnloadXAcc";
            this.txtMoveToUnloadXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadXAcc.TabIndex = 89;
            this.txtMoveToUnloadXAcc.Tag = "ProcessParameter:MoveToUnloadXAcc";
            this.txtMoveToUnloadXAcc.Text = "0";
            // 
            // lblMoveToUnloadYPos
            // 
            this.lblMoveToUnloadYPos.AutoSize = true;
            this.lblMoveToUnloadYPos.Location = new System.Drawing.Point(485, 216);
            this.lblMoveToUnloadYPos.Name = "lblMoveToUnloadYPos";
            this.lblMoveToUnloadYPos.Size = new System.Drawing.Size(184, 15);
            this.lblMoveToUnloadYPos.TabIndex = 90;
            this.lblMoveToUnloadYPos.Text = "Move To Unload Y Position(mm)";
            // 
            // txtMoveToUnloadYPos
            // 
            this.txtMoveToUnloadYPos.Location = new System.Drawing.Point(805, 214);
            this.txtMoveToUnloadYPos.Name = "txtMoveToUnloadYPos";
            this.txtMoveToUnloadYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadYPos.TabIndex = 91;
            this.txtMoveToUnloadYPos.Tag = "ProcessParameter:MoveToUnloadYPos";
            this.txtMoveToUnloadYPos.Text = "0";
            // 
            // lblMoveToUnloadYSpeed
            // 
            this.lblMoveToUnloadYSpeed.AutoSize = true;
            this.lblMoveToUnloadYSpeed.Location = new System.Drawing.Point(485, 238);
            this.lblMoveToUnloadYSpeed.Name = "lblMoveToUnloadYSpeed";
            this.lblMoveToUnloadYSpeed.Size = new System.Drawing.Size(196, 15);
            this.lblMoveToUnloadYSpeed.TabIndex = 92;
            this.lblMoveToUnloadYSpeed.Text = "Move To Unload Y Speed(mm/sec)";
            // 
            // txtMoveToUnloadYSpeed
            // 
            this.txtMoveToUnloadYSpeed.Location = new System.Drawing.Point(805, 236);
            this.txtMoveToUnloadYSpeed.Name = "txtMoveToUnloadYSpeed";
            this.txtMoveToUnloadYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadYSpeed.TabIndex = 93;
            this.txtMoveToUnloadYSpeed.Tag = "ProcessParameter:MoveToUnloadYSpeed";
            this.txtMoveToUnloadYSpeed.Text = "0";
            // 
            // lblMoveToUnloadYAcc
            // 
            this.lblMoveToUnloadYAcc.AutoSize = true;
            this.lblMoveToUnloadYAcc.Location = new System.Drawing.Point(485, 260);
            this.lblMoveToUnloadYAcc.Name = "lblMoveToUnloadYAcc";
            this.lblMoveToUnloadYAcc.Size = new System.Drawing.Size(211, 15);
            this.lblMoveToUnloadYAcc.TabIndex = 94;
            this.lblMoveToUnloadYAcc.Text = "Move To Unload Y Accelation(mm/s²)";
            // 
            // txtMoveToUnloadYAcc
            // 
            this.txtMoveToUnloadYAcc.Location = new System.Drawing.Point(805, 258);
            this.txtMoveToUnloadYAcc.Name = "txtMoveToUnloadYAcc";
            this.txtMoveToUnloadYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToUnloadYAcc.TabIndex = 95;
            this.txtMoveToUnloadYAcc.Tag = "ProcessParameter:MoveToUnloadYAcc";
            this.txtMoveToUnloadYAcc.Text = "0";
            // 
            // grpPowerMeterRecipe
            // 
            this.grpPowerMeterRecipe.Controls.Add(this.lstPmRecipeList);
            this.grpPowerMeterRecipe.Controls.Add(this.btnPmRefresh);
            this.grpPowerMeterRecipe.Controls.Add(this.lblPmSelectedDisplay);
            this.grpPowerMeterRecipe.Controls.Add(this.txtSelectedPmRecipeName);
            this.grpPowerMeterRecipe.Location = new System.Drawing.Point(477, 293);
            this.grpPowerMeterRecipe.Name = "grpPowerMeterRecipe";
            this.grpPowerMeterRecipe.Size = new System.Drawing.Size(461, 116);
            this.grpPowerMeterRecipe.TabIndex = 2000;
            this.grpPowerMeterRecipe.TabStop = false;
            this.grpPowerMeterRecipe.Text = "Powermeter Recipe (Select Only)";
            // 
            // lstPmRecipeList
            // 
            this.lstPmRecipeList.FormattingEnabled = true;
            this.lstPmRecipeList.ItemHeight = 15;
            this.lstPmRecipeList.Location = new System.Drawing.Point(10, 26);
            this.lstPmRecipeList.Name = "lstPmRecipeList";
            this.lstPmRecipeList.Size = new System.Drawing.Size(210, 79);
            this.lstPmRecipeList.TabIndex = 2001;
            this.lstPmRecipeList.SelectedIndexChanged += new System.EventHandler(this.lstPmRecipeList_SelectedIndexChanged);
            // 
            // btnPmRefresh
            // 
            this.btnPmRefresh.Location = new System.Drawing.Point(233, 26);
            this.btnPmRefresh.Name = "btnPmRefresh";
            this.btnPmRefresh.Size = new System.Drawing.Size(90, 23);
            this.btnPmRefresh.TabIndex = 2002;
            this.btnPmRefresh.Text = "Refresh";
            this.btnPmRefresh.UseVisualStyleBackColor = true;
            this.btnPmRefresh.Click += new System.EventHandler(this.btnPmRefresh_Click);
            // 
            // lblPmSelectedDisplay
            // 
            this.lblPmSelectedDisplay.AutoSize = true;
            this.lblPmSelectedDisplay.Location = new System.Drawing.Point(233, 56);
            this.lblPmSelectedDisplay.Name = "lblPmSelectedDisplay";
            this.lblPmSelectedDisplay.Size = new System.Drawing.Size(96, 15);
            this.lblPmSelectedDisplay.TabIndex = 2003;
            this.lblPmSelectedDisplay.Text = "Selected: (None)";
            // 
            // txtSelectedPmRecipeName
            // 
            this.txtSelectedPmRecipeName.Location = new System.Drawing.Point(233, 82);
            this.txtSelectedPmRecipeName.Name = "txtSelectedPmRecipeName";
            this.txtSelectedPmRecipeName.ReadOnly = true;
            this.txtSelectedPmRecipeName.Size = new System.Drawing.Size(170, 23);
            this.txtSelectedPmRecipeName.TabIndex = 2004;
            this.txtSelectedPmRecipeName.Tag = "ProcessParameter:PowerMeterRecipeName";
            // 
            // lblProcessYPos
            // 
            this.lblProcessYPos.AutoSize = true;
            this.lblProcessYPos.Enabled = false;
            this.lblProcessYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblProcessYPos.Location = new System.Drawing.Point(12, 112);
            this.lblProcessYPos.Name = "lblProcessYPos";
            this.lblProcessYPos.Size = new System.Drawing.Size(135, 15);
            this.lblProcessYPos.TabIndex = 42;
            this.lblProcessYPos.Text = "Process Y Position(mm)";
            this.lblProcessYPos.Visible = false;
            // 
            // txtProcessYPos
            // 
            this.txtProcessYPos.Enabled = false;
            this.txtProcessYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtProcessYPos.Location = new System.Drawing.Point(262, 110);
            this.txtProcessYPos.Name = "txtProcessYPos";
            this.txtProcessYPos.Size = new System.Drawing.Size(60, 23);
            this.txtProcessYPos.TabIndex = 43;
            this.txtProcessYPos.Tag = "ProcessParameter:ProcessYPos";
            this.txtProcessYPos.Text = "0";
            this.txtProcessYPos.Visible = false;
            // 
            // lblMoveToProcessEndYPos
            // 
            this.lblMoveToProcessEndYPos.AutoSize = true;
            this.lblMoveToProcessEndYPos.Enabled = false;
            this.lblMoveToProcessEndYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToProcessEndYPos.Location = new System.Drawing.Point(-3, 17);
            this.lblMoveToProcessEndYPos.Name = "lblMoveToProcessEndYPos";
            this.lblMoveToProcessEndYPos.Size = new System.Drawing.Size(210, 15);
            this.lblMoveToProcessEndYPos.TabIndex = 48;
            this.lblMoveToProcessEndYPos.Text = "Move To Process End Y Position(mm)";
            // 
            // txtMoveToProcessEndYPos
            // 
            this.txtMoveToProcessEndYPos.Enabled = false;
            this.txtMoveToProcessEndYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToProcessEndYPos.Location = new System.Drawing.Point(357, 15);
            this.txtMoveToProcessEndYPos.Name = "txtMoveToProcessEndYPos";
            this.txtMoveToProcessEndYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcessEndYPos.TabIndex = 49;
            this.txtMoveToProcessEndYPos.Tag = "ProcessParameter:MoveToProcessEndYPos";
            this.txtMoveToProcessEndYPos.Text = "0";
            // 
            // lblMoveToProcessEndYSpeed
            // 
            this.lblMoveToProcessEndYSpeed.AutoSize = true;
            this.lblMoveToProcessEndYSpeed.Enabled = false;
            this.lblMoveToProcessEndYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToProcessEndYSpeed.Location = new System.Drawing.Point(-3, 39);
            this.lblMoveToProcessEndYSpeed.Name = "lblMoveToProcessEndYSpeed";
            this.lblMoveToProcessEndYSpeed.Size = new System.Drawing.Size(222, 15);
            this.lblMoveToProcessEndYSpeed.TabIndex = 50;
            this.lblMoveToProcessEndYSpeed.Text = "Move To Process End Y Speed(mm/sec)";
            // 
            // txtMoveToProcessEndYSpeed
            // 
            this.txtMoveToProcessEndYSpeed.Enabled = false;
            this.txtMoveToProcessEndYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToProcessEndYSpeed.Location = new System.Drawing.Point(357, 37);
            this.txtMoveToProcessEndYSpeed.Name = "txtMoveToProcessEndYSpeed";
            this.txtMoveToProcessEndYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcessEndYSpeed.TabIndex = 51;
            this.txtMoveToProcessEndYSpeed.Tag = "ProcessParameter:MoveToProcessEndYSpeed";
            this.txtMoveToProcessEndYSpeed.Text = "0";
            // 
            // lblMoveToProcessEndYAcc
            // 
            this.lblMoveToProcessEndYAcc.AutoSize = true;
            this.lblMoveToProcessEndYAcc.Enabled = false;
            this.lblMoveToProcessEndYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToProcessEndYAcc.Location = new System.Drawing.Point(-3, 61);
            this.lblMoveToProcessEndYAcc.Name = "lblMoveToProcessEndYAcc";
            this.lblMoveToProcessEndYAcc.Size = new System.Drawing.Size(237, 15);
            this.lblMoveToProcessEndYAcc.TabIndex = 52;
            this.lblMoveToProcessEndYAcc.Text = "Move To Process End Y Accelation(mm/s²)";
            // 
            // txtMoveToProcessEndYAcc
            // 
            this.txtMoveToProcessEndYAcc.Enabled = false;
            this.txtMoveToProcessEndYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToProcessEndYAcc.Location = new System.Drawing.Point(357, 59);
            this.txtMoveToProcessEndYAcc.Name = "txtMoveToProcessEndYAcc";
            this.txtMoveToProcessEndYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToProcessEndYAcc.TabIndex = 53;
            this.txtMoveToProcessEndYAcc.Tag = "ProcessParameter:MoveToProcessEndYAcc";
            this.txtMoveToProcessEndYAcc.Text = "0";
            // 
            // lblMoveToInspectionYPos
            // 
            this.lblMoveToInspectionYPos.AutoSize = true;
            this.lblMoveToInspectionYPos.Enabled = false;
            this.lblMoveToInspectionYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToInspectionYPos.Location = new System.Drawing.Point(-3, 83);
            this.lblMoveToInspectionYPos.Name = "lblMoveToInspectionYPos";
            this.lblMoveToInspectionYPos.Size = new System.Drawing.Size(201, 15);
            this.lblMoveToInspectionYPos.TabIndex = 54;
            this.lblMoveToInspectionYPos.Text = "Move To Inspection Y Position(mm)";
            // 
            // txtMoveToInspectionYPos
            // 
            this.txtMoveToInspectionYPos.Enabled = false;
            this.txtMoveToInspectionYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToInspectionYPos.Location = new System.Drawing.Point(357, 81);
            this.txtMoveToInspectionYPos.Name = "txtMoveToInspectionYPos";
            this.txtMoveToInspectionYPos.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToInspectionYPos.TabIndex = 55;
            this.txtMoveToInspectionYPos.Tag = "ProcessParameter:MoveToInspectionYPos";
            this.txtMoveToInspectionYPos.Text = "0";
            // 
            // lblMoveToInspectionYSpeed
            // 
            this.lblMoveToInspectionYSpeed.AutoSize = true;
            this.lblMoveToInspectionYSpeed.Enabled = false;
            this.lblMoveToInspectionYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToInspectionYSpeed.Location = new System.Drawing.Point(-3, 105);
            this.lblMoveToInspectionYSpeed.Name = "lblMoveToInspectionYSpeed";
            this.lblMoveToInspectionYSpeed.Size = new System.Drawing.Size(213, 15);
            this.lblMoveToInspectionYSpeed.TabIndex = 56;
            this.lblMoveToInspectionYSpeed.Text = "Move To Inspection Y Speed(mm/sec)";
            // 
            // txtMoveToInspectionYSpeed
            // 
            this.txtMoveToInspectionYSpeed.Enabled = false;
            this.txtMoveToInspectionYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToInspectionYSpeed.Location = new System.Drawing.Point(357, 103);
            this.txtMoveToInspectionYSpeed.Name = "txtMoveToInspectionYSpeed";
            this.txtMoveToInspectionYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToInspectionYSpeed.TabIndex = 57;
            this.txtMoveToInspectionYSpeed.Tag = "ProcessParameter:MoveToInspectionYSpeed";
            this.txtMoveToInspectionYSpeed.Text = "0";
            // 
            // lblMoveToInspectionYAcc
            // 
            this.lblMoveToInspectionYAcc.AutoSize = true;
            this.lblMoveToInspectionYAcc.Enabled = false;
            this.lblMoveToInspectionYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblMoveToInspectionYAcc.Location = new System.Drawing.Point(-3, 127);
            this.lblMoveToInspectionYAcc.Name = "lblMoveToInspectionYAcc";
            this.lblMoveToInspectionYAcc.Size = new System.Drawing.Size(228, 15);
            this.lblMoveToInspectionYAcc.TabIndex = 58;
            this.lblMoveToInspectionYAcc.Text = "Move To Inspection Y Accelation(mm/s²)";
            // 
            // txtMoveToInspectionYAcc
            // 
            this.txtMoveToInspectionYAcc.Enabled = false;
            this.txtMoveToInspectionYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtMoveToInspectionYAcc.Location = new System.Drawing.Point(357, 125);
            this.txtMoveToInspectionYAcc.Name = "txtMoveToInspectionYAcc";
            this.txtMoveToInspectionYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtMoveToInspectionYAcc.TabIndex = 59;
            this.txtMoveToInspectionYAcc.Tag = "ProcessParameter:MoveToInspectionYAcc";
            this.txtMoveToInspectionYAcc.Text = "0";
            // 
            // lblInspectionXPos
            // 
            this.lblInspectionXPos.AutoSize = true;
            this.lblInspectionXPos.Enabled = false;
            this.lblInspectionXPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionXPos.Location = new System.Drawing.Point(-3, 149);
            this.lblInspectionXPos.Name = "lblInspectionXPos";
            this.lblInspectionXPos.Size = new System.Drawing.Size(150, 15);
            this.lblInspectionXPos.TabIndex = 60;
            this.lblInspectionXPos.Text = "Inspection X Position(mm)";
            // 
            // txtInspectionXPos
            // 
            this.txtInspectionXPos.Enabled = false;
            this.txtInspectionXPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionXPos.Location = new System.Drawing.Point(357, 147);
            this.txtInspectionXPos.Name = "txtInspectionXPos";
            this.txtInspectionXPos.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionXPos.TabIndex = 61;
            this.txtInspectionXPos.Tag = "ProcessParameter:InspectionXPos";
            this.txtInspectionXPos.Text = "0";
            // 
            // lblInspectionXSpeed
            // 
            this.lblInspectionXSpeed.AutoSize = true;
            this.lblInspectionXSpeed.Enabled = false;
            this.lblInspectionXSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionXSpeed.Location = new System.Drawing.Point(-3, 171);
            this.lblInspectionXSpeed.Name = "lblInspectionXSpeed";
            this.lblInspectionXSpeed.Size = new System.Drawing.Size(162, 15);
            this.lblInspectionXSpeed.TabIndex = 62;
            this.lblInspectionXSpeed.Text = "Inspection X Speed(mm/sec)";
            // 
            // txtInspectionXSpeed
            // 
            this.txtInspectionXSpeed.Enabled = false;
            this.txtInspectionXSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionXSpeed.Location = new System.Drawing.Point(357, 169);
            this.txtInspectionXSpeed.Name = "txtInspectionXSpeed";
            this.txtInspectionXSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionXSpeed.TabIndex = 63;
            this.txtInspectionXSpeed.Tag = "ProcessParameter:InspectionXSpeed";
            this.txtInspectionXSpeed.Text = "0";
            // 
            // lblInspectionXAcc
            // 
            this.lblInspectionXAcc.AutoSize = true;
            this.lblInspectionXAcc.Enabled = false;
            this.lblInspectionXAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionXAcc.Location = new System.Drawing.Point(-3, 193);
            this.lblInspectionXAcc.Name = "lblInspectionXAcc";
            this.lblInspectionXAcc.Size = new System.Drawing.Size(177, 15);
            this.lblInspectionXAcc.TabIndex = 64;
            this.lblInspectionXAcc.Text = "Inspection X Accelation(mm/s²)";
            // 
            // txtInspectionXAcc
            // 
            this.txtInspectionXAcc.Enabled = false;
            this.txtInspectionXAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionXAcc.Location = new System.Drawing.Point(357, 191);
            this.txtInspectionXAcc.Name = "txtInspectionXAcc";
            this.txtInspectionXAcc.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionXAcc.TabIndex = 65;
            this.txtInspectionXAcc.Tag = "ProcessParameter:InspectionXAcc";
            this.txtInspectionXAcc.Text = "0";
            // 
            // lblInspectionYPos
            // 
            this.lblInspectionYPos.AutoSize = true;
            this.lblInspectionYPos.Enabled = false;
            this.lblInspectionYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionYPos.Location = new System.Drawing.Point(-3, 215);
            this.lblInspectionYPos.Name = "lblInspectionYPos";
            this.lblInspectionYPos.Size = new System.Drawing.Size(150, 15);
            this.lblInspectionYPos.TabIndex = 66;
            this.lblInspectionYPos.Text = "Inspection Y Position(mm)";
            // 
            // txtInspectionYPos
            // 
            this.txtInspectionYPos.Enabled = false;
            this.txtInspectionYPos.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionYPos.Location = new System.Drawing.Point(357, 213);
            this.txtInspectionYPos.Name = "txtInspectionYPos";
            this.txtInspectionYPos.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionYPos.TabIndex = 67;
            this.txtInspectionYPos.Tag = "ProcessParameter:InspectionYPos";
            this.txtInspectionYPos.Text = "0";
            // 
            // lblInspectionYSpeed
            // 
            this.lblInspectionYSpeed.AutoSize = true;
            this.lblInspectionYSpeed.Enabled = false;
            this.lblInspectionYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionYSpeed.Location = new System.Drawing.Point(-3, 237);
            this.lblInspectionYSpeed.Name = "lblInspectionYSpeed";
            this.lblInspectionYSpeed.Size = new System.Drawing.Size(162, 15);
            this.lblInspectionYSpeed.TabIndex = 68;
            this.lblInspectionYSpeed.Text = "Inspection Y Speed(mm/sec)";
            // 
            // txtInspectionYSpeed
            // 
            this.txtInspectionYSpeed.Enabled = false;
            this.txtInspectionYSpeed.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionYSpeed.Location = new System.Drawing.Point(357, 235);
            this.txtInspectionYSpeed.Name = "txtInspectionYSpeed";
            this.txtInspectionYSpeed.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionYSpeed.TabIndex = 69;
            this.txtInspectionYSpeed.Tag = "ProcessParameter:InspectionYSpeed";
            this.txtInspectionYSpeed.Text = "0";
            // 
            // lblInspectionYAcc
            // 
            this.lblInspectionYAcc.AutoSize = true;
            this.lblInspectionYAcc.Enabled = false;
            this.lblInspectionYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblInspectionYAcc.Location = new System.Drawing.Point(-3, 259);
            this.lblInspectionYAcc.Name = "lblInspectionYAcc";
            this.lblInspectionYAcc.Size = new System.Drawing.Size(177, 15);
            this.lblInspectionYAcc.TabIndex = 70;
            this.lblInspectionYAcc.Text = "Inspection Y Accelation(mm/s²)";
            // 
            // txtInspectionYAcc
            // 
            this.txtInspectionYAcc.Enabled = false;
            this.txtInspectionYAcc.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtInspectionYAcc.Location = new System.Drawing.Point(357, 257);
            this.txtInspectionYAcc.Name = "txtInspectionYAcc";
            this.txtInspectionYAcc.Size = new System.Drawing.Size(60, 23);
            this.txtInspectionYAcc.TabIndex = 71;
            this.txtInspectionYAcc.Tag = "ProcessParameter:InspectionYAcc";
            this.txtInspectionYAcc.Text = "0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblMoveToProcessEndYSpeed);
            this.groupBox1.Controls.Add(this.txtInspectionYAcc);
            this.groupBox1.Controls.Add(this.lblInspectionYAcc);
            this.groupBox1.Controls.Add(this.txtInspectionYSpeed);
            this.groupBox1.Controls.Add(this.lblInspectionYSpeed);
            this.groupBox1.Controls.Add(this.txtInspectionYPos);
            this.groupBox1.Controls.Add(this.lblInspectionYPos);
            this.groupBox1.Controls.Add(this.txtInspectionXAcc);
            this.groupBox1.Controls.Add(this.lblInspectionXAcc);
            this.groupBox1.Controls.Add(this.txtInspectionXSpeed);
            this.groupBox1.Controls.Add(this.lblInspectionXSpeed);
            this.groupBox1.Controls.Add(this.txtInspectionXPos);
            this.groupBox1.Controls.Add(this.lblInspectionXPos);
            this.groupBox1.Controls.Add(this.txtMoveToInspectionYAcc);
            this.groupBox1.Controls.Add(this.lblMoveToInspectionYAcc);
            this.groupBox1.Controls.Add(this.txtMoveToInspectionYSpeed);
            this.groupBox1.Controls.Add(this.lblMoveToInspectionYSpeed);
            this.groupBox1.Controls.Add(this.txtMoveToInspectionYPos);
            this.groupBox1.Controls.Add(this.lblMoveToInspectionYPos);
            this.groupBox1.Controls.Add(this.txtMoveToProcessEndYAcc);
            this.groupBox1.Controls.Add(this.lblMoveToProcessEndYAcc);
            this.groupBox1.Controls.Add(this.txtMoveToProcessEndYSpeed);
            this.groupBox1.Controls.Add(this.txtMoveToProcessEndYPos);
            this.groupBox1.Controls.Add(this.lblMoveToProcessEndYPos);
            this.groupBox1.Controls.Add(this.lblProcessYPos);
            this.groupBox1.Controls.Add(this.txtProcessYPos);
            this.groupBox1.Location = new System.Drawing.Point(516, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(42, 34);
            this.groupBox1.TabIndex = 2001;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Visible = false;
            // 
            // ParameterSettingForm
            // 
            this.ClientSize = new System.Drawing.Size(1120, 691);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabMain);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParameterSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parameter Setting";
            this.tabMain.ResumeLayout(false);
            this.tabProcessParams.ResumeLayout(false);
            this.tabProcessParams.PerformLayout();
            this.grpProcessOption.ResumeLayout(false);
            this.grpProcessOption.PerformLayout();
            this.grpSettlingTime.ResumeLayout(false);
            this.grpSettlingTime.PerformLayout();
            this.grpInspectionOption.ResumeLayout(false);
            this.grpInspectionOption.PerformLayout();
            this.grpPowerMeterRecipe.ResumeLayout(false);
            this.grpPowerMeterRecipe.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private Label lblMoveToLoadYPos;
        private TextBox txtMoveToLoadYPos;
        private Label lblMoveToLoadYSpeed;
        private TextBox txtMoveToLoadYSpeed;
        private Label lblMoveToLoadYAcc;
        private TextBox txtMoveToLoadYAcc;
        private Label lblMoveToLoadXPos;
        private TextBox txtMoveToLoadXPos;
        private Label lblMoveToLoadXPosSpeed;
        private TextBox txtMoveToLoadXSpeed;
        private Label lblMoveToLoadXAcc;
        private TextBox txtMoveToLoadXAcc;
        private GroupBox grpPowerMeterRecipe;
        private ListBox lstPmRecipeList;
        private Button btnPmRefresh;
        private Label lblPmSelectedDisplay;
        private TextBox txtSelectedPmRecipeName;
        private GroupBox groupBox1;
        private GroupBox grpInspectionOption;
        private RadioButton rbtnAllInspection;
        private RadioButton rbtn1LineInspection;
        private GroupBox grpSettlingTime;
        private Label lalAfterSeqMs;
        private Label lalAfterMoveMs;
        private TextBox txtAutoDelayAfterMoveMs;
        private TextBox txtAutoDelayAfterSeqMs;
        private GroupBox grpProcessOption;
        private RadioButton rbtnAllStepProcess;
        private RadioButton rbtn1StepProcess;
        private RadioButton rbtnStepbyStepAllInspection;
    }
}
