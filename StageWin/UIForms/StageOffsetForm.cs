using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using StageWin.Driver.Motion;
using Timer = System.Windows.Forms.Timer;
using Core.Config;

namespace StageWin.UI
{
    public partial class StageOffsetForm : Form
    {
        private IMotionController _motion;   // null 가능
        private readonly Timer _tmr = new Timer { Interval = 100 };

        

        public StageOffsetForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(10);

            this.FormClosing += (s, e) =>
            {
                try { _tmr?.Stop(); } catch { }
            };

            this.Load += (s, e) => TryLoadStageOffset();
        }

        // 런타임에 모션 주입 (MotionManualForm과 동일 패턴)
        public void AttachMotion(IMotionController motion)
        {
            if (motion == null) throw new ArgumentNullException(nameof(motion));

            _motion = motion;

            WireEvents();

            _tmr.Tick -= Tmr_Tick;
            _tmr.Tick += Tmr_Tick;
            _tmr.Start();
            SafeUiPoll();
        }

        private bool _eventsWired;
        private void WireEvents()
        {
            if (_eventsWired) return;

            if (btnLaserPosMove != null)
                btnLaserPosMove.Click += async (_, __) => await MoveToAsync(numLaserXTarget, numLaserYTarget);
            if (btnReview2PosMove != null)
                btnReview2PosMove.Click += async (_, __) => await MoveToAsync(numReview2XTarget, numReview2YTarget);
            if (btnAlignCam1PosMove != null)
                btnAlignCam1PosMove.Click += async (_, __) => await MoveToAsync(numAlignCamXTarget, numAlignCamYTarget);

            if (btnMoveLaserPosStop != null) btnMoveLaserPosStop.Click += async (_, __) => await ReturnMoveAsync(numLaserXTarget, numLaserYTarget);
            if (btnReview2MoveStop != null) btnReview2MoveStop.Click += async (_, __) => await ReturnMoveAsync(numReview2XTarget, numReview2YTarget);
            if (btnAlignCam1MoveStop != null) btnAlignCam1MoveStop.Click += async (_, __) => await ReturnMoveAsync(numAlignCamXTarget, numAlignCamYTarget);

            if (btnOffsetSave != null) btnOffsetSave.Click += (_, __) => TrySaveStageOffset();

            _eventsWired = true;
        }

        private string JsonPath => Path.Combine(AppConfig.ConfigRoot, "ParameterSetting.json");

        private static string ToInvString(decimal v) => v.ToString(CultureInfo.InvariantCulture);

        private IEnumerable<KeyValuePair<string, NumericUpDown>> EnumerateStageOffsetFields()
        {
            yield return new KeyValuePair<string, NumericUpDown>("LaserX", numLaserXTarget);
            yield return new KeyValuePair<string, NumericUpDown>("LaserY", numLaserYTarget);

            yield return new KeyValuePair<string, NumericUpDown>("Review2X", numReview2XTarget);
            yield return new KeyValuePair<string, NumericUpDown>("Review2Y", numReview2YTarget);

            yield return new KeyValuePair<string, NumericUpDown>("AlignCam1X", numAlignCamXTarget);
            yield return new KeyValuePair<string, NumericUpDown>("AlignCam1Y", numAlignCamYTarget);

            yield return new KeyValuePair<string, NumericUpDown>("MoveXSpeed", numStageOffsetMoveXSpeed);
            yield return new KeyValuePair<string, NumericUpDown>("MoveYSpeed", numStageOffsetMoveYSpeed);
        }

        private static bool TryParseInvariantDecimal(string s, out decimal v)
        {
            return decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
        }

        private ParameterSettingDocument LoadDocOrNew()
        {
            try
            {
                if (!File.Exists(JsonPath)) return new ParameterSettingDocument();
                var ser = new DataContractJsonSerializer(typeof(ParameterSettingDocument));
                using (var fs = new FileStream(JsonPath, FileMode.Open, FileAccess.Read))
                {
                    var doc = (ParameterSettingDocument)ser.ReadObject(fs);
                    if (doc.StageOffsetValue == null)
                        doc.StageOffsetValue = new System.Collections.Generic.Dictionary<string, string>();
                    return doc;
                }
            }
            catch
            {
                return new ParameterSettingDocument();
            }
        }

