using System;
using System.Globalization;
using System.IO;
using System.Text;
using Core.Config;

namespace Core.Logging
{
    /// <summary>
    /// Review 측정 결과 전용 Text Logger
    ///
    /// 저장 위치:
    /// D:\AppLog\ReviewMeasure\yyyyMMdd\ReviewMeasure_yyyyMMdd.txt
    ///
    /// 목적:
    /// - Manual / SemiAuto / Auto 어떤 모드에서 측정했는지 기록
    /// - Single / Step / Flying / ManualInspection 어떤 방식으로 측정했는지 기록
    /// - 어떤 셀(Row, Col, VecR, VecC)을 측정했고 결과가 무엇인지 기록
    /// - Recipe 저장 결과와 별도로 순수 측정 이력 보관
    /// </summary>
    public static class ReviewMeasureTextLogger
    {
        private static readonly object _sync = new object();

        public sealed class ReviewMeasureRecord
        {
            public DateTime Time { get; set; }

            public string SessionId { get; set; }
            public string ProgramMode { get; set; }
            public string RecipeName { get; set; }
            public string MethodName { get; set; }
            public string OpTag { get; set; }

            public int Row { get; set; }
            public int Col { get; set; }
            public int VectorRow { get; set; }
            public int VectorCol { get; set; }

            public int GlobalR { get; set; }
            public int GlobalC { get; set; }

            public double StageX { get; set; }
            public double StageY { get; set; }

            public double TargetX { get; set; }
            public double TargetY { get; set; }
            public double MarkX { get; set; }
            public double MarkY { get; set; }

            public double ErrXpx { get; set; }
            public double ErrYpx { get; set; }
            public double ErrXmm { get; set; }
            public double ErrYmm { get; set; }

            public double TolXpx { get; set; }
            public double TolYpx { get; set; }

            public int FindResult { get; set; }
            public string Grade { get; set; }
            public string Note { get; set; }
        }

        public static string MakeSessionId(string opTag)
        {
            string tag = MakeSafeText(opTag);
            if (string.IsNullOrWhiteSpace(tag)) tag = "REVIEW";

            return DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture) + "_" + tag;
        }

        public static void WriteSessionState(
            string sessionId,
            string programMode,
            string recipeName,
            string methodName,
            string opTag,
            string state,
            string note)
        {
            Write(new ReviewMeasureRecord
            {
                Time = DateTime.Now,
                SessionId = sessionId,
                ProgramMode = programMode,
                RecipeName = recipeName,
                MethodName = methodName,
                OpTag = opTag,

                Row = -1,
                Col = -1,
                VectorRow = -1,
                VectorCol = -1,
                GlobalR = -1,
                GlobalC = -1,

                StageX = 0.0,
                StageY = 0.0,
                TargetX = 0.0,
                TargetY = 0.0,
                MarkX = 0.0,
                MarkY = 0.0,
                ErrXpx = 0.0,
                ErrYpx = 0.0,
                ErrXmm = 0.0,
                ErrYmm = 0.0,
                TolXpx = 0.0,
                TolYpx = 0.0,

                FindResult = -1,
                Grade = state,
                Note = note
            });
        }

        public static void Write(ReviewMeasureRecord r)
        {
            if (r == null) return;

            try
            {
                if (r.Time == DateTime.MinValue)
                    r.Time = DateTime.Now;

                string path = GetDailyFilePath();

                lock (_sync)
                {
                    bool needHeader = !File.Exists(path) || new FileInfo(path).Length <= 0;

                    using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        if (needHeader)
                            sw.WriteLine(MakeHeaderLine());

                        sw.WriteLine(MakeDataLine(r));
                    }
                }
            }
            catch
            {
                // 측정 로그 저장 실패가 설비 동작을 막으면 안 되므로 무시
            }
        }

        private static string GetDailyFilePath()
        {
            string root = null;

            try
            {
                if (AppConfig.Current != null)
                    root = AppConfig.Current.ReviewMeasureLogDirPath;
            }
            catch
            {
                root = null;
            }

            if (string.IsNullOrWhiteSpace(root))
                root = Path.Combine(@"D:\AppLog", "ReviewMeasure");

            string day = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string dir = Path.Combine(root, day);

            Directory.CreateDirectory(dir);

            return Path.Combine(dir, "ReviewMeasure_" + day + ".txt");
        }

        private static string MakeHeaderLine()
        {
            return string.Join("\t", new[]
            {
                "Time",
                "SessionId",
                "Mode",
                "Recipe",
                "Method",
                "OpTag",
                "Row",
                "Col",
                "VecR",
                "VecC",
                "GlobalR",
                "GlobalC",
                "StageX_mm",
                "StageY_mm",
                "TargetX_px",
                "TargetY_px",
                "MarkX_px",
                "MarkY_px",
                "ErrX_px",
                "ErrY_px",
                "ErrX_mm",
                "ErrY_mm",
                "TolX_px",
                "TolY_px",
                "FindResult",
                "Grade",
                "Note"
            });
        }

        private static string MakeDataLine(ReviewMeasureRecord r)
        {
            return string.Join("\t", new[]
            {
                S(r.Time.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                S(r.SessionId),
                S(r.ProgramMode),
                S(r.RecipeName),
                S(r.MethodName),
                S(r.OpTag),

                I(r.Row),
                I(r.Col),
                I(r.VectorRow),
                I(r.VectorCol),
                I(r.GlobalR),
                I(r.GlobalC),

                D(r.StageX),
                D(r.StageY),
                D(r.TargetX),
                D(r.TargetY),
                D(r.MarkX),
                D(r.MarkY),
                D(r.ErrXpx),
                D(r.ErrYpx),
                D(r.ErrXmm),
                D(r.ErrYmm),
                D(r.TolXpx),
                D(r.TolYpx),

                I(r.FindResult),
                S(r.Grade),
                S(r.Note)
            });
        }

        private static string S(string s)
        {
            if (s == null) return "";

            return s
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Trim();
        }

        private static string I(int v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }

        private static string D(double v)
        {
            if (double.IsNaN(v)) return "NaN";
            if (double.IsInfinity(v)) return v > 0 ? "Infinity" : "-Infinity";

            return v.ToString("0.######", CultureInfo.InvariantCulture);
        }

        private static string MakeSafeText(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";

            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');

            s = s.Replace(" ", "_")
                 .Replace("\t", "_")
                 .Replace("\r", "_")
                 .Replace("\n", "_");

            if (s.Length > 40)
                s = s.Substring(0, 40);

            return s;
        }
    }
}