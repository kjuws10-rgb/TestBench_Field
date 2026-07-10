using System;
using System.Collections.Generic;
using System.Linq;
using StageWin.WagoIO;

namespace StageWin.Safety
{
    public static class SafetyPolicy
    {
        public const string DOOR_KEYWORD = "Door";
        public const bool LASER_ON_BLOCKS_UNLOCK = true;
        public static Func<bool> WagoSimProvider { get; set; } = () => false;
        private static bool IsWagoSim => WagoSimProvider?.Invoke() == true;
        // "정지" 판정 허용 오차 (mm/s). 현장 상황 따라 0 ~ 0.05 정도 권장
        public const double STOP_SPEED_EPS = 0.05;
        public const bool REQUIRE_STOP_FOR_LOCK = false;

        public const string IN_MODE_KEY_NAME = "Mode Key Status";
        public const string IN_GRIP_SWITCH_ENABLE = "Grip Switch Enable";   // X1017
        public static readonly HashSet<DigitalStatus> ALLOWED_MODE_FOR_UNLOCK =
            new HashSet<DigitalStatus> { DigitalStatus.Teach, DigitalStatus.None };

        public const string OUT_ACS_MC1                 = "ACS Controller 1'st MC On / Off";        // Y1000
        public const string OUT_ACS_MC2_Y               = "ACS Controller 2'nd MC - Stage Y";       // Y1001
        public const string OUT_ACS_MC2_X               = "ACS Controller 2'nd MC - Stage X";       // Y1002
        public const string OUT_ACS_MC_ALLOK            = "ACS MC ALL ON OK";                       // Y100F

        public const string IN_ACS_MC1_MAIN             = "ACS 1'st MC Status - Main";              // X1030
        public const string IN_ACS_MC2_Y                = "ACS 2'nd MC Status - Y AXIS";            // X1031
        public const string IN_ACS_MC2_X                = "ACS 2'nd MC Status - X AXIS";            // X1032

        public const string OUT_AJIN_MC2_T              = "Stage Theta  Motor Drive MC";            // Y1003
        public const string OUT_AJIN_MC2_Z              = "Maint Z (1_2) Motor Drive MC";           // Y1004

        public const string IN_AJIN_MC1_MAIN            = "Rotary Motor 1'st MC Status - Main";     // X1033

        public const string OUT_COOLING_Y1              = "Y1 Linear Motor Air Cooling V/V";         // Y1010
        public const string OUT_COOLING_Y2              = "Y2 Linear Motor Air Cooling V/V";         // Y1011
        public const string OUT_COOLING_X               = "X Linear Motor Air Cooling V/V";          // Y1012
        public const string OUT_COOLING_LASER           = "Laser Box Air Cooling V/V";              // Y1013
        public const string OUT_LASER_INTERLOCK_OK      = "Laser Interlock All OK (WIPS -> K2)";    // Y100B
        public const string OUT_STAGE_MOVING_OK         = "Stage Moving OK Interlock";              // Y100E

        public const string IN_DOOR_FRONT1 = "Clean Booth Front Door 1 Status";
        public const string IN_DOOR_FRONT2 = "Clean Booth Front Door 2 Status";
        public const string IN_DOOR_REAR1 = "Clean Booth Rear Door 1 Status";
        public const string IN_DOOR_REAR2 = "Clean Booth Rear Door 2 Status";
        public const string IN_DOOR_SIDE1 = "Clean Booth Side Door 1 Status";
        public const string IN_DOOR_SIDE2 = "Clean Booth Side Door 2 Status";

        public const string IN_EMS_GRIP_SWITCH      = "Grip Switch EMS";
        public const string IN_EMS_PM_RACK          = "PM Rack EMS Switch";
        public const string IN_EMS_CHAMBER_SWITCH1  = "Chamber EMS Switch Status 1";
        public const string IN_EMS_CHAMBER_SWITCH2  = "Chamber EMS Switch Status 2";
        public const string IN_EMS_CHAMBER_SWITCH3  = "Chamber EMS Switch Status 3";



        public const string IN_LASER_STATUS = "Laser ON / OFF Status";

