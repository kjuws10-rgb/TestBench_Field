using System.Windows.Forms;
using System.Drawing;
using StageWin.Etc;

namespace StageWin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.bottomTabs = new System.Windows.Forms.TabControl();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.tabManual = new System.Windows.Forms.TabPage();
            this.ManualTabs = new System.Windows.Forms.TabControl();
            this.tabMotion = new System.Windows.Forms.TabPage();
            this.tabESC = new System.Windows.Forms.TabPage();
            this.tabStageOffset = new System.Windows.Forms.TabPage();
            this.lblManualLog = new System.Windows.Forms.Label();
            this.lstManual = new System.Windows.Forms.ListBox();
            this.tabRecipe = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.lstRecipeScanLog = new System.Windows.Forms.ListBox();
            this.btnRefreshLocalList = new System.Windows.Forms.Button();
            this.btnDownloadScanToLocal = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlScanIf = new System.Windows.Forms.Panel();
            this.lblScanIf = new System.Windows.Forms.Label();
            this.btnReqRecipeList = new System.Windows.Forms.Button();
            this.txtNewRecipe = new System.Windows.Forms.TextBox();
            this.btnReqRecipeAdd = new System.Windows.Forms.Button();
            this.lstRecipeList = new System.Windows.Forms.ListBox();
            this.lstScanRecipeList = new System.Windows.Forms.ListBox();
            this.pnlRecipeHost = new System.Windows.Forms.Panel();
            this.RecipeScanTabs = new System.Windows.Forms.TabControl();
            this.tabRecipeEdit = new System.Windows.Forms.TabPage();
            this.tabOpticControl = new System.Windows.Forms.TabPage();
            this.tabVisionControl = new System.Windows.Forms.TabPage();
            this.lstTcpLog = new System.Windows.Forms.ListBox();
            this.lblVisionDesc = new System.Windows.Forms.Label();
            this.btnReq2ndAlign = new System.Windows.Forms.Button();
            this.lblVisionIf = new System.Windows.Forms.Label();
            this.btnReqAlign = new System.Windows.Forms.Button();
            this.tabIO = new System.Windows.Forms.TabPage();
            this.pnlWagoIo = new System.Windows.Forms.Panel();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlLogs = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.lblLogs = new System.Windows.Forms.Label();
            this.pnlAlarms = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAlarms = new System.Windows.Forms.Label();
            this.gridAlarms = new System.Windows.Forms.DataGridView();
            this.lstStageRcpList = new System.Windows.Forms.ListBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnForceAbort = new System.Windows.Forms.Button();
            this.pnlMode = new System.Windows.Forms.Panel();
            this.lblRecipeName = new System.Windows.Forms.Label();
            this.lblEquipState = new System.Windows.Forms.Label();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbtnModeManual = new System.Windows.Forms.RadioButton();
            this.rbtnModeSemiAuto = new System.Windows.Forms.RadioButton();
            this.rbtnModeAuto = new System.Windows.Forms.RadioButton();
            this.pnlPcTcp = new System.Windows.Forms.Panel();
            this.lblScan = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblVision = new System.Windows.Forms.Label();
            this.ledServer = new System.Windows.Forms.Label();
            this.ledScan = new System.Windows.Forms.Label();
            this.ledVision = new System.Windows.Forms.Label();
            this.lstSystem = new System.Windows.Forms.ListBox();
            this.btnAlarmReset = new System.Windows.Forms.Button();
            this.pnlTowerLamp = new System.Windows.Forms.Panel();
            this.btnAutoParameters = new System.Windows.Forms.Button();
            this.bottomTabs.SuspendLayout();
            this.tabManual.SuspendLayout();
            this.ManualTabs.SuspendLayout();
            this.tabRecipe.SuspendLayout();
            this.pnlScanIf.SuspendLayout();
            this.pnlRecipeHost.SuspendLayout();
            this.RecipeScanTabs.SuspendLayout();
            this.tabVisionControl.SuspendLayout();
            this.tabIO.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlLogs.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pnlAlarms.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.pnlMode.SuspendLayout();
            this.gbMode.SuspendLayout();
            this.pnlPcTcp.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomTabs
            // 
            this.bottomTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomTabs.Controls.Add(this.tabOverview);
            this.bottomTabs.Controls.Add(this.tabManual);
            this.bottomTabs.Controls.Add(this.tabRecipe);
            this.bottomTabs.Controls.Add(this.tabIO);
            this.bottomTabs.Controls.Add(this.tabLogs);
            this.bottomTabs.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.bottomTabs.Location = new System.Drawing.Point(0, 92);
            this.bottomTabs.Name = "bottomTabs";
            this.bottomTabs.SelectedIndex = 0;
            this.bottomTabs.Size = new System.Drawing.Size(1649, 846);
            this.bottomTabs.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Location = new System.Drawing.Point(4, 24);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Size = new System.Drawing.Size(1641, 818);
            this.tabOverview.TabIndex = 0;
            this.tabOverview.Text = "① Overview";
            // 
            // tabManual
            // 
            this.tabManual.Controls.Add(this.ManualTabs);
            this.tabManual.Controls.Add(this.lblManualLog);
            this.tabManual.Controls.Add(this.lstManual);
            this.tabManual.Location = new System.Drawing.Point(4, 24);
            this.tabManual.Name = "tabManual";
            this.tabManual.Size = new System.Drawing.Size(1641, 818);
            this.tabManual.TabIndex = 1;
            this.tabManual.Text = "② Manual";
            // 
            // ManualTabs
            // 
            this.ManualTabs.Controls.Add(this.tabMotion);
            this.ManualTabs.Controls.Add(this.tabESC);
            this.ManualTabs.Controls.Add(this.tabStageOffset);
            this.ManualTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManualTabs.Location = new System.Drawing.Point(0, 0);
            this.ManualTabs.Name = "ManualTabs";
            this.ManualTabs.SelectedIndex = 0;
            this.ManualTabs.Size = new System.Drawing.Size(1641, 818);
            this.ManualTabs.TabIndex = 4;
            // 
            // tabMotion
            // 
            this.tabMotion.Location = new System.Drawing.Point(4, 24);
            this.tabMotion.Name = "tabMotion";
            this.tabMotion.Padding = new System.Windows.Forms.Padding(3);
            this.tabMotion.Size = new System.Drawing.Size(1633, 790);
            this.tabMotion.TabIndex = 0;
            this.tabMotion.Text = "Motion";
            this.tabMotion.UseVisualStyleBackColor = true;
            // 
            // tabESC
            // 
            this.tabESC.Location = new System.Drawing.Point(4, 22);
            this.tabESC.Name = "tabESC";
            this.tabESC.Padding = new System.Windows.Forms.Padding(3);
            this.tabESC.Size = new System.Drawing.Size(1633, 792);
            this.tabESC.TabIndex = 1;
            this.tabESC.Text = "ESC";
            this.tabESC.UseVisualStyleBackColor = true;
            // 
            // tabStageOffset
            // 
            this.tabStageOffset.Location = new System.Drawing.Point(4, 22);
            this.tabStageOffset.Name = "tabStageOffset";
            this.tabStageOffset.Padding = new System.Windows.Forms.Padding(3);
            this.tabStageOffset.Size = new System.Drawing.Size(1633, 792);
            this.tabStageOffset.TabIndex = 2;
            this.tabStageOffset.Text = "Stage Offset";
            this.tabStageOffset.UseVisualStyleBackColor = true;
            // 
            // lblManualLog
            // 
            this.lblManualLog.AutoSize = true;
            this.lblManualLog.Location = new System.Drawing.Point(12, 602);
            this.lblManualLog.Name = "lblManualLog";
            this.lblManualLog.Size = new System.Drawing.Size(71, 15);
            this.lblManualLog.TabIndex = 2;
            this.lblManualLog.Text = "Manual Log";
            // 
            // lstManual
            // 
            this.lstManual.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstManual.HorizontalScrollbar = true;
            this.lstManual.ItemHeight = 15;
            this.lstManual.Location = new System.Drawing.Point(14, 617);
            this.lstManual.Name = "lstManual";
            this.lstManual.Size = new System.Drawing.Size(1029, 169);
            this.lstManual.TabIndex = 3;
            // 
            // tabRecipe
            // 
            this.tabRecipe.Controls.Add(this.label16);
            this.tabRecipe.Controls.Add(this.lstRecipeScanLog);
            this.tabRecipe.Controls.Add(this.btnRefreshLocalList);
            this.tabRecipe.Controls.Add(this.btnDownloadScanToLocal);
            this.tabRecipe.Controls.Add(this.label2);
            this.tabRecipe.Controls.Add(this.label1);
            this.tabRecipe.Controls.Add(this.pnlScanIf);
            this.tabRecipe.Controls.Add(this.lstRecipeList);
            this.tabRecipe.Controls.Add(this.lstScanRecipeList);
            this.tabRecipe.Controls.Add(this.pnlRecipeHost);
            this.tabRecipe.Location = new System.Drawing.Point(4, 24);
            this.tabRecipe.Name = "tabRecipe";
            this.tabRecipe.Size = new System.Drawing.Size(1641, 818);
            this.tabRecipe.TabIndex = 2;
            this.tabRecipe.Text = "③ Recipe + Scan & Vision I/F";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(10, 617);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(256, 23);
            this.label16.TabIndex = 7;
            this.label16.Text = "Recipe && Scan Log";
            // 
            // lstRecipeScanLog
            // 
            this.lstRecipeScanLog.HorizontalScrollbar = true;
            this.lstRecipeScanLog.ItemHeight = 15;
            this.lstRecipeScanLog.Location = new System.Drawing.Point(10, 643);
            this.lstRecipeScanLog.Name = "lstRecipeScanLog";
            this.lstRecipeScanLog.Size = new System.Drawing.Size(320, 184);
            this.lstRecipeScanLog.TabIndex = 2;
            // 
            // btnRefreshLocalList
            // 
            this.btnRefreshLocalList.Location = new System.Drawing.Point(271, 381);
            this.btnRefreshLocalList.Name = "btnRefreshLocalList";
            this.btnRefreshLocalList.Size = new System.Drawing.Size(59, 26);
            this.btnRefreshLocalList.TabIndex = 6;
            this.btnRefreshLocalList.Text = "Refresh";
            // 
            // btnDownloadScanToLocal
            // 
            this.btnDownloadScanToLocal.Location = new System.Drawing.Point(191, 346);
            this.btnDownloadScanToLocal.Name = "btnDownloadScanToLocal";
            this.btnDownloadScanToLocal.Size = new System.Drawing.Size(139, 26);
            this.btnDownloadScanToLocal.TabIndex = 5;
            this.btnDownloadScanToLocal.Text = "▼ Local Download";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(256, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sharing RecipeList (Stage ↔ Scan PC)";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 388);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local RecipeList (Stage PC)";
            // 
            // pnlScanIf
            // 
            this.pnlScanIf.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlScanIf.Controls.Add(this.lblScanIf);
            this.pnlScanIf.Controls.Add(this.btnReqRecipeList);
            this.pnlScanIf.Controls.Add(this.txtNewRecipe);
            this.pnlScanIf.Controls.Add(this.btnReqRecipeAdd);
            this.pnlScanIf.Location = new System.Drawing.Point(10, 10);
            this.pnlScanIf.Name = "pnlScanIf";
            this.pnlScanIf.Size = new System.Drawing.Size(320, 120);
            this.pnlScanIf.TabIndex = 0;
            // 
            // lblScanIf
            // 
            this.lblScanIf.AutoSize = true;
            this.lblScanIf.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblScanIf.Location = new System.Drawing.Point(10, 8);
            this.lblScanIf.Name = "lblScanIf";
            this.lblScanIf.Size = new System.Drawing.Size(53, 15);
            this.lblScanIf.TabIndex = 0;
            this.lblScanIf.Text = "Scan I/F";
            // 
            // btnReqRecipeList
            // 
            this.btnReqRecipeList.Location = new System.Drawing.Point(14, 34);
            this.btnReqRecipeList.Name = "btnReqRecipeList";
            this.btnReqRecipeList.Size = new System.Drawing.Size(290, 26);
            this.btnReqRecipeList.TabIndex = 1;
            this.btnReqRecipeList.Text = "Request Recipe List";
            // 
            // txtNewRecipe
            // 
            this.txtNewRecipe.Location = new System.Drawing.Point(14, 68);
            this.txtNewRecipe.Name = "txtNewRecipe";
            this.txtNewRecipe.Size = new System.Drawing.Size(190, 23);
            this.txtNewRecipe.TabIndex = 2;
            // 
            // btnReqRecipeAdd
            // 
            this.btnReqRecipeAdd.Location = new System.Drawing.Point(210, 66);
            this.btnReqRecipeAdd.Name = "btnReqRecipeAdd";
            this.btnReqRecipeAdd.Size = new System.Drawing.Size(94, 26);
            this.btnReqRecipeAdd.TabIndex = 3;
            this.btnReqRecipeAdd.Text = "Add";
            // 
            // lstRecipeList
            // 
            this.lstRecipeList.HorizontalScrollbar = true;
            this.lstRecipeList.ItemHeight = 15;
            this.lstRecipeList.Location = new System.Drawing.Point(10, 413);
            this.lstRecipeList.Name = "lstRecipeList";
            this.lstRecipeList.Size = new System.Drawing.Size(320, 184);
            this.lstRecipeList.TabIndex = 1;
            // 
            // lstScanRecipeList
            // 
            this.lstScanRecipeList.HorizontalScrollbar = true;
            this.lstScanRecipeList.ItemHeight = 15;
            this.lstScanRecipeList.Location = new System.Drawing.Point(10, 168);
            this.lstScanRecipeList.Name = "lstScanRecipeList";
            this.lstScanRecipeList.Size = new System.Drawing.Size(320, 169);
            this.lstScanRecipeList.TabIndex = 1;
            // 
            // pnlRecipeHost
            // 
            this.pnlRecipeHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRecipeHost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRecipeHost.Controls.Add(this.RecipeScanTabs);
            this.pnlRecipeHost.Location = new System.Drawing.Point(336, 10);
            this.pnlRecipeHost.Name = "pnlRecipeHost";
            this.pnlRecipeHost.Size = new System.Drawing.Size(1298, 796);
            this.pnlRecipeHost.TabIndex = 2;
            // 
            // RecipeScanTabs
            // 
            this.RecipeScanTabs.Controls.Add(this.tabRecipeEdit);
            this.RecipeScanTabs.Controls.Add(this.tabOpticControl);
            this.RecipeScanTabs.Controls.Add(this.tabVisionControl);
            this.RecipeScanTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecipeScanTabs.Location = new System.Drawing.Point(0, 0);
            this.RecipeScanTabs.Name = "RecipeScanTabs";
            this.RecipeScanTabs.SelectedIndex = 0;
            this.RecipeScanTabs.Size = new System.Drawing.Size(1296, 794);
            this.RecipeScanTabs.TabIndex = 0;
            // 
            // tabRecipeEdit
            // 
            this.tabRecipeEdit.Location = new System.Drawing.Point(4, 24);
            this.tabRecipeEdit.Name = "tabRecipeEdit";
            this.tabRecipeEdit.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecipeEdit.Size = new System.Drawing.Size(1288, 766);
            this.tabRecipeEdit.TabIndex = 0;
            this.tabRecipeEdit.Text = "Recipe Edit";
            this.tabRecipeEdit.UseVisualStyleBackColor = true;
            // 
            // tabOpticControl
            // 
            this.tabOpticControl.Location = new System.Drawing.Point(4, 22);
            this.tabOpticControl.Name = "tabOpticControl";
            this.tabOpticControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabOpticControl.Size = new System.Drawing.Size(1288, 768);
            this.tabOpticControl.TabIndex = 1;
            this.tabOpticControl.Text = "Optic Control";
            this.tabOpticControl.UseVisualStyleBackColor = true;
            // 
            // tabVisionControl
            // 
            this.tabVisionControl.Controls.Add(this.lstTcpLog);
            this.tabVisionControl.Controls.Add(this.lblVisionDesc);
            this.tabVisionControl.Controls.Add(this.btnReq2ndAlign);
            this.tabVisionControl.Controls.Add(this.lblVisionIf);
            this.tabVisionControl.Controls.Add(this.btnReqAlign);
            this.tabVisionControl.Location = new System.Drawing.Point(4, 22);
            this.tabVisionControl.Name = "tabVisionControl";
            this.tabVisionControl.Size = new System.Drawing.Size(1288, 768);
            this.tabVisionControl.TabIndex = 2;
            this.tabVisionControl.Text = "Vision Control";
            this.tabVisionControl.UseVisualStyleBackColor = true;
            // 
            // lstTcpLog
            // 
            this.lstTcpLog.HorizontalScrollbar = true;
            this.lstTcpLog.ItemHeight = 15;
            this.lstTcpLog.Location = new System.Drawing.Point(23, 448);
            this.lstTcpLog.Name = "lstTcpLog";
            this.lstTcpLog.Size = new System.Drawing.Size(1231, 304);
            this.lstTcpLog.TabIndex = 9;
            // 
            // lblVisionDesc
            // 
            this.lblVisionDesc.AutoSize = true;
            this.lblVisionDesc.Location = new System.Drawing.Point(21, 429);
            this.lblVisionDesc.Name = "lblVisionDesc";
            this.lblVisionDesc.Size = new System.Drawing.Size(68, 15);
            this.lblVisionDesc.TabIndex = 8;
            this.lblVisionDesc.Text = "Comm Log";
            // 
            // btnReq2ndAlign
            // 
            this.btnReq2ndAlign.Location = new System.Drawing.Point(23, 95);
            this.btnReq2ndAlign.Name = "btnReq2ndAlign";
            this.btnReq2ndAlign.Size = new System.Drawing.Size(320, 26);
            this.btnReq2ndAlign.TabIndex = 7;
            this.btnReq2ndAlign.Text = "Request 2nd Align";
            // 
            // lblVisionIf
            // 
            this.lblVisionIf.AutoSize = true;
            this.lblVisionIf.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblVisionIf.Location = new System.Drawing.Point(19, 18);
            this.lblVisionIf.Name = "lblVisionIf";
            this.lblVisionIf.Size = new System.Drawing.Size(60, 15);
            this.lblVisionIf.TabIndex = 4;
            this.lblVisionIf.Text = "Vision I/F";
            // 
            // btnReqAlign
            // 
            this.btnReqAlign.Location = new System.Drawing.Point(23, 44);
            this.btnReqAlign.Name = "btnReqAlign";
            this.btnReqAlign.Size = new System.Drawing.Size(320, 26);
            this.btnReqAlign.TabIndex = 5;
            this.btnReqAlign.Text = "Request Glass Align";
            // 
            // tabIO
            // 
            this.tabIO.Controls.Add(this.pnlWagoIo);
            this.tabIO.Location = new System.Drawing.Point(4, 24);
            this.tabIO.Name = "tabIO";
            this.tabIO.Size = new System.Drawing.Size(1641, 818);
            this.tabIO.TabIndex = 3;
            this.tabIO.Text = "④ I/O";
            // 
            // pnlWagoIo
            // 
            this.pnlWagoIo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlWagoIo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlWagoIo.Location = new System.Drawing.Point(8, 10);
            this.pnlWagoIo.Name = "pnlWagoIo";
            this.pnlWagoIo.Size = new System.Drawing.Size(1626, 800);
            this.pnlWagoIo.TabIndex = 1;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tableLayoutPanel1);
            this.tabLogs.Location = new System.Drawing.Point(4, 24);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Size = new System.Drawing.Size(1641, 818);
            this.tabLogs.TabIndex = 4;
            this.tabLogs.Text = "⑤ Logs & Alarms";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.pnlLogs, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlAlarms, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1641, 818);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // pnlLogs
            // 
            this.pnlLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLogs.Controls.Add(this.tableLayoutPanel2);
            this.pnlLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLogs.Location = new System.Drawing.Point(3, 3);
            this.pnlLogs.Name = "pnlLogs";
            this.pnlLogs.Size = new System.Drawing.Size(486, 812);
            this.pnlLogs.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnSaveLog, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lstLog, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblLogs, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(484, 810);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveLog.Location = new System.Drawing.Point(3, 783);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(478, 24);
            this.btnSaveLog.TabIndex = 2;
            this.btnSaveLog.Text = "Save Logs to CSV";
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.ItemHeight = 15;
            this.lstLog.Location = new System.Drawing.Point(3, 18);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(478, 759);
            this.lstLog.TabIndex = 1;
            // 
            // lblLogs
            // 
            this.lblLogs.AutoSize = true;
            this.lblLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogs.Location = new System.Drawing.Point(3, 0);
            this.lblLogs.Name = "lblLogs";
            this.lblLogs.Size = new System.Drawing.Size(478, 15);
            this.lblLogs.TabIndex = 0;
            this.lblLogs.Text = "Runtime Logs";
            // 
            // pnlAlarms
            // 
            this.pnlAlarms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAlarms.Controls.Add(this.tableLayoutPanel3);
            this.pnlAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAlarms.Location = new System.Drawing.Point(495, 3);
            this.pnlAlarms.Name = "pnlAlarms";
            this.pnlAlarms.Size = new System.Drawing.Size(1143, 812);
            this.pnlAlarms.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lblAlarms, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.gridAlarms, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1141, 810);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // lblAlarms
            // 
            this.lblAlarms.AutoSize = true;
            this.lblAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAlarms.Location = new System.Drawing.Point(3, 0);
            this.lblAlarms.Name = "lblAlarms";
            this.lblAlarms.Size = new System.Drawing.Size(1135, 15);
            this.lblAlarms.TabIndex = 0;
            this.lblAlarms.Text = "Alarms";
            // 
            // gridAlarms
            // 
            this.gridAlarms.AllowUserToAddRows = false;
            this.gridAlarms.AllowUserToDeleteRows = false;
            this.gridAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAlarms.Location = new System.Drawing.Point(3, 18);
            this.gridAlarms.MultiSelect = false;
            this.gridAlarms.Name = "gridAlarms";
            this.gridAlarms.ReadOnly = true;
            this.gridAlarms.RowHeadersVisible = false;
            this.gridAlarms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAlarms.Size = new System.Drawing.Size(1135, 789);
            this.gridAlarms.TabIndex = 1;
            // 
            // lstStageRcpList
            // 
            this.lstStageRcpList.HorizontalScrollbar = true;
            this.lstStageRcpList.ItemHeight = 12;
            this.lstStageRcpList.Location = new System.Drawing.Point(10, 382);
            this.lstStageRcpList.Name = "lstStageRcpList";
            this.lstStageRcpList.Size = new System.Drawing.Size(320, 244);
            this.lstStageRcpList.TabIndex = 1;
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.tableLayoutPanel4);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1674, 87);
            this.headerPanel.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 6;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel4.Controls.Add(this.btnForceAbort, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.pnlMode, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.pnlPcTcp, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.lstSystem, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnAlarmReset, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.pnlTowerLamp, 5, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1674, 87);
            this.tableLayoutPanel4.TabIndex = 14;
            // 
            // btnForceAbort
            // 
            this.btnForceAbort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnForceAbort.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnForceAbort.Location = new System.Drawing.Point(1324, 3);
            this.btnForceAbort.Name = "btnForceAbort";
            this.btnForceAbort.Size = new System.Drawing.Size(111, 81);
            this.btnForceAbort.TabIndex = 17;
            this.btnForceAbort.Text = "⚠ Force Abort";
            // 
            // pnlMode
            // 
            this.pnlMode.Controls.Add(this.btnAutoParameters);
            this.pnlMode.Controls.Add(this.lblRecipeName);
            this.pnlMode.Controls.Add(this.lblEquipState);
            this.pnlMode.Controls.Add(this.gbMode);
            this.pnlMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMode.Location = new System.Drawing.Point(3, 3);
            this.pnlMode.Name = "pnlMode";
            this.pnlMode.Size = new System.Drawing.Size(613, 81);
            this.pnlMode.TabIndex = 15;
            // 
            // lblRecipeName
            // 
            this.lblRecipeName.AutoSize = true;
            this.lblRecipeName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblRecipeName.Location = new System.Drawing.Point(254, 8);
            this.lblRecipeName.Name = "lblRecipeName";
            this.lblRecipeName.Size = new System.Drawing.Size(113, 15);
            this.lblRecipeName.TabIndex = 11;
            this.lblRecipeName.Text = "Recipe : No Select";
            // 
            // lblEquipState
            // 
            this.lblEquipState.AutoSize = true;
            this.lblEquipState.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.lblEquipState.Location = new System.Drawing.Point(3, 4);
            this.lblEquipState.Name = "lblEquipState";
            this.lblEquipState.Size = new System.Drawing.Size(207, 21);
            this.lblEquipState.TabIndex = 8;
            this.lblEquipState.Text = "EQUIPMENT STATE : IDLE";
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbtnModeManual);
            this.gbMode.Controls.Add(this.rbtnModeSemiAuto);
            this.gbMode.Controls.Add(this.rbtnModeAuto);
            this.gbMode.Font = new System.Drawing.Font("맑은 고딕", 8.25F);
            this.gbMode.Location = new System.Drawing.Point(6, 33);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(399, 45);
            this.gbMode.TabIndex = 10;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "OperationMode";
            // 
            // rbtnModeManual
            // 
            this.rbtnModeManual.AutoSize = true;
            this.rbtnModeManual.Location = new System.Drawing.Point(274, 20);
            this.rbtnModeManual.Name = "rbtnModeManual";
            this.rbtnModeManual.Size = new System.Drawing.Size(70, 17);
            this.rbtnModeManual.TabIndex = 0;
            this.rbtnModeManual.TabStop = true;
            this.rbtnModeManual.Text = "MANUAL";
            this.rbtnModeManual.UseVisualStyleBackColor = true;
            // 
            // rbtnModeSemiAuto
            // 
            this.rbtnModeSemiAuto.AutoSize = true;
            this.rbtnModeSemiAuto.Location = new System.Drawing.Point(156, 20);
            this.rbtnModeSemiAuto.Name = "rbtnModeSemiAuto";
            this.rbtnModeSemiAuto.Size = new System.Drawing.Size(50, 17);
            this.rbtnModeSemiAuto.TabIndex = 0;
            this.rbtnModeSemiAuto.TabStop = true;
            this.rbtnModeSemiAuto.Text = "SEMI";
            this.rbtnModeSemiAuto.UseVisualStyleBackColor = true;
            // 
            // rbtnModeAuto
            // 
            this.rbtnModeAuto.AutoSize = true;
            this.rbtnModeAuto.Location = new System.Drawing.Point(32, 20);
            this.rbtnModeAuto.Name = "rbtnModeAuto";
            this.rbtnModeAuto.Size = new System.Drawing.Size(55, 17);
            this.rbtnModeAuto.TabIndex = 0;
            this.rbtnModeAuto.TabStop = true;
            this.rbtnModeAuto.Text = "AUTO";
            this.rbtnModeAuto.UseVisualStyleBackColor = true;
            // 
            // pnlPcTcp
            // 
            this.pnlPcTcp.Controls.Add(this.lblScan);
            this.pnlPcTcp.Controls.Add(this.lblServer);
            this.pnlPcTcp.Controls.Add(this.lblVision);
            this.pnlPcTcp.Controls.Add(this.ledServer);
            this.pnlPcTcp.Controls.Add(this.ledScan);
            this.pnlPcTcp.Controls.Add(this.ledVision);
            this.pnlPcTcp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPcTcp.Location = new System.Drawing.Point(1207, 3);
            this.pnlPcTcp.Name = "pnlPcTcp";
            this.pnlPcTcp.Size = new System.Drawing.Size(111, 81);
            this.pnlPcTcp.TabIndex = 15;
            // 
            // lblScan
            // 
            this.lblScan.AutoSize = true;
            this.lblScan.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblScan.Location = new System.Drawing.Point(27, 34);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(68, 15);
            this.lblScan.TabIndex = 3;
            this.lblScan.Text = "SCAN LINK";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblServer.Location = new System.Drawing.Point(27, 7);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(76, 15);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "SERVER RUN";
            // 
            // lblVision
            // 
            this.lblVision.AutoSize = true;
            this.lblVision.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblVision.Location = new System.Drawing.Point(27, 60);
            this.lblVision.Name = "lblVision";
            this.lblVision.Size = new System.Drawing.Size(75, 15);
            this.lblVision.TabIndex = 4;
            this.lblVision.Text = "VISION LINK";
            // 
            // ledServer
            // 
            this.ledServer.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledServer.Location = new System.Drawing.Point(7, 4);
            this.ledServer.Name = "ledServer";
            this.ledServer.Size = new System.Drawing.Size(18, 18);
            this.ledServer.TabIndex = 5;
            this.ledServer.Click += new System.EventHandler(this.ledServer_Click);
            // 
            // ledScan
            // 
            this.ledScan.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledScan.Location = new System.Drawing.Point(7, 31);
            this.ledScan.Name = "ledScan";
            this.ledScan.Size = new System.Drawing.Size(18, 18);
            this.ledScan.TabIndex = 6;
            // 
            // ledVision
            // 
            this.ledVision.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ledVision.Location = new System.Drawing.Point(7, 57);
            this.ledVision.Name = "ledVision";
            this.ledVision.Size = new System.Drawing.Size(18, 18);
            this.ledVision.TabIndex = 7;
            // 
            // lstSystem
            // 
            this.lstSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSystem.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lstSystem.HorizontalScrollbar = true;
            this.lstSystem.ItemHeight = 15;
            this.lstSystem.Location = new System.Drawing.Point(622, 3);
            this.lstSystem.Name = "lstSystem";
            this.lstSystem.Size = new System.Drawing.Size(579, 81);
            this.lstSystem.TabIndex = 9;
            // 
            // btnAlarmReset
            // 
            this.btnAlarmReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAlarmReset.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnAlarmReset.Location = new System.Drawing.Point(1441, 3);
            this.btnAlarmReset.Name = "btnAlarmReset";
            this.btnAlarmReset.Size = new System.Drawing.Size(111, 81);
            this.btnAlarmReset.TabIndex = 11;
            this.btnAlarmReset.Text = "Alarm Reset";
            // 
            // pnlTowerLamp
            // 
            this.pnlTowerLamp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTowerLamp.Location = new System.Drawing.Point(1558, 3);
            this.pnlTowerLamp.Name = "pnlTowerLamp";
            this.pnlTowerLamp.Size = new System.Drawing.Size(113, 81);
            this.pnlTowerLamp.TabIndex = 16;
            // 
            // btnAutoParameters
            // 
            this.btnAutoParameters.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnAutoParameters.Location = new System.Drawing.Point(468, 40);
            this.btnAutoParameters.Name = "btnAutoParameters";
            this.btnAutoParameters.Size = new System.Drawing.Size(130, 38);
            this.btnAutoParameters.TabIndex = 12;
            this.btnAutoParameters.Text = "Auto Param";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1674, 947);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.bottomTabs);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Laser Test Bench";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.bottomTabs.ResumeLayout(false);
            this.tabManual.ResumeLayout(false);
            this.tabManual.PerformLayout();
            this.ManualTabs.ResumeLayout(false);
            this.tabRecipe.ResumeLayout(false);
            this.pnlScanIf.ResumeLayout(false);
            this.pnlScanIf.PerformLayout();
            this.pnlRecipeHost.ResumeLayout(false);
            this.RecipeScanTabs.ResumeLayout(false);
            this.tabVisionControl.ResumeLayout(false);
            this.tabVisionControl.PerformLayout();
            this.tabIO.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlLogs.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pnlAlarms.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.pnlMode.ResumeLayout(false);
            this.pnlMode.PerformLayout();
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.pnlPcTcp.ResumeLayout(false);
            this.pnlPcTcp.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.TabControl bottomTabs;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.TabPage tabManual;
        private System.Windows.Forms.TabPage tabRecipe;
        private System.Windows.Forms.TabPage tabIO;
        private System.Windows.Forms.TabPage tabLogs;
        private Label lblManualLog;
        private ListBox lstManual;

        // Recipe
        private Panel pnlScanIf;
        private Label lblScanIf;
        private Button btnReqRecipeList;
        private TextBox txtNewRecipe;
        private Button btnReqRecipeAdd;
        private ListBox lstScanRecipeList;
        private Panel pnlRecipeHost;
        private Panel pnlWagoIo;

        // Logs
        private Panel pnlLogs;
        private Label lblLogs;
        private ListBox lstLog;
        private Button btnSaveLog;

        private Panel pnlAlarms;
        private Label lblAlarms;
        private DataGridView gridAlarms;
        private Label label1;
        private ListBox lstRecipeList;
        private ListBox lstStageRcpList;
        private Panel headerPanel;
        private Label lblEquipState;
        private Label lblVision;
        private Label lblScan;
        private Label lblServer;
        private Label ledVision;
        private Label ledScan;
        private Label ledServer;
        private ListBox lstRecipeScanLog;
        private Label label2;
        private Button btnDownloadScanToLocal;
        private Button btnRefreshLocalList;
        private TabControl RecipeScanTabs;
        private TabPage tabRecipeEdit;
        private TabPage tabOpticControl;
        private Label label16;
        private ListBox lstSystem;
        private TabPage tabVisionControl;
        private Button btnReq2ndAlign;
        private Label lblVisionIf;
        private Button btnReqAlign;
        private ListBox lstTcpLog;
        private Label lblVisionDesc;
        private TabControl ManualTabs;
        private TabPage tabMotion;
        private TabPage tabESC;
        private TabPage tabStageOffset;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private GroupBox gbMode;
        private RadioButton rbtnModeManual;
        private RadioButton rbtnModeSemiAuto;
        private RadioButton rbtnModeAuto;
        private Button btnAlarmReset;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel pnlMode;
        private Panel pnlPcTcp;
        private Panel pnlTowerLamp;
        private Button btnForceAbort;
        private Label lblRecipeName;
        private Button btnAutoParameters;
    }
}