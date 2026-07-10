using Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace StageWin.Core.Recipe
{
    public enum AutoInspectionMode { OneLine = 0, AllLines = 1 }

    [DataContract]
    public sealed class RecipeHeader
    {
        [DataMember(Order = 1)] public string Name = "NEW_RECIPE";
        [DataMember(Order = 2)] public string SchemaVersion = "1.2.0";
        //[DataMember(Order = 2)] public double AlignMarkX = 0;  // 도면 기준 얼라인 X (mm) // 사용안함
        //[DataMember(Order = 3)] public double AlignMarkY = 0;  // 도면 기준 얼라인 Y (mm) // 사용안함
    }

    [DataContract]
    public sealed class RecipeParameters
    {
        // 설계 그리드 정의
        [DataMember(Order = 1)] public int Lines = 1;            // Y 개수(행)
        [DataMember(Order = 2)] public int HolesPerLine = 1;     // X 개수(열)
        [DataMember(Order = 3)] public double PitchX = 0;        // mm
        [DataMember(Order = 4)] public double PitchY = 0;        // mm
        // 얼라인 기준 첫 홀 위치(도면 좌표)
        [DataMember(Order = 5)] public double FirstHoleX = 0;    // mm
        [DataMember(Order = 6)] public double FirstHoleY = 0;    // mm
        // 벡터 2D index 정의
        [DataMember(Order = 7)] public int VectorRows { get; set; } = 1; // R0..R(N-1)
        [DataMember(Order = 8)] public int VectorCols { get; set; } = 1; // C0..C(M-1)

        public sealed class ScanFlyingParam
        {
            public bool UseFlyCompY { get; set; } = false; // 기본 비활성화(현행 유지)
            public bool Serpentine { get; set; } = true;
            public int StartCol { get; set; } = 0;    // 0 = Auto (FlyDir 기준)
            public int FlyDirY { get; set; } = +1;         // MainY 엔코더 증가 방향 (+1/-1)
            public double FlySpeedY { get; set; } = 0.0;   // mm/s
            public double PerHoleTimeSec { get; set; } = 0.0; // 인접 Col 간 가공 시간 간격(초)
            public bool MajorIsY { get; set; } = true;    // rbtnYProcess=true, rbtnXProcess=false
            public bool Forward { get; set; } = true;    // rbtnForwardProcess=true
            public int FlyDirX { get; set; } = +1;     // X 엔코더 증가 방향(+1/-1)
            public double FlySpeedX { get; set; } = 0.0;  // X 주행속도(mm/s)
            public double PerHoleTimeSecX { get; set; } = 0.0; // 인접 Row 간 평균 처리시간(초)
            public int StartRow { get; set; } = 0;      // X-주행일 때 Row 시작(1 or Lines)
        }

        public sealed class ReviewFlyingParam
        {
            public bool UseFlyCompY { get; set; } = false; // 기본 비활성화(현행 유지)
            public bool Serpentine { get; set; } = true;
            public int StartCol { get; set; } = 0;    // 0 = Auto (FlyDir 기준)
            public int FlyDirY { get; set; } = +1;         // MainY 엔코더 증가 방향 (+1/-1)
            public double FlySpeedY { get; set; } = 0.0;   // mm/s
            public double PerHoleTimeSec { get; set; } = 0.0; // 인접 Col 간 가공 시간 간격(초)
            public bool MajorIsY { get; set; } = true;    // rbtnYProcess=true, rbtnXProcess=false
            public bool Forward { get; set; } = true;    // rbtnForwardProcess=true
            public int FlyDirX { get; set; } = +1;     // X 엔코더 증가 방향(+1/-1)
            public double FlySpeedX { get; set; } = 0.0;  // X 주행속도(mm/s)
            public double PerHoleTimeSecX { get; set; } = 0.0; // 인접 Row 간 평균 처리시간(초)
            public int StartRow { get; set; } = 0;      // X-주행일 때 Row 시작(1 or Lines)
        }
        public ScanFlyingParam ScanFlying { get; set; } = new ScanFlyingParam();
        public ReviewFlyingParam ReviewFlying { get; set; } = new ReviewFlyingParam(); // Review도 플라잉이면 사용
    }

    [DataContract]
    public sealed class Offsets
    {
        [DataMember(Order = 1)] public double ScanToReviewOffsetX;  // 스캐너↔리뷰 ΔX
        [DataMember(Order = 2)] public double ScanToReviewOffsetY;  // 스캐너↔리뷰 ΔY
        [DataMember(Order = 3)] public double ReviewOffsetX;        // 도면→리뷰   ΔX
        [DataMember(Order = 4)] public double ReviewOffsetY;        // 도면→리뷰   ΔY
        //[DataMember(Order = 90)] public double ScannerOffsetX;     // 도면→스캐너 ΔX(사용안함)
        //[DataMember(Order = 91)] public double ScannerOffsetY;     // 도면→스캐너 ΔY(사용안함)
    }

    [DataContract]
    public sealed class Criteria
    {
        // 단순 기준: |ErrX|<=TolX && |ErrY|<=TolY 면 OK
        [DataMember(Order = 1)] public double TolX = 0.010; // mm
        [DataMember(Order = 2)] public double TolY = 0.010; // mm
    }

    [DataContract]
    public sealed class MeasResult
    {
        [DataMember(Order = 1)] public int Row;
        [DataMember(Order = 2)] public int Col;
        [DataMember(Order = 3)] public int VectorRow;
        [DataMember(Order = 4)] public int VectorCol;
        [DataMember(Order = 5)] public double TargetX; // px
        [DataMember(Order = 6)] public double TargetY; // px
        [DataMember(Order = 7)] public double MeasX;   // px (≒ MarkX)
        [DataMember(Order = 8)] public double MeasY;   // px (≒ MarkY)
        [DataMember(Order = 9)] public double ErrX;    // TargetX - MeasX
        [DataMember(Order = 10)] public double ErrY;    // TargetY - MeasY
        [DataMember(Order = 11)] public string Grade;   // "OK" / "NG"
        [DataMember(Order = 12)] public int FindResult;
        // 다음 PROCESS_SCANNING에 실제 적용할 누적 보정량(mm).
        // ErrX/ErrY는 이번 Review 측정 오차로 유지하고, 적용값은 별도로 누적한다.
        [DataMember(Order = 13)] public double AppliedOffsetX;
        [DataMember(Order = 14)] public double AppliedOffsetY;
        [DataMember(Order = 15)] public bool HasAppliedOffset;

        public static bool TryGetAppliedOffset(MeasResult result, out double ox, out double oy)
        {
            ox = 0.0;
            oy = 0.0;
            if (result == null) return false;

            if (result.HasAppliedOffset)
            {
                ox = result.AppliedOffsetX;
                oy = result.AppliedOffsetY;
                return true;
            }

            // Legacy fallback: older recipes used ErrX/ErrY directly as offsets.
            ox = result.ErrX;
            oy = result.ErrY;
            return true;
        }

        public void AccumulateAppliedOffset(MeasResult previous, bool resetAppliedOffsets)
        {
            if (resetAppliedOffsets)
            {
                AppliedOffsetX = 0.0;
                AppliedOffsetY = 0.0;
                HasAppliedOffset = false;
                return;
            }

            double prevX = 0.0, prevY = 0.0;
            bool hasPrevious = TryGetAppliedOffset(previous, out prevX, out prevY);

            if (FindResult == 1)
            {
                if (hasPrevious && IsSameMeasurement(previous, this))
                {
                    AppliedOffsetX = prevX;
                    AppliedOffsetY = prevY;
                    HasAppliedOffset = true;
                    return;
                }

                AppliedOffsetX = prevX + ErrX;
                AppliedOffsetY = prevY + ErrY;
                HasAppliedOffset = true;
            }
            else if (hasPrevious)
            {
                AppliedOffsetX = prevX;
                AppliedOffsetY = prevY;
                HasAppliedOffset = true;
            }
            else
            {
                AppliedOffsetX = 0.0;
                AppliedOffsetY = 0.0;
                HasAppliedOffset = false;
            }
        }

        private static bool IsSameMeasurement(MeasResult previous, MeasResult current)
        {
            if (previous == null || current == null) return false;
            const double eps = 1e-12;
            return previous.FindResult == current.FindResult
                && Math.Abs(previous.TargetX - current.TargetX) <= eps
                && Math.Abs(previous.TargetY - current.TargetY) <= eps
                && Math.Abs(previous.MeasX - current.MeasX) <= eps
                && Math.Abs(previous.MeasY - current.MeasY) <= eps
                && Math.Abs(previous.ErrX - current.ErrX) <= eps
                && Math.Abs(previous.ErrY - current.ErrY) <= eps;
        }
    }

    [DataContract]
    public sealed class ToolingParam
    {
        [DataMember(Order = 1)] public double Power { get; set; }          // dPower
        [DataMember(Order = 2)] public double Freq { get; set; }           // dFreq
        [DataMember(Order = 3)] public double ProcessSpeed { get; set; }   // dProcessSpeed
        [DataMember(Order = 4)] public int ShotCount { get; set; }         // iLaserShotCount
        [DataMember(Order = 5)] public double JumpSpeed { get; set; }      // dJumpSpeed
        [DataMember(Order = 6)] public string ToolName { get; set; }
        [DataMember(Order = 7)] public double AttPos { get; set; }       // dAttPos
        [DataMember(Order = 8)] public string AttSrcName { get; set; }   // User | <파워미터레시피명>
        public ToolingParam Clone() => (ToolingParam)this.MemberwiseClone();
        public static ToolingParam CreateDefault() => new ToolingParam
        {
            Power = 10.0,
            Freq = 20.0,
            ProcessSpeed = 100.0,
            ShotCount = 1,
            JumpSpeed = 50.0,
            ToolName = "Default",
            AttPos = 0.0,
            AttSrcName = "User"
        };
    }

    [DataContract]
    public sealed class PowerRecipeCatalog
    {
        [DataMember(Order = 1)]
        public Dictionary<string, PowerOnlyDoc> Recipes { get; set; }
            = new Dictionary<string, PowerOnlyDoc>(StringComparer.OrdinalIgnoreCase);
    }

    [DataContract]
    public sealed class PowerMeasParam
    {
        // 셀(Row-Col)에서 글로벌을 덮어쓰고 싶을 때만 저장 (기본값이면 저장 안 해도 OK)
        [DataMember(Order = 1)] public double? SetPowerW;   // null = 글로벌/레시피 사용
        [DataMember(Order = 2)] public double? SetAtten;
        [DataMember(Order = 3)] public int? DwellMs;
        [DataMember(Order = 4)] public int? Shots;
        [DataMember(Order = 5)] public int? PreBurn;
        [DataMember(Order = 6)] public int? AvgCount;       // IF#2
    }

    [DataContract]
    public sealed class PowerMeasResult
    {
        [DataMember(Order = 1)] public int Row;
        [DataMember(Order = 2)] public int Col;
        [DataMember(Order = 3)] public double MeasMilliW;   // mW
        [DataMember(Order = 4)] public double DevMilliW;    // mW (표준편차/분산계수 등 가용 시)
        [DataMember(Order = 5)] public double ErrorPercent; // (Meas vs SetPower) %
        [DataMember(Order = 6)] public string Grade;        // "OK" / "NG" / "NONE"
        [DataMember(Order = 7)] public string Time;         // "yyyy-MM-dd HH:mm:ss"
    }

    [DataContract]
    public sealed class PowerTableRow
    {
        [DataMember(Order = 1)] public double TargetW { get; set; }  // 목표 파워(W)
        [DataMember(Order = 2)] public double AttPos { get; set; }  // Attenuator Position
        [DataMember(Order = 3)] public double AttMin { get; set; }  // 검색 Min
        [DataMember(Order = 4)] public double AttMax { get; set; }  // 검색 Max
    }

    [DataContract]
    public sealed class PowerTablePerFreq
    {
        [DataMember(Order = 1)] public double Frequency { get; set; }         // kHz 등 장비 기준
        [DataMember(Order = 2)] public double MinW { get; set; } = 0.5;
        [DataMember(Order = 3)] public double MaxW { get; set; } = 12.0;
        [DataMember(Order = 4)] public double StepW { get; set; } = 0.5;
        [DataMember(Order = 5)] public double DiscardSec { get; set; } = 3.0; // 초기 버림
        [DataMember(Order = 6)] public double MeasSec { get; set; } = 3.0;    // 평균
        [DataMember(Order = 7)] public List<PowerTableRow> Rows { get; set; } = new List<PowerTableRow>();
        [DataMember(Order = 8)] public string UpdatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [DataContract]
    public sealed class PowerCriteria
    {
        [DataMember(Order = 1)] public double TolPercent = 5.0; // 퍼센트 허용오차
        [DataMember(Order = 2)] public double AbsTolW = 0.10; // 절대 허용오차(±W), 검증 등에서 사용
    }

    [DataContract]
    public sealed class PowerOnlyDoc
    {
        [DataMember(Order = 1)]
        public Dictionary<double, PowerTablePerFreq> PowerTables { get; set; }
            = new Dictionary<double, PowerTablePerFreq>();
        [DataMember(Order = 2)] public double PowerMeterStartX { get; set; } = 0.0;
        [DataMember(Order = 3)] public double PowerMeterStartY { get; set; } = 0.0;

        public void SavePowerTable(PowerTablePerFreq table)
        {
            if (table == null) return;
            if (PowerTables == null) PowerTables = new Dictionary<double, PowerTablePerFreq>();
            PowerTables[table.Frequency] = table;
        }

        // _doc(DefaultTooling.Freq) → 후보가 없을 때 가장 가까운 테이블 선택
        public PowerTablePerFreq PickTable(double preferredFreq)
        {
            if (PowerTables == null || PowerTables.Count == 0) return null;
            if (preferredFreq > 0 && PowerTables.TryGetValue(preferredFreq, out var exact)) return exact;
            var bestKey = PowerTables.Keys.OrderBy(k => Math.Abs(k - preferredFreq)).First();
            return PowerTables[bestKey];
        }

        // (마이그레이션용) 기존 RecipeDoc -> PowerOnlyDoc 축약
        public static PowerOnlyDoc FromRecipeDoc(RecipeDoc src)
        {
            return new PowerOnlyDoc
            {
                PowerTables = src?.PowerTables ?? new Dictionary<double, PowerTablePerFreq>(),
                PowerMeterStartX = src?.PowerMeterStartX ?? 0.0,
                PowerMeterStartY = src?.PowerMeterStartY ?? 0.0
            };
        }
    }

    [DataContract]
    public sealed class RecipeDoc
    {
        [DataMember(Order = 1)] public RecipeHeader Header = new RecipeHeader();
        [DataMember(Order = 2)] public RecipeParameters Parameters = new RecipeParameters();
        [DataMember(Order = 3)] public Offsets Offset = new Offsets();
        [DataMember(Order = 4)] public Criteria Crit = new Criteria();
        // 측정 결과(저장용): key="r-c" => 결과
        [DataMember(Order = 5)] public Dictionary<string, MeasResult> Results = new Dictionary<string, MeasResult>();
        [DataMember(Order = 6)] public Dictionary<string, MeasResult> ResultsVec { get; set; }

        // 모든 Hole에 기본적으로 적용할 디폴트 Tooling
        [DataMember(Order = 7)]
        public ToolingParam DefaultTooling { get; set; } = ToolingParam.CreateDefault();
        // 개별 Hole 전용 Tooling (Key: "row-col", 예: "3-5")
        [DataMember(Order = 8)]
        public Dictionary<string, ToolingParam> Toolings { get; set; } = new Dictionary<string, ToolingParam>();
        // 레시피 귀속: 파워 판정 기준
        [DataMember(Order = 9)] public PowerCriteria PowerCrit { get; set; } = new PowerCriteria();
        // per-hole 파라미터 오버라이드 (없으면 글로벌/레시피 값 사용)
        [DataMember(Order = 10)] public Dictionary<string, PowerMeasParam> PowerParams { get; set; } = new Dictionary<string, PowerMeasParam>();
        // per-hole 측정 결과(mW)
        [DataMember(Order = 11)] public Dictionary<string, PowerMeasResult> PowerResults { get; set; } = new Dictionary<string, PowerMeasResult>();

        // 주파수별 Power Table 보관
        [DataMember(Order = 12)] public Dictionary<double, PowerTablePerFreq> PowerTables { get; set; } = new Dictionary<double, PowerTablePerFreq>();
        [DataMember(Order = 13)] public double PowerMeterStartX { get; set; } = 0.0;
        [DataMember(Order = 14)] public double PowerMeterStartY { get; set; } = 0.0;

        // 편의 헬퍼
        public static string KeyOf(int row, int col) => row.ToString() + "-" + col.ToString();

        //  기존 결과 기반(퍼센트 허용)으로 Att 채우기 
        public bool TryResolveAttenuatorForPower(double desiredPowerW, out double atten)
        {
            atten = 0;
            if (PowerResults == null || PowerResults.Count == 0) return false;

            double tolPct = Math.Max(0.0, PowerCrit?.TolPercent ?? 5.0);

            var list = new List<(double measW, double att, string grade, double diffPct)>();

            foreach (var kv in PowerResults)
            {
                var key = kv.Key;
                var res = kv.Value; if (res == null) continue;

                double measW = res.MeasMilliW / 1000.0;

                // 1) per-hole override에 SetAtten 있으면 우선
                if (PowerParams != null && PowerParams.TryGetValue(key, out var ov) && ov?.SetAtten.HasValue == true)
                {
                    double attEff = ov.SetAtten.Value;
                    double diffPct = Math.Abs((measW - desiredPowerW) / Math.Max(1e-12, desiredPowerW)) * 100.0;
                    list.Add((measW, attEff, string.IsNullOrWhiteSpace(res.Grade) ? "NONE" : res.Grade, diffPct));
                    continue;
                }

                // 2) 기본은 레시피 Attenuator
                double? attFromRecipe = 0;
                if (!attFromRecipe.HasValue) continue;

                {
                    double diffPct = Math.Abs((measW - desiredPowerW) / Math.Max(1e-12, desiredPowerW)) * 100.0;
                    list.Add((measW, attFromRecipe.Value, string.IsNullOrWhiteSpace(res.Grade) ? "NONE" : res.Grade, diffPct));
                }
            }
            if (list.Count == 0) return false;
            var okWithin = list
                .Where(x => x.grade.Equals("OK", StringComparison.OrdinalIgnoreCase) && x.diffPct <= tolPct)
                .OrderBy(x => x.diffPct)
                .FirstOrDefault();
            if (okWithin.measW > 0) { atten = okWithin.att; return true; }
            var anyWithin = list.Where(x => x.diffPct <= tolPct).OrderBy(x => x.diffPct).FirstOrDefault();
            if (anyWithin.measW > 0) { atten = anyWithin.att; return true; }
            var nearest = list.OrderBy(x => x.diffPct).First();
            atten = nearest.att;
            return true;
        }
    }
    public sealed class PowerRecipeStore
    {
        readonly string _filePath;
        PowerRecipeCatalog _catalog;

        private PowerRecipeStore(string filePath)
        {
            _filePath = string.IsNullOrWhiteSpace(filePath) ? Path.Combine(AppConfig.ConfigRoot, "PowerRecipes.json") : filePath;
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? AppConfig.ConfigRoot);
            Load();
        }
        public static PowerRecipeStore Open(string filePath) => new PowerRecipeStore(filePath);
        public List<string> ListNames()
            => _catalog.Recipes.Keys.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
        public bool Exists(string name)
            => !string.IsNullOrWhiteSpace(name) && _catalog.Recipes.ContainsKey(name);
        public PowerOnlyDoc Load(string name)
        {
            if (!Exists(name)) throw new FileNotFoundException("power recipe not found", name);
            return _catalog.Recipes[name] ?? new PowerOnlyDoc();
        }
        public PowerOnlyDoc New() => new PowerOnlyDoc();
        // 이름을 키로 넘기도록 변경
        public void Save(string name, PowerOnlyDoc doc)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Recipe name is required.");
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            _catalog.Recipes[name] = doc;
            Flush();
        }
        public void Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (_catalog.Recipes.Remove(name)) Flush();
        }
        public void Flush()
        {
            var set = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            var ser = new DataContractJsonSerializer(typeof(PowerRecipeCatalog), set);
            using (var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                ser.WriteObject(fs, _catalog);
        }
        private void Load()
        {
            if (!File.Exists(_filePath))
            {
                _catalog = new PowerRecipeCatalog();
                Flush(); // 빈 파일 생성
                return;
            }
            try
            {
                var set = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                var ser = new DataContractJsonSerializer(typeof(PowerRecipeCatalog), set);
                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    _catalog = (PowerRecipeCatalog)ser.ReadObject(fs) ?? new PowerRecipeCatalog();
            }
            catch
            {
                try { File.Copy(_filePath, _filePath + ".bak", overwrite: true); } catch { }
                _catalog = new PowerRecipeCatalog();
                Flush();
            }
        }
    }
    public sealed class RecipeStore
    {
        readonly string _dir;
        public string GetFullPath(string name) => PathOf(name);
        public RecipeStore(string directory)
        {
            _dir = string.IsNullOrWhiteSpace(directory) ? "Recipes" : directory;
            try { Directory.CreateDirectory(_dir); } catch { /* 권한/경합 무시 */ }
        }
        string PathOf(string name)
        {
            var baseName = string.IsNullOrWhiteSpace(name) ? "NEW_RECIPE" : name;
            var safe = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars()));
            return System.IO.Path.Combine(_dir, safe + ".json");
        }
        public List<string> ListNames()
        {
            if (!Directory.Exists(_dir)) return new List<string>();
            return Directory
                .GetFiles(_dir, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        public RecipeDoc New(string name = "NEW_RECIPE") => new RecipeDoc { Header = { Name = name } };
        
        //  내부: JSON 정리/파싱 유틸 
        private static string NormalizeJson(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return raw;

            // 선두 BOM(U+FEFF) 및 잘못 저장된 "ï»¿" 제거
            if (raw.Length > 0 && raw[0] == '\uFEFF') raw = raw.Substring(1);
            if (raw.StartsWith("ï»¿")) raw = raw.Substring(3);

            // 양 끝 공백/제어문자 제거
            raw = raw.Trim('\uFEFF', '\u200B', '\u0000', '\u001A', '\r', '\n', '\t', ' ', '\u2028', '\u2029');

            // 꼬리 콤마 교정
            raw = raw.Replace(",}", "}").Replace(",]", "]");
            return raw;
        }

        private static RecipeDoc DeserializeFromString(string json)
        {
            var set = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            var ser = new DataContractJsonSerializer(typeof(RecipeDoc), set);
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var ms = new MemoryStream(bytes, 0, bytes.Length, false))
                return (RecipeDoc)ser.ReadObject(ms);
        }
        private static string RemoveTopLevelObjectProperty(string json, string propName)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(propName)) return json;

            // "Results" 를 찾아 시작점 획득
            string needle = "\"" + propName + "\"";
            int keyPos = json.IndexOf(needle, StringComparison.Ordinal);
            if (keyPos < 0) return json;

            // 콜론 위치로 이동
            int colon = keyPos + needle.Length;
            while (colon < json.Length && char.IsWhiteSpace(json[colon])) colon++;
            if (colon >= json.Length || json[colon] != ':')
            {
                int next = json.IndexOf(needle, keyPos + needle.Length, StringComparison.Ordinal);
                if (next < 0) return json;
                keyPos = next;
                colon = keyPos + needle.Length;
                while (colon < json.Length && char.IsWhiteSpace(json[colon])) colon++;
                if (colon >= json.Length || json[colon] != ':') return json;
            }

            // 값 시작으로 이동
            int valStart = colon + 1;
            while (valStart < json.Length && char.IsWhiteSpace(json[valStart])) valStart++;
            if (valStart >= json.Length || json[valStart] != '{') return json; // 객체가 아니면 스킵

            // 객체 블록의 끝 '}' 찾기(중첩/문자열 대응)
            int pos = valStart;
            int depth = 0;
            bool inStr = false;
            bool esc = false;
            int objEnd = -1;

            while (pos < json.Length)
            {
                char ch = json[pos];

                if (inStr)
                {
                    if (esc) esc = false;
                    else if (ch == '\\') esc = true;
                    else if (ch == '"') inStr = false;
                }
                else
                {
                    if (ch == '"') inStr = true;
                    else if (ch == '{') depth++;
                    else if (ch == '}')
                    {
                        depth--;
                        if (depth == 0) { objEnd = pos; break; }
                    }
                }
                pos++;
            }
            if (objEnd < 0) return json; // 매칭 실패

            // 제거 범위: 속성명부터 객체 끝까지, 그리고 뒤/앞의 콤마 정리
            int removeStart = keyPos;
            int removeEnd = objEnd + 1;

            // 뒤쪽에 콤마가 있으면 같이 제거
            int t = removeEnd;
            while (t < json.Length && char.IsWhiteSpace(json[t])) t++;
            if (t < json.Length && json[t] == ',') removeEnd = t + 1;
            else
            {
                int p = removeStart - 1;
                while (p >= 0 && char.IsWhiteSpace(json[p])) p--;
                if (p >= 0 && json[p] == ',') removeStart = p;
            }

            string result = json.Remove(removeStart, removeEnd - removeStart);
            result = result.Replace(",}", "}").Replace(",]", "]");
            return result;
        }
        private static void Sanitize(RecipeDoc doc)
        {
            if (doc == null) return;

            if (doc.Header == null) doc.Header = new RecipeHeader();
            if (doc.Parameters == null) doc.Parameters = new RecipeParameters();
            if (doc.Offset == null) doc.Offset = new Offsets();
            if (doc.Crit == null) doc.Crit = new Criteria();
            if (doc.Parameters.ScanFlying == null) doc.Parameters.ScanFlying = new RecipeParameters.ScanFlyingParam();
            if (doc.Parameters.ReviewFlying == null) doc.Parameters.ReviewFlying = new RecipeParameters.ReviewFlyingParam();
            // Results(px)
            if (doc.Results == null)
                doc.Results = new Dictionary<string, MeasResult>(StringComparer.OrdinalIgnoreCase);
            else
            {
                var merged = new Dictionary<string, MeasResult>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in doc.Results)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                    merged[kv.Key] = kv.Value ?? new MeasResult();
                }
                doc.Results = merged;
            }

            // Tooling
            if (doc.DefaultTooling == null) doc.DefaultTooling = ToolingParam.CreateDefault();
            if (doc.Toolings == null) doc.Toolings = new Dictionary<string, ToolingParam>(StringComparer.OrdinalIgnoreCase);
            else
            {
                var cleaned = new Dictionary<string, ToolingParam>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in doc.Toolings)
                    if (!string.IsNullOrWhiteSpace(kv.Key) && kv.Value != null)
                    {
                        cleaned[kv.Key] = kv.Value;
                        if (string.IsNullOrWhiteSpace(cleaned[kv.Key].ToolName)) cleaned[kv.Key].ToolName = "User";
                    }
                doc.Toolings = cleaned;
            }

            // Powermeter
            if (doc.PowerCrit == null) doc.PowerCrit = new PowerCriteria();

            if (doc.PowerParams == null) doc.PowerParams = new Dictionary<string, PowerMeasParam>(StringComparer.OrdinalIgnoreCase);
            else
            {
                var cleaned = new Dictionary<string, PowerMeasParam>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in doc.PowerParams)
                    if (!string.IsNullOrWhiteSpace(kv.Key) && kv.Value != null)
                        cleaned[kv.Key] = kv.Value;
                doc.PowerParams = cleaned;
            }

            if (doc.PowerResults == null) doc.PowerResults = new Dictionary<string, PowerMeasResult>(StringComparer.OrdinalIgnoreCase);
            else
            {
                var cleaned = new Dictionary<string, PowerMeasResult>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in doc.PowerResults)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                    var v = kv.Value ?? new PowerMeasResult();
                    if (string.IsNullOrWhiteSpace(v.Grade)) v.Grade = "NONE";
                    if (string.IsNullOrWhiteSpace(v.Time)) v.Time = "";
                    cleaned[kv.Key] = v;
                }
                doc.PowerResults = cleaned;
            }
            // Power Tables
            if (doc.PowerTables == null) doc.PowerTables = new Dictionary<double, PowerTablePerFreq>();

            // 하한 방어
            doc.Parameters.Lines = Math.Max(0, doc.Parameters.Lines);
            doc.Parameters.HolesPerLine = Math.Max(0, doc.Parameters.HolesPerLine);

            if (doc.Parameters.VectorRows <= 0) doc.Parameters.VectorRows = 1;
            if (doc.Parameters.VectorCols <= 0) doc.Parameters.VectorCols = 1;
        }
        public RecipeDoc Load(string name)
        {
            var fn = PathOf(name);
            if (!File.Exists(fn)) throw new FileNotFoundException("recipe not found", fn);

            var raw = File.ReadAllText(fn, Encoding.UTF8);
            var json = NormalizeJson(raw);

            try
            {
                var doc = DeserializeFromString(json);
                Sanitize(doc);
                return doc;
            }
            catch (SerializationException) { }
            catch { }

            string j = RemoveTopLevelObjectProperty(json, "Results");
            try
            {
                var doc2 = DeserializeFromString(j);
                Sanitize(doc2);
                return doc2;
            }
            catch { }

            j = RemoveTopLevelObjectProperty(j, "PowerResults");
            try
            {
                var doc3 = DeserializeFromString(j);
                Sanitize(doc3);
                if (doc3.PowerResults == null) doc3.PowerResults = new Dictionary<string, PowerMeasResult>(StringComparer.OrdinalIgnoreCase);
                return doc3;
            }
            catch { }

            j = RemoveTopLevelObjectProperty(j, "PowerParams");
            var doc4 = DeserializeFromString(j);
            Sanitize(doc4);
            if (doc4.PowerResults == null) doc4.PowerResults = new Dictionary<string, PowerMeasResult>(StringComparer.OrdinalIgnoreCase);
            if (doc4.PowerParams == null) doc4.PowerParams = new Dictionary<string, PowerMeasParam>(StringComparer.OrdinalIgnoreCase);
            return doc4;
        }
        public void Save(RecipeDoc doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            if (doc.Header == null || string.IsNullOrWhiteSpace(doc.Header.Name))
                throw new InvalidOperationException("Recipe name is required.");

            var fn = PathOf(doc.Header.Name);
            var set = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            var ser = new DataContractJsonSerializer(typeof(RecipeDoc), set);

            using (var fs = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ser.WriteObject(fs, doc);
                fs.Flush(true);
            }
        }
        public void Delete(string name)
        {
            var fn = PathOf(name);
            if (File.Exists(fn)) File.Delete(fn);
        }
    }
}