        public const string IN_MOTOR_TEMP_Y1 = "Liner Motor Temp Alarm - Y1 AXIS";
        public const string IN_MOTOR_TEMP_Y2 = "Liner Motor Temp Alarm - Y2 AXIS";
        public const string IN_MOTOR_TEMP_X = "Liner Motor Temp Alarm - X AXIS";

        public static readonly string[] CLEAN_BOOTH_DOOR_STATUS_INPUTS = new[]
        {
            IN_DOOR_FRONT1, IN_DOOR_FRONT2, IN_DOOR_REAR1, IN_DOOR_REAR2, IN_DOOR_SIDE1, IN_DOOR_SIDE2
        };
        private static bool IsGripEnabled(ISafetyContext ctx)
        {
            var st = ctx.GetInputStatus(IN_GRIP_SWITCH_ENABLE);
            return st == DigitalStatus.Enable || st == DigitalStatus.On;
        }
        
        private static (bool? anyOpen, string whichOpen) GetDoorAggregate(ISafetyContext ctx)
        {
            // 신선도 우선
            foreach (var n in CLEAN_BOOTH_DOOR_STATUS_INPUTS)
                if (!IsFreshInput(n)) return (null, null);

            foreach (var n in CLEAN_BOOTH_DOOR_STATUS_INPUTS)
                if (ctx.GetInput(n)) return (true, n);

            return (false, null);
        }
        private static bool NameEq(string a, string b)
        => string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

        private static readonly TimeSpan FRESH_MAX_AGE = TimeSpan.FromSeconds(15);

        private static bool IsFreshInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (VirtualBus.DigitalInputs.TryGet(name, out var v))
            {
                if (v.UpdatedAt == default) return false;
                return (DateTime.UtcNow - v.UpdatedAt) <= FRESH_MAX_AGE;
            }
            return false;
        }

        public struct EvalResult
        {
            public bool Allowed;
            public bool SuppressPopup;
            public string Reason;

            public static EvalResult Allow() => new EvalResult { Allowed = true };
            public static EvalResult Block(string why, bool suppress = false)
                => new EvalResult { Allowed = false, SuppressPopup = suppress, Reason = why };
        }

        private static EvalResult BlockUncertain(string why)
            => EvalResult.Block(why, suppress: true);

        private static EvalResult CheckDoorOutputEx(ISafetyContext ctx, string outputName, bool desired)
        {
            bool isDoor = !string.IsNullOrWhiteSpace(outputName) &&
                          outputName.IndexOf(DOOR_KEYWORD, StringComparison.OrdinalIgnoreCase) >= 0;
            if (!isDoor) return EvalResult.Allow();

            bool isUnlock = (desired == true);

            // 레이저 OFF 필요
            if (LASER_ON_BLOCKS_UNLOCK && isUnlock)
            {
                // 레이저 상태 입력 신선도 확인 (없으면 묵음 차단)
                if (!IsFreshInput(IN_LASER_STATUS))
                    return BlockUncertain("초기 IO 동기화 중(레이저 상태 미확정)");

                if (ctx.IsLaserOn())
                    return EvalResult.Block("Laser가 ON 상태에서는 도어 Unlock 금지.");
            }

            // 모든 축 정지
            if (isUnlock || REQUIRE_STOP_FOR_LOCK)
            {
                var (stopped, detail) = AreAllAxesStopped(ctx, STOP_SPEED_EPS);
                if (!stopped)
                    return EvalResult.Block($"스테이지 동작중: {detail}. 정지 후 {(isUnlock ? "Unlock" : "Lock")} 가능합니다.", suppress: true);
            }

            // Mode Key (Unlock 시)
            if (isUnlock)
            {
                // 모드키 입력 신선도 체크 (미확정이면 묵음 차단)
                if (!IsFreshInput(IN_MODE_KEY_NAME))
                    return BlockUncertain("초기 IO 동기화 중(Mode Key 미확정)");

                var mode = ResolveModeKey(ctx);
                if (!ALLOWED_MODE_FOR_UNLOCK.Contains(mode))
                    return EvalResult.Block($"Mode Key 상태({mode})에서는 Unlock 금지.");
            }

            // 프로그램 모드
            if (isUnlock)
            {
                var pm = ctx.Mode;
                if (pm != ProgramMode.Manual)
                    return EvalResult.Block($"현재 프로그램 모드({pm})에서는 Unlock 금지. Manual 모드에서만 허용됩니다.");
            }

            return EvalResult.Allow();
        }

