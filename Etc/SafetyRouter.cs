using System;
using System.Collections.Generic;

namespace StageWin.Safety
{
    public sealed class SafetyRouter
    {
        // 간단한 패턴 매핑(이름에 Door 포함되면 DoorOutput으로)
        private readonly List<Func<string, bool>> _doorMatchers = new List<Func<string, bool>>()
        {
            name => !string.IsNullOrWhiteSpace(name) &&
                    name.IndexOf(SafetyPolicy.DOOR_KEYWORD, StringComparison.OrdinalIgnoreCase) >= 0
        };
        public SafetyIntent ResolveForOutputWrite(string outputName, bool desired, string source = null)
        {
            foreach (var m in _doorMatchers)
                if (m(outputName))
                    return SafetyIntent.ForDoorOutput(outputName, desired, source);

            if (IsAcsMcOutput(outputName))
                return SafetyIntent.ForAcsMcOutput(outputName, desired, source);

            if (IsAjinMcOutput(outputName))
                return SafetyIntent.ForAjinMcOutput(outputName, desired, source);

            if (IsCoolingVvOutput(outputName))
                return SafetyIntent.ForCoolingVv(outputName, desired, source);

            // 필요시 Laser, Light, Vacuum 계속 추가
            // if (outputName.Contains("Laser")) return SafetyIntent.ForLaser(...);

            // 매칭 안 되면 ‘출력 관련 인터락 없음’ 처리 (null)
            return null;
        }
        private static bool IsAcsMcOutput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return
                string.Equals(name, SafetyPolicy.OUT_ACS_MC1, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_ACS_MC2_Y, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_ACS_MC2_X, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_ACS_MC_ALLOK, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_STAGE_MOVING_OK, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAjinMcOutput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return
                string.Equals(name, SafetyPolicy.OUT_AJIN_MC2_T, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_AJIN_MC2_Z, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCoolingVvOutput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return
                string.Equals(name, SafetyPolicy.OUT_COOLING_Y1, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_COOLING_Y2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_COOLING_X, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, SafetyPolicy.OUT_COOLING_LASER, StringComparison.OrdinalIgnoreCase);
        }

        // 폼에서 바로 Intent를 명시 생성하는게 더 가독성 좋으므로
        // XYMove/Servo/MC 계열은 보통 Resolve로 안 돌리고 빌더로 직접 생성하는 걸 권장
    }

    // 내부 로직변수 예시: 프로그램 모드
    public enum ProgramMode { Manual, SemiAuto, Auto }

    // 어떤 종류의 인터락을 태울지 식별
    public enum InterlockKind
    {
        DoorOutput,       // 도어 Lock/Unlock 출력
        XYMove,           // XY 이동
        ServoOn_MaintZ,   // MaintZ 서보 ON
        ServoOn_Theta,    // Theta 서보 ON
        McOn,             // MC On
        McOff,            // MC Off
        ACSMcOutput,      // ACS MC 출력
        AjinMcOutput,     // Ajin MC 출력
        CoolingVv,        // Cooling Valve
        AutoKey,          // Auto전환 시 동작
        MotorTemp,        // Motor Temp알람
        // 필요 시 계속 추가...
    }

    // 호출자가 “무엇을 하려는지”를 전달하는 의도 객체
    public sealed class SafetyIntent
    {
        public InterlockKind Kind { get; }
        public string OutputName { get; }       // DoorOutput 등에서 사용
        public bool? DesiredState { get; }      // DoorOutput 등에서 사용
        public double? PlanSpeedAbs { get; }    // XYMove 등에서 사용
        public string Source { get; }           // 선택: 호출자/폼 이름 등

        private SafetyIntent(InterlockKind k, string outName = null, bool? desired = null,
                             double? planSpeed = null, string source = null)
        {
            Kind = k; OutputName = outName; DesiredState = desired; PlanSpeedAbs = planSpeed; Source = source;
        }

        // 빌더
        public static SafetyIntent ForDoorOutput(string outputName, bool desired, string source = null)
            => new SafetyIntent(InterlockKind.DoorOutput, outName: outputName, desired: desired, source: source);

        public static SafetyIntent ForAcsMcOutput(string outputName, bool desired, string source = null)
            => new SafetyIntent(InterlockKind.ACSMcOutput, outName: outputName, desired: desired, source: source);

        public static SafetyIntent ForAjinMcOutput(string outputName, bool desired, string source = null)
            => new SafetyIntent(InterlockKind.AjinMcOutput, outName: outputName, desired: desired, source: source);

        public static SafetyIntent ForCoolingVv(string outputName, bool desired, string source = null)
            => new SafetyIntent(InterlockKind.CoolingVv, outName: outputName, desired: desired, source: source);

        public static SafetyIntent ForAutoKey(string source = null)
            => new SafetyIntent(InterlockKind.AutoKey, source: source);

        public static SafetyIntent ForMotorTemp(string source = null)
            => new SafetyIntent(InterlockKind.MotorTemp, source: source);

        public static SafetyIntent ForXYMove(double planSpeedAbs, string source = null)
            => new SafetyIntent(InterlockKind.XYMove, planSpeed: planSpeedAbs, source: source);

        public static SafetyIntent ForServoOnMaintZ(string source = null)
            => new SafetyIntent(InterlockKind.ServoOn_MaintZ, source: source);

        public static SafetyIntent ForServoOnTheta(string source = null)
            => new SafetyIntent(InterlockKind.ServoOn_Theta, source: source);

        public static SafetyIntent ForMcOn(string source = null)
            => new SafetyIntent(InterlockKind.McOn, source: source);

        public static SafetyIntent ForMcOff(string source = null)
            => new SafetyIntent(InterlockKind.McOff, source: source);
    }

    // 알람(메시지) 방출 인터페이스 — UI/로그/팝업 등 자유 교체
    public interface IAlarmSink
    {
        void Notify(string title, string message);
    }

    


}