        private void SaveDoc(ParameterSettingDocument doc)
        {
            var ser = new DataContractJsonSerializer(typeof(ParameterSettingDocument));
            Directory.CreateDirectory(AppConfig.ConfigRoot);
            using (var fs = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
            {
                ser.WriteObject(fs, doc);
            }
        }

        private void TrySaveStageOffset()
        {
            try
            {
                var doc = LoadDocOrNew();

                if (doc.StageOffsetValue == null)
                    doc.StageOffsetValue = new System.Collections.Generic.Dictionary<string, string>();

                foreach (var kv in EnumerateStageOffsetFields())
                {
                    if (kv.Value == null) continue;
                    doc.StageOffsetValue[kv.Key] = ToInvString(kv.Value.Value);
                }

                SaveDoc(doc);

                MessageBox.Show("ParameterSetting.json 저장 완료",
                    "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ParameterSetting.json 저장 실패: " + ex.Message,
                    "Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TryLoadStageOffset()
        {
            if (!File.Exists(JsonPath)) return;

            try
            {
                var doc = LoadDocOrNew();
                var d = doc.StageOffsetValue;
                if (d == null) return;

                foreach (var kv in EnumerateStageOffsetFields())
                {
                    if (kv.Value == null) continue;
                    if (!d.TryGetValue(kv.Key, out var s)) continue;
                    if (!TryParseInvariantDecimal(s, out var v)) continue;
                    kv.Value.Value = v;
                }
            }
            catch
            {
            }
        }

        private async Task MoveToAsync(NumericUpDown numX, NumericUpDown numY)
        {
            if (_motion == null) return;
            if (numX == null || numY == null) return;

            double x = (double)numX.Value;
            double y = (double)numY.Value;
            double vx = numStageOffsetMoveXSpeed != null ? (double)numStageOffsetMoveXSpeed.Value : 0;
            double vy = numStageOffsetMoveYSpeed != null ? (double)numStageOffsetMoveYSpeed.Value : 0;

            if (vx <= 0 || vy <= 0) return;

            var px0 = _motion.GetProfile(Axis.X);
            var py0 = _motion.GetProfile(Axis.Y);

            var fx = _motion.GetPosition(Axis.X);
            var fy = _motion.GetPosition(Axis.Y);
            var dx = -x;
            var dy = -y;

            // PGT 확인 팝업 확인 했을 경우 시작.
            var result = MessageBox.Show(
                this,
                "Stage Offset Move를 시작하겠습니까?",
                "Offset Move",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.OK)
                return;

            try
            {
                _motion.SetProfile(Axis.X, vx, px0.Acceleration, px0.Deceleration);
                _motion.SetProfile(Axis.Y, vy, py0.Acceleration, py0.Deceleration);

                var tx = _motion.MoveRelAsync(Axis.X, dx, vx, px0.Acceleration, px0.Deceleration);
                var ty = _motion.MoveRelAsync(Axis.Y, dy, vy, py0.Acceleration, py0.Deceleration);
                await Task.WhenAll(tx, ty);
            }
            catch
            {
            }
            finally
            {
                try { _motion.SetProfile(Axis.X, px0.Velocity, px0.Acceleration, px0.Deceleration); } catch { }
                try { _motion.SetProfile(Axis.Y, py0.Velocity, py0.Acceleration, py0.Deceleration); } catch { }

                MessageBox.Show("Stage구동을 완료했습니다.", "Move", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task ReturnMoveAsync(NumericUpDown numX, NumericUpDown numY)
        {
            if (_motion == null) return;
            if (numX == null || numY == null) return;

            double x = (double)numX.Value;
            double y = (double)numY.Value;
            double vx = numStageOffsetMoveXSpeed != null ? (double)numStageOffsetMoveXSpeed.Value : 0;
            double vy = numStageOffsetMoveYSpeed != null ? (double)numStageOffsetMoveYSpeed.Value : 0;

            if (vx <= 0 || vy <= 0) return;

            var px0 = _motion.GetProfile(Axis.X);
            var py0 = _motion.GetProfile(Axis.Y);

            var dx = x;
            var dy = y;

            // PGT 확인 팝업 확인 했을 경우 시작.
            var result = MessageBox.Show(
                this,
                "Stage Offset Return Move를 시작하겠습니까?",
                "Offset Return Move",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.OK)
                return;

            try
            {
                _motion.SetProfile(Axis.X, vx, px0.Acceleration, px0.Deceleration);
                _motion.SetProfile(Axis.Y, vy, py0.Acceleration, py0.Deceleration);

                var tx = _motion.MoveRelAsync(Axis.X, dx, vx, px0.Acceleration, px0.Deceleration);
                var ty = _motion.MoveRelAsync(Axis.Y, dy, vy, py0.Acceleration, py0.Deceleration);
                await Task.WhenAll(tx, ty);
            }
            catch
            {
            }
            finally
            {
                try { _motion.SetProfile(Axis.X, px0.Velocity, px0.Acceleration, px0.Deceleration); } catch { }
                try { _motion.SetProfile(Axis.Y, py0.Velocity, py0.Acceleration, py0.Deceleration); } catch { }

                MessageBox.Show("Stage Return Move를 완료했습니다.", "Return Move", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Tmr_Tick(object sender, EventArgs e) => SafeUiPoll();

        private void SafeUiPoll()
        {
            if (_motion == null) return;

            try
            {
                double x = _motion.GetPosition(Axis.X);
                double tX = _motion.GetTargetPosition(Axis.X);
                double aX = _motion.GetAPosition(Axis.X);

                double? vx = TryGetVelocity(Axis.X);

                if (lbXPos != null) lbXPos.Text = $"{x:F3} mm";
                if (lblXtPos != null) lblXtPos.Text = $"{tX:F3} mm";
                if (lblXAPos != null) lblXAPos.Text = $"{aX:F3} mm";
                if (StageXSpeed != null && vx.HasValue) StageXSpeed.Text = $"{vx.Value:F3} mm/Sec";

                double y = _motion.GetPosition(Axis.Y);
                double tY = _motion.GetTargetPosition(Axis.Y);
                double aY = _motion.GetAPosition(Axis.Y);

                double? vy = TryGetVelocity(Axis.Y);

                if (lbYPos != null) lbYPos.Text = $"{y:F3} mm";
                if (lblYtPos != null) lblYtPos.Text = $"{tY:F3} mm";
                if (lblYAPos != null) lblYAPos.Text = $"{aY:F3} mm";
                if (StageYSpeed != null && vy.HasValue) StageYSpeed.Text = $"{vy.Value:F3} mm/Sec";
            }
            catch
            {
            }
        }

        private double? TryGetVelocity(Axis axis)
        {
            try
            {
                var mi = _motion?.GetType().GetMethod("GetVelocity", new[] { typeof(Axis) });
                if (mi == null) return null;
                var v = mi.Invoke(_motion, new object[] { axis });
                if (v is double d) return d;
                return null;
            }
            catch
            {
                return null;
            }
        }

    }
}