        private static EvalResult CheckAcsMcOutputEx(ISafetyContext ctx, SafetyIntent intent)
        {
            string outputName = intent.OutputName;
            bool desired = intent.DesiredState ?? false;
            string src = intent.Source ?? "";

            bool target =
                NameEq(outputName, OUT_ACS_MC1) ||
                NameEq(outputName, OUT_ACS_MC2_Y) ||
                NameEq(outputName, OUT_ACS_MC2_X) ||
                NameEq(outputName, OUT_ACS_MC_ALLOK)||
                NameEq(outputName, OUT_STAGE_MOVING_OK);
            if (!target) return EvalResult.Allow();

            // Manual Only
            var pm = ctx.Mode;
            if (pm != ProgramMode.Manual)
                return EvalResult.Block($"현재 프로그램 모드({pm})에서는 Ajin/ACS MC 제어가 금지됩니다. Manual 모드에서만 허용됩니다.");

            // Y100F: Auto만
            if (NameEq(outputName, OUT_ACS_MC_ALLOK) || NameEq(outputName, OUT_STAGE_MOVING_OK))
            {
                if (string.Equals(src, "Auto", StringComparison.OrdinalIgnoreCase))
                    return EvalResult.Allow();

                return EvalResult.Block("ACS MC ALL ON OK(Y100F)은 자동으로 제어됩니다. 수동으로 변경할 수 없습니다.");
            }

            // 2차측 ON은 1차측(X1030) ON 선행 — 신선도 우선
            if (desired && (NameEq(outputName, OUT_ACS_MC2_Y) || NameEq(outputName, OUT_ACS_MC2_X)))
            {
                if (!IsFreshInput(IN_ACS_MC1_MAIN))
                    return BlockUncertain("초기 IO 동기화 중(ACS 1'st MC 미확정)");

                if (!ctx.GetInput(IN_ACS_MC1_MAIN))
                    return EvalResult.Block("1'st MC Status(X1030)가 OFF 상태입니다. 먼저 1'st MC(Y1000)를 ON 하세요.");
            }

            return EvalResult.Allow();
        }

        private static EvalResult CheckAjinMcOutputEx(ISafetyContext ctx, SafetyIntent intent)
        {
            string outputName = intent.OutputName;
            bool desired = intent.DesiredState ?? false;

            bool target =
                NameEq(outputName, OUT_AJIN_MC2_T) ||
                NameEq(outputName, OUT_AJIN_MC2_Z);
            if (!target) return EvalResult.Allow();

            var pm = ctx.Mode;
            if (pm != ProgramMode.Manual)
                return EvalResult.Block($"현재 프로그램 모드({pm})에서는 Ajin MC 제어가 금지됩니다. Manual 모드에서만 허용됩니다.");

            if (desired)
            {
                // 1차측 입력 신선도/상태
                if (!IsFreshInput(IN_AJIN_MC1_MAIN))
                    return BlockUncertain("초기 IO 동기화 중(Rotary 1'st MC 미확정)");

                if (!ctx.GetInput(IN_AJIN_MC1_MAIN))
                    return EvalResult.Block("Rotary Motor 1'st MC Status - Main 이 OFF 입니다. 먼저 1'st MC를 ON 하세요.");
            }
            return EvalResult.Allow();
        }

        private static EvalResult CheckCoolingVvOutputEx(ISafetyContext ctx, SafetyIntent intent)
        {
            string outputName = intent.OutputName;
            bool target =
                NameEq(outputName, OUT_COOLING_Y1) ||
                NameEq(outputName, OUT_COOLING_Y2) ||
                NameEq(outputName, OUT_COOLING_X) ||
                NameEq(outputName, OUT_COOLING_LASER);

            if (!target) return EvalResult.Allow();

            var pm = ctx.Mode;
            if (pm != ProgramMode.Manual)
                return EvalResult.Block($"현재 프로그램 모드({pm})에서는 Cooling V/V 제어가 금지됩니다. Manual 모드에서만 허용됩니다.");

            return EvalResult.Allow();
        }

