using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using StageWin.WagoIO;

namespace StageWin.Alarm
{
    //  알람 표시용 모델 
    public sealed class AlarmRow : INotifyPropertyChanged
    {
        private DateTime _time = DateTime.Now;
        private int _code;
        private string _level;
        private string _message;

        public DateTime Time
        {
            get => _time;
            set { if (_time != value) { _time = value; OnPropertyChanged(nameof(Time)); } }
        }
        public int Code
        {
            get => _code;
            set { if (_code != value) { _code = value; OnPropertyChanged(nameof(Code)); } }
        }
        public string Level
        {
            get => _level;
            set { if (_level != value) { _level = value; OnPropertyChanged(nameof(Level)); } }
        }
        public string Message
        {
            get => _message;
            set { if (_message != value) { _message = value; OnPropertyChanged(nameof(Message)); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    [DataContract]
    public sealed class AlarmConfig
    {
        [DataMember(Order = 1)]
        public AlarmDefinition[] Alarms { get; set; } = Array.Empty<AlarmDefinition>();

        public static AlarmConfig Load(string path)
        {
            try
            {
                if (!File.Exists(path)) return new AlarmConfig();

                var raw = File.ReadAllText(path, Encoding.UTF8);
                raw = StripJsonComments(raw);
                raw = RemoveTrailingCommas(raw);

                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(raw)))
                {
                    // 먼저 문자열 기반 DTO로 역직렬화 (enum/기본값 문제 회피)
                    var ser = new DataContractJsonSerializer(typeof(AlarmConfigDto));
                    var dto = ser.ReadObject(ms) as AlarmConfigDto;

                    // 실제 모델로 수동 매핑(기본값/enum 변환 포함)
                    return FromDto(dto);
                }
            }
            catch
            {
                return new AlarmConfig();
            }
        }

        static AlarmConfig FromDto(AlarmConfigDto dto)
        {
            var result = new AlarmConfig();
            if (dto?.Alarms == null || dto.Alarms.Length == 0)
            {
                result.Alarms = Array.Empty<AlarmDefinition>();
                return result;
            }

            result.Alarms = dto.Alarms.Select(a => new AlarmDefinition
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                Source = ParseEnum(a.Source, AlarmSource.X),
                Tag = a.Tag,
                Condition = string.IsNullOrWhiteSpace(a.Condition) ? "On" : a.Condition.Trim(),
                SkipWhenAnyFalseTags = a.SkipWhenAnyFalseTags ?? Array.Empty<string>(),
                Level = ParseEnum(a.Level, AlarmLevel.Alarm),
                Latch = a.Latch ?? true,
                ForceManual = a.ForceManual ?? true,
                AllowSemi = a.AllowSemi ?? false,
                AllowAuto = a.AllowAuto ?? false
            }).ToArray();

            return result;
        }

        static T ParseEnum<T>(string s, T @default) where T : struct
        {
            if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse<T>(s, true, out var v))
                return v;
            return @default;
        }

        // 문자열 리터럴("...")은 건드리지 않고,
        // 라인 주석(//...)과 블록 주석(/*...*/)을 제거
        static string StripJsonComments(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            var sb = new StringBuilder(s.Length);
            bool inString = false, escaped = false, inLine = false, inBlock = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                char next = (i + 1 < s.Length) ? s[i + 1] : '\0';

                if (inLine)
                {
                    if (c == '\r' || c == '\n') { inLine = false; sb.Append(c); }
                    continue;
                }
                if (inBlock)
                {
                    if (c == '*' && next == '/') { inBlock = false; i++; }
                    continue;
                }

                if (!inString)
                {
                    if (c == '/' && next == '/') { inLine = true; i++; continue; }
                    if (c == '/' && next == '*') { inBlock = true; i++; continue; }
                }

                sb.Append(c);

                if (c == '"' && !escaped) inString = !inString;
                escaped = (c == '\\') && !escaped;
            }
            return sb.ToString();
        }

        // 문자열 리터럴은 건드리지 않고, ]나 } 바로 앞의
        // "마지막 콤마"만 제거
        static string RemoveTrailingCommas(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            var sb = new StringBuilder(s.Length);
            bool inString = false, escaped = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c == ',' && !inString)
                {
                    int j = i + 1;
                    // 콤마 뒤 공백 스킵
                    while (j < s.Length && char.IsWhiteSpace(s[j])) j++;
                    if (j < s.Length && (s[j] == '}' || s[j] == ']'))
                    {
                        // 트레일링 콤마 → 건너뛰기
                        continue;
                    }
                }

                sb.Append(c);

                if (c == '"' && !escaped) inString = !inString;
                escaped = (c == '\\') && !escaped;
            }
            return sb.ToString();
        }
        public AlarmDefinition Find(string idOrTag)
            => Alarms.FirstOrDefault(a =>
                   string.Equals(a.Id, idOrTag, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(a.Tag) &&
                    string.Equals(a.Tag, idOrTag, StringComparison.OrdinalIgnoreCase)));
    }

    // ===== DTO들: 문자열/nullable 기반 =====
    [DataContract]
    public sealed class AlarmConfigDto
    {
        [DataMember(Order = 1)]
        public AlarmDefinitionDto[] Alarms { get; set; }
    }

    [DataContract]
    public sealed class AlarmDefinitionDto
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public int Code { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public string Source { get; set; }
        [DataMember(Order = 5)] public string Tag { get; set; }
        [DataMember(Order = 6)] public string Condition { get; set; } = "On";
        [DataMember(Order = 7)] public string[] SkipWhenAnyFalseTags { get; set; }
        [DataMember(Order = 8)] public string Level { get; set; }
        [DataMember(Order = 9)] public bool? Latch { get; set; } = true;
        [DataMember(Order = 10)] public bool? ForceManual { get; set; }
        [DataMember(Order = 11)] public bool? AllowSemi { get; set; }
        [DataMember(Order = 12)] public bool? AllowAuto { get; set; }
    }
}
