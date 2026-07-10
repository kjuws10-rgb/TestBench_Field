using System;
using System.Windows.Forms;

namespace StageWin.Etc
{
    public partial class MoveConfirmForm : Form
    {
        // 디자이너/디자인타임용 기본 생성자 (필수)
        public MoveConfirmForm()
        {
            InitializeComponent();

            // 디자인 타임에서도 안전한 기본값
            lblTitle.Text = "Move";
            tbCurX.Text = tbCurY.Text = tbTgtX.Text = tbTgtY.Text = 0.0.ToString("N3");
        }

        // 런타임에서 사용하는 편의 생성자
        public MoveConfirmForm(string axisName, double curX, double curY, double tgtX, double tgtY)
            : this() // 기본 생성자 호출 → 컨트롤들이 null 아님
        {
            lblTitle.Text = string.Format("{0} Move", axisName);
            tbCurX.Text = curX.ToString("N3");
            tbCurY.Text = curY.ToString("N3");
            tbTgtX.Text = tgtX.ToString("N3");
            tbTgtY.Text = tgtY.ToString("N3");
        }
    }
}