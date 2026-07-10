using System.Windows.Forms;
using System.Drawing;

namespace StageWin.UI
{
    partial class RecipeForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblRecipe = new System.Windows.Forms.Label();
            this.txtRecipe = new System.Windows.Forms.TextBox();
            this.lblAlign = new System.Windows.Forms.Label();
            this.lblAX = new System.Windows.Forms.Label();
            this.numAlignX = new System.Windows.Forms.NumericUpDown();
            this.lblAY = new System.Windows.Forms.Label();
            this.numAlignY = new System.Windows.Forms.NumericUpDown();
            this.lblOffset = new System.Windows.Forms.Label();
            this.lblSOX = new System.Windows.Forms.Label();
            this.numScanToRevOfsX = new System.Windows.Forms.NumericUpDown();
            this.lblSOY = new System.Windows.Forms.Label();
            this.numScanToRevOfsY = new System.Windows.Forms.NumericUpDown();
            this.lblROX = new System.Windows.Forms.Label();
            this.numRevOfsX = new System.Windows.Forms.NumericUpDown();
            this.lblROY = new System.Windows.Forms.Label();
            this.numRevOfsY = new System.Windows.Forms.NumericUpDown();
            this.lblCrit = new System.Windows.Forms.Label();
            this.lblTolX = new System.Windows.Forms.Label();
            this.numTolX = new System.Windows.Forms.NumericUpDown();
            this.lblTolY = new System.Windows.Forms.Label();
            this.numTolY = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.grpParam = new System.Windows.Forms.GroupBox();
            this.lblVecRow = new System.Windows.Forms.Label();
            this._btnPowerMeterEditor = new System.Windows.Forms.Button();
            this._btnToolingEditor = new System.Windows.Forms.Button();
            this.lblLines = new System.Windows.Forms.Label();
            this.numVectorRow = new System.Windows.Forms.NumericUpDown();
            this.numLines = new System.Windows.Forms.NumericUpDown();
            this.lblVecCol = new System.Windows.Forms.Label();
            this.lblCols = new System.Windows.Forms.Label();
            this.numVectorCol = new System.Windows.Forms.NumericUpDown();
            this.numHoles = new System.Windows.Forms.NumericUpDown();
            this.lblPitchX = new System.Windows.Forms.Label();
            this.numPitchX = new System.Windows.Forms.NumericUpDown();
            this.lblPitchY = new System.Windows.Forms.Label();
            this.numPitchY = new System.Windows.Forms.NumericUpDown();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.lblFirstX = new System.Windows.Forms.Label();
            this.numFirstX = new System.Windows.Forms.NumericUpDown();
            this.lblFirstY = new System.Windows.Forms.Label();
            this.numFirstY = new System.Windows.Forms.NumericUpDown();
            this.tab = new System.Windows.Forms.TabControl();
            this.tabDrawScan = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDesign = new System.Windows.Forms.Label();
            this.gridScan = new System.Windows.Forms.DataGridView();
            this.btnMoveScan = new System.Windows.Forms.Button();
            this.lblScan = new System.Windows.Forms.Label();
            this.gridDesign = new System.Windows.Forms.DataGridView();
            this.tabReview = new System.Windows.Forms.TabPage();
            this.btnInitalReviewResults = new System.Windows.Forms.Button();
            this.btnMeasureLine = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtnXInspection = new System.Windows.Forms.RadioButton();
            this.rbtnYInspection = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnForwardInspection = new System.Windows.Forms.RadioButton();
            this.rbtnBackwardInspection = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbtnOneWayInspection = new System.Windows.Forms.RadioButton();
            this.rbtnSnakeInspection = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.numFlyVisionLines = new System.Windows.Forms.NumericUpDown();
            this.lblReview = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnVisionStepHoles = new System.Windows.Forms.RadioButton();
            this.rbtnVisionStepAllWholeLine = new System.Windows.Forms.RadioButton();
            this.rbtnVisionStepAllHoles = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.numVisionStepCount = new System.Windows.Forms.NumericUpDown();
            this.btnMoveReview = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnVisionStepbyStep = new System.Windows.Forms.RadioButton();
            this.rbtnVisionOntheFly = new System.Windows.Forms.RadioButton();
            this.btnApplyReviewResult = new System.Windows.Forms.Button();
            this.btnMeasureSingle = new System.Windows.Forms.Button();
            this.gridReviewDetail = new System.Windows.Forms.DataGridView();
            this.gridReview = new System.Windows.Forms.DataGridView();
            this.grpRecipeForm = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnHelper = new System.Windows.Forms.Button();
            this.grpMotionData = new System.Windows.Forms.GroupBox();
            this.txtTPos = new System.Windows.Forms.TextBox();
            this.txtZPos = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblRPos = new System.Windows.Forms.Label();
            this.txtRPos = new System.Windows.Forms.TextBox();
            this.lblMPos = new System.Windows.Forms.Label();
            this.txtMPos = new System.Windows.Forms.TextBox();
            this.RecipeFormPanel = new System.Windows.Forms.TableLayoutPanel();
            this.bsDesign = new System.Windows.Forms.BindingSource(this.components);
            this.bsScan = new System.Windows.Forms.BindingSource(this.components);
            this.bsReview = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numAlignX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanToRevOfsX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanToRevOfsY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevOfsX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevOfsY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolY)).BeginInit();
            this.grpParam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVectorRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVectorCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHoles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstY)).BeginInit();
            this.tab.SuspendLayout();
            this.tabDrawScan.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridScan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDesign)).BeginInit();
            this.tabReview.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlyVisionLines)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionStepCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReviewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridReview)).BeginInit();
            this.grpRecipeForm.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.grpMotionData.SuspendLayout();
            this.RecipeFormPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsDesign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsScan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsReview)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(6, 19);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(93, 15);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Recipe Header";
            // 
            // lblRecipe
            // 
            this.lblRecipe.AutoSize = true;
            this.lblRecipe.Location = new System.Drawing.Point(6, 41);
            this.lblRecipe.Name = "lblRecipe";
            this.lblRecipe.Size = new System.Drawing.Size(78, 15);
            this.lblRecipe.TabIndex = 1;
            this.lblRecipe.Text = "Recipe Name";
            // 
            // txtRecipe
            // 
            this.txtRecipe.Location = new System.Drawing.Point(96, 39);
            this.txtRecipe.Name = "txtRecipe";
            this.txtRecipe.Size = new System.Drawing.Size(250, 23);
            this.txtRecipe.TabIndex = 2;
            // 
            // lblAlign
            // 
            this.lblAlign.AutoSize = true;
            this.lblAlign.Location = new System.Drawing.Point(-1, 28);
            this.lblAlign.Name = "lblAlign";
            this.lblAlign.Size = new System.Drawing.Size(123, 15);
            this.lblAlign.TabIndex = 3;
            this.lblAlign.Text = "Align Mark (Drawing)";
            // 
            // lblAX
            // 
            this.lblAX.AutoSize = true;
            this.lblAX.Location = new System.Drawing.Point(119, 28);
            this.lblAX.Name = "lblAX";
            this.lblAX.Size = new System.Drawing.Size(14, 15);
            this.lblAX.TabIndex = 4;
            this.lblAX.Text = "X";
            // 
            // numAlignX
            // 
            this.numAlignX.DecimalPlaces = 3;
            this.numAlignX.Location = new System.Drawing.Point(139, 26);
            this.numAlignX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAlignX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numAlignX.Name = "numAlignX";
            this.numAlignX.Size = new System.Drawing.Size(90, 23);
            this.numAlignX.TabIndex = 5;
            // 
            // lblAY
            // 
            this.lblAY.AutoSize = true;
            this.lblAY.Location = new System.Drawing.Point(234, 28);
            this.lblAY.Name = "lblAY";
            this.lblAY.Size = new System.Drawing.Size(14, 15);
            this.lblAY.TabIndex = 6;
            this.lblAY.Text = "Y";
            // 
            // numAlignY
            // 
            this.numAlignY.DecimalPlaces = 3;
            this.numAlignY.Location = new System.Drawing.Point(254, 26);
            this.numAlignY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAlignY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numAlignY.Name = "numAlignY";
            this.numAlignY.Size = new System.Drawing.Size(90, 23);
            this.numAlignY.TabIndex = 7;
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(366, 41);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(202, 15);
            this.lblOffset.TabIndex = 8;
            this.lblOffset.Text = "Physical Offsets (Scanner ↔ Review)";
            // 
            // lblSOX
            // 
            this.lblSOX.AutoSize = true;
            this.lblSOX.Location = new System.Drawing.Point(366, 67);
            this.lblSOX.Name = "lblSOX";
            this.lblSOX.Size = new System.Drawing.Size(91, 15);
            this.lblSOX.TabIndex = 9;
            this.lblSOX.Text = "canToRev Ofs X";
            // 
            // numScanToRevOfsX
            // 
            this.numScanToRevOfsX.DecimalPlaces = 3;
            this.numScanToRevOfsX.Location = new System.Drawing.Point(462, 65);
            this.numScanToRevOfsX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numScanToRevOfsX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numScanToRevOfsX.Name = "numScanToRevOfsX";
            this.numScanToRevOfsX.Size = new System.Drawing.Size(90, 23);
            this.numScanToRevOfsX.TabIndex = 10;
            // 
            // lblSOY
            // 
            this.lblSOY.AutoSize = true;
            this.lblSOY.Location = new System.Drawing.Point(559, 67);
            this.lblSOY.Name = "lblSOY";
            this.lblSOY.Size = new System.Drawing.Size(14, 15);
            this.lblSOY.TabIndex = 11;
            this.lblSOY.Text = "Y";
            // 
            // numScanToRevOfsY
            // 
            this.numScanToRevOfsY.DecimalPlaces = 3;
            this.numScanToRevOfsY.Location = new System.Drawing.Point(579, 65);
            this.numScanToRevOfsY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numScanToRevOfsY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numScanToRevOfsY.Name = "numScanToRevOfsY";
            this.numScanToRevOfsY.Size = new System.Drawing.Size(90, 23);
            this.numScanToRevOfsY.TabIndex = 12;
            // 
            // lblROX
            // 
            this.lblROX.AutoSize = true;
            this.lblROX.Location = new System.Drawing.Point(700, 67);
            this.lblROX.Name = "lblROX";
            this.lblROX.Size = new System.Drawing.Size(77, 15);
            this.lblROX.TabIndex = 13;
            this.lblROX.Text = "Review Ofs X";
            // 
            // numRevOfsX
            // 
            this.numRevOfsX.DecimalPlaces = 3;
            this.numRevOfsX.Location = new System.Drawing.Point(780, 65);
            this.numRevOfsX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numRevOfsX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numRevOfsX.Name = "numRevOfsX";
            this.numRevOfsX.Size = new System.Drawing.Size(90, 23);
            this.numRevOfsX.TabIndex = 14;
            // 
            // lblROY
            // 
            this.lblROY.AutoSize = true;
            this.lblROY.Location = new System.Drawing.Point(875, 67);
            this.lblROY.Name = "lblROY";
            this.lblROY.Size = new System.Drawing.Size(14, 15);
            this.lblROY.TabIndex = 15;
            this.lblROY.Text = "Y";
            // 
            // numRevOfsY
            // 
            this.numRevOfsY.DecimalPlaces = 3;
            this.numRevOfsY.Location = new System.Drawing.Point(895, 65);
            this.numRevOfsY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numRevOfsY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numRevOfsY.Name = "numRevOfsY";
            this.numRevOfsY.Size = new System.Drawing.Size(90, 23);
            this.numRevOfsY.TabIndex = 16;
            // 
            // lblCrit
            // 
            this.lblCrit.AutoSize = true;
            this.lblCrit.Location = new System.Drawing.Point(366, 104);
            this.lblCrit.Name = "lblCrit";
            this.lblCrit.Size = new System.Drawing.Size(150, 15);
            this.lblCrit.TabIndex = 17;
            this.lblCrit.Text = "Review Criteria (|Err| ≤ Tol)";
            // 
            // lblTolX
            // 
            this.lblTolX.AutoSize = true;
            this.lblTolX.Location = new System.Drawing.Point(428, 127);
            this.lblTolX.Name = "lblTolX";
            this.lblTolX.Size = new System.Drawing.Size(30, 15);
            this.lblTolX.TabIndex = 18;
            this.lblTolX.Text = "TolX";
            // 
            // numTolX
            // 
            this.numTolX.DecimalPlaces = 3;
            this.numTolX.Location = new System.Drawing.Point(463, 125);
            this.numTolX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTolX.Name = "numTolX";
            this.numTolX.Size = new System.Drawing.Size(80, 23);
            this.numTolX.TabIndex = 19;
            // 
            // lblTolY
            // 
            this.lblTolY.AutoSize = true;
            this.lblTolY.Location = new System.Drawing.Point(545, 127);
            this.lblTolY.Name = "lblTolY";
            this.lblTolY.Size = new System.Drawing.Size(30, 15);
            this.lblTolY.TabIndex = 20;
            this.lblTolY.Text = "TolY";
            // 
            // numTolY
            // 
            this.numTolY.DecimalPlaces = 3;
            this.numTolY.Location = new System.Drawing.Point(580, 125);
            this.numTolY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTolY.Name = "numTolY";
            this.numTolY.Size = new System.Drawing.Size(80, 23);
            this.numTolY.TabIndex = 21;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1009, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 24);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "Save";
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(1109, 39);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(95, 24);
            this.btnCommit.TabIndex = 23;
            this.btnCommit.Text = "Commit";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1009, 67);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(95, 24);
            this.btnNew.TabIndex = 24;
            this.btnNew.Text = "New";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(1109, 67);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 24);
            this.btnDelete.TabIndex = 25;
            this.btnDelete.Text = "Delete";
            // 
            // grpParam
            // 
            this.grpParam.Controls.Add(this.lblVecRow);
            this.grpParam.Controls.Add(this._btnPowerMeterEditor);
            this.grpParam.Controls.Add(this._btnToolingEditor);
            this.grpParam.Controls.Add(this.lblLines);
            this.grpParam.Controls.Add(this.numVectorRow);
            this.grpParam.Controls.Add(this.numLines);
            this.grpParam.Controls.Add(this.lblVecCol);
            this.grpParam.Controls.Add(this.lblCols);
            this.grpParam.Controls.Add(this.numVectorCol);
            this.grpParam.Controls.Add(this.numHoles);
            this.grpParam.Controls.Add(this.lblPitchX);
            this.grpParam.Controls.Add(this.numPitchX);
            this.grpParam.Controls.Add(this.lblPitchY);
            this.grpParam.Controls.Add(this.numPitchY);
            this.grpParam.Controls.Add(this.btnRebuild);
            this.grpParam.Location = new System.Drawing.Point(8, 164);
            this.grpParam.Name = "grpParam";
            this.grpParam.Size = new System.Drawing.Size(1143, 114);
            this.grpParam.TabIndex = 26;
            this.grpParam.TabStop = false;
            this.grpParam.Text = "Layout Parameters (Drawing Coordinates)";
            // 
            // lblVecRow
            // 
            this.lblVecRow.AutoSize = true;
            this.lblVecRow.Location = new System.Drawing.Point(274, 28);
            this.lblVecRow.Name = "lblVecRow";
            this.lblVecRow.Size = new System.Drawing.Size(65, 15);
            this.lblVecRow.TabIndex = 0;
            this.lblVecRow.Text = "VectorRow";
            // 
            // _btnPowerMeterEditor
            // 
            this._btnPowerMeterEditor.Location = new System.Drawing.Point(963, 28);
            this._btnPowerMeterEditor.Name = "_btnPowerMeterEditor";
            this._btnPowerMeterEditor.Size = new System.Drawing.Size(154, 25);
            this._btnPowerMeterEditor.TabIndex = 14;
            this._btnPowerMeterEditor.Text = "PowerMeter Editor";
            // 
            // _btnToolingEditor
            // 
            this._btnToolingEditor.Location = new System.Drawing.Point(823, 28);
            this._btnToolingEditor.Name = "_btnToolingEditor";
            this._btnToolingEditor.Size = new System.Drawing.Size(120, 25);
            this._btnToolingEditor.TabIndex = 13;
            this._btnToolingEditor.Text = "Tooling Editor";
            // 
            // lblLines
            // 
            this.lblLines.AutoSize = true;
            this.lblLines.Location = new System.Drawing.Point(16, 28);
            this.lblLines.Name = "lblLines";
            this.lblLines.Size = new System.Drawing.Size(34, 15);
            this.lblLines.TabIndex = 0;
            this.lblLines.Text = "Lines";
            // 
            // numVectorRow
            // 
            this.numVectorRow.Location = new System.Drawing.Point(343, 26);
            this.numVectorRow.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numVectorRow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVectorRow.Name = "numVectorRow";
            this.numVectorRow.Size = new System.Drawing.Size(60, 23);
            this.numVectorRow.TabIndex = 1;
            this.numVectorRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numLines
            // 
            this.numLines.Location = new System.Drawing.Point(58, 26);
            this.numLines.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLines.Name = "numLines";
            this.numLines.Size = new System.Drawing.Size(60, 23);
            this.numLines.TabIndex = 1;
            this.numLines.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblVecCol
            // 
            this.lblVecCol.AutoSize = true;
            this.lblVecCol.Location = new System.Drawing.Point(412, 28);
            this.lblVecCol.Name = "lblVecCol";
            this.lblVecCol.Size = new System.Drawing.Size(60, 15);
            this.lblVecCol.TabIndex = 2;
            this.lblVecCol.Text = "VectorCol";
            // 
            // lblCols
            // 
            this.lblCols.AutoSize = true;
            this.lblCols.Location = new System.Drawing.Point(132, 28);
            this.lblCols.Name = "lblCols";
            this.lblCols.Size = new System.Drawing.Size(37, 15);
            this.lblCols.TabIndex = 2;
            this.lblCols.Text = "Holes";
            // 
            // numVectorCol
            // 
            this.numVectorCol.Location = new System.Drawing.Point(478, 26);
            this.numVectorCol.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numVectorCol.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVectorCol.Name = "numVectorCol";
            this.numVectorCol.Size = new System.Drawing.Size(60, 23);
            this.numVectorCol.TabIndex = 3;
            this.numVectorCol.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numHoles
            // 
            this.numHoles.Location = new System.Drawing.Point(177, 26);
            this.numHoles.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numHoles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHoles.Name = "numHoles";
            this.numHoles.Size = new System.Drawing.Size(60, 23);
            this.numHoles.TabIndex = 3;
            this.numHoles.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblPitchX
            // 
            this.lblPitchX.AutoSize = true;
            this.lblPitchX.Location = new System.Drawing.Point(8, 67);
            this.lblPitchX.Name = "lblPitchX";
            this.lblPitchX.Size = new System.Drawing.Size(41, 15);
            this.lblPitchX.TabIndex = 4;
            this.lblPitchX.Text = "PitchX";
            // 
            // numPitchX
            // 
            this.numPitchX.DecimalPlaces = 3;
            this.numPitchX.Location = new System.Drawing.Point(58, 65);
            this.numPitchX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numPitchX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numPitchX.Name = "numPitchX";
            this.numPitchX.Size = new System.Drawing.Size(60, 23);
            this.numPitchX.TabIndex = 5;
            // 
            // lblPitchY
            // 
            this.lblPitchY.AutoSize = true;
            this.lblPitchY.Location = new System.Drawing.Point(128, 67);
            this.lblPitchY.Name = "lblPitchY";
            this.lblPitchY.Size = new System.Drawing.Size(41, 15);
            this.lblPitchY.TabIndex = 6;
            this.lblPitchY.Text = "PitchY";
            // 
            // numPitchY
            // 
            this.numPitchY.DecimalPlaces = 3;
            this.numPitchY.Location = new System.Drawing.Point(177, 65);
            this.numPitchY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numPitchY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numPitchY.Name = "numPitchY";
            this.numPitchY.Size = new System.Drawing.Size(60, 23);
            this.numPitchY.TabIndex = 7;
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(583, 28);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(120, 25);
            this.btnRebuild.TabIndex = 12;
            this.btnRebuild.Text = "Rebuild Grid";
            // 
            // lblFirstX
            // 
            this.lblFirstX.AutoSize = true;
            this.lblFirstX.Location = new System.Drawing.Point(17, 64);
            this.lblFirstX.Name = "lblFirstX";
            this.lblFirstX.Size = new System.Drawing.Size(36, 15);
            this.lblFirstX.TabIndex = 8;
            this.lblFirstX.Text = "FirstX";
            // 
            // numFirstX
            // 
            this.numFirstX.DecimalPlaces = 3;
            this.numFirstX.Location = new System.Drawing.Point(62, 62);
            this.numFirstX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numFirstX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numFirstX.Name = "numFirstX";
            this.numFirstX.Size = new System.Drawing.Size(60, 23);
            this.numFirstX.TabIndex = 9;
            // 
            // lblFirstY
            // 
            this.lblFirstY.AutoSize = true;
            this.lblFirstY.Location = new System.Drawing.Point(152, 64);
            this.lblFirstY.Name = "lblFirstY";
            this.lblFirstY.Size = new System.Drawing.Size(36, 15);
            this.lblFirstY.TabIndex = 10;
            this.lblFirstY.Text = "FirstY";
            // 
            // numFirstY
            // 
            this.numFirstY.DecimalPlaces = 3;
            this.numFirstY.Location = new System.Drawing.Point(197, 62);
            this.numFirstY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numFirstY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numFirstY.Name = "numFirstY";
            this.numFirstY.Size = new System.Drawing.Size(60, 23);
            this.numFirstY.TabIndex = 11;
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tabDrawScan);
            this.tab.Controls.Add(this.tabReview);
            this.tab.Location = new System.Drawing.Point(9, 284);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(1430, 579);
            this.tab.TabIndex = 27;
            // 
            // tabDrawScan
            // 
            this.tabDrawScan.Controls.Add(this.tableLayoutPanel1);
            this.tabDrawScan.Location = new System.Drawing.Point(4, 24);
            this.tabDrawScan.Name = "tabDrawScan";
            this.tabDrawScan.Size = new System.Drawing.Size(1422, 551);
            this.tabDrawScan.TabIndex = 0;
            this.tabDrawScan.Text = "Drawing + Scanner";
            this.tabDrawScan.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.1F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 99.9F));
            this.tableLayoutPanel1.Controls.Add(this.lblDesign, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gridScan, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveScan, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblScan, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.gridDesign, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1422, 551);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lblDesign
            // 
            this.lblDesign.AutoSize = true;
            this.lblDesign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDesign.Location = new System.Drawing.Point(3, 0);
            this.lblDesign.Name = "lblDesign";
            this.lblDesign.Size = new System.Drawing.Size(1, 27);
            this.lblDesign.TabIndex = 0;
            this.lblDesign.Text = "① Drawing Coordinates (generated by Parameters)";
            // 
            // gridScan
            // 
            this.gridScan.AllowUserToAddRows = false;
            this.gridScan.AllowUserToDeleteRows = false;
            this.gridScan.AllowUserToResizeColumns = false;
            this.gridScan.AllowUserToResizeRows = false;
            this.gridScan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridScan.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridScan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.gridScan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.Format = "N3";
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridScan.DefaultCellStyle = dataGridViewCellStyle10;
            this.gridScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridScan.Location = new System.Drawing.Point(4, 57);
            this.gridScan.MultiSelect = false;
            this.gridScan.Name = "gridScan";
            this.gridScan.ReadOnly = true;
            this.gridScan.RowHeadersVisible = false;
            this.gridScan.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridScan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridScan.Size = new System.Drawing.Size(1415, 491);
            this.gridScan.TabIndex = 4;
            // 
            // btnMoveScan
            // 
            this.btnMoveScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMoveScan.Location = new System.Drawing.Point(4, 30);
            this.btnMoveScan.Name = "btnMoveScan";
            this.btnMoveScan.Size = new System.Drawing.Size(1415, 21);
            this.btnMoveScan.TabIndex = 3;
            this.btnMoveScan.Text = "Move To Selected";
            // 
            // lblScan
            // 
            this.lblScan.AutoSize = true;
            this.lblScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScan.Location = new System.Drawing.Point(4, 0);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(1415, 27);
            this.lblScan.TabIndex = 2;
            this.lblScan.Text = "② Scanner Coordinates";
            // 
            // gridDesign
            // 
            this.gridDesign.AllowUserToAddRows = false;
            this.gridDesign.AllowUserToDeleteRows = false;
            this.gridDesign.AllowUserToResizeColumns = false;
            this.gridDesign.AllowUserToResizeRows = false;
            this.gridDesign.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridDesign.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridDesign.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.gridDesign.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.Format = "N3";
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDesign.DefaultCellStyle = dataGridViewCellStyle12;
            this.gridDesign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDesign.Location = new System.Drawing.Point(3, 57);
            this.gridDesign.MultiSelect = false;
            this.gridDesign.Name = "gridDesign";
            this.gridDesign.ReadOnly = true;
            this.gridDesign.RowHeadersVisible = false;
            this.gridDesign.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridDesign.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDesign.Size = new System.Drawing.Size(1, 491);
            this.gridDesign.TabIndex = 1;
            this.gridDesign.Visible = false;
            // 
            // tabReview
            // 
            this.tabReview.Controls.Add(this.btnInitalReviewResults);
            this.tabReview.Controls.Add(this.btnMeasureLine);
            this.tabReview.Controls.Add(this.groupBox6);
            this.tabReview.Controls.Add(this.lblReview);
            this.tabReview.Controls.Add(this.groupBox2);
            this.tabReview.Controls.Add(this.btnMoveReview);
            this.tabReview.Controls.Add(this.groupBox1);
            this.tabReview.Controls.Add(this.btnApplyReviewResult);
            this.tabReview.Controls.Add(this.btnMeasureSingle);
            this.tabReview.Controls.Add(this.gridReviewDetail);
            this.tabReview.Controls.Add(this.gridReview);
            this.tabReview.Location = new System.Drawing.Point(4, 24);
            this.tabReview.Name = "tabReview";
            this.tabReview.Size = new System.Drawing.Size(1422, 551);
            this.tabReview.TabIndex = 1;
            this.tabReview.Text = "Review";
            this.tabReview.UseVisualStyleBackColor = true;
            // 
            // btnInitalReviewResults
            // 
            this.btnInitalReviewResults.Location = new System.Drawing.Point(1216, 29);
            this.btnInitalReviewResults.Name = "btnInitalReviewResults";
            this.btnInitalReviewResults.Size = new System.Drawing.Size(158, 47);
            this.btnInitalReviewResults.TabIndex = 16;
            this.btnInitalReviewResults.Text = "Init Review Results";
            // 
            // btnMeasureLine
            // 
            this.btnMeasureLine.Location = new System.Drawing.Point(1216, 474);
            this.btnMeasureLine.Name = "btnMeasureLine";
            this.btnMeasureLine.Size = new System.Drawing.Size(200, 55);
            this.btnMeasureLine.TabIndex = 4;
            this.btnMeasureLine.Text = "Measure\r\n(Step / On the Fly)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox3);
            this.groupBox6.Controls.Add(this.groupBox4);
            this.groupBox6.Controls.Add(this.groupBox5);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.numFlyVisionLines);
            this.groupBox6.Location = new System.Drawing.Point(1213, 298);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(203, 169);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "On the Fly Condition";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbtnXInspection);
            this.groupBox3.Controls.Add(this.rbtnYInspection);
            this.groupBox3.Location = new System.Drawing.Point(13, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(144, 32);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            // 
            // rbtnXInspection
            // 
            this.rbtnXInspection.AutoSize = true;
            this.rbtnXInspection.Location = new System.Drawing.Point(14, 11);
            this.rbtnXInspection.Name = "rbtnXInspection";
            this.rbtnXInspection.Size = new System.Drawing.Size(32, 19);
            this.rbtnXInspection.TabIndex = 11;
            this.rbtnXInspection.Text = "X";
            this.rbtnXInspection.UseVisualStyleBackColor = true;
            // 
            // rbtnYInspection
            // 
            this.rbtnYInspection.AutoSize = true;
            this.rbtnYInspection.Checked = true;
            this.rbtnYInspection.Location = new System.Drawing.Point(98, 11);
            this.rbtnYInspection.Name = "rbtnYInspection";
            this.rbtnYInspection.Size = new System.Drawing.Size(32, 19);
            this.rbtnYInspection.TabIndex = 12;
            this.rbtnYInspection.TabStop = true;
            this.rbtnYInspection.Text = "Y";
            this.rbtnYInspection.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbtnForwardInspection);
            this.groupBox4.Controls.Add(this.rbtnBackwardInspection);
            this.groupBox4.Location = new System.Drawing.Point(13, 90);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(184, 32);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            // 
            // rbtnForwardInspection
            // 
            this.rbtnForwardInspection.AutoSize = true;
            this.rbtnForwardInspection.Location = new System.Drawing.Point(14, 11);
            this.rbtnForwardInspection.Name = "rbtnForwardInspection";
            this.rbtnForwardInspection.Size = new System.Drawing.Size(68, 19);
            this.rbtnForwardInspection.TabIndex = 11;
            this.rbtnForwardInspection.Text = "Forward";
            this.rbtnForwardInspection.UseVisualStyleBackColor = true;
            // 
            // rbtnBackwardInspection
            // 
            this.rbtnBackwardInspection.AutoSize = true;
            this.rbtnBackwardInspection.Checked = true;
            this.rbtnBackwardInspection.Location = new System.Drawing.Point(98, 11);
            this.rbtnBackwardInspection.Name = "rbtnBackwardInspection";
            this.rbtnBackwardInspection.Size = new System.Drawing.Size(76, 19);
            this.rbtnBackwardInspection.TabIndex = 12;
            this.rbtnBackwardInspection.TabStop = true;
            this.rbtnBackwardInspection.Text = "Backward";
            this.rbtnBackwardInspection.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbtnOneWayInspection);
            this.groupBox5.Controls.Add(this.rbtnSnakeInspection);
            this.groupBox5.Location = new System.Drawing.Point(13, 126);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(184, 32);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            // 
            // rbtnOneWayInspection
            // 
            this.rbtnOneWayInspection.AutoSize = true;
            this.rbtnOneWayInspection.Checked = true;
            this.rbtnOneWayInspection.Location = new System.Drawing.Point(14, 11);
            this.rbtnOneWayInspection.Name = "rbtnOneWayInspection";
            this.rbtnOneWayInspection.Size = new System.Drawing.Size(70, 19);
            this.rbtnOneWayInspection.TabIndex = 11;
            this.rbtnOneWayInspection.TabStop = true;
            this.rbtnOneWayInspection.Text = "OneWay";
            this.rbtnOneWayInspection.UseVisualStyleBackColor = true;
            // 
            // rbtnSnakeInspection
            // 
            this.rbtnSnakeInspection.AutoSize = true;
            this.rbtnSnakeInspection.Location = new System.Drawing.Point(98, 11);
            this.rbtnSnakeInspection.Name = "rbtnSnakeInspection";
            this.rbtnSnakeInspection.Size = new System.Drawing.Size(57, 19);
            this.rbtnSnakeInspection.TabIndex = 12;
            this.rbtnSnakeInspection.Text = "Snake";
            this.rbtnSnakeInspection.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(18, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "Line Count";
            // 
            // numFlyVisionLines
            // 
            this.numFlyVisionLines.Location = new System.Drawing.Point(90, 25);
            this.numFlyVisionLines.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFlyVisionLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFlyVisionLines.Name = "numFlyVisionLines";
            this.numFlyVisionLines.Size = new System.Drawing.Size(63, 23);
            this.numFlyVisionLines.TabIndex = 3;
            this.numFlyVisionLines.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblReview
            // 
            this.lblReview.AutoSize = true;
            this.lblReview.Location = new System.Drawing.Point(9, 11);
            this.lblReview.Name = "lblReview";
            this.lblReview.Size = new System.Drawing.Size(109, 15);
            this.lblReview.TabIndex = 0;
            this.lblReview.Text = "③ Review Measure";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnVisionStepHoles);
            this.groupBox2.Controls.Add(this.rbtnVisionStepAllWholeLine);
            this.groupBox2.Controls.Add(this.rbtnVisionStepAllHoles);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numVisionStepCount);
            this.groupBox2.Location = new System.Drawing.Point(1213, 207);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 85);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Step by Step Condition";
            // 
            // rbtnVisionStepHoles
            // 
            this.rbtnVisionStepHoles.AutoSize = true;
            this.rbtnVisionStepHoles.Location = new System.Drawing.Point(149, 25);
            this.rbtnVisionStepHoles.Name = "rbtnVisionStepHoles";
            this.rbtnVisionStepHoles.Size = new System.Drawing.Size(49, 19);
            this.rbtnVisionStepHoles.TabIndex = 5;
            this.rbtnVisionStepHoles.TabStop = true;
            this.rbtnVisionStepHoles.Text = "Step";
            this.rbtnVisionStepHoles.UseVisualStyleBackColor = true;
            // 
            // rbtnVisionStepAllWholeLine
            // 
            this.rbtnVisionStepAllWholeLine.AutoSize = true;
            this.rbtnVisionStepAllWholeLine.Location = new System.Drawing.Point(100, 57);
            this.rbtnVisionStepAllWholeLine.Name = "rbtnVisionStepAllWholeLine";
            this.rbtnVisionStepAllWholeLine.Size = new System.Drawing.Size(94, 19);
            this.rbtnVisionStepAllWholeLine.TabIndex = 4;
            this.rbtnVisionStepAllWholeLine.TabStop = true;
            this.rbtnVisionStepAllWholeLine.Text = "All Cell Lines";
            this.rbtnVisionStepAllWholeLine.UseVisualStyleBackColor = true;
            // 
            // rbtnVisionStepAllHoles
            // 
            this.rbtnVisionStepAllHoles.AutoSize = true;
            this.rbtnVisionStepAllHoles.Location = new System.Drawing.Point(16, 57);
            this.rbtnVisionStepAllHoles.Name = "rbtnVisionStepAllHoles";
            this.rbtnVisionStepAllHoles.Size = new System.Drawing.Size(73, 19);
            this.rbtnVisionStepAllHoles.TabIndex = 4;
            this.rbtnVisionStepAllHoles.TabStop = true;
            this.rbtnVisionStepAllHoles.Text = "All Holes";
            this.rbtnVisionStepAllHoles.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(5, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Step Count";
            // 
            // numVisionStepCount
            // 
            this.numVisionStepCount.Location = new System.Drawing.Point(80, 23);
            this.numVisionStepCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numVisionStepCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVisionStepCount.Name = "numVisionStepCount";
            this.numVisionStepCount.Size = new System.Drawing.Size(63, 23);
            this.numVisionStepCount.TabIndex = 0;
            this.numVisionStepCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnMoveReview
            // 
            this.btnMoveReview.Location = new System.Drawing.Point(865, 84);
            this.btnMoveReview.Name = "btnMoveReview";
            this.btnMoveReview.Size = new System.Drawing.Size(332, 23);
            this.btnMoveReview.TabIndex = 1;
            this.btnMoveReview.Text = "Move To Selected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnVisionStepbyStep);
            this.groupBox1.Controls.Add(this.rbtnVisionOntheFly);
            this.groupBox1.Location = new System.Drawing.Point(1213, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 60);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inspection Type";
            // 
            // rbtnVisionStepbyStep
            // 
            this.rbtnVisionStepbyStep.AutoSize = true;
            this.rbtnVisionStepbyStep.Location = new System.Drawing.Point(18, 26);
            this.rbtnVisionStepbyStep.Name = "rbtnVisionStepbyStep";
            this.rbtnVisionStepbyStep.Size = new System.Drawing.Size(49, 19);
            this.rbtnVisionStepbyStep.TabIndex = 9;
            this.rbtnVisionStepbyStep.Text = "Step";
            this.rbtnVisionStepbyStep.UseVisualStyleBackColor = true;
            // 
            // rbtnVisionOntheFly
            // 
            this.rbtnVisionOntheFly.AutoSize = true;
            this.rbtnVisionOntheFly.Checked = true;
            this.rbtnVisionOntheFly.Location = new System.Drawing.Point(80, 26);
            this.rbtnVisionOntheFly.Name = "rbtnVisionOntheFly";
            this.rbtnVisionOntheFly.Size = new System.Drawing.Size(81, 19);
            this.rbtnVisionOntheFly.TabIndex = 10;
            this.rbtnVisionOntheFly.TabStop = true;
            this.rbtnVisionOntheFly.Text = "On the Fly";
            this.rbtnVisionOntheFly.UseVisualStyleBackColor = true;
            // 
            // btnApplyReviewResult
            // 
            this.btnApplyReviewResult.Location = new System.Drawing.Point(1039, 29);
            this.btnApplyReviewResult.Name = "btnApplyReviewResult";
            this.btnApplyReviewResult.Size = new System.Drawing.Size(158, 47);
            this.btnApplyReviewResult.TabIndex = 2;
            this.btnApplyReviewResult.Text = "Apply Review Results";
            // 
            // btnMeasureSingle
            // 
            this.btnMeasureSingle.Location = new System.Drawing.Point(1216, 82);
            this.btnMeasureSingle.Name = "btnMeasureSingle";
            this.btnMeasureSingle.Size = new System.Drawing.Size(158, 47);
            this.btnMeasureSingle.TabIndex = 2;
            this.btnMeasureSingle.Text = "Measure (Single)";
            // 
            // gridReviewDetail
            // 
            this.gridReviewDetail.AllowUserToAddRows = false;
            this.gridReviewDetail.AllowUserToDeleteRows = false;
            this.gridReviewDetail.AllowUserToResizeColumns = false;
            this.gridReviewDetail.AllowUserToResizeRows = false;
            this.gridReviewDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridReviewDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridReviewDetail.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridReviewDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.gridReviewDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.Format = "N3";
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridReviewDetail.DefaultCellStyle = dataGridViewCellStyle14;
            this.gridReviewDetail.Location = new System.Drawing.Point(12, 29);
            this.gridReviewDetail.MultiSelect = false;
            this.gridReviewDetail.Name = "gridReviewDetail";
            this.gridReviewDetail.ReadOnly = true;
            this.gridReviewDetail.RowHeadersVisible = false;
            this.gridReviewDetail.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridReviewDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridReviewDetail.Size = new System.Drawing.Size(847, 74);
            this.gridReviewDetail.TabIndex = 3;
            // 
            // gridReview
            // 
            this.gridReview.AllowUserToAddRows = false;
            this.gridReview.AllowUserToDeleteRows = false;
            this.gridReview.AllowUserToResizeColumns = false;
            this.gridReview.AllowUserToResizeRows = false;
            this.gridReview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridReview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridReview.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridReview.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.gridReview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("맑은 고딕", 9F);
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle16.Format = "N3";
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridReview.DefaultCellStyle = dataGridViewCellStyle16;
            this.gridReview.Location = new System.Drawing.Point(12, 129);
            this.gridReview.MultiSelect = false;
            this.gridReview.Name = "gridReview";
            this.gridReview.ReadOnly = true;
            this.gridReview.RowHeadersVisible = false;
            this.gridReview.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridReview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridReview.Size = new System.Drawing.Size(1185, 380);
            this.gridReview.TabIndex = 3;
            // 
            // grpRecipeForm
            // 
            this.grpRecipeForm.Controls.Add(this.groupBox7);
            this.grpRecipeForm.Controls.Add(this.btnHelper);
            this.grpRecipeForm.Controls.Add(this.grpMotionData);
            this.grpRecipeForm.Controls.Add(this.lblHeader);
            this.grpRecipeForm.Controls.Add(this.tab);
            this.grpRecipeForm.Controls.Add(this.grpParam);
            this.grpRecipeForm.Controls.Add(this.lblRecipe);
            this.grpRecipeForm.Controls.Add(this.btnDelete);
            this.grpRecipeForm.Controls.Add(this.txtRecipe);
            this.grpRecipeForm.Controls.Add(this.btnNew);
            this.grpRecipeForm.Controls.Add(this.btnCommit);
            this.grpRecipeForm.Controls.Add(this.btnSave);
            this.grpRecipeForm.Controls.Add(this.numTolY);
            this.grpRecipeForm.Controls.Add(this.lblTolY);
            this.grpRecipeForm.Controls.Add(this.numTolX);
            this.grpRecipeForm.Controls.Add(this.lblOffset);
            this.grpRecipeForm.Controls.Add(this.lblTolX);
            this.grpRecipeForm.Controls.Add(this.lblSOX);
            this.grpRecipeForm.Controls.Add(this.lblCrit);
            this.grpRecipeForm.Controls.Add(this.numScanToRevOfsX);
            this.grpRecipeForm.Controls.Add(this.numRevOfsY);
            this.grpRecipeForm.Controls.Add(this.lblSOY);
            this.grpRecipeForm.Controls.Add(this.lblROY);
            this.grpRecipeForm.Controls.Add(this.numScanToRevOfsY);
            this.grpRecipeForm.Controls.Add(this.numRevOfsX);
            this.grpRecipeForm.Controls.Add(this.lblROX);
            this.grpRecipeForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpRecipeForm.Location = new System.Drawing.Point(3, 3);
            this.grpRecipeForm.Name = "grpRecipeForm";
            this.grpRecipeForm.Size = new System.Drawing.Size(1445, 869);
            this.grpRecipeForm.TabIndex = 29;
            this.grpRecipeForm.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lblAlign);
            this.groupBox7.Controls.Add(this.numAlignY);
            this.groupBox7.Controls.Add(this.lblAY);
            this.groupBox7.Controls.Add(this.numAlignX);
            this.groupBox7.Controls.Add(this.lblAX);
            this.groupBox7.Controls.Add(this.numFirstX);
            this.groupBox7.Controls.Add(this.numFirstY);
            this.groupBox7.Controls.Add(this.lblFirstY);
            this.groupBox7.Controls.Add(this.lblFirstX);
            this.groupBox7.Location = new System.Drawing.Point(806, 22);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(26, 19);
            this.groupBox7.TabIndex = 30;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "groupBox7";
            this.groupBox7.Visible = false;
            // 
            // btnHelper
            // 
            this.btnHelper.Location = new System.Drawing.Point(15, 121);
            this.btnHelper.Name = "btnHelper";
            this.btnHelper.Size = new System.Drawing.Size(42, 25);
            this.btnHelper.TabIndex = 13;
            this.btnHelper.Text = "Help";
            // 
            // grpMotionData
            // 
            this.grpMotionData.Controls.Add(this.txtTPos);
            this.grpMotionData.Controls.Add(this.txtZPos);
            this.grpMotionData.Controls.Add(this.label6);
            this.grpMotionData.Controls.Add(this.label5);
            this.grpMotionData.Controls.Add(this.lblRPos);
            this.grpMotionData.Controls.Add(this.txtRPos);
            this.grpMotionData.Controls.Add(this.lblMPos);
            this.grpMotionData.Controls.Add(this.txtMPos);
            this.grpMotionData.Location = new System.Drawing.Point(1158, 121);
            this.grpMotionData.Name = "grpMotionData";
            this.grpMotionData.Size = new System.Drawing.Size(212, 157);
            this.grpMotionData.TabIndex = 29;
            this.grpMotionData.TabStop = false;
            this.grpMotionData.Text = "Motion Display";
            // 
            // txtTPos
            // 
            this.txtTPos.Location = new System.Drawing.Point(107, 110);
            this.txtTPos.Name = "txtTPos";
            this.txtTPos.ReadOnly = true;
            this.txtTPos.Size = new System.Drawing.Size(85, 23);
            this.txtTPos.TabIndex = 18;
            // 
            // txtZPos
            // 
            this.txtZPos.Location = new System.Drawing.Point(107, 81);
            this.txtZPos.Name = "txtZPos";
            this.txtZPos.ReadOnly = true;
            this.txtZPos.Size = new System.Drawing.Size(85, 23);
            this.txtZPos.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(17, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 23);
            this.label6.TabIndex = 16;
            this.label6.Text = "Theta Pos";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(17, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 23);
            this.label5.TabIndex = 15;
            this.label5.Text = "Z Pos";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRPos
            // 
            this.lblRPos.Location = new System.Drawing.Point(17, 23);
            this.lblRPos.Name = "lblRPos";
            this.lblRPos.Size = new System.Drawing.Size(84, 23);
            this.lblRPos.TabIndex = 13;
            this.lblRPos.Text = "Review X Pos";
            this.lblRPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRPos
            // 
            this.txtRPos.Location = new System.Drawing.Point(107, 23);
            this.txtRPos.Name = "txtRPos";
            this.txtRPos.ReadOnly = true;
            this.txtRPos.Size = new System.Drawing.Size(85, 23);
            this.txtRPos.TabIndex = 14;
            // 
            // lblMPos
            // 
            this.lblMPos.Location = new System.Drawing.Point(17, 52);
            this.lblMPos.Name = "lblMPos";
            this.lblMPos.Size = new System.Drawing.Size(84, 23);
            this.lblMPos.TabIndex = 11;
            this.lblMPos.Text = "Main Y Pos";
            this.lblMPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMPos
            // 
            this.txtMPos.Location = new System.Drawing.Point(107, 52);
            this.txtMPos.Name = "txtMPos";
            this.txtMPos.ReadOnly = true;
            this.txtMPos.Size = new System.Drawing.Size(85, 23);
            this.txtMPos.TabIndex = 12;
            // 
            // RecipeFormPanel
            // 
            this.RecipeFormPanel.ColumnCount = 1;
            this.RecipeFormPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.RecipeFormPanel.Controls.Add(this.grpRecipeForm, 0, 0);
            this.RecipeFormPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecipeFormPanel.Location = new System.Drawing.Point(0, 0);
            this.RecipeFormPanel.Name = "RecipeFormPanel";
            this.RecipeFormPanel.RowCount = 1;
            this.RecipeFormPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.RecipeFormPanel.Size = new System.Drawing.Size(1451, 875);
            this.RecipeFormPanel.TabIndex = 30;
            // 
            // RecipeForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(2176, 1312);
            this.Controls.Add(this.RecipeFormPanel);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RecipeForm";
            this.Text = "RecipeForm";
            ((System.ComponentModel.ISupportInitialize)(this.numAlignX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanToRevOfsX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanToRevOfsY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevOfsX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevOfsY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolY)).EndInit();
            this.grpParam.ResumeLayout(false);
            this.grpParam.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVectorRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVectorCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHoles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstY)).EndInit();
            this.tab.ResumeLayout(false);
            this.tabDrawScan.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridScan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDesign)).EndInit();
            this.tabReview.ResumeLayout(false);
            this.tabReview.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlyVisionLines)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionStepCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReviewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridReview)).EndInit();
            this.grpRecipeForm.ResumeLayout(false);
            this.grpRecipeForm.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.grpMotionData.ResumeLayout(false);
            this.grpMotionData.PerformLayout();
            this.RecipeFormPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsDesign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsScan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsReview)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private BindingSource bsDesign, bsScan, bsReview;
        private Label lblHeader, lblRecipe, lblAlign, lblOffset, lblAX, lblAY, lblSOX, lblSOY, lblROX, lblROY, lblCrit, lblTolX, lblTolY;
        private TextBox txtRecipe;
        private NumericUpDown numAlignX, numAlignY, numScanToRevOfsX, numScanToRevOfsY, numRevOfsX, numRevOfsY, numTolX, numTolY;

        private Button btnSave, btnCommit, btnNew, btnDelete;

        private GroupBox grpParam;
        private Label lblLines, lblCols, lblPitchX, lblPitchY, lblFirstX, lblFirstY;
        private NumericUpDown numLines, numHoles, numPitchX, numPitchY, numFirstX, numFirstY;
        private Button btnRebuild;

        private TabControl tab;
        private TabPage tabDrawScan, tabReview;

        private Label lblDesign;
        private DataGridView gridDesign;
        private DataGridViewTextBoxColumn colDIdx, colDRow, colDCol, colDX, colDY;

        private Label lblScan; private Button btnMoveScan;
        private DataGridView gridScan;
        private DataGridViewTextBoxColumn colSIdx, colSRow, colSCol, colSX, colSY;

        private Label lblReview; private Button btnMoveReview, btnMeasureSingle;
        private DataGridView gridReview;
        private DataGridViewTextBoxColumn colRIdx, colRRow, colRCol, colRTgtX, colRTgtY, colRMeasX, colRMeasY, colRErrX, colRErrY, colRGrade;
        private Button _btnToolingEditor;
        private GroupBox grpRecipeForm;
        private TableLayoutPanel RecipeFormPanel;
        private GroupBox grpMotionData;
        private Label lblMPos;
        private TextBox txtMPos;
        private Label lblRPos;
        private TextBox txtRPos;
        private Button btnMeasureLine;
        private Button _btnPowerMeterEditor;
        private GroupBox groupBox1;
        private RadioButton rbtnVisionStepbyStep;
        private RadioButton rbtnVisionOntheFly;
        private GroupBox groupBox2;
        private Label label3;
        private NumericUpDown numVisionStepCount;
        private GroupBox groupBox6;
        private GroupBox groupBox3;
        private RadioButton rbtnXInspection;
        private RadioButton rbtnYInspection;
        private GroupBox groupBox4;
        private RadioButton rbtnForwardInspection;
        private RadioButton rbtnBackwardInspection;
        private GroupBox groupBox5;
        private RadioButton rbtnOneWayInspection;
        private RadioButton rbtnSnakeInspection;
        private Label label4;
        private NumericUpDown numFlyVisionLines;
        private TextBox txtTPos;
        private TextBox txtZPos;
        private Label label6;
        private Label label5;
        private Label lblVecRow;
        private NumericUpDown numVectorRow;
        private Label lblVecCol;
        private NumericUpDown numVectorCol;
        private Button btnHelper;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView gridReviewDetail;
        private Button btnInitalReviewResults;
        private Button btnApplyReviewResult;
        private RadioButton rbtnVisionStepAllWholeLine;
        private RadioButton rbtnVisionStepAllHoles;
        private RadioButton rbtnVisionStepHoles;
        private GroupBox groupBox7;
    }
}