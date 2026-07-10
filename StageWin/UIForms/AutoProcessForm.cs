using Core.Config;
using StageWin.Safety;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageWin.UI
{
    public partial class AutoProcessForm : Form
    {
        public Func<ProgramMode> ModeProvider { get; set; }
        /// <summary>
        /// Semi-Auto 버튼 하나를 눌렀을 때 호출
        ///  - seqId: 어떤 시퀀스인지
        ///  - dryRun: Dry Run 체크 여부
        /// </summary>
        public event Func<AutoSequenceId, Task> RequestRunSemiAuto;

        /// <summary>
        /// Auto Start 버튼을 눌렀을 때 호출
        /// </summary>
        public event Func<AutoProcessRequest, Task> RequestRunAuto;

        /// <summary>
        /// Auto Stop / 전체 STOP 버튼
        /// </summary>
        public event Action RequestStopAll;
        private bool DryRun => chkDryRun?.Checked ?? false;
        private readonly Timer _tmrSemiBlink = new Timer { Interval = 300 };
        private readonly Timer _tmrAutoBlink = new Timer { Interval = 300 };
        private Button _semiBlinkTarget;
        private Color _semiBlinkBack0;
        private bool _semiBlinkState;
        private Control _autoBlinkTarget;      // 패널/그룹박스 모두 수용
        private Color _autoBlinkBack0;
        private bool _autoBlinkState;
        private const string AUTO_SEQ_SETTING_FILE = "AutoSequenceSetting.json";
        private bool _loadingAutoSeqSetting = false;
        private readonly Timer _tmrAutoSeqSaveDebounce = new Timer { Interval = 250 };
        public enum AutoSeqVisualState
        {
            Idle,
            Wait,
            Running,
            Done,
            Skip,
            Error
        }

        public AutoProcessForm()
        {
            InitializeComponent();

            // Semi-Auto 버튼 바인딩
            btnSemiLoad.Click               += async (s, e)   => await FireSemiAsync(AutoSequenceId.Load);
            btnSemiMoveToAlign.Click        += async (s, e)   => await FireSemiAsync(AutoSequenceId.MoveToAlign);
            btnSemiAlign.Click              += async (s, e)   => await FireSemiAsync(AutoSequenceId.Align);
            btnSemiPowerMeter.Click         += async (s, e)   => await FireSemiAsync(AutoSequenceId.PowerMeter);
            btnSemiProcessReady.Click       += async (s, e)   => await FireSemiAsync(AutoSequenceId.ProcessReady);
            btnSemiProcess.Click            += async (s, e)   => await FireSemiAsync(AutoSequenceId.Process);
            btnSemiInspection.Click         += async (s, e)   => await FireSemiAsync(AutoSequenceId.Inspection);
            btnSemiMoveToAlignCheck.Click   += async (s, e)   => await FireSemiAsync(AutoSequenceId.MoveToAlignCheck);
            btnSemiAlignCheck.Click         += async (s, e)   => await FireSemiAsync(AutoSequenceId.AlignCheck);
            btnSemiMoveToUnload.Click       += async (s, e)   => await FireSemiAsync(AutoSequenceId.MoveToUnload);
            btnSemiUnload.Click             += async (s, e)   => await FireSemiAsync(AutoSequenceId.Unload);
            if (btnAutoStart != null)   btnAutoStart.Click    += async (s, e) => await FireAutoAsync();
            if (btnSemiStop != null)    btnSemiStop.Click     += (s, e)       => RequestStopAll?.Invoke();
            if (btnAutoStop != null)    btnAutoStop.Click     += (s, e)       => RequestStopAll?.Invoke();

            _tmrSemiBlink.Tick += (s, e) => {
                if (_semiBlinkTarget == null)
                {
                    _tmrSemiBlink.Stop();
                    return;
                }
                _semiBlinkState = !_semiBlinkState;
                _semiBlinkTarget.BackColor = _semiBlinkState ? Color.OrangeRed : _semiBlinkBack0;
            };
            _tmrAutoBlink.Tick += (s, e) => {
                if (_autoBlinkTarget == null)
                {
                    _tmrAutoBlink.Stop();
                    return;
                }
                _autoBlinkState = !_autoBlinkState;
                _autoBlinkTarget.BackColor = _autoBlinkState ? Color.Gold : _autoBlinkBack0;
            };

            _tmrAutoSeqSaveDebounce.Tick += (s, e) =>
            {
                _tmrAutoSeqSaveDebounce.Stop();
                SaveAutoSeqSettingNow();
            };

            // 저장된 체크박스 구성 복원
            try
            {
                _loadingAutoSeqSetting = true;
                var preset = LoadAutoSeqSetting();
                if (preset != null && preset.Length > 0)
                    ApplyAutoSeqSettingToCheckboxes(preset);
            }
            finally
            {
                _loadingAutoSeqSetting = false;
            }

            // 체크박스 변경 시 자동 저장
            HookAutoSeqCheckboxChangedEvents();
        }

        [DataContract]
        private sealed class AutoSequenceSetting
        {
            [DataMember(Order = 1)]
            public string[] Enabled { get; set; } = Array.Empty<string>();
        }
        private string GetAutoSeqSettingPath()
        {
            try { return AppConfig.GetConfigFile(AUTO_SEQ_SETTING_FILE); }
            catch { return Path.Combine(AppConfig.ConfigRoot, AUTO_SEQ_SETTING_FILE); }
        }
        private void SaveAutoSeqSettingNow()
        {
            if (_loadingAutoSeqSetting) return;

            try
            {
                var path = GetAutoSeqSettingPath();
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                var enabled = GetCheckedSequences();
                var setting = new AutoSequenceSetting
                {
                    Enabled = enabled?.Select(s => s.ToString()).ToArray() ?? Array.Empty<string>()
                };

                var ser = new DataContractJsonSerializer(typeof(AutoSequenceSetting));
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    ser.WriteObject(fs, setting);
            }
            catch
            {
                // 저장 실패는 UI 동작에 영향 주지 않도록 무시
            }
        }
        private AutoSequenceId[] LoadAutoSeqSetting()
        {
            try
            {
                var path = GetAutoSeqSettingPath();
                if (!File.Exists(path)) return Array.Empty<AutoSequenceId>();

                var ser = new DataContractJsonSerializer(typeof(AutoSequenceSetting));
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var setting = (AutoSequenceSetting)ser.ReadObject(fs);
                    if (setting?.Enabled == null || setting.Enabled.Length == 0)
                        return Array.Empty<AutoSequenceId>();

                    var list = new List<AutoSequenceId>();
                    foreach (var s in setting.Enabled)
                    {
                        if (Enum.TryParse<AutoSequenceId>(s, out var id))
                            list.Add(id);
                    }
                    return list.ToArray();
                }
            }
            catch
            {
                return Array.Empty<AutoSequenceId>();
            }
        }

        private void ApplyAutoSeqSettingToCheckboxes(AutoSequenceId[] seqs)
        {
            var set = new HashSet<AutoSequenceId>(seqs ?? Array.Empty<AutoSequenceId>());

            if (chkSeqLoad != null)             chkSeqLoad.Checked              = set.Contains(AutoSequenceId.Load);
            if (chkSeqMoveToAlign != null)      chkSeqMoveToAlign.Checked       = set.Contains(AutoSequenceId.MoveToAlign);
            if (chkSeqAlign != null)            chkSeqAlign.Checked             = set.Contains(AutoSequenceId.Align);
            if (chkSeqPwmCheck != null)         chkSeqPwmCheck.Checked          = set.Contains(AutoSequenceId.PowerMeter);
            if (chkSeqProcessReady != null)     chkSeqProcessReady.Checked      = set.Contains(AutoSequenceId.ProcessReady);
            if (chkSeqProcess != null)          chkSeqProcess.Checked           = set.Contains(AutoSequenceId.Process);
            if (chkSeqInspection != null)       chkSeqInspection.Checked        = set.Contains(AutoSequenceId.Inspection);
            if (chkSeqMoveToAlignCheck != null) chkSeqMoveToAlignCheck.Checked  = set.Contains(AutoSequenceId.MoveToAlignCheck);
            if (chkSeqAlignCheck != null)       chkSeqAlignCheck.Checked        = set.Contains(AutoSequenceId.AlignCheck);
            if (chkSeqMoveToUnload != null)     chkSeqMoveToUnload.Checked      = set.Contains(AutoSequenceId.MoveToUnload);
            if (chkSeqUnload != null)           chkSeqUnload.Checked            = set.Contains(AutoSequenceId.Unload);
        }

        private void HookAutoSeqCheckboxChangedEvents()
        {
            void hook(CheckBox cb)
            {
                if (cb == null) return;
                cb.CheckedChanged += (s, e) =>
                {
                    if (_loadingAutoSeqSetting) return;
                    _tmrAutoSeqSaveDebounce.Stop();
                    _tmrAutoSeqSaveDebounce.Start();
                };
            }

            hook(chkSeqLoad);
            hook(chkSeqMoveToAlign);
            hook(chkSeqAlign);
            hook(chkSeqPwmCheck);
            hook(chkSeqProcessReady);
            hook(chkSeqProcess);
            hook(chkSeqInspection);
            hook(chkSeqMoveToAlignCheck);
            hook(chkSeqAlignCheck);
            hook(chkSeqMoveToUnload);
            hook(chkSeqUnload);
        }
        private Label GetStateLabel(AutoSequenceId seq)
        {
            switch (seq)
            {
                case AutoSequenceId.Load:                   return lalSeqLoadSts;
                case AutoSequenceId.MoveToAlign:            return lalSeqMoveToAlignSts;
                case AutoSequenceId.Align:                  return lalSeqAlignSts;
                case AutoSequenceId.PowerMeter:             return lalSeqPowerMeterSts;
                case AutoSequenceId.ProcessReady:           return lalSeqProcessReadySts;
                case AutoSequenceId.Process:                return lalSeqProcessSts;
                case AutoSequenceId.Inspection:             return lalSeqInspectionSts;
                case AutoSequenceId.MoveToAlignCheck:       return lalSeqMoveToAlignCheckSts;
                case AutoSequenceId.AlignCheck:             return lalSeqAlignCheckSts;
                case AutoSequenceId.MoveToUnload:           return lalSeqMoveToUnloadSts;
                case AutoSequenceId.Unload:                 return lalSeqUnloadSts;
                default: return null;
            }
        }

        public void ResetAutoSeqStates()
        {
            Action<Label> reset = lbl =>
            {
                if (lbl == null) return;
                lbl.Text = "";
                lbl.BackColor = SystemColors.Control;
                lbl.ForeColor = SystemColors.ControlText;
            };

            reset(lalSeqLoadSts);
            reset(lalSeqMoveToAlignSts);
            reset(lalSeqAlignSts);
            reset(lalSeqProcessReadySts);
            reset(lalSeqProcessSts);
            reset(lalSeqInspectionSts);
            reset(lalSeqMoveToAlignCheckSts);
            reset(lalSeqAlignCheckSts);
            reset(lalSeqMoveToUnloadSts);
            reset(lalSeqUnloadSts);
        }

        public void SetAutoSeqState(AutoSequenceId seq, AutoSeqVisualState state)
        {
            var lbl = GetStateLabel(seq);
            if (lbl == null) return;

            switch (state)
            {
                case AutoSeqVisualState.Idle:
                    lbl.Text = "";
                    lbl.BackColor = SystemColors.Control;
                    lbl.ForeColor = SystemColors.ControlText;
                    break;

                case AutoSeqVisualState.Wait:
                    lbl.Text = "WAIT";
                    lbl.BackColor = Color.LightGray;
                    lbl.ForeColor = Color.Black;
                    break;

                case AutoSeqVisualState.Running:
                    lbl.Text = "RUN";
                    lbl.BackColor = Color.Orange;
                    lbl.ForeColor = Color.Black;
                    break;

                case AutoSeqVisualState.Done:
                    lbl.Text = "OK";
                    lbl.BackColor = Color.LightGreen;
                    lbl.ForeColor = Color.Black;
                    break;

                case AutoSeqVisualState.Skip:
                    lbl.Text = "SKIP";
                    lbl.BackColor = Color.Silver;
                    lbl.ForeColor = Color.Black;
                    break;

                case AutoSeqVisualState.Error:
                    lbl.Text = "ERR";
                    lbl.BackColor = Color.Red;
                    lbl.ForeColor = Color.White;
                    break;
            }
        }
        private Button GetSemiButton(AutoSequenceId id)
        {
            switch (id)
            {
                case AutoSequenceId.Load:               return btnSemiLoad;
                case AutoSequenceId.MoveToAlign:        return btnSemiMoveToAlign;
                case AutoSequenceId.Align:              return btnSemiAlign;
                case AutoSequenceId.PowerMeter:         return btnSemiPowerMeter;
                case AutoSequenceId.ProcessReady:       return btnSemiProcessReady;
                case AutoSequenceId.Process:            return btnSemiProcess;
                case AutoSequenceId.Inspection:         return btnSemiInspection;
                case AutoSequenceId.MoveToAlignCheck:   return btnSemiMoveToAlignCheck;
                case AutoSequenceId.AlignCheck:         return btnSemiAlignCheck;
                case AutoSequenceId.MoveToUnload:       return btnSemiMoveToUnload;
                case AutoSequenceId.Unload:             return btnSemiUnload;
                default: return null;
            }
        }
        private Control GetAutoPanel()
        {
            if (chkSeqLoad != null)
            {
                var p = chkSeqLoad.Parent;          // 보통 Panel
                if (p != null && p.Parent != null)  // 보통 p.Parent 가 TableLayoutPanel
                    return p.Parent;
                if (p != null) return p;
            }
            if (grpAuto != null) return grpAuto;
            return null;
        }
        private Control GetAutoSeqPanel(AutoSequenceId seq)
        {
            CheckBox cb = null;

            switch (seq)
            {
                case AutoSequenceId.Load:               cb = chkSeqLoad;                break;
                case AutoSequenceId.MoveToAlign:        cb = chkSeqMoveToAlign;         break;
                case AutoSequenceId.Align:              cb = chkSeqAlign;               break;
                case AutoSequenceId.PowerMeter:         cb = chkSeqPwmCheck;            break;
                case AutoSequenceId.ProcessReady:       cb = chkSeqProcessReady;        break;
                case AutoSequenceId.Process:            cb = chkSeqProcess;             break;
                case AutoSequenceId.Inspection:         cb = chkSeqInspection;          break;
                case AutoSequenceId.MoveToAlignCheck:   cb = chkSeqMoveToAlignCheck;    break;
                case AutoSequenceId.AlignCheck:         cb = chkSeqAlignCheck;          break;
                case AutoSequenceId.MoveToUnload:       cb = chkSeqMoveToUnload;        break;
                case AutoSequenceId.Unload:             cb = chkSeqUnload;              break;
            }
            // CheckBox가 Panel 안에 있다고 가정
            // (TableLayoutPanel → Panel → CheckBox 구조)
            return cb?.Parent;
        }
        public void SetSequenceBlink(bool isFullAuto, AutoSequenceId seq, bool running)
        {
            if (isFullAuto)
            {
                // Full Auto: 패널 깜빡임 대신 상태 라벨만 갱신
                if (!running)
                    SetAutoSeqState(seq, AutoSeqVisualState.Done);
                else
                    SetAutoSeqState(seq, AutoSeqVisualState.Running);

                // 혹시 기존 Auto 깜빡이 타이머가 돌고 있다면 정지
                _tmrAutoBlink.Stop();
                if (_autoBlinkTarget != null)
                    _autoBlinkTarget.BackColor = _autoBlinkBack0;
                _autoBlinkTarget = null;
                return;
            }

            // Semi-Auto 버튼 깜빡임은 기존 로직 유지
            if (!running)
            {
                _tmrSemiBlink.Stop();
                if (_semiBlinkTarget != null) _semiBlinkTarget.BackColor = _semiBlinkBack0;
                _semiBlinkTarget = null;
                return;
            }
            _semiBlinkTarget = GetSemiButton(seq);
            if (_semiBlinkTarget == null) return;
            _semiBlinkBack0 = _semiBlinkTarget.BackColor;
            _semiBlinkState = false;
            _tmrSemiBlink.Start();
        }
        private async Task FireSemiAsync(AutoSequenceId id)
        {
            var h = RequestRunSemiAuto;
            if (h == null)
            {
                MessageBox.Show(this, "Semi-Auto 실행 핸들러가 연결되어 있지 않습니다.",
                    "Semi-Auto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ModeProvider != null && ModeProvider() == ProgramMode.Manual)
            {
                MessageBox.Show(this, "Manual 모드에서는 Semi-Auto 시퀀스를 실행할 수 없습니다.",
                    "Semi-Auto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await h(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Semi-Auto 시퀀스 실행 중 오류: " + ex.Message,
                    "Semi-Auto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task FireAutoAsync()
        {
            var h = RequestRunAuto;
            if (h == null)
            {
                MessageBox.Show(this, "Auto 실행 핸들러가 연결되어 있지 않습니다.",
                    "Auto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ModeProvider != null && ModeProvider() == ProgramMode.Manual)
            {
                MessageBox.Show(this, "Manual 모드에서는 Auto 시퀀스를 실행할 수 없습니다.",
                    "Auto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var list = new List<AutoSequenceId>();
                if (chkSeqLoad.Checked) list.Add(AutoSequenceId.Load);
                if (chkSeqMoveToAlign.Checked) list.Add(AutoSequenceId.MoveToAlign);
                if (chkSeqAlign.Checked) list.Add(AutoSequenceId.Align);
                if (chkSeqPwmCheck.Checked) list.Add(AutoSequenceId.PowerMeter);
                if (chkSeqProcessReady.Checked) list.Add(AutoSequenceId.ProcessReady);
                if (chkSeqProcess.Checked) list.Add(AutoSequenceId.Process);
                if (chkSeqInspection.Checked) list.Add(AutoSequenceId.Inspection);
                if (chkSeqMoveToAlignCheck.Checked) list.Add(AutoSequenceId.MoveToAlignCheck);
                if (chkSeqAlignCheck.Checked) list.Add(AutoSequenceId.AlignCheck);
                if (chkSeqMoveToUnload.Checked) list.Add(AutoSequenceId.MoveToUnload);
                if (chkSeqUnload.Checked) list.Add(AutoSequenceId.Unload);

                var req = new AutoProcessRequest
                {
                    DryRun = this.DryRun,
                    EnabledSequences = list.ToArray()
                };
                SaveAutoSeqSettingNow();
                await h(req);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Auto 시퀀스 실행 중 오류: " + ex.Message,
                    "Auto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto 탭의 체크박스 → 시퀀스 리스트
        /// (Auto UI 만들 때 체크박스 이름만 맞춰주면 됨)
        /// </summary>
        private AutoSequenceId[] GetCheckedSequences()
        {
            var list = new List<AutoSequenceId>();

            if (chkSeqLoad !=               null && chkSeqLoad.Checked)             list.Add(AutoSequenceId.Load);
            if (chkSeqMoveToAlign !=        null && chkSeqMoveToAlign.Checked)      list.Add(AutoSequenceId.MoveToAlign);
            if (chkSeqAlign !=              null && chkSeqAlign.Checked)            list.Add(AutoSequenceId.Align);
            if (chkSeqPwmCheck !=           null && chkSeqPwmCheck.Checked)         list.Add(AutoSequenceId.PowerMeter);
            if (chkSeqProcessReady !=       null && chkSeqProcessReady.Checked)     list.Add(AutoSequenceId.ProcessReady);
            if (chkSeqProcess !=            null && chkSeqProcess.Checked)          list.Add(AutoSequenceId.Process);
            if (chkSeqInspection !=         null && chkSeqInspection.Checked)       list.Add(AutoSequenceId.Inspection);
            if (chkSeqMoveToAlignCheck !=   null && chkSeqMoveToAlignCheck.Checked) list.Add(AutoSequenceId.MoveToAlignCheck);
            if (chkSeqAlignCheck !=         null && chkSeqAlignCheck.Checked)       list.Add(AutoSequenceId.AlignCheck);
            if (chkSeqMoveToUnload !=       null && chkSeqMoveToUnload.Checked)     list.Add(AutoSequenceId.MoveToUnload);
            if (chkSeqUnload !=             null && chkSeqUnload.Checked)           list.Add(AutoSequenceId.Unload);
            return list.ToArray();
        }

        // Form1에서 탭 선택 제어용
        public void SelectSemiAutoTab()
        {
            if (tabMode != null && tabSemiAuto != null)
                tabMode.SelectedTab = tabSemiAuto;
        }

        public void SelectAutoTab()
        {
            if (tabMode != null && tabAuto != null)
                tabMode.SelectedTab = tabAuto;
        }
        public void UpdateModeState(ProgramMode mode, bool enable)
        {
            // 전체 폼 활성화/비활성
            this.Enabled = enable;

            // 모드 라벨이 있으면 갱신(없으면 생략 가능)
            //if (lblMode != null) lblMode.Text = $"Mode : {mode}";

            // 그룹별로 세밀하게 제어하고 싶으면 이렇게 (컨트롤 이름은 실제 UI에 맞게 수정)
            bool semi = enable && mode == ProgramMode.SemiAuto;
            bool auto = enable && mode == ProgramMode.Auto;

            if (grpSemiAuto != null) grpSemiAuto.Enabled = semi;
            if (grpAuto != null) grpAuto.Enabled = auto;

            // Stop 버튼은 Manual에서도 허용하고 싶으면 enable 대신 true로
            if (btnSemiStop != null) btnSemiStop.Enabled = enable;
            if (btnAutoStop != null) btnAutoStop.Enabled = enable;
        }

    
    }
}
