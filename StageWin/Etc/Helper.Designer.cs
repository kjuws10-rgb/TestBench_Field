namespace StageWin.Etc
{
    partial class Helper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblProcessMap = new System.Windows.Forms.Label();
            this.picProcMap2 = new System.Windows.Forms.PictureBox();
            this.picProcMap1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picProcMap2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProcMap1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProcessMap
            // 
            this.lblProcessMap.AutoSize = true;
            this.lblProcessMap.Location = new System.Drawing.Point(12, 9);
            this.lblProcessMap.Name = "lblProcessMap";
            this.lblProcessMap.Size = new System.Drawing.Size(97, 12);
            this.lblProcessMap.TabIndex = 9;
            this.lblProcessMap.Text = "※ Process Map";
            // 
            // picProcMap2
            // 
            this.picProcMap2.Location = new System.Drawing.Point(301, 29);
            this.picProcMap2.Name = "picProcMap2";
            this.picProcMap2.Size = new System.Drawing.Size(265, 330);
            this.picProcMap2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picProcMap2.TabIndex = 7;
            this.picProcMap2.TabStop = false;
            // 
            // picProcMap1
            // 
            this.picProcMap1.Location = new System.Drawing.Point(12, 29);
            this.picProcMap1.Name = "picProcMap1";
            this.picProcMap1.Size = new System.Drawing.Size(283, 330);
            this.picProcMap1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picProcMap1.TabIndex = 8;
            this.picProcMap1.TabStop = false;
            // 
            // Helper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblProcessMap);
            this.Controls.Add(this.picProcMap2);
            this.Controls.Add(this.picProcMap1);
            this.Name = "Helper";
            this.Text = "Helper";
            ((System.ComponentModel.ISupportInitialize)(this.picProcMap2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProcMap1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProcessMap;
        private System.Windows.Forms.PictureBox picProcMap2;
        private System.Windows.Forms.PictureBox picProcMap1;
    }
}