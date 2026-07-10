using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Logging
{
    /// <summary>
    /// - System(기본)  : D:\AppLog\System\yyyyMMdd.log
    /// - Event(조작)   : D:\AppLog\Event\yyyyMMdd.log
    /// - Monitor(상태) : D:\AppLog\Monitor\yyyyMMdd.log
    /// - Auto(기판1장) : D:\AppLog\Auto\yyyyMMdd\Auto_<BoardId>_<Recipe>_<StartTime>.log
    /// </summary>
    public static class Logger
    {
        public static event Action<string> OnLog;

        //  절대경로 고정
        public static string LogDirectory { get; private set; } = @"D:\AppLog";

        static readonly object _sync = new object();

        // Auto(기판 단위) 컨텍스트
        private sealed class AutoContext
        {
            public string BoardId;
            public string RecipeName;
            public bool DryRun;
            public DateTime StartAt;
            public Stopwatch TotalSw = Stopwatch.StartNew();
            public readonly List<StepRecord> Steps = new List<StepRecord>();
            public string AutoFilePath;
        }
        private sealed class StepRecord
        {
            public string Name;
            public TimeSpan Duration;
            public bool Ok;
            public string Note;
        }

        private static readonly AsyncLocal<AutoContext> _auto = new AsyncLocal<AutoContext>();
        static Logger()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory); // D:\AppLog 생성
            }
            catch
            {
                // D: 사용 불가 시 안전 폴백(프로젝트\Logs)
                LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                try { Directory.CreateDirectory(LogDirectory); } catch { }
            }

            // 하위 폴더 준비
            TryCreateDir(Path.Combine(LogDirectory, "System"));
            TryCreateDir(Path.Combine(LogDirectory, "Event"));
            TryCreateDir(Path.Combine(LogDirectory, "Monitor"));
            TryCreateDir(Path.Combine(LogDirectory, "Auto"));
        }

        private static string F(params (string k, object v)[] kvs)
        {
            if (kvs == null || kvs.Length == 0) return "";
            var parts = new List<string>(kvs.Length);
            foreach (var kv in kvs)
            {
                if (string.IsNullOrWhiteSpace(kv.k)) continue;
                string val;
                if (kv.v == null) val = "null";
                else if (kv.v is double d) val = d.ToString("0.###");
                else if (kv.v is float f) val = f.ToString("0.###");
                else val = kv.v.ToString();
                parts.Add($"{kv.k}={val}");
            }
            return string.Join(", ", parts);
        }

        private static void TryCreateDir(string dir)
        {
            try { Directory.CreateDirectory(dir); } catch { }
        }

        private static string NowLinePrefix(string level) => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}]";

        private static void WriteLineToFile(string path, string line)
        {
            try
            {
                // 폴더 보장
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir)) TryCreateDir(dir);

                // 동시접근 보호
                lock (_sync)
                {
                    using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch
            {
                // 파일 접근 에러는 무시 (동작 우선)
            }
        }

        private static void EmitToUiAndDebug(string line)
        {
            try { OnLog?.Invoke(line); } catch { }
            try { Debug.WriteLine(line); } catch { }
        }

        private static string DailyPath(string categoryFolder)
        {
            return Path.Combine(LogDirectory, categoryFolder, $"{DateTime.Now:yyyyMMdd}.log");
        }

        private static void Write(string categoryTag, string level, string msg, string filePath)
        {
            string line = $"{NowLinePrefix(level)} [{categoryTag}] {msg}";
            EmitToUiAndDebug(line);
            WriteLineToFile(filePath, line);
        }

        // ===== 기본(System) =====
        public static void Info(string msg) => Write("SYS", "I", msg, DailyPath("System"));
        public static void Warn(string msg) => Write("SYS", "W", msg, DailyPath("System"));
        public static void Error(string msg, Exception ex = null)
            => Write("SYS", "E",
                ex == null ? msg : $"{msg} :: {ex.GetType().Name} - {ex.Message}",
                DailyPath("System"));

        // ===== Event(조작 이력) =====
        public static void Event(string msg) => Write("EVT", "I", msg, DailyPath("Event"));
        public static void EventWarn(string msg) => Write("EVT", "W", msg, DailyPath("Event"));
        public static void EventError(string msg, Exception ex = null)
            => Write("EVT", "E",
                ex == null ? msg : $"{msg} :: {ex.GetType().Name} - {ex.Message}",
                DailyPath("Event"));

        // ===== Monitor(상태/모니터링) =====
        public static void Monitor(string msg) => Write("MON", "I", msg, DailyPath("Monitor"));
        public static void MonitorWarn(string msg) => Write("MON", "W", msg, DailyPath("Monitor"));
        public static void MonitorError(string msg, Exception ex = null)
            => Write("MON", "E",
                ex == null ? msg : $"{msg} :: {ex.GetType().Name} - {ex.Message}",
                DailyPath("Monitor"));

        // ===== Auto(기판 1장 단위) =====
        public static bool IsAutoBoardActive => (_auto.Value != null);

        /// <summary>
        /// Full Auto 시작 시점에 "기판 1장 로그 파일" 생성
        /// 테스트 프로그램 기준:
        /// - UniqueId / Barcode / LotId / PanelId / GlassId 필수 조건 없음
        /// - Meta Interlock 없음
        /// </summary>
        public static void AutoBeginBoard(string boardId, string recipeName, bool dryRun, IEnumerable<string> enabledSeqNames)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(boardId)) boardId = "Board";

                recipeName = (recipeName ?? "").Trim();
                if (string.IsNullOrWhiteSpace(recipeName)) recipeName = "NoRecipe";

                string safeRecipe = MakeSafeFileName(recipeName);
                string safeBoard = MakeSafeFileName(boardId);

                string dayDir = Path.Combine(LogDirectory, "Auto", $"{DateTime.Now:yyyyMMdd}");
                TryCreateDir(dayDir);

                string fileName = $"Auto_{safeBoard}_{safeRecipe}_{DateTime.Now:HHmmss}.log";
                string path = Path.Combine(dayDir, fileName);

                var ctx = new AutoContext
                {
                    BoardId = boardId,
                    RecipeName = recipeName,
                    DryRun = dryRun,
                    StartAt = DateTime.Now,
                    AutoFilePath = path
                };
                _auto.Value = ctx;
                AutoWrite("I", "==================== FULL AUTO BOARD LOG BEGIN ====================");
                AutoWrite("I", $"BoardId     : {ctx.BoardId}");
                AutoWrite("I", $"Recipe      : {ctx.RecipeName}");
                AutoWrite("I", $"DryRun      : {ctx.DryRun}");
                AutoWrite("I", $"StartTime   : {ctx.StartAt:yyyy-MM-dd HH:mm:ss.fff}");
                AutoWrite("I", $"Sequences   : {(enabledSeqNames == null ? "" : string.Join(", ", enabledSeqNames))}");
                AutoWrite("I", "===================================================================");
            }
            catch (Exception ex)
            {
                Error("[AutoBeginBoard] failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Auto Step 시작/종료를 기록하고, 종료 시 tact 분석용 누적
        /// </summary>
        public static IDisposable AutoStepScope(string stepName, string note = null, params (string k, object v)[] fields)
        {
            var ctx = _auto.Value;
            var sw = Stopwatch.StartNew();

            if (ctx != null)
            {
                var f = F(fields);
                AutoWrite("I", $"[STEP-START] {stepName}"
                    + (string.IsNullOrWhiteSpace(note) ? "" : $" | {note}")
                    + (string.IsNullOrWhiteSpace(f) ? "" : $" | {f}"));
            }

            return new Scope(() =>
            {
                sw.Stop();
                if (ctx != null)
                {
                    ctx.Steps.Add(new StepRecord
                    {
                        Name = stepName,
                        Duration = sw.Elapsed,
                        Ok = true,
                        Note = note
                    });
                    AutoWrite("I", $"[STEP-END]   {stepName} | dt={FormatMs(sw.Elapsed)}");
                }
            });
        }
        public static T AutoRpc<T>(string name, int timeoutMs, Func<T> call)
        {
            var ctx = _auto.Value;
            var sw = Stopwatch.StartNew();
            try
            {
                var ret = call();
                sw.Stop();
                if (ctx != null)
                    AutoWrite("I", $"[RPC] {name} | rttMs={(int)sw.Elapsed.TotalMilliseconds} | timeoutMs={timeoutMs} | result=OK");
                return ret;
            }
            catch (Exception ex)
            {
                sw.Stop();
                if (ctx != null)
                    AutoWrite("E", $"[RPC] {name} | rttMs={(int)sw.Elapsed.TotalMilliseconds} | timeoutMs={timeoutMs} | result=EX | {ex.GetType().Name}:{ex.Message}");
                throw;
            }
        }

        public static async System.Threading.Tasks.Task<T> AutoRpcAsync<T>(string name, int timeoutMs, Func<System.Threading.Tasks.Task<T>> call)
        {
            var ctx = _auto.Value;
            var sw = Stopwatch.StartNew();
            try
            {
                var ret = await call().ConfigureAwait(false);
                sw.Stop();
                if (ctx != null)
                    AutoWrite("I", $"[RPC] {name} | rttMs={(int)sw.Elapsed.TotalMilliseconds} | timeoutMs={timeoutMs} | result=OK");
                return ret;
            }
            catch (Exception ex)
            {
                sw.Stop();
                if (ctx != null)
                    AutoWrite("E", $"[RPC] {name} | rttMs={(int)sw.Elapsed.TotalMilliseconds} | timeoutMs={timeoutMs} | result=EX | {ex.GetType().Name}:{ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Step이 예외/NG로 끝났을 때 표시 (AutoStepScope와 함께 쓰면 좋음)
        /// </summary>
        public static void AutoStepFail(string stepName, string reason)
        {
            var ctx = _auto.Value;
            if (ctx == null) return;

            ctx.Steps.Add(new StepRecord
            {
                Name = stepName,
                Duration = TimeSpan.Zero,
                Ok = false,
                Note = reason
            });

            AutoWrite("E", $"[STEP-FAIL]  {stepName} | {reason}");
        }

        /// <summary>
        /// Full Auto 종료 시점에 총 tact + step별 요약 출력
        /// </summary>
        public static void AutoEndBoard(bool ok, string endReason = null)
        {
            var ctx = _auto.Value;
            if (ctx == null) return;

            try
            {
                ctx.TotalSw.Stop();

                AutoWrite("I", "==================== FULL AUTO BOARD LOG END ======================");
                AutoWrite("I", $"Result      : {(ok ? "OK" : "NG")}" + (string.IsNullOrWhiteSpace(endReason) ? "" : $" | {endReason}"));
                AutoWrite("I", $"EndTime     : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                AutoWrite("I", $"TotalTact   : {FormatMs(ctx.TotalSw.Elapsed)}");

                AutoWrite("I", "-------------------- STEP TACT SUMMARY ----------------------------");
                // 같은 스텝명은 합산
                var grouped = ctx.Steps
                    .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                    .GroupBy(s => s.Name)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Total = new TimeSpan(g.Sum(x => x.Duration.Ticks)),
                        Count = g.Count(),
                        Fail = g.Any(x => !x.Ok)
                    })
                    .OrderByDescending(x => x.Total);

                foreach (var g in grouped)
                {
                    AutoWrite(g.Fail ? "W" : "I",
                        $"[TACT] {g.Name} | sum={FormatMs(g.Total)} | count={g.Count}" + (g.Fail ? " | HAS_FAIL" : ""));
                }
                AutoWrite("I", "------------------------------------------------------------------");
                AutoWrite("I", "==================================================================");
            }
            catch (Exception ex)
            {
                Error("[AutoEndBoard] failed", ex);
            }
            finally
            {
                _auto.Value = null;
            }
        }

        public static void AutoInfo(string msg) => AutoWrite("I", msg);
        public static void AutoWarn(string msg) => AutoWrite("W", msg);
        public static void AutoError(string msg) => AutoWrite("E", msg);

        private static void AutoWrite(string level, string msg)
        {
            var ctx = _auto.Value;
            if (ctx == null) return;

            string line = $"{NowLinePrefix(level)} [AUTO] {msg}";
            // Auto는 UI에 뿌려도 되지만, 너무 많으면 UI가 느려질 수 있어 "파일만" 남기고 싶으면 아래 EmitToUiAndDebug(line) 주석 처리
            EmitToUiAndDebug(line);
            WriteLineToFile(ctx.AutoFilePath, line);
        }

        private static string MakeSafeFileName(string s)
        {
            if (string.IsNullOrEmpty(s)) return "NA";
            foreach (var c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            // 너무 길면 잘라냄
            if (s.Length > 60) s = s.Substring(0, 60);
            return s;
        }

        private static string FormatMs(TimeSpan ts)
        {
            // 보기 좋은 tact 표기: mm:ss.fff (1시간 이상이면 hh:mm:ss.fff)
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            return $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }

        private sealed class Scope : IDisposable
        {
            private Action _onDispose;
            public Scope(Action onDispose) { _onDispose = onDispose; }
            public void Dispose()
            {
                var a = Interlocked.Exchange(ref _onDispose, null);
                if (a != null) a();
            }
        }
    }
}