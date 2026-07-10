using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.Config;
using Core.Logging;
using StageWin.Alarm;
using StageWin.Safety;
using StageWin.WagoIO;

namespace StageWin.Alarm
{
    public interface IBoolTagProvider
    {
        bool? GetInput(string tagName);   // X 영역
        bool? GetOutput(string tagName);  // Y 영역
    }

    public sealed class WagoIoProvider : IBoolTagProvider
    {
        public bool? GetInput(string tagName)
        {
            return VirtualBus.DigitalInputs.TryGet(tagName, out var v) ? v.RawBit : (bool?)null;
        }

        public bool? GetOutput(string tagName)
        {
            return VirtualBus.DigitalOutputs.TryGet(tagName, out var v) ? v.RawBit : (bool?)null;
        }
    }

    public sealed class AlarmManager : IDisposable
    {
        [Flags]
        public enum AllowedModes
        {
            None = 0,
            Manual = 1,
            Semi = 2,
            Auto = 4
        }

        private DateTime _bootStartedAt;
        private readonly TimeSpan _firstPopupHoldoff = TimeSpan.FromSeconds(2);
        private readonly List<string> _pendingAnnounce = new List<string>();

        readonly IBoolTagProvider _provider;
        readonly AlarmConfig _cfg;
        readonly BindingList<AlarmRow> _view; // gridAlarms 바인딩
        readonly Action<string> _log;
        readonly Func<ProgramMode> _modeProvider;
        readonly Action<ProgramMode> _forceModeSetter; // Manual 강제 전환용

        readonly System.Windows.Forms.Timer _poll = new System.Windows.Forms.Timer { Interval = 100 };
        readonly HashSet<string> _warned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> _announcedLatched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 런타임 상태
        readonly ConcurrentDictionary<string, AlarmRuntimeState> _states = new ConcurrentDictionary<string, AlarmRuntimeState>();

        // 팝업 스팸 방지: 이번 폴링에서 새로 Latched된 ID들
        readonly List<string> _justLatched = new List<string>();

        public AlarmManager(
            IBoolTagProvider provider,
            AlarmConfig cfg,
            BindingList<AlarmRow> view,
            Action<string> log,
            Func<ProgramMode> modeProvider,
            Action<ProgramMode> forceModeSetter)
        {
            _provider = provider;
            _cfg = cfg ?? new AlarmConfig();
            _view = view;
            _log = log ?? (_ => { });
            _modeProvider = modeProvider ?? (() => ProgramMode.Manual);
            _forceModeSetter = forceModeSetter ?? (_ => { });
            _poll.Tick += (s, e) => PollOnce();
        }

        public void Start()
        {
            _log($"[ALARM] Definitions loaded: {(_cfg?.Alarms?.Length ?? 0)}");
            ValidateConfigOnce();
            _bootStartedAt = DateTime.Now;
            _poll.Start();
        }
        private bool InHoldoff() => (DateTime.Now - _bootStartedAt) < _firstPopupHoldoff;
        public void Stop() => _poll.Stop();
        public void Dispose() => _poll?.Dispose();
        void ValidateConfigOnce()
        {
            foreach (var a in _cfg.Alarms ?? Array.Empty<AlarmDefinition>())
            {
                if (a.Source == AlarmSource.USER) continue;

                if (!TagExists(a.Tag))
                    WarnOnce($"[ALARM][cfg] Tag not found: '{a.Tag}' (Id={a.Id}, Source={a.Source})");

                if (a.SkipWhenAnyFalseTags != null)
                {
                    foreach (var t in a.SkipWhenAnyFalseTags)
                        if (!TagExists(t))
                            WarnOnce($"[ALARM][cfg] Skip tag not found: '{t}' (Id={a.Id})");
                }
            }
        }
        bool TagExists(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return false;
            return _provider.GetInput(tag).HasValue || _provider.GetOutput(tag).HasValue;
        }
        public bool HasActiveLevel(AlarmLevel level)
        {
            try { return _cfg.Alarms.Any(a => IsLatched(a) && a.Level == level); }
            catch { return false; }
        }
        void WarnOnce(string msg)
        {
            if (_warned.Add(msg)) _log(msg);
        }
        private bool IsLatched(AlarmDefinition def)
        {
            var key = def.Id ?? def.Tag ?? "";
            if (string.IsNullOrWhiteSpace(key)) return false;
            return _states.TryGetValue(key, out var st) && st.Latched;
        }