        private static EvalResult CheckAutoKeyEx(ISafetyContext ctx)
        {
            // 모드키 입력 신선도 체크(다른 Ex 체계와 동일하게 신선도 우선)
            if (!IsFreshInput(IN_MODE_KEY_NAME))
                return BlockUncertain("초기 IO 동기화 중(Mode Key 미확정)");

            var mode = ResolveModeKey(ctx);
            if (mode != DigitalStatus.Auto)
                return EvalResult.Block("Mode Key가 Auto가 아닙니다.");

            // 도어 상태 신선도/닫힘 여부 확인 (기존 GetDoorAggregate 재활용)
            var (anyOpen, whichOpen) = GetDoorAggregate(ctx);
            if (anyOpen == null)
                return BlockUncertain("초기 IO 동기화 중(도어 상태 미확정)");
            if (anyOpen == true)
                return EvalResult.Block($"[{whichOpen}] 열림 상태 → Interlock OK 설정 금지.");

            // 여기까지 통과면 Auto 전환시 Interlock OK 허용
            return EvalResult.Allow();
        }
        private static EvalResult CheckMotorTempEx(ISafetyContext ctx)
        {
            // 신선도 체크(모두 준비될 때만 판단)
            if (!IsFreshInput(IN_MOTOR_TEMP_Y1) ||
                !IsFreshInput(IN_MOTOR_TEMP_Y2) ||
                !IsFreshInput(IN_MOTOR_TEMP_X))
                return BlockUncertain("초기 IO 동기화 중(Linear Motor Temp 미확정)");

            // ON=정상, OFF=알람
            bool y1Ok = ctx.GetInput(IN_MOTOR_TEMP_Y1);
            bool y2Ok = ctx.GetInput(IN_MOTOR_TEMP_Y2);
            bool xOk = ctx.GetInput(IN_MOTOR_TEMP_X);

            // Y1/Y2 둘 중 하나라도 OFF → Y 축 트립
            if (!y1Ok || !y2Ok)
            {
                string which = (!y1Ok && !y2Ok) ? "Y1/Y2" : (!y1Ok ? "Y1" : "Y2");
                return EvalResult.Block($"Linear Motor Temp Alarm - {which} 감지 → Main Y 정지 후 Servo OFF 수행.");
            }

            // X OFF → X 축 트립
            if (!xOk)
                return EvalResult.Block("Linear Motor Temp Alarm - X 감지 → Review X 정지 후 Servo OFF 수행.");

            return EvalResult.Allow();
        }
        /// <summary>
        /// 리미트 방향 인터락 (Ajin, ACS 무관. 외부에서 리미트 스냅샷을 제공)
        /// dir: +1(+) / -1(-) / 0(무의미)
        /// sense(): (limitMinusOn, limitPlusOn) 를 반환하는 스냅샷 제공자
        /// axisLabel: 메시지용 축 이름("MaintZ", "Theta" 등)
        /// </summary>
        public static EvalResult CheckLimitDirection(
            Func<(bool minusOn, bool plusOn)> sense,
            int dir,
            string axisLabel = "Axis")
        {
            if (sense == null || dir == 0) return EvalResult.Allow();

            var (m, p) = sense();
            if (p && dir > 0) return EvalResult.Block($"{axisLabel}: +Limit 감지 상태 → +방향 구동 금지.");
            if (m && dir < 0) return EvalResult.Block($"{axisLabel}: -Limit 감지 상태 → -방향 구동 금지.");
            if (m && p) return EvalResult.Block($"{axisLabel}: ±Limit 동시 감지(이상) → 구동 금지.");
            return EvalResult.Allow();
        }

        /// <summary>
        /// Jog 버튼 활성/비활성 계산 (Ajin/ACS 무관)
        /// sense(): (limitMinusOn, limitPlusOn)
        /// </summary>
        public static (bool AllowPlus, bool AllowMinus, string Hint) GetJogInterlock(
            Func<(bool minusOn, bool plusOn)> sense)
        {
            if (sense == null) return (true, true, null);
            var (m, p) = sense();
            if (m && p) return (false, false, "±Limit 동시 감지");
            if (p) return (false, true, "+Limit 감지 → -만 허용");
            if (m) return (true, false, "-Limit 감지 → +만 허용");
            return (true, true, null);
        }

