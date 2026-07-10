namespace StageWin.Etc
{
    partial class TowerLampForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbLampRed = new System.Windows.Forms.Label();
            this.lbLampGreen = new System.Windows.Forms.Label();
            this.lbLampYellow = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBuzzerOff = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(452, 146);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.lbLampRed, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbLampGreen, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbLampYellow, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(452, 146);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // lbLampRed
            // 
            this.lbLampRed.BackColor = System.Drawing.Color.Gray;
            this.lbLampRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLampRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLampRed.Location = new System.Drawing.Point(3, 0);
            this.lbLampRed.Name = "lbLampRed";
            this.lbLampRed.Size = new System.Drawing.Size(144, 146);
            this.lbLampRed.TabIndex = 0;
            // 
            // lbLampGreen
            // 
            this.lbLampGreen.BackColor = System.Drawing.Color.Gray;
            this.lbLampGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLampGreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLampGreen.Location = new System.Drawing.Point(303, 0);
            this.lbLampGreen.Name = "lbLampGreen";
            this.lbLampGreen.Size = new System.Drawing.Size(146, 146);
            this.lbLampGreen.TabIndex = 2;
            // 
            // lbLampYellow
            // 
            this.lbLampYellow.BackColor = System.Drawing.Color.Gray;
            this.lbLampYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLampYellow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLampYellow.Location = new System.Drawing.Point(153, 0);
            this.lbLampYellow.Name = "lbLampYellow";
            this.lbLampYellow.Size = new System.Drawing.Size(144, 146);
            this.lbLampYellow.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnBuzzerOff, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(458, 305);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnBuzzerOff
            // 
            this.btnBuzzerOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBuzzerOff.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnBuzzerOff.Location = new System.Drawing.Point(3, 155);
            this.btnBuzzerOff.Name = "btnBuzzerOff";
            this.btnBuzzerOff.Size = new System.Drawing.Size(452, 147);
            this.btnBuzzerOff.TabIndex = 1;
            this.btnBuzzerOff.Text = "Buzzer OFF";
            this.btnBuzzerOff.UseVisualStyleBackColor = true;
            // 
            // TowerLampForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 305);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TowerLampForm";
            this.Text = "r";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbLampRed;
        private System.Windows.Forms.Label lbLampYellow;
        private System.Windows.Forms.Label lbLampGreen;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnBuzzerOff;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}