        public void ResetLatched()
        {
            try
            {
                var now = DateTime.Now;
                var stillActive = new List<AlarmDefinition>();

                foreach (var def in _cfg.Alarms)
                {
                    var key = def.Id ?? def.Tag ?? "";
                    if (!_states.TryGetValue(key, out var st)) continue;
                    if (!st.Latched) continue;

                    bool stillOn = EvaluateNow(def, forReset: true);
                    if (!stillOn)
                    {
                        st.Latched = false;
                        AppendRow(now, def, $"CLEARED - {def.Name}");
                        _log($"[ALARM] Cleared: {def.Id} [{def.Level}]");

                        // 기존 행 제거 로직 유지
                        var rows = _view.Where(v => v.Code == def.Code && v.Message.Contains(def.Name)).ToList();
                        foreach (var r in rows) _view.Remove(r);

                        _announcedLatched.Remove(key);
                    }
                    else
                    {
                        // Reset했지만 여전히 살아있음 → 새 행 추가 금지, 시간만 now로 갱신
                        stillActive.Add(def);
                        UpdateActiveRowTime(def, now);
                    }
                }

                if (stillActive.Count > 0)
                    PopupStillActiveAfterReset(stillActive);
            }
            catch (Exception ex)
            {
                Logger.Error("[AlarmManager.ResetLatched]", ex);
            }
        }

        public void RaiseUserAlarm(string idOrTag)
        {
            var def = _cfg.Find(idOrTag);
            if (def == null || def.Source != AlarmSource.USER) return;

            var key = def.Id ?? def.Tag;
            var st = _states.GetOrAdd(key, _ => new AlarmRuntimeState());

            if (!st.Latched)
            {
                st.CurrentActive = true;
                st.Latched = true;
                st.FirstTriggered = DateTime.Now;
                st.TriggerCount++;

                AppendRow(DateTime.Now, def, def.Name);
                _log($"[ALARM] USER Latched: {def.Id} [{def.Level}]");

                _justLatched.Clear();
                _justLatched.Add(key);
                PopupNewLatched();

                EnforceModeIfRequired(dueToNewLatched: true);
            }
        }

        public void ClearUserAlarm(string idOrTag, bool requireResetButton = true)
        {
            // requireResetButton=true면 여기선 표시만 변화 없이 → Reset 버튼에서 해제됨
            // false면 즉시 Latched 해제
            var def = _cfg.Find(idOrTag);
            if (def == null || def.Source != AlarmSource.USER) return;
            if (!_states.TryGetValue(def.Id ?? def.Tag ?? "", out var st)) return;

            if (!requireResetButton)
            {
                if (st.Latched)
                {
                    st.Latched = false;
                    AppendRow(DateTime.Now, def, $"CLEARED - {def.Name}");
                }
            }
            else
            {
                // 폴링에서 CurrentActive=false로 만들고, Reset에서 해제되도록 유도
                st.CurrentActive = false;
            }
        }
        // 현재 Latch된 알람을 기준으로 허용 모드 계산
        public AllowedModes GetAllowedModes()
        {
            // Alarm/Fatal Latch 하나라도 있으면 Manual만
            if (_cfg.Alarms.Any(a => IsLatched(a) && (a.Level == AlarmLevel.Alarm || a.Level == AlarmLevel.Fatal)))
                return AllowedModes.Manual;

            // ForceManual=true Latch 정책(예외 허용 교집합)
            var fm = _cfg.Alarms.Where(a => IsLatched(a) && a.ForceManual).ToList();
            if (fm.Count == 0)
           {
            // ②-1 ForceManual Latch가 없더라도, 비-ForceManual Latch 중 Auto 금지(cap)이 있으면 Auto만 제거
            bool capAuto = _cfg.Alarms.Any(a => IsLatched(a) && !a.ForceManual && !a.AllowAuto);
            return capAuto
                ? (AllowedModes.Manual | AllowedModes.Semi)           // Auto만 금지
                : (AllowedModes.Manual | AllowedModes.Semi | AllowedModes.Auto);
           }

            bool autoAllowed = fm.All(a => a.AllowAuto);
            // Auto가 허용되면 Semi는 자동 허용 (요구사항)
            bool semiAllowed = fm.All(a => a.AllowSemi) || autoAllowed;

            var allowed = AllowedModes.Manual;
            if (semiAllowed) allowed |= AllowedModes.Semi;
            if (autoAllowed) allowed |= AllowedModes.Auto;
            return allowed;
        }
        private bool HasRestriction()
        {
            // Alarm/Fatal Latch 또는 ForceManual Latch가 하나라도 있으면 제약 존재
            return _cfg.Alarms.Any(a =>
                IsLatched(a) && (
                    a.Level == AlarmLevel.Alarm ||
                    a.Level == AlarmLevel.Fatal ||
                    a.ForceManual));
        }
        public ProgramMode GetClosestAllowedMode(ProgramMode want)
        {
            var allowed = GetAllowedModes();
            bool allowM = allowed.HasFlag(AllowedModes.Manual);
            bool allowS = allowed.HasFlag(AllowedModes.Semi);
            bool allowA = allowed.HasFlag(AllowedModes.Auto);

            // 그대로 허용되면 그대로
            if (want == ProgramMode.Manual && allowM) return ProgramMode.Manual;
            if (want == ProgramMode.SemiAuto && allowS) return ProgramMode.SemiAuto;
            if (want == ProgramMode.Auto && allowA) return ProgramMode.Auto;

            // 하향 강제
            if (want == ProgramMode.Auto && allowS) return ProgramMode.SemiAuto;
            return ProgramMode.Manual; // 항상 Manual은 허용되도록 계산되어 있음
        }

        // 호출부에서 "Latch 새로 발생했는지" 신호를 주도록 변경
        void EnforceModeIfRequired(bool dueToNewLatched)
        {
            var cur = _modeProvider();
            var allowed = GetAllowedModes();

            ProgramMode target;

            if (dueToNewLatched & _cfg.Alarms.Any(a => IsLatched(a) && (a.Level == AlarmLevel.Alarm || a.Level == AlarmLevel.Fatal)))
            {
                // 심각 알람 새 Latch → Manual로 1회 강제
                target = ProgramMode.Manual;
            }
            else
            {
                // 현재 모드가 허용되면 건드리지 않음(사용자 선택 존중)
                if (IsModeAllowed(cur)) return;

                // 허용되지 않으면 하향 강제
                target = GetClosestAllowedMode(cur);
            }

            if (cur != target)
            {
                _forceModeSetter(target);
                _log($"[ALARM] Force change Mode → {target.ToString().ToUpper()}");
            }
        }

        public bool IsModeAllowed(ProgramMode m)
        {
            var allowed = GetAllowedModes();
            return (m == ProgramMode.Manual && allowed.HasFlag(AllowedModes.Manual))
                || (m == ProgramMode.SemiAuto && allowed.HasFlag(AllowedModes.Semi))
                || (m == ProgramMode.Auto && allowed.HasFlag(AllowedModes.Auto));
        }

        // 허용모드 중 “우선 목표 모드” 결정(Alarm/Fatal이면 Manual, 아니면 Auto > Semi > Manual 우선)
        public ProgramMode GetEnforcedMode()
        {
            var allowed = GetAllowedModes();

            // 우선순위: Auto > Semi > Manual
            if (allowed.HasFlag(AllowedModes.Auto)) return ProgramMode.Auto;
            if (allowed.HasFlag(AllowedModes.Semi)) return ProgramMode.SemiAuto;
            return ProgramMode.Manual;
        }
        void PollOnce()
        {
            try
            {
                _justLatched.Clear();

                foreach (var def in _cfg.Alarms)
                {
                    if (def.Source == AlarmSource.USER) continue;

                    bool active = EvaluateNow(def, forReset: false);
                    var key = def.Id ?? def.Tag ?? "";
                    var st = _states.GetOrAdd(key, _ => new AlarmRuntimeState());

                    bool wasActive = st.CurrentActive;
                    st.CurrentActive = active;

                    // 비활성 구간으로 들어오면 '안내했던 기록'을 지워 재발 시 다시 1회만 팝업되게
                    if (!active)
                        _announcedLatched.Remove(key);

                    // 새 Latch(처음 active)만 처리
                    if (!st.Latched && active)
                    {
                        st.Latched = def.Latch;
                        st.FirstTriggered = DateTime.Now;
                        st.TriggerCount++;

                        AppendRow(st.FirstTriggered, def, def.Name);
                        _log($"[ALARM] Latched: {def.Id} [{def.Level}]");

                        if (!_announcedLatched.Contains(key))
                        {
                            if (InHoldoff()) _pendingAnnounce.Add(key);
                            else _justLatched.Add(key);
                        }
                    }
                }

                if (!InHoldoff())
                {
                    var toAnnounce = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var id in _pendingAnnounce) toAnnounce.Add(id);
                    foreach (var id in _justLatched) toAnnounce.Add(id);

                    if (toAnnounce.Count > 0)
                    {
                        _justLatched.Clear();
                        _justLatched.AddRange(toAnnounce);
                        //PopupNewLatched();
                        _pendingAnnounce.Clear();
                    }
                }
                //PGT test
                //EnforceModeIfRequired(dueToNewLatched: _justLatched.Count > 0);
            }
            catch (Exception ex)
            {
                Logger.Error("[AlarmManager.PollOnce]", ex);
            }
        }

