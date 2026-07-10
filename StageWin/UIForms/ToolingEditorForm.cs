using System;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using StageWin.Core.Recipe;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;
using Core.Config;

namespace StageWin.UI
{
    public sealed partial class ToolingEditorForm : Form
    {
        [DataContract]
        private sealed class ToolRecipeCatalog
        {
            [DataMember(Order = 1)]
            public Dictionary<string, ToolingParam> Items { get; set; } =
                new Dictionary<string, ToolingParam>(StringComparer.OrdinalIgnoreCase);

            private static string NewPath =>
                AppConfig.Current?.ToolRecipesPath
                ?? Path.Combine(AppConfig.ConfigRoot, "ToolRecipes.json");

            private static string LegacyPath =>
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ToolRecipes.json");

            public static ToolRecipeCatalog Load()
            {
                try
                {
                    if (File.Exists(NewPath))
                        return Deserialize(File.ReadAllText(NewPath, Encoding.UTF8));

                    if (File.Exists(LegacyPath))
                        return Deserialize(File.ReadAllText(LegacyPath, Encoding.UTF8));
                }
                catch { }
                return new ToolRecipeCatalog();
            }

            public void Save()
            {
                try
                {
                    var dir = Path.GetDirectoryName(NewPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                    var ser = new DataContractJsonSerializer(typeof(ToolRecipeCatalog),
                        new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
                    using (var ms = new MemoryStream())
                    {
                        ser.WriteObject(ms, this);
                        File.WriteAllText(NewPath, Encoding.UTF8.GetString(ms.ToArray()), Encoding.UTF8);
                    }
                }
                catch { /* ignore */ }
            }

            private static ToolRecipeCatalog Deserialize(string json)
            {
                var ser = new DataContractJsonSerializer(typeof(ToolRecipeCatalog),
                    new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    return (ToolRecipeCatalog)ser.ReadObject(ms);
            }

            public IEnumerable<string> NamesWithBuiltins()
            {
                yield return "Default";
                foreach (var k in Items.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase)) yield return k;
                yield return "User";
            }
        }

        private RecipeDoc _doc;
        private ToolRecipeCatalog _catalog;

        private readonly BindingList<Row> _rows = new BindingList<Row>();
        private readonly BindingList<string> _toolNameChoices = new BindingList<string>();
        private readonly BindingList<string> _attSrcChoices = new BindingList<string>();
        private readonly BindingSource _bs = new BindingSource();

        private PowerRecipeStore _pmStore;

        private ContextMenuStrip _ctxTool; // Tool Name 전용
        private ContextMenuStrip _ctxAtt;  // Att Source 전용

        private static readonly Color AttPendingShade = Color.FromArgb(230, 230, 230);

        public event Action RequestApply;

        public ToolingEditorForm()
        {
            InitializeComponent();
            Text = "Tooling Editor (Local)";
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
            _grid.AutoGenerateColumns = false;
            EnsureGridColumns();
            _bs.DataSource = _rows;
            _grid.DataSource = _bs;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            _grid.RowTemplate.Height = 24;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            _grid.ScrollBars = ScrollBars.Both;
        }
        public ToolingEditorForm(RecipeDoc doc) : this()
        {
            InitializeRuntime(doc);
        }
        private bool _runtimeInited = false;
        public void LoadFrom(RecipeDoc doc)
        {
            if (!_runtimeInited) InitializeRuntime(doc);
            else
            {
                _doc = doc ?? throw new ArgumentNullException(nameof(doc));
                RefreshAttSrcChoices(); // doc 바뀌면 필요한 선택지/계산 다시
                RefreshToolNameChoices();
                LoadFromDoc();
            }
        }
        private void InitializeRuntime(RecipeDoc doc)
        {
            _runtimeInited = true;
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            _catalog = ToolRecipeCatalog.Load();
            _pmStore = PowerRecipeStore.Open(AppConfig.Current.PowerRecipesPath);
            RefreshAttSrcChoices();
            WireCellSelectionBehavior(_grid);
            BuildContextMenus();
            _grid.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (Control.MouseButtons == MouseButtons.Right) _grid.CurrentCell = _grid[e.ColumnIndex, e.RowIndex];
                var col = _grid.Columns[e.ColumnIndex].Name;
                if (col == "cName") _grid.ContextMenuStrip = _ctxTool;
                else if (col == "cAttSrc") _grid.ContextMenuStrip = _ctxAtt;
                else _grid.ContextMenuStrip = null;
            };
            _grid.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
                var col = _grid.Columns[e.ColumnIndex].Name;
                if (col == "cP" || col == "cF")
                {
                    _rows[e.RowIndex].ToolName = "User";
                    RefreshAttPendingAndMaybeRecalc(e.RowIndex);
                }
                else if (col == "cAttSrc")
                {
                    var chosen = (_grid[e.ColumnIndex, e.RowIndex].Value ?? "").ToString();
                    EnsureAttSrcChoice(chosen);
                    var rowtemp = _rows[e.RowIndex];
                    rowtemp.AttSrcName = string.IsNullOrWhiteSpace(chosen) ? "User" : chosen;
                    RefreshAttPendingAndMaybeRecalc(e.RowIndex);
                }
                else if (col == "cName")
                {
                    var chosen = (_grid[e.ColumnIndex, e.RowIndex].Value ?? "").ToString();
                    ApplyToolToCurrentSelection(chosen, applyByLine: false);
                }
            };
            _grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                var col = _grid.CurrentCell?.OwningColumn;
                if (col is DataGridViewComboBoxColumn || col is DataGridViewCheckBoxColumn)
                {
                    _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    _grid.EndEdit();
                }
            };
            _grid.DataError += (s, e) =>
            {
                if (e.Exception is ArgumentException)
                {
                    var colName = _grid.Columns[e.ColumnIndex].Name;
                    if (colName == "cName")
                    {
                        var val = _grid[e.ColumnIndex, e.RowIndex].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(val) && !_toolNameChoices.Contains(val))
                        { _toolNameChoices.Add(val); e.ThrowException = false; return; }
                        _grid[e.ColumnIndex, e.RowIndex].Value = "User";
                        e.ThrowException = false;
                        return;
                    }
                    if (colName == "cAttSrc")
                    {
                        var val = _grid[e.ColumnIndex, e.RowIndex].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(val) && !_attSrcChoices.Contains(val))
                        { _attSrcChoices.Add(val); e.ThrowException = false; return; }
                        _grid[e.ColumnIndex, e.RowIndex].Value = "User";
                        e.ThrowException = false;
                        return;
                    }
                }
            };
            _grid.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                var row = _grid.Rows[e.RowIndex].DataBoundItem as Row;
                if (row == null) return;
                var col = _grid.Columns[e.ColumnIndex].Name;
                if (row.AttPending && (col == "cAtt" || col == "cAttSrc"))
                    e.CellStyle.BackColor = AttPendingShade;
            };
            RefreshToolNameChoices();
            LoadFromDoc();
        }
        public void SaveInto(RecipeDoc doc)
        {
            if (doc == null) return;

            doc.DefaultTooling = new ToolingParam
            {
                Power = (double)numDefPower.Value,
                Freq = (double)numDefFreq.Value,
                ProcessSpeed = (double)numDefProc.Value,
                ShotCount = (int)numDefShot.Value,
                JumpSpeed = (double)numDefJump.Value,
                AttPos = 0.0,
                AttSrcName = "User",
                ToolName = "Default"
            };

            if (doc.Toolings == null)
                doc.Toolings = new Dictionary<string, ToolingParam>(StringComparer.OrdinalIgnoreCase);
            doc.Toolings.Clear();

            foreach (var r in _rows)
            {
                doc.Toolings[r.CellKey] = new ToolingParam
                {
                    Power = r.Power,
                    Freq = r.Freq,
                    ProcessSpeed = r.ProcessSpeed,
                    ShotCount = r.ShotCount,
                    JumpSpeed = r.JumpSpeed,
                    ToolName = string.IsNullOrWhiteSpace(r.ToolName) ? "User" : r.ToolName,
                    AttPos = r.AttPos,
                    AttSrcName = string.IsNullOrWhiteSpace(r.AttSrcName) ? "User" : r.AttSrcName
                };
            }
        }
        
        //  Powermeter / Att 연동
        private void RefreshAttSrcChoices()
        {
            _attSrcChoices.Clear();
            _attSrcChoices.Add("User");
            if (_pmStore != null)
                foreach (var n in _pmStore.ListNames())
                    _attSrcChoices.Add(n);
        }
        private void EnsureAttSrcChoice(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (!_attSrcChoices.Contains(name)) _attSrcChoices.Add(name);
        }
        private bool TryResolveAttFromPM(Row r, out double att)
        {
            att = 0;
            try
            {
                if (_pmStore == null) return false;
                var name = r.AttSrcName;
                if (string.IsNullOrWhiteSpace(name) || string.Equals(name, "User", StringComparison.OrdinalIgnoreCase))
                    return false;

                var pm = _pmStore.Load(name);
                if (pm == null) return false;

                double freq = (r.Freq > 0 ? r.Freq : (_doc?.DefaultTooling?.Freq ?? 0));
                var table = pm.PickTable(freq);
                if (table?.Rows == null || table.Rows.Count == 0) return false;

                var rows = table.Rows.OrderBy(tar => tar.TargetW).ToArray();
                double w = r.Power;

                if (w <= rows.First().TargetW + 1e-12) { att = rows.First().AttPos; return true; }
                if (w >= rows.Last().TargetW - 1e-12) { att = rows.Last().AttPos; return true; }

                var exact = rows.FirstOrDefault(x => Math.Abs(x.TargetW - w) <= 1e-12);
                if (exact != null) { att = exact.AttPos; return true; }

                var lower = rows.Last(x => x.TargetW <= w);
                var upper = rows.First(x => x.TargetW >= w);
                if (Math.Abs(upper.TargetW - lower.TargetW) < 1e-12) { att = lower.AttPos; return true; }

                double t = (w - lower.TargetW) / (upper.TargetW - lower.TargetW);
                double pos = lower.AttPos + (upper.AttPos - lower.AttPos) * t;

                double mn = Math.Min(lower.AttMin, upper.AttMin);
                double mx = Math.Max(lower.AttMax, upper.AttMax);
                att = Math.Min(mx, Math.Max(mn, pos));
                return true;
            }
            catch { return false; }
        }
        private void RefreshAttPendingAndMaybeRecalc(Row r)
        {
            if (r == null) return;

            if (string.Equals(r.AttSrcName ?? "User", "User", StringComparison.OrdinalIgnoreCase))
            {
                r.AttPending = false;
                return;
            }

            if (TryResolveAttFromPM(r, out var att))
            {
                if (Math.Abs(r.AttPos - att) > 1e-9)
                    r.AttPos = att;
                r.AttPending = false;
            }
            else
            {
                r.AttPending = true;
            }
        }
        private void RefreshAttPendingAndMaybeRecalc(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows.Count) return;
            var r = _rows[rowIndex];
            RefreshAttPendingAndMaybeRecalc(r);
            _bs.ResetItem(rowIndex);
        }
        private sealed class Row
        {
            public int Index { get; set; }
            public int RowNo { get; set; }     // Line
            public int ColNo { get; set; }     // Hole
            public int VectorRow { get; set; } // 0..vecRows-1
            public int VectorCol { get; set; } // 0..vecCols-1
            public int GlobalR { get; set; }   // (hole-1)*vecRows + VectorRow
            public int GlobalC { get; set; }   // (line-1)*vecCols + VectorCol
            public string RC => $"R{GlobalR}C{VectorCol}";
            public double X { get; set; }      // Review X (mm)  ※ gridScan의 cellX
            public double Y { get; set; }      // Main Y (mm)    ※ gridScan의 cellY
            public double Power { get; set; }
            public double Freq { get; set; }
            public double ProcessSpeed { get; set; }
            public int ShotCount { get; set; }
            public double JumpSpeed { get; set; }
            public string ToolName { get; set; }
            public double AttPos { get; set; }
            public string AttSrcName { get; set; } = "User";
            public bool AttPending { get; set; }
            public string CellKey => $"{RowNo}-{ColNo}-{VectorRow}-{VectorCol}";
            public string StageKey => $"{RowNo}-{ColNo}";
        }
        private static double CenterOrigin(int n)
        {
            if (n <= 1) return 0.0;
            return (n - 1) * 0.5;
        }
        private void ComputeScanCellXY(int line1, int hole1, int vR, int vC, out double x, out double y, out int globalR, out int globalC)
        {
            x = 0; y = 0; globalR = 0; globalC = 0;
            if (_doc == null) return;
            int lines = Math.Max(1, _doc.Parameters?.Lines ?? 1);
            int holes = Math.Max(1, _doc.Parameters?.HolesPerLine ?? 1);
            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, _doc.Parameters?.VectorCols ?? 1);
            line1 = Math.Max(1, Math.Min(lines, line1));
            hole1 = Math.Max(1, Math.Min(holes, hole1));
            vR = Math.Max(0, Math.Min(vecRows - 1, vR));
            vC = Math.Max(0, Math.Min(vecCols - 1, vC));
            double pitchX = _doc.Parameters?.PitchX ?? 0.0;
            double pitchY = _doc.Parameters?.PitchY ?? 0.0;
            double xCenterReview = _doc.Offset?.ReviewOffsetX ?? 0.0;
            double yCenterReview = _doc.Offset?.ReviewOffsetY ?? 0.0;
            double vOrgR = CenterOrigin(vecRows);
            double vOrgC = CenterOrigin(vecCols);
            globalR = (hole1 - 1) * vecRows + vR;
            globalC = (line1 - 1) * vecCols + vC;
            double rx = xCenterReview - ((globalC - vOrgC) * pitchX);
            double ry = yCenterReview - ((globalR - vOrgR) * pitchY);
            double scanToRevX = _doc.Offset?.ScanToReviewOffsetX ?? 0.0;
            double scanToRevY = _doc.Offset?.ScanToReviewOffsetY ?? 0.0;
            x = rx - scanToRevX;
            y = ry - scanToRevY;
        }
        private void EnsureGridColumns()
        {
            if (_grid.Columns.Count > 0) return;

            AddNumberCol("cIdx", "#", "Index", 50, 0, DataGridViewContentAlignment.MiddleCenter, readOnly: true);
            AddNumberCol("cRow", "Line", "RowNo", 50, 0, DataGridViewContentAlignment.MiddleCenter, readOnly: true);
            AddNumberCol("cCol", "Holes", "ColNo", 60, 0, DataGridViewContentAlignment.MiddleCenter, readOnly: true);
            AddNumberCol("cVecR", "VecR", "VectorRow", 55, 0, DataGridViewContentAlignment.MiddleCenter, readOnly: true);
            AddNumberCol("cVecC", "VecC", "VectorCol", 55, 0, DataGridViewContentAlignment.MiddleCenter, readOnly: true);
            var cRC = new DataGridViewTextBoxColumn
            {
                Name = "cRC",
                HeaderText = "R/C",
                DataPropertyName = "RC",
                Width = 70,
                ReadOnly = true,
                Resizable = DataGridViewTriState.False
            };
            cRC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns.Add(cRC);

            AddNumberCol("cX", "Review X (mm)", "X", 105, 3, DataGridViewContentAlignment.MiddleRight, readOnly: true);
            AddNumberCol("cY", "Main Y (mm)", "Y", 95, 3, DataGridViewContentAlignment.MiddleRight, readOnly: true);
            AddNumberCol("cP", "Power", "Power", 90, 3, DataGridViewContentAlignment.MiddleRight);
            AddNumberCol("cF", "Freq", "Freq", 90, 3, DataGridViewContentAlignment.MiddleRight);
            AddNumberCol("cS", "ProcSpd", "ProcessSpeed", 90, 0, DataGridViewContentAlignment.MiddleRight);
            AddNumberCol("cShot", "Shot", "ShotCount", 70, 0, DataGridViewContentAlignment.MiddleCenter);
            AddNumberCol("cJ", "Jump", "JumpSpeed", 90, 2, DataGridViewContentAlignment.MiddleRight);
            AddNumberCol("cAtt", "Att Pos", "AttPos", 90, 3, DataGridViewContentAlignment.MiddleRight);

            var cAttSrc = new DataGridViewComboBoxColumn
            {
                Name = "cAttSrc",
                HeaderText = "Att Source",
                DataPropertyName = "AttSrcName",
                Width = 160,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                DataSource = _attSrcChoices
            };
            _grid.Columns.Add(cAttSrc);

            var combo = new DataGridViewComboBoxColumn
            {
                Name = "cName",
                HeaderText = "Tool Name",
                DataPropertyName = "ToolName",
                Width = 140,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                DataSource = _toolNameChoices
            };
            _grid.Columns.Add(combo);
            _grid.CellBeginEdit += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (_grid.Columns[e.ColumnIndex].Name != "cAtt") return;
                var r = _grid.Rows[e.RowIndex].DataBoundItem as Row;
                if (r != null && !string.Equals(r.AttSrcName, "User", StringComparison.OrdinalIgnoreCase))
                    e.Cancel = true;
            };
        }
        private DataGridViewTextBoxColumn AddNumberCol(
            string name, string header, string prop,
            int width, int decimalPlaces,
            DataGridViewContentAlignment align,
            bool readOnly = false)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.False
            };
            col.DefaultCellStyle.Alignment = align;
            col.DefaultCellStyle.Format = "N" + Math.Max(0, decimalPlaces);
            col.ValueType = (decimalPlaces == 0) ? typeof(int) : typeof(double);
            _grid.Columns.Add(col);
            return col;
        }
        private void EnsureChoicesContain(IEnumerable<string> names)
        {
            foreach (var n in names.Where(x => !string.IsNullOrWhiteSpace(x)))
                if (!_toolNameChoices.Contains(n)) _toolNameChoices.Add(n);
        }

        private void RefreshToolNameChoices()
        {
            _toolNameChoices.Clear();
            EnsureChoicesContain(_catalog.NamesWithBuiltins());
        }
        private void LoadFromDoc()
        {
            var d = _doc.DefaultTooling ?? ToolingParam.CreateDefault();
            numDefPower.Value = (decimal)d.Power;
            numDefFreq.Value = (decimal)d.Freq;
            numDefProc.Value = (decimal)d.ProcessSpeed;
            numDefShot.Value = d.ShotCount;
            numDefJump.Value = (decimal)d.JumpSpeed;
            RefreshToolNameChoices();

            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Default", "User" };
            if (_doc.Toolings != null)
                foreach (var tpv in _doc.Toolings.Values)
                    if (!string.IsNullOrWhiteSpace(tpv.ToolName))
                        usedNames.Add(tpv.ToolName);
            EnsureChoicesContain(usedNames);

            _rows.Clear();
            _bs.ResetBindings(false);

            int lines = Math.Max(1, _doc.Parameters?.Lines ?? 1);
            int holes = Math.Max(1, _doc.Parameters?.HolesPerLine ?? 1);
            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, _doc.Parameters?.VectorCols ?? 1);
            int idx = 1;
            for (int line = 1; line <= lines; line++)
            {
                for (int hole = 1; hole <= holes; hole++)
                {
                    for (int vR = 0; vR < vecRows; vR++)
                    {
                        for (int vC = 0; vC < vecCols; vC++)
                        {
                            ComputeScanCellXY(line, hole, vR, vC, out double x, out double y, out int gR, out int gC);
                            ToolingParam tp = null;

                            string cellKey = $"{line}-{hole}-{vR}-{vC}";
                            string stageKey = $"{line}-{hole}";

                            if (_doc.Toolings != null)
                            {
                                if (_doc.Toolings.TryGetValue(cellKey, out var foundCell)) tp = foundCell;
                                else if (_doc.Toolings.TryGetValue(stageKey, out var foundStage)) tp = foundStage;
                            }
                            if (tp == null) tp = _doc.DefaultTooling ?? ToolingParam.CreateDefault();

                            double att = (tp.AttPos != 0) ? tp.AttPos : 0.0;
                            string attSrc = string.IsNullOrWhiteSpace(tp.AttSrcName) ? "User" : tp.AttSrcName;

                            _rows.Add(new Row
                            {
                                Index = idx++,
                                RowNo = line,
                                ColNo = hole,
                                VectorRow = vR,
                                VectorCol = vC,
                                GlobalR = gR,
                                GlobalC = gC,
                                X = x,
                                Y = y,

                                Power = tp.Power,
                                Freq = tp.Freq,
                                ProcessSpeed = tp.ProcessSpeed,
                                ShotCount = tp.ShotCount,
                                JumpSpeed = tp.JumpSpeed,
                                ToolName = string.IsNullOrWhiteSpace(tp.ToolName)
                                    ? (tp == _doc.DefaultTooling ? "Default" : "User")
                                    : tp.ToolName,
                                AttPos = att,
                                AttSrcName = attSrc
                            });

                            RefreshAttPendingAndMaybeRecalc(_rows[_rows.Count - 1]);
                        }
                    }
                }
            }
            _bs.ResetBindings(false);
        }
        private void BuildContextMenus()
        {
            _ctxTool = new ContextMenuStrip();
            var miApplySel = new ToolStripMenuItem("Apply Selected Tool → Selected");
            var miApplyLine = new ToolStripMenuItem("Apply Selected Tool → Same Line(s)");
            var miToolApplyDefaultAll = new ToolStripMenuItem("Apply Default Tool → All");
            var miToolCopySelAll = new ToolStripMenuItem("Copy Selected Tool → All");
            var miSaveRowAsTemplate = new ToolStripMenuItem("Save Current Row As Template...");
            var miDeleteTemplate = new ToolStripMenuItem("Delete Template");

            _ctxTool.Items.AddRange(new ToolStripItem[] {
                miApplySel, miApplyLine,
                new ToolStripSeparator(),
                miToolApplyDefaultAll, miToolCopySelAll,
                new ToolStripSeparator(),
                miSaveRowAsTemplate,
                new ToolStripSeparator(),
                miDeleteTemplate
            });

            miApplySel.Click += (s, e) =>
            {
                var r = _grid.CurrentRow?.DataBoundItem as Row;
                if (r != null) ApplyToolToCurrentSelection(r.ToolName, applyByLine: false);
            };
            miApplyLine.Click += (s, e) =>
            {
                var r = _grid.CurrentRow?.DataBoundItem as Row;
                if (r != null) ApplyToolToCurrentSelection(r.ToolName, applyByLine: true);
            };
            miSaveRowAsTemplate.Click += (s, e) => SaveCurrentRowAsTemplate();
            miToolApplyDefaultAll.Click += (s, e) => ToolApplyDefaultAll();
            miToolCopySelAll.Click += (s, e) => ToolCopySelectedToAll();

            _ctxTool.Opening += (s, e) =>
            {
                miDeleteTemplate.DropDownItems.Clear();
                var deletables = _catalog.Items.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray();
                if (deletables.Length == 0)
                {
                    miDeleteTemplate.DropDownItems.Add(new ToolStripMenuItem("(no user templates)") { Enabled = false });
                    miDeleteTemplate.Enabled = false;
                    return;
                }
                foreach (var name in deletables)
                {
                    var item = new ToolStripMenuItem(name);
                    item.Click += (s2, e2) => DeleteTemplate(name);
                    miDeleteTemplate.DropDownItems.Add(item);
                }
                miDeleteTemplate.Enabled = true;
            };

            _ctxAtt = new ContextMenuStrip();
            var miApplyAttSel = new ToolStripMenuItem("Apply Selected AttSrc → Selected");
            var miApplyAttLine = new ToolStripMenuItem("Apply Selected AttSrc → Same Line(s)");
            var miAttApplyDefaultAll = new ToolStripMenuItem("Apply Default AttSrc → All");
            var miAttCopySelAll = new ToolStripMenuItem("Copy Selected AttSrc → All");

            _ctxAtt.Items.AddRange(new ToolStripItem[] {
                miApplyAttSel, miApplyAttLine,
                new ToolStripSeparator(),
                miAttApplyDefaultAll, miAttCopySelAll
            });

            miApplyAttSel.Click += (s2, e2) =>
            {
                var r = _grid.CurrentRow?.DataBoundItem as Row;
                if (r != null) ApplyAttSrcToSelection(r.AttSrcName, applyByLine: false);
            };
            miApplyAttLine.Click += (s2, e2) =>
            {
                var r = _grid.CurrentRow?.DataBoundItem as Row;
                if (r != null) ApplyAttSrcToSelection(r.AttSrcName, applyByLine: true);
            };
            miAttApplyDefaultAll.Click += (s, e) => AttApplyDefaultAll();
            miAttCopySelAll.Click += (s, e) => AttCopySelectedToAll();
        }
        private void SaveCurrentRowAsTemplate()
        {
            var row = _grid.CurrentRow?.DataBoundItem as Row;
            if (row == null) return;

            var name = PromptForString(this, "Template Name", "Enter a name for this tool:", row.ToolName);
            if (string.IsNullOrWhiteSpace(name)) return;

            _catalog.Items[name] = new ToolingParam
            {
                Power = row.Power,
                Freq = row.Freq,
                ProcessSpeed = row.ProcessSpeed,
                ShotCount = row.ShotCount,
                JumpSpeed = row.JumpSpeed,
                ToolName = name
            };
            _catalog.Save();
            RefreshToolNameChoices();
            ApplyToolToCurrentSelection(name, applyByLine: false);
        }
        private static string PromptForString(IWin32Window owner, string title, string label, string initial = "")
        {
            using (var f = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = new Size(380, 120),
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var lbl = new Label { Left = 12, Top = 12, Width = 340, Text = label };
                var tb = new TextBox { Left = 12, Top = 36, Width = 340, Text = initial };
                var ok = new Button { Left = 190, Top = 70, Width = 80, Text = "OK", DialogResult = DialogResult.OK };
                var ck = new Button { Left = 280, Top = 70, Width = 80, Text = "Cancel", DialogResult = DialogResult.Cancel };
                f.AcceptButton = ok; f.CancelButton = ck;
                f.Controls.AddRange(new Control[] { lbl, tb, ok, ck });

                return f.ShowDialog(owner) == DialogResult.OK ? tb.Text : null;
            }
        }
        private void DeleteTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (!_catalog.Items.ContainsKey(name)) return;

            var q = MessageBox.Show(this,
                $"Delete template '{name}'?\r\n(This won’t change numeric values already applied to rows.)",
                "Delete Template",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (q != DialogResult.Yes) return;

            try
            {
                _catalog.Items.Remove(name);
                _catalog.Save();

                RefreshToolNameChoices();

                foreach (var r in _rows)
                    if (string.Equals(r.ToolName, name, StringComparison.OrdinalIgnoreCase))
                        r.ToolName = "User";

                _grid.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to delete template: " + ex.Message,
                    "Delete Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ApplyToolToCurrentSelection(string toolName, bool applyByLine)
        {
            if (string.IsNullOrWhiteSpace(toolName)) return;
            EnsureChoicesContain(new[] { toolName });

            ToolingParam src;
            if (string.Equals(toolName, "Default", StringComparison.OrdinalIgnoreCase))
                src = (_doc.DefaultTooling ?? ToolingParam.CreateDefault()).Clone();
            else if (string.Equals(toolName, "User", StringComparison.OrdinalIgnoreCase))
                src = null;
            else
            {
                if (!_catalog.Items.TryGetValue(toolName, out var found)) return;
                src = found.Clone();
            }

            var targetRows = new HashSet<int>();
            foreach (DataGridViewCell c in _grid.SelectedCells)
                if (c.RowIndex >= 0 && c.RowIndex < _rows.Count) targetRows.Add(c.RowIndex);

            if (applyByLine && targetRows.Count > 0)
            {
                var lines = new HashSet<int>(targetRows.Select(i => _rows[i].RowNo));
                targetRows.Clear();
                for (int i = 0; i < _rows.Count; i++)
                    if (lines.Contains(_rows[i].RowNo)) targetRows.Add(i);
            }

            foreach (int i in targetRows)
            {
                var r = _rows[i];
                if (src != null)
                {
                    r.Power = src.Power;
                    r.Freq = src.Freq;
                    r.ProcessSpeed = src.ProcessSpeed;
                    r.ShotCount = src.ShotCount;
                    r.JumpSpeed = src.JumpSpeed;
                }
                r.ToolName = toolName;

                RefreshAttPendingAndMaybeRecalc(r);
                _bs.ResetItem(i);
            }
        }
        private void ApplyAttSrcToSelection(string srcName, bool applyByLine)
        {
            if (string.IsNullOrWhiteSpace(srcName)) srcName = "User";
            EnsureAttSrcChoice(srcName);

            var targetIdx = new HashSet<int>();
            foreach (DataGridViewCell c in _grid.SelectedCells)
                if (c.RowIndex >= 0 && c.RowIndex < _rows.Count) targetIdx.Add(c.RowIndex);

            if (applyByLine && targetIdx.Count > 0)
            {
                var lines = new HashSet<int>(targetIdx.Select(i => _rows[i].RowNo));
                targetIdx.Clear();
                for (int i = 0; i < _rows.Count; i++)
                    if (lines.Contains(_rows[i].RowNo)) targetIdx.Add(i);
            }

            foreach (int i in targetIdx)
            {
                var r = _rows[i];
                r.AttSrcName = srcName;
                RefreshAttPendingAndMaybeRecalc(r);
                _bs.ResetItem(i);
            }
        }
        private void ToolApplyDefaultAll()
        {
            var d = new ToolingParam
            {
                Power = (double)numDefPower.Value,
                Freq = (double)numDefFreq.Value,
                ProcessSpeed = (double)numDefProc.Value,
                ShotCount = (int)numDefShot.Value,
                JumpSpeed = (double)numDefJump.Value
            };

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                r.Power = d.Power;
                r.Freq = d.Freq;
                r.ProcessSpeed = d.ProcessSpeed;
                r.ShotCount = d.ShotCount;
                r.JumpSpeed = d.JumpSpeed;
                r.ToolName = "Default";

                RefreshAttPendingAndMaybeRecalc(r);
                _bs.ResetItem(i);
            }
        }
        private void ToolCopySelectedToAll()
        {
            _grid.EndEdit();
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var src = _grid.CurrentRow?.DataBoundItem as Row;
            if (src == null) return;

            var toolName = string.IsNullOrWhiteSpace(src.ToolName) ? "User" : src.ToolName;
            EnsureChoicesContain(new[] { toolName });

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                r.Power = src.Power;
                r.Freq = src.Freq;
                r.ProcessSpeed = src.ProcessSpeed;
                r.ShotCount = src.ShotCount;
                r.JumpSpeed = src.JumpSpeed;
                r.ToolName = toolName;

                RefreshAttPendingAndMaybeRecalc(r);
                _bs.ResetItem(i);
            }
        }
        private void AttApplyDefaultAll()
        {
            double defAtt = 0.0;

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                r.AttSrcName = "User";
                r.AttPos = defAtt;
                r.AttPending = false;
                _bs.ResetItem(i);
            }
        }
        private void AttCopySelectedToAll()
        {
            _grid.EndEdit();
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var src = _grid.CurrentRow?.DataBoundItem as Row;
            if (src == null) return;

            string srcName = string.IsNullOrWhiteSpace(src.AttSrcName) ? "User" : src.AttSrcName;
            EnsureAttSrcChoice(srcName);

            if (string.Equals(srcName, "User", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    var r = _rows[i];
                    r.AttSrcName = "User";
                    r.AttPos = src.AttPos;
                    r.AttPending = false;
                    _bs.ResetItem(i);
                }
            }
            else
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    var r = _rows[i];
                    r.AttSrcName = srcName;
                    RefreshAttPendingAndMaybeRecalc(r);
                    _bs.ResetItem(i);
                }
            }
        }
        private bool HasPendingAtt() => _rows.Any(r => r.AttPending);
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (HasPendingAtt())
            {
                MessageBox.Show(this,
                    "일부 행의 Att Pos가 선택된 Powermeter 레시피 기준으로 갱신되지 않았습니다.\r\n" +
                    "음영(Att Source/Att Pos) 표시된 셀을 확인 후 레시피를 적용해 주세요.",
                    "Att 미반영",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommitGridEdits();
            SaveToDoc();
            RequestApply?.Invoke();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private static int CenterIndexLower(int n)
        {
            if (n <= 1) return 0;
            return (n - 1) / 2; // even이면 낮은 쪽 센터
        }
        private void SaveToDoc()
        {
            _doc.DefaultTooling = new ToolingParam
            {
                Power = (double)numDefPower.Value,
                Freq = (double)numDefFreq.Value,
                ProcessSpeed = (double)numDefProc.Value,
                ShotCount = (int)numDefShot.Value,
                JumpSpeed = (double)numDefJump.Value,
                ToolName = "Default",
                AttPos = 0.0,
                AttSrcName = "User"
            };

            if (_doc.Toolings == null)
                _doc.Toolings = new Dictionary<string, ToolingParam>(StringComparer.OrdinalIgnoreCase);
            _doc.Toolings.Clear();

            int vecRows = Math.Max(1, _doc.Parameters?.VectorRows ?? 1);
            int vecCols = Math.Max(1, _doc.Parameters?.VectorCols ?? 1);
            int cVR = CenterIndexLower(vecRows);
            int cVC = CenterIndexLower(vecCols);

            // 1) 셀키 저장(기본값과 다른 것만)
            foreach (Row r in _rows)
            {
                var cur = new ToolingParam
                {
                    Power = r.Power,
                    Freq = r.Freq,
                    ProcessSpeed = r.ProcessSpeed,
                    ShotCount = r.ShotCount,
                    JumpSpeed = r.JumpSpeed,
                    ToolName = string.IsNullOrWhiteSpace(r.ToolName) ? "User" : r.ToolName,
                    AttPos = r.AttPos,
                    AttSrcName = string.IsNullOrWhiteSpace(r.AttSrcName) ? "User" : r.AttSrcName
                };

                if (!IsSame(cur, _doc.DefaultTooling))
                    _doc.Toolings[r.CellKey] = cur;
            }

            // 2) 레거시(stageKey)도 센터셀 값을 저장(기존 BuildToolingListForScan이 stageKey만 볼 때 대응)
            //    - 센터셀도 기본값이면 굳이 stageKey 안 저장
            var centerMap = _rows
                .Where(r => r.VectorRow == cVR && r.VectorCol == cVC)
                .GroupBy(r => r.StageKey);

            foreach (var g in centerMap)
            {
                var r = g.First();
                var cur = new ToolingParam
                {
                    Power = r.Power,
                    Freq = r.Freq,
                    ProcessSpeed = r.ProcessSpeed,
                    ShotCount = r.ShotCount,
                    JumpSpeed = r.JumpSpeed,
                    ToolName = string.IsNullOrWhiteSpace(r.ToolName) ? "User" : r.ToolName,
                    AttPos = r.AttPos,
                    AttSrcName = string.IsNullOrWhiteSpace(r.AttSrcName) ? "User" : r.AttSrcName
                };

                if (!IsSame(cur, _doc.DefaultTooling))
                    _doc.Toolings[r.StageKey] = cur;
            }
        }
        private void CommitGridEdits()
        {
            _grid.EndEdit();
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            var cm = this.BindingContext[_bs] as CurrencyManager;
            cm?.EndCurrentEdit();
        }
        private static bool IsSame(ToolingParam a, ToolingParam b)
        {
            if (a == null || b == null) return false;
            return Math.Abs(a.Power - b.Power) <= 1e-9
                && Math.Abs(a.Freq - b.Freq) <= 1e-9
                && Math.Abs(a.ProcessSpeed - b.ProcessSpeed) <= 1e-9
                && a.ShotCount == b.ShotCount
                && Math.Abs(a.JumpSpeed - b.JumpSpeed) <= 1e-9
                && Math.Abs(a.AttPos - b.AttPos) <= 1e-9
                && string.Equals(a.AttSrcName ?? "User", b.AttSrcName ?? "User", StringComparison.OrdinalIgnoreCase);
        }
        private void WireCellSelectionBehavior(DataGridView gv)
        {
            bool shift = false;
            gv.AllowUserToAddRows = false;
            gv.AllowUserToDeleteRows = false;
            gv.AllowUserToResizeColumns = false;
            gv.AllowUserToResizeRows = false;
            gv.RowHeadersVisible = false;

            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            gv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gv.ScrollBars = ScrollBars.Both;

            gv.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey) { shift = true; gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; }
            };
            gv.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey) { shift = false; gv.SelectionMode = DataGridViewSelectionMode.CellSelect; }
            };
            gv.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (!Control.ModifierKeys.HasFlag(Keys.Control) && !shift) gv.ClearSelection();
                if (shift) gv.Rows[e.RowIndex].Selected = true; else gv[e.ColumnIndex, e.RowIndex].Selected = true;
            };
        }
        private void ToolingEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK && !e.Cancel)
            {
                CommitGridEdits();
                SaveToDoc();
                RequestApply?.Invoke();
            }
        }
    }
}
