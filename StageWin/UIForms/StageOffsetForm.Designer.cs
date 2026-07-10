namespace StageWin.UI
{
    partial class StageOffsetForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblYAPos = new System.Windows.Forms.Label();
            this.XAPos = new System.Windows.Forms.Label();
            this.lblYtPos = new System.Windows.Forms.Label();
            this.TargetXPos = new System.Windows.Forms.Label();
            this.capPosX = new System.Windows.Forms.Label();
            this.lbYPos = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StageYSpeed = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbXPos = new System.Windows.Forms.Label();
            this.lblXAPos = new System.Windows.Forms.Label();
            this.StageXSpeed = new System.Windows.Forms.Label();
            this.lblXtPos = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numLaserXTarget = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.numReview2XTarget = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.numAlignCamXTarget = new System.Windows.Forms.NumericUpDown();
            this.btnLaserPosMove = new System.Windows.Forms.Button();
            this.btnReview2PosMove = new System.Windows.Forms.Button();
            this.btnAlignCam1PosMove = new System.Windows.Forms.Button();
            this.btnMoveLaserPosStop = new System.Windows.Forms.Button();
            this.btnReview2MoveStop = new System.Windows.Forms.Button();
            this.btnAlignCam1MoveStop = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numLaserYTarget = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.numReview2YTarget = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.numAlignCamYTarget = new System.Windows.Forms.NumericUpDown();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.numStageOffsetMoveYSpeed = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.numStageOffsetMoveXSpeed = new System.Windows.Forms.NumericUpDown();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btnOffsetSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserXTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReview2XTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignCamXTarget)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserYTarget)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReview2YTarget)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignCamYTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStageOffsetMoveYSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStageOffsetMoveXSpeed)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblYAPos
            // 
            this.lblYAPos.AutoSize = true;
            this.lblYAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblYAPos.Location = new System.Drawing.Point(315, 50);
            this.lblYAPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblYAPos.Name = "lblYAPos";
            this.lblYAPos.Size = new System.Drawing.Size(64, 15);
            this.lblYAPos.TabIndex = 11;
            this.lblYAPos.Text = "0.000 mm";
            // 
            // XAPos
            // 
            this.XAPos.AutoSize = true;
            this.XAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.XAPos.Location = new System.Drawing.Point(315, 32);
            this.XAPos.Name = "XAPos";
            this.XAPos.Size = new System.Drawing.Size(146, 15);
            this.XAPos.TabIndex = 10;
            this.XAPos.Text = "Encoder 보정 전 Position";
            // 
            // lblYtPos
            // 
            this.lblYtPos.AutoSize = true;
            this.lblYtPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblYtPos.Location = new System.Drawing.Point(155, 50);
            this.lblYtPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblYtPos.Name = "lblYtPos";
            this.lblYtPos.Size = new System.Drawing.Size(64, 15);
            this.lblYtPos.TabIndex = 9;
            this.lblYtPos.Text = "0.000 mm";
            // 
            // TargetXPos
            // 
            this.TargetXPos.AutoSize = true;
            this.TargetXPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.TargetXPos.Location = new System.Drawing.Point(155, 32);
            this.TargetXPos.Name = "TargetXPos";
            this.TargetXPos.Size = new System.Drawing.Size(95, 15);
            this.TargetXPos.TabIndex = 8;
            this.TargetXPos.Text = "Target Position";
            // 
            // capPosX
            // 
            this.capPosX.AutoSize = true;
            this.capPosX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capPosX.Location = new System.Drawing.Point(12, 32);
            this.capPosX.Name = "capPosX";
            this.capPosX.Size = new System.Drawing.Size(62, 15);
            this.capPosX.TabIndex = 6;
            this.capPosX.Text = "F Position";
            // 
            // lbYPos
            // 
            this.lbYPos.AutoSize = true;
            this.lbYPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYPos.Location = new System.Drawing.Point(12, 50);
            this.lbYPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbYPos.Name = "lbYPos";
            this.lbYPos.Size = new System.Drawing.Size(64, 15);
            this.lbYPos.TabIndex = 7;
            this.lbYPos.Text = "0.000 mm";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StageYSpeed);
            this.groupBox1.Controls.Add(this.TargetXPos);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lbYPos);
            this.groupBox1.Controls.Add(this.lblYAPos);
            this.groupBox1.Controls.Add(this.capPosX);
            this.groupBox1.Controls.Add(this.XAPos);
            this.groupBox1.Controls.Add(this.lblYtPos);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(28, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 89);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stage Y Status";
            // 
            // StageYSpeed
            // 
            this.StageYSpeed.AutoSize = true;
            this.StageYSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.StageYSpeed.Location = new System.Drawing.Point(501, 50);
            this.StageYSpeed.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.StageYSpeed.Name = "StageYSpeed";
            this.StageYSpeed.Size = new System.Drawing.Size(89, 15);
            this.StageYSpeed.TabIndex = 15;
            this.StageYSpeed.Text = "0.000 mm/Sec";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(501, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 15);
            this.label10.TabIndex = 14;
            this.label10.Text = "속도";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbXPos);
            this.groupBox2.Controls.Add(this.lblXAPos);
            this.groupBox2.Controls.Add(this.StageXSpeed);
            this.groupBox2.Controls.Add(this.lblXtPos);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(28, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(756, 89);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stage X Status";
            // 
            // lbXPos
            // 
            this.lbXPos.AutoSize = true;
            this.lbXPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXPos.Location = new System.Drawing.Point(12, 50);
            this.lbXPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbXPos.Name = "lbXPos";
            this.lbXPos.Size = new System.Drawing.Size(64, 15);
            this.lbXPos.TabIndex = 16;
            this.lbXPos.Text = "0.000 mm";
            // 
            // lblXAPos
            // 
            this.lblXAPos.AutoSize = true;
            this.lblXAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblXAPos.Location = new System.Drawing.Point(315, 50);
            this.lblXAPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblXAPos.Name = "lblXAPos";
            this.lblXAPos.Size = new System.Drawing.Size(64, 15);
            this.lblXAPos.TabIndex = 18;
            this.lblXAPos.Text = "0.000 mm";
            // 
            // StageXSpeed
            // 
            this.StageXSpeed.AutoSize = true;
            this.StageXSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.StageXSpeed.Location = new System.Drawing.Point(501, 50);
            this.StageXSpeed.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.StageXSpeed.Name = "StageXSpeed";
            this.StageXSpeed.Size = new System.Drawing.Size(89, 15);
            this.StageXSpeed.TabIndex = 13;
            this.StageXSpeed.Text = "0.000 mm/Sec";
            // 
            // lblXtPos
            // 
            this.lblXtPos.AutoSize = true;
            this.lblXtPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblXtPos.Location = new System.Drawing.Point(155, 50);
            this.lblXtPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblXtPos.Name = "lblXtPos";
            this.lblXtPos.Size = new System.Drawing.Size(64, 15);
            this.lblXtPos.TabIndex = 17;
            this.lblXtPos.Text = "0.000 mm";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(501, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "속도";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(155, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Target Position";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(12, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "F Position";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(315, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Encoder 보정 전 Position";
            // 
            // numLaserXTarget
            // 
            this.numLaserXTarget.DecimalPlaces = 3;
            this.numLaserXTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numLaserXTarget.Location = new System.Drawing.Point(156, 271);
            this.numLaserXTarget.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numLaserXTarget.Name = "numLaserXTarget";
            this.numLaserXTarget.Size = new System.Drawing.Size(100, 23);
            this.numLaserXTarget.TabIndex = 16;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(125, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Stage X Offset";
            // 
            // numReview2XTarget
            // 
            this.numReview2XTarget.DecimalPlaces = 3;
            this.numReview2XTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numReview2XTarget.Location = new System.Drawing.Point(128, 49);
            this.numReview2XTarget.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numReview2XTarget.Name = "numReview2XTarget";
            this.numReview2XTarget.Size = new System.Drawing.Size(100, 23);
            this.numReview2XTarget.TabIndex = 26;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label21.Location = new System.Drawing.Point(125, 26);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(108, 13);
            this.label21.TabIndex = 33;
            this.label21.Text = "Stage X Offset";
            // 
            // numAlignCamXTarget
            // 
            this.numAlignCamXTarget.DecimalPlaces = 3;
            this.numAlignCamXTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numAlignCamXTarget.Location = new System.Drawing.Point(156, 505);
            this.numAlignCamXTarget.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numAlignCamXTarget.Name = "numAlignCamXTarget";
            this.numAlignCamXTarget.Size = new System.Drawing.Size(100, 23);
            this.numAlignCamXTarget.TabIndex = 31;
            // 
            // btnLaserPosMove
            // 
            this.btnLaserPosMove.AutoSize = true;
            this.btnLaserPosMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnLaserPosMove.Location = new System.Drawing.Point(428, 46);
            this.btnLaserPosMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnLaserPosMove.Name = "btnLaserPosMove";
            this.btnLaserPosMove.Size = new System.Drawing.Size(130, 25);
            this.btnLaserPosMove.TabIndex = 35;
            this.btnLaserPosMove.Text = "Move";
            // 
            // btnReview2PosMove
            // 
            this.btnReview2PosMove.AutoSize = true;
            this.btnReview2PosMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnReview2PosMove.Location = new System.Drawing.Point(428, 46);
            this.btnReview2PosMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnReview2PosMove.Name = "btnReview2PosMove";
            this.btnReview2PosMove.Size = new System.Drawing.Size(130, 25);
            this.btnReview2PosMove.TabIndex = 37;
            this.btnReview2PosMove.Text = "Move";
            // 
            // btnAlignCam1PosMove
            // 
            this.btnAlignCam1PosMove.AutoSize = true;
            this.btnAlignCam1PosMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnAlignCam1PosMove.Location = new System.Drawing.Point(428, 48);
            this.btnAlignCam1PosMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnAlignCam1PosMove.Name = "btnAlignCam1PosMove";
            this.btnAlignCam1PosMove.Size = new System.Drawing.Size(130, 25);
            this.btnAlignCam1PosMove.TabIndex = 38;
            this.btnAlignCam1PosMove.Text = "Move";
            // 
            // btnMoveLaserPosStop
            // 
            this.btnMoveLaserPosStop.AutoSize = true;
            this.btnMoveLaserPosStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMoveLaserPosStop.Location = new System.Drawing.Point(581, 46);
            this.btnMoveLaserPosStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMoveLaserPosStop.Name = "btnMoveLaserPosStop";
            this.btnMoveLaserPosStop.Size = new System.Drawing.Size(130, 25);
            this.btnMoveLaserPosStop.TabIndex = 39;
            this.btnMoveLaserPosStop.Text = "Return";
            // 
            // btnReview2MoveStop
            // 
            this.btnReview2MoveStop.AutoSize = true;
            this.btnReview2MoveStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnReview2MoveStop.Location = new System.Drawing.Point(581, 46);
            this.btnReview2MoveStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnReview2MoveStop.Name = "btnReview2MoveStop";
            this.btnReview2MoveStop.Size = new System.Drawing.Size(130, 25);
            this.btnReview2MoveStop.TabIndex = 41;
            this.btnReview2MoveStop.Text = "Return";
            // 
            // btnAlignCam1MoveStop
            // 
            this.btnAlignCam1MoveStop.AutoSize = true;
            this.btnAlignCam1MoveStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnAlignCam1MoveStop.Location = new System.Drawing.Point(581, 48);
            this.btnAlignCam1MoveStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnAlignCam1MoveStop.Name = "btnAlignCam1MoveStop";
            this.btnAlignCam1MoveStop.Size = new System.Drawing.Size(130, 25);
            this.btnAlignCam1MoveStop.TabIndex = 42;
            this.btnAlignCam1MoveStop.Text = "Return";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnMoveLaserPosStop);
            this.groupBox3.Controls.Add(this.btnLaserPosMove);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numLaserYTarget);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox3.Location = new System.Drawing.Point(28, 222);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(756, 89);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Laser Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(266, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Stage Y Offset";
            // 
            // numLaserYTarget
            // 
            this.numLaserYTarget.DecimalPlaces = 3;
            this.numLaserYTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numLaserYTarget.Location = new System.Drawing.Point(269, 49);
            this.numLaserYTarget.Maximum = new decimal(new int[] {
            2340,
            0,
            0,
            0});
            this.numLaserYTarget.Name = "numLaserYTarget";
            this.numLaserYTarget.Size = new System.Drawing.Size(100, 23);
            this.numLaserYTarget.TabIndex = 45;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.btnReview2MoveStop);
            this.groupBox5.Controls.Add(this.btnReview2PosMove);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.numReview2XTarget);
            this.groupBox5.Controls.Add(this.numReview2YTarget);
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox5.Location = new System.Drawing.Point(28, 338);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(756, 89);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Review Cam 2X Position";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(125, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Stage X Offset";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(266, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(108, 13);
            this.label11.TabIndex = 54;
            this.label11.Text = "Stage Y Offset";
            // 
            // numReview2YTarget
            // 
            this.numReview2YTarget.DecimalPlaces = 3;
            this.numReview2YTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numReview2YTarget.Location = new System.Drawing.Point(269, 49);
            this.numReview2YTarget.Maximum = new decimal(new int[] {
            2340,
            0,
            0,
            0});
            this.numReview2YTarget.Name = "numReview2YTarget";
            this.numReview2YTarget.Size = new System.Drawing.Size(100, 23);
            this.numReview2YTarget.TabIndex = 53;
            // 
            // groupBox6
            // 
            this.groupBox6.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox6.Location = new System.Drawing.Point(0, 93);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(756, 89);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Review Cam 2X Position";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnAlignCam1MoveStop);
            this.groupBox7.Controls.Add(this.label14);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Controls.Add(this.numAlignCamYTarget);
            this.groupBox7.Controls.Add(this.btnAlignCam1PosMove);
            this.groupBox7.Controls.Add(this.groupBox8);
            this.groupBox7.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox7.Location = new System.Drawing.Point(28, 454);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(756, 89);
            this.groupBox7.TabIndex = 19;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Align Cam1 Position";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label14.Location = new System.Drawing.Point(266, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(108, 13);
            this.label14.TabIndex = 58;
            this.label14.Text = "Stage Y Offset";
            // 
            // numAlignCamYTarget
            // 
            this.numAlignCamYTarget.DecimalPlaces = 3;
            this.numAlignCamYTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numAlignCamYTarget.Location = new System.Drawing.Point(269, 51);
            this.numAlignCamYTarget.Maximum = new decimal(new int[] {
            2340,
            0,
            0,
            0});
            this.numAlignCamYTarget.Name = "numAlignCamYTarget";
            this.numAlignCamYTarget.Size = new System.Drawing.Size(100, 23);
            this.numAlignCamYTarget.TabIndex = 57;
            // 
            // groupBox8
            // 
            this.groupBox8.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox8.Location = new System.Drawing.Point(0, 93);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(756, 89);
            this.groupBox8.TabIndex = 18;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Review Cam 2X Position";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label22.Location = new System.Drawing.Point(7, 104);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(189, 13);
            this.label22.TabIndex = 68;
            this.label22.Text = "Stage Y Speed (mm/Sec)";
            // 
            // numStageOffsetMoveYSpeed
            // 
            this.numStageOffsetMoveYSpeed.DecimalPlaces = 3;
            this.numStageOffsetMoveYSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numStageOffsetMoveYSpeed.Location = new System.Drawing.Point(10, 132);
            this.numStageOffsetMoveYSpeed.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numStageOffsetMoveYSpeed.Name = "numStageOffsetMoveYSpeed";
            this.numStageOffsetMoveYSpeed.Size = new System.Drawing.Size(100, 23);
            this.numStageOffsetMoveYSpeed.TabIndex = 67;
            this.numStageOffsetMoveYSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label23.Location = new System.Drawing.Point(7, 29);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(189, 13);
            this.label23.TabIndex = 66;
            this.label23.Text = "Stage X Speed (mm/Sec)";
            // 
            // numStageOffsetMoveXSpeed
            // 
            this.numStageOffsetMoveXSpeed.DecimalPlaces = 3;
            this.numStageOffsetMoveXSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numStageOffsetMoveXSpeed.Location = new System.Drawing.Point(10, 57);
            this.numStageOffsetMoveXSpeed.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numStageOffsetMoveXSpeed.Name = "numStageOffsetMoveXSpeed";
            this.numStageOffsetMoveXSpeed.Size = new System.Drawing.Size(100, 23);
            this.numStageOffsetMoveXSpeed.TabIndex = 65;
            this.numStageOffsetMoveXSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label23);
            this.groupBox9.Controls.Add(this.label22);
            this.groupBox9.Controls.Add(this.numStageOffsetMoveXSpeed);
            this.groupBox9.Controls.Add(this.numStageOffsetMoveYSpeed);
            this.groupBox9.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox9.Location = new System.Drawing.Point(806, 12);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(225, 183);
            this.groupBox9.TabIndex = 16;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Speed Config";
            // 
            // btnOffsetSave
            // 
            this.btnOffsetSave.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOffsetSave.Location = new System.Drawing.Point(806, 231);
            this.btnOffsetSave.Name = "btnOffsetSave";
            this.btnOffsetSave.Size = new System.Drawing.Size(225, 43);
            this.btnOffsetSave.TabIndex = 34;
            this.btnOffsetSave.Text = "Parameter Save";
            this.btnOffsetSave.UseVisualStyleBackColor = true;
            // 
            // StageOffsetForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1065, 584);
            this.Controls.Add(this.btnOffsetSave);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.numAlignCamXTarget);
            this.Controls.Add(this.numLaserXTarget);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox7);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Name = "StageOffsetForm";
            this.Text = "StageOffsetForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserXTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReview2XTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignCamXTarget)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLaserYTarget)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReview2YTarget)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlignCamYTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStageOffsetMoveYSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStageOffsetMoveXSpeed)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label lblYAPos;
        private System.Windows.Forms.Label XAPos;
        private System.Windows.Forms.Label lblYtPos;
        private System.Windows.Forms.Label TargetXPos;
        private System.Windows.Forms.Label capPosX;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label StageYSpeed;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label StageXSpeed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numLaserXTarget;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numReview2XTarget;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.NumericUpDown numAlignCamXTarget;
        private System.Windows.Forms.Button btnLaserPosMove;
        private System.Windows.Forms.Button btnReview2PosMove;
        private System.Windows.Forms.Button btnAlignCam1PosMove;
        private System.Windows.Forms.Button btnMoveLaserPosStop;
        private System.Windows.Forms.Button btnReview2MoveStop;
        private System.Windows.Forms.Button btnAlignCam1MoveStop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label lbXPos;
        private System.Windows.Forms.Label lblXAPos;
        private System.Windows.Forms.Label lblXtPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numLaserYTarget;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numReview2YTarget;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numAlignCamYTarget;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown numStageOffsetMoveYSpeed;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown numStageOffsetMoveXSpeed;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btnOffsetSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbYPos;
    }
}
