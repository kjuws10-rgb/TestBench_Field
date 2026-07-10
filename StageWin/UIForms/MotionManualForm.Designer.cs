using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace StageWin.UI
{
    partial class MotionManualForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlX = new System.Windows.Forms.Panel();
            this.p2pCardX = new System.Windows.Forms.Panel();
            this.capP2PX = new System.Windows.Forms.Label();
            this.rowP2PX = new System.Windows.Forms.FlowLayoutPanel();
            this.lbXA = new System.Windows.Forms.Label();
            this.numXP2PA = new System.Windows.Forms.NumericUpDown();
            this.lbXB = new System.Windows.Forms.Label();
            this.numXP2PB = new System.Windows.Forms.NumericUpDown();
            this.lbXv = new System.Windows.Forms.Label();
            this.numXP2PVel = new System.Windows.Forms.NumericUpDown();
            this.lbXac = new System.Windows.Forms.Label();
            this.numXP2PAcc = new System.Windows.Forms.NumericUpDown();
            this.lbXd = new System.Windows.Forms.Label();
            this.numXP2PDec = new System.Windows.Forms.NumericUpDown();
            this.lbXdwell = new System.Windows.Forms.Label();
            this.numXP2PDwell = new System.Windows.Forms.NumericUpDown();
            this.lbXcnt = new System.Windows.Forms.Label();
            this.numXP2PCount = new System.Windows.Forms.NumericUpDown();
            this.btnXP2PStart = new System.Windows.Forms.Button();
            this.btnXP2PStop = new System.Windows.Forms.Button();
            this.lbXP2PStatus = new System.Windows.Forms.Label();
            this.jogCardX = new System.Windows.Forms.Panel();
            this.capJogX = new System.Windows.Forms.Label();
            this.rowJogX = new System.Windows.Forms.FlowLayoutPanel();
            this.lbXJog = new System.Windows.Forms.Label();
            this.numXJog = new System.Windows.Forms.NumericUpDown();
            this.btnXJogMinus = new System.Windows.Forms.Button();
            this.btnXJogPlus = new System.Windows.Forms.Button();
            this.moveCardX = new System.Windows.Forms.Panel();
            this.capMoveX = new System.Windows.Forms.Label();
            this.rowMoveX = new System.Windows.Forms.FlowLayoutPanel();
            this.lbXTarget = new System.Windows.Forms.Label();
            this.numXTarget = new System.Windows.Forms.NumericUpDown();
            this.rXAbs = new System.Windows.Forms.RadioButton();
            this.rXRel = new System.Windows.Forms.RadioButton();
            this.btnXMove = new System.Windows.Forms.Button();
            this.btnXStop = new System.Windows.Forms.Button();
            this.btnXHome = new System.Windows.Forms.Button();
            this.btnXServo = new System.Windows.Forms.Button();
            this.profileCardX = new System.Windows.Forms.Panel();
            this.capProfX = new System.Windows.Forms.Label();
            this.gridX = new System.Windows.Forms.TableLayoutPanel();
            this.lbXVel = new System.Windows.Forms.Label();
            this.numXSpeed = new System.Windows.Forms.NumericUpDown();
            this.lbXAcc = new System.Windows.Forms.Label();
            this.numXAcc = new System.Windows.Forms.NumericUpDown();
            this.lbXDec = new System.Windows.Forms.Label();
            this.numXDec = new System.Windows.Forms.NumericUpDown();
            this.posCardX = new System.Windows.Forms.Panel();
            this.lblXAPos = new System.Windows.Forms.Label();
            this.XAPos = new System.Windows.Forms.Label();
            this.lblXtPos = new System.Windows.Forms.Label();
            this.TargetXPos = new System.Windows.Forms.Label();
            this.capPosX = new System.Windows.Forms.Label();
            this.lbXPos = new System.Windows.Forms.Label();
            this.headerX = new System.Windows.Forms.TableLayoutPanel();
            this.lbXTitle = new System.Windows.Forms.Label();
            this.chipsX = new System.Windows.Forms.FlowLayoutPanel();
            this.chipXServo = new System.Windows.Forms.Label();
            this.chipXInpos = new System.Windows.Forms.Label();
            this.chipXMove = new System.Windows.Forms.Label();
            this.chipXHomeStatus = new System.Windows.Forms.Label();
            this.pnlY = new System.Windows.Forms.Panel();
            this.p2pCardY = new System.Windows.Forms.Panel();
            this.capP2PY = new System.Windows.Forms.Label();
            this.rowP2PY = new System.Windows.Forms.FlowLayoutPanel();
            this.lbYA = new System.Windows.Forms.Label();
            this.numYP2PA = new System.Windows.Forms.NumericUpDown();
            this.lbYB = new System.Windows.Forms.Label();
            this.numYP2PB = new System.Windows.Forms.NumericUpDown();
            this.lbYv = new System.Windows.Forms.Label();
            this.numYP2PVel = new System.Windows.Forms.NumericUpDown();
            this.lbYac = new System.Windows.Forms.Label();
            this.numYP2PAcc = new System.Windows.Forms.NumericUpDown();
            this.lbYd = new System.Windows.Forms.Label();
            this.numYP2PDec = new System.Windows.Forms.NumericUpDown();
            this.lbYdwell = new System.Windows.Forms.Label();
            this.numYP2PDwell = new System.Windows.Forms.NumericUpDown();
            this.lbYcnt = new System.Windows.Forms.Label();
            this.numYP2PCount = new System.Windows.Forms.NumericUpDown();
            this.btnYP2PStart = new System.Windows.Forms.Button();
            this.btnYP2PStop = new System.Windows.Forms.Button();
            this.lbYP2PStatus = new System.Windows.Forms.Label();
            this.jogCardY = new System.Windows.Forms.Panel();
            this.capJogY = new System.Windows.Forms.Label();
            this.rowJogY = new System.Windows.Forms.FlowLayoutPanel();
            this.lbYJog = new System.Windows.Forms.Label();
            this.numYJog = new System.Windows.Forms.NumericUpDown();
            this.btnYJogMinus = new System.Windows.Forms.Button();
            this.btnYJogPlus = new System.Windows.Forms.Button();
            this.moveCardY = new System.Windows.Forms.Panel();
            this.capMoveY = new System.Windows.Forms.Label();
            this.rowMoveY = new System.Windows.Forms.FlowLayoutPanel();
            this.lbYTarget = new System.Windows.Forms.Label();
            this.numYTarget = new System.Windows.Forms.NumericUpDown();
            this.rYAbs = new System.Windows.Forms.RadioButton();
            this.rYRel = new System.Windows.Forms.RadioButton();
            this.btnYMove = new System.Windows.Forms.Button();
            this.btnYStop = new System.Windows.Forms.Button();
            this.btnYHome = new System.Windows.Forms.Button();
            this.btnYServo = new System.Windows.Forms.Button();
            this.profileCardY = new System.Windows.Forms.Panel();
            this.capProfY = new System.Windows.Forms.Label();
            this.gridY = new System.Windows.Forms.TableLayoutPanel();
            this.lbYVel = new System.Windows.Forms.Label();
            this.numYSpeed = new System.Windows.Forms.NumericUpDown();
            this.lbYAcc = new System.Windows.Forms.Label();
            this.numYAcc = new System.Windows.Forms.NumericUpDown();
            this.lbYDec = new System.Windows.Forms.Label();
            this.numYDec = new System.Windows.Forms.NumericUpDown();
            this.posCardY = new System.Windows.Forms.Panel();
            this.lblYAPos = new System.Windows.Forms.Label();
            this.AYPos = new System.Windows.Forms.Label();
            this.lblYtPos = new System.Windows.Forms.Label();
            this.capPosY = new System.Windows.Forms.Label();
            this.TargetYPos = new System.Windows.Forms.Label();
            this.lbYPos = new System.Windows.Forms.Label();
            this.headerY = new System.Windows.Forms.TableLayoutPanel();
            this.lbYTitle = new System.Windows.Forms.Label();
            this.chipsY = new System.Windows.Forms.FlowLayoutPanel();
            this.chipYServo = new System.Windows.Forms.Label();
            this.chipYInpos = new System.Windows.Forms.Label();
            this.chipYMove = new System.Windows.Forms.Label();
            this.chipYHomeStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.numMZP2PA = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numMZP2PB = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numMZP2PVel = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numMZP2PAcc = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numMZP2PDec = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numMZP2PDwell = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numMZP2PCount = new System.Windows.Forms.NumericUpDown();
            this.btnMZP2PStart = new System.Windows.Forms.Button();
            this.btnMZP2PStop = new System.Windows.Forms.Button();
            this.lbMZP2PStatus = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.numMZJog = new System.Windows.Forms.NumericUpDown();
            this.btnMZJogMinus = new System.Windows.Forms.Button();
            this.btnMZJogPlus = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.numMZTarget = new System.Windows.Forms.NumericUpDown();
            this.rMZAbs = new System.Windows.Forms.RadioButton();
            this.rMZRel = new System.Windows.Forms.RadioButton();
            this.btnMZMove = new System.Windows.Forms.Button();
            this.btnMZStop = new System.Windows.Forms.Button();
            this.btnMZHome = new System.Windows.Forms.Button();
            this.btnMZServo = new System.Windows.Forms.Button();
            this.ZHomeParamButton = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.numMZSpeed = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.numMZAcc = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.numMZDec = new System.Windows.Forms.NumericUpDown();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.lbMZPos = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.chipMZServo = new System.Windows.Forms.Label();
            this.chipMZInpos = new System.Windows.Forms.Label();
            this.chipMZMove = new System.Windows.Forms.Label();
            this.chipMZHomeStatus = new System.Windows.Forms.Label();
            this.btnMZAlarmReset = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label24 = new System.Windows.Forms.Label();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label25 = new System.Windows.Forms.Label();
            this.numTP2PA = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.numTP2PB = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.numTP2PVel = new System.Windows.Forms.NumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.numTP2PAcc = new System.Windows.Forms.NumericUpDown();
            this.label29 = new System.Windows.Forms.Label();
            this.numTP2PDec = new System.Windows.Forms.NumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.numTP2PDwell = new System.Windows.Forms.NumericUpDown();
            this.label31 = new System.Windows.Forms.Label();
            this.numTP2PCount = new System.Windows.Forms.NumericUpDown();
            this.btnTP2PStart = new System.Windows.Forms.Button();
            this.btnTP2PStop = new System.Windows.Forms.Button();
            this.lbTP2PStatus = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label33 = new System.Windows.Forms.Label();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.label34 = new System.Windows.Forms.Label();
            this.numTJog = new System.Windows.Forms.NumericUpDown();
            this.btnTJogMinus = new System.Windows.Forms.Button();
            this.btnTJogPlus = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.label35 = new System.Windows.Forms.Label();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.label36 = new System.Windows.Forms.Label();
            this.numTTarget = new System.Windows.Forms.NumericUpDown();
            this.rTAbs = new System.Windows.Forms.RadioButton();
            this.rTRel = new System.Windows.Forms.RadioButton();
            this.btnTMove = new System.Windows.Forms.Button();
            this.btnTStop = new System.Windows.Forms.Button();
            this.btnTHome = new System.Windows.Forms.Button();
            this.btnTServo = new System.Windows.Forms.Button();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label37 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label38 = new System.Windows.Forms.Label();
            this.numTSpeed = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.numTAcc = new System.Windows.Forms.NumericUpDown();
            this.label40 = new System.Windows.Forms.Label();
            this.numTDec = new System.Windows.Forms.NumericUpDown();
            this.panel12 = new System.Windows.Forms.Panel();
            this.label41 = new System.Windows.Forms.Label();
            this.lbTPos = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label43 = new System.Windows.Forms.Label();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.chipTServo = new System.Windows.Forms.Label();
            this.chipTInpos = new System.Windows.Forms.Label();
            this.chipTMove = new System.Windows.Forms.Label();
            this.chipTHomeStatus = new System.Windows.Forms.Label();
            this.btnTAlarmReset = new System.Windows.Forms.Button();
            this.tlRoot.SuspendLayout();
            this.pnlX.SuspendLayout();
            this.p2pCardX.SuspendLayout();
            this.rowP2PX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PVel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PDec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PDwell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PCount)).BeginInit();
            this.jogCardX.SuspendLayout();
            this.rowJogX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXJog)).BeginInit();
            this.moveCardX.SuspendLayout();
            this.rowMoveX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXTarget)).BeginInit();
            this.profileCardX.SuspendLayout();
            this.gridX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXDec)).BeginInit();
            this.posCardX.SuspendLayout();
            this.headerX.SuspendLayout();
            this.chipsX.SuspendLayout();
            this.pnlY.SuspendLayout();
            this.p2pCardY.SuspendLayout();
            this.rowP2PY.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PVel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PDec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PDwell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PCount)).BeginInit();
            this.jogCardY.SuspendLayout();
            this.rowJogY.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYJog)).BeginInit();
            this.moveCardY.SuspendLayout();
            this.rowMoveY.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYTarget)).BeginInit();
            this.profileCardY.SuspendLayout();
            this.gridY.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYDec)).BeginInit();
            this.posCardY.SuspendLayout();
            this.headerY.SuspendLayout();
            this.chipsY.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PVel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PDec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PDwell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PCount)).BeginInit();
            this.panel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZJog)).BeginInit();
            this.panel4.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZTarget)).BeginInit();
            this.panel5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZDec)).BeginInit();
            this.panel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PVel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PDec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PDwell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PCount)).BeginInit();
            this.panel9.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTJog)).BeginInit();
            this.panel10.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTTarget)).BeginInit();
            this.panel11.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTDec)).BeginInit();
            this.panel12.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlRoot
            // 
            this.tlRoot.BackColor = System.Drawing.SystemColors.Control;
            this.tlRoot.ColumnCount = 2;
            this.tlRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlRoot.Controls.Add(this.pnlX, 0, 0);
            this.tlRoot.Controls.Add(this.pnlY, 1, 0);
            this.tlRoot.Controls.Add(this.panel1, 0, 1);
            this.tlRoot.Controls.Add(this.panel7, 1, 1);
            this.tlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlRoot.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.tlRoot.Location = new System.Drawing.Point(10, 10);
            this.tlRoot.Name = "tlRoot";
            this.tlRoot.Padding = new System.Windows.Forms.Padding(4);
            this.tlRoot.RowCount = 2;
            this.tlRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlRoot.Size = new System.Drawing.Size(1645, 841);
            this.tlRoot.TabIndex = 0;
            // 
            // pnlX
            // 
            this.pnlX.BackColor = System.Drawing.Color.Transparent;
            this.pnlX.Controls.Add(this.p2pCardX);
            this.pnlX.Controls.Add(this.jogCardX);
            this.pnlX.Controls.Add(this.moveCardX);
            this.pnlX.Controls.Add(this.profileCardX);
            this.pnlX.Controls.Add(this.posCardX);
            this.pnlX.Controls.Add(this.headerX);
            this.pnlX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.pnlX.Location = new System.Drawing.Point(7, 7);
            this.pnlX.Name = "pnlX";
            this.pnlX.Padding = new System.Windows.Forms.Padding(8);
            this.pnlX.Size = new System.Drawing.Size(812, 410);
            this.pnlX.TabIndex = 0;
            // 
            // p2pCardX
            // 
            this.p2pCardX.BackColor = System.Drawing.Color.White;
            this.p2pCardX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p2pCardX.Controls.Add(this.capP2PX);
            this.p2pCardX.Controls.Add(this.rowP2PX);
            this.p2pCardX.Dock = System.Windows.Forms.DockStyle.Top;
            this.p2pCardX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.p2pCardX.Location = new System.Drawing.Point(8, 310);
            this.p2pCardX.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.p2pCardX.Name = "p2pCardX";
            this.p2pCardX.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.p2pCardX.Size = new System.Drawing.Size(796, 95);
            this.p2pCardX.TabIndex = 0;
            // 
            // capP2PX
            // 
            this.capP2PX.AutoSize = true;
            this.capP2PX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capP2PX.Location = new System.Drawing.Point(12, 2);
            this.capP2PX.Name = "capP2PX";
            this.capP2PX.Size = new System.Drawing.Size(94, 15);
            this.capP2PX.TabIndex = 0;
            this.capP2PX.Text = "P2P Repeat (X)";
            // 
            // rowP2PX
            // 
            this.rowP2PX.AutoSize = true;
            this.rowP2PX.Controls.Add(this.lbXA);
            this.rowP2PX.Controls.Add(this.numXP2PA);
            this.rowP2PX.Controls.Add(this.lbXB);
            this.rowP2PX.Controls.Add(this.numXP2PB);
            this.rowP2PX.Controls.Add(this.lbXv);
            this.rowP2PX.Controls.Add(this.numXP2PVel);
            this.rowP2PX.Controls.Add(this.lbXac);
            this.rowP2PX.Controls.Add(this.numXP2PAcc);
            this.rowP2PX.Controls.Add(this.lbXd);
            this.rowP2PX.Controls.Add(this.numXP2PDec);
            this.rowP2PX.Controls.Add(this.lbXdwell);
            this.rowP2PX.Controls.Add(this.numXP2PDwell);
            this.rowP2PX.Controls.Add(this.lbXcnt);
            this.rowP2PX.Controls.Add(this.numXP2PCount);
            this.rowP2PX.Controls.Add(this.btnXP2PStart);
            this.rowP2PX.Controls.Add(this.btnXP2PStop);
            this.rowP2PX.Controls.Add(this.lbXP2PStatus);
            this.rowP2PX.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowP2PX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowP2PX.Location = new System.Drawing.Point(12, 10);
            this.rowP2PX.Name = "rowP2PX";
            this.rowP2PX.Size = new System.Drawing.Size(770, 58);
            this.rowP2PX.TabIndex = 1;
            // 
            // lbXA
            // 
            this.lbXA.AutoSize = true;
            this.lbXA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXA.Location = new System.Drawing.Point(0, 6);
            this.lbXA.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbXA.Name = "lbXA";
            this.lbXA.Size = new System.Drawing.Size(45, 15);
            this.lbXA.TabIndex = 0;
            this.lbXA.Text = "A(mm)";
            // 
            // numXP2PA
            // 
            this.numXP2PA.DecimalPlaces = 3;
            this.numXP2PA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PA.Location = new System.Drawing.Point(52, 3);
            this.numXP2PA.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numXP2PA.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numXP2PA.Name = "numXP2PA";
            this.numXP2PA.Size = new System.Drawing.Size(80, 23);
            this.numXP2PA.TabIndex = 1;
            // 
            // lbXB
            // 
            this.lbXB.AutoSize = true;
            this.lbXB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXB.Location = new System.Drawing.Point(143, 6);
            this.lbXB.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXB.Name = "lbXB";
            this.lbXB.Size = new System.Drawing.Size(45, 15);
            this.lbXB.TabIndex = 2;
            this.lbXB.Text = "B(mm)";
            // 
            // numXP2PB
            // 
            this.numXP2PB.DecimalPlaces = 3;
            this.numXP2PB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PB.Location = new System.Drawing.Point(195, 3);
            this.numXP2PB.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numXP2PB.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numXP2PB.Name = "numXP2PB";
            this.numXP2PB.Size = new System.Drawing.Size(80, 23);
            this.numXP2PB.TabIndex = 3;
            this.numXP2PB.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lbXv
            // 
            this.lbXv.AutoSize = true;
            this.lbXv.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXv.Location = new System.Drawing.Point(286, 6);
            this.lbXv.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXv.Name = "lbXv";
            this.lbXv.Size = new System.Drawing.Size(25, 15);
            this.lbXv.TabIndex = 4;
            this.lbXv.Text = "Vel";
            // 
            // numXP2PVel
            // 
            this.numXP2PVel.DecimalPlaces = 2;
            this.numXP2PVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PVel.Location = new System.Drawing.Point(318, 3);
            this.numXP2PVel.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numXP2PVel.Name = "numXP2PVel";
            this.numXP2PVel.Size = new System.Drawing.Size(68, 23);
            this.numXP2PVel.TabIndex = 5;
            this.numXP2PVel.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lbXac
            // 
            this.lbXac.AutoSize = true;
            this.lbXac.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXac.Location = new System.Drawing.Point(397, 6);
            this.lbXac.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXac.Name = "lbXac";
            this.lbXac.Size = new System.Drawing.Size(27, 15);
            this.lbXac.TabIndex = 6;
            this.lbXac.Text = "Acc";
            // 
            // numXP2PAcc
            // 
            this.numXP2PAcc.DecimalPlaces = 2;
            this.numXP2PAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PAcc.Location = new System.Drawing.Point(431, 3);
            this.numXP2PAcc.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numXP2PAcc.Name = "numXP2PAcc";
            this.numXP2PAcc.Size = new System.Drawing.Size(68, 23);
            this.numXP2PAcc.TabIndex = 7;
            this.numXP2PAcc.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbXd
            // 
            this.lbXd.AutoSize = true;
            this.lbXd.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXd.Location = new System.Drawing.Point(510, 6);
            this.lbXd.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXd.Name = "lbXd";
            this.lbXd.Size = new System.Drawing.Size(29, 15);
            this.lbXd.TabIndex = 8;
            this.lbXd.Text = "Dec";
            // 
            // numXP2PDec
            // 
            this.numXP2PDec.DecimalPlaces = 2;
            this.numXP2PDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PDec.Location = new System.Drawing.Point(546, 3);
            this.numXP2PDec.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numXP2PDec.Name = "numXP2PDec";
            this.numXP2PDec.Size = new System.Drawing.Size(68, 23);
            this.numXP2PDec.TabIndex = 9;
            this.numXP2PDec.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbXdwell
            // 
            this.lbXdwell.AutoSize = true;
            this.lbXdwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXdwell.Location = new System.Drawing.Point(625, 6);
            this.lbXdwell.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXdwell.Name = "lbXdwell";
            this.lbXdwell.Size = new System.Drawing.Size(64, 15);
            this.lbXdwell.TabIndex = 10;
            this.lbXdwell.Text = "Dwell(ms)";
            // 
            // numXP2PDwell
            // 
            this.numXP2PDwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PDwell.Location = new System.Drawing.Point(696, 3);
            this.numXP2PDwell.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numXP2PDwell.Name = "numXP2PDwell";
            this.numXP2PDwell.Size = new System.Drawing.Size(68, 23);
            this.numXP2PDwell.TabIndex = 11;
            this.numXP2PDwell.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // lbXcnt
            // 
            this.lbXcnt.AutoSize = true;
            this.lbXcnt.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXcnt.Location = new System.Drawing.Point(8, 35);
            this.lbXcnt.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbXcnt.Name = "lbXcnt";
            this.lbXcnt.Size = new System.Drawing.Size(41, 15);
            this.lbXcnt.TabIndex = 12;
            this.lbXcnt.Text = "Count";
            // 
            // numXP2PCount
            // 
            this.numXP2PCount.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXP2PCount.Location = new System.Drawing.Point(56, 32);
            this.numXP2PCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numXP2PCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numXP2PCount.Name = "numXP2PCount";
            this.numXP2PCount.Size = new System.Drawing.Size(68, 23);
            this.numXP2PCount.TabIndex = 13;
            this.numXP2PCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnXP2PStart
            // 
            this.btnXP2PStart.AutoSize = true;
            this.btnXP2PStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXP2PStart.Location = new System.Drawing.Point(137, 33);
            this.btnXP2PStart.Margin = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.btnXP2PStart.Name = "btnXP2PStart";
            this.btnXP2PStart.Size = new System.Drawing.Size(65, 25);
            this.btnXP2PStart.TabIndex = 14;
            this.btnXP2PStart.Text = "Start";
            // 
            // btnXP2PStop
            // 
            this.btnXP2PStop.AutoSize = true;
            this.btnXP2PStop.Enabled = false;
            this.btnXP2PStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXP2PStop.Location = new System.Drawing.Point(208, 33);
            this.btnXP2PStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnXP2PStop.Name = "btnXP2PStop";
            this.btnXP2PStop.Size = new System.Drawing.Size(65, 25);
            this.btnXP2PStop.TabIndex = 15;
            this.btnXP2PStop.Text = "Stop";
            // 
            // lbXP2PStatus
            // 
            this.lbXP2PStatus.AutoSize = true;
            this.lbXP2PStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXP2PStatus.Location = new System.Drawing.Point(285, 37);
            this.lbXP2PStatus.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.lbXP2PStatus.Name = "lbXP2PStatus";
            this.lbXP2PStatus.Size = new System.Drawing.Size(29, 15);
            this.lbXP2PStatus.TabIndex = 16;
            this.lbXP2PStatus.Text = "Idle";
            // 
            // jogCardX
            // 
            this.jogCardX.BackColor = System.Drawing.Color.White;
            this.jogCardX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.jogCardX.Controls.Add(this.capJogX);
            this.jogCardX.Controls.Add(this.rowJogX);
            this.jogCardX.Dock = System.Windows.Forms.DockStyle.Top;
            this.jogCardX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.jogCardX.Location = new System.Drawing.Point(8, 247);
            this.jogCardX.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.jogCardX.Name = "jogCardX";
            this.jogCardX.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.jogCardX.Size = new System.Drawing.Size(796, 63);
            this.jogCardX.TabIndex = 0;
            // 
            // capJogX
            // 
            this.capJogX.AutoSize = true;
            this.capJogX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capJogX.Location = new System.Drawing.Point(12, 2);
            this.capJogX.Name = "capJogX";
            this.capJogX.Size = new System.Drawing.Size(27, 15);
            this.capJogX.TabIndex = 0;
            this.capJogX.Text = "Jog";
            // 
            // rowJogX
            // 
            this.rowJogX.AutoSize = true;
            this.rowJogX.Controls.Add(this.lbXJog);
            this.rowJogX.Controls.Add(this.numXJog);
            this.rowJogX.Controls.Add(this.btnXJogMinus);
            this.rowJogX.Controls.Add(this.btnXJogPlus);
            this.rowJogX.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowJogX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowJogX.Location = new System.Drawing.Point(12, 10);
            this.rowJogX.Name = "rowJogX";
            this.rowJogX.Size = new System.Drawing.Size(770, 29);
            this.rowJogX.TabIndex = 1;
            // 
            // lbXJog
            // 
            this.lbXJog.AutoSize = true;
            this.lbXJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXJog.Location = new System.Drawing.Point(0, 6);
            this.lbXJog.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbXJog.Name = "lbXJog";
            this.lbXJog.Size = new System.Drawing.Size(44, 15);
            this.lbXJog.TabIndex = 0;
            this.lbXJog.Text = "Speed";
            // 
            // numXJog
            // 
            this.numXJog.DecimalPlaces = 2;
            this.numXJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXJog.Location = new System.Drawing.Point(51, 3);
            this.numXJog.Name = "numXJog";
            this.numXJog.Size = new System.Drawing.Size(90, 23);
            this.numXJog.TabIndex = 1;
            this.numXJog.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // btnXJogMinus
            // 
            this.btnXJogMinus.AutoSize = true;
            this.btnXJogMinus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXJogMinus.Location = new System.Drawing.Point(152, 4);
            this.btnXJogMinus.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnXJogMinus.Name = "btnXJogMinus";
            this.btnXJogMinus.Size = new System.Drawing.Size(75, 25);
            this.btnXJogMinus.TabIndex = 2;
            this.btnXJogMinus.Text = "◀ JOG-";
            // 
            // btnXJogPlus
            // 
            this.btnXJogPlus.AutoSize = true;
            this.btnXJogPlus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXJogPlus.Location = new System.Drawing.Point(233, 4);
            this.btnXJogPlus.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnXJogPlus.Name = "btnXJogPlus";
            this.btnXJogPlus.Size = new System.Drawing.Size(75, 25);
            this.btnXJogPlus.TabIndex = 3;
            this.btnXJogPlus.Text = "JOG+ ▶";
            // 
            // moveCardX
            // 
            this.moveCardX.BackColor = System.Drawing.Color.White;
            this.moveCardX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moveCardX.Controls.Add(this.capMoveX);
            this.moveCardX.Controls.Add(this.rowMoveX);
            this.moveCardX.Dock = System.Windows.Forms.DockStyle.Top;
            this.moveCardX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.moveCardX.Location = new System.Drawing.Point(8, 164);
            this.moveCardX.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.moveCardX.Name = "moveCardX";
            this.moveCardX.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.moveCardX.Size = new System.Drawing.Size(796, 83);
            this.moveCardX.TabIndex = 1;
            // 
            // capMoveX
            // 
            this.capMoveX.AutoSize = true;
            this.capMoveX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capMoveX.Location = new System.Drawing.Point(12, 2);
            this.capMoveX.Name = "capMoveX";
            this.capMoveX.Size = new System.Drawing.Size(39, 15);
            this.capMoveX.TabIndex = 0;
            this.capMoveX.Text = "Move";
            // 
            // rowMoveX
            // 
            this.rowMoveX.AutoSize = true;
            this.rowMoveX.Controls.Add(this.lbXTarget);
            this.rowMoveX.Controls.Add(this.numXTarget);
            this.rowMoveX.Controls.Add(this.rXAbs);
            this.rowMoveX.Controls.Add(this.rXRel);
            this.rowMoveX.Controls.Add(this.btnXMove);
            this.rowMoveX.Controls.Add(this.btnXStop);
            this.rowMoveX.Controls.Add(this.btnXHome);
            this.rowMoveX.Controls.Add(this.btnXServo);
            this.rowMoveX.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowMoveX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowMoveX.Location = new System.Drawing.Point(12, 10);
            this.rowMoveX.Name = "rowMoveX";
            this.rowMoveX.Size = new System.Drawing.Size(770, 29);
            this.rowMoveX.TabIndex = 1;
            // 
            // lbXTarget
            // 
            this.lbXTarget.AutoSize = true;
            this.lbXTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXTarget.Location = new System.Drawing.Point(0, 6);
            this.lbXTarget.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbXTarget.Name = "lbXTarget";
            this.lbXTarget.Size = new System.Drawing.Size(46, 15);
            this.lbXTarget.TabIndex = 0;
            this.lbXTarget.Text = "Target";
            // 
            // numXTarget
            // 
            this.numXTarget.DecimalPlaces = 3;
            this.numXTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXTarget.Location = new System.Drawing.Point(53, 3);
            this.numXTarget.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numXTarget.Minimum = new decimal(new int[] {
            960,
            0,
            0,
            -2147483648});
            this.numXTarget.Name = "numXTarget";
            this.numXTarget.Size = new System.Drawing.Size(100, 23);
            this.numXTarget.TabIndex = 1;
            // 
            // rXAbs
            // 
            this.rXAbs.AutoSize = true;
            this.rXAbs.Checked = true;
            this.rXAbs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rXAbs.Location = new System.Drawing.Point(164, 6);
            this.rXAbs.Margin = new System.Windows.Forms.Padding(8, 6, 6, 0);
            this.rXAbs.Name = "rXAbs";
            this.rXAbs.Size = new System.Drawing.Size(48, 19);
            this.rXAbs.TabIndex = 2;
            this.rXAbs.TabStop = true;
            this.rXAbs.Text = "ABS";
            // 
            // rXRel
            // 
            this.rXRel.AutoSize = true;
            this.rXRel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rXRel.Location = new System.Drawing.Point(218, 6);
            this.rXRel.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.rXRel.Name = "rXRel";
            this.rXRel.Size = new System.Drawing.Size(45, 19);
            this.rXRel.TabIndex = 3;
            this.rXRel.Text = "REL";
            // 
            // btnXMove
            // 
            this.btnXMove.AutoSize = true;
            this.btnXMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXMove.Location = new System.Drawing.Point(277, 4);
            this.btnXMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnXMove.Name = "btnXMove";
            this.btnXMove.Size = new System.Drawing.Size(75, 25);
            this.btnXMove.TabIndex = 4;
            this.btnXMove.Text = "Move";
            // 
            // btnXStop
            // 
            this.btnXStop.AutoSize = true;
            this.btnXStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXStop.Location = new System.Drawing.Point(358, 4);
            this.btnXStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnXStop.Name = "btnXStop";
            this.btnXStop.Size = new System.Drawing.Size(75, 25);
            this.btnXStop.TabIndex = 5;
            this.btnXStop.Text = "Stop";
            // 
            // btnXHome
            // 
            this.btnXHome.AutoSize = true;
            this.btnXHome.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXHome.Location = new System.Drawing.Point(439, 4);
            this.btnXHome.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnXHome.Name = "btnXHome";
            this.btnXHome.Size = new System.Drawing.Size(75, 25);
            this.btnXHome.TabIndex = 6;
            this.btnXHome.Text = "Home";
            // 
            // btnXServo
            // 
            this.btnXServo.AutoSize = true;
            this.btnXServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnXServo.Location = new System.Drawing.Point(520, 4);
            this.btnXServo.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnXServo.Name = "btnXServo";
            this.btnXServo.Size = new System.Drawing.Size(98, 25);
            this.btnXServo.TabIndex = 7;
            this.btnXServo.Text = "Servo ON/OFF";
            // 
            // profileCardX
            // 
            this.profileCardX.BackColor = System.Drawing.Color.White;
            this.profileCardX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.profileCardX.Controls.Add(this.capProfX);
            this.profileCardX.Controls.Add(this.gridX);
            this.profileCardX.Dock = System.Windows.Forms.DockStyle.Top;
            this.profileCardX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.profileCardX.Location = new System.Drawing.Point(8, 105);
            this.profileCardX.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.profileCardX.Name = "profileCardX";
            this.profileCardX.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.profileCardX.Size = new System.Drawing.Size(796, 59);
            this.profileCardX.TabIndex = 2;
            // 
            // capProfX
            // 
            this.capProfX.AutoSize = true;
            this.capProfX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capProfX.Location = new System.Drawing.Point(12, 1);
            this.capProfX.Name = "capProfX";
            this.capProfX.Size = new System.Drawing.Size(125, 15);
            this.capProfX.TabIndex = 0;
            this.capProfX.Text = "Profile (Vel/Acc/Dec)";
            // 
            // gridX
            // 
            this.gridX.AutoSize = true;
            this.gridX.ColumnCount = 6;
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridX.Controls.Add(this.lbXVel, 0, 0);
            this.gridX.Controls.Add(this.numXSpeed, 1, 0);
            this.gridX.Controls.Add(this.lbXAcc, 2, 0);
            this.gridX.Controls.Add(this.numXAcc, 3, 0);
            this.gridX.Controls.Add(this.lbXDec, 4, 0);
            this.gridX.Controls.Add(this.numXDec, 5, 0);
            this.gridX.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.gridX.Location = new System.Drawing.Point(12, 10);
            this.gridX.Name = "gridX";
            this.gridX.RowCount = 1;
            this.gridX.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gridX.Size = new System.Drawing.Size(770, 29);
            this.gridX.TabIndex = 1;
            // 
            // lbXVel
            // 
            this.lbXVel.AutoSize = true;
            this.lbXVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXVel.Location = new System.Drawing.Point(0, 6);
            this.lbXVel.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbXVel.Name = "lbXVel";
            this.lbXVel.Size = new System.Drawing.Size(25, 15);
            this.lbXVel.TabIndex = 0;
            this.lbXVel.Text = "Vel";
            // 
            // numXSpeed
            // 
            this.numXSpeed.DecimalPlaces = 2;
            this.numXSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXSpeed.Location = new System.Drawing.Point(32, 3);
            this.numXSpeed.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numXSpeed.Name = "numXSpeed";
            this.numXSpeed.Size = new System.Drawing.Size(90, 23);
            this.numXSpeed.TabIndex = 1;
            this.numXSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lbXAcc
            // 
            this.lbXAcc.AutoSize = true;
            this.lbXAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXAcc.Location = new System.Drawing.Point(258, 6);
            this.lbXAcc.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.lbXAcc.Name = "lbXAcc";
            this.lbXAcc.Size = new System.Drawing.Size(27, 15);
            this.lbXAcc.TabIndex = 2;
            this.lbXAcc.Text = "Acc";
            // 
            // numXAcc
            // 
            this.numXAcc.DecimalPlaces = 2;
            this.numXAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXAcc.Location = new System.Drawing.Point(292, 3);
            this.numXAcc.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numXAcc.Name = "numXAcc";
            this.numXAcc.Size = new System.Drawing.Size(90, 23);
            this.numXAcc.TabIndex = 3;
            this.numXAcc.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbXDec
            // 
            this.lbXDec.AutoSize = true;
            this.lbXDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXDec.Location = new System.Drawing.Point(518, 6);
            this.lbXDec.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.lbXDec.Name = "lbXDec";
            this.lbXDec.Size = new System.Drawing.Size(29, 15);
            this.lbXDec.TabIndex = 4;
            this.lbXDec.Text = "Dec";
            // 
            // numXDec
            // 
            this.numXDec.DecimalPlaces = 2;
            this.numXDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numXDec.Location = new System.Drawing.Point(554, 3);
            this.numXDec.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numXDec.Name = "numXDec";
            this.numXDec.Size = new System.Drawing.Size(90, 23);
            this.numXDec.TabIndex = 5;
            this.numXDec.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // posCardX
            // 
            this.posCardX.BackColor = System.Drawing.Color.White;
            this.posCardX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.posCardX.Controls.Add(this.lblXAPos);
            this.posCardX.Controls.Add(this.XAPos);
            this.posCardX.Controls.Add(this.lblXtPos);
            this.posCardX.Controls.Add(this.TargetXPos);
            this.posCardX.Controls.Add(this.capPosX);
            this.posCardX.Controls.Add(this.lbXPos);
            this.posCardX.Dock = System.Windows.Forms.DockStyle.Top;
            this.posCardX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.posCardX.Location = new System.Drawing.Point(8, 44);
            this.posCardX.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.posCardX.Name = "posCardX";
            this.posCardX.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.posCardX.Size = new System.Drawing.Size(796, 61);
            this.posCardX.TabIndex = 3;
            // 
            // lblXAPos
            // 
            this.lblXAPos.AutoSize = true;
            this.lblXAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblXAPos.Location = new System.Drawing.Point(315, 24);
            this.lblXAPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblXAPos.Name = "lblXAPos";
            this.lblXAPos.Size = new System.Drawing.Size(64, 15);
            this.lblXAPos.TabIndex = 5;
            this.lblXAPos.Text = "0.000 mm";
            // 
            // XAPos
            // 
            this.XAPos.AutoSize = true;
            this.XAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.XAPos.Location = new System.Drawing.Point(315, 6);
            this.XAPos.Name = "XAPos";
            this.XAPos.Size = new System.Drawing.Size(146, 15);
            this.XAPos.TabIndex = 4;
            this.XAPos.Text = "Encoder 보정 전 Position";
            // 
            // lblXtPos
            // 
            this.lblXtPos.AutoSize = true;
            this.lblXtPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblXtPos.Location = new System.Drawing.Point(155, 24);
            this.lblXtPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblXtPos.Name = "lblXtPos";
            this.lblXtPos.Size = new System.Drawing.Size(64, 15);
            this.lblXtPos.TabIndex = 3;
            this.lblXtPos.Text = "0.000 mm";
            // 
            // TargetXPos
            // 
            this.TargetXPos.AutoSize = true;
            this.TargetXPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.TargetXPos.Location = new System.Drawing.Point(155, 6);
            this.TargetXPos.Name = "TargetXPos";
            this.TargetXPos.Size = new System.Drawing.Size(95, 15);
            this.TargetXPos.TabIndex = 2;
            this.TargetXPos.Text = "Target Position";
            // 
            // capPosX
            // 
            this.capPosX.AutoSize = true;
            this.capPosX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capPosX.Location = new System.Drawing.Point(12, 6);
            this.capPosX.Name = "capPosX";
            this.capPosX.Size = new System.Drawing.Size(62, 15);
            this.capPosX.TabIndex = 0;
            this.capPosX.Text = "F Position";
            // 
            // lbXPos
            // 
            this.lbXPos.AutoSize = true;
            this.lbXPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXPos.Location = new System.Drawing.Point(12, 24);
            this.lbXPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbXPos.Name = "lbXPos";
            this.lbXPos.Size = new System.Drawing.Size(64, 15);
            this.lbXPos.TabIndex = 1;
            this.lbXPos.Text = "0.000 mm";
            // 
            // headerX
            // 
            this.headerX.ColumnCount = 2;
            this.headerX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.headerX.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.headerX.Controls.Add(this.lbXTitle, 0, 0);
            this.headerX.Controls.Add(this.chipsX, 1, 0);
            this.headerX.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.headerX.Location = new System.Drawing.Point(8, 8);
            this.headerX.Name = "headerX";
            this.headerX.RowCount = 1;
            this.headerX.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.headerX.Size = new System.Drawing.Size(796, 36);
            this.headerX.TabIndex = 4;
            // 
            // lbXTitle
            // 
            this.lbXTitle.AutoSize = true;
            this.lbXTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbXTitle.Location = new System.Drawing.Point(0, 8);
            this.lbXTitle.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lbXTitle.Name = "lbXTitle";
            this.lbXTitle.Size = new System.Drawing.Size(68, 15);
            this.lbXTitle.TabIndex = 0;
            this.lbXTitle.Text = "Review (X)";
            // 
            // chipsX
            // 
            this.chipsX.Controls.Add(this.chipXServo);
            this.chipsX.Controls.Add(this.chipXInpos);
            this.chipsX.Controls.Add(this.chipXMove);
            this.chipsX.Controls.Add(this.chipXHomeStatus);
            this.chipsX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chipsX.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.chipsX.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipsX.Location = new System.Drawing.Point(401, 3);
            this.chipsX.Name = "chipsX";
            this.chipsX.Size = new System.Drawing.Size(392, 30);
            this.chipsX.TabIndex = 1;
            this.chipsX.WrapContents = false;
            // 
            // chipXServo
            // 
            this.chipXServo.AutoSize = true;
            this.chipXServo.BackColor = System.Drawing.Color.Gainsboro;
            this.chipXServo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipXServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipXServo.Location = new System.Drawing.Point(325, 4);
            this.chipXServo.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipXServo.Name = "chipXServo";
            this.chipXServo.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipXServo.Size = new System.Drawing.Size(67, 25);
            this.chipXServo.TabIndex = 0;
            this.chipXServo.Text = "SERVO";
            // 
            // chipXInpos
            // 
            this.chipXInpos.AutoSize = true;
            this.chipXInpos.BackColor = System.Drawing.Color.Gainsboro;
            this.chipXInpos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipXInpos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipXInpos.Location = new System.Drawing.Point(250, 4);
            this.chipXInpos.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipXInpos.Name = "chipXInpos";
            this.chipXInpos.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipXInpos.Size = new System.Drawing.Size(71, 25);
            this.chipXInpos.TabIndex = 1;
            this.chipXInpos.Text = "IN-POS";
            // 
            // chipXMove
            // 
            this.chipXMove.AutoSize = true;
            this.chipXMove.BackColor = System.Drawing.Color.Gainsboro;
            this.chipXMove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipXMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipXMove.Location = new System.Drawing.Point(165, 4);
            this.chipXMove.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipXMove.Name = "chipXMove";
            this.chipXMove.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipXMove.Size = new System.Drawing.Size(81, 25);
            this.chipXMove.TabIndex = 2;
            this.chipXMove.Text = "MOVING";
            // 
            // chipXHomeStatus
            // 
            this.chipXHomeStatus.AutoSize = true;
            this.chipXHomeStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.chipXHomeStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipXHomeStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipXHomeStatus.Location = new System.Drawing.Point(87, 4);
            this.chipXHomeStatus.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipXHomeStatus.Name = "chipXHomeStatus";
            this.chipXHomeStatus.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipXHomeStatus.Size = new System.Drawing.Size(74, 25);
            this.chipXHomeStatus.TabIndex = 3;
            this.chipXHomeStatus.Text = "HOMED";
            // 
            // pnlY
            // 
            this.pnlY.BackColor = System.Drawing.Color.Transparent;
            this.pnlY.Controls.Add(this.p2pCardY);
            this.pnlY.Controls.Add(this.jogCardY);
            this.pnlY.Controls.Add(this.moveCardY);
            this.pnlY.Controls.Add(this.profileCardY);
            this.pnlY.Controls.Add(this.posCardY);
            this.pnlY.Controls.Add(this.headerY);
            this.pnlY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.pnlY.Location = new System.Drawing.Point(825, 7);
            this.pnlY.Name = "pnlY";
            this.pnlY.Padding = new System.Windows.Forms.Padding(8);
            this.pnlY.Size = new System.Drawing.Size(813, 410);
            this.pnlY.TabIndex = 1;
            // 
            // p2pCardY
            // 
            this.p2pCardY.BackColor = System.Drawing.Color.White;
            this.p2pCardY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p2pCardY.Controls.Add(this.capP2PY);
            this.p2pCardY.Controls.Add(this.rowP2PY);
            this.p2pCardY.Dock = System.Windows.Forms.DockStyle.Top;
            this.p2pCardY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.p2pCardY.Location = new System.Drawing.Point(8, 310);
            this.p2pCardY.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.p2pCardY.Name = "p2pCardY";
            this.p2pCardY.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.p2pCardY.Size = new System.Drawing.Size(797, 95);
            this.p2pCardY.TabIndex = 0;
            // 
            // capP2PY
            // 
            this.capP2PY.AutoSize = true;
            this.capP2PY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capP2PY.Location = new System.Drawing.Point(12, 2);
            this.capP2PY.Name = "capP2PY";
            this.capP2PY.Size = new System.Drawing.Size(93, 15);
            this.capP2PY.TabIndex = 0;
            this.capP2PY.Text = "P2P Repeat (Y)";
            // 
            // rowP2PY
            // 
            this.rowP2PY.AutoSize = true;
            this.rowP2PY.Controls.Add(this.lbYA);
            this.rowP2PY.Controls.Add(this.numYP2PA);
            this.rowP2PY.Controls.Add(this.lbYB);
            this.rowP2PY.Controls.Add(this.numYP2PB);
            this.rowP2PY.Controls.Add(this.lbYv);
            this.rowP2PY.Controls.Add(this.numYP2PVel);
            this.rowP2PY.Controls.Add(this.lbYac);
            this.rowP2PY.Controls.Add(this.numYP2PAcc);
            this.rowP2PY.Controls.Add(this.lbYd);
            this.rowP2PY.Controls.Add(this.numYP2PDec);
            this.rowP2PY.Controls.Add(this.lbYdwell);
            this.rowP2PY.Controls.Add(this.numYP2PDwell);
            this.rowP2PY.Controls.Add(this.lbYcnt);
            this.rowP2PY.Controls.Add(this.numYP2PCount);
            this.rowP2PY.Controls.Add(this.btnYP2PStart);
            this.rowP2PY.Controls.Add(this.btnYP2PStop);
            this.rowP2PY.Controls.Add(this.lbYP2PStatus);
            this.rowP2PY.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowP2PY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowP2PY.Location = new System.Drawing.Point(12, 10);
            this.rowP2PY.Name = "rowP2PY";
            this.rowP2PY.Size = new System.Drawing.Size(771, 58);
            this.rowP2PY.TabIndex = 1;
            // 
            // lbYA
            // 
            this.lbYA.AutoSize = true;
            this.lbYA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYA.Location = new System.Drawing.Point(0, 6);
            this.lbYA.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbYA.Name = "lbYA";
            this.lbYA.Size = new System.Drawing.Size(45, 15);
            this.lbYA.TabIndex = 0;
            this.lbYA.Text = "A(mm)";
            // 
            // numYP2PA
            // 
            this.numYP2PA.DecimalPlaces = 3;
            this.numYP2PA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PA.Location = new System.Drawing.Point(52, 3);
            this.numYP2PA.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numYP2PA.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numYP2PA.Name = "numYP2PA";
            this.numYP2PA.Size = new System.Drawing.Size(80, 23);
            this.numYP2PA.TabIndex = 1;
            // 
            // lbYB
            // 
            this.lbYB.AutoSize = true;
            this.lbYB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYB.Location = new System.Drawing.Point(143, 6);
            this.lbYB.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYB.Name = "lbYB";
            this.lbYB.Size = new System.Drawing.Size(45, 15);
            this.lbYB.TabIndex = 2;
            this.lbYB.Text = "B(mm)";
            // 
            // numYP2PB
            // 
            this.numYP2PB.DecimalPlaces = 3;
            this.numYP2PB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PB.Location = new System.Drawing.Point(195, 3);
            this.numYP2PB.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numYP2PB.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numYP2PB.Name = "numYP2PB";
            this.numYP2PB.Size = new System.Drawing.Size(80, 23);
            this.numYP2PB.TabIndex = 3;
            this.numYP2PB.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lbYv
            // 
            this.lbYv.AutoSize = true;
            this.lbYv.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYv.Location = new System.Drawing.Point(286, 6);
            this.lbYv.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYv.Name = "lbYv";
            this.lbYv.Size = new System.Drawing.Size(25, 15);
            this.lbYv.TabIndex = 4;
            this.lbYv.Text = "Vel";
            // 
            // numYP2PVel
            // 
            this.numYP2PVel.DecimalPlaces = 2;
            this.numYP2PVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PVel.Location = new System.Drawing.Point(318, 3);
            this.numYP2PVel.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numYP2PVel.Name = "numYP2PVel";
            this.numYP2PVel.Size = new System.Drawing.Size(68, 23);
            this.numYP2PVel.TabIndex = 5;
            this.numYP2PVel.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lbYac
            // 
            this.lbYac.AutoSize = true;
            this.lbYac.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYac.Location = new System.Drawing.Point(397, 6);
            this.lbYac.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYac.Name = "lbYac";
            this.lbYac.Size = new System.Drawing.Size(27, 15);
            this.lbYac.TabIndex = 6;
            this.lbYac.Text = "Acc";
            // 
            // numYP2PAcc
            // 
            this.numYP2PAcc.DecimalPlaces = 2;
            this.numYP2PAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PAcc.Location = new System.Drawing.Point(431, 3);
            this.numYP2PAcc.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numYP2PAcc.Name = "numYP2PAcc";
            this.numYP2PAcc.Size = new System.Drawing.Size(68, 23);
            this.numYP2PAcc.TabIndex = 7;
            this.numYP2PAcc.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbYd
            // 
            this.lbYd.AutoSize = true;
            this.lbYd.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYd.Location = new System.Drawing.Point(510, 6);
            this.lbYd.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYd.Name = "lbYd";
            this.lbYd.Size = new System.Drawing.Size(29, 15);
            this.lbYd.TabIndex = 8;
            this.lbYd.Text = "Dec";
            // 
            // numYP2PDec
            // 
            this.numYP2PDec.DecimalPlaces = 2;
            this.numYP2PDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PDec.Location = new System.Drawing.Point(546, 3);
            this.numYP2PDec.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numYP2PDec.Name = "numYP2PDec";
            this.numYP2PDec.Size = new System.Drawing.Size(68, 23);
            this.numYP2PDec.TabIndex = 9;
            this.numYP2PDec.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbYdwell
            // 
            this.lbYdwell.AutoSize = true;
            this.lbYdwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYdwell.Location = new System.Drawing.Point(625, 6);
            this.lbYdwell.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYdwell.Name = "lbYdwell";
            this.lbYdwell.Size = new System.Drawing.Size(64, 15);
            this.lbYdwell.TabIndex = 10;
            this.lbYdwell.Text = "Dwell(ms)";
            // 
            // numYP2PDwell
            // 
            this.numYP2PDwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PDwell.Location = new System.Drawing.Point(696, 3);
            this.numYP2PDwell.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numYP2PDwell.Name = "numYP2PDwell";
            this.numYP2PDwell.Size = new System.Drawing.Size(68, 23);
            this.numYP2PDwell.TabIndex = 11;
            this.numYP2PDwell.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // lbYcnt
            // 
            this.lbYcnt.AutoSize = true;
            this.lbYcnt.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYcnt.Location = new System.Drawing.Point(8, 35);
            this.lbYcnt.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.lbYcnt.Name = "lbYcnt";
            this.lbYcnt.Size = new System.Drawing.Size(41, 15);
            this.lbYcnt.TabIndex = 12;
            this.lbYcnt.Text = "Count";
            // 
            // numYP2PCount
            // 
            this.numYP2PCount.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYP2PCount.Location = new System.Drawing.Point(56, 32);
            this.numYP2PCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numYP2PCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numYP2PCount.Name = "numYP2PCount";
            this.numYP2PCount.Size = new System.Drawing.Size(68, 23);
            this.numYP2PCount.TabIndex = 13;
            this.numYP2PCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnYP2PStart
            // 
            this.btnYP2PStart.AutoSize = true;
            this.btnYP2PStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYP2PStart.Location = new System.Drawing.Point(137, 33);
            this.btnYP2PStart.Margin = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.btnYP2PStart.Name = "btnYP2PStart";
            this.btnYP2PStart.Size = new System.Drawing.Size(65, 25);
            this.btnYP2PStart.TabIndex = 14;
            this.btnYP2PStart.Text = "Start";
            // 
            // btnYP2PStop
            // 
            this.btnYP2PStop.AutoSize = true;
            this.btnYP2PStop.Enabled = false;
            this.btnYP2PStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYP2PStop.Location = new System.Drawing.Point(208, 33);
            this.btnYP2PStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnYP2PStop.Name = "btnYP2PStop";
            this.btnYP2PStop.Size = new System.Drawing.Size(65, 25);
            this.btnYP2PStop.TabIndex = 15;
            this.btnYP2PStop.Text = "Stop";
            // 
            // lbYP2PStatus
            // 
            this.lbYP2PStatus.AutoSize = true;
            this.lbYP2PStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYP2PStatus.Location = new System.Drawing.Point(285, 37);
            this.lbYP2PStatus.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.lbYP2PStatus.Name = "lbYP2PStatus";
            this.lbYP2PStatus.Size = new System.Drawing.Size(29, 15);
            this.lbYP2PStatus.TabIndex = 16;
            this.lbYP2PStatus.Text = "Idle";
            // 
            // jogCardY
            // 
            this.jogCardY.BackColor = System.Drawing.Color.White;
            this.jogCardY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.jogCardY.Controls.Add(this.capJogY);
            this.jogCardY.Controls.Add(this.rowJogY);
            this.jogCardY.Dock = System.Windows.Forms.DockStyle.Top;
            this.jogCardY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.jogCardY.Location = new System.Drawing.Point(8, 247);
            this.jogCardY.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.jogCardY.Name = "jogCardY";
            this.jogCardY.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.jogCardY.Size = new System.Drawing.Size(797, 63);
            this.jogCardY.TabIndex = 0;
            // 
            // capJogY
            // 
            this.capJogY.AutoSize = true;
            this.capJogY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capJogY.Location = new System.Drawing.Point(12, 2);
            this.capJogY.Name = "capJogY";
            this.capJogY.Size = new System.Drawing.Size(27, 15);
            this.capJogY.TabIndex = 0;
            this.capJogY.Text = "Jog";
            // 
            // rowJogY
            // 
            this.rowJogY.AutoSize = true;
            this.rowJogY.Controls.Add(this.lbYJog);
            this.rowJogY.Controls.Add(this.numYJog);
            this.rowJogY.Controls.Add(this.btnYJogMinus);
            this.rowJogY.Controls.Add(this.btnYJogPlus);
            this.rowJogY.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowJogY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowJogY.Location = new System.Drawing.Point(12, 10);
            this.rowJogY.Name = "rowJogY";
            this.rowJogY.Size = new System.Drawing.Size(771, 29);
            this.rowJogY.TabIndex = 1;
            // 
            // lbYJog
            // 
            this.lbYJog.AutoSize = true;
            this.lbYJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYJog.Location = new System.Drawing.Point(0, 6);
            this.lbYJog.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbYJog.Name = "lbYJog";
            this.lbYJog.Size = new System.Drawing.Size(44, 15);
            this.lbYJog.TabIndex = 0;
            this.lbYJog.Text = "Speed";
            // 
            // numYJog
            // 
            this.numYJog.DecimalPlaces = 2;
            this.numYJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYJog.Location = new System.Drawing.Point(51, 3);
            this.numYJog.Name = "numYJog";
            this.numYJog.Size = new System.Drawing.Size(90, 23);
            this.numYJog.TabIndex = 1;
            this.numYJog.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // btnYJogMinus
            // 
            this.btnYJogMinus.AutoSize = true;
            this.btnYJogMinus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYJogMinus.Location = new System.Drawing.Point(152, 4);
            this.btnYJogMinus.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnYJogMinus.Name = "btnYJogMinus";
            this.btnYJogMinus.Size = new System.Drawing.Size(75, 25);
            this.btnYJogMinus.TabIndex = 2;
            this.btnYJogMinus.Text = "◀ JOG-";
            // 
            // btnYJogPlus
            // 
            this.btnYJogPlus.AutoSize = true;
            this.btnYJogPlus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYJogPlus.Location = new System.Drawing.Point(233, 4);
            this.btnYJogPlus.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnYJogPlus.Name = "btnYJogPlus";
            this.btnYJogPlus.Size = new System.Drawing.Size(75, 25);
            this.btnYJogPlus.TabIndex = 3;
            this.btnYJogPlus.Text = "JOG+ ▶";
            // 
            // moveCardY
            // 
            this.moveCardY.BackColor = System.Drawing.Color.White;
            this.moveCardY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moveCardY.Controls.Add(this.capMoveY);
            this.moveCardY.Controls.Add(this.rowMoveY);
            this.moveCardY.Dock = System.Windows.Forms.DockStyle.Top;
            this.moveCardY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.moveCardY.Location = new System.Drawing.Point(8, 164);
            this.moveCardY.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.moveCardY.Name = "moveCardY";
            this.moveCardY.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.moveCardY.Size = new System.Drawing.Size(797, 83);
            this.moveCardY.TabIndex = 1;
            // 
            // capMoveY
            // 
            this.capMoveY.AutoSize = true;
            this.capMoveY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capMoveY.Location = new System.Drawing.Point(12, 2);
            this.capMoveY.Name = "capMoveY";
            this.capMoveY.Size = new System.Drawing.Size(39, 15);
            this.capMoveY.TabIndex = 0;
            this.capMoveY.Text = "Move";
            // 
            // rowMoveY
            // 
            this.rowMoveY.AutoSize = true;
            this.rowMoveY.Controls.Add(this.lbYTarget);
            this.rowMoveY.Controls.Add(this.numYTarget);
            this.rowMoveY.Controls.Add(this.rYAbs);
            this.rowMoveY.Controls.Add(this.rYRel);
            this.rowMoveY.Controls.Add(this.btnYMove);
            this.rowMoveY.Controls.Add(this.btnYStop);
            this.rowMoveY.Controls.Add(this.btnYHome);
            this.rowMoveY.Controls.Add(this.btnYServo);
            this.rowMoveY.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowMoveY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rowMoveY.Location = new System.Drawing.Point(12, 10);
            this.rowMoveY.Name = "rowMoveY";
            this.rowMoveY.Size = new System.Drawing.Size(771, 29);
            this.rowMoveY.TabIndex = 1;
            // 
            // lbYTarget
            // 
            this.lbYTarget.AutoSize = true;
            this.lbYTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYTarget.Location = new System.Drawing.Point(0, 6);
            this.lbYTarget.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbYTarget.Name = "lbYTarget";
            this.lbYTarget.Size = new System.Drawing.Size(46, 15);
            this.lbYTarget.TabIndex = 0;
            this.lbYTarget.Text = "Target";
            // 
            // numYTarget
            // 
            this.numYTarget.DecimalPlaces = 3;
            this.numYTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYTarget.Location = new System.Drawing.Point(53, 3);
            this.numYTarget.Maximum = new decimal(new int[] {
            2340,
            0,
            0,
            0});
            this.numYTarget.Minimum = new decimal(new int[] {
            2340,
            0,
            0,
            -2147483648});
            this.numYTarget.Name = "numYTarget";
            this.numYTarget.Size = new System.Drawing.Size(100, 23);
            this.numYTarget.TabIndex = 1;
            // 
            // rYAbs
            // 
            this.rYAbs.AutoSize = true;
            this.rYAbs.Checked = true;
            this.rYAbs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rYAbs.Location = new System.Drawing.Point(164, 6);
            this.rYAbs.Margin = new System.Windows.Forms.Padding(8, 6, 6, 0);
            this.rYAbs.Name = "rYAbs";
            this.rYAbs.Size = new System.Drawing.Size(48, 19);
            this.rYAbs.TabIndex = 2;
            this.rYAbs.TabStop = true;
            this.rYAbs.Text = "ABS";
            // 
            // rYRel
            // 
            this.rYRel.AutoSize = true;
            this.rYRel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rYRel.Location = new System.Drawing.Point(218, 6);
            this.rYRel.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.rYRel.Name = "rYRel";
            this.rYRel.Size = new System.Drawing.Size(45, 19);
            this.rYRel.TabIndex = 3;
            this.rYRel.Text = "REL";
            // 
            // btnYMove
            // 
            this.btnYMove.AutoSize = true;
            this.btnYMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYMove.Location = new System.Drawing.Point(277, 4);
            this.btnYMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnYMove.Name = "btnYMove";
            this.btnYMove.Size = new System.Drawing.Size(75, 25);
            this.btnYMove.TabIndex = 4;
            this.btnYMove.Text = "Move";
            // 
            // btnYStop
            // 
            this.btnYStop.AutoSize = true;
            this.btnYStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYStop.Location = new System.Drawing.Point(358, 4);
            this.btnYStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnYStop.Name = "btnYStop";
            this.btnYStop.Size = new System.Drawing.Size(75, 25);
            this.btnYStop.TabIndex = 5;
            this.btnYStop.Text = "Stop";
            // 
            // btnYHome
            // 
            this.btnYHome.AutoSize = true;
            this.btnYHome.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYHome.Location = new System.Drawing.Point(439, 4);
            this.btnYHome.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnYHome.Name = "btnYHome";
            this.btnYHome.Size = new System.Drawing.Size(75, 25);
            this.btnYHome.TabIndex = 6;
            this.btnYHome.Text = "Home";
            // 
            // btnYServo
            // 
            this.btnYServo.AutoSize = true;
            this.btnYServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnYServo.Location = new System.Drawing.Point(520, 4);
            this.btnYServo.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnYServo.Name = "btnYServo";
            this.btnYServo.Size = new System.Drawing.Size(98, 25);
            this.btnYServo.TabIndex = 7;
            this.btnYServo.Text = "Servo ON/OFF";
            // 
            // profileCardY
            // 
            this.profileCardY.BackColor = System.Drawing.Color.White;
            this.profileCardY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.profileCardY.Controls.Add(this.capProfY);
            this.profileCardY.Controls.Add(this.gridY);
            this.profileCardY.Dock = System.Windows.Forms.DockStyle.Top;
            this.profileCardY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.profileCardY.Location = new System.Drawing.Point(8, 105);
            this.profileCardY.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.profileCardY.Name = "profileCardY";
            this.profileCardY.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.profileCardY.Size = new System.Drawing.Size(797, 59);
            this.profileCardY.TabIndex = 2;
            // 
            // capProfY
            // 
            this.capProfY.AutoSize = true;
            this.capProfY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capProfY.Location = new System.Drawing.Point(12, 1);
            this.capProfY.Name = "capProfY";
            this.capProfY.Size = new System.Drawing.Size(125, 15);
            this.capProfY.TabIndex = 0;
            this.capProfY.Text = "Profile (Vel/Acc/Dec)";
            // 
            // gridY
            // 
            this.gridY.AutoSize = true;
            this.gridY.ColumnCount = 6;
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gridY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.gridY.Controls.Add(this.lbYVel, 0, 0);
            this.gridY.Controls.Add(this.numYSpeed, 1, 0);
            this.gridY.Controls.Add(this.lbYAcc, 2, 0);
            this.gridY.Controls.Add(this.numYAcc, 3, 0);
            this.gridY.Controls.Add(this.lbYDec, 4, 0);
            this.gridY.Controls.Add(this.numYDec, 5, 0);
            this.gridY.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.gridY.Location = new System.Drawing.Point(12, 10);
            this.gridY.Name = "gridY";
            this.gridY.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gridY.Size = new System.Drawing.Size(771, 29);
            this.gridY.TabIndex = 1;
            // 
            // lbYVel
            // 
            this.lbYVel.AutoSize = true;
            this.lbYVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYVel.Location = new System.Drawing.Point(0, 6);
            this.lbYVel.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.lbYVel.Name = "lbYVel";
            this.lbYVel.Size = new System.Drawing.Size(25, 15);
            this.lbYVel.TabIndex = 0;
            this.lbYVel.Text = "Vel";
            // 
            // numYSpeed
            // 
            this.numYSpeed.DecimalPlaces = 2;
            this.numYSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYSpeed.Location = new System.Drawing.Point(32, 3);
            this.numYSpeed.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numYSpeed.Name = "numYSpeed";
            this.numYSpeed.Size = new System.Drawing.Size(90, 23);
            this.numYSpeed.TabIndex = 1;
            this.numYSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lbYAcc
            // 
            this.lbYAcc.AutoSize = true;
            this.lbYAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYAcc.Location = new System.Drawing.Point(258, 6);
            this.lbYAcc.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.lbYAcc.Name = "lbYAcc";
            this.lbYAcc.Size = new System.Drawing.Size(27, 15);
            this.lbYAcc.TabIndex = 2;
            this.lbYAcc.Text = "Acc";
            // 
            // numYAcc
            // 
            this.numYAcc.DecimalPlaces = 2;
            this.numYAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYAcc.Location = new System.Drawing.Point(292, 3);
            this.numYAcc.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numYAcc.Name = "numYAcc";
            this.numYAcc.Size = new System.Drawing.Size(90, 23);
            this.numYAcc.TabIndex = 3;
            this.numYAcc.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lbYDec
            // 
            this.lbYDec.AutoSize = true;
            this.lbYDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYDec.Location = new System.Drawing.Point(518, 6);
            this.lbYDec.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.lbYDec.Name = "lbYDec";
            this.lbYDec.Size = new System.Drawing.Size(29, 15);
            this.lbYDec.TabIndex = 4;
            this.lbYDec.Text = "Dec";
            // 
            // numYDec
            // 
            this.numYDec.DecimalPlaces = 2;
            this.numYDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numYDec.Location = new System.Drawing.Point(554, 3);
            this.numYDec.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numYDec.Name = "numYDec";
            this.numYDec.Size = new System.Drawing.Size(90, 23);
            this.numYDec.TabIndex = 5;
            this.numYDec.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // posCardY
            // 
            this.posCardY.BackColor = System.Drawing.Color.White;
            this.posCardY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.posCardY.Controls.Add(this.lblYAPos);
            this.posCardY.Controls.Add(this.AYPos);
            this.posCardY.Controls.Add(this.lblYtPos);
            this.posCardY.Controls.Add(this.capPosY);
            this.posCardY.Controls.Add(this.TargetYPos);
            this.posCardY.Controls.Add(this.lbYPos);
            this.posCardY.Dock = System.Windows.Forms.DockStyle.Top;
            this.posCardY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.posCardY.Location = new System.Drawing.Point(8, 44);
            this.posCardY.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.posCardY.Name = "posCardY";
            this.posCardY.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.posCardY.Size = new System.Drawing.Size(797, 61);
            this.posCardY.TabIndex = 3;
            // 
            // lblYAPos
            // 
            this.lblYAPos.AutoSize = true;
            this.lblYAPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblYAPos.Location = new System.Drawing.Point(299, 24);
            this.lblYAPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblYAPos.Name = "lblYAPos";
            this.lblYAPos.Size = new System.Drawing.Size(64, 15);
            this.lblYAPos.TabIndex = 7;
            this.lblYAPos.Text = "0.000 mm";
            // 
            // AYPos
            // 
            this.AYPos.AutoSize = true;
            this.AYPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.AYPos.Location = new System.Drawing.Point(299, 6);
            this.AYPos.Name = "AYPos";
            this.AYPos.Size = new System.Drawing.Size(146, 15);
            this.AYPos.TabIndex = 6;
            this.AYPos.Text = "Encoder 보정 전 Position";
            // 
            // lblYtPos
            // 
            this.lblYtPos.AutoSize = true;
            this.lblYtPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblYtPos.Location = new System.Drawing.Point(144, 24);
            this.lblYtPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lblYtPos.Name = "lblYtPos";
            this.lblYtPos.Size = new System.Drawing.Size(64, 15);
            this.lblYtPos.TabIndex = 5;
            this.lblYtPos.Text = "0.000 mm";
            // 
            // capPosY
            // 
            this.capPosY.AutoSize = true;
            this.capPosY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.capPosY.Location = new System.Drawing.Point(12, 6);
            this.capPosY.Name = "capPosY";
            this.capPosY.Size = new System.Drawing.Size(62, 15);
            this.capPosY.TabIndex = 0;
            this.capPosY.Text = "F Position";
            // 
            // TargetYPos
            // 
            this.TargetYPos.AutoSize = true;
            this.TargetYPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.TargetYPos.Location = new System.Drawing.Point(144, 6);
            this.TargetYPos.Name = "TargetYPos";
            this.TargetYPos.Size = new System.Drawing.Size(95, 15);
            this.TargetYPos.TabIndex = 4;
            this.TargetYPos.Text = "Target Position";
            // 
            // lbYPos
            // 
            this.lbYPos.AutoSize = true;
            this.lbYPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYPos.Location = new System.Drawing.Point(12, 24);
            this.lbYPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbYPos.Name = "lbYPos";
            this.lbYPos.Size = new System.Drawing.Size(64, 15);
            this.lbYPos.TabIndex = 1;
            this.lbYPos.Text = "0.000 mm";
            // 
            // headerY
            // 
            this.headerY.ColumnCount = 2;
            this.headerY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.headerY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.headerY.Controls.Add(this.lbYTitle, 0, 0);
            this.headerY.Controls.Add(this.chipsY, 1, 0);
            this.headerY.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.headerY.Location = new System.Drawing.Point(8, 8);
            this.headerY.Name = "headerY";
            this.headerY.RowCount = 1;
            this.headerY.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.headerY.Size = new System.Drawing.Size(797, 36);
            this.headerY.TabIndex = 4;
            // 
            // lbYTitle
            // 
            this.lbYTitle.AutoSize = true;
            this.lbYTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbYTitle.Location = new System.Drawing.Point(0, 8);
            this.lbYTitle.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lbYTitle.Name = "lbYTitle";
            this.lbYTitle.Size = new System.Drawing.Size(55, 15);
            this.lbYTitle.TabIndex = 0;
            this.lbYTitle.Text = "Main (Y)";
            // 
            // chipsY
            // 
            this.chipsY.Controls.Add(this.chipYServo);
            this.chipsY.Controls.Add(this.chipYInpos);
            this.chipsY.Controls.Add(this.chipYMove);
            this.chipsY.Controls.Add(this.chipYHomeStatus);
            this.chipsY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chipsY.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.chipsY.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipsY.Location = new System.Drawing.Point(401, 3);
            this.chipsY.Name = "chipsY";
            this.chipsY.Size = new System.Drawing.Size(393, 30);
            this.chipsY.TabIndex = 1;
            this.chipsY.WrapContents = false;
            // 
            // chipYServo
            // 
            this.chipYServo.AutoSize = true;
            this.chipYServo.BackColor = System.Drawing.Color.Gainsboro;
            this.chipYServo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipYServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipYServo.Location = new System.Drawing.Point(326, 4);
            this.chipYServo.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipYServo.Name = "chipYServo";
            this.chipYServo.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipYServo.Size = new System.Drawing.Size(67, 25);
            this.chipYServo.TabIndex = 0;
            this.chipYServo.Text = "SERVO";
            // 
            // chipYInpos
            // 
            this.chipYInpos.AutoSize = true;
            this.chipYInpos.BackColor = System.Drawing.Color.Gainsboro;
            this.chipYInpos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipYInpos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipYInpos.Location = new System.Drawing.Point(251, 4);
            this.chipYInpos.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipYInpos.Name = "chipYInpos";
            this.chipYInpos.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipYInpos.Size = new System.Drawing.Size(71, 25);
            this.chipYInpos.TabIndex = 1;
            this.chipYInpos.Text = "IN-POS";
            // 
            // chipYMove
            // 
            this.chipYMove.AutoSize = true;
            this.chipYMove.BackColor = System.Drawing.Color.Gainsboro;
            this.chipYMove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipYMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipYMove.Location = new System.Drawing.Point(166, 4);
            this.chipYMove.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipYMove.Name = "chipYMove";
            this.chipYMove.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipYMove.Size = new System.Drawing.Size(81, 25);
            this.chipYMove.TabIndex = 2;
            this.chipYMove.Text = "MOVING";
            // 
            // chipYHomeStatus
            // 
            this.chipYHomeStatus.AutoSize = true;
            this.chipYHomeStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.chipYHomeStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipYHomeStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipYHomeStatus.Location = new System.Drawing.Point(88, 4);
            this.chipYHomeStatus.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipYHomeStatus.Name = "chipYHomeStatus";
            this.chipYHomeStatus.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipYHomeStatus.Size = new System.Drawing.Size(74, 25);
            this.chipYHomeStatus.TabIndex = 4;
            this.chipYHomeStatus.Text = "HOMED";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel1.Location = new System.Drawing.Point(7, 423);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(8);
            this.panel1.Size = new System.Drawing.Size(812, 411);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.flowLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(8, 310);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel2.Size = new System.Drawing.Size(796, 95);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "P2P Repeat (Y)";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PA);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PB);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PVel);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PAcc);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PDec);
            this.flowLayoutPanel1.Controls.Add(this.label7);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PDwell);
            this.flowLayoutPanel1.Controls.Add(this.label8);
            this.flowLayoutPanel1.Controls.Add(this.numMZP2PCount);
            this.flowLayoutPanel1.Controls.Add(this.btnMZP2PStart);
            this.flowLayoutPanel1.Controls.Add(this.btnMZP2PStop);
            this.flowLayoutPanel1.Controls.Add(this.lbMZP2PStatus);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(770, 58);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(0, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "A(mm)";
            // 
            // numMZP2PA
            // 
            this.numMZP2PA.DecimalPlaces = 3;
            this.numMZP2PA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PA.Location = new System.Drawing.Point(52, 3);
            this.numMZP2PA.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMZP2PA.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numMZP2PA.Name = "numMZP2PA";
            this.numMZP2PA.Size = new System.Drawing.Size(80, 23);
            this.numMZP2PA.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(143, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "B(mm)";
            // 
            // numMZP2PB
            // 
            this.numMZP2PB.DecimalPlaces = 3;
            this.numMZP2PB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PB.Location = new System.Drawing.Point(195, 3);
            this.numMZP2PB.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMZP2PB.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numMZP2PB.Name = "numMZP2PB";
            this.numMZP2PB.Size = new System.Drawing.Size(80, 23);
            this.numMZP2PB.TabIndex = 3;
            this.numMZP2PB.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(286, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Vel";
            // 
            // numMZP2PVel
            // 
            this.numMZP2PVel.DecimalPlaces = 2;
            this.numMZP2PVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PVel.Location = new System.Drawing.Point(318, 3);
            this.numMZP2PVel.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMZP2PVel.Name = "numMZP2PVel";
            this.numMZP2PVel.Size = new System.Drawing.Size(68, 23);
            this.numMZP2PVel.TabIndex = 5;
            this.numMZP2PVel.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(397, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Acc";
            // 
            // numMZP2PAcc
            // 
            this.numMZP2PAcc.DecimalPlaces = 2;
            this.numMZP2PAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PAcc.Location = new System.Drawing.Point(431, 3);
            this.numMZP2PAcc.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMZP2PAcc.Name = "numMZP2PAcc";
            this.numMZP2PAcc.Size = new System.Drawing.Size(68, 23);
            this.numMZP2PAcc.TabIndex = 7;
            this.numMZP2PAcc.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(510, 6);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "Dec";
            // 
            // numMZP2PDec
            // 
            this.numMZP2PDec.DecimalPlaces = 2;
            this.numMZP2PDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PDec.Location = new System.Drawing.Point(546, 3);
            this.numMZP2PDec.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMZP2PDec.Name = "numMZP2PDec";
            this.numMZP2PDec.Size = new System.Drawing.Size(68, 23);
            this.numMZP2PDec.TabIndex = 9;
            this.numMZP2PDec.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(625, 6);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "Dwell(ms)";
            // 
            // numMZP2PDwell
            // 
            this.numMZP2PDwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PDwell.Location = new System.Drawing.Point(696, 3);
            this.numMZP2PDwell.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMZP2PDwell.Name = "numMZP2PDwell";
            this.numMZP2PDwell.Size = new System.Drawing.Size(68, 23);
            this.numMZP2PDwell.TabIndex = 11;
            this.numMZP2PDwell.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(8, 35);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "Count";
            // 
            // numMZP2PCount
            // 
            this.numMZP2PCount.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZP2PCount.Location = new System.Drawing.Point(56, 32);
            this.numMZP2PCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numMZP2PCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMZP2PCount.Name = "numMZP2PCount";
            this.numMZP2PCount.Size = new System.Drawing.Size(68, 23);
            this.numMZP2PCount.TabIndex = 13;
            this.numMZP2PCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnMZP2PStart
            // 
            this.btnMZP2PStart.AutoSize = true;
            this.btnMZP2PStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZP2PStart.Location = new System.Drawing.Point(137, 33);
            this.btnMZP2PStart.Margin = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.btnMZP2PStart.Name = "btnMZP2PStart";
            this.btnMZP2PStart.Size = new System.Drawing.Size(65, 25);
            this.btnMZP2PStart.TabIndex = 14;
            this.btnMZP2PStart.Text = "Start";
            // 
            // btnMZP2PStop
            // 
            this.btnMZP2PStop.AutoSize = true;
            this.btnMZP2PStop.Enabled = false;
            this.btnMZP2PStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZP2PStop.Location = new System.Drawing.Point(208, 33);
            this.btnMZP2PStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMZP2PStop.Name = "btnMZP2PStop";
            this.btnMZP2PStop.Size = new System.Drawing.Size(65, 25);
            this.btnMZP2PStop.TabIndex = 15;
            this.btnMZP2PStop.Text = "Stop";
            // 
            // lbMZP2PStatus
            // 
            this.lbMZP2PStatus.AutoSize = true;
            this.lbMZP2PStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbMZP2PStatus.Location = new System.Drawing.Point(285, 37);
            this.lbMZP2PStatus.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.lbMZP2PStatus.Name = "lbMZP2PStatus";
            this.lbMZP2PStatus.Size = new System.Drawing.Size(29, 15);
            this.lbMZP2PStatus.TabIndex = 16;
            this.lbMZP2PStatus.Text = "Idle";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.flowLayoutPanel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel3.Location = new System.Drawing.Point(8, 247);
            this.panel3.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel3.Size = new System.Drawing.Size(796, 63);
            this.panel3.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(12, 2);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 15);
            this.label10.TabIndex = 0;
            this.label10.Text = "Jog";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.label11);
            this.flowLayoutPanel2.Controls.Add(this.numMZJog);
            this.flowLayoutPanel2.Controls.Add(this.btnMZJogMinus);
            this.flowLayoutPanel2.Controls.Add(this.btnMZJogPlus);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(770, 29);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(0, 6);
            this.label11.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "Speed";
            // 
            // numMZJog
            // 
            this.numMZJog.DecimalPlaces = 2;
            this.numMZJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZJog.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numMZJog.Location = new System.Drawing.Point(51, 3);
            this.numMZJog.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMZJog.Name = "numMZJog";
            this.numMZJog.Size = new System.Drawing.Size(90, 23);
            this.numMZJog.TabIndex = 1;
            this.numMZJog.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnMZJogMinus
            // 
            this.btnMZJogMinus.AutoSize = true;
            this.btnMZJogMinus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZJogMinus.Location = new System.Drawing.Point(152, 4);
            this.btnMZJogMinus.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnMZJogMinus.Name = "btnMZJogMinus";
            this.btnMZJogMinus.Size = new System.Drawing.Size(75, 25);
            this.btnMZJogMinus.TabIndex = 2;
            this.btnMZJogMinus.Text = "◀ JOG-";
            // 
            // btnMZJogPlus
            // 
            this.btnMZJogPlus.AutoSize = true;
            this.btnMZJogPlus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZJogPlus.Location = new System.Drawing.Point(233, 4);
            this.btnMZJogPlus.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMZJogPlus.Name = "btnMZJogPlus";
            this.btnMZJogPlus.Size = new System.Drawing.Size(75, 25);
            this.btnMZJogPlus.TabIndex = 3;
            this.btnMZJogPlus.Text = "JOG+ ▶";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.flowLayoutPanel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel4.Location = new System.Drawing.Point(8, 164);
            this.panel4.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel4.Size = new System.Drawing.Size(796, 83);
            this.panel4.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(12, 2);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "Move";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.label13);
            this.flowLayoutPanel3.Controls.Add(this.numMZTarget);
            this.flowLayoutPanel3.Controls.Add(this.rMZAbs);
            this.flowLayoutPanel3.Controls.Add(this.rMZRel);
            this.flowLayoutPanel3.Controls.Add(this.btnMZMove);
            this.flowLayoutPanel3.Controls.Add(this.btnMZStop);
            this.flowLayoutPanel3.Controls.Add(this.btnMZHome);
            this.flowLayoutPanel3.Controls.Add(this.btnMZServo);
            this.flowLayoutPanel3.Controls.Add(this.ZHomeParamButton);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(770, 29);
            this.flowLayoutPanel3.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(0, 6);
            this.label13.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 15);
            this.label13.TabIndex = 0;
            this.label13.Text = "Target";
            // 
            // numMZTarget
            // 
            this.numMZTarget.DecimalPlaces = 3;
            this.numMZTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZTarget.Location = new System.Drawing.Point(53, 3);
            this.numMZTarget.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMZTarget.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numMZTarget.Name = "numMZTarget";
            this.numMZTarget.Size = new System.Drawing.Size(100, 23);
            this.numMZTarget.TabIndex = 1;
            // 
            // rMZAbs
            // 
            this.rMZAbs.AutoSize = true;
            this.rMZAbs.Checked = true;
            this.rMZAbs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rMZAbs.Location = new System.Drawing.Point(164, 6);
            this.rMZAbs.Margin = new System.Windows.Forms.Padding(8, 6, 6, 0);
            this.rMZAbs.Name = "rMZAbs";
            this.rMZAbs.Size = new System.Drawing.Size(48, 19);
            this.rMZAbs.TabIndex = 2;
            this.rMZAbs.TabStop = true;
            this.rMZAbs.Text = "ABS";
            // 
            // rMZRel
            // 
            this.rMZRel.AutoSize = true;
            this.rMZRel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rMZRel.Location = new System.Drawing.Point(218, 6);
            this.rMZRel.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.rMZRel.Name = "rMZRel";
            this.rMZRel.Size = new System.Drawing.Size(45, 19);
            this.rMZRel.TabIndex = 3;
            this.rMZRel.Text = "REL";
            // 
            // btnMZMove
            // 
            this.btnMZMove.AutoSize = true;
            this.btnMZMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZMove.Location = new System.Drawing.Point(277, 4);
            this.btnMZMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnMZMove.Name = "btnMZMove";
            this.btnMZMove.Size = new System.Drawing.Size(75, 25);
            this.btnMZMove.TabIndex = 4;
            this.btnMZMove.Text = "Move";
            // 
            // btnMZStop
            // 
            this.btnMZStop.AutoSize = true;
            this.btnMZStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZStop.Location = new System.Drawing.Point(358, 4);
            this.btnMZStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMZStop.Name = "btnMZStop";
            this.btnMZStop.Size = new System.Drawing.Size(75, 25);
            this.btnMZStop.TabIndex = 5;
            this.btnMZStop.Text = "Stop";
            // 
            // btnMZHome
            // 
            this.btnMZHome.AutoSize = true;
            this.btnMZHome.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZHome.Location = new System.Drawing.Point(439, 4);
            this.btnMZHome.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMZHome.Name = "btnMZHome";
            this.btnMZHome.Size = new System.Drawing.Size(75, 25);
            this.btnMZHome.TabIndex = 6;
            this.btnMZHome.Text = "Home";
            // 
            // btnMZServo
            // 
            this.btnMZServo.AutoSize = true;
            this.btnMZServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZServo.Location = new System.Drawing.Point(520, 4);
            this.btnMZServo.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnMZServo.Name = "btnMZServo";
            this.btnMZServo.Size = new System.Drawing.Size(98, 25);
            this.btnMZServo.TabIndex = 7;
            this.btnMZServo.Text = "Servo ON/OFF";
            // 
            // ZHomeParamButton
            // 
            this.ZHomeParamButton.AutoSize = true;
            this.ZHomeParamButton.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.ZHomeParamButton.Location = new System.Drawing.Point(626, 4);
            this.ZHomeParamButton.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.ZHomeParamButton.Name = "ZHomeParamButton";
            this.ZHomeParamButton.Size = new System.Drawing.Size(114, 25);
            this.ZHomeParamButton.TabIndex = 8;
            this.ZHomeParamButton.Text = "Param(변경X)";
            this.ZHomeParamButton.Click += new System.EventHandler(this.ZHomeParamButton_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label14);
            this.panel5.Controls.Add(this.tableLayoutPanel1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel5.Location = new System.Drawing.Point(8, 105);
            this.panel5.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel5.Size = new System.Drawing.Size(796, 59);
            this.panel5.TabIndex = 2;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(12, 1);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(125, 15);
            this.label14.TabIndex = 0;
            this.label14.Text = "Profile (Vel/Acc/Dec)";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMZSpeed, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label16, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMZAcc, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label17, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMZDec, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(770, 29);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(0, 6);
            this.label15.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(25, 15);
            this.label15.TabIndex = 0;
            this.label15.Text = "Vel";
            // 
            // numMZSpeed
            // 
            this.numMZSpeed.DecimalPlaces = 2;
            this.numMZSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numMZSpeed.Location = new System.Drawing.Point(32, 3);
            this.numMZSpeed.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMZSpeed.Name = "numMZSpeed";
            this.numMZSpeed.Size = new System.Drawing.Size(90, 23);
            this.numMZSpeed.TabIndex = 1;
            this.numMZSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(258, 6);
            this.label16.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(27, 15);
            this.label16.TabIndex = 2;
            this.label16.Text = "Acc";
            // 
            // numMZAcc
            // 
            this.numMZAcc.DecimalPlaces = 2;
            this.numMZAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZAcc.Location = new System.Drawing.Point(292, 3);
            this.numMZAcc.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMZAcc.Name = "numMZAcc";
            this.numMZAcc.Size = new System.Drawing.Size(90, 23);
            this.numMZAcc.TabIndex = 3;
            this.numMZAcc.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(518, 6);
            this.label17.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 15);
            this.label17.TabIndex = 4;
            this.label17.Text = "Dec";
            // 
            // numMZDec
            // 
            this.numMZDec.DecimalPlaces = 2;
            this.numMZDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numMZDec.Location = new System.Drawing.Point(554, 3);
            this.numMZDec.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMZDec.Name = "numMZDec";
            this.numMZDec.Size = new System.Drawing.Size(90, 23);
            this.numMZDec.TabIndex = 5;
            this.numMZDec.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.label18);
            this.panel6.Controls.Add(this.lbMZPos);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel6.Location = new System.Drawing.Point(8, 44);
            this.panel6.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel6.Size = new System.Drawing.Size(796, 61);
            this.panel6.TabIndex = 3;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(12, 6);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 15);
            this.label18.TabIndex = 0;
            this.label18.Text = "Position";
            // 
            // lbMZPos
            // 
            this.lbMZPos.AutoSize = true;
            this.lbMZPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbMZPos.Location = new System.Drawing.Point(12, 24);
            this.lbMZPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbMZPos.Name = "lbMZPos";
            this.lbMZPos.Size = new System.Drawing.Size(64, 15);
            this.lbMZPos.TabIndex = 1;
            this.lbMZPos.Text = "0.000 mm";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label20, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel4, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(796, 36);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(0, 8);
            this.label20.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(60, 15);
            this.label20.TabIndex = 0;
            this.label20.Text = "Maint (Z)";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.chipMZServo);
            this.flowLayoutPanel4.Controls.Add(this.chipMZInpos);
            this.flowLayoutPanel4.Controls.Add(this.chipMZMove);
            this.flowLayoutPanel4.Controls.Add(this.chipMZHomeStatus);
            this.flowLayoutPanel4.Controls.Add(this.btnMZAlarmReset);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(401, 3);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(392, 30);
            this.flowLayoutPanel4.TabIndex = 1;
            this.flowLayoutPanel4.WrapContents = false;
            // 
            // chipMZServo
            // 
            this.chipMZServo.AutoSize = true;
            this.chipMZServo.BackColor = System.Drawing.Color.Gainsboro;
            this.chipMZServo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipMZServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipMZServo.Location = new System.Drawing.Point(325, 4);
            this.chipMZServo.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipMZServo.Name = "chipMZServo";
            this.chipMZServo.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipMZServo.Size = new System.Drawing.Size(67, 25);
            this.chipMZServo.TabIndex = 0;
            this.chipMZServo.Text = "SERVO";
            // 
            // chipMZInpos
            // 
            this.chipMZInpos.AutoSize = true;
            this.chipMZInpos.BackColor = System.Drawing.Color.Gainsboro;
            this.chipMZInpos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipMZInpos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipMZInpos.Location = new System.Drawing.Point(250, 4);
            this.chipMZInpos.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipMZInpos.Name = "chipMZInpos";
            this.chipMZInpos.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipMZInpos.Size = new System.Drawing.Size(71, 25);
            this.chipMZInpos.TabIndex = 1;
            this.chipMZInpos.Text = "IN-POS";
            // 
            // chipMZMove
            // 
            this.chipMZMove.AutoSize = true;
            this.chipMZMove.BackColor = System.Drawing.Color.Gainsboro;
            this.chipMZMove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipMZMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipMZMove.Location = new System.Drawing.Point(165, 4);
            this.chipMZMove.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipMZMove.Name = "chipMZMove";
            this.chipMZMove.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipMZMove.Size = new System.Drawing.Size(81, 25);
            this.chipMZMove.TabIndex = 2;
            this.chipMZMove.Text = "MOVING";
            // 
            // chipMZHomeStatus
            // 
            this.chipMZHomeStatus.AutoSize = true;
            this.chipMZHomeStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.chipMZHomeStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipMZHomeStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipMZHomeStatus.Location = new System.Drawing.Point(87, 4);
            this.chipMZHomeStatus.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipMZHomeStatus.Name = "chipMZHomeStatus";
            this.chipMZHomeStatus.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipMZHomeStatus.Size = new System.Drawing.Size(74, 25);
            this.chipMZHomeStatus.TabIndex = 4;
            this.chipMZHomeStatus.Text = "HOMED";
            // 
            // btnMZAlarmReset
            // 
            this.btnMZAlarmReset.AutoSize = true;
            this.btnMZAlarmReset.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnMZAlarmReset.Location = new System.Drawing.Point(8, 4);
            this.btnMZAlarmReset.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnMZAlarmReset.Name = "btnMZAlarmReset";
            this.btnMZAlarmReset.Size = new System.Drawing.Size(75, 25);
            this.btnMZAlarmReset.TabIndex = 5;
            this.btnMZAlarmReset.Text = "Alarm";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Transparent;
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Controls.Add(this.panel9);
            this.panel7.Controls.Add(this.panel10);
            this.panel7.Controls.Add(this.panel11);
            this.panel7.Controls.Add(this.panel12);
            this.panel7.Controls.Add(this.tableLayoutPanel4);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel7.Location = new System.Drawing.Point(825, 423);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(8);
            this.panel7.Size = new System.Drawing.Size(813, 411);
            this.panel7.TabIndex = 3;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.label24);
            this.panel8.Controls.Add(this.flowLayoutPanel5);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel8.Location = new System.Drawing.Point(8, 310);
            this.panel8.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel8.Name = "panel8";
            this.panel8.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel8.Size = new System.Drawing.Size(797, 95);
            this.panel8.TabIndex = 0;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label24.Location = new System.Drawing.Point(12, 2);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(93, 15);
            this.label24.TabIndex = 0;
            this.label24.Text = "P2P Repeat (Y)";
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.Controls.Add(this.label25);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PA);
            this.flowLayoutPanel5.Controls.Add(this.label26);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PB);
            this.flowLayoutPanel5.Controls.Add(this.label27);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PVel);
            this.flowLayoutPanel5.Controls.Add(this.label28);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PAcc);
            this.flowLayoutPanel5.Controls.Add(this.label29);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PDec);
            this.flowLayoutPanel5.Controls.Add(this.label30);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PDwell);
            this.flowLayoutPanel5.Controls.Add(this.label31);
            this.flowLayoutPanel5.Controls.Add(this.numTP2PCount);
            this.flowLayoutPanel5.Controls.Add(this.btnTP2PStart);
            this.flowLayoutPanel5.Controls.Add(this.btnTP2PStop);
            this.flowLayoutPanel5.Controls.Add(this.lbTP2PStatus);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(771, 58);
            this.flowLayoutPanel5.TabIndex = 1;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(0, 6);
            this.label25.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(46, 15);
            this.label25.TabIndex = 0;
            this.label25.Text = "A(deg)";
            // 
            // numTP2PA
            // 
            this.numTP2PA.DecimalPlaces = 2;
            this.numTP2PA.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTP2PA.Location = new System.Drawing.Point(53, 3);
            this.numTP2PA.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTP2PA.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147418112});
            this.numTP2PA.Name = "numTP2PA";
            this.numTP2PA.Size = new System.Drawing.Size(80, 23);
            this.numTP2PA.TabIndex = 1;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label26.Location = new System.Drawing.Point(144, 6);
            this.label26.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(46, 15);
            this.label26.TabIndex = 2;
            this.label26.Text = "B(deg)";
            // 
            // numTP2PB
            // 
            this.numTP2PB.DecimalPlaces = 2;
            this.numTP2PB.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PB.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTP2PB.Location = new System.Drawing.Point(197, 3);
            this.numTP2PB.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTP2PB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147418112});
            this.numTP2PB.Name = "numTP2PB";
            this.numTP2PB.Size = new System.Drawing.Size(80, 23);
            this.numTP2PB.TabIndex = 3;
            this.numTP2PB.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label27.Location = new System.Drawing.Point(288, 6);
            this.label27.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(25, 15);
            this.label27.TabIndex = 4;
            this.label27.Text = "Vel";
            // 
            // numTP2PVel
            // 
            this.numTP2PVel.DecimalPlaces = 3;
            this.numTP2PVel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PVel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numTP2PVel.Location = new System.Drawing.Point(320, 3);
            this.numTP2PVel.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numTP2PVel.Name = "numTP2PVel";
            this.numTP2PVel.Size = new System.Drawing.Size(68, 23);
            this.numTP2PVel.TabIndex = 5;
            this.numTP2PVel.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label28.Location = new System.Drawing.Point(399, 6);
            this.label28.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(27, 15);
            this.label28.TabIndex = 6;
            this.label28.Text = "Acc";
            // 
            // numTP2PAcc
            // 
            this.numTP2PAcc.DecimalPlaces = 2;
            this.numTP2PAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PAcc.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTP2PAcc.Location = new System.Drawing.Point(433, 3);
            this.numTP2PAcc.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTP2PAcc.Name = "numTP2PAcc";
            this.numTP2PAcc.Size = new System.Drawing.Size(68, 23);
            this.numTP2PAcc.TabIndex = 7;
            this.numTP2PAcc.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label29.Location = new System.Drawing.Point(512, 6);
            this.label29.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(29, 15);
            this.label29.TabIndex = 8;
            this.label29.Text = "Dec";
            // 
            // numTP2PDec
            // 
            this.numTP2PDec.DecimalPlaces = 2;
            this.numTP2PDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PDec.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTP2PDec.Location = new System.Drawing.Point(548, 3);
            this.numTP2PDec.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTP2PDec.Name = "numTP2PDec";
            this.numTP2PDec.Size = new System.Drawing.Size(68, 23);
            this.numTP2PDec.TabIndex = 9;
            this.numTP2PDec.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label30.Location = new System.Drawing.Point(627, 6);
            this.label30.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(64, 15);
            this.label30.TabIndex = 10;
            this.label30.Text = "Dwell(ms)";
            // 
            // numTP2PDwell
            // 
            this.numTP2PDwell.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PDwell.Location = new System.Drawing.Point(698, 3);
            this.numTP2PDwell.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTP2PDwell.Name = "numTP2PDwell";
            this.numTP2PDwell.Size = new System.Drawing.Size(68, 23);
            this.numTP2PDwell.TabIndex = 11;
            this.numTP2PDwell.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label31.Location = new System.Drawing.Point(8, 35);
            this.label31.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(41, 15);
            this.label31.TabIndex = 12;
            this.label31.Text = "Count";
            // 
            // numTP2PCount
            // 
            this.numTP2PCount.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTP2PCount.Location = new System.Drawing.Point(56, 32);
            this.numTP2PCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTP2PCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTP2PCount.Name = "numTP2PCount";
            this.numTP2PCount.Size = new System.Drawing.Size(68, 23);
            this.numTP2PCount.TabIndex = 13;
            this.numTP2PCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnTP2PStart
            // 
            this.btnTP2PStart.AutoSize = true;
            this.btnTP2PStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTP2PStart.Location = new System.Drawing.Point(137, 33);
            this.btnTP2PStart.Margin = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.btnTP2PStart.Name = "btnTP2PStart";
            this.btnTP2PStart.Size = new System.Drawing.Size(65, 25);
            this.btnTP2PStart.TabIndex = 14;
            this.btnTP2PStart.Text = "Start";
            // 
            // btnTP2PStop
            // 
            this.btnTP2PStop.AutoSize = true;
            this.btnTP2PStop.Enabled = false;
            this.btnTP2PStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTP2PStop.Location = new System.Drawing.Point(208, 33);
            this.btnTP2PStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnTP2PStop.Name = "btnTP2PStop";
            this.btnTP2PStop.Size = new System.Drawing.Size(65, 25);
            this.btnTP2PStop.TabIndex = 15;
            this.btnTP2PStop.Text = "Stop";
            // 
            // lbTP2PStatus
            // 
            this.lbTP2PStatus.AutoSize = true;
            this.lbTP2PStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbTP2PStatus.Location = new System.Drawing.Point(285, 37);
            this.lbTP2PStatus.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.lbTP2PStatus.Name = "lbTP2PStatus";
            this.lbTP2PStatus.Size = new System.Drawing.Size(29, 15);
            this.lbTP2PStatus.TabIndex = 16;
            this.lbTP2PStatus.Text = "Idle";
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.White;
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.label33);
            this.panel9.Controls.Add(this.flowLayoutPanel6);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel9.Location = new System.Drawing.Point(8, 247);
            this.panel9.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel9.Name = "panel9";
            this.panel9.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel9.Size = new System.Drawing.Size(797, 63);
            this.panel9.TabIndex = 0;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label33.Location = new System.Drawing.Point(12, 2);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(27, 15);
            this.label33.TabIndex = 0;
            this.label33.Text = "Jog";
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.Controls.Add(this.label34);
            this.flowLayoutPanel6.Controls.Add(this.numTJog);
            this.flowLayoutPanel6.Controls.Add(this.btnTJogMinus);
            this.flowLayoutPanel6.Controls.Add(this.btnTJogPlus);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(771, 29);
            this.flowLayoutPanel6.TabIndex = 1;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label34.Location = new System.Drawing.Point(0, 6);
            this.label34.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(44, 15);
            this.label34.TabIndex = 0;
            this.label34.Text = "Speed";
            // 
            // numTJog
            // 
            this.numTJog.DecimalPlaces = 3;
            this.numTJog.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTJog.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numTJog.Location = new System.Drawing.Point(51, 3);
            this.numTJog.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numTJog.Name = "numTJog";
            this.numTJog.Size = new System.Drawing.Size(90, 23);
            this.numTJog.TabIndex = 1;
            this.numTJog.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // btnTJogMinus
            // 
            this.btnTJogMinus.AutoSize = true;
            this.btnTJogMinus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTJogMinus.Location = new System.Drawing.Point(152, 4);
            this.btnTJogMinus.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnTJogMinus.Name = "btnTJogMinus";
            this.btnTJogMinus.Size = new System.Drawing.Size(75, 25);
            this.btnTJogMinus.TabIndex = 2;
            this.btnTJogMinus.Text = "◀ JOG-";
            // 
            // btnTJogPlus
            // 
            this.btnTJogPlus.AutoSize = true;
            this.btnTJogPlus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTJogPlus.Location = new System.Drawing.Point(233, 4);
            this.btnTJogPlus.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnTJogPlus.Name = "btnTJogPlus";
            this.btnTJogPlus.Size = new System.Drawing.Size(75, 25);
            this.btnTJogPlus.TabIndex = 3;
            this.btnTJogPlus.Text = "JOG+ ▶";
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.White;
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.label35);
            this.panel10.Controls.Add(this.flowLayoutPanel7);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel10.Location = new System.Drawing.Point(8, 164);
            this.panel10.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel10.Size = new System.Drawing.Size(797, 83);
            this.panel10.TabIndex = 1;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label35.Location = new System.Drawing.Point(12, 2);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(39, 15);
            this.label35.TabIndex = 0;
            this.label35.Text = "Move";
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.Controls.Add(this.label36);
            this.flowLayoutPanel7.Controls.Add(this.numTTarget);
            this.flowLayoutPanel7.Controls.Add(this.rTAbs);
            this.flowLayoutPanel7.Controls.Add(this.rTRel);
            this.flowLayoutPanel7.Controls.Add(this.btnTMove);
            this.flowLayoutPanel7.Controls.Add(this.btnTStop);
            this.flowLayoutPanel7.Controls.Add(this.btnTHome);
            this.flowLayoutPanel7.Controls.Add(this.btnTServo);
            this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel7.Location = new System.Drawing.Point(12, 10);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(771, 29);
            this.flowLayoutPanel7.TabIndex = 1;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label36.Location = new System.Drawing.Point(0, 6);
            this.label36.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(46, 15);
            this.label36.TabIndex = 0;
            this.label36.Text = "Target";
            // 
            // numTTarget
            // 
            this.numTTarget.DecimalPlaces = 3;
            this.numTTarget.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTTarget.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTTarget.Location = new System.Drawing.Point(53, 3);
            this.numTTarget.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTTarget.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147418112});
            this.numTTarget.Name = "numTTarget";
            this.numTTarget.Size = new System.Drawing.Size(100, 23);
            this.numTTarget.TabIndex = 1;
            // 
            // rTAbs
            // 
            this.rTAbs.AutoSize = true;
            this.rTAbs.Checked = true;
            this.rTAbs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rTAbs.Location = new System.Drawing.Point(164, 6);
            this.rTAbs.Margin = new System.Windows.Forms.Padding(8, 6, 6, 0);
            this.rTAbs.Name = "rTAbs";
            this.rTAbs.Size = new System.Drawing.Size(48, 19);
            this.rTAbs.TabIndex = 2;
            this.rTAbs.TabStop = true;
            this.rTAbs.Text = "ABS";
            // 
            // rTRel
            // 
            this.rTRel.AutoSize = true;
            this.rTRel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rTRel.Location = new System.Drawing.Point(218, 6);
            this.rTRel.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.rTRel.Name = "rTRel";
            this.rTRel.Size = new System.Drawing.Size(45, 19);
            this.rTRel.TabIndex = 3;
            this.rTRel.Text = "REL";
            // 
            // btnTMove
            // 
            this.btnTMove.AutoSize = true;
            this.btnTMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTMove.Location = new System.Drawing.Point(277, 4);
            this.btnTMove.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnTMove.Name = "btnTMove";
            this.btnTMove.Size = new System.Drawing.Size(75, 25);
            this.btnTMove.TabIndex = 4;
            this.btnTMove.Text = "Move";
            // 
            // btnTStop
            // 
            this.btnTStop.AutoSize = true;
            this.btnTStop.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTStop.Location = new System.Drawing.Point(358, 4);
            this.btnTStop.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnTStop.Name = "btnTStop";
            this.btnTStop.Size = new System.Drawing.Size(75, 25);
            this.btnTStop.TabIndex = 5;
            this.btnTStop.Text = "Stop";
            // 
            // btnTHome
            // 
            this.btnTHome.AutoSize = true;
            this.btnTHome.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTHome.Location = new System.Drawing.Point(439, 4);
            this.btnTHome.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnTHome.Name = "btnTHome";
            this.btnTHome.Size = new System.Drawing.Size(75, 25);
            this.btnTHome.TabIndex = 6;
            this.btnTHome.Text = "Home";
            // 
            // btnTServo
            // 
            this.btnTServo.AutoSize = true;
            this.btnTServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTServo.Location = new System.Drawing.Point(520, 4);
            this.btnTServo.Margin = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.btnTServo.Name = "btnTServo";
            this.btnTServo.Size = new System.Drawing.Size(98, 25);
            this.btnTServo.TabIndex = 7;
            this.btnTServo.Text = "Servo ON/OFF";
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.White;
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Controls.Add(this.label37);
            this.panel11.Controls.Add(this.tableLayoutPanel3);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel11.Location = new System.Drawing.Point(8, 105);
            this.panel11.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel11.Name = "panel11";
            this.panel11.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel11.Size = new System.Drawing.Size(797, 59);
            this.panel11.TabIndex = 2;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label37.Location = new System.Drawing.Point(12, 1);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(125, 15);
            this.label37.TabIndex = 0;
            this.label37.Text = "Profile (Vel/Acc/Dec)";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 6;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.label38, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.numTSpeed, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label39, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.numTAcc, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label40, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.numTDec, 5, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 10);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(771, 29);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label38.Location = new System.Drawing.Point(0, 6);
            this.label38.Margin = new System.Windows.Forms.Padding(0, 6, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(25, 15);
            this.label38.TabIndex = 0;
            this.label38.Text = "Vel";
            // 
            // numTSpeed
            // 
            this.numTSpeed.DecimalPlaces = 3;
            this.numTSpeed.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numTSpeed.Location = new System.Drawing.Point(32, 3);
            this.numTSpeed.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numTSpeed.Name = "numTSpeed";
            this.numTSpeed.Size = new System.Drawing.Size(90, 23);
            this.numTSpeed.TabIndex = 1;
            this.numTSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label39.Location = new System.Drawing.Point(258, 6);
            this.label39.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(27, 15);
            this.label39.TabIndex = 2;
            this.label39.Text = "Acc";
            // 
            // numTAcc
            // 
            this.numTAcc.DecimalPlaces = 2;
            this.numTAcc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTAcc.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTAcc.Location = new System.Drawing.Point(292, 3);
            this.numTAcc.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTAcc.Name = "numTAcc";
            this.numTAcc.Size = new System.Drawing.Size(90, 23);
            this.numTAcc.TabIndex = 3;
            this.numTAcc.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label40.Location = new System.Drawing.Point(518, 6);
            this.label40.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(29, 15);
            this.label40.TabIndex = 4;
            this.label40.Text = "Dec";
            // 
            // numTDec
            // 
            this.numTDec.DecimalPlaces = 2;
            this.numTDec.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.numTDec.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTDec.Location = new System.Drawing.Point(554, 3);
            this.numTDec.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numTDec.Name = "numTDec";
            this.numTDec.Size = new System.Drawing.Size(90, 23);
            this.numTDec.TabIndex = 5;
            this.numTDec.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.White;
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.label41);
            this.panel12.Controls.Add(this.lbTPos);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.panel12.Location = new System.Drawing.Point(8, 44);
            this.panel12.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panel12.Name = "panel12";
            this.panel12.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.panel12.Size = new System.Drawing.Size(797, 61);
            this.panel12.TabIndex = 3;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label41.Location = new System.Drawing.Point(12, 6);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(52, 15);
            this.label41.TabIndex = 0;
            this.label41.Text = "Position";
            // 
            // lbTPos
            // 
            this.lbTPos.AutoSize = true;
            this.lbTPos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lbTPos.Location = new System.Drawing.Point(12, 24);
            this.lbTPos.Margin = new System.Windows.Forms.Padding(12, 20, 0, 8);
            this.lbTPos.Name = "lbTPos";
            this.lbTPos.Size = new System.Drawing.Size(79, 15);
            this.lbTPos.TabIndex = 1;
            this.lbTPos.Text = "0.00000 deg";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.label43, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel8, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(797, 36);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.label43.Location = new System.Drawing.Point(0, 8);
            this.label43.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(190, 15);
            this.label43.TabIndex = 0;
            this.label43.Text = "Theta (T) (360deg = Home위치)";
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Controls.Add(this.chipTServo);
            this.flowLayoutPanel8.Controls.Add(this.chipTInpos);
            this.flowLayoutPanel8.Controls.Add(this.chipTMove);
            this.flowLayoutPanel8.Controls.Add(this.chipTHomeStatus);
            this.flowLayoutPanel8.Controls.Add(this.btnTAlarmReset);
            this.flowLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel8.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(401, 3);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(393, 30);
            this.flowLayoutPanel8.TabIndex = 1;
            this.flowLayoutPanel8.WrapContents = false;
            // 
            // chipTServo
            // 
            this.chipTServo.AutoSize = true;
            this.chipTServo.BackColor = System.Drawing.Color.Gainsboro;
            this.chipTServo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipTServo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipTServo.Location = new System.Drawing.Point(326, 4);
            this.chipTServo.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipTServo.Name = "chipTServo";
            this.chipTServo.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipTServo.Size = new System.Drawing.Size(67, 25);
            this.chipTServo.TabIndex = 0;
            this.chipTServo.Text = "SERVO";
            // 
            // chipTInpos
            // 
            this.chipTInpos.AutoSize = true;
            this.chipTInpos.BackColor = System.Drawing.Color.Gainsboro;
            this.chipTInpos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipTInpos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipTInpos.Location = new System.Drawing.Point(251, 4);
            this.chipTInpos.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipTInpos.Name = "chipTInpos";
            this.chipTInpos.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipTInpos.Size = new System.Drawing.Size(71, 25);
            this.chipTInpos.TabIndex = 1;
            this.chipTInpos.Text = "IN-POS";
            // 
            // chipTMove
            // 
            this.chipTMove.AutoSize = true;
            this.chipTMove.BackColor = System.Drawing.Color.Gainsboro;
            this.chipTMove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipTMove.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipTMove.Location = new System.Drawing.Point(166, 4);
            this.chipTMove.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipTMove.Name = "chipTMove";
            this.chipTMove.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipTMove.Size = new System.Drawing.Size(81, 25);
            this.chipTMove.TabIndex = 2;
            this.chipTMove.Text = "MOVING";
            // 
            // chipTHomeStatus
            // 
            this.chipTHomeStatus.AutoSize = true;
            this.chipTHomeStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.chipTHomeStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chipTHomeStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.chipTHomeStatus.Location = new System.Drawing.Point(88, 4);
            this.chipTHomeStatus.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.chipTHomeStatus.Name = "chipTHomeStatus";
            this.chipTHomeStatus.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.chipTHomeStatus.Size = new System.Drawing.Size(74, 25);
            this.chipTHomeStatus.TabIndex = 5;
            this.chipTHomeStatus.Text = "HOMED";
            // 
            // btnTAlarmReset
            // 
            this.btnTAlarmReset.AutoSize = true;
            this.btnTAlarmReset.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnTAlarmReset.Location = new System.Drawing.Point(9, 4);
            this.btnTAlarmReset.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.btnTAlarmReset.Name = "btnTAlarmReset";
            this.btnTAlarmReset.Size = new System.Drawing.Size(75, 25);
            this.btnTAlarmReset.TabIndex = 6;
            this.btnTAlarmReset.Text = "Alarm";
            // 
            // MotionManualForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1665, 861);
            this.Controls.Add(this.tlRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MotionManualForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Manual Motion";
            this.tlRoot.ResumeLayout(false);
            this.pnlX.ResumeLayout(false);
            this.p2pCardX.ResumeLayout(false);
            this.p2pCardX.PerformLayout();
            this.rowP2PX.ResumeLayout(false);
            this.rowP2PX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PVel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PDec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PDwell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXP2PCount)).EndInit();
            this.jogCardX.ResumeLayout(false);
            this.jogCardX.PerformLayout();
            this.rowJogX.ResumeLayout(false);
            this.rowJogX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXJog)).EndInit();
            this.moveCardX.ResumeLayout(false);
            this.moveCardX.PerformLayout();
            this.rowMoveX.ResumeLayout(false);
            this.rowMoveX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXTarget)).EndInit();
            this.profileCardX.ResumeLayout(false);
            this.profileCardX.PerformLayout();
            this.gridX.ResumeLayout(false);
            this.gridX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numXSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numXDec)).EndInit();
            this.posCardX.ResumeLayout(false);
            this.posCardX.PerformLayout();
            this.headerX.ResumeLayout(false);
            this.headerX.PerformLayout();
            this.chipsX.ResumeLayout(false);
            this.chipsX.PerformLayout();
            this.pnlY.ResumeLayout(false);
            this.p2pCardY.ResumeLayout(false);
            this.p2pCardY.PerformLayout();
            this.rowP2PY.ResumeLayout(false);
            this.rowP2PY.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PVel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PDec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PDwell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYP2PCount)).EndInit();
            this.jogCardY.ResumeLayout(false);
            this.jogCardY.PerformLayout();
            this.rowJogY.ResumeLayout(false);
            this.rowJogY.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYJog)).EndInit();
            this.moveCardY.ResumeLayout(false);
            this.moveCardY.PerformLayout();
            this.rowMoveY.ResumeLayout(false);
            this.rowMoveY.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYTarget)).EndInit();
            this.profileCardY.ResumeLayout(false);
            this.profileCardY.PerformLayout();
            this.gridY.ResumeLayout(false);
            this.gridY.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYDec)).EndInit();
            this.posCardY.ResumeLayout(false);
            this.posCardY.PerformLayout();
            this.headerY.ResumeLayout(false);
            this.headerY.PerformLayout();
            this.chipsY.ResumeLayout(false);
            this.chipsY.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PVel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PDec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PDwell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZP2PCount)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZJog)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZTarget)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMZSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMZDec)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PVel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PDec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PDwell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTP2PCount)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTJog)).EndInit();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTTarget)).EndInit();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTDec)).EndInit();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);

        }

        // --- 필드 선언부는 기존 그대로 ---
        private TableLayoutPanel tlRoot;
        private Panel pnlX, pnlY;
        private TableLayoutPanel headerX, headerY;
        private FlowLayoutPanel chipsX, chipsY;

        private Panel posCardX, profileCardX, moveCardX, jogCardX;
        private Panel posCardY, profileCardY, moveCardY, jogCardY;

        private Label lbXPos, lbYPos;
        private Label chipXServo, chipXMove, chipXInpos;
        private Label chipYServo, chipYMove, chipYInpos;

        private NumericUpDown numXSpeed, numXAcc, numXDec;
        private NumericUpDown numYSpeed, numYAcc, numYDec;

        private NumericUpDown numXTarget, numYTarget;
        private RadioButton rXAbs, rXRel, rYAbs, rYRel;

        private NumericUpDown numXJog, numYJog;
        private Button btnXMove, btnXStop, btnXHome, btnXJogMinus, btnXJogPlus, btnXServo;
        private Button btnYMove, btnYStop, btnYHome, btnYJogMinus, btnYJogPlus, btnYServo;
        private Label capJogX;
        private FlowLayoutPanel rowJogX;
        private Label lbXJog;
        private Label capMoveX;
        private FlowLayoutPanel rowMoveX;
        private Label lbXTarget;
        private Label capProfX;
        private TableLayoutPanel gridX;
        private Label lbXVel;
        private Label lbXAcc;
        private Label lbXDec;
        private Label capPosX;
        private Label lbXTitle;
        private Label capJogY;
        private FlowLayoutPanel rowJogY;
        private Label lbYJog;
        private Label capMoveY;
        private FlowLayoutPanel rowMoveY;
        private Label lbYTarget;
        private Label capProfY;
        private TableLayoutPanel gridY;
        private Label lbYVel;
        private Label lbYAcc;
        private Label lbYDec;
        private Label capPosY;
        private Label lbYTitle;

        // --- P2P UI & 상태 ---
        private Panel p2pCardX, p2pCardY;

        private NumericUpDown numXP2PA, numXP2PB, numXP2PVel, numXP2PAcc, numXP2PDec, numXP2PDwell, numXP2PCount;
        private NumericUpDown numYP2PA, numYP2PB, numYP2PVel, numYP2PAcc, numYP2PDec, numYP2PDwell, numYP2PCount;

        private Button btnXP2PStart, btnXP2PStop, btnYP2PStart, btnYP2PStop;
        private Label lbXP2PStatus, lbYP2PStatus;

        private Label capP2PX;
        private FlowLayoutPanel rowP2PX;
        private Label lbXA;
        private Label lbXB;
        private Label lbXv;
        private Label lbXac;
        private Label lbXd;
        private Label lbXdwell;
        private Label lbXcnt;
        private Label capP2PY;
        private FlowLayoutPanel rowP2PY;
        private Label lbYA;
        private Label lbYB;
        private Label lbYv;
        private Label lbYac;
        private Label lbYd;
        private Label lbYdwell;
        private Label lbYcnt;
        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label2;
        private NumericUpDown numMZP2PA;
        private Label label3;
        private NumericUpDown numMZP2PB;
        private Label label4;
        private NumericUpDown numMZP2PVel;
        private Label label5;
        private NumericUpDown numMZP2PAcc;
        private Label label6;
        private NumericUpDown numMZP2PDec;
        private Label label7;
        private NumericUpDown numMZP2PDwell;
        private Label label8;
        private NumericUpDown numMZP2PCount;
        private Button btnMZP2PStart;
        private Button btnMZP2PStop;
        private Label lbMZP2PStatus;
        private Panel panel3;
        private Label label10;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label11;
        private NumericUpDown numMZJog;
        private Button btnMZJogMinus;
        private Button btnMZJogPlus;
        private Panel panel4;
        private Label label12;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label13;
        private NumericUpDown numMZTarget;
        private RadioButton rMZAbs;
        private RadioButton rMZRel;
        private Button btnMZMove;
        private Button btnMZStop;
        private Button btnMZHome;
        private Button btnMZServo;
        private Panel panel5;
        private Label label14;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label15;
        private NumericUpDown numMZSpeed;
        private Label label16;
        private NumericUpDown numMZAcc;
        private Label label17;
        private NumericUpDown numMZDec;
        private Panel panel6;
        private Label label18;
        private Label lbMZPos;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label20;
        private FlowLayoutPanel flowLayoutPanel4;
        private Label chipMZServo;
        private Label chipMZInpos;
        private Label chipMZMove;
        private Panel panel7;
        private Panel panel8;
        private Label label24;
        private FlowLayoutPanel flowLayoutPanel5;
        private Label label25;
        private NumericUpDown numTP2PA;
        private Label label26;
        private NumericUpDown numTP2PB;
        private Label label27;
        private NumericUpDown numTP2PVel;
        private Label label28;
        private NumericUpDown numTP2PAcc;
        private Label label29;
        private NumericUpDown numTP2PDec;
        private Label label30;
        private NumericUpDown numTP2PDwell;
        private Label label31;
        private NumericUpDown numTP2PCount;
        private Button btnTP2PStart;
        private Button btnTP2PStop;
        private Label lbTP2PStatus;
        private Panel panel9;
        private Label label33;
        private FlowLayoutPanel flowLayoutPanel6;
        private Label label34;
        private NumericUpDown numTJog;
        private Button btnTJogMinus;
        private Button btnTJogPlus;
        private Panel panel10;
        private Label label35;
        private FlowLayoutPanel flowLayoutPanel7;
        private Label label36;
        private NumericUpDown numTTarget;
        private RadioButton rTAbs;
        private RadioButton rTRel;
        private Button btnTMove;
        private Button btnTStop;
        private Button btnTHome;
        private Button btnTServo;
        private Panel panel11;
        private Label label37;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label38;
        private NumericUpDown numTSpeed;
        private Label label39;
        private NumericUpDown numTAcc;
        private Label label40;
        private NumericUpDown numTDec;
        private Panel panel12;
        private Label label41;
        private Label lbTPos;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label43;
        private FlowLayoutPanel flowLayoutPanel8;
        private Label chipTServo;
        private Label chipTInpos;
        private Label chipTMove;
        private Label chipXHomeStatus;
        private Label chipYHomeStatus;
        private Label chipMZHomeStatus;
        private Label chipTHomeStatus;
        private Button btnMZAlarmReset;
        private Button btnTAlarmReset;
        private Label lblXtPos;
        private Label TargetXPos;
        private Label lblYtPos;
        private Label TargetYPos;
        private Label lblXAPos;
        private Label XAPos;
        private Label lblYAPos;
        private Label AYPos;
		private Button ZHomeParamButton;
    }
}