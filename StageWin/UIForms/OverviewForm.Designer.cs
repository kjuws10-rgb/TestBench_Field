using System.Windows.Forms;
using System.Drawing;
using StageWin.Etc;

namespace StageWin.UI
{
    public partial class OverviewForm
    {
        private System.ComponentModel.IContainer components = null;

        private GroupBox grpCommunications;
        private Label lblAcs;
        private Label lblAjin;
        private Label lblLds;
        private Label lblWago;

        private GroupBox grpAxes;
        private GroupBox grpInputs;
        private GroupBox grpOutputs;
        private GroupBox grpNotes;
        private Label lblNotes;

        private Timer tmrRefresh;

        private Panel panel1;
        private ListBox lstOverview;

        // ---- Motion (값 Label + LedLabel) ----
        private Label lblMainY_Pos, lblMainY_ActVel, lblMainY_CmdVel, lblMainY_Rms;
        private Label ledMainY_InPos, ledMainY_Servo, ledMainY_Alarm;

        private Label lblReviewX_Pos, lblReviewX_ActVel, lblReviewX_CmdVel, lblReviewX_Rms;
        private Label ledReviewX_InPos, ledReviewX_Servo, ledReviewX_Alarm;

        private Label lblMaintZ_Pos, lblMaintZ_ActVel, lblMaintZ_CmdVel, lblMaintZ_Rms;
        private Label ledMaintZ_InPos, ledMaintZ_Servo, ledMaintZ_AmpAlarm;

        private Label lblTheta_Pos, lblTheta_ActVel, lblTheta_CmdVel, lblTheta_Rms;
        private Label ledTheta_InPos, ledTheta_Servo, ledTheta_AmpAlarm;

        // ---- Inputs (LedLabel) ----
        private Label ledPmFan_Front1, ledPmFan_Front2, ledPmFan_Side1, ledPmFan_Side2, ledPmFan_Rear1, ledPmFan_Rear2;
        private Label ledUiFan_Front1, ledUiFan_Front2, ledUiFan_Rear1, ledUiFan_Rear2, ledUiFan_Side1, ledUiFan_Side2;

        private Label ledGripSwitchEms, ledPmRackEms, ledChamberEms1, ledChamberEms2, ledChamberEms3, ledModeKey;
        private Label ledDoorFront1, ledDoorFront2, ledDoorRear1, ledDoorRear2, ledDoorSide1, ledDoorSide2;

        private Label ledLaserStatus;
        private Label ledAcs1Main, ledAcs2Y, ledAcs2X, ledRot1Main, ledRot2Theta, ledRot2MaintZ12;

        // ---- Outputs (버튼 + 상태 Led) ----
        private Button btnFrontDoor1_Lock, btnFrontDoor1_Unlock;
        private Button btnFrontDoor2_Lock, btnFrontDoor2_Unlock;
        private Button btnRearDoor1_Lock, btnRearDoor1_Unlock;
        private Button btnRearDoor2_Lock, btnRearDoor2_Unlock;
        private Button btnSideDoor1_Lock, btnSideDoor1_Unlock;
        private Button btnSideDoor2_Lock, btnSideDoor2_Unlock;

        private Button btnCleanBoothLed_On, btnCleanBoothLed_Off;
        private Label ledFrontDoor1_LockState, ledFrontDoor2_LockState, ledRearDoor1_LockState, ledRearDoor2_LockState, ledSideDoor1_LockState, ledSideDoor2_LockState;
        private Label ledCleanBoothLed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpCommunications = new System.Windows.Forms.GroupBox();
            this.lblAcs = new System.Windows.Forms.Label();
            this.lblAjin = new System.Windows.Forms.Label();
            this.lblLds = new System.Windows.Forms.Label();
            this.lblWago = new System.Windows.Forms.Label();
            this.grpAxes = new System.Windows.Forms.GroupBox();
            this.lblHdrAxis = new System.Windows.Forms.Label();
            this.lblHdrPos = new System.Windows.Forms.Label();
            this.lblHdrAct = new System.Windows.Forms.Label();
            this.lblHdrCmd = new System.Windows.Forms.Label();
            this.lblHdrRms = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblHdrLed = new System.Windows.Forms.Label();
            this.lblRx = new System.Windows.Forms.Label();
            this.lblReviewX_Pos = new System.Windows.Forms.Label();
            this.lblReviewX_ActVel = new System.Windows.Forms.Label();
            this.lblReviewX_CmdVel = new System.Windows.Forms.Label();
            this.ledTheta_Homed = new System.Windows.Forms.Label();
            this.ledMaintZ_Homed = new System.Windows.Forms.Label();
            this.ledMainY_Homed = new System.Windows.Forms.Label();
            this.ledReviewX_Homed = new System.Windows.Forms.Label();
            this.lblReviewX_Rms = new System.Windows.Forms.Label();
            this.ledReviewX_Servo = new System.Windows.Forms.Label();
            this.ledReviewX_InPos = new System.Windows.Forms.Label();
            this.ledReviewX_Alarm = new System.Windows.Forms.Label();
            this.lblMy = new System.Windows.Forms.Label();
            this.lblMainY_Pos = new System.Windows.Forms.Label();
            this.lblMainY_ActVel = new System.Windows.Forms.Label();
            this.lblMainY_CmdVel = new System.Windows.Forms.Label();
            this.lblMainY_Rms = new System.Windows.Forms.Label();
            this.ledMainY_Servo = new System.Windows.Forms.Label();
            this.ledMainY_InPos = new System.Windows.Forms.Label();
            this.ledMainY_Alarm = new System.Windows.Forms.Label();
            this.lblMz = new System.Windows.Forms.Label();
            this.lblMaintZ_Pos = new System.Windows.Forms.Label();
            this.lblMaintZ_ActVel = new System.Windows.Forms.Label();
            this.lblMaintZ_CmdVel = new System.Windows.Forms.Label();
            this.lblMaintZ_Rms = new System.Windows.Forms.Label();
            this.ledMaintZ_Servo = new System.Windows.Forms.Label();
            this.ledMaintZ_InPos = new System.Windows.Forms.Label();
            this.ledMaintZ_AmpAlarm = new System.Windows.Forms.Label();
            this.lblTh = new System.Windows.Forms.Label();
            this.lblTheta_Pos = new System.Windows.Forms.Label();
            this.lblTheta_ActVel = new System.Windows.Forms.Label();
            this.lblTheta_CmdVel = new System.Windows.Forms.Label();
            this.lblTheta_Rms = new System.Windows.Forms.Label();
            this.ledTheta_Servo = new System.Windows.Forms.Label();
            this.ledTheta_InPos = new System.Windows.Forms.Label();
            this.ledTheta_AmpAlarm = new System.Windows.Forms.Label();
            this.grpInputs = new System.Windows.Forms.GroupBox();
            this.lblPmF1 = new System.Windows.Forms.Label();
            this.ledPmFan_Front1 = new System.Windows.Forms.Label();
            this.lblPmF2 = new System.Windows.Forms.Label();
            this.ledPmFan_Front2 = new System.Windows.Forms.Label();
            this.lblPmS1 = new System.Windows.Forms.Label();
            this.ledPmFan_Side1 = new System.Windows.Forms.Label();
            this.lblPmS2 = new System.Windows.Forms.Label();
            this.ledPmFan_Side2 = new System.Windows.Forms.Label();
            this.lblPmR1 = new System.Windows.Forms.Label();
            this.ledPmFan_Rear1 = new System.Windows.Forms.Label();
            this.lblPmR2 = new System.Windows.Forms.Label();
            this.ledPmFan_Rear2 = new System.Windows.Forms.Label();
            this.lblUiF1 = new System.Windows.Forms.Label();
            this.ledUiFan_Front1 = new System.Windows.Forms.Label();
            this.lblUiF2 = new System.Windows.Forms.Label();
            this.ledUiFan_Front2 = new System.Windows.Forms.Label();
            this.lblUiR1 = new System.Windows.Forms.Label();
            this.ledUiFan_Rear1 = new System.Windows.Forms.Label();
            this.lblUiR2 = new System.Windows.Forms.Label();
            this.ledUiFan_Rear2 = new System.Windows.Forms.Label();
            this.lblUiS1 = new System.Windows.Forms.Label();
            this.ledUiFan_Side1 = new System.Windows.Forms.Label();
            this.lblUiS2 = new System.Windows.Forms.Label();
            this.ledUiFan_Side2 = new System.Windows.Forms.Label();
            this.lblGrip = new System.Windows.Forms.Label();
            this.ledGripSwitchEms = new System.Windows.Forms.Label();
            this.lblPmEms = new System.Windows.Forms.Label();
            this.ledPmRackEms = new System.Windows.Forms.Label();
            this.lblCh1 = new System.Windows.Forms.Label();
            this.ledChamberEms1 = new System.Windows.Forms.Label();
            this.lblCh2 = new System.Windows.Forms.Label();
            this.ledChamberEms2 = new System.Windows.Forms.Label();
            this.lblCh3 = new System.Windows.Forms.Label();
            this.ledChamberEms3 = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.ledModeKey = new System.Windows.Forms.Label();
            this.lblDf1 = new System.Windows.Forms.Label();
            this.ledDoorFront1 = new System.Windows.Forms.Label();
            this.lblDf2 = new System.Windows.Forms.Label();
            this.ledDoorFront2 = new System.Windows.Forms.Label();
            this.lblDr1 = new System.Windows.Forms.Label();
            this.ledDoorRear1 = new System.Windows.Forms.Label();
            this.lblDr2 = new System.Windows.Forms.Label();
            this.ledDoorRear2 = new System.Windows.Forms.Label();
            this.lblDs1 = new System.Windows.Forms.Label();
            this.ledDoorSide1 = new System.Windows.Forms.Label();
            this.lblDs2 = new System.Windows.Forms.Label();
            this.ledDoorSide2 = new System.Windows.Forms.Label();
            this.lblLaser = new System.Windows.Forms.Label();
            this.ledLaserStatus = new System.Windows.Forms.Label();
            this.lblAcs1 = new System.Windows.Forms.Label();
            this.ledAcs1Main = new System.Windows.Forms.Label();
            this.lblAcs2Y = new System.Windows.Forms.Label();
            this.ledAcs2Y = new System.Windows.Forms.Label();
            this.lblAcs2X = new System.Windows.Forms.Label();
            this.ledAcs2X = new System.Windows.Forms.Label();
            this.lblRot1 = new System.Windows.Forms.Label();
            this.ledRot1Main = new System.Windows.Forms.Label();
            this.lblRot2 = new System.Windows.Forms.Label();
            this.ledRot2Theta = new System.Windows.Forms.Label();
            this.lblRot2M = new System.Windows.Forms.Label();
            this.ledRot2MaintZ12 = new System.Windows.Forms.Label();
            this.grpOutputs = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMc2ndZO = new System.Windows.Forms.Label();
            this.lblMc2ndXO = new System.Windows.Forms.Label();
            this.lblMc2ndYO = new System.Windows.Forms.Label();
            this.lblLaserBoxAirVv = new System.Windows.Forms.Label();
            this.lblY2MotorAirVv = new System.Windows.Forms.Label();
            this.lblXMotorAirVv = new System.Windows.Forms.Label();
            this.lblY1MotorAirVv = new System.Windows.Forms.Label();
            this.lblMc1stO = new System.Windows.Forms.Label();
            this.btnAjinMc2_OutputTOn = new System.Windows.Forms.Button();
            this.btnAjinMc2_OutputZOn = new System.Windows.Forms.Button();
            this.btnAjinMc2_OutputTOff = new System.Windows.Forms.Button();
            this.btnACSMc2_OutputXOn = new System.Windows.Forms.Button();
            this.btnAjinMc2_OutputZOff = new System.Windows.Forms.Button();
            this.btnACSMc2_OutputYOn = new System.Windows.Forms.Button();
            this.btnACSMc2_OutputXOff = new System.Windows.Forms.Button();
            this.lblO1 = new System.Windows.Forms.Label();
            this.ledAjinMc2_OutputTState = new System.Windows.Forms.Label();
            this.btnACSMc2_OutputYOff = new System.Windows.Forms.Button();
            this.btnLaserBox_AirCoolingVvOpen = new System.Windows.Forms.Button();
            this.btnY2Motor_AirCoolingVvOpen = new System.Windows.Forms.Button();
            this.ledAjinMc2_OutputZState = new System.Windows.Forms.Label();
            this.btnXMotor_AirCoolingVvOpen = new System.Windows.Forms.Button();
            this.btnLaserBox_AirCoolingVvClose = new System.Windows.Forms.Button();
            this.btnY1Motor_AirCoolingVvOpen = new System.Windows.Forms.Button();
            this.btnY2Motor_AirCoolingVvClose = new System.Windows.Forms.Button();
            this.btnXMotor_AirCoolingVvClose = new System.Windows.Forms.Button();
            this.btnACSMc1_OutputOn = new System.Windows.Forms.Button();
            this.btnY1Motor_AirCoolingVvClose = new System.Windows.Forms.Button();
            this.ledACSMc2_OutputXState = new System.Windows.Forms.Label();
            this.ledLaserBox_AirCoolingVvState = new System.Windows.Forms.Label();
            this.btnACSMc1_OutputOff = new System.Windows.Forms.Button();
            this.ledY2Motor_AirCoolingVvState = new System.Windows.Forms.Label();
            this.ledXMotor_AirCoolingVvState = new System.Windows.Forms.Label();
            this.ledACSMc2_OutputYState = new System.Windows.Forms.Label();
            this.ledY1Motor_AirCoolingVvState = new System.Windows.Forms.Label();
            this.btnFrontDoor1_Lock = new System.Windows.Forms.Button();
            this.ledACSMc1_OutputState = new System.Windows.Forms.Label();
            this.btnFrontDoor1_Unlock = new System.Windows.Forms.Button();
            this.ledFrontDoor1_LockState = new System.Windows.Forms.Label();
            this.lblO2 = new System.Windows.Forms.Label();
            this.btnFrontDoor2_Lock = new System.Windows.Forms.Button();
            this.btnFrontDoor2_Unlock = new System.Windows.Forms.Button();
            this.ledFrontDoor2_LockState = new System.Windows.Forms.Label();
            this.lblO3 = new System.Windows.Forms.Label();
            this.btnRearDoor1_Lock = new System.Windows.Forms.Button();
            this.btnRearDoor1_Unlock = new System.Windows.Forms.Button();
            this.ledRearDoor1_LockState = new System.Windows.Forms.Label();
            this.lblO4 = new System.Windows.Forms.Label();
            this.btnRearDoor2_Lock = new System.Windows.Forms.Button();
            this.btnRearDoor2_Unlock = new System.Windows.Forms.Button();
            this.ledRearDoor2_LockState = new System.Windows.Forms.Label();
            this.lblO5 = new System.Windows.Forms.Label();
            this.btnSideDoor1_Lock = new System.Windows.Forms.Button();
            this.btnSideDoor1_Unlock = new System.Windows.Forms.Button();
            this.ledSideDoor1_LockState = new System.Windows.Forms.Label();
            this.lblO6 = new System.Windows.Forms.Label();
            this.btnSideDoor2_Lock = new System.Windows.Forms.Button();
            this.btnSideDoor2_Unlock = new System.Windows.Forms.Button();
            this.ledSideDoor2_LockState = new System.Windows.Forms.Label();
            this.lblO7 = new System.Windows.Forms.Label();
            this.btnCleanBoothLed_On = new System.Windows.Forms.Button();
            this.btnCleanBoothLed_Off = new System.Windows.Forms.Button();
            this.ledCleanBoothLed = new System.Windows.Forms.Label();
            this.grpNotes = new System.Windows.Forms.GroupBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lstOverview = new System.Windows.Forms.ListBox();
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.grpInterlockParam = new System.Windows.Forms.GroupBox();
            this.lblItkEmsAllOk = new System.Windows.Forms.Label();
            this.ledItkEmsAllOk = new System.Windows.Forms.Label();
            this.lblItkAjinMcAllOk = new System.Windows.Forms.Label();
            this.ledItkAjinMcAllOk = new System.Windows.Forms.Label();
            this.lblItkAcsMcAllOk = new System.Windows.Forms.Label();
            this.ledItkAcsMcAllOk = new System.Windows.Forms.Label();
            this.lblItkStageMoveOk = new System.Windows.Forms.Label();
            this.ledItkStageMoveOk = new System.Windows.Forms.Label();
            this.lblItkLaserReady = new System.Windows.Forms.Label();
            this.ledItkLaserReady = new System.Windows.Forms.Label();
            this.lblItkModeKey = new System.Windows.Forms.Label();
            this.ledItkModeKey = new System.Windows.Forms.Label();
            this.grpLogs = new System.Windows.Forms.GroupBox();
            this.grpCommunications.SuspendLayout();
            this.grpAxes.SuspendLayout();
            this.grpInputs.SuspendLayout();
            this.grpOutputs.SuspendLayout();
            this.grpNotes.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpInterlockParam.SuspendLayout();
            this.grpLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCommunications
            // 
            this.grpCommunications.Controls.Add(this.lblAcs);
            this.grpCommunications.Controls.Add(this.lblAjin);
            this.grpCommunications.Controls.Add(this.lblLds);
            this.grpCommunications.Controls.Add(this.lblWago);
            this.grpCommunications.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpCommunications.Location = new System.Drawing.Point(12, 12);
            this.grpCommunications.Name = "grpCommunications";
            this.grpCommunications.Size = new System.Drawing.Size(720, 110);
            this.grpCommunications.TabIndex = 0;
            this.grpCommunications.TabStop = false;
            this.grpCommunications.Text = "Communications";
            // 
            // lblAcs
            // 
            this.lblAcs.BackColor = System.Drawing.Color.LightGray;
            this.lblAcs.Location = new System.Drawing.Point(18, 28);
            this.lblAcs.Name = "lblAcs";
            this.lblAcs.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblAcs.Size = new System.Drawing.Size(330, 26);
            this.lblAcs.TabIndex = 0;
            this.lblAcs.Text = "ACS Motion: -";
            this.lblAcs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAjin
            // 
            this.lblAjin.BackColor = System.Drawing.Color.LightGray;
            this.lblAjin.Location = new System.Drawing.Point(366, 28);
            this.lblAjin.Name = "lblAjin";
            this.lblAjin.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblAjin.Size = new System.Drawing.Size(330, 26);
            this.lblAjin.TabIndex = 1;
            this.lblAjin.Text = "Ajin Motion: -";
            this.lblAjin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLds
            // 
            this.lblLds.BackColor = System.Drawing.Color.LightGray;
            this.lblLds.Location = new System.Drawing.Point(18, 66);
            this.lblLds.Name = "lblLds";
            this.lblLds.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblLds.Size = new System.Drawing.Size(330, 26);
            this.lblLds.TabIndex = 2;
            this.lblLds.Text = "LDS: -";
            this.lblLds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWago
            // 
            this.lblWago.BackColor = System.Drawing.Color.LightGray;
            this.lblWago.Location = new System.Drawing.Point(366, 66);
            this.lblWago.Name = "lblWago";
            this.lblWago.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblWago.Size = new System.Drawing.Size(330, 26);
            this.lblWago.TabIndex = 3;
            this.lblWago.Text = "WAGO IO: -";
            this.lblWago.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpAxes
            // 
            this.grpAxes.Controls.Add(this.lblHdrAxis);
            this.grpAxes.Controls.Add(this.lblHdrPos);
            this.grpAxes.Controls.Add(this.lblHdrAct);
            this.grpAxes.Controls.Add(this.lblHdrCmd);
            this.grpAxes.Controls.Add(this.lblHdrRms);
            this.grpAxes.Controls.Add(this.label3);
            this.grpAxes.Controls.Add(this.lblHdrLed);
            this.grpAxes.Controls.Add(this.lblRx);
            this.grpAxes.Controls.Add(this.lblReviewX_Pos);
            this.grpAxes.Controls.Add(this.lblReviewX_ActVel);
            this.grpAxes.Controls.Add(this.lblReviewX_CmdVel);
            this.grpAxes.Controls.Add(this.ledTheta_Homed);
            this.grpAxes.Controls.Add(this.ledMaintZ_Homed);
            this.grpAxes.Controls.Add(this.ledMainY_Homed);
            this.grpAxes.Controls.Add(this.ledReviewX_Homed);
            this.grpAxes.Controls.Add(this.lblReviewX_Rms);
            this.grpAxes.Controls.Add(this.ledReviewX_Servo);
            this.grpAxes.Controls.Add(this.ledReviewX_InPos);
            this.grpAxes.Controls.Add(this.ledReviewX_Alarm);
            this.grpAxes.Controls.Add(this.lblMy);
            this.grpAxes.Controls.Add(this.lblMainY_Pos);
            this.grpAxes.Controls.Add(this.lblMainY_ActVel);
            this.grpAxes.Controls.Add(this.lblMainY_CmdVel);
            this.grpAxes.Controls.Add(this.lblMainY_Rms);
            this.grpAxes.Controls.Add(this.ledMainY_Servo);
            this.grpAxes.Controls.Add(this.ledMainY_InPos);
            this.grpAxes.Controls.Add(this.ledMainY_Alarm);
            this.grpAxes.Controls.Add(this.lblMz);
            this.grpAxes.Controls.Add(this.lblMaintZ_Pos);
            this.grpAxes.Controls.Add(this.lblMaintZ_ActVel);
            this.grpAxes.Controls.Add(this.lblMaintZ_CmdVel);
            this.grpAxes.Controls.Add(this.lblMaintZ_Rms);
            this.grpAxes.Controls.Add(this.ledMaintZ_Servo);
            this.grpAxes.Controls.Add(this.ledMaintZ_InPos);
            this.grpAxes.Controls.Add(this.ledMaintZ_AmpAlarm);
            this.grpAxes.Controls.Add(this.lblTh);
            this.grpAxes.Controls.Add(this.lblTheta_Pos);
            this.grpAxes.Controls.Add(this.lblTheta_ActVel);
            this.grpAxes.Controls.Add(this.lblTheta_CmdVel);
            this.grpAxes.Controls.Add(this.lblTheta_Rms);
            this.grpAxes.Controls.Add(this.ledTheta_Servo);
            this.grpAxes.Controls.Add(this.ledTheta_InPos);
            this.grpAxes.Controls.Add(this.ledTheta_AmpAlarm);
            this.grpAxes.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpAxes.Location = new System.Drawing.Point(12, 128);
            this.grpAxes.Name = "grpAxes";
            this.grpAxes.Size = new System.Drawing.Size(720, 147);
            this.grpAxes.TabIndex = 1;
            this.grpAxes.TabStop = false;
            this.grpAxes.Text = "Axes";
            // 
            // lblHdrAxis
            // 
            this.lblHdrAxis.Location = new System.Drawing.Point(14, 24);
            this.lblHdrAxis.Name = "lblHdrAxis";
            this.lblHdrAxis.Size = new System.Drawing.Size(160, 18);
            this.lblHdrAxis.TabIndex = 0;
            this.lblHdrAxis.Text = "Axis";
            this.lblHdrAxis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHdrPos
            // 
            this.lblHdrPos.Location = new System.Drawing.Point(233, 24);
            this.lblHdrPos.Name = "lblHdrPos";
            this.lblHdrPos.Size = new System.Drawing.Size(63, 18);
            this.lblHdrPos.TabIndex = 1;
            this.lblHdrPos.Text = "Pos";
            this.lblHdrPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHdrAct
            // 
            this.lblHdrAct.Location = new System.Drawing.Point(319, 24);
            this.lblHdrAct.Name = "lblHdrAct";
            this.lblHdrAct.Size = new System.Drawing.Size(80, 18);
            this.lblHdrAct.TabIndex = 2;
            this.lblHdrAct.Text = "ActVel";
            this.lblHdrAct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHdrCmd
            // 
            this.lblHdrCmd.Location = new System.Drawing.Point(409, 24);
            this.lblHdrCmd.Name = "lblHdrCmd";
            this.lblHdrCmd.Size = new System.Drawing.Size(80, 18);
            this.lblHdrCmd.TabIndex = 3;
            this.lblHdrCmd.Text = "CmdVel";
            this.lblHdrCmd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHdrRms
            // 
            this.lblHdrRms.Location = new System.Drawing.Point(499, 24);
            this.lblHdrRms.Name = "lblHdrRms";
            this.lblHdrRms.Size = new System.Drawing.Size(80, 18);
            this.lblHdrRms.TabIndex = 4;
            this.lblHdrRms.Text = "RMS";
            this.lblHdrRms.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(174, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Homed";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHdrLed
            // 
            this.lblHdrLed.Location = new System.Drawing.Point(578, 24);
            this.lblHdrLed.Name = "lblHdrLed";
            this.lblHdrLed.Size = new System.Drawing.Size(125, 18);
            this.lblHdrLed.TabIndex = 5;
            this.lblHdrLed.Text = "Servo/InPos/Alarm";
            this.lblHdrLed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRx
            // 
            this.lblRx.Location = new System.Drawing.Point(14, 46);
            this.lblRx.Name = "lblRx";
            this.lblRx.Size = new System.Drawing.Size(135, 18);
            this.lblRx.TabIndex = 6;
            this.lblRx.Text = "Review X";
            this.lblRx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblReviewX_Pos
            // 
            this.lblReviewX_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReviewX_Pos.Location = new System.Drawing.Point(229, 46);
            this.lblReviewX_Pos.Name = "lblReviewX_Pos";
            this.lblReviewX_Pos.Size = new System.Drawing.Size(80, 18);
            this.lblReviewX_Pos.TabIndex = 7;
            this.lblReviewX_Pos.Text = "0.000";
            this.lblReviewX_Pos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReviewX_ActVel
            // 
            this.lblReviewX_ActVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReviewX_ActVel.Location = new System.Drawing.Point(319, 46);
            this.lblReviewX_ActVel.Name = "lblReviewX_ActVel";
            this.lblReviewX_ActVel.Size = new System.Drawing.Size(80, 18);
            this.lblReviewX_ActVel.TabIndex = 8;
            this.lblReviewX_ActVel.Text = "0.000";
            this.lblReviewX_ActVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReviewX_CmdVel
            // 
            this.lblReviewX_CmdVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReviewX_CmdVel.Location = new System.Drawing.Point(409, 46);
            this.lblReviewX_CmdVel.Name = "lblReviewX_CmdVel";
            this.lblReviewX_CmdVel.Size = new System.Drawing.Size(80, 18);
            this.lblReviewX_CmdVel.TabIndex = 9;
            this.lblReviewX_CmdVel.Text = "0.000";
            this.lblReviewX_CmdVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ledTheta_Homed
            // 
            this.ledTheta_Homed.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledTheta_Homed.Location = new System.Drawing.Point(193, 112);
            this.ledTheta_Homed.Name = "ledTheta_Homed";
            this.ledTheta_Homed.Size = new System.Drawing.Size(18, 18);
            this.ledTheta_Homed.TabIndex = 11;
            // 
            // ledMaintZ_Homed
            // 
            this.ledMaintZ_Homed.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMaintZ_Homed.Location = new System.Drawing.Point(193, 90);
            this.ledMaintZ_Homed.Name = "ledMaintZ_Homed";
            this.ledMaintZ_Homed.Size = new System.Drawing.Size(18, 18);
            this.ledMaintZ_Homed.TabIndex = 11;
            // 
            // ledMainY_Homed
            // 
            this.ledMainY_Homed.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMainY_Homed.Location = new System.Drawing.Point(193, 68);
            this.ledMainY_Homed.Name = "ledMainY_Homed";
            this.ledMainY_Homed.Size = new System.Drawing.Size(18, 18);
            this.ledMainY_Homed.TabIndex = 11;
            // 
            // ledReviewX_Homed
            // 
            this.ledReviewX_Homed.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledReviewX_Homed.Location = new System.Drawing.Point(193, 46);
            this.ledReviewX_Homed.Name = "ledReviewX_Homed";
            this.ledReviewX_Homed.Size = new System.Drawing.Size(18, 18);
            this.ledReviewX_Homed.TabIndex = 11;
            // 
            // lblReviewX_Rms
            // 
            this.lblReviewX_Rms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReviewX_Rms.Location = new System.Drawing.Point(499, 46);
            this.lblReviewX_Rms.Name = "lblReviewX_Rms";
            this.lblReviewX_Rms.Size = new System.Drawing.Size(80, 18);
            this.lblReviewX_Rms.TabIndex = 10;
            this.lblReviewX_Rms.Text = "0.000";
            this.lblReviewX_Rms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ledReviewX_Servo
            // 
            this.ledReviewX_Servo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledReviewX_Servo.Location = new System.Drawing.Point(599, 46);
            this.ledReviewX_Servo.Name = "ledReviewX_Servo";
            this.ledReviewX_Servo.Size = new System.Drawing.Size(18, 18);
            this.ledReviewX_Servo.TabIndex = 11;
            // 
            // ledReviewX_InPos
            // 
            this.ledReviewX_InPos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledReviewX_InPos.Location = new System.Drawing.Point(621, 46);
            this.ledReviewX_InPos.Name = "ledReviewX_InPos";
            this.ledReviewX_InPos.Size = new System.Drawing.Size(18, 18);
            this.ledReviewX_InPos.TabIndex = 12;
            // 
            // ledReviewX_Alarm
            // 
            this.ledReviewX_Alarm.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledReviewX_Alarm.Location = new System.Drawing.Point(643, 46);
            this.ledReviewX_Alarm.Name = "ledReviewX_Alarm";
            this.ledReviewX_Alarm.Size = new System.Drawing.Size(18, 18);
            this.ledReviewX_Alarm.TabIndex = 13;
            // 
            // lblMy
            // 
            this.lblMy.Location = new System.Drawing.Point(14, 68);
            this.lblMy.Name = "lblMy";
            this.lblMy.Size = new System.Drawing.Size(135, 18);
            this.lblMy.TabIndex = 14;
            this.lblMy.Text = "Main Y";
            this.lblMy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMainY_Pos
            // 
            this.lblMainY_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMainY_Pos.Location = new System.Drawing.Point(229, 68);
            this.lblMainY_Pos.Name = "lblMainY_Pos";
            this.lblMainY_Pos.Size = new System.Drawing.Size(80, 18);
            this.lblMainY_Pos.TabIndex = 15;
            this.lblMainY_Pos.Text = "0.000";
            this.lblMainY_Pos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMainY_ActVel
            // 
            this.lblMainY_ActVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMainY_ActVel.Location = new System.Drawing.Point(319, 68);
            this.lblMainY_ActVel.Name = "lblMainY_ActVel";
            this.lblMainY_ActVel.Size = new System.Drawing.Size(80, 18);
            this.lblMainY_ActVel.TabIndex = 16;
            this.lblMainY_ActVel.Text = "0.000";
            this.lblMainY_ActVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMainY_CmdVel
            // 
            this.lblMainY_CmdVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMainY_CmdVel.Location = new System.Drawing.Point(409, 68);
            this.lblMainY_CmdVel.Name = "lblMainY_CmdVel";
            this.lblMainY_CmdVel.Size = new System.Drawing.Size(80, 18);
            this.lblMainY_CmdVel.TabIndex = 17;
            this.lblMainY_CmdVel.Text = "0.000";
            this.lblMainY_CmdVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMainY_Rms
            // 
            this.lblMainY_Rms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMainY_Rms.Location = new System.Drawing.Point(499, 68);
            this.lblMainY_Rms.Name = "lblMainY_Rms";
            this.lblMainY_Rms.Size = new System.Drawing.Size(80, 18);
            this.lblMainY_Rms.TabIndex = 18;
            this.lblMainY_Rms.Text = "0.000";
            this.lblMainY_Rms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ledMainY_Servo
            // 
            this.ledMainY_Servo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMainY_Servo.Location = new System.Drawing.Point(599, 68);
            this.ledMainY_Servo.Name = "ledMainY_Servo";
            this.ledMainY_Servo.Size = new System.Drawing.Size(18, 18);
            this.ledMainY_Servo.TabIndex = 19;
            // 
            // ledMainY_InPos
            // 
            this.ledMainY_InPos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMainY_InPos.Location = new System.Drawing.Point(621, 68);
            this.ledMainY_InPos.Name = "ledMainY_InPos";
            this.ledMainY_InPos.Size = new System.Drawing.Size(18, 18);
            this.ledMainY_InPos.TabIndex = 20;
            // 
            // ledMainY_Alarm
            // 
            this.ledMainY_Alarm.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMainY_Alarm.Location = new System.Drawing.Point(643, 68);
            this.ledMainY_Alarm.Name = "ledMainY_Alarm";
            this.ledMainY_Alarm.Size = new System.Drawing.Size(18, 18);
            this.ledMainY_Alarm.TabIndex = 21;
            // 
            // lblMz
            // 
            this.lblMz.Location = new System.Drawing.Point(14, 90);
            this.lblMz.Name = "lblMz";
            this.lblMz.Size = new System.Drawing.Size(135, 18);
            this.lblMz.TabIndex = 22;
            this.lblMz.Text = "Maint Z";
            this.lblMz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMaintZ_Pos
            // 
            this.lblMaintZ_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMaintZ_Pos.Location = new System.Drawing.Point(229, 90);
            this.lblMaintZ_Pos.Name = "lblMaintZ_Pos";
            this.lblMaintZ_Pos.Size = new System.Drawing.Size(80, 18);
            this.lblMaintZ_Pos.TabIndex = 23;
            this.lblMaintZ_Pos.Text = "0.000";
            this.lblMaintZ_Pos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMaintZ_ActVel
            // 
            this.lblMaintZ_ActVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMaintZ_ActVel.Location = new System.Drawing.Point(319, 90);
            this.lblMaintZ_ActVel.Name = "lblMaintZ_ActVel";
            this.lblMaintZ_ActVel.Size = new System.Drawing.Size(80, 18);
            this.lblMaintZ_ActVel.TabIndex = 24;
            this.lblMaintZ_ActVel.Text = "0.000";
            this.lblMaintZ_ActVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMaintZ_CmdVel
            // 
            this.lblMaintZ_CmdVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMaintZ_CmdVel.Location = new System.Drawing.Point(409, 90);
            this.lblMaintZ_CmdVel.Name = "lblMaintZ_CmdVel";
            this.lblMaintZ_CmdVel.Size = new System.Drawing.Size(80, 18);
            this.lblMaintZ_CmdVel.TabIndex = 25;
            this.lblMaintZ_CmdVel.Text = "0.000";
            this.lblMaintZ_CmdVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMaintZ_Rms
            // 
            this.lblMaintZ_Rms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMaintZ_Rms.Location = new System.Drawing.Point(499, 90);
            this.lblMaintZ_Rms.Name = "lblMaintZ_Rms";
            this.lblMaintZ_Rms.Size = new System.Drawing.Size(80, 18);
            this.lblMaintZ_Rms.TabIndex = 26;
            this.lblMaintZ_Rms.Text = "0.000";
            this.lblMaintZ_Rms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ledMaintZ_Servo
            // 
            this.ledMaintZ_Servo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMaintZ_Servo.Location = new System.Drawing.Point(599, 90);
            this.ledMaintZ_Servo.Name = "ledMaintZ_Servo";
            this.ledMaintZ_Servo.Size = new System.Drawing.Size(18, 18);
            this.ledMaintZ_Servo.TabIndex = 27;
            // 
            // ledMaintZ_InPos
            // 
            this.ledMaintZ_InPos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMaintZ_InPos.Location = new System.Drawing.Point(621, 90);
            this.ledMaintZ_InPos.Name = "ledMaintZ_InPos";
            this.ledMaintZ_InPos.Size = new System.Drawing.Size(18, 18);
            this.ledMaintZ_InPos.TabIndex = 28;
            // 
            // ledMaintZ_AmpAlarm
            // 
            this.ledMaintZ_AmpAlarm.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledMaintZ_AmpAlarm.Location = new System.Drawing.Point(643, 90);
            this.ledMaintZ_AmpAlarm.Name = "ledMaintZ_AmpAlarm";
            this.ledMaintZ_AmpAlarm.Size = new System.Drawing.Size(18, 18);
            this.ledMaintZ_AmpAlarm.TabIndex = 29;
            // 
            // lblTh
            // 
            this.lblTh.Location = new System.Drawing.Point(14, 112);
            this.lblTh.Name = "lblTh";
            this.lblTh.Size = new System.Drawing.Size(135, 18);
            this.lblTh.TabIndex = 30;
            this.lblTh.Text = "Theta";
            this.lblTh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTheta_Pos
            // 
            this.lblTheta_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTheta_Pos.Location = new System.Drawing.Point(229, 112);
            this.lblTheta_Pos.Name = "lblTheta_Pos";
            this.lblTheta_Pos.Size = new System.Drawing.Size(80, 18);
            this.lblTheta_Pos.TabIndex = 31;
            this.lblTheta_Pos.Text = "0.00000";
            this.lblTheta_Pos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTheta_ActVel
            // 
            this.lblTheta_ActVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTheta_ActVel.Location = new System.Drawing.Point(319, 112);
            this.lblTheta_ActVel.Name = "lblTheta_ActVel";
            this.lblTheta_ActVel.Size = new System.Drawing.Size(80, 18);
            this.lblTheta_ActVel.TabIndex = 32;
            this.lblTheta_ActVel.Text = "0.000";
            this.lblTheta_ActVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTheta_CmdVel
            // 
            this.lblTheta_CmdVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTheta_CmdVel.Location = new System.Drawing.Point(409, 112);
            this.lblTheta_CmdVel.Name = "lblTheta_CmdVel";
            this.lblTheta_CmdVel.Size = new System.Drawing.Size(80, 18);
            this.lblTheta_CmdVel.TabIndex = 33;
            this.lblTheta_CmdVel.Text = "0.000";
            this.lblTheta_CmdVel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTheta_Rms
            // 
            this.lblTheta_Rms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTheta_Rms.Location = new System.Drawing.Point(499, 112);
            this.lblTheta_Rms.Name = "lblTheta_Rms";
            this.lblTheta_Rms.Size = new System.Drawing.Size(80, 18);
            this.lblTheta_Rms.TabIndex = 34;
            this.lblTheta_Rms.Text = "0.000";
            this.lblTheta_Rms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ledTheta_Servo
            // 
            this.ledTheta_Servo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledTheta_Servo.Location = new System.Drawing.Point(599, 112);
            this.ledTheta_Servo.Name = "ledTheta_Servo";
            this.ledTheta_Servo.Size = new System.Drawing.Size(18, 18);
            this.ledTheta_Servo.TabIndex = 35;
            // 
            // ledTheta_InPos
            // 
            this.ledTheta_InPos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledTheta_InPos.Location = new System.Drawing.Point(621, 112);
            this.ledTheta_InPos.Name = "ledTheta_InPos";
            this.ledTheta_InPos.Size = new System.Drawing.Size(18, 18);
            this.ledTheta_InPos.TabIndex = 36;
            // 
            // ledTheta_AmpAlarm
            // 
            this.ledTheta_AmpAlarm.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledTheta_AmpAlarm.Location = new System.Drawing.Point(643, 112);
            this.ledTheta_AmpAlarm.Name = "ledTheta_AmpAlarm";
            this.ledTheta_AmpAlarm.Size = new System.Drawing.Size(18, 18);
            this.ledTheta_AmpAlarm.TabIndex = 37;
            // 
            // grpInputs
            // 
            this.grpInputs.Controls.Add(this.lblPmF1);
            this.grpInputs.Controls.Add(this.ledPmFan_Front1);
            this.grpInputs.Controls.Add(this.lblPmF2);
            this.grpInputs.Controls.Add(this.ledPmFan_Front2);
            this.grpInputs.Controls.Add(this.lblPmS1);
            this.grpInputs.Controls.Add(this.ledPmFan_Side1);
            this.grpInputs.Controls.Add(this.lblPmS2);
            this.grpInputs.Controls.Add(this.ledPmFan_Side2);
            this.grpInputs.Controls.Add(this.lblPmR1);
            this.grpInputs.Controls.Add(this.ledPmFan_Rear1);
            this.grpInputs.Controls.Add(this.lblPmR2);
            this.grpInputs.Controls.Add(this.ledPmFan_Rear2);
            this.grpInputs.Controls.Add(this.lblUiF1);
            this.grpInputs.Controls.Add(this.ledUiFan_Front1);
            this.grpInputs.Controls.Add(this.lblUiF2);
            this.grpInputs.Controls.Add(this.ledUiFan_Front2);
            this.grpInputs.Controls.Add(this.lblUiR1);
            this.grpInputs.Controls.Add(this.ledUiFan_Rear1);
            this.grpInputs.Controls.Add(this.lblUiR2);
            this.grpInputs.Controls.Add(this.ledUiFan_Rear2);
            this.grpInputs.Controls.Add(this.lblUiS1);
            this.grpInputs.Controls.Add(this.ledUiFan_Side1);
            this.grpInputs.Controls.Add(this.lblUiS2);
            this.grpInputs.Controls.Add(this.ledUiFan_Side2);
            this.grpInputs.Controls.Add(this.lblGrip);
            this.grpInputs.Controls.Add(this.ledGripSwitchEms);
            this.grpInputs.Controls.Add(this.lblPmEms);
            this.grpInputs.Controls.Add(this.ledPmRackEms);
            this.grpInputs.Controls.Add(this.lblCh1);
            this.grpInputs.Controls.Add(this.ledChamberEms1);
            this.grpInputs.Controls.Add(this.lblCh2);
            this.grpInputs.Controls.Add(this.ledChamberEms2);
            this.grpInputs.Controls.Add(this.lblCh3);
            this.grpInputs.Controls.Add(this.ledChamberEms3);
            this.grpInputs.Controls.Add(this.lblMode);
            this.grpInputs.Controls.Add(this.ledModeKey);
            this.grpInputs.Controls.Add(this.lblDf1);
            this.grpInputs.Controls.Add(this.ledDoorFront1);
            this.grpInputs.Controls.Add(this.lblDf2);
            this.grpInputs.Controls.Add(this.ledDoorFront2);
            this.grpInputs.Controls.Add(this.lblDr1);
            this.grpInputs.Controls.Add(this.ledDoorRear1);
            this.grpInputs.Controls.Add(this.lblDr2);
            this.grpInputs.Controls.Add(this.ledDoorRear2);
            this.grpInputs.Controls.Add(this.lblDs1);
            this.grpInputs.Controls.Add(this.ledDoorSide1);
            this.grpInputs.Controls.Add(this.lblDs2);
            this.grpInputs.Controls.Add(this.ledDoorSide2);
            this.grpInputs.Controls.Add(this.lblLaser);
            this.grpInputs.Controls.Add(this.ledLaserStatus);
            this.grpInputs.Controls.Add(this.lblAcs1);
            this.grpInputs.Controls.Add(this.ledAcs1Main);
            this.grpInputs.Controls.Add(this.lblAcs2Y);
            this.grpInputs.Controls.Add(this.ledAcs2Y);
            this.grpInputs.Controls.Add(this.lblAcs2X);
            this.grpInputs.Controls.Add(this.ledAcs2X);
            this.grpInputs.Controls.Add(this.lblRot1);
            this.grpInputs.Controls.Add(this.ledRot1Main);
            this.grpInputs.Controls.Add(this.lblRot2);
            this.grpInputs.Controls.Add(this.ledRot2Theta);
            this.grpInputs.Controls.Add(this.lblRot2M);
            this.grpInputs.Controls.Add(this.ledRot2MaintZ12);
            this.grpInputs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpInputs.Location = new System.Drawing.Point(12, 314);
            this.grpInputs.Name = "grpInputs";
            this.grpInputs.Size = new System.Drawing.Size(720, 256);
            this.grpInputs.TabIndex = 2;
            this.grpInputs.TabStop = false;
            this.grpInputs.Text = "Inputs";
            // 
            // lblPmF1
            // 
            this.lblPmF1.Location = new System.Drawing.Point(14, 24);
            this.lblPmF1.Name = "lblPmF1";
            this.lblPmF1.Size = new System.Drawing.Size(170, 18);
            this.lblPmF1.TabIndex = 0;
            this.lblPmF1.Text = "PM Fan Front1";
            this.lblPmF1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Front1
            // 
            this.ledPmFan_Front1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Front1.Location = new System.Drawing.Point(194, 23);
            this.ledPmFan_Front1.Name = "ledPmFan_Front1";
            this.ledPmFan_Front1.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Front1.TabIndex = 1;
            // 
            // lblPmF2
            // 
            this.lblPmF2.Location = new System.Drawing.Point(244, 24);
            this.lblPmF2.Name = "lblPmF2";
            this.lblPmF2.Size = new System.Drawing.Size(170, 18);
            this.lblPmF2.TabIndex = 2;
            this.lblPmF2.Text = "PM Fan Front2";
            this.lblPmF2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Front2
            // 
            this.ledPmFan_Front2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Front2.Location = new System.Drawing.Point(424, 23);
            this.ledPmFan_Front2.Name = "ledPmFan_Front2";
            this.ledPmFan_Front2.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Front2.TabIndex = 3;
            // 
            // lblPmS1
            // 
            this.lblPmS1.Location = new System.Drawing.Point(474, 24);
            this.lblPmS1.Name = "lblPmS1";
            this.lblPmS1.Size = new System.Drawing.Size(170, 18);
            this.lblPmS1.TabIndex = 4;
            this.lblPmS1.Text = "PM Fan Side1";
            this.lblPmS1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Side1
            // 
            this.ledPmFan_Side1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Side1.Location = new System.Drawing.Point(654, 23);
            this.ledPmFan_Side1.Name = "ledPmFan_Side1";
            this.ledPmFan_Side1.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Side1.TabIndex = 5;
            // 
            // lblPmS2
            // 
            this.lblPmS2.Location = new System.Drawing.Point(14, 44);
            this.lblPmS2.Name = "lblPmS2";
            this.lblPmS2.Size = new System.Drawing.Size(170, 18);
            this.lblPmS2.TabIndex = 6;
            this.lblPmS2.Text = "PM Fan Side2";
            this.lblPmS2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Side2
            // 
            this.ledPmFan_Side2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Side2.Location = new System.Drawing.Point(194, 43);
            this.ledPmFan_Side2.Name = "ledPmFan_Side2";
            this.ledPmFan_Side2.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Side2.TabIndex = 7;
            // 
            // lblPmR1
            // 
            this.lblPmR1.Location = new System.Drawing.Point(244, 44);
            this.lblPmR1.Name = "lblPmR1";
            this.lblPmR1.Size = new System.Drawing.Size(170, 18);
            this.lblPmR1.TabIndex = 8;
            this.lblPmR1.Text = "PM Fan Rear1";
            this.lblPmR1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Rear1
            // 
            this.ledPmFan_Rear1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Rear1.Location = new System.Drawing.Point(424, 43);
            this.ledPmFan_Rear1.Name = "ledPmFan_Rear1";
            this.ledPmFan_Rear1.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Rear1.TabIndex = 9;
            // 
            // lblPmR2
            // 
            this.lblPmR2.Location = new System.Drawing.Point(474, 44);
            this.lblPmR2.Name = "lblPmR2";
            this.lblPmR2.Size = new System.Drawing.Size(170, 18);
            this.lblPmR2.TabIndex = 10;
            this.lblPmR2.Text = "PM Fan Rear2";
            this.lblPmR2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmFan_Rear2
            // 
            this.ledPmFan_Rear2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmFan_Rear2.Location = new System.Drawing.Point(654, 43);
            this.ledPmFan_Rear2.Name = "ledPmFan_Rear2";
            this.ledPmFan_Rear2.Size = new System.Drawing.Size(18, 18);
            this.ledPmFan_Rear2.TabIndex = 11;
            // 
            // lblUiF1
            // 
            this.lblUiF1.Location = new System.Drawing.Point(14, 64);
            this.lblUiF1.Name = "lblUiF1";
            this.lblUiF1.Size = new System.Drawing.Size(170, 18);
            this.lblUiF1.TabIndex = 12;
            this.lblUiF1.Text = "UI Fan Front1";
            this.lblUiF1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Front1
            // 
            this.ledUiFan_Front1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Front1.Location = new System.Drawing.Point(194, 63);
            this.ledUiFan_Front1.Name = "ledUiFan_Front1";
            this.ledUiFan_Front1.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Front1.TabIndex = 13;
            // 
            // lblUiF2
            // 
            this.lblUiF2.Location = new System.Drawing.Point(244, 64);
            this.lblUiF2.Name = "lblUiF2";
            this.lblUiF2.Size = new System.Drawing.Size(170, 18);
            this.lblUiF2.TabIndex = 14;
            this.lblUiF2.Text = "UI Fan Front2";
            this.lblUiF2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Front2
            // 
            this.ledUiFan_Front2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Front2.Location = new System.Drawing.Point(424, 63);
            this.ledUiFan_Front2.Name = "ledUiFan_Front2";
            this.ledUiFan_Front2.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Front2.TabIndex = 15;
            // 
            // lblUiR1
            // 
            this.lblUiR1.Location = new System.Drawing.Point(474, 64);
            this.lblUiR1.Name = "lblUiR1";
            this.lblUiR1.Size = new System.Drawing.Size(170, 18);
            this.lblUiR1.TabIndex = 16;
            this.lblUiR1.Text = "UI Fan Rear1";
            this.lblUiR1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Rear1
            // 
            this.ledUiFan_Rear1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Rear1.Location = new System.Drawing.Point(654, 63);
            this.ledUiFan_Rear1.Name = "ledUiFan_Rear1";
            this.ledUiFan_Rear1.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Rear1.TabIndex = 17;
            // 
            // lblUiR2
            // 
            this.lblUiR2.Location = new System.Drawing.Point(14, 84);
            this.lblUiR2.Name = "lblUiR2";
            this.lblUiR2.Size = new System.Drawing.Size(170, 18);
            this.lblUiR2.TabIndex = 18;
            this.lblUiR2.Text = "UI Fan Rear2";
            this.lblUiR2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Rear2
            // 
            this.ledUiFan_Rear2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Rear2.Location = new System.Drawing.Point(194, 83);
            this.ledUiFan_Rear2.Name = "ledUiFan_Rear2";
            this.ledUiFan_Rear2.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Rear2.TabIndex = 19;
            // 
            // lblUiS1
            // 
            this.lblUiS1.Location = new System.Drawing.Point(244, 84);
            this.lblUiS1.Name = "lblUiS1";
            this.lblUiS1.Size = new System.Drawing.Size(170, 18);
            this.lblUiS1.TabIndex = 20;
            this.lblUiS1.Text = "UI Fan Side1";
            this.lblUiS1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Side1
            // 
            this.ledUiFan_Side1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Side1.Location = new System.Drawing.Point(424, 83);
            this.ledUiFan_Side1.Name = "ledUiFan_Side1";
            this.ledUiFan_Side1.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Side1.TabIndex = 21;
            // 
            // lblUiS2
            // 
            this.lblUiS2.Location = new System.Drawing.Point(474, 84);
            this.lblUiS2.Name = "lblUiS2";
            this.lblUiS2.Size = new System.Drawing.Size(170, 18);
            this.lblUiS2.TabIndex = 22;
            this.lblUiS2.Text = "UI Fan Side2";
            this.lblUiS2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledUiFan_Side2
            // 
            this.ledUiFan_Side2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledUiFan_Side2.Location = new System.Drawing.Point(654, 83);
            this.ledUiFan_Side2.Name = "ledUiFan_Side2";
            this.ledUiFan_Side2.Size = new System.Drawing.Size(18, 18);
            this.ledUiFan_Side2.TabIndex = 23;
            // 
            // lblGrip
            // 
            this.lblGrip.Location = new System.Drawing.Point(14, 104);
            this.lblGrip.Name = "lblGrip";
            this.lblGrip.Size = new System.Drawing.Size(170, 18);
            this.lblGrip.TabIndex = 24;
            this.lblGrip.Text = "Grip EMS";
            this.lblGrip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledGripSwitchEms
            // 
            this.ledGripSwitchEms.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledGripSwitchEms.Location = new System.Drawing.Point(194, 103);
            this.ledGripSwitchEms.Name = "ledGripSwitchEms";
            this.ledGripSwitchEms.Size = new System.Drawing.Size(18, 18);
            this.ledGripSwitchEms.TabIndex = 25;
            // 
            // lblPmEms
            // 
            this.lblPmEms.Location = new System.Drawing.Point(244, 104);
            this.lblPmEms.Name = "lblPmEms";
            this.lblPmEms.Size = new System.Drawing.Size(170, 18);
            this.lblPmEms.TabIndex = 26;
            this.lblPmEms.Text = "PM Rack EMS";
            this.lblPmEms.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledPmRackEms
            // 
            this.ledPmRackEms.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledPmRackEms.Location = new System.Drawing.Point(424, 103);
            this.ledPmRackEms.Name = "ledPmRackEms";
            this.ledPmRackEms.Size = new System.Drawing.Size(18, 18);
            this.ledPmRackEms.TabIndex = 27;
            // 
            // lblCh1
            // 
            this.lblCh1.Location = new System.Drawing.Point(474, 104);
            this.lblCh1.Name = "lblCh1";
            this.lblCh1.Size = new System.Drawing.Size(170, 18);
            this.lblCh1.TabIndex = 28;
            this.lblCh1.Text = "Chamber EMS1";
            this.lblCh1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledChamberEms1
            // 
            this.ledChamberEms1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledChamberEms1.Location = new System.Drawing.Point(654, 103);
            this.ledChamberEms1.Name = "ledChamberEms1";
            this.ledChamberEms1.Size = new System.Drawing.Size(18, 18);
            this.ledChamberEms1.TabIndex = 29;
            // 
            // lblCh2
            // 
            this.lblCh2.Location = new System.Drawing.Point(14, 124);
            this.lblCh2.Name = "lblCh2";
            this.lblCh2.Size = new System.Drawing.Size(170, 18);
            this.lblCh2.TabIndex = 30;
            this.lblCh2.Text = "Chamber EMS2";
            this.lblCh2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledChamberEms2
            // 
            this.ledChamberEms2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledChamberEms2.Location = new System.Drawing.Point(194, 123);
            this.ledChamberEms2.Name = "ledChamberEms2";
            this.ledChamberEms2.Size = new System.Drawing.Size(18, 18);
            this.ledChamberEms2.TabIndex = 31;
            // 
            // lblCh3
            // 
            this.lblCh3.Location = new System.Drawing.Point(244, 124);
            this.lblCh3.Name = "lblCh3";
            this.lblCh3.Size = new System.Drawing.Size(170, 18);
            this.lblCh3.TabIndex = 32;
            this.lblCh3.Text = "Chamber EMS3";
            this.lblCh3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledChamberEms3
            // 
            this.ledChamberEms3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledChamberEms3.Location = new System.Drawing.Point(424, 123);
            this.ledChamberEms3.Name = "ledChamberEms3";
            this.ledChamberEms3.Size = new System.Drawing.Size(18, 18);
            this.ledChamberEms3.TabIndex = 33;
            // 
            // lblMode
            // 
            this.lblMode.Location = new System.Drawing.Point(474, 124);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(170, 18);
            this.lblMode.TabIndex = 34;
            this.lblMode.Text = "Mode Key";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledModeKey
            // 
            this.ledModeKey.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledModeKey.Location = new System.Drawing.Point(654, 123);
            this.ledModeKey.Name = "ledModeKey";
            this.ledModeKey.Size = new System.Drawing.Size(18, 18);
            this.ledModeKey.TabIndex = 35;
            // 
            // lblDf1
            // 
            this.lblDf1.Location = new System.Drawing.Point(14, 144);
            this.lblDf1.Name = "lblDf1";
            this.lblDf1.Size = new System.Drawing.Size(170, 18);
            this.lblDf1.TabIndex = 36;
            this.lblDf1.Text = "Door Front1";
            this.lblDf1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorFront1
            // 
            this.ledDoorFront1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorFront1.Location = new System.Drawing.Point(194, 143);
            this.ledDoorFront1.Name = "ledDoorFront1";
            this.ledDoorFront1.Size = new System.Drawing.Size(18, 18);
            this.ledDoorFront1.TabIndex = 37;
            // 
            // lblDf2
            // 
            this.lblDf2.Location = new System.Drawing.Point(244, 144);
            this.lblDf2.Name = "lblDf2";
            this.lblDf2.Size = new System.Drawing.Size(170, 18);
            this.lblDf2.TabIndex = 38;
            this.lblDf2.Text = "Door Front2";
            this.lblDf2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorFront2
            // 
            this.ledDoorFront2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorFront2.Location = new System.Drawing.Point(424, 143);
            this.ledDoorFront2.Name = "ledDoorFront2";
            this.ledDoorFront2.Size = new System.Drawing.Size(18, 18);
            this.ledDoorFront2.TabIndex = 39;
            // 
            // lblDr1
            // 
            this.lblDr1.Location = new System.Drawing.Point(474, 144);
            this.lblDr1.Name = "lblDr1";
            this.lblDr1.Size = new System.Drawing.Size(170, 18);
            this.lblDr1.TabIndex = 40;
            this.lblDr1.Text = "Door Rear1";
            this.lblDr1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorRear1
            // 
            this.ledDoorRear1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorRear1.Location = new System.Drawing.Point(654, 143);
            this.ledDoorRear1.Name = "ledDoorRear1";
            this.ledDoorRear1.Size = new System.Drawing.Size(18, 18);
            this.ledDoorRear1.TabIndex = 41;
            // 
            // lblDr2
            // 
            this.lblDr2.Location = new System.Drawing.Point(14, 164);
            this.lblDr2.Name = "lblDr2";
            this.lblDr2.Size = new System.Drawing.Size(170, 18);
            this.lblDr2.TabIndex = 42;
            this.lblDr2.Text = "Door Rear2";
            this.lblDr2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorRear2
            // 
            this.ledDoorRear2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorRear2.Location = new System.Drawing.Point(194, 163);
            this.ledDoorRear2.Name = "ledDoorRear2";
            this.ledDoorRear2.Size = new System.Drawing.Size(18, 18);
            this.ledDoorRear2.TabIndex = 43;
            // 
            // lblDs1
            // 
            this.lblDs1.Location = new System.Drawing.Point(244, 164);
            this.lblDs1.Name = "lblDs1";
            this.lblDs1.Size = new System.Drawing.Size(170, 18);
            this.lblDs1.TabIndex = 44;
            this.lblDs1.Text = "Door Side1";
            this.lblDs1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorSide1
            // 
            this.ledDoorSide1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorSide1.Location = new System.Drawing.Point(424, 163);
            this.ledDoorSide1.Name = "ledDoorSide1";
            this.ledDoorSide1.Size = new System.Drawing.Size(18, 18);
            this.ledDoorSide1.TabIndex = 45;
            // 
            // lblDs2
            // 
            this.lblDs2.Location = new System.Drawing.Point(474, 164);
            this.lblDs2.Name = "lblDs2";
            this.lblDs2.Size = new System.Drawing.Size(170, 18);
            this.lblDs2.TabIndex = 46;
            this.lblDs2.Text = "Door Side2";
            this.lblDs2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledDoorSide2
            // 
            this.ledDoorSide2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledDoorSide2.Location = new System.Drawing.Point(654, 163);
            this.ledDoorSide2.Name = "ledDoorSide2";
            this.ledDoorSide2.Size = new System.Drawing.Size(18, 18);
            this.ledDoorSide2.TabIndex = 47;
            // 
            // lblLaser
            // 
            this.lblLaser.Location = new System.Drawing.Point(14, 184);
            this.lblLaser.Name = "lblLaser";
            this.lblLaser.Size = new System.Drawing.Size(170, 18);
            this.lblLaser.TabIndex = 48;
            this.lblLaser.Text = "Laser Status";
            this.lblLaser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledLaserStatus
            // 
            this.ledLaserStatus.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledLaserStatus.Location = new System.Drawing.Point(194, 183);
            this.ledLaserStatus.Name = "ledLaserStatus";
            this.ledLaserStatus.Size = new System.Drawing.Size(18, 18);
            this.ledLaserStatus.TabIndex = 49;
            // 
            // lblAcs1
            // 
            this.lblAcs1.Location = new System.Drawing.Point(244, 184);
            this.lblAcs1.Name = "lblAcs1";
            this.lblAcs1.Size = new System.Drawing.Size(170, 18);
            this.lblAcs1.TabIndex = 50;
            this.lblAcs1.Text = "ACS1 Main";
            this.lblAcs1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledAcs1Main
            // 
            this.ledAcs1Main.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledAcs1Main.Location = new System.Drawing.Point(424, 183);
            this.ledAcs1Main.Name = "ledAcs1Main";
            this.ledAcs1Main.Size = new System.Drawing.Size(18, 18);
            this.ledAcs1Main.TabIndex = 51;
            // 
            // lblAcs2Y
            // 
            this.lblAcs2Y.Location = new System.Drawing.Point(474, 184);
            this.lblAcs2Y.Name = "lblAcs2Y";
            this.lblAcs2Y.Size = new System.Drawing.Size(170, 18);
            this.lblAcs2Y.TabIndex = 52;
            this.lblAcs2Y.Text = "ACS2 Y";
            this.lblAcs2Y.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledAcs2Y
            // 
            this.ledAcs2Y.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledAcs2Y.Location = new System.Drawing.Point(654, 183);
            this.ledAcs2Y.Name = "ledAcs2Y";
            this.ledAcs2Y.Size = new System.Drawing.Size(18, 18);
            this.ledAcs2Y.TabIndex = 53;
            // 
            // lblAcs2X
            // 
            this.lblAcs2X.Location = new System.Drawing.Point(14, 204);
            this.lblAcs2X.Name = "lblAcs2X";
            this.lblAcs2X.Size = new System.Drawing.Size(170, 18);
            this.lblAcs2X.TabIndex = 54;
            this.lblAcs2X.Text = "ACS2 X";
            this.lblAcs2X.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledAcs2X
            // 
            this.ledAcs2X.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledAcs2X.Location = new System.Drawing.Point(194, 203);
            this.ledAcs2X.Name = "ledAcs2X";
            this.ledAcs2X.Size = new System.Drawing.Size(18, 18);
            this.ledAcs2X.TabIndex = 55;
            // 
            // lblRot1
            // 
            this.lblRot1.Location = new System.Drawing.Point(244, 204);
            this.lblRot1.Name = "lblRot1";
            this.lblRot1.Size = new System.Drawing.Size(170, 18);
            this.lblRot1.TabIndex = 56;
            this.lblRot1.Text = "Rot1 Main";
            this.lblRot1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledRot1Main
            // 
            this.ledRot1Main.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledRot1Main.Location = new System.Drawing.Point(424, 203);
            this.ledRot1Main.Name = "ledRot1Main";
            this.ledRot1Main.Size = new System.Drawing.Size(18, 18);
            this.ledRot1Main.TabIndex = 57;
            // 
            // lblRot2
            // 
            this.lblRot2.Location = new System.Drawing.Point(474, 204);
            this.lblRot2.Name = "lblRot2";
            this.lblRot2.Size = new System.Drawing.Size(170, 18);
            this.lblRot2.TabIndex = 58;
            this.lblRot2.Text = "Rot2 Theta";
            this.lblRot2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledRot2Theta
            // 
            this.ledRot2Theta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledRot2Theta.Location = new System.Drawing.Point(654, 203);
            this.ledRot2Theta.Name = "ledRot2Theta";
            this.ledRot2Theta.Size = new System.Drawing.Size(18, 18);
            this.ledRot2Theta.TabIndex = 59;
            // 
            // lblRot2M
            // 
            this.lblRot2M.Location = new System.Drawing.Point(14, 224);
            this.lblRot2M.Name = "lblRot2M";
            this.lblRot2M.Size = new System.Drawing.Size(170, 18);
            this.lblRot2M.TabIndex = 60;
            this.lblRot2M.Text = "Rot2 MaintZ1/2";
            this.lblRot2M.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledRot2MaintZ12
            // 
            this.ledRot2MaintZ12.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledRot2MaintZ12.Location = new System.Drawing.Point(194, 223);
            this.ledRot2MaintZ12.Name = "ledRot2MaintZ12";
            this.ledRot2MaintZ12.Size = new System.Drawing.Size(18, 18);
            this.ledRot2MaintZ12.TabIndex = 61;
            // 
            // grpOutputs
            // 
            this.grpOutputs.Controls.Add(this.label2);
            this.grpOutputs.Controls.Add(this.lblMc2ndZO);
            this.grpOutputs.Controls.Add(this.lblMc2ndXO);
            this.grpOutputs.Controls.Add(this.lblMc2ndYO);
            this.grpOutputs.Controls.Add(this.lblLaserBoxAirVv);
            this.grpOutputs.Controls.Add(this.lblY2MotorAirVv);
            this.grpOutputs.Controls.Add(this.lblXMotorAirVv);
            this.grpOutputs.Controls.Add(this.lblY1MotorAirVv);
            this.grpOutputs.Controls.Add(this.lblMc1stO);
            this.grpOutputs.Controls.Add(this.btnAjinMc2_OutputTOn);
            this.grpOutputs.Controls.Add(this.btnAjinMc2_OutputZOn);
            this.grpOutputs.Controls.Add(this.btnAjinMc2_OutputTOff);
            this.grpOutputs.Controls.Add(this.btnACSMc2_OutputXOn);
            this.grpOutputs.Controls.Add(this.btnAjinMc2_OutputZOff);
            this.grpOutputs.Controls.Add(this.btnACSMc2_OutputYOn);
            this.grpOutputs.Controls.Add(this.btnACSMc2_OutputXOff);
            this.grpOutputs.Controls.Add(this.lblO1);
            this.grpOutputs.Controls.Add(this.ledAjinMc2_OutputTState);
            this.grpOutputs.Controls.Add(this.btnACSMc2_OutputYOff);
            this.grpOutputs.Controls.Add(this.btnLaserBox_AirCoolingVvOpen);
            this.grpOutputs.Controls.Add(this.btnY2Motor_AirCoolingVvOpen);
            this.grpOutputs.Controls.Add(this.ledAjinMc2_OutputZState);
            this.grpOutputs.Controls.Add(this.btnXMotor_AirCoolingVvOpen);
            this.grpOutputs.Controls.Add(this.btnLaserBox_AirCoolingVvClose);
            this.grpOutputs.Controls.Add(this.btnY1Motor_AirCoolingVvOpen);
            this.grpOutputs.Controls.Add(this.btnY2Motor_AirCoolingVvClose);
            this.grpOutputs.Controls.Add(this.btnXMotor_AirCoolingVvClose);
            this.grpOutputs.Controls.Add(this.btnACSMc1_OutputOn);
            this.grpOutputs.Controls.Add(this.btnY1Motor_AirCoolingVvClose);
            this.grpOutputs.Controls.Add(this.ledACSMc2_OutputXState);
            this.grpOutputs.Controls.Add(this.ledLaserBox_AirCoolingVvState);
            this.grpOutputs.Controls.Add(this.btnACSMc1_OutputOff);
            this.grpOutputs.Controls.Add(this.ledY2Motor_AirCoolingVvState);
            this.grpOutputs.Controls.Add(this.ledXMotor_AirCoolingVvState);
            this.grpOutputs.Controls.Add(this.ledACSMc2_OutputYState);
            this.grpOutputs.Controls.Add(this.ledY1Motor_AirCoolingVvState);
            this.grpOutputs.Controls.Add(this.btnFrontDoor1_Lock);
            this.grpOutputs.Controls.Add(this.ledACSMc1_OutputState);
            this.grpOutputs.Controls.Add(this.btnFrontDoor1_Unlock);
            this.grpOutputs.Controls.Add(this.ledFrontDoor1_LockState);
            this.grpOutputs.Controls.Add(this.lblO2);
            this.grpOutputs.Controls.Add(this.btnFrontDoor2_Lock);
            this.grpOutputs.Controls.Add(this.btnFrontDoor2_Unlock);
            this.grpOutputs.Controls.Add(this.ledFrontDoor2_LockState);
            this.grpOutputs.Controls.Add(this.lblO3);
            this.grpOutputs.Controls.Add(this.btnRearDoor1_Lock);
            this.grpOutputs.Controls.Add(this.btnRearDoor1_Unlock);
            this.grpOutputs.Controls.Add(this.ledRearDoor1_LockState);
            this.grpOutputs.Controls.Add(this.lblO4);
            this.grpOutputs.Controls.Add(this.btnRearDoor2_Lock);
            this.grpOutputs.Controls.Add(this.btnRearDoor2_Unlock);
            this.grpOutputs.Controls.Add(this.ledRearDoor2_LockState);
            this.grpOutputs.Controls.Add(this.lblO5);
            this.grpOutputs.Controls.Add(this.btnSideDoor1_Lock);
            this.grpOutputs.Controls.Add(this.btnSideDoor1_Unlock);
            this.grpOutputs.Controls.Add(this.ledSideDoor1_LockState);
            this.grpOutputs.Controls.Add(this.lblO6);
            this.grpOutputs.Controls.Add(this.btnSideDoor2_Lock);
            this.grpOutputs.Controls.Add(this.btnSideDoor2_Unlock);
            this.grpOutputs.Controls.Add(this.ledSideDoor2_LockState);
            this.grpOutputs.Controls.Add(this.lblO7);
            this.grpOutputs.Controls.Add(this.btnCleanBoothLed_On);
            this.grpOutputs.Controls.Add(this.btnCleanBoothLed_Off);
            this.grpOutputs.Controls.Add(this.ledCleanBoothLed);
            this.grpOutputs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpOutputs.Location = new System.Drawing.Point(12, 576);
            this.grpOutputs.Name = "grpOutputs";
            this.grpOutputs.Size = new System.Drawing.Size(1323, 243);
            this.grpOutputs.TabIndex = 3;
            this.grpOutputs.TabStop = false;
            this.grpOutputs.Text = "Outputs";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(449, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Ajin 2nd MC Output T";
            // 
            // lblMc2ndZO
            // 
            this.lblMc2ndZO.Location = new System.Drawing.Point(449, 144);
            this.lblMc2ndZO.Name = "lblMc2ndZO";
            this.lblMc2ndZO.Size = new System.Drawing.Size(140, 20);
            this.lblMc2ndZO.TabIndex = 0;
            this.lblMc2ndZO.Text = "Ajin 2nd MC Output Z";
            // 
            // lblMc2ndXO
            // 
            this.lblMc2ndXO.Location = new System.Drawing.Point(449, 84);
            this.lblMc2ndXO.Name = "lblMc2ndXO";
            this.lblMc2ndXO.Size = new System.Drawing.Size(140, 20);
            this.lblMc2ndXO.TabIndex = 0;
            this.lblMc2ndXO.Text = "ACS 2nd MC Output X";
            // 
            // lblMc2ndYO
            // 
            this.lblMc2ndYO.Location = new System.Drawing.Point(449, 54);
            this.lblMc2ndYO.Name = "lblMc2ndYO";
            this.lblMc2ndYO.Size = new System.Drawing.Size(140, 20);
            this.lblMc2ndYO.TabIndex = 0;
            this.lblMc2ndYO.Text = "ACS 2nd MC Output Y";
            // 
            // lblLaserBoxAirVv
            // 
            this.lblLaserBoxAirVv.Location = new System.Drawing.Point(879, 114);
            this.lblLaserBoxAirVv.Name = "lblLaserBoxAirVv";
            this.lblLaserBoxAirVv.Size = new System.Drawing.Size(148, 20);
            this.lblLaserBoxAirVv.TabIndex = 0;
            this.lblLaserBoxAirVv.Text = "Laser Box Air V/V";
            // 
            // lblY2MotorAirVv
            // 
            this.lblY2MotorAirVv.Location = new System.Drawing.Point(879, 54);
            this.lblY2MotorAirVv.Name = "lblY2MotorAirVv";
            this.lblY2MotorAirVv.Size = new System.Drawing.Size(148, 20);
            this.lblY2MotorAirVv.TabIndex = 0;
            this.lblY2MotorAirVv.Text = "Y2 Linear Motor Air V/V";
            // 
            // lblXMotorAirVv
            // 
            this.lblXMotorAirVv.Location = new System.Drawing.Point(879, 84);
            this.lblXMotorAirVv.Name = "lblXMotorAirVv";
            this.lblXMotorAirVv.Size = new System.Drawing.Size(148, 20);
            this.lblXMotorAirVv.TabIndex = 0;
            this.lblXMotorAirVv.Text = "X Linear Motor Air V/V";
            // 
            // lblY1MotorAirVv
            // 
            this.lblY1MotorAirVv.Location = new System.Drawing.Point(879, 24);
            this.lblY1MotorAirVv.Name = "lblY1MotorAirVv";
            this.lblY1MotorAirVv.Size = new System.Drawing.Size(148, 20);
            this.lblY1MotorAirVv.TabIndex = 0;
            this.lblY1MotorAirVv.Text = "Y1 Linear Motor Air V/V";
            // 
            // lblMc1stO
            // 
            this.lblMc1stO.Location = new System.Drawing.Point(449, 24);
            this.lblMc1stO.Name = "lblMc1stO";
            this.lblMc1stO.Size = new System.Drawing.Size(140, 20);
            this.lblMc1stO.TabIndex = 0;
            this.lblMc1stO.Text = "ACS 1st MC Output";
            // 
            // btnAjinMc2_OutputTOn
            // 
            this.btnAjinMc2_OutputTOn.Location = new System.Drawing.Point(597, 169);
            this.btnAjinMc2_OutputTOn.Name = "btnAjinMc2_OutputTOn";
            this.btnAjinMc2_OutputTOn.Size = new System.Drawing.Size(90, 26);
            this.btnAjinMc2_OutputTOn.TabIndex = 1;
            this.btnAjinMc2_OutputTOn.Text = "MC On";
            // 
            // btnAjinMc2_OutputZOn
            // 
            this.btnAjinMc2_OutputZOn.Location = new System.Drawing.Point(597, 139);
            this.btnAjinMc2_OutputZOn.Name = "btnAjinMc2_OutputZOn";
            this.btnAjinMc2_OutputZOn.Size = new System.Drawing.Size(90, 26);
            this.btnAjinMc2_OutputZOn.TabIndex = 1;
            this.btnAjinMc2_OutputZOn.Text = "MC On";
            // 
            // btnAjinMc2_OutputTOff
            // 
            this.btnAjinMc2_OutputTOff.Location = new System.Drawing.Point(697, 169);
            this.btnAjinMc2_OutputTOff.Name = "btnAjinMc2_OutputTOff";
            this.btnAjinMc2_OutputTOff.Size = new System.Drawing.Size(90, 26);
            this.btnAjinMc2_OutputTOff.TabIndex = 2;
            this.btnAjinMc2_OutputTOff.Text = "MC Off";
            // 
            // btnACSMc2_OutputXOn
            // 
            this.btnACSMc2_OutputXOn.Location = new System.Drawing.Point(597, 79);
            this.btnACSMc2_OutputXOn.Name = "btnACSMc2_OutputXOn";
            this.btnACSMc2_OutputXOn.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc2_OutputXOn.TabIndex = 1;
            this.btnACSMc2_OutputXOn.Text = "MC On";
            // 
            // btnAjinMc2_OutputZOff
            // 
            this.btnAjinMc2_OutputZOff.Location = new System.Drawing.Point(697, 139);
            this.btnAjinMc2_OutputZOff.Name = "btnAjinMc2_OutputZOff";
            this.btnAjinMc2_OutputZOff.Size = new System.Drawing.Size(90, 26);
            this.btnAjinMc2_OutputZOff.TabIndex = 2;
            this.btnAjinMc2_OutputZOff.Text = "MC Off";
            // 
            // btnACSMc2_OutputYOn
            // 
            this.btnACSMc2_OutputYOn.Location = new System.Drawing.Point(597, 49);
            this.btnACSMc2_OutputYOn.Name = "btnACSMc2_OutputYOn";
            this.btnACSMc2_OutputYOn.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc2_OutputYOn.TabIndex = 1;
            this.btnACSMc2_OutputYOn.Text = "MC On";
            // 
            // btnACSMc2_OutputXOff
            // 
            this.btnACSMc2_OutputXOff.Location = new System.Drawing.Point(697, 79);
            this.btnACSMc2_OutputXOff.Name = "btnACSMc2_OutputXOff";
            this.btnACSMc2_OutputXOff.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc2_OutputXOff.TabIndex = 2;
            this.btnACSMc2_OutputXOff.Text = "MC Off";
            // 
            // lblO1
            // 
            this.lblO1.Location = new System.Drawing.Point(14, 24);
            this.lblO1.Name = "lblO1";
            this.lblO1.Size = new System.Drawing.Size(145, 20);
            this.lblO1.TabIndex = 0;
            this.lblO1.Text = "Front Door 1";
            // 
            // ledAjinMc2_OutputTState
            // 
            this.ledAjinMc2_OutputTState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledAjinMc2_OutputTState.Location = new System.Drawing.Point(807, 172);
            this.ledAjinMc2_OutputTState.Name = "ledAjinMc2_OutputTState";
            this.ledAjinMc2_OutputTState.Size = new System.Drawing.Size(18, 18);
            this.ledAjinMc2_OutputTState.TabIndex = 3;
            // 
            // btnACSMc2_OutputYOff
            // 
            this.btnACSMc2_OutputYOff.Location = new System.Drawing.Point(697, 49);
            this.btnACSMc2_OutputYOff.Name = "btnACSMc2_OutputYOff";
            this.btnACSMc2_OutputYOff.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc2_OutputYOff.TabIndex = 2;
            this.btnACSMc2_OutputYOff.Text = "MC Off";
            // 
            // btnLaserBox_AirCoolingVvOpen
            // 
            this.btnLaserBox_AirCoolingVvOpen.Location = new System.Drawing.Point(1037, 109);
            this.btnLaserBox_AirCoolingVvOpen.Name = "btnLaserBox_AirCoolingVvOpen";
            this.btnLaserBox_AirCoolingVvOpen.Size = new System.Drawing.Size(90, 26);
            this.btnLaserBox_AirCoolingVvOpen.TabIndex = 1;
            this.btnLaserBox_AirCoolingVvOpen.Text = "Open";
            // 
            // btnY2Motor_AirCoolingVvOpen
            // 
            this.btnY2Motor_AirCoolingVvOpen.Location = new System.Drawing.Point(1037, 49);
            this.btnY2Motor_AirCoolingVvOpen.Name = "btnY2Motor_AirCoolingVvOpen";
            this.btnY2Motor_AirCoolingVvOpen.Size = new System.Drawing.Size(90, 26);
            this.btnY2Motor_AirCoolingVvOpen.TabIndex = 1;
            this.btnY2Motor_AirCoolingVvOpen.Text = "Open";
            // 
            // ledAjinMc2_OutputZState
            // 
            this.ledAjinMc2_OutputZState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledAjinMc2_OutputZState.Location = new System.Drawing.Point(807, 142);
            this.ledAjinMc2_OutputZState.Name = "ledAjinMc2_OutputZState";
            this.ledAjinMc2_OutputZState.Size = new System.Drawing.Size(18, 18);
            this.ledAjinMc2_OutputZState.TabIndex = 3;
            // 
            // btnXMotor_AirCoolingVvOpen
            // 
            this.btnXMotor_AirCoolingVvOpen.Location = new System.Drawing.Point(1037, 79);
            this.btnXMotor_AirCoolingVvOpen.Name = "btnXMotor_AirCoolingVvOpen";
            this.btnXMotor_AirCoolingVvOpen.Size = new System.Drawing.Size(90, 26);
            this.btnXMotor_AirCoolingVvOpen.TabIndex = 1;
            this.btnXMotor_AirCoolingVvOpen.Text = "Open";
            // 
            // btnLaserBox_AirCoolingVvClose
            // 
            this.btnLaserBox_AirCoolingVvClose.Location = new System.Drawing.Point(1137, 109);
            this.btnLaserBox_AirCoolingVvClose.Name = "btnLaserBox_AirCoolingVvClose";
            this.btnLaserBox_AirCoolingVvClose.Size = new System.Drawing.Size(90, 26);
            this.btnLaserBox_AirCoolingVvClose.TabIndex = 2;
            this.btnLaserBox_AirCoolingVvClose.Text = "Close";
            // 
            // btnY1Motor_AirCoolingVvOpen
            // 
            this.btnY1Motor_AirCoolingVvOpen.Location = new System.Drawing.Point(1037, 19);
            this.btnY1Motor_AirCoolingVvOpen.Name = "btnY1Motor_AirCoolingVvOpen";
            this.btnY1Motor_AirCoolingVvOpen.Size = new System.Drawing.Size(90, 26);
            this.btnY1Motor_AirCoolingVvOpen.TabIndex = 1;
            this.btnY1Motor_AirCoolingVvOpen.Text = "Open";
            // 
            // btnY2Motor_AirCoolingVvClose
            // 
            this.btnY2Motor_AirCoolingVvClose.Location = new System.Drawing.Point(1137, 49);
            this.btnY2Motor_AirCoolingVvClose.Name = "btnY2Motor_AirCoolingVvClose";
            this.btnY2Motor_AirCoolingVvClose.Size = new System.Drawing.Size(90, 26);
            this.btnY2Motor_AirCoolingVvClose.TabIndex = 2;
            this.btnY2Motor_AirCoolingVvClose.Text = "Close";
            // 
            // btnXMotor_AirCoolingVvClose
            // 
            this.btnXMotor_AirCoolingVvClose.Location = new System.Drawing.Point(1137, 79);
            this.btnXMotor_AirCoolingVvClose.Name = "btnXMotor_AirCoolingVvClose";
            this.btnXMotor_AirCoolingVvClose.Size = new System.Drawing.Size(90, 26);
            this.btnXMotor_AirCoolingVvClose.TabIndex = 2;
            this.btnXMotor_AirCoolingVvClose.Text = "Close";
            // 
            // btnACSMc1_OutputOn
            // 
            this.btnACSMc1_OutputOn.Location = new System.Drawing.Point(597, 19);
            this.btnACSMc1_OutputOn.Name = "btnACSMc1_OutputOn";
            this.btnACSMc1_OutputOn.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc1_OutputOn.TabIndex = 1;
            this.btnACSMc1_OutputOn.Text = "MC On";
            // 
            // btnY1Motor_AirCoolingVvClose
            // 
            this.btnY1Motor_AirCoolingVvClose.Location = new System.Drawing.Point(1137, 19);
            this.btnY1Motor_AirCoolingVvClose.Name = "btnY1Motor_AirCoolingVvClose";
            this.btnY1Motor_AirCoolingVvClose.Size = new System.Drawing.Size(90, 26);
            this.btnY1Motor_AirCoolingVvClose.TabIndex = 2;
            this.btnY1Motor_AirCoolingVvClose.Text = "Close";
            // 
            // ledACSMc2_OutputXState
            // 
            this.ledACSMc2_OutputXState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledACSMc2_OutputXState.Location = new System.Drawing.Point(807, 82);
            this.ledACSMc2_OutputXState.Name = "ledACSMc2_OutputXState";
            this.ledACSMc2_OutputXState.Size = new System.Drawing.Size(18, 18);
            this.ledACSMc2_OutputXState.TabIndex = 3;
            // 
            // ledLaserBox_AirCoolingVvState
            // 
            this.ledLaserBox_AirCoolingVvState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledLaserBox_AirCoolingVvState.Location = new System.Drawing.Point(1247, 112);
            this.ledLaserBox_AirCoolingVvState.Name = "ledLaserBox_AirCoolingVvState";
            this.ledLaserBox_AirCoolingVvState.Size = new System.Drawing.Size(18, 18);
            this.ledLaserBox_AirCoolingVvState.TabIndex = 3;
            // 
            // btnACSMc1_OutputOff
            // 
            this.btnACSMc1_OutputOff.Location = new System.Drawing.Point(697, 19);
            this.btnACSMc1_OutputOff.Name = "btnACSMc1_OutputOff";
            this.btnACSMc1_OutputOff.Size = new System.Drawing.Size(90, 26);
            this.btnACSMc1_OutputOff.TabIndex = 2;
            this.btnACSMc1_OutputOff.Text = "MC Off";
            // 
            // ledY2Motor_AirCoolingVvState
            // 
            this.ledY2Motor_AirCoolingVvState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledY2Motor_AirCoolingVvState.Location = new System.Drawing.Point(1247, 52);
            this.ledY2Motor_AirCoolingVvState.Name = "ledY2Motor_AirCoolingVvState";
            this.ledY2Motor_AirCoolingVvState.Size = new System.Drawing.Size(18, 18);
            this.ledY2Motor_AirCoolingVvState.TabIndex = 3;
            // 
            // ledXMotor_AirCoolingVvState
            // 
            this.ledXMotor_AirCoolingVvState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledXMotor_AirCoolingVvState.Location = new System.Drawing.Point(1247, 82);
            this.ledXMotor_AirCoolingVvState.Name = "ledXMotor_AirCoolingVvState";
            this.ledXMotor_AirCoolingVvState.Size = new System.Drawing.Size(18, 18);
            this.ledXMotor_AirCoolingVvState.TabIndex = 3;
            // 
            // ledACSMc2_OutputYState
            // 
            this.ledACSMc2_OutputYState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledACSMc2_OutputYState.Location = new System.Drawing.Point(807, 52);
            this.ledACSMc2_OutputYState.Name = "ledACSMc2_OutputYState";
            this.ledACSMc2_OutputYState.Size = new System.Drawing.Size(18, 18);
            this.ledACSMc2_OutputYState.TabIndex = 3;
            // 
            // ledY1Motor_AirCoolingVvState
            // 
            this.ledY1Motor_AirCoolingVvState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledY1Motor_AirCoolingVvState.Location = new System.Drawing.Point(1247, 22);
            this.ledY1Motor_AirCoolingVvState.Name = "ledY1Motor_AirCoolingVvState";
            this.ledY1Motor_AirCoolingVvState.Size = new System.Drawing.Size(18, 18);
            this.ledY1Motor_AirCoolingVvState.TabIndex = 3;
            // 
            // btnFrontDoor1_Lock
            // 
            this.btnFrontDoor1_Lock.Location = new System.Drawing.Point(170, 19);
            this.btnFrontDoor1_Lock.Name = "btnFrontDoor1_Lock";
            this.btnFrontDoor1_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnFrontDoor1_Lock.TabIndex = 1;
            this.btnFrontDoor1_Lock.Text = "Lock";
            // 
            // ledACSMc1_OutputState
            // 
            this.ledACSMc1_OutputState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledACSMc1_OutputState.Location = new System.Drawing.Point(807, 22);
            this.ledACSMc1_OutputState.Name = "ledACSMc1_OutputState";
            this.ledACSMc1_OutputState.Size = new System.Drawing.Size(18, 18);
            this.ledACSMc1_OutputState.TabIndex = 3;
            // 
            // btnFrontDoor1_Unlock
            // 
            this.btnFrontDoor1_Unlock.Location = new System.Drawing.Point(270, 19);
            this.btnFrontDoor1_Unlock.Name = "btnFrontDoor1_Unlock";
            this.btnFrontDoor1_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnFrontDoor1_Unlock.TabIndex = 2;
            this.btnFrontDoor1_Unlock.Text = "Unlock";
            // 
            // ledFrontDoor1_LockState
            // 
            this.ledFrontDoor1_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledFrontDoor1_LockState.Location = new System.Drawing.Point(380, 22);
            this.ledFrontDoor1_LockState.Name = "ledFrontDoor1_LockState";
            this.ledFrontDoor1_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledFrontDoor1_LockState.TabIndex = 3;
            // 
            // lblO2
            // 
            this.lblO2.Location = new System.Drawing.Point(14, 54);
            this.lblO2.Name = "lblO2";
            this.lblO2.Size = new System.Drawing.Size(145, 20);
            this.lblO2.TabIndex = 4;
            this.lblO2.Text = "Front Door 2";
            // 
            // btnFrontDoor2_Lock
            // 
            this.btnFrontDoor2_Lock.Location = new System.Drawing.Point(170, 49);
            this.btnFrontDoor2_Lock.Name = "btnFrontDoor2_Lock";
            this.btnFrontDoor2_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnFrontDoor2_Lock.TabIndex = 5;
            this.btnFrontDoor2_Lock.Text = "Lock";
            // 
            // btnFrontDoor2_Unlock
            // 
            this.btnFrontDoor2_Unlock.Location = new System.Drawing.Point(270, 49);
            this.btnFrontDoor2_Unlock.Name = "btnFrontDoor2_Unlock";
            this.btnFrontDoor2_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnFrontDoor2_Unlock.TabIndex = 6;
            this.btnFrontDoor2_Unlock.Text = "Unlock";
            // 
            // ledFrontDoor2_LockState
            // 
            this.ledFrontDoor2_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledFrontDoor2_LockState.Location = new System.Drawing.Point(380, 52);
            this.ledFrontDoor2_LockState.Name = "ledFrontDoor2_LockState";
            this.ledFrontDoor2_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledFrontDoor2_LockState.TabIndex = 7;
            // 
            // lblO3
            // 
            this.lblO3.Location = new System.Drawing.Point(14, 84);
            this.lblO3.Name = "lblO3";
            this.lblO3.Size = new System.Drawing.Size(145, 20);
            this.lblO3.TabIndex = 8;
            this.lblO3.Text = "Rear Door 1";
            // 
            // btnRearDoor1_Lock
            // 
            this.btnRearDoor1_Lock.Location = new System.Drawing.Point(170, 79);
            this.btnRearDoor1_Lock.Name = "btnRearDoor1_Lock";
            this.btnRearDoor1_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnRearDoor1_Lock.TabIndex = 9;
            this.btnRearDoor1_Lock.Text = "Lock";
            // 
            // btnRearDoor1_Unlock
            // 
            this.btnRearDoor1_Unlock.Location = new System.Drawing.Point(270, 79);
            this.btnRearDoor1_Unlock.Name = "btnRearDoor1_Unlock";
            this.btnRearDoor1_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnRearDoor1_Unlock.TabIndex = 10;
            this.btnRearDoor1_Unlock.Text = "Unlock";
            // 
            // ledRearDoor1_LockState
            // 
            this.ledRearDoor1_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledRearDoor1_LockState.Location = new System.Drawing.Point(380, 82);
            this.ledRearDoor1_LockState.Name = "ledRearDoor1_LockState";
            this.ledRearDoor1_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledRearDoor1_LockState.TabIndex = 11;
            // 
            // lblO4
            // 
            this.lblO4.Location = new System.Drawing.Point(14, 114);
            this.lblO4.Name = "lblO4";
            this.lblO4.Size = new System.Drawing.Size(145, 20);
            this.lblO4.TabIndex = 12;
            this.lblO4.Text = "Rear Door 2";
            // 
            // btnRearDoor2_Lock
            // 
            this.btnRearDoor2_Lock.Location = new System.Drawing.Point(170, 109);
            this.btnRearDoor2_Lock.Name = "btnRearDoor2_Lock";
            this.btnRearDoor2_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnRearDoor2_Lock.TabIndex = 13;
            this.btnRearDoor2_Lock.Text = "Lock";
            // 
            // btnRearDoor2_Unlock
            // 
            this.btnRearDoor2_Unlock.Location = new System.Drawing.Point(270, 109);
            this.btnRearDoor2_Unlock.Name = "btnRearDoor2_Unlock";
            this.btnRearDoor2_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnRearDoor2_Unlock.TabIndex = 14;
            this.btnRearDoor2_Unlock.Text = "Unlock";
            // 
            // ledRearDoor2_LockState
            // 
            this.ledRearDoor2_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledRearDoor2_LockState.Location = new System.Drawing.Point(380, 112);
            this.ledRearDoor2_LockState.Name = "ledRearDoor2_LockState";
            this.ledRearDoor2_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledRearDoor2_LockState.TabIndex = 15;
            // 
            // lblO5
            // 
            this.lblO5.Location = new System.Drawing.Point(14, 144);
            this.lblO5.Name = "lblO5";
            this.lblO5.Size = new System.Drawing.Size(145, 20);
            this.lblO5.TabIndex = 16;
            this.lblO5.Text = "Side Door 1";
            // 
            // btnSideDoor1_Lock
            // 
            this.btnSideDoor1_Lock.Location = new System.Drawing.Point(170, 139);
            this.btnSideDoor1_Lock.Name = "btnSideDoor1_Lock";
            this.btnSideDoor1_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnSideDoor1_Lock.TabIndex = 17;
            this.btnSideDoor1_Lock.Text = "Lock";
            // 
            // btnSideDoor1_Unlock
            // 
            this.btnSideDoor1_Unlock.Location = new System.Drawing.Point(270, 139);
            this.btnSideDoor1_Unlock.Name = "btnSideDoor1_Unlock";
            this.btnSideDoor1_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnSideDoor1_Unlock.TabIndex = 18;
            this.btnSideDoor1_Unlock.Text = "Unlock";
            // 
            // ledSideDoor1_LockState
            // 
            this.ledSideDoor1_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledSideDoor1_LockState.Location = new System.Drawing.Point(380, 142);
            this.ledSideDoor1_LockState.Name = "ledSideDoor1_LockState";
            this.ledSideDoor1_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledSideDoor1_LockState.TabIndex = 19;
            // 
            // lblO6
            // 
            this.lblO6.Location = new System.Drawing.Point(14, 174);
            this.lblO6.Name = "lblO6";
            this.lblO6.Size = new System.Drawing.Size(145, 20);
            this.lblO6.TabIndex = 20;
            this.lblO6.Text = "Side Door 2";
            // 
            // btnSideDoor2_Lock
            // 
            this.btnSideDoor2_Lock.Location = new System.Drawing.Point(170, 169);
            this.btnSideDoor2_Lock.Name = "btnSideDoor2_Lock";
            this.btnSideDoor2_Lock.Size = new System.Drawing.Size(90, 26);
            this.btnSideDoor2_Lock.TabIndex = 21;
            this.btnSideDoor2_Lock.Text = "Lock";
            // 
            // btnSideDoor2_Unlock
            // 
            this.btnSideDoor2_Unlock.Location = new System.Drawing.Point(270, 169);
            this.btnSideDoor2_Unlock.Name = "btnSideDoor2_Unlock";
            this.btnSideDoor2_Unlock.Size = new System.Drawing.Size(90, 26);
            this.btnSideDoor2_Unlock.TabIndex = 22;
            this.btnSideDoor2_Unlock.Text = "Unlock";
            // 
            // ledSideDoor2_LockState
            // 
            this.ledSideDoor2_LockState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledSideDoor2_LockState.Location = new System.Drawing.Point(380, 172);
            this.ledSideDoor2_LockState.Name = "ledSideDoor2_LockState";
            this.ledSideDoor2_LockState.Size = new System.Drawing.Size(18, 18);
            this.ledSideDoor2_LockState.TabIndex = 23;
            // 
            // lblO7
            // 
            this.lblO7.Location = new System.Drawing.Point(14, 204);
            this.lblO7.Name = "lblO7";
            this.lblO7.Size = new System.Drawing.Size(145, 20);
            this.lblO7.TabIndex = 24;
            this.lblO7.Text = "Clean Booth LED Lamp";
            // 
            // btnCleanBoothLed_On
            // 
            this.btnCleanBoothLed_On.Location = new System.Drawing.Point(170, 199);
            this.btnCleanBoothLed_On.Name = "btnCleanBoothLed_On";
            this.btnCleanBoothLed_On.Size = new System.Drawing.Size(90, 26);
            this.btnCleanBoothLed_On.TabIndex = 25;
            this.btnCleanBoothLed_On.Text = "On";
            // 
            // btnCleanBoothLed_Off
            // 
            this.btnCleanBoothLed_Off.Location = new System.Drawing.Point(270, 199);
            this.btnCleanBoothLed_Off.Name = "btnCleanBoothLed_Off";
            this.btnCleanBoothLed_Off.Size = new System.Drawing.Size(90, 26);
            this.btnCleanBoothLed_Off.TabIndex = 26;
            this.btnCleanBoothLed_Off.Text = "Off";
            // 
            // ledCleanBoothLed
            // 
            this.ledCleanBoothLed.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledCleanBoothLed.Location = new System.Drawing.Point(380, 202);
            this.ledCleanBoothLed.Name = "ledCleanBoothLed";
            this.ledCleanBoothLed.Size = new System.Drawing.Size(18, 18);
            this.ledCleanBoothLed.TabIndex = 27;
            // 
            // grpNotes
            // 
            this.grpNotes.Controls.Add(this.lblNotes);
            this.grpNotes.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpNotes.Location = new System.Drawing.Point(752, 12);
            this.grpNotes.Name = "grpNotes";
            this.grpNotes.Size = new System.Drawing.Size(720, 110);
            this.grpNotes.TabIndex = 4;
            this.grpNotes.TabStop = false;
            this.grpNotes.Text = "Notes";
            // 
            // lblNotes
            // 
            this.lblNotes.Location = new System.Drawing.Point(18, 26);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(690, 70);
            this.lblNotes.TabIndex = 0;
            this.lblNotes.Text = "• Online : 장비 연결 정상\n• Offline : 통신 안됨/끊김\n• Simulation : 시뮬레이터 동작 (Online와 동일 음영, " +
    "텍스트만 Simulation)";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lstOverview);
            this.panel1.Location = new System.Drawing.Point(10, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(856, 403);
            this.panel1.TabIndex = 5;
            // 
            // lstOverview
            // 
            this.lstOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstOverview.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lstOverview.HorizontalScrollbar = true;
            this.lstOverview.ItemHeight = 15;
            this.lstOverview.Location = new System.Drawing.Point(0, 0);
            this.lstOverview.Name = "lstOverview";
            this.lstOverview.Size = new System.Drawing.Size(856, 403);
            this.lstOverview.TabIndex = 0;
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Interval = 300;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
            // 
            // grpInterlockParam
            // 
            this.grpInterlockParam.Controls.Add(this.lblItkEmsAllOk);
            this.grpInterlockParam.Controls.Add(this.ledItkEmsAllOk);
            this.grpInterlockParam.Controls.Add(this.lblItkAjinMcAllOk);
            this.grpInterlockParam.Controls.Add(this.ledItkAjinMcAllOk);
            this.grpInterlockParam.Controls.Add(this.lblItkAcsMcAllOk);
            this.grpInterlockParam.Controls.Add(this.ledItkAcsMcAllOk);
            this.grpInterlockParam.Controls.Add(this.lblItkStageMoveOk);
            this.grpInterlockParam.Controls.Add(this.ledItkStageMoveOk);
            this.grpInterlockParam.Controls.Add(this.lblItkLaserReady);
            this.grpInterlockParam.Controls.Add(this.ledItkLaserReady);
            this.grpInterlockParam.Controls.Add(this.lblItkModeKey);
            this.grpInterlockParam.Controls.Add(this.ledItkModeKey);
            this.grpInterlockParam.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpInterlockParam.Location = new System.Drawing.Point(1351, 576);
            this.grpInterlockParam.Name = "grpInterlockParam";
            this.grpInterlockParam.Size = new System.Drawing.Size(278, 243);
            this.grpInterlockParam.TabIndex = 6;
            this.grpInterlockParam.TabStop = false;
            this.grpInterlockParam.Text = "Interlock Parameters";
            // 
            // lblItkEmsAllOk
            // 
            this.lblItkEmsAllOk.Location = new System.Drawing.Point(25, 142);
            this.lblItkEmsAllOk.Name = "lblItkEmsAllOk";
            this.lblItkEmsAllOk.Size = new System.Drawing.Size(170, 18);
            this.lblItkEmsAllOk.TabIndex = 2;
            this.lblItkEmsAllOk.Text = "EMS Switch All OK";
            this.lblItkEmsAllOk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkEmsAllOk
            // 
            this.ledItkEmsAllOk.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkEmsAllOk.Location = new System.Drawing.Point(202, 142);
            this.ledItkEmsAllOk.Name = "ledItkEmsAllOk";
            this.ledItkEmsAllOk.Size = new System.Drawing.Size(18, 18);
            this.ledItkEmsAllOk.TabIndex = 3;
            // 
            // lblItkAjinMcAllOk
            // 
            this.lblItkAjinMcAllOk.Location = new System.Drawing.Point(25, 119);
            this.lblItkAjinMcAllOk.Name = "lblItkAjinMcAllOk";
            this.lblItkAjinMcAllOk.Size = new System.Drawing.Size(170, 18);
            this.lblItkAjinMcAllOk.TabIndex = 2;
            this.lblItkAjinMcAllOk.Text = "AJIN MC All Ok";
            this.lblItkAjinMcAllOk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkAjinMcAllOk
            // 
            this.ledItkAjinMcAllOk.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkAjinMcAllOk.Location = new System.Drawing.Point(202, 119);
            this.ledItkAjinMcAllOk.Name = "ledItkAjinMcAllOk";
            this.ledItkAjinMcAllOk.Size = new System.Drawing.Size(18, 18);
            this.ledItkAjinMcAllOk.TabIndex = 3;
            // 
            // lblItkAcsMcAllOk
            // 
            this.lblItkAcsMcAllOk.Location = new System.Drawing.Point(25, 96);
            this.lblItkAcsMcAllOk.Name = "lblItkAcsMcAllOk";
            this.lblItkAcsMcAllOk.Size = new System.Drawing.Size(170, 18);
            this.lblItkAcsMcAllOk.TabIndex = 2;
            this.lblItkAcsMcAllOk.Text = "ACS MC All Ok";
            this.lblItkAcsMcAllOk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkAcsMcAllOk
            // 
            this.ledItkAcsMcAllOk.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkAcsMcAllOk.Location = new System.Drawing.Point(202, 96);
            this.ledItkAcsMcAllOk.Name = "ledItkAcsMcAllOk";
            this.ledItkAcsMcAllOk.Size = new System.Drawing.Size(18, 18);
            this.ledItkAcsMcAllOk.TabIndex = 3;
            // 
            // lblItkStageMoveOk
            // 
            this.lblItkStageMoveOk.Location = new System.Drawing.Point(25, 73);
            this.lblItkStageMoveOk.Name = "lblItkStageMoveOk";
            this.lblItkStageMoveOk.Size = new System.Drawing.Size(170, 18);
            this.lblItkStageMoveOk.TabIndex = 2;
            this.lblItkStageMoveOk.Text = "Stage Move Ok";
            this.lblItkStageMoveOk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkStageMoveOk
            // 
            this.ledItkStageMoveOk.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkStageMoveOk.Location = new System.Drawing.Point(202, 73);
            this.ledItkStageMoveOk.Name = "ledItkStageMoveOk";
            this.ledItkStageMoveOk.Size = new System.Drawing.Size(18, 18);
            this.ledItkStageMoveOk.TabIndex = 3;
            // 
            // lblItkLaserReady
            // 
            this.lblItkLaserReady.Location = new System.Drawing.Point(25, 50);
            this.lblItkLaserReady.Name = "lblItkLaserReady";
            this.lblItkLaserReady.Size = new System.Drawing.Size(170, 18);
            this.lblItkLaserReady.TabIndex = 2;
            this.lblItkLaserReady.Text = "Laser All Ready";
            this.lblItkLaserReady.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkLaserReady
            // 
            this.ledItkLaserReady.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkLaserReady.Location = new System.Drawing.Point(202, 50);
            this.ledItkLaserReady.Name = "ledItkLaserReady";
            this.ledItkLaserReady.Size = new System.Drawing.Size(18, 18);
            this.ledItkLaserReady.TabIndex = 3;
            // 
            // lblItkModeKey
            // 
            this.lblItkModeKey.Location = new System.Drawing.Point(25, 27);
            this.lblItkModeKey.Name = "lblItkModeKey";
            this.lblItkModeKey.Size = new System.Drawing.Size(170, 18);
            this.lblItkModeKey.TabIndex = 2;
            this.lblItkModeKey.Text = "Auto/Teach Key";
            this.lblItkModeKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ledItkModeKey
            // 
            this.ledItkModeKey.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledItkModeKey.Location = new System.Drawing.Point(202, 27);
            this.ledItkModeKey.Name = "ledItkModeKey";
            this.ledItkModeKey.Size = new System.Drawing.Size(18, 18);
            this.ledItkModeKey.TabIndex = 3;
            // 
            // grpLogs
            // 
            this.grpLogs.Controls.Add(this.panel1);
            this.grpLogs.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.grpLogs.Location = new System.Drawing.Point(752, 128);
            this.grpLogs.Name = "grpLogs";
            this.grpLogs.Size = new System.Drawing.Size(877, 442);
            this.grpLogs.TabIndex = 8;
            this.grpLogs.TabStop = false;
            this.grpLogs.Text = "Notes";
            // 
            // OverviewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1641, 853);
            this.Controls.Add(this.grpLogs);
            this.Controls.Add(this.grpInterlockParam);
            this.Controls.Add(this.grpCommunications);
            this.Controls.Add(this.grpAxes);
            this.Controls.Add(this.grpInputs);
            this.Controls.Add(this.grpOutputs);
            this.Controls.Add(this.grpNotes);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverviewForm";
            this.Text = "Overview";
            this.grpCommunications.ResumeLayout(false);
            this.grpAxes.ResumeLayout(false);
            this.grpInputs.ResumeLayout(false);
            this.grpOutputs.ResumeLayout(false);
            this.grpNotes.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpInterlockParam.ResumeLayout(false);
            this.grpLogs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Label lblHdrAxis;
        private Label lblHdrPos;
        private Label lblHdrAct;
        private Label lblHdrCmd;
        private Label lblHdrRms;
        private Label lblHdrLed;
        private Label lblRx;
        private Label lblMy;
        private Label lblMz;
        private Label lblTh;
        private Label lblPmF1;
        private Label lblPmF2;
        private Label lblPmS1;
        private Label lblPmS2;
        private Label lblPmR1;
        private Label lblPmR2;
        private Label lblUiF1;
        private Label lblUiF2;
        private Label lblUiR1;
        private Label lblUiR2;
        private Label lblUiS1;
        private Label lblUiS2;
        private Label lblGrip;
        private Label lblPmEms;
        private Label lblCh1;
        private Label lblCh2;
        private Label lblCh3;
        private Label lblMode;
        private Label lblDf1;
        private Label lblDf2;
        private Label lblDr1;
        private Label lblDr2;
        private Label lblDs1;
        private Label lblDs2;
        private Label lblLaser;
        private Label lblAcs1;
        private Label lblAcs2Y;
        private Label lblAcs2X;
        private Label lblRot1;
        private Label lblRot2;
        private Label lblRot2M;
        private Label lblO1;
        private Label lblO2;
        private Label lblO3;
        private Label lblO4;
        private Label lblO5;
        private Label lblO6;
        private Label lblO7;
        private Label lblMc2ndXO;
        private Label lblMc2ndYO;
        private Label lblMc1stO;
        private Button btnACSMc2_OutputXOn;
        private Button btnACSMc2_OutputYOn;
        private Button btnACSMc2_OutputXOff;
        private Button btnACSMc2_OutputYOff;
        private Button btnACSMc1_OutputOn;
        private Label ledACSMc2_OutputXState;
        private Button btnACSMc1_OutputOff;
        private Label ledACSMc2_OutputYState;
        private Label ledACSMc1_OutputState;
        private Label lblMc2ndZO;
        private Button btnAjinMc2_OutputZOn;
        private Button btnAjinMc2_OutputZOff;
        private Label ledAjinMc2_OutputZState;
        private Label label2;
        private Button btnAjinMc2_OutputTOn;
        private Button btnAjinMc2_OutputTOff;
        private Label ledAjinMc2_OutputTState;
        private Label lblLaserBoxAirVv;
        private Label lblY2MotorAirVv;
        private Label lblXMotorAirVv;
        private Label lblY1MotorAirVv;
        private Button btnLaserBox_AirCoolingVvOpen;
        private Button btnY2Motor_AirCoolingVvOpen;
        private Button btnXMotor_AirCoolingVvOpen;
        private Button btnLaserBox_AirCoolingVvClose;
        private Button btnY1Motor_AirCoolingVvOpen;
        private Button btnY2Motor_AirCoolingVvClose;
        private Button btnXMotor_AirCoolingVvClose;
        private Button btnY1Motor_AirCoolingVvClose;
        private Label ledLaserBox_AirCoolingVvState;
        private Label ledY2Motor_AirCoolingVvState;
        private Label ledXMotor_AirCoolingVvState;
        private Label ledY1Motor_AirCoolingVvState;
        private Label label3;
        private Label ledTheta_Homed;
        private Label ledMaintZ_Homed;
        private Label ledMainY_Homed;
        private Label ledReviewX_Homed;
        private GroupBox grpInterlockParam;
        private Label lblItkEmsAllOk;
        private Label ledItkEmsAllOk;
        private Label lblItkAjinMcAllOk;
        private Label ledItkAjinMcAllOk;
        private Label lblItkAcsMcAllOk;
        private Label ledItkAcsMcAllOk;
        private Label lblItkStageMoveOk;
        private Label ledItkStageMoveOk;
        private Label lblItkLaserReady;
        private Label ledItkLaserReady;
        private Label lblItkModeKey;
        private Label ledItkModeKey;
        private GroupBox grpLogs;
    }
}
