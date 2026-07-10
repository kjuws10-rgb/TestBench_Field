namespace StageWin.UI
{
    partial class IOForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_Input = new System.Windows.Forms.Panel();
            this.dgvInput = new System.Windows.Forms.DataGridView();
            this.IDX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DATA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SIG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnInputNextAll = new System.Windows.Forms.Button();
            this.btnInputNext = new System.Windows.Forms.Button();
            this.btnInputPrev = new System.Windows.Forms.Button();
            this.btnInputPrevAll = new System.Windows.Forms.Button();
            this.btnInputStatus = new System.Windows.Forms.Button();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.dgvOutput = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOutputNextAll = new System.Windows.Forms.Button();
            this.btnOutputStatus = new System.Windows.Forms.Button();
            this.btnOutputNext = new System.Windows.Forms.Button();
            this.btnOutputPrev = new System.Windows.Forms.Button();
            this.btnOutputPrevAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbConnectionStatus = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnForceOutputMode = new System.Windows.Forms.Button();
            this.panel_Input.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).BeginInit();
            this.panel_Output.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectionStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_Input
            // 
            this.panel_Input.Controls.Add(this.dgvInput);
            this.panel_Input.Location = new System.Drawing.Point(14, 32);
            this.panel_Input.Name = "panel_Input";
            this.panel_Input.Size = new System.Drawing.Size(570, 560);
            this.panel_Input.TabIndex = 0;
            // 
            // dgvInput
            // 
            this.dgvInput.AllowUserToAddRows = false;
            this.dgvInput.AllowUserToDeleteRows = false;
            this.dgvInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDX,
            this.DATA,
            this.SIG});
            this.dgvInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInput.Location = new System.Drawing.Point(0, 0);
            this.dgvInput.Name = "dgvInput";
            this.dgvInput.RowTemplate.Height = 23;
            this.dgvInput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvInput.Size = new System.Drawing.Size(570, 560);
            this.dgvInput.TabIndex = 0;
            // 
            // IDX
            // 
            this.IDX.HeaderText = "IDX";
            this.IDX.Name = "IDX";
            this.IDX.ReadOnly = true;
            // 
            // DATA
            // 
            this.DATA.HeaderText = "DATA";
            this.DATA.Name = "DATA";
            this.DATA.ReadOnly = true;
            // 
            // SIG
            // 
            this.SIG.HeaderText = "SIG";
            this.SIG.Name = "SIG";
            this.SIG.ReadOnly = true;
            // 
            // btnInputNextAll
            // 
            this.btnInputNextAll.Location = new System.Drawing.Point(369, 600);
            this.btnInputNextAll.Name = "btnInputNextAll";
            this.btnInputNextAll.Size = new System.Drawing.Size(54, 23);
            this.btnInputNextAll.TabIndex = 5;
            this.btnInputNextAll.Text = "▶▶";
            this.btnInputNextAll.UseVisualStyleBackColor = true;
            this.btnInputNextAll.Click += new System.EventHandler(this.btnInputNextAll_Click);
            // 
            // btnInputNext
            // 
            this.btnInputNext.Location = new System.Drawing.Point(309, 600);
            this.btnInputNext.Name = "btnInputNext";
            this.btnInputNext.Size = new System.Drawing.Size(54, 23);
            this.btnInputNext.TabIndex = 4;
            this.btnInputNext.Text = "▶";
            this.btnInputNext.UseVisualStyleBackColor = true;
            this.btnInputNext.Click += new System.EventHandler(this.btnInputNext_Click);
            // 
            // btnInputPrev
            // 
            this.btnInputPrev.Location = new System.Drawing.Point(85, 600);
            this.btnInputPrev.Name = "btnInputPrev";
            this.btnInputPrev.Size = new System.Drawing.Size(54, 23);
            this.btnInputPrev.TabIndex = 3;
            this.btnInputPrev.Text = "◀";
            this.btnInputPrev.UseVisualStyleBackColor = true;
            this.btnInputPrev.Click += new System.EventHandler(this.btnInputPrev_Click);
            // 
            // btnInputPrevAll
            // 
            this.btnInputPrevAll.Location = new System.Drawing.Point(25, 600);
            this.btnInputPrevAll.Name = "btnInputPrevAll";
            this.btnInputPrevAll.Size = new System.Drawing.Size(54, 23);
            this.btnInputPrevAll.TabIndex = 2;
            this.btnInputPrevAll.Text = "◀◀";
            this.btnInputPrevAll.UseVisualStyleBackColor = true;
            this.btnInputPrevAll.Click += new System.EventHandler(this.btnInputPrevAll_Click);
            // 
            // btnInputStatus
            // 
            this.btnInputStatus.Location = new System.Drawing.Point(172, 600);
            this.btnInputStatus.Name = "btnInputStatus";
            this.btnInputStatus.Size = new System.Drawing.Size(103, 23);
            this.btnInputStatus.TabIndex = 1;
            this.btnInputStatus.UseVisualStyleBackColor = true;
            this.btnInputStatus.Click += new System.EventHandler(this.btnInputStatus_Click);
            // 
            // panel_Output
            // 
            this.panel_Output.Controls.Add(this.dgvOutput);
            this.panel_Output.Location = new System.Drawing.Point(603, 32);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Size = new System.Drawing.Size(570, 560);
            this.panel_Output.TabIndex = 1;
            // 
            // dgvOutput
            // 
            this.dgvOutput.AllowUserToAddRows = false;
            this.dgvOutput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.dgvOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOutput.Location = new System.Drawing.Point(0, 0);
            this.dgvOutput.Name = "dgvOutput";
            this.dgvOutput.RowTemplate.Height = 23;
            this.dgvOutput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvOutput.Size = new System.Drawing.Size(570, 560);
            this.dgvOutput.TabIndex = 0;
            this.dgvOutput.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOutput_CellClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "IDX";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "DATA";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "SIG";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // btnOutputNextAll
            // 
            this.btnOutputNextAll.Location = new System.Drawing.Point(957, 600);
            this.btnOutputNextAll.Name = "btnOutputNextAll";
            this.btnOutputNextAll.Size = new System.Drawing.Size(54, 23);
            this.btnOutputNextAll.TabIndex = 9;
            this.btnOutputNextAll.Text = "▶▶";
            this.btnOutputNextAll.UseVisualStyleBackColor = true;
            this.btnOutputNextAll.Click += new System.EventHandler(this.btnOutputNextAll_Click);
            // 
            // btnOutputStatus
            // 
            this.btnOutputStatus.Location = new System.Drawing.Point(760, 600);
            this.btnOutputStatus.Name = "btnOutputStatus";
            this.btnOutputStatus.Size = new System.Drawing.Size(103, 23);
            this.btnOutputStatus.TabIndex = 2;
            this.btnOutputStatus.UseVisualStyleBackColor = true;
            this.btnOutputStatus.Click += new System.EventHandler(this.btnOutputStatus_Click);
            // 
            // btnOutputNext
            // 
            this.btnOutputNext.Location = new System.Drawing.Point(897, 600);
            this.btnOutputNext.Name = "btnOutputNext";
            this.btnOutputNext.Size = new System.Drawing.Size(54, 23);
            this.btnOutputNext.TabIndex = 8;
            this.btnOutputNext.Text = "▶";
            this.btnOutputNext.UseVisualStyleBackColor = true;
            this.btnOutputNext.Click += new System.EventHandler(this.btnOutputNext_Click);
            // 
            // btnOutputPrev
            // 
            this.btnOutputPrev.Location = new System.Drawing.Point(673, 600);
            this.btnOutputPrev.Name = "btnOutputPrev";
            this.btnOutputPrev.Size = new System.Drawing.Size(54, 23);
            this.btnOutputPrev.TabIndex = 7;
            this.btnOutputPrev.Text = "◀";
            this.btnOutputPrev.UseVisualStyleBackColor = true;
            this.btnOutputPrev.Click += new System.EventHandler(this.btnOutputPrev_Click);
            // 
            // btnOutputPrevAll
            // 
            this.btnOutputPrevAll.Location = new System.Drawing.Point(613, 600);
            this.btnOutputPrevAll.Name = "btnOutputPrevAll";
            this.btnOutputPrevAll.Size = new System.Drawing.Size(54, 23);
            this.btnOutputPrevAll.TabIndex = 6;
            this.btnOutputPrevAll.Text = "◀◀";
            this.btnOutputPrevAll.UseVisualStyleBackColor = true;
            this.btnOutputPrevAll.Click += new System.EventHandler(this.btnOutputPrevAll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(244, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "INPUT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(821, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "OUTPUT";
            // 
            // pbConnectionStatus
            // 
            this.pbConnectionStatus.BackColor = System.Drawing.SystemColors.GrayText;
            this.pbConnectionStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbConnectionStatus.Location = new System.Drawing.Point(18, 651);
            this.pbConnectionStatus.Name = "pbConnectionStatus";
            this.pbConnectionStatus.Size = new System.Drawing.Size(20, 20);
            this.pbConnectionStatus.TabIndex = 4;
            this.pbConnectionStatus.TabStop = false;
            this.pbConnectionStatus.Click += new System.EventHandler(this.pbConnectionStatus_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(7, 638);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 11);
            this.label3.TabIndex = 5;
            this.label3.Text = "WAGO";
            // 
            // btnForceOutputMode
            // 
            this.btnForceOutputMode.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnForceOutputMode.Location = new System.Drawing.Point(879, 638);
            this.btnForceOutputMode.Name = "btnForceOutputMode";
            this.btnForceOutputMode.Size = new System.Drawing.Size(139, 33);
            this.btnForceOutputMode.TabIndex = 6;
            this.btnForceOutputMode.Text = "Force Output Mode";
            this.btnForceOutputMode.UseVisualStyleBackColor = true;
            this.btnForceOutputMode.Click += new System.EventHandler(this.btnForceOutputMode_Click);
            // 
            // IOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 695);
            this.Controls.Add(this.btnInputNextAll);
            this.Controls.Add(this.btnOutputNextAll);
            this.Controls.Add(this.btnInputNext);
            this.Controls.Add(this.btnForceOutputMode);
            this.Controls.Add(this.btnInputPrev);
            this.Controls.Add(this.btnOutputStatus);
            this.Controls.Add(this.btnInputPrevAll);
            this.Controls.Add(this.btnInputStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOutputNext);
            this.Controls.Add(this.pbConnectionStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOutputPrev);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOutputPrevAll);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.panel_Input);
            this.Name = "IOForm";
            this.Text = "Wago IO";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOForm_FormClosing);
            this.panel_Input.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).EndInit();
            this.panel_Output.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectionStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_Input;
        private System.Windows.Forms.DataGridView dgvInput;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.DataGridView dgvOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Button btnInputStatus;
        private System.Windows.Forms.Button btnOutputStatus;
        private System.Windows.Forms.Button btnInputPrevAll;
        private System.Windows.Forms.Button btnInputPrev;
        private System.Windows.Forms.Button btnInputNext;
        private System.Windows.Forms.Button btnInputNextAll;
        private System.Windows.Forms.Button btnOutputNextAll;
        private System.Windows.Forms.Button btnOutputNext;
        private System.Windows.Forms.Button btnOutputPrev;
        private System.Windows.Forms.Button btnOutputPrevAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDX;
        private System.Windows.Forms.DataGridViewTextBoxColumn DATA;
        private System.Windows.Forms.DataGridViewTextBoxColumn SIG;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pbConnectionStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnForceOutputMode;
    }
}

