using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageWin.Etc
{
    public partial class Helper : Form
    {
        public Helper()
        {
            InitializeComponent();
        }
        private void TryLoadProcMapImage()
        {
            if (picProcMap1 == null) return;

            picProcMap1.SizeMode = PictureBoxSizeMode.Zoom; picProcMap1.BackColor = Color.White;
            picProcMap2.SizeMode = PictureBoxSizeMode.Zoom; picProcMap2.BackColor = Color.White;
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates = { Path.Combine(baseDir, "MapInfo1.jpg"), Path.Combine(baseDir, "MapInfo2.jpg") };
            try
            {
                if (File.Exists(candidates[0])) LoadImageNoLock(picProcMap1, candidates[0]);
                if (File.Exists(candidates[1])) LoadImageNoLock(picProcMap2, candidates[1]);
            }
            catch { /* 파일 잠김/깨짐 등은 조용히 무시 */ }
        }
        private static void LoadImageNoLock(PictureBox target, string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var img = Image.FromStream(fs))
            {
                var bmp = new Bitmap(img);           // 스트림 분리(파일 잠금 방지)
                var old = target.Image;              // 이전 이미지 정리
                target.Image = bmp;
                old?.Dispose();
            }
        }
    }
}