        private bool EvaluateNow(AlarmDefinition def, bool forReset)
        {
            // 스킵 조건
            if (def.SkipWhenAnyFalseTags != null && def.SkipWhenAnyFalseTags.Length > 0)
            {
                foreach (var t in def.SkipWhenAnyFalseTags)
                {
                    var s = _provider.GetInput(t) ?? _provider.GetOutput(t);
                    if (s == false) return false; // skip → 항상 not active 로 간주
                }
            }

            bool? cur = null;
            switch (def.Source)
            {
                case AlarmSource.X:
                    cur = _provider.GetInput(def.Tag);
                    break;
                case AlarmSource.Y:
                    cur = _provider.GetOutput(def.Tag);
                    break;
                default:
                    cur = null;
                    break;
            }

            if (cur == null)
            {
                WarnOnce($"[ALARM][poll] Tag not found at runtime: '{def.Tag}' (Id={def.Id}, Source={def.Source})");
                return false; // 태그 없음 → 미활성
            }

            var key = def.Id ?? def.Tag ?? "";
            var st = _states.GetOrAdd(key, _ => new AlarmRuntimeState());

            if (!st.HasBaseline)
            {
                st.LastInputState = cur.Value;
                st.HasBaseline = true;

                if (!forReset && (def.Condition == "EdgeRise" || def.Condition == "EdgeFall"))
                    return false; // 첫 바퀴에서 엣지 발생했다고 보지 않음
            }

            bool result;
            switch ((def.Condition ?? "On").Trim())
            {
                case "On": result = cur.Value; break;
                case "Off": result = !cur.Value; break;
                case "EdgeRise": result = (cur.Value == true && st.LastInputState == false); break;
                case "EdgeFall": result = (cur.Value == false && st.LastInputState == true); break;
                default: result = cur.Value; break;
            }

            // edge 상태 업데이트(Reset용 평가에서도 갱신)
            st.LastInputState = cur.Value;

            // Reset 시에는 Edge 조건을 On/Off로 환산
            if (forReset)
            {
                if (def.Condition == "EdgeRise") result = cur.Value;
                if (def.Condition == "EdgeFall") result = !cur.Value;
            }

            return result;
        }
        private void PopupNewLatched()
        {
            try
            {
                // 팝업 대상: Warning 제외
                var toShow = _justLatched
                    .Select(id => _cfg.Find(id))
                    .Where(def => def != null && def.Level != AlarmLevel.Warning)
                    .ToList();

                // 팝업은 Alarm/Fatal/Unlock만, Warning은 팝업 X (grid에는 이미 AppendRow로 들어감)
                if (toShow.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("알람이 발생했습니다.");
                    foreach (var def in toShow)
                        sb.AppendLine($"- [{def.Level}] ({def.Code}) {def.Name}");

                    MessageBox.Show(sb.ToString(), "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 재팝업 방지 마크는 기존대로 전부(Warning 포함) 찍어 둔다
                foreach (var id in _justLatched) _announcedLatched.Add(id);
            }
            catch { /* UI 오류 무시 */ }
        }

        // Reset 직후에도 살아있는 알람들 1회 요약 팝업
        private void PopupStillActiveAfterReset(IEnumerable<AlarmDefinition> defs)
        {
            try
            {
                // 팝업 대상에서 Warning 제외
                var list = defs.Where(d => d.Level != AlarmLevel.Warning).ToList();
                if (list.Count == 0) return; // 전부 Warning이면 팝업 자체 생략

                var sb = new StringBuilder();
                sb.AppendLine("알람이 여전히 유지 중입니다. (Reset 이후)");
                foreach (var def in list)
                    sb.AppendLine($"- [{def.Level}] ({def.Code}) {def.Name}");

                MessageBox.Show(sb.ToString(), "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { /* UI 오류 무시 */ }
        }
        private void PopupIfJustLatched(AlarmDefinition def)
        {
            try
            {
                if (def?.Level == AlarmLevel.Warning) return; // Warning은 팝업 금지
                MessageBox.Show($"[{def.Level}] ({def.Code}) {def.Name}",
                    "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }
        }

        private AlarmRow FindActiveRow(AlarmDefinition def)
        {
            // 같은 Code이고, CLEARED가 아닌(=활성 알람) 메시지를 가진 마지막 행을 찾아 업데이트 대상으로 사용
            for (int i = _view.Count - 1; i >= 0; i--)
            {
                var r = _view[i];
                if (r.Code == def.Code &&
                    !(r.Message?.StartsWith("CLEARED", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (r.Message?.IndexOf(def.Name ?? "", StringComparison.OrdinalIgnoreCase) >= 0))
                    return r;
            }
            return null;
        }

        private void UpdateActiveRowTime(AlarmDefinition def, DateTime? when = null)
        {
            var row = FindActiveRow(def);
            if (row != null) row.Time = when ?? DateTime.Now; // INotifyPropertyChanged로 그리드 즉시 갱신
        }

        void AppendRow(DateTime t, AlarmDefinition def, string msg)
        {
            try
            {
                bool isClear = msg.StartsWith("CLEARED", StringComparison.OrdinalIgnoreCase);

                // 활성 알람(클리어 메시지가 아닌 경우)인데 같은 알람 행이 이미 있으면 '추가' 대신 '시간만 갱신'
                if (!isClear)
                {
                    var existing = FindActiveRow(def);
                    if (existing != null)
                    {
                        existing.Time = t;   // ← 여기서 끝! (중복행 방지)
                        return;
                    }
                }

                // 신규 행 추가(클리어는 항상 새 행으로 남기고 싶다면 이대로 유지)
                _view.Add(new AlarmRow
                {
                    Time = t,
                    Code = def.Code,
                    Level = def.Level.ToString(),
                    Message = msg
                });

                // CSV 기록(신규 행 추가시에만 기록, 중복 갱신일 때는 기록하지 않음)
                try
                {
                    var path = AppConfig.Current?.AlarmCsvPath;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        var line = $"{t:yyyy-MM-dd HH:mm:ss.fff},{def.Code},{def.Level},\"{msg.Replace("\"", "\"\"")}\"{Environment.NewLine}";
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                        System.IO.File.AppendAllText(path, line, Encoding.UTF8);
                    }
                }
                catch { /* 파일 접근 실패 무시 */ }
            }
            catch { }
        }
    }
}
