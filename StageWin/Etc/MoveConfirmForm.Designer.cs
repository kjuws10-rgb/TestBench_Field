using System.Drawing;
using System.Windows.Forms;

namespace StageWin.Etc
{
    partial class MoveConfirmForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblCur;
        private Label lblTgt;
        private TextBox tbCurX, tbCurY, tbTgtX, tbTgtY;
        private Button btnYes, btnNo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCur = new System.Windows.Forms.Label();
            this.lblTgt = new System.Windows.Forms.Label();
            this.tbCurX = new System.Windows.Forms.TextBox();
            this.tbCurY = new System.Windows.Forms.TextBox();
            this.tbTgtX = new System.Windows.Forms.TextBox();
            this.tbTgtY = new System.Windows.Forms.TextBox();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(37, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Move";
            // 
            // lblCur
            // 
            this.lblCur.AutoSize = true;
            this.lblCur.Location = new System.Drawing.Point(12, 44);
            this.lblCur.Name = "lblCur";
            this.lblCur.Size = new System.Drawing.Size(76, 15);
            this.lblCur.TabIndex = 1;
            this.lblCur.Text = "Current (X,Y)";
            // 
            // lblTgt
            // 
            this.lblTgt.AutoSize = true;
            this.lblTgt.Location = new System.Drawing.Point(12, 76);
            this.lblTgt.Name = "lblTgt";
            this.lblTgt.Size = new System.Drawing.Size(73, 15);
            this.lblTgt.TabIndex = 2;
            this.lblTgt.Text = "Target  (X,Y)";
            // 
            // tbCurX
            // 
            this.tbCurX.Location = new System.Drawing.Point(110, 40);
            this.tbCurX.Name = "tbCurX";
            this.tbCurX.ReadOnly = true;
            this.tbCurX.Size = new System.Drawing.Size(90, 23);
            this.tbCurX.TabIndex = 3;
            this.tbCurX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbCurY
            // 
            this.tbCurY.Location = new System.Drawing.Point(206, 40);
            this.tbCurY.Name = "tbCurY";
            this.tbCurY.ReadOnly = true;
            this.tbCurY.Size = new System.Drawing.Size(90, 23);
            this.tbCurY.TabIndex = 4;
            this.tbCurY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbTgtX
            // 
            this.tbTgtX.Location = new System.Drawing.Point(110, 72);
            this.tbTgtX.Name = "tbTgtX";
            this.tbTgtX.ReadOnly = true;
            this.tbTgtX.Size = new System.Drawing.Size(90, 23);
            this.tbTgtX.TabIndex = 5;
            this.tbTgtX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbTgtY
            // 
            this.tbTgtY.Location = new System.Drawing.Point(206, 72);
            this.tbTgtY.Name = "tbTgtY";
            this.tbTgtY.ReadOnly = true;
            this.tbTgtY.Size = new System.Drawing.Size(90, 23);
            this.tbTgtY.TabIndex = 6;
            this.tbTgtY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnYes
            // 
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(156, 110);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(80, 26);
            this.btnYes.TabIndex = 7;
            this.btnYes.Text = "예";
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(251, 110);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(80, 26);
            this.btnNo.TabIndex = 8;
            this.btnNo.Text = "아니오";
            // 
            // MoveConfirmForm
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(360, 160);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblCur);
            this.Controls.Add(this.lblTgt);
            this.Controls.Add(this.tbCurX);
            this.Controls.Add(this.tbCurY);
            this.Controls.Add(this.tbTgtX);
            this.Controls.Add(this.tbTgtY);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.btnNo);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MoveConfirmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move Confirm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}