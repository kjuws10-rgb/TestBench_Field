using System;
using System.Windows.Forms;

namespace StageWin.UI
{
    partial class AutoProcessForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMode;
        private TabPage tabAuto;
        private TabPage tabSemiAuto;

        // Auto 탭 컨트롤들
        private CheckBox chkDryRun;
        private Button btnAutoStart;
        private Button btnAutoStop;
        private CheckBox chkSeqLoad;
        private CheckBox chkSeqMoveToAlign;
        private CheckBox chkSeqAlign;
        private CheckBox chkSeqProcessReady;
        private CheckBox chkSeqProcess;
        private CheckBox chkSeqMoveToAlignCheck;
        private CheckBox chkSeqAlignCheck;
        private CheckBox chkSeqMoveToUnload;
        private CheckBox chkSeqUnload;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMode = new System.Windows.Forms.TabControl();
            this.tabAuto = new System.Windows.Forms.TabPage();
            this.grpAuto = new System.Windows.Forms.GroupBox();
            this.tlpAutoCheckStep = new System.Windows.Forms.TableLayoutPanel();
            this.lalSeqPowerMeterSts = new System.Windows.Forms.Label();
            this.chkSeqPwmCheck = new System.Windows.Forms.CheckBox();
            this.lalSeqMoveToUnloadSts = new System.Windows.Forms.Label();
            this.lalSeqAlignCheckSts = new System.Windows.Forms.Label();
            this.lalSeqMoveToAlignCheckSts = new System.Windows.Forms.Label();
            this.lalSeqInspectionSts = new System.Windows.Forms.Label();
            this.lalSeqProcessSts = new System.Windows.Forms.Label();
            this.lalSeqProcessReadySts = new System.Windows.Forms.Label();
            this.lalSeqAlignSts = new System.Windows.Forms.Label();
            this.lalSeqMoveToAlignSts = new System.Windows.Forms.Label();
            this.chkSeqLoad = new System.Windows.Forms.CheckBox();
            this.chkSeqMoveToAlign = new System.Windows.Forms.CheckBox();
            this.chkSeqAlign = new System.Windows.Forms.CheckBox();
            this.lalSeqLoadSts = new System.Windows.Forms.Label();
            this.chkSeqMoveToUnload = new System.Windows.Forms.CheckBox();
            this.chkSeqAlignCheck = new System.Windows.Forms.CheckBox();
            this.chkSeqMoveToAlignCheck = new System.Windows.Forms.CheckBox();
            this.chkSeqProcess = new System.Windows.Forms.CheckBox();
            this.chkSeqProcessReady = new System.Windows.Forms.CheckBox();
            this.lalSeqUnloadSts = new System.Windows.Forms.Label();
            this.chkSeqUnload = new System.Windows.Forms.CheckBox();
            this.btnAutoStop = new System.Windows.Forms.Button();
            this.chkDryRun = new System.Windows.Forms.CheckBox();
            this.btnAutoStart = new System.Windows.Forms.Button();
            this.tabSemiAuto = new System.Windows.Forms.TabPage();
            this.grpSemiAuto = new System.Windows.Forms.GroupBox();
            this.btnSemiPowerMeter = new System.Windows.Forms.Button();
            this.btnSemiStop = new System.Windows.Forms.Button();
            this.btnSemiLoad = new System.Windows.Forms.Button();
            this.btnSemiProcessReady = new System.Windows.Forms.Button();
            this.btnSemiUnload = new System.Windows.Forms.Button();
            this.btnSemiMoveToAlignCheck = new System.Windows.Forms.Button();
            this.btnSemiInspection = new System.Windows.Forms.Button();
            this.btnSemiAlign = new System.Windows.Forms.Button();
            this.btnSemiAlignCheck = new System.Windows.Forms.Button();
            this.btnSemiMoveToUnload = new System.Windows.Forms.Button();
            this.btnSemiMoveToAlign = new System.Windows.Forms.Button();
            this.btnSemiProcess = new System.Windows.Forms.Button();
            this.chkSeqInspection = new System.Windows.Forms.CheckBox();
            this.tabMode.SuspendLayout();
            this.tabAuto.SuspendLayout();
            this.grpAuto.SuspendLayout();
            this.tlpAutoCheckStep.SuspendLayout();
            this.tabSemiAuto.SuspendLayout();
            this.grpSemiAuto.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMode
            // 
            this.tabMode.Controls.Add(this.tabAuto);
            this.tabMode.Controls.Add(this.tabSemiAuto);
            this.tabMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMode.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.tabMode.Location = new System.Drawing.Point(0, 0);
            this.tabMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabMode.Name = "tabMode";
            this.tabMode.SelectedIndex = 0;
            this.tabMode.Size = new System.Drawing.Size(268, 789);
            this.tabMode.TabIndex = 0;
            // 
            // tabAuto
            // 
            this.tabAuto.Controls.Add(this.grpAuto);
            this.tabAuto.Location = new System.Drawing.Point(4, 24);
            this.tabAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabAuto.Name = "tabAuto";
            this.tabAuto.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabAuto.Size = new System.Drawing.Size(260, 761);
            this.tabAuto.TabIndex = 0;
            this.tabAuto.Text = "AUTO";
            this.tabAuto.UseVisualStyleBackColor = true;
            // 
            // grpAuto
            // 
            this.grpAuto.Controls.Add(this.tlpAutoCheckStep);
            this.grpAuto.Controls.Add(this.btnAutoStop);
            this.grpAuto.Controls.Add(this.chkDryRun);
            this.grpAuto.Controls.Add(this.btnAutoStart);
            this.grpAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAuto.Location = new System.Drawing.Point(3, 4);
            this.grpAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpAuto.Name = "grpAuto";
            this.grpAuto.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpAuto.Size = new System.Drawing.Size(254, 753);
            this.grpAuto.TabIndex = 13;
            this.grpAuto.TabStop = false;
            this.grpAuto.Text = "AUTO SEQ";
            // 
            // tlpAutoCheckStep
            // 
            this.tlpAutoCheckStep.ColumnCount = 2;
            this.tlpAutoCheckStep.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tlpAutoCheckStep.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqPowerMeterSts, 1, 3);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqPwmCheck, 0, 3);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqMoveToUnloadSts, 1, 9);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqAlignCheckSts, 1, 8);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqMoveToAlignCheckSts, 1, 7);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqInspectionSts, 1, 6);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqProcessSts, 1, 5);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqProcessReadySts, 1, 4);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqAlignSts, 1, 2);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqMoveToAlignSts, 1, 1);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqLoad, 0, 0);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqMoveToAlign, 0, 1);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqAlign, 0, 2);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqLoadSts, 1, 0);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqMoveToUnload, 0, 9);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqAlignCheck, 0, 8);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqMoveToAlignCheck, 0, 7);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqInspection, 0, 6);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqProcess, 0, 5);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqProcessReady, 0, 4);
            this.tlpAutoCheckStep.Controls.Add(this.lalSeqUnloadSts, 1, 10);
            this.tlpAutoCheckStep.Controls.Add(this.chkSeqUnload, 0, 10);
            this.tlpAutoCheckStep.Location = new System.Drawing.Point(17, 68);
            this.tlpAutoCheckStep.Name = "tlpAutoCheckStep";
            this.tlpAutoCheckStep.RowCount = 11;
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tlpAutoCheckStep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpAutoCheckStep.Size = new System.Drawing.Size(222, 414);
            this.tlpAutoCheckStep.TabIndex = 13;
            // 
            // lalSeqPowerMeterSts
            // 
            this.lalSeqPowerMeterSts.AutoSize = true;
            this.lalSeqPowerMeterSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqPowerMeterSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqPowerMeterSts.Location = new System.Drawing.Point(169, 111);
            this.lalSeqPowerMeterSts.Name = "lalSeqPowerMeterSts";
            this.lalSeqPowerMeterSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqPowerMeterSts.TabIndex = 23;
            this.lalSeqPowerMeterSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSeqPwmCheck
            // 
            this.chkSeqPwmCheck.AutoSize = true;
            this.chkSeqPwmCheck.Location = new System.Drawing.Point(3, 115);
            this.chkSeqPwmCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqPwmCheck.Name = "chkSeqPwmCheck";
            this.chkSeqPwmCheck.Size = new System.Drawing.Size(105, 19);
            this.chkSeqPwmCheck.TabIndex = 22;
            this.chkSeqPwmCheck.Text = "4) PowerMeter";
            this.chkSeqPwmCheck.UseVisualStyleBackColor = true;
            // 
            // lalSeqMoveToUnloadSts
            // 
            this.lalSeqMoveToUnloadSts.AutoSize = true;
            this.lalSeqMoveToUnloadSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqMoveToUnloadSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqMoveToUnloadSts.Location = new System.Drawing.Point(169, 333);
            this.lalSeqMoveToUnloadSts.Name = "lalSeqMoveToUnloadSts";
            this.lalSeqMoveToUnloadSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqMoveToUnloadSts.TabIndex = 19;
            this.lalSeqMoveToUnloadSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqAlignCheckSts
            // 
            this.lalSeqAlignCheckSts.AutoSize = true;
            this.lalSeqAlignCheckSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqAlignCheckSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqAlignCheckSts.Location = new System.Drawing.Point(169, 296);
            this.lalSeqAlignCheckSts.Name = "lalSeqAlignCheckSts";
            this.lalSeqAlignCheckSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqAlignCheckSts.TabIndex = 18;
            this.lalSeqAlignCheckSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqMoveToAlignCheckSts
            // 
            this.lalSeqMoveToAlignCheckSts.AutoSize = true;
            this.lalSeqMoveToAlignCheckSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqMoveToAlignCheckSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqMoveToAlignCheckSts.Location = new System.Drawing.Point(169, 259);
            this.lalSeqMoveToAlignCheckSts.Name = "lalSeqMoveToAlignCheckSts";
            this.lalSeqMoveToAlignCheckSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqMoveToAlignCheckSts.TabIndex = 17;
            this.lalSeqMoveToAlignCheckSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqInspectionSts
            // 
            this.lalSeqInspectionSts.AutoSize = true;
            this.lalSeqInspectionSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqInspectionSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqInspectionSts.Location = new System.Drawing.Point(169, 222);
            this.lalSeqInspectionSts.Name = "lalSeqInspectionSts";
            this.lalSeqInspectionSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqInspectionSts.TabIndex = 16;
            this.lalSeqInspectionSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqProcessSts
            // 
            this.lalSeqProcessSts.AutoSize = true;
            this.lalSeqProcessSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqProcessSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqProcessSts.Location = new System.Drawing.Point(169, 185);
            this.lalSeqProcessSts.Name = "lalSeqProcessSts";
            this.lalSeqProcessSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqProcessSts.TabIndex = 15;
            this.lalSeqProcessSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqProcessReadySts
            // 
            this.lalSeqProcessReadySts.AutoSize = true;
            this.lalSeqProcessReadySts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqProcessReadySts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqProcessReadySts.Location = new System.Drawing.Point(169, 148);
            this.lalSeqProcessReadySts.Name = "lalSeqProcessReadySts";
            this.lalSeqProcessReadySts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqProcessReadySts.TabIndex = 14;
            this.lalSeqProcessReadySts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqAlignSts
            // 
            this.lalSeqAlignSts.AutoSize = true;
            this.lalSeqAlignSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqAlignSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqAlignSts.Location = new System.Drawing.Point(169, 74);
            this.lalSeqAlignSts.Name = "lalSeqAlignSts";
            this.lalSeqAlignSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqAlignSts.TabIndex = 13;
            this.lalSeqAlignSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lalSeqMoveToAlignSts
            // 
            this.lalSeqMoveToAlignSts.AutoSize = true;
            this.lalSeqMoveToAlignSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqMoveToAlignSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqMoveToAlignSts.Location = new System.Drawing.Point(169, 37);
            this.lalSeqMoveToAlignSts.Name = "lalSeqMoveToAlignSts";
            this.lalSeqMoveToAlignSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqMoveToAlignSts.TabIndex = 12;
            this.lalSeqMoveToAlignSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSeqLoad
            // 
            this.chkSeqLoad.AutoSize = true;
            this.chkSeqLoad.Location = new System.Drawing.Point(3, 4);
            this.chkSeqLoad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqLoad.Name = "chkSeqLoad";
            this.chkSeqLoad.Size = new System.Drawing.Size(67, 19);
            this.chkSeqLoad.TabIndex = 1;
            this.chkSeqLoad.Text = "1) Load";
            this.chkSeqLoad.UseVisualStyleBackColor = true;
            // 
            // chkSeqMoveToAlign
            // 
            this.chkSeqMoveToAlign.AutoSize = true;
            this.chkSeqMoveToAlign.Location = new System.Drawing.Point(3, 41);
            this.chkSeqMoveToAlign.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqMoveToAlign.Name = "chkSeqMoveToAlign";
            this.chkSeqMoveToAlign.Size = new System.Drawing.Size(120, 19);
            this.chkSeqMoveToAlign.TabIndex = 2;
            this.chkSeqMoveToAlign.Text = "2) Move To Align";
            this.chkSeqMoveToAlign.UseVisualStyleBackColor = true;
            // 
            // chkSeqAlign
            // 
            this.chkSeqAlign.AutoSize = true;
            this.chkSeqAlign.Location = new System.Drawing.Point(3, 78);
            this.chkSeqAlign.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqAlign.Name = "chkSeqAlign";
            this.chkSeqAlign.Size = new System.Drawing.Size(69, 19);
            this.chkSeqAlign.TabIndex = 3;
            this.chkSeqAlign.Text = "3) Align";
            this.chkSeqAlign.UseVisualStyleBackColor = true;
            // 
            // lalSeqLoadSts
            // 
            this.lalSeqLoadSts.AutoSize = true;
            this.lalSeqLoadSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqLoadSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqLoadSts.Location = new System.Drawing.Point(169, 0);
            this.lalSeqLoadSts.Name = "lalSeqLoadSts";
            this.lalSeqLoadSts.Size = new System.Drawing.Size(50, 37);
            this.lalSeqLoadSts.TabIndex = 11;
            this.lalSeqLoadSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSeqMoveToUnload
            // 
            this.chkSeqMoveToUnload.AutoSize = true;
            this.chkSeqMoveToUnload.Location = new System.Drawing.Point(3, 337);
            this.chkSeqMoveToUnload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqMoveToUnload.Name = "chkSeqMoveToUnload";
            this.chkSeqMoveToUnload.Size = new System.Drawing.Size(137, 19);
            this.chkSeqMoveToUnload.TabIndex = 9;
            this.chkSeqMoveToUnload.Text = "10) Move To Unload";
            this.chkSeqMoveToUnload.UseVisualStyleBackColor = true;
            // 
            // chkSeqAlignCheck
            // 
            this.chkSeqAlignCheck.AutoSize = true;
            this.chkSeqAlignCheck.Location = new System.Drawing.Point(3, 300);
            this.chkSeqAlignCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqAlignCheck.Name = "chkSeqAlignCheck";
            this.chkSeqAlignCheck.Size = new System.Drawing.Size(131, 19);
            this.chkSeqAlignCheck.TabIndex = 8;
            this.chkSeqAlignCheck.Text = "9) 2nd Align Check";
            this.chkSeqAlignCheck.UseVisualStyleBackColor = true;
            // 
            // chkSeqMoveToAlignCheck
            // 
            this.chkSeqMoveToAlignCheck.AutoSize = true;
            this.chkSeqMoveToAlignCheck.Location = new System.Drawing.Point(3, 263);
            this.chkSeqMoveToAlignCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqMoveToAlignCheck.Name = "chkSeqMoveToAlignCheck";
            this.chkSeqMoveToAlignCheck.Size = new System.Drawing.Size(145, 19);
            this.chkSeqMoveToAlignCheck.TabIndex = 7;
            this.chkSeqMoveToAlignCheck.Text = "8) Move To 2nd Align";
            this.chkSeqMoveToAlignCheck.UseVisualStyleBackColor = true;
            // 
            // chkSeqProcess
            // 
            this.chkSeqProcess.AutoSize = true;
            this.chkSeqProcess.Location = new System.Drawing.Point(3, 189);
            this.chkSeqProcess.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqProcess.Name = "chkSeqProcess";
            this.chkSeqProcess.Size = new System.Drawing.Size(81, 19);
            this.chkSeqProcess.TabIndex = 5;
            this.chkSeqProcess.Text = "6) Process";
            this.chkSeqProcess.UseVisualStyleBackColor = true;
            // 
            // chkSeqProcessReady
            // 
            this.chkSeqProcessReady.AutoSize = true;
            this.chkSeqProcessReady.Location = new System.Drawing.Point(3, 152);
            this.chkSeqProcessReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqProcessReady.Name = "chkSeqProcessReady";
            this.chkSeqProcessReady.Size = new System.Drawing.Size(117, 19);
            this.chkSeqProcessReady.TabIndex = 4;
            this.chkSeqProcessReady.Text = "5) Process Ready";
            this.chkSeqProcessReady.UseVisualStyleBackColor = true;
            // 
            // lalSeqUnloadSts
            // 
            this.lalSeqUnloadSts.AutoSize = true;
            this.lalSeqUnloadSts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lalSeqUnloadSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lalSeqUnloadSts.Location = new System.Drawing.Point(169, 370);
            this.lalSeqUnloadSts.Name = "lalSeqUnloadSts";
            this.lalSeqUnloadSts.Size = new System.Drawing.Size(50, 44);
            this.lalSeqUnloadSts.TabIndex = 21;
            this.lalSeqUnloadSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSeqUnload
            // 
            this.chkSeqUnload.AutoSize = true;
            this.chkSeqUnload.Location = new System.Drawing.Point(3, 374);
            this.chkSeqUnload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqUnload.Name = "chkSeqUnload";
            this.chkSeqUnload.Size = new System.Drawing.Size(86, 19);
            this.chkSeqUnload.TabIndex = 10;
            this.chkSeqUnload.Text = "11) Unload";
            this.chkSeqUnload.UseVisualStyleBackColor = true;
            // 
            // btnAutoStop
            // 
            this.btnAutoStop.BackColor = System.Drawing.Color.LightCoral;
            this.btnAutoStop.ForeColor = System.Drawing.Color.MistyRose;
            this.btnAutoStop.Location = new System.Drawing.Point(26, 616);
            this.btnAutoStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoStop.Name = "btnAutoStop";
            this.btnAutoStop.Size = new System.Drawing.Size(200, 107);
            this.btnAutoStop.TabIndex = 12;
            this.btnAutoStop.Text = "AUTO STOP";
            this.btnAutoStop.UseVisualStyleBackColor = false;
            // 
            // chkDryRun
            // 
            this.chkDryRun.AutoSize = true;
            this.chkDryRun.Location = new System.Drawing.Point(137, 31);
            this.chkDryRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkDryRun.Name = "chkDryRun";
            this.chkDryRun.Size = new System.Drawing.Size(105, 19);
            this.chkDryRun.TabIndex = 0;
            this.chkDryRun.Text = "Dry Run Mode";
            this.chkDryRun.UseVisualStyleBackColor = true;
            // 
            // btnAutoStart
            // 
            this.btnAutoStart.Location = new System.Drawing.Point(26, 516);
            this.btnAutoStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoStart.Name = "btnAutoStart";
            this.btnAutoStart.Size = new System.Drawing.Size(200, 80);
            this.btnAutoStart.TabIndex = 11;
            this.btnAutoStart.Text = "AUTO START";
            this.btnAutoStart.UseVisualStyleBackColor = true;
            // 
            // tabSemiAuto
            // 
            this.tabSemiAuto.Controls.Add(this.grpSemiAuto);
            this.tabSemiAuto.Location = new System.Drawing.Point(4, 24);
            this.tabSemiAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSemiAuto.Name = "tabSemiAuto";
            this.tabSemiAuto.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSemiAuto.Size = new System.Drawing.Size(260, 761);
            this.tabSemiAuto.TabIndex = 1;
            this.tabSemiAuto.Text = "SEMI AUTO";
            this.tabSemiAuto.UseVisualStyleBackColor = true;
            // 
            // grpSemiAuto
            // 
            this.grpSemiAuto.Controls.Add(this.btnSemiPowerMeter);
            this.grpSemiAuto.Controls.Add(this.btnSemiStop);
            this.grpSemiAuto.Controls.Add(this.btnSemiLoad);
            this.grpSemiAuto.Controls.Add(this.btnSemiProcessReady);
            this.grpSemiAuto.Controls.Add(this.btnSemiUnload);
            this.grpSemiAuto.Controls.Add(this.btnSemiMoveToAlignCheck);
            this.grpSemiAuto.Controls.Add(this.btnSemiInspection);
            this.grpSemiAuto.Controls.Add(this.btnSemiAlign);
            this.grpSemiAuto.Controls.Add(this.btnSemiAlignCheck);
            this.grpSemiAuto.Controls.Add(this.btnSemiMoveToUnload);
            this.grpSemiAuto.Controls.Add(this.btnSemiMoveToAlign);
            this.grpSemiAuto.Controls.Add(this.btnSemiProcess);
            this.grpSemiAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSemiAuto.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.grpSemiAuto.Location = new System.Drawing.Point(3, 4);
            this.grpSemiAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpSemiAuto.Name = "grpSemiAuto";
            this.grpSemiAuto.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpSemiAuto.Size = new System.Drawing.Size(254, 753);
            this.grpSemiAuto.TabIndex = 11;
            this.grpSemiAuto.TabStop = false;
            this.grpSemiAuto.Text = "SEMI AUTO FUNCTIONS";
            // 
            // btnSemiPowerMeter
            // 
            this.btnSemiPowerMeter.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiPowerMeter.Location = new System.Drawing.Point(29, 198);
            this.btnSemiPowerMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiPowerMeter.Name = "btnSemiPowerMeter";
            this.btnSemiPowerMeter.Size = new System.Drawing.Size(200, 50);
            this.btnSemiPowerMeter.TabIndex = 22;
            this.btnSemiPowerMeter.Text = "Power Meter";
            this.btnSemiPowerMeter.UseVisualStyleBackColor = true;
            // 
            // btnSemiStop
            // 
            this.btnSemiStop.BackColor = System.Drawing.Color.LightCoral;
            this.btnSemiStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSemiStop.ForeColor = System.Drawing.Color.MistyRose;
            this.btnSemiStop.Location = new System.Drawing.Point(29, 653);
            this.btnSemiStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiStop.Name = "btnSemiStop";
            this.btnSemiStop.Size = new System.Drawing.Size(200, 87);
            this.btnSemiStop.TabIndex = 21;
            this.btnSemiStop.Text = "STOP";
            this.btnSemiStop.UseVisualStyleBackColor = false;
            // 
            // btnSemiLoad
            // 
            this.btnSemiLoad.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSemiLoad.Location = new System.Drawing.Point(29, 33);
            this.btnSemiLoad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiLoad.Name = "btnSemiLoad";
            this.btnSemiLoad.Size = new System.Drawing.Size(200, 50);
            this.btnSemiLoad.TabIndex = 11;
            this.btnSemiLoad.Text = "Load";
            this.btnSemiLoad.UseVisualStyleBackColor = true;
            // 
            // btnSemiProcessReady
            // 
            this.btnSemiProcessReady.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiProcessReady.Location = new System.Drawing.Point(29, 253);
            this.btnSemiProcessReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiProcessReady.Name = "btnSemiProcessReady";
            this.btnSemiProcessReady.Size = new System.Drawing.Size(200, 50);
            this.btnSemiProcessReady.TabIndex = 14;
            this.btnSemiProcessReady.Text = "Process Ready";
            this.btnSemiProcessReady.UseVisualStyleBackColor = true;
            // 
            // btnSemiUnload
            // 
            this.btnSemiUnload.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiUnload.Location = new System.Drawing.Point(29, 583);
            this.btnSemiUnload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiUnload.Name = "btnSemiUnload";
            this.btnSemiUnload.Size = new System.Drawing.Size(200, 50);
            this.btnSemiUnload.TabIndex = 20;
            this.btnSemiUnload.Text = "Unload";
            this.btnSemiUnload.UseVisualStyleBackColor = true;
            // 
            // btnSemiMoveToAlignCheck
            // 
            this.btnSemiMoveToAlignCheck.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiMoveToAlignCheck.Location = new System.Drawing.Point(29, 418);
            this.btnSemiMoveToAlignCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiMoveToAlignCheck.Name = "btnSemiMoveToAlignCheck";
            this.btnSemiMoveToAlignCheck.Size = new System.Drawing.Size(200, 50);
            this.btnSemiMoveToAlignCheck.TabIndex = 17;
            this.btnSemiMoveToAlignCheck.Text = "Move To Align Check";
            this.btnSemiMoveToAlignCheck.UseVisualStyleBackColor = true;
            // 
            // btnSemiInspection
            // 
            this.btnSemiInspection.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiInspection.Location = new System.Drawing.Point(29, 363);
            this.btnSemiInspection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiInspection.Name = "btnSemiInspection";
            this.btnSemiInspection.Size = new System.Drawing.Size(200, 50);
            this.btnSemiInspection.TabIndex = 16;
            this.btnSemiInspection.Text = "Inspection";
            this.btnSemiInspection.UseVisualStyleBackColor = true;
            // 
            // btnSemiAlign
            // 
            this.btnSemiAlign.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiAlign.Location = new System.Drawing.Point(29, 143);
            this.btnSemiAlign.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiAlign.Name = "btnSemiAlign";
            this.btnSemiAlign.Size = new System.Drawing.Size(200, 50);
            this.btnSemiAlign.TabIndex = 13;
            this.btnSemiAlign.Text = "Align";
            this.btnSemiAlign.UseVisualStyleBackColor = true;
            // 
            // btnSemiAlignCheck
            // 
            this.btnSemiAlignCheck.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiAlignCheck.Location = new System.Drawing.Point(29, 473);
            this.btnSemiAlignCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiAlignCheck.Name = "btnSemiAlignCheck";
            this.btnSemiAlignCheck.Size = new System.Drawing.Size(200, 50);
            this.btnSemiAlignCheck.TabIndex = 18;
            this.btnSemiAlignCheck.Text = "Align Check";
            this.btnSemiAlignCheck.UseVisualStyleBackColor = true;
            // 
            // btnSemiMoveToUnload
            // 
            this.btnSemiMoveToUnload.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiMoveToUnload.Location = new System.Drawing.Point(29, 528);
            this.btnSemiMoveToUnload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiMoveToUnload.Name = "btnSemiMoveToUnload";
            this.btnSemiMoveToUnload.Size = new System.Drawing.Size(200, 50);
            this.btnSemiMoveToUnload.TabIndex = 19;
            this.btnSemiMoveToUnload.Text = "Move To Unload";
            this.btnSemiMoveToUnload.UseVisualStyleBackColor = true;
            // 
            // btnSemiMoveToAlign
            // 
            this.btnSemiMoveToAlign.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiMoveToAlign.Location = new System.Drawing.Point(29, 88);
            this.btnSemiMoveToAlign.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiMoveToAlign.Name = "btnSemiMoveToAlign";
            this.btnSemiMoveToAlign.Size = new System.Drawing.Size(200, 50);
            this.btnSemiMoveToAlign.TabIndex = 12;
            this.btnSemiMoveToAlign.Text = "Move To Align";
            this.btnSemiMoveToAlign.UseVisualStyleBackColor = true;
            // 
            // btnSemiProcess
            // 
            this.btnSemiProcess.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnSemiProcess.Location = new System.Drawing.Point(29, 308);
            this.btnSemiProcess.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSemiProcess.Name = "btnSemiProcess";
            this.btnSemiProcess.Size = new System.Drawing.Size(200, 50);
            this.btnSemiProcess.TabIndex = 15;
            this.btnSemiProcess.Text = "Process";
            this.btnSemiProcess.UseVisualStyleBackColor = true;
            // 
            // chkSeqInspection
            // 
            this.chkSeqInspection.AutoSize = true;
            this.chkSeqInspection.Location = new System.Drawing.Point(3, 226);
            this.chkSeqInspection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSeqInspection.Name = "chkSeqInspection";
            this.chkSeqInspection.Size = new System.Drawing.Size(96, 19);
            this.chkSeqInspection.TabIndex = 6;
            this.chkSeqInspection.Text = "7) Inspection";
            this.chkSeqInspection.UseVisualStyleBackColor = true;
            // 
            // AutoProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 789);
            this.Controls.Add(this.tabMode);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "AutoProcessForm";
            this.Text = "Auto Process";
            this.tabMode.ResumeLayout(false);
            this.tabAuto.ResumeLayout(false);
            this.grpAuto.ResumeLayout(false);
            this.grpAuto.PerformLayout();
            this.tlpAutoCheckStep.ResumeLayout(false);
            this.tlpAutoCheckStep.PerformLayout();
            this.tabSemiAuto.ResumeLayout(false);
            this.grpSemiAuto.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private GroupBox grpAuto;
        private GroupBox grpSemiAuto;
        private Button btnSemiStop;
        private Button btnSemiLoad;
        private Button btnSemiProcessReady;
        private Button btnSemiUnload;
        private Button btnSemiMoveToAlignCheck;
        private Button btnSemiInspection;
        private Button btnSemiAlign;
        private Button btnSemiAlignCheck;
        private Button btnSemiMoveToUnload;
        private Button btnSemiMoveToAlign;
        private Button btnSemiProcess;
        private TableLayoutPanel tlpAutoCheckStep;
        private Label lalSeqAlignCheckSts;
        private Label lalSeqMoveToAlignCheckSts;
        private Label lalSeqInspectionSts;
        private Label lalSeqProcessSts;
        private Label lalSeqProcessReadySts;
        private Label lalSeqAlignSts;
        private Label lalSeqMoveToAlignSts;
        private Label lalSeqLoadSts;
        private Label lalSeqPowerMeterSts;
        private CheckBox chkSeqPwmCheck;
        private Label lalSeqMoveToUnloadSts;
        private Label lalSeqUnloadSts;
        private Button btnSemiPowerMeter;
        private CheckBox chkSeqInspection;
    }
}
