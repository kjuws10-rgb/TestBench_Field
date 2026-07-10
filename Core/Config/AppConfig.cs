using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Core.Config
{
    [DataContract]
    public sealed class AppConfig
    {
        public static AppConfig Current { get; private set; }

        // 고정 설정 루트 (절대경로)
        public static readonly string ConfigRoot = @"D:\AppConfig";

        [DataMember(Order = 1)]  public string  LocalRecipeDirPath { get; set; }  // 기본: D:\ConfigPath\Recipes
        [DataMember(Order = 2)]  public string  ScanRecipeDirPath  { get; set; }  // 기본: D:\ConfigPath\Recipes.Scan
        [DataMember(Order = 3)]  public int     StagePort          { get; set; }  // 기본: 5000
        [DataMember(Order = 4)]  public string  CoordsDirPath      { get; set; }  // 기본: D:\ConfigPath\Coords
        [DataMember(Order = 5)]  public string  AlarmCsvPath       { get; set; }  // 기본: D:\AppLog\alarm_history.csv
        [DataMember(Order = 6)]  public string  MotionDriver       { get; set; }  // "Sim" | "ACS"
        [DataMember(Order = 7)]  public string  AcsIp              { get; set; } 
        [DataMember(Order = 8)]  public int     AcsPort            { get; set; } = 701;
        [DataMember(Order = 9)]  public bool    AcsUseSimulator    { get; set; } = false;
        [DataMember(Order = 10)] public string  ToolRecipesPath    { get; set; }

        //  고정 IP → Role 매핑 
        [DataContract]
        public sealed class FixedRoleEntry
        {
            [DataMember(Order = 1)] public string Role { get; set; } // "Scan" | "Vision"
            [DataMember(Order = 2)] public string Ip   { get; set; } // "192.168.0.21" / "127.0.0.2"
        }
        [DataMember(Order = 11)] public FixedRoleEntry[] FixedRoleMap            { get; set; } = Array.Empty<FixedRoleEntry>();
        [DataMember(Order = 12)] public string           StageServerIp           { get; set; } // 서버 IP (Loopback=127.0.0.1, 실장비=192.168.x.x)
        [DataMember(Order = 13)] public string           NetMode                 { get; set; } // "Loopback" | "Real"
        [DataMember(Order = 14)] public string           LoopbackScanIp          { get; set; } = "127.0.0.2";
        [DataMember(Order = 15)] public string           LoopbackVisionIp        { get; set; } = "127.0.0.3";
        [DataMember(Order = 16)] public string           ClientBindIpOverride    { get; set; } // 실장비에서 특정 NIC로 바인딩 강제 시 사용(보통 null)
        [DataMember(Order = 17)] public string           PowerRecipesPath        { get; set; } // 단일 JSON 카탈로그 파일
        [DataMember(Order = 18)] public bool             WagoUseSimulator        { get; set; } = false;
        [DataMember(Order = 19)] public string           WagoIp                  { get; set; } = "192.168.1.2";
        [DataMember(Order = 20)] public int              WagoPort                { get; set; } = 502;
        [DataMember(Order = 21)] public string           WagoCfgPath             { get; set; } = Path.Combine(ConfigRoot, "IO.cfg");
        [DataMember(Order = 22)] public string           LdsIp                   { get; set; } = "10.0.0.40";
        [DataMember(Order = 23)] public int              LdsPort                 { get; set; } = 49300;
        [DataMember(Order = 24)] public bool             LdsUseSimulator         { get; set; } = false;
        [DataMember(Order = 25)] public string           LdsQueryCommand         { get; set; } // 폴링형 장비면 필요, 아닐 땐 null
        [DataMember(Order = 26)] public string           LdsNewLine              { get; set; } = "\n"; // 텔넷형 장비라면 "\r\n"일 수도 있음
        [DataMember(Order = 27)] public string           AcsTrigVarX             { get; set; } = "POS_ARRAY_X1"; // X 트리거 좌표 배열
        [DataMember(Order = 28)] public string           AcsTrigVarY             { get; set; } = "POS_ARRAY_Y1"; // Y 트리거 좌표 배열
        [DataMember(Order = 29)] public int              AcsBufferX              { get; set; } = 14;
        [DataMember(Order = 30)] public int              AcsBufferY              { get; set; } = 13;
        [DataMember(Order = 31)] public string           ReviewTrigOutXName      { get; set; } = "Review Camera Trigger - X";
        [DataMember(Order = 32)] public string           ReviewTrigOutYName      { get; set; } = "Review Camera Trigger - Y";
        [DataMember(Order = 33)] public int              ReviewTrigPulseMs       { get; set; } = 100;
        [DataMember(Order = 34)] public double           ScanFlyingReadyOffset   { get; set; } = 200.0;
        [DataMember(Order = 35)] public double           ReviewFlyingReadyOffset { get; set; } = 100.0;
        [DataMember(Order = 36)] public string           ReviewMeasureLogDirPath { get; set; }   // 기본: D:\AppLog\ReviewMeasure


        // 구버전 호환(있으면 받아들이되, 상대경로는 ConfigRoot 기준으로 해석)
        [DataMember(Name = "RecipeDirPath", EmitDefaultValue = false)] private string _compatRecipeDirPath  { get; set; }
        [DataMember(Name = "RecipesDir",    EmitDefaultValue = false)] private string _compatRecipesDir     { get; set; }
        [DataMember(Name = "CoordsDir",     EmitDefaultValue = false)] private string _compatCoordsDir      { get; set; }
        [DataMember(Name = "AlarmCsv",      EmitDefaultValue = false)] private string _compatAlarmCsv       { get; set; }

        public static void Load(string jsonPath = null)
        {
            try { Directory.CreateDirectory(ConfigRoot); } catch { /* 권한 오류 무시 */ }

            string path = string.IsNullOrWhiteSpace(jsonPath)
                        ? Path.Combine(ConfigRoot, "appsettings.json")
                        : jsonPath;

            // 기본값
            var def = new AppConfig
            {
                LocalRecipeDirPath = Path.Combine(@"D:\ConfigPath", "Recipes"),
                ScanRecipeDirPath = Path.Combine(@"D:\ConfigPath", "Recipes.Scan"),
                StagePort = 5000,
                CoordsDirPath = Path.Combine(@"D:\ConfigPath", "Coords"),
                AlarmCsvPath = Path.Combine(@"D:\AppLog", "alarm_history.csv"),
                ToolRecipesPath = Path.Combine(ConfigRoot, "ToolRecipes.json"),
                PowerRecipesPath = Path.Combine(ConfigRoot, "PowerRecipes.json"),
                StageServerIp = "127.0.0.1",
                NetMode = "Loopback",
                LoopbackScanIp = "127.0.0.2",
                LoopbackVisionIp = "127.0.0.3",
                ClientBindIpOverride = null,
                ScanFlyingReadyOffset = 200.0,
                ReviewFlyingReadyOffset = 100.0,
                ReviewMeasureLogDirPath = Path.Combine(@"D:\AppLog", "ReviewMeasure"),
            };

            if (!File.Exists(path))
            {
                Current = def;
                TrySave(path, def);
                EnsureDirs(Current);
                MigrateToolRecipesIfNeeded(Current);
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var ser = new DataContractJsonSerializer(typeof(AppConfig));
                    Current = (AppConfig)ser.ReadObject(fs) ?? def;
                }
            }
            catch (Exception ex)
            {
                // 디버깅 편의
                System.Diagnostics.Debug.WriteLine("[AppConfig.Load] JSON read failed: " + ex.Message);
                Current = def;
            }

            // 구 키 매핑
            string CompatDir(string p)
            {
                if (string.IsNullOrWhiteSpace(p)) return null;
                return Path.IsPathRooted(p) ? p : Path.Combine(ConfigRoot, p);
            }

            if (string.IsNullOrWhiteSpace(Current.LocalRecipeDirPath))
            {
                var c = CompatDir(Current._compatRecipeDirPath) ?? CompatDir(Current._compatRecipesDir);
                Current.LocalRecipeDirPath = c ?? def.LocalRecipeDirPath;
            }
            if (string.IsNullOrWhiteSpace(Current.ScanRecipeDirPath))
                Current.ScanRecipeDirPath = def.ScanRecipeDirPath;

            if (string.IsNullOrWhiteSpace(Current.CoordsDirPath) && !string.IsNullOrWhiteSpace(Current._compatCoordsDir))
                Current.CoordsDirPath = CompatDir(Current._compatCoordsDir);

            if (string.IsNullOrWhiteSpace(Current.AlarmCsvPath) && !string.IsNullOrWhiteSpace(Current._compatAlarmCsv))
                Current.AlarmCsvPath = CompatDir(Current._compatAlarmCsv);

            if (Current.StagePort <= 0 || Current.StagePort > 65535)
                Current.StagePort = def.StagePort;

            if (string.IsNullOrWhiteSpace(Current.ToolRecipesPath))
                Current.ToolRecipesPath = def.ToolRecipesPath;

            EnsureDirs(Current);
            MigrateToolRecipesIfNeeded(Current);
            MigratePowerRecipesIfNeeded(Current);
            NormalizeCriticalFields(Current);
            EnsureDirs(Current);
        }

        public static void Save(string jsonPath = null)
        {
            string path = string.IsNullOrWhiteSpace(jsonPath)
                        ? Path.Combine(ConfigRoot, "appsettings.json")
                        : jsonPath;
            TrySave(path, Current ?? new AppConfig());
        }

        private static void TrySave(string path, AppConfig cfg)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ConfigRoot);
                var ser = new DataContractJsonSerializer(typeof(AppConfig));
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    ser.WriteObject(fs, cfg);
            }
            catch { /* 무시 */ }
        }

        private static void EnsureDirs(AppConfig c)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(c.LocalRecipeDirPath)) Directory.CreateDirectory(c.LocalRecipeDirPath);
                if (!string.IsNullOrWhiteSpace(c.ScanRecipeDirPath)) Directory.CreateDirectory(c.ScanRecipeDirPath);
                if (!string.IsNullOrWhiteSpace(c.CoordsDirPath)) Directory.CreateDirectory(c.CoordsDirPath);
                if (!string.IsNullOrWhiteSpace(c.AlarmCsvPath))
                {
                    var dir = Path.GetDirectoryName(c.AlarmCsvPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                }
                if (!string.IsNullOrWhiteSpace(c.ToolRecipesPath))
                {
                    var dir = Path.GetDirectoryName(c.ToolRecipesPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                }
                if (!string.IsNullOrWhiteSpace(c.PowerRecipesPath))
                {
                    var dir = Path.GetDirectoryName(c.PowerRecipesPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                }
                if (!string.IsNullOrWhiteSpace(c.ReviewMeasureLogDirPath))
                {
                    Directory.CreateDirectory(c.ReviewMeasureLogDirPath);
                }
            }
            catch { /* 무시 */ }
        }

        // 모든 설정 파일을 ConfigRoot에서 얻는 헬퍼(앞으로 추가될 설정에 사용) 
        public static string GetConfigFile(string fileName)
            => Path.Combine(ConfigRoot, fileName);

        // 레거시 ToolRecipes.json 마이그레이션 
        private static void MigrateToolRecipesIfNeeded(AppConfig c)
        {
            try
            {
                var legacy = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ToolRecipes.json");
                if (File.Exists(legacy) && !File.Exists(c.ToolRecipesPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(c.ToolRecipesPath));
                    File.Copy(legacy, c.ToolRecipesPath, overwrite: false);
                }
            }
            catch { /* 무시 */ }
        }
        private static void NormalizeCriticalFields(AppConfig c)
        {
            // 변수명 기본값 보장
            if (string.IsNullOrWhiteSpace(c.AcsTrigVarX)) c.AcsTrigVarX = "POS_ARRAY_X1";
            if (string.IsNullOrWhiteSpace(c.AcsTrigVarY)) c.AcsTrigVarY = "POS_ARRAY_Y1";

            if (string.IsNullOrWhiteSpace(c.ReviewTrigOutXName)) c.ReviewTrigOutXName = "Review Camera Trigger - X";
            if (string.IsNullOrWhiteSpace(c.ReviewTrigOutYName)) c.ReviewTrigOutYName = "Review Camera Trigger - Y";
            if (c.ReviewTrigPulseMs <= 0) c.ReviewTrigPulseMs = 100;

            if (double.IsNaN(c.ScanFlyingReadyOffset) || double.IsInfinity(c.ScanFlyingReadyOffset)) c.ScanFlyingReadyOffset = 200.0;
            if (c.ScanFlyingReadyOffset < 0) c.ScanFlyingReadyOffset = Math.Abs(c.ScanFlyingReadyOffset);
            if (c.ScanFlyingReadyOffset < 1e-3) c.ScanFlyingReadyOffset = 200.0;

            if (double.IsNaN(c.ReviewFlyingReadyOffset) || double.IsInfinity(c.ReviewFlyingReadyOffset)) c.ReviewFlyingReadyOffset = 100.0;
            if (c.ReviewFlyingReadyOffset < 0) c.ReviewFlyingReadyOffset = Math.Abs(c.ReviewFlyingReadyOffset);
            if (c.ReviewFlyingReadyOffset < 1e-3) c.ReviewFlyingReadyOffset = 100.0;
            if (string.IsNullOrWhiteSpace(c.ReviewMeasureLogDirPath)) c.ReviewMeasureLogDirPath = Path.Combine(@"D:\AppLog", "ReviewMeasure");
        }

        private static void MigratePowerRecipesIfNeeded(AppConfig c)
        {
            try
            {
                if (File.Exists(c.PowerRecipesPath)) return; // 이미 단일 파일 있으면 종료

                var oldDir = Path.Combine(
                    string.IsNullOrWhiteSpace(c.LocalRecipeDirPath) ? "Recipes" : c.LocalRecipeDirPath,
                    "PowerRecipes");

                if (!Directory.Exists(oldDir)) return;

                var oldStore = new StageWin.Core.Recipe.RecipeStore(oldDir);
                var names = oldStore.ListNames();

                var newStore = StageWin.Core.Recipe.PowerRecipeStore.Open(c.PowerRecipesPath);
                foreach (var n in names)
                {
                    try
                    {
                        var full = oldStore.Load(n); // RecipeDoc
                        var slim = StageWin.Core.Recipe.PowerOnlyDoc.FromRecipeDoc(full);
                        newStore.Save(n, slim);
                    }
                    catch { /* 개별 실패 무시 */ }
                }
            }
            catch { /* 무시 */ }
        }


    }
}
