using System;
using System.Runtime.Serialization;

namespace StageWin.Alarm
{
    public enum AlarmSource
    {
        X,      // 입력 IO
        Y,      // 출력 IO
        USER    // 코드에서 수동 발생
    }

    public enum AlarmLevel
    {
        Unlock,     // 정보성 / 해제 신호 성격
        Warning,
        Alarm,      // 일반 알람
        Fatal       // 치명(미사용이면 생략 가능)
    }

    [DataContract]
    public sealed class AlarmDefinition
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public int Code { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public AlarmSource Source { get; set; }
        [DataMember(Order = 5)] public string Tag { get; set; }
        [DataMember(Order = 6)] public string Condition { get; set; } = "On";
        [DataMember(Order = 7)] public string[] SkipWhenAnyFalseTags { get; set; } = Array.Empty<string>();
        [DataMember(Order = 8)] public AlarmLevel Level { get; set; } = AlarmLevel.Alarm;
        [DataMember(Order = 9)] public bool Latch { get; set; } = true;

        // 모드 정책
        [DataMember(Order = 10)] public bool ForceManual { get; set; } = true; // 기본: Manual 강제
        [DataMember(Order = 11)] public bool AllowSemi { get; set; } = false;  // 예외 허용
        [DataMember(Order = 12)] public bool AllowAuto { get; set; } = false;  // 예외 허용
    }

    public sealed class AlarmRuntimeState
    {
        public bool CurrentActive;
        public bool Latched;
        public bool LastInputState;
        public DateTime FirstTriggered;
        public int TriggerCount;
        public bool HasBaseline;
    }
}
