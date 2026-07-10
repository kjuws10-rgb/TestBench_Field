using System;
using System.Windows.Forms;
using Core.Config;

namespace StageWin
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppConfig.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MessageBox.Show("### LD Type SW 연결(좌측광학계)###");
            Application.Run(new Form1());
        }
    }
}