        // 평가 리스트
        public static EvalResult EvaluateEx(ISafetyContext ctx, SafetyIntent intent)
        {
            if (IsWagoSim) return EvalResult.Allow();

            switch (intent.Kind)
            {
                case InterlockKind.DoorOutput:
                    return CheckDoorOutputEx(ctx, intent.OutputName, intent.DesiredState ?? false);
                case InterlockKind.ACSMcOutput:
                    return CheckAcsMcOutputEx(ctx, intent);
                case InterlockKind.AjinMcOutput:
                    return CheckAjinMcOutputEx(ctx, intent);
                case InterlockKind.CoolingVv:
                    return CheckCoolingVvOutputEx(ctx, intent);
                case InterlockKind.AutoKey:
                    return CheckAutoKeyEx(ctx);
                case InterlockKind.MotorTemp:
                    return CheckMotorTempEx(ctx);
                default:
                    return EvalResult.Block("지원하지 않는 인터락 종류.");
            }
        }

        // Evaluate는 내부적으로 EvaluateEx를 호출하고, suppress면 팝업 생략
        public static bool Evaluate(ISafetyContext ctx, SafetyIntent intent, out string reason, IAlarmSink sink = null)
        {
            var r = EvaluateEx(ctx, intent);
            reason = r.Reason;

            if (!r.Allowed && sink != null && !r.SuppressPopup)
                sink.Notify("Safety Interlock", reason ?? "인터락 조건 불만족으로 동작 차단.");

            return r.Allowed;
        }
        // XY Servo ON 사전 체크
        public static bool CheckAcsXYServoOnAllowed(ISafetyContext ctx, out string reason)
        {
            if (IsWagoSim)
            {
                reason = null;
                return true;
            }

            reason = null;
            // Y100F가 ON일 때만 XY Servo ON 허용
            if (!ctx.GetOutput(OUT_ACS_MC_ALLOK))
            {
                reason = "ACS MC ALL ON OK(Y100F)가 OFF 입니다. X/Y Servo ON 금지.";
                return false;
            }
            return true;
        }
        public static EvalResult CheckGlobalMotionInterlockForAxis(ISafetyContext ctx)
        {
            if (IsWagoSim) return EvalResult.Allow();

            // 1) 도어 집계 (신선도 우선)
            var (any, which) = GetDoorAggregate(ctx);
            if (any == null)
                return BlockUncertain("초기 IO 동기화 중(도어 상태 미확정)");

            // 2) 문 열림 시: Grip Enable이면 예외 허용, 아니면 차단
            // 예외 허용이어도 3)에서 Y100E를 최종 확인함)
            if (any == true)
            {
                if (!IsGripEnabled(ctx))
                    return EvalResult.Block($"[{which}] 열림 상태 → 축 구동 금지.");
                // else: Grip Enable → 일단 통과, 아래 3) Y100E 체크 계속 진행
            }

            // 3) Stage Moving OK(Y100E) 최종 확인
            //  - OFF면 축 구동 금지 (도어/Grip 조건을 통과했어도 차단)
            //  - 출력 신호는 신선도 개념이 없어 Fail-Safe로 OFF=차단 처리
            if (!ctx.GetOutput(OUT_STAGE_MOVING_OK))
                return EvalResult.Block("Stage Moving OK Interlock(Y100E)가 OFF 상태 → 축 구동 금지.");

            // 4) 모든 조건 통과
            return EvalResult.Allow();
        }
        public static EvalResult CheckLaserOnPreconditions(ISafetyContext ctx)
        {
            if (IsWagoSim) return EvalResult.Allow();

            var (anyOpen, whichOpen) = GetDoorAggregate(ctx);
            if (anyOpen == null) return BlockUncertain("초기 IO 동기화 중(도어 상태 미확정)");
            if (anyOpen == true) return EvalResult.Block($"[{whichOpen}] 열림 상태에서는 Laser ON 금지.");

            var pm = ctx.Mode;
            if (pm == ProgramMode.Manual)
            {
                var grip = ctx.GetInputStatus(IN_GRIP_SWITCH_ENABLE);
                if (grip == DigitalStatus.None || grip == DigitalStatus.Unknown || grip == DigitalStatus.Off)
                    return EvalResult.Block("Grip Switch 비활성/없음 상태에서는 Laser ON 금지.");
            }
            return EvalResult.Allow();
        }
        public static bool ShouldForceAxesStop(ISafetyContext ctx, out string reason)
        {
            if (IsWagoSim)
            {
                reason = null;
                return false;
            }

            reason = null;
            var grip = ctx.GetInputStatus(IN_GRIP_SWITCH_ENABLE);
            //grip = DigitalStatus.Enable;        // Test KJW
            if (grip == DigitalStatus.None) // 정확 요구사항: NONE일 때
            {
                reason = "Grip Switch 상태가 NONE으로 변경됨 → 모든 축 정지.";
                return true;
            }
            return false;
        }
        // === 유틸: 정지 판정(읽기 쉬운 문자열 디테일 포함) ===
        public static (bool stopped, string detail) AreAllAxesStopped(ISafetyContext ctx, double eps)
        {
            double vx = Math.Abs(ctx.GetXActualVelocity());
            double vy = Math.Abs(ctx.GetYActualVelocity());
            double vz = Math.Abs(ctx.GetMaintZActualVelocity());
            double vt = Math.Abs(ctx.GetThetaActualVelocity());

            bool ok = vx <= eps && vy <= eps && vz <= eps && vt <= eps;
            string d = $"X:{vx:F3}, Y:{vy:F3}, Z:{vz:F3}, T:{vt:F3}";
            return (ok, d);
        }
        public static bool ShouldForceLaserOff(ISafetyContext ctx, out string reason)
        {
            if (IsWagoSim)
            {
                reason = null;
                return false;
            }

            reason = null;
            // 1) 도어 상태 우선(미확정이면 Fail-Safe로 OFF)
            var (anyOpen, whichOpen) = GetDoorAggregate(ctx);
            if (anyOpen == null)
            {
                reason = "도어 상태 미확정 → 안전을 위해 Laser OFF.";
                return true;
            }
            if (anyOpen == true)
            {
                reason = $"[{whichOpen}] 열림 상태 → Laser OFF.";
                return true;
            }

            // 2) Manual 모드일 때만 Grip Switch 조건 적용
            if (ctx.Mode == ProgramMode.Manual)
            {
                var grip = ctx.GetInputStatus(IN_GRIP_SWITCH_ENABLE);
                var ok = (grip == DigitalStatus.Enable || grip == DigitalStatus.On);
                if (!ok)
                {
                    reason = "Manual 모드에서 Grip Switch 비활성 → Laser OFF.";
                    return true;
                }
            }

            return false;
        }
        // === 유틸: Mode Key 해석(기존 함수 그대로 사용 가능) ===
        private static DigitalStatus ResolveModeKey(ISafetyContext ctx)
        {
            // 단순화 버전: 본 이름 -> 라벨 파싱 -> RawBit
            var s = ctx.GetInputStatus(IN_MODE_KEY_NAME);
            if (s != DigitalStatus.Unknown) return s;

            var label = ctx.GetInputLabel(IN_MODE_KEY_NAME);
            if (TryParseFromLabel(label, out s)) return s;

            return ctx.GetInput(IN_MODE_KEY_NAME) ? DigitalStatus.Auto : DigitalStatus.Teach;
        }

        private static bool TryParseFromLabel(string label, out DigitalStatus status)
        {
            status = DigitalStatus.Unknown;
            if (string.IsNullOrWhiteSpace(label)) return false;
            var t = label.Trim().ToUpperInvariant();
            if (t.Contains("TEACH") || t.Contains("MANUAL") || t.Contains("HAND")) { status = DigitalStatus.Teach; return true; }
            if (t.Contains("AUTO") || t.Contains("AUTOMATIC")) { status = DigitalStatus.Auto; return true; }
            if (t.Contains("LOCK")) { status = DigitalStatus.Lock; return true; }
            if (t.Contains("UNLOCK")) { status = DigitalStatus.Unlock; return true; }
            if (t == "ON" || t.Contains("ENABLE")) { status = DigitalStatus.On; return true; }
            if (t == "OFF" || t.Contains("DISABLE")) { status = DigitalStatus.Off; return true; }
            if (t == "NA" || t == "N/A" || t == "NONE") { status = DigitalStatus.None; return true; }
            return false;
        }
    }


}
