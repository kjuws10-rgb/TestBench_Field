using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageWin.Etc
{
    public partial class StatusSelectionForm : Form
    {
        public string SelectedStatus { get; private set; }

        public StatusSelectionForm(List<string> statusList)
        {
            InitializeComponent();

            // ListBox에 Status 값 채우기
            foreach (var status in statusList)
            {
                listBox1.Items.Add(status);
            }

            listBox1.DoubleClick += (s, e) => {
                if (listBox1.SelectedItem != null)
                {
                    SelectedStatus = listBox1.SelectedItem.ToString();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
