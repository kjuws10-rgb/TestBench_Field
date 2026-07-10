using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using Core.Config; // AppConfig.ConfigRoot 사용
using StageWin.Core.Recipe;

namespace StageWin.UI
{
    public partial class ParameterSettingForm : Form
    {
        private string JsonPath => Path.Combine(AppConfig.ConfigRoot, "ParameterSetting.json");
        private PowerRecipeStore _pmStore;
        private bool _pmSuppressEvent;

        public ParameterSettingForm()
        {
            InitializeComponent();
            this.Load += ParameterSettingForm_Load;
        }

        private void ParameterSettingForm_Load(object sender, EventArgs e)
        {
            if (txtSelectedPmRecipeName != null) txtSelectedPmRecipeName.Tag = "ProcessParameter:PowerMeterRecipeName";
            if (rbtn1LineInspection != null) rbtn1LineInspection.Tag = "ProcessParameter:InspectionMode";
            if (rbtnAllInspection != null) rbtnAllInspection.Tag = "ProcessParameter:InspectionMode";
            if (rbtn1StepProcess != null) rbtn1StepProcess.Tag = "ProcessParameter:StepProcessMode";
            if (rbtnAllStepProcess != null) rbtnAllStepProcess.Tag = "ProcessParameter:StepProcessMode";
            // Semi/Auto Inspection에서 전셀 Step-by-Step Review 검사 사용 여부
            if (rbtnStepbyStepAllInspection != null) rbtnStepbyStepAllInspection.Tag = "ProcessParameter:StepByStepAllInspection";
            TryLoad();

            try
            {
                _pmStore = PowerRecipeStore.Open(AppConfig.Current?.PowerRecipesPath);
            }
            catch
            {
                _pmStore = PowerRecipeStore.Open(null);
            }

            RefreshPmRecipeListAndApplySelection();
        }
        private void RefreshPmRecipeListAndApplySelection()
        {
            if (lstPmRecipeList == null) return;

            var names = _pmStore?.ListNames() ?? new System.Collections.Generic.List<string>();
            _pmSuppressEvent = true;
            try
            {
                lstPmRecipeList.BeginUpdate();
                lstPmRecipeList.Items.Clear();
                foreach (var n in names) lstPmRecipeList.Items.Add(n);
                lstPmRecipeList.EndUpdate();

                // 기존 저장값(텍스트박스) 기준으로 선택 복원
                var savedName = (txtSelectedPmRecipeName?.Text ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(savedName))
                {
                    int idx = lstPmRecipeList.FindStringExact(savedName);
                    if (idx >= 0) lstPmRecipeList.SelectedIndex = idx;
                }

                // 저장값이 없거나 못 찾으면 첫번째 선택(있으면)
                if (lstPmRecipeList.SelectedIndex < 0 && lstPmRecipeList.Items.Count > 0)
                    lstPmRecipeList.SelectedIndex = 0;

                // 선택 결과를 텍스트박스에 최종 반영
                ApplyPmSelectionToText();
            }
            finally
            {
                _pmSuppressEvent = false;
            }
        }
        private void ApplyPmSelectionToText()
        {
            if (txtSelectedPmRecipeName == null) return;
            var name = lstPmRecipeList?.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(name)) name = "";
            txtSelectedPmRecipeName.Text = name;
            if (lblPmSelectedDisplay != null)
                lblPmSelectedDisplay.Text = string.IsNullOrWhiteSpace(name) ? "Selected: (None)" : $"Selected: {name}";
        }
        private void btnPmRefresh_Click(object sender, EventArgs e)
        {
            RefreshPmRecipeListAndApplySelection();
        }
        private void lstPmRecipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_pmSuppressEvent) return;
            ApplyPmSelectionToText();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            TrySave();
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            TryLoad();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void TrySave()
        {
            var doc = new ParameterSettingDocument();

            foreach (var tb in EnumerateAll<TextBox>(this))
                SaveTaggedValue(doc, tb.Tag as string, tb.Text ?? string.Empty);

            foreach (var nud in EnumerateAll<NumericUpDown>(this))
                SaveTaggedValue(doc, nud.Tag as string, nud.Value.ToString(CultureInfo.InvariantCulture));

            SaveInspectionMode(doc);
            SaveStepProcessMode(doc);

            // 추가
            SaveStepByStepAllInspectionMode(doc);

            var ser = new DataContractJsonSerializer(typeof(ParameterSettingDocument));
            Directory.CreateDirectory(AppConfig.ConfigRoot);

            using (var fs = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
                ser.WriteObject(fs, doc);

            MessageBox.Show("ParameterSetting.json 저장 완료",
                "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SaveInspectionMode(ParameterSettingDocument doc)
        {
            string mode = (rbtn1LineInspection?.Checked ?? false) ? "1LINE" : "ALL";
            SaveTaggedValue(doc, "ProcessParameter:InspectionMode", mode);
        }
        private void SaveStepProcessMode(ParameterSettingDocument doc)
        {
            string mode = (rbtn1StepProcess?.Checked ?? false) ? "1STEP" : "ALL";
            SaveTaggedValue(doc, "ProcessParameter:StepProcessMode", mode);
        }
        private static void SaveTaggedValue(ParameterSettingDocument doc, string tag, string val)
        {
            if (doc == null) return;
            if (string.IsNullOrWhiteSpace(tag) || tag.IndexOf(':') < 0) return;

            var sp = tag.Split(new[] { ':' }, 2);
            var section = sp[0].Trim();
            var key = sp[1].Trim();

            switch (section)
            {
                case "ParameterValue": doc.ParameterValue[key] = val; break;
                case "ESCFlatnessValue": doc.ESCFlatnessValue[key] = val; break;
                case "ProcessParameter": doc.ProcessParameter[key] = val; break;
                case "StageOffsetValue": doc.StageOffsetValue[key] = val; break;
            }
        }
        private void TryLoad()
        {
            if (!File.Exists(JsonPath)) return;

            try
            {
                var ser = new DataContractJsonSerializer(typeof(ParameterSettingDocument));
                using (var fs = new FileStream(JsonPath, FileMode.Open, FileAccess.Read))
                {
                    var doc = (ParameterSettingDocument)ser.ReadObject(fs);
                    foreach (var tb in EnumerateAll<TextBox>(this))
                    {
                        var tag = tb.Tag as string;
                        if (!TryGetTaggedValue(doc, tag, out var val)) continue;
                        tb.Text = val;
                    }
                    foreach (var nud in EnumerateAll<NumericUpDown>(this))
                    {
                        var tag = nud.Tag as string;
                        if (!TryGetTaggedValue(doc, tag, out var val)) continue;

                        if (decimal.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var dv) ||
                            decimal.TryParse(val, NumberStyles.Float, CultureInfo.CurrentCulture, out dv))
                        {
                            if (dv < nud.Minimum) dv = nud.Minimum;
                            if (dv > nud.Maximum) dv = nud.Maximum;
                            nud.Value = dv;
                        }
                    }
                    LoadInspectionMode(doc);
                    LoadStepProcessMode(doc);
                    LoadStepByStepAllInspectionMode(doc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ParameterSetting.json 읽기 실패: " + ex.Message,
                    "Load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SaveStepByStepAllInspectionMode(ParameterSettingDocument doc)
        {
            string enabled = (rbtnStepbyStepAllInspection != null && rbtnStepbyStepAllInspection.Checked)
                ? "1"
                : "0";

            SaveTaggedValue(doc, "ProcessParameter:StepByStepAllInspection", enabled);
        }

        private void LoadStepByStepAllInspectionMode(ParameterSettingDocument doc)
        {
            bool enabled = false;

            if (TryGetTaggedValue(doc, "ProcessParameter:StepByStepAllInspection", out var v))
                enabled = ParseBoolText(v);

            if (rbtnStepbyStepAllInspection != null)
                rbtnStepbyStepAllInspection.Checked = enabled;
        }

        private static bool ParseBoolText(string s)
        {
            s = (s ?? "").Trim();

            if (string.IsNullOrWhiteSpace(s)) return false;

            if (bool.TryParse(s, out var b)) return b;
            if (int.TryParse(s, out var i)) return i != 0;

            s = s.ToUpperInvariant();

            return s == "Y"
                || s == "YES"
                || s == "ON"
                || s == "CHECKED"
                || s == "ENABLE"
                || s == "ENABLED"
                || s == "STEP"
                || s == "STEP_ALL"
                || s == "STEP_ALL_WHOLELINE"
                || s == "STEPBYSTEPALLINSPECTION";
        }
        private void LoadInspectionMode(ParameterSettingDocument doc)
        {
            string mode = "ALL";
            if (TryGetTaggedValue(doc, "ProcessParameter:InspectionMode", out var v) && !string.IsNullOrWhiteSpace(v))
                mode = v.Trim().ToUpperInvariant();
            bool oneLine = (mode == "1LINE" || mode == "ONE" || mode == "ONELINE");
            if (rbtn1LineInspection != null) rbtn1LineInspection.Checked = oneLine;
            if (rbtnAllInspection != null) rbtnAllInspection.Checked = !oneLine;
        }
        private void LoadStepProcessMode(ParameterSettingDocument doc)
        {
            string mode = "ALL";
            if (TryGetTaggedValue(doc, "ProcessParameter:StepProcessMode", out var v) && !string.IsNullOrWhiteSpace(v))
                mode = v.Trim().ToUpperInvariant();
            bool oneStep = (mode == "1STEP" || mode == "ONE" || mode == "ONESTEP" || mode == "1LINE" || mode == "ONELINE");
            if (rbtn1StepProcess != null) rbtn1StepProcess.Checked = oneStep;
            if (rbtnAllStepProcess != null) rbtnAllStepProcess.Checked = !oneStep;
        }
        private static bool TryGetTaggedValue(ParameterSettingDocument doc, string tag, out string val)
        {
            val = null;
            if (doc == null) return false;
            if (string.IsNullOrWhiteSpace(tag) || tag.IndexOf(':') < 0) return false;

            var sp = tag.Split(new[] { ':' }, 2);
            var section = sp[0].Trim();
            var key = sp[1].Trim();

            if (section == "ParameterValue" && doc.ParameterValue != null && doc.ParameterValue.TryGetValue(key, out var pv)) { val = pv; return true; }
            if (section == "ESCFlatnessValue" && doc.ESCFlatnessValue != null && doc.ESCFlatnessValue.TryGetValue(key, out var fv)) { val = fv; return true; }
            if (section == "ProcessParameter" && doc.ProcessParameter != null && doc.ProcessParameter.TryGetValue(key, out var pp)) { val = pp; return true; }
            if (section == "StageOffsetValue" && doc.StageOffsetValue != null && doc.StageOffsetValue.TryGetValue(key, out var sv)) { val = sv; return true; }

            return false;
        }
        // ---------- 유틸 함수들 ----------
        private static System.Collections.Generic.IEnumerable<T> EnumerateAll<T>(Control root)
            where T : Control
        {
            foreach (Control c in root.Controls)
            {
                if (c is T t)
                    yield return t;

                foreach (var child in EnumerateAll<T>(c))
                    yield return child;
            }
        }
        private static T FindControlByTag<T>(Control root, string tag) where T : Control
        {
            foreach (var c in EnumerateAll<T>(root))
            {
                if (c.Tag as string == tag)
                    return c;
            }
            return null;
        }
    }

    [DataContract]
    public sealed class ParameterSettingDocument
    {
        [DataMember(Order = 1)]
        public System.Collections.Generic.Dictionary<string, string> ParameterValue
        { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

        [DataMember(Order = 2)]
        public System.Collections.Generic.Dictionary<string, string> ESCFlatnessValue
        { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

        [DataMember(Order = 3)]
        public System.Collections.Generic.Dictionary<string, string> ProcessParameter
        { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

        [DataMember(Order = 4)]
        public System.Collections.Generic.Dictionary<string, string> StageOffsetValue
        { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

        [DataMember(Order = 99)]
        public string Version { get; set; } = "1.0";
    }
}
