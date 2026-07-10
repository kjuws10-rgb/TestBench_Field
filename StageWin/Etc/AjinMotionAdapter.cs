using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Config;
using StageWin.WagoIO;

// Ajin SDK 네임스페이스는 프로젝트 참조 추가 시 자동 제공됩니다.
// using AXT;

namespace StageWin.Etc
{
    /// <summary>
    /// X104 모듈의 DI를 AjinAxis용 AxisIoState로 매핑.
    ///   Stage Theta : X104-0040(-L), 0041(Home), 0042(+L)
    ///   Maint Z1    : X104-0043(-L), 0044(Home), 0045(+L)
    ///   Maint Z2    : X104-0046(-L), 0047(Home), 0048(+L) // (현재 축 미정)
    /// </summary>
    public static class WagoIoSense
    {
        public static Func<AjinAxis, AxisIoState> Build(Func<string, bool> readDi)
        {
            if (readDi == null) throw new ArgumentNullException(nameof(readDi));

            // 내부 헬퍼: active-low(리미트), active-high(홈) 정규화
            bool LimitOn(string name) { var raw = readDi(name); return !raw; } // sensed->true
            bool HomeOn(string name) { var raw = readDi(name); return raw; } // sensed->true

            return (ax) =>
            {
                switch (ax)
                {
                    case AjinAxis.T: // Stage Theta
                        return new AxisIoState
                        {
                            LimitMinus = LimitOn("Stage Theta : - Limit"),
                            Home = HomeOn("Stage Theta : Home"),
                            LimitPlus = LimitOn("Stage Theta : + Limit"),
                        };
                    case AjinAxis.MaintZ1: // Maint Z1
                        return new AxisIoState
                        {
                            LimitMinus = LimitOn("Maint Z1 : - Limit"),
                            Home = HomeOn("Maint Z1 : Home"),
                            LimitPlus = LimitOn("Maint Z1 : + Limit"),
                        };
                    default:
                        return default(AxisIoState);
                }
            };
        }

        /// <summary>
        /// VirtualBus 이름 포맷이 설비마다 달라질 수 있어 여러 포맷을 순차 시도.
        /// true = 센서 ON(=Alarm/On), false = OFF
        /// </summary>
        public static bool ReadByAddressFlexible(string node)
        {
            if (VirtualBus.DigitalInputs.TryGet(node, out var v))
                    return v.RawBit;
            return false;
        }
    }
    public struct AxisIoState
    {
        public bool LimitMinus; // -리미트 센서 ON?
        public bool LimitPlus;  // +리미트 센서 ON?
        public bool Home;       // 홈 센서 ON?
    }
    public interface IAjinStatus
    {
        bool IsServoOn(AjinAxis axis);
        bool IsInPosition(AjinAxis axis, double posTolerance = 0.01);
        bool IsAlarm(AjinAxis axis);
        bool IsHomed(AjinAxis axis);
    }

    /// <summary> Ajin 보드 물리축: 0=Z, 1=T </summary>
    public enum AjinAxis { T = 0, MaintZ1 = 1, MaintZ2 = 2 }

    /// <summary> Home 파라미터(검증 코드 패턴 유지) </summary>
    public sealed class AjinHomeParams
    {
        public int Direction { get; set; } = -1;                  // -1 / 0 / 1
        public uint HomeSignal { get; set; } = 5;                 // 라이브러리 enum 인덱스
        public uint UseZPhase { get; set; } = 2;                  // 0/1
        public double ClearTimeMs { get; set; } = 1000;
        public double Offset { get; set; } = 0;
        public double Vel1st { get; set; } = 0.3;
        public double Vel2nd { get; set; } = 0.1;
        public double Vel3rd { get; set; } = 0.1;
        public double VelLast { get; set; } = 1.0;
        public double Acc1st { get; set; } = 1.0;
        public double Acc2nd { get; set; } = 1.0;
    }

    /// <summary> Ajin 모션 어댑터(검증 로직 준수, 단일 클래스) </summary>
    [DataContract]
    internal sealed class AjinHomedStateDoc
    {
        [DataMember] public bool MaintZ1 { get; set; }
        [DataMember] public bool T { get; set; }
    }

    public sealed class AjinMotionAdapter : IDisposable, IAjinStatus
    {
        private const int DefaultAxlOpenMode = 7;
        private bool _opened;

        // 축별 분해능: pulses per engineering unit (mm or deg)
        private readonly Dictionary<AjinAxis, double> _pulsesPerUnit =
            new Dictionary<AjinAxis, double>
            {
                { AjinAxis.MaintZ1, 1000.0 },                 // 0.001 mm/pulse
                { AjinAxis.T,      1.0 / 0.0000528 }         // 18939.393939... pulses/deg
            };
        private readonly Dictionary<AjinAxis, double> _defVel =
            new Dictionary<AjinAxis, double> { { AjinAxis.MaintZ1, 5.0 }, { AjinAxis.T, 5.0 } };
        private readonly Dictionary<AjinAxis, double> _defAcc =
            new Dictionary<AjinAxis, double> { { AjinAxis.MaintZ1, 50.0 }, { AjinAxis.T, 50.0 } };
        private readonly Dictionary<AjinAxis, double> _defDec =
            new Dictionary<AjinAxis, double> { { AjinAxis.MaintZ1, 50.0 }, { AjinAxis.T, 50.0 } };

        // 축별 락(직렬화)
        private readonly SemaphoreSlim[] _axisLocks = { new SemaphoreSlim(1, 1), new SemaphoreSlim(1, 1) };

        // 축별 현재 명령 취소 토큰(Stop/새 명령 시 이전 거 취소)
        private readonly CancellationTokenSource[] _axisMoveCts = new CancellationTokenSource[2];
        private readonly object _ctsLock = new object();

        // 간단 상태 캐시 (검증코드처럼 서보/홈 상태는 내부적으로 관리)
        private readonly Dictionary<AjinAxis, bool> _servoOn = new Dictionary<AjinAxis, bool>
        {
            { AjinAxis.MaintZ1, false }, { AjinAxis.T, false }
        };
        private readonly Dictionary<AjinAxis, bool> _homed = new Dictionary<AjinAxis, bool>
        {
            { AjinAxis.MaintZ1, false }, { AjinAxis.T, false }
        };

        // 동시 명령 보호
        private readonly Dictionary<AjinAxis, SemaphoreSlim> _locks = new Dictionary<AjinAxis, SemaphoreSlim>   
        {
            { AjinAxis.MaintZ1, new SemaphoreSlim(1,1) },
            { AjinAxis.T, new SemaphoreSlim(1,1) }
        };

        public bool IsConnected => _opened;
        
        // 유닛<->펄스 변환 헬퍼
        private double Ppu(AjinAxis ax) => _pulsesPerUnit[ax];
        private double UnitToPulse(AjinAxis ax, double vUnit) => vUnit * Ppu(ax);                 // pos
        private double PulseToUnit(AjinAxis ax, double vPulse) => vPulse / Ppu(ax);
        private double VelUnitToPulse(AjinAxis ax, double velUnit) => velUnit * Ppu(ax);          // unit/s -> pulse/s
        private double AccUnitToPulse(AjinAxis ax, double accUnit) => accUnit * Ppu(ax);
        // (A) 외부 IO 사용 여부(켜면 Home/Limit 모두 Wago 기반 소프트 로직으로 동작)
        public bool UseExternalIoForHomeAndLimits { get; set; } = false;
        // (B) 센서 스냅샷 공급자(외부에서 주입) — Ajin은 Wago를 모름
        public Func<AjinAxis, AxisIoState> IoSenseProvider { get; set; } = null;
        // (C) 리미트/홈 관련 알람(옵션) — UI에서 구독하여 메시지/로그 처리
        public event Action<AjinAxis, string> IoAlarm;
        // (D) 소프트 원점 오프셋(하드웨어 좌표 -> 논리 좌표)
        private readonly Dictionary<AjinAxis, double> _logicalOffset =
            new Dictionary<AjinAxis, double> { { AjinAxis.MaintZ1, 0.0 }, { AjinAxis.T, 0.0 } };
        // 논리좌표 설정(현재 HW값을 읽어 오프셋을 갱신)
        private void SetLogicalPosition(AjinAxis axis, double logicalPos /*unit*/)
        {
            double hwPulse = 0;
            //CAXM.AxmStatusSetActPos((int)axis, hwPulse);
            CAXM.AxmStatusGetActPos((int)axis, ref hwPulse);     // HW 실제값
            double hwUnit = PulseToUnit(axis, hwPulse);
            _logicalOffset[axis] = hwUnit - logicalPos;          // 이후 GetActPos는 hwUnit - offset으로 리턴
        }

        // 현재 논리좌표 = HW - offset
        public double GetActPos(AjinAxis axis)
        {
            if (!IsConnected) return 0.00123;
            double pPulse = 0;
            CAXM.AxmStatusGetActPos((int)axis, ref pPulse);
            return PulseToUnit(axis, pPulse) - _logicalOffset[axis];
        }

        // 내부 헬퍼: IO 스냅샷 읽기(없으면 기본값)
        private AxisIoState Sense(AjinAxis ax) =>
            (IoSenseProvider != null) ? IoSenseProvider(ax) : default;

        // 내부 헬퍼: 방향(+/-)에 따라 현재 리미트위반인지 가드
        private void GuardLimitBeforeMove(AjinAxis axis, int dir /*-1/0/+1*/)
        {
            if (!UseExternalIoForHomeAndLimits || IoSenseProvider == null) return;
            var s = Sense(axis);
            if (dir > 0 && s.LimitPlus)
                throw new Exception($"+Limit ON 상태에서 +방향 이동 금지({axis})");
            if (dir < 0 && s.LimitMinus)
                throw new Exception($"-Limit ON 상태에서 -방향 이동 금지({axis})");
        }
        private void RaiseIoAlarm(AjinAxis axis, string msg)
        {
            try { IoAlarm?.Invoke(axis, msg); } catch { /* ignore */ }
        }
        #region 연결/해제
        private string HomedStatePath => Path.Combine(AppConfig.ConfigRoot, "AjinHomedState.json");

        private void SaveHomedState()
        {
            try
            {
                var doc = new AjinHomedStateDoc
                {
                    MaintZ1 = _homed[AjinAxis.MaintZ1],
                    T = _homed[AjinAxis.T],
                };

                Directory.CreateDirectory(AppConfig.ConfigRoot);
                var ser = new DataContractJsonSerializer(typeof(AjinHomedStateDoc));
                using (var fs = new FileStream(HomedStatePath, FileMode.Create, FileAccess.Write))
                {
                    ser.WriteObject(fs, doc);
                }
            }
            catch { }
        }

        private void LoadHomedStateFromFile()
        {
            try
            {
                if (!File.Exists(HomedStatePath)) return;

                var ser = new DataContractJsonSerializer(typeof(AjinHomedStateDoc));
                using (var fs = new FileStream(HomedStatePath, FileMode.Open, FileAccess.Read))
                {
                    var doc = (AjinHomedStateDoc)ser.ReadObject(fs);
                    if (doc == null) return;

                    _homed[AjinAxis.MaintZ1] = doc.MaintZ1;
                    _homed[AjinAxis.T] = doc.T;
                }
            }
            catch { }
        }

        public void Connect(int axlOpenMode = DefaultAxlOpenMode)
        {
            if (_opened) return;
            uint r = CAXL.AxlOpen(axlOpenMode);
            if (r != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                throw new Exception($"Ajin AxlOpen 실패(ret={r})");
            _opened = true;

            // 현재 보드 상태로 캐시 시드 (서보/홈)
            foreach (AjinAxis ax in new[] { AjinAxis.MaintZ1, AjinAxis.T })
            {
                try { _servoOn[ax] = IsServoOn(ax); } catch { _servoOn[ax] = false; }
                // 홈 여부는 장비마다 다르니 안전하게 false로 시작(성공 Home 시에만 true 세팅)
                SeedHomeCacheFromHw();
            }

            SeedHomeCacheFromHw();
            LoadHomedStateFromFile();
        }
        private void SeedHomeCacheFromHw()
        {
            foreach (AjinAxis ax in new[] { AjinAxis.MaintZ1, AjinAxis.T })
            {
                try
                {
                    uint res = 0;
                    uint rr = CAXM.AxmHomeGetResult((int)ax, ref res);
                    if (rr == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS &&
                        res == (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS)
                        _homed[ax] = true;
                    else
                        _homed[ax] = false;
                }
                catch { _homed[ax] = false; }
            }
        }
        public void Disconnect()
        {
            if (!_opened) return;
            try { CAXL.AxlClose(); }
            finally { _opened = false; }
        }
        public void Dispose()
        {
            foreach (var s in _locks.Values) s.Dispose();
            if (_opened)
            {
                try { CAXL.AxlClose(); } catch { /* ignore */ }
                _opened = false;
            }
        }
        #endregion

        #region Servo / Home
        public void ServoOn(AjinAxis axis)
        {
            EnsureOpened();
            uint r = CAXM.AxmSignalServoOn((int)axis, 1);
            if (r != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                throw new Exception($"AxmSignalServoOn(ON) 실패 Axis={axis} (ret={r})");
            // 하드웨어 반영 후 캐시 갱신
            _servoOn[axis] = IsServoOn(axis);
        }
        public void ServoOff(AjinAxis axis)
        {
            EnsureOpened();
            uint r = CAXM.AxmSignalServoOn((int)axis, 0);
            if (r != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                throw new Exception($"AxmSignalServoOn(OFF) 실패 Axis={axis} (ret={r})");
            _servoOn[axis] = IsServoOn(axis);
            _homed[axis] = false;
            //servo off하면 홈 꺼진것도 저장.
            SaveHomedState();
        }
        public void ZeroSet(AjinAxis axis, double logicalZero = 0.0)
        {
            SetLogicalPosition(axis, logicalZero);
        }
        public async Task HomeAsync(AjinAxis axis, AjinHomeParams p, CancellationToken ct = default)
        {
            if (UseExternalIoForHomeAndLimits && IoSenseProvider != null)
            {
                await HomeAsync_ExternalIo(axis, p, ct);
                return;
            }
        }
        private async Task HomeAsync_ExternalIo(AjinAxis axis, AjinHomeParams p, CancellationToken ct)
        {
            EnsureOpened();
            if (!IsServoOn(axis))
                throw new Exception($"홈 시작 실패: Axis={axis} Servo가 OFF");
            if (p.Direction == 0)
                throw new ArgumentException("Home 방향(p.Direction)은 +1 또는 -1 이어야 합니다.");

            var linked = CreateLinkedAxisToken(axis, ct);

            //Home 재실행하면 Home 상태 끄기
            _homed[axis] = false;

            await WithAxisLock(axis, async () =>
            {
                try { CAXM.AxmMoveEStop((int)axis); } catch { }
                await WaitUntilNotMoving(axis, linked);

                int dirToLimit = Math.Sign(p.Direction);     // +1: +리미트로, -1: -리미트로
                int dirToHome = -dirToLimit;                // 반대방향으로 홈 캡처

                // 0) 시작 시 선택한 방향 리미트가 이미 ON이면, 반대방향으로 살짝 떼어내기
                var s0 = Sense(axis);
                if ((dirToLimit > 0 && s0.LimitPlus) || (dirToLimit < 0 && s0.LimitMinus))
                {
                    double backoff = 2.0; // unit
                    await MoveAbsAsync(axis, GetActPos(axis) + dirToHome * backoff, p.Vel2nd, p.Acc2nd, p.Acc2nd, ct: linked);
                }

                // (1) 리미트 방향으로 속행 → 해당 리미트 센서 On 될 때까지
                await MoveVelUntil(axis,
                    velUnit: 0.1 * dirToLimit, //Math.Max(0.3, p.Vel1st) * dirToLimit,
                    accUnit: Math.Max(0.1, p.Acc1st),
                    decUnit: Math.Max(0.1, p.Acc1st),
                    condition: () => {
                        var s = Sense(axis);
                        return (dirToLimit > 0) ? s.LimitPlus : s.LimitMinus; // On이면 조건 성립
                    },
                    guard: () => {
                        var s = Sense(axis);
                        return (dirToLimit > 0) ? s.LimitMinus : s.LimitPlus; // 반대 리미트가 먼저 켜지면 이상
                    },
                    timeoutMs: 600_000,
                    ct: linked);
                //if (axis == AjinAxis.T)
                //{
                //    // (T축 전용) 락 내부에서는 센서 기반 홈 시퀀스만 수행하고,
                //    // 360deg 절대 위치 이동은 Z축 extra move와 같이 락 밖에서 처리한다.
                //    // (여기서는 추가 동작 없음)
                //}


                // ==============================================================================================================================================================
                // >> button_HomeSearchStart_Click() : "Start Home Search" 버튼 클릭시 호출되는 함수.
                //  - 홈서치를 위한 파라미터의 값을 가져와 홈서치 구동을 실행한다.
                //  - AxmHomeSetMethod : 검색 진행 방향, 원점으로 사용할 신호, 원점 센서 Active Level, 엔코더 Z상 검출 여부, ClearTime, Offset 설정하는 함수
                //  - AxmHomeSetVel    : 원점을 빠르고 정밀하게 검색하기 위해 여러 단계의 스텝으로 검색을 하는데 이때 각 스텝에 사용 될 속도를 설정하는 함수
                //  - AxmHomeSetStart  : 설정한 축의 원점 검색을 시작하는 함수
                //
                // ※ [INFO]
                //  - AxmHomeSetMethod 주의사항 : 원점 레벨을 잘못 설정할 경우 현재의 홈 상태에 따라 -방향으로 설정해도 +방향으로 동작할 수 있으며, 홈을 찾는데 문제가 될 수 있다.
                //  - AxmHomeSetMethod HomeClrTime : 원점 검색 완료 후 지령 위치와 Encoder 위치를 Clear하기 위해 대기하는 시간설정[mSec 단위]
                //  - AxmHomeSetMethod HomeOffset : 원점 검색 완료 후 기구 원점으로 이동 후 원점 재설정할 위치
                //  - 정밀한 홈서치 구동을 위해서 마지막 속도는 작게 설정하는 것을 권장 드립니다.
                // ==============================================================================================================================================================
                //일단 지금은 Z축기준으로만 확인.
                if (axis == AjinAxis.MaintZ1)
                {
                    int HomeSearchDir = 1;
                    uint HomeSearchSignal = p.HomeSignal;
                    uint ZPhaseUse = p.UseZPhase;
                    double HomeClrTime = p.ClearTimeMs;
                    double HomeOffset = p.Offset;
                    CAXM.AxmHomeSetMethod((int)axis, HomeSearchDir, HomeSearchSignal, ZPhaseUse, HomeClrTime, HomeOffset);

                    double VelFirst = 100;
                    double VelSecond = 50;
                    double VelThird = 50;
                    double VelLast = 20;
                    double AccelFirst = 50;
                    double AccelSecond = 50;
                    CAXM.AxmHomeSetVel((int)axis, VelFirst, VelSecond, VelThird, VelLast, AccelFirst, AccelSecond);

                    CAXM.AxmHomeSetStart((int)axis);
                    uint HomeResult = 0;

                    while (true)
                    {
                        CAXM.AxmHomeGetResult((int)axis, ref HomeResult);
                        if (HomeResult == (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS)
                        {
                            break;
                        }

                        // CPU 사용량을 줄이기 위해 잠시 대기
                        await Task.Delay(50);
                    }
                }
            });

            // - Limit 이동 완료 후 T축을 리밋으로 갔던 방향의 반대방향으로 360deg 상대이동
            if (axis == AjinAxis.T)
            {
                int dirToLimit = Math.Sign(p.Direction);   // +1: +리밋으로 갔음, -1: -리밋으로 갔음
                int dirToHome = -dirToLimit;               // 리밋 반대 방향

                double vT = 0.1;
                double aT = 0.1;

                await MoveVelUntil(axis,
                    velUnit: Math.Max(0.05, vT) * dirToHome,
                    accUnit: Math.Max(0.05, aT),
                    decUnit: Math.Max(0.05, aT),
                    condition: () =>
                    {
                        var s = Sense(axis);
                        return s.Home;
                    },
                    guard: () =>
                    {
                        var s = Sense(axis);
                        return (dirToHome > 0) ? s.LimitPlus : s.LimitMinus;
                    },
                    timeoutMs: 600_000,
                    ct: ct);
                //Home 기준 360으로 하자
                //SetLogicalPosition(axis, 0.0);
            }

            // Ajin 홈 완료 후 Z축을 약 +0.197 unit 만큼 추가 상대 이동 -- 여기가 Home Sensor 확인 가능.
            if (axis == AjinAxis.MaintZ1)
            {
                double extra = 1; //0.197;
                if (Math.Abs(extra) > 0)
                {
                    double v = (p.VelLast > 0) ? p.VelLast : Math.Max(0.5, p.Vel2nd);
                    double a = (p.Acc2nd > 0) ? p.Acc2nd : Math.Max(1.0, p.Acc1st);

                    // Lock 때문에 밖으로 빼버림 ;;;
                    await MoveRelAsync(axis, extra, v, a, a, 0.01, ct);

                    var after = Sense(axis);
                }
            }

            System.Threading.Thread.Sleep(1000);
            bool homeOk = true;
            //bool ZHomeOK = true;
            if (UseExternalIoForHomeAndLimits && IoSenseProvider != null)
            {
                var snap = Sense(axis);
                homeOk = snap.Home;
                //ZHomeOK = snap.Home;
                if (!homeOk)
                {
                    RaiseIoAlarm(axis, $"{axis}: Home offset 후에도 Home 센서가 ON 상태가 아닙니다.");
                }
            }

            if (homeOk)
            {
                _homed[axis] = true;
                SaveHomedState();
            }
        }

        #endregion

        #region Move / Jog / Stop
        /// <summary>
        /// 절대이동 + 위치도달 대기(검증 코드 패턴: InMotion 0 && |pos-target|<=tolerance)
        /// </summary>
        public async Task MoveAbsAsync(
            AjinAxis axis, double target /*unit*/, double vel, double acc, double dec,
            double tolerance = 0.01, CancellationToken ct = default)
        {
            var linked = CreateLinkedAxisToken(axis, ct);

            await WithAxisLock(axis, async () =>
            {
                await WaitUntilNotMoving(axis, linked);

                // 논리좌표 기준 방향 계산 & 사전 리미트 가드
                double cur = GetActPos(axis);
                int dir = Math.Sign(target - cur);
                GuardLimitBeforeMove(axis, dir);

                // HW 명령용 목표는 "논리목표 + 오프셋"
                double posPulse = UnitToPulse(axis, target + _logicalOffset[axis]);
                double velPulse = VelUnitToPulse(axis, vel);
                double accPulse = AccUnitToPulse(axis, acc);
                double decPulse = AccUnitToPulse(axis, dec);

                uint r = CAXM.AxmMoveStartPos((int)axis, posPulse, velPulse, accPulse, decPulse);
                if (r != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    throw new Exception($"AxmMoveStartPos 실패 Axis={axis} (ret={r})");

                // 도달 대기: 이동 중 주기적으로 리미트 감시
                await WaitInPositionOrStopped_WithLimit(axis, target, dir, tolerance, linked);
            });
        }
        private async Task WaitInPositionOrStopped_WithLimit(
    AjinAxis axis, double targetUnit, int dir, double tolUnit, CancellationToken ct)
        {
            const int poll = 20;
            int steady = 0, need = 3;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                if (UseExternalIoForHomeAndLimits && IoSenseProvider != null && dir != 0)
                {
                    var s = Sense(axis);
                    if ((dir > 0 && s.LimitPlus) || (dir < 0 && s.LimitMinus))
                    {
                        CAXM.AxmMoveSStop((int)axis);
                        RaiseIoAlarm(axis, $"이동 중 리미트 감지 → 정지 ({axis}, dir={(dir > 0 ? "+" : "-")})");
                        throw new OperationCanceledException("Limit triggered during motion.");
                    }
                }

                bool moving = IsInMotion(axis);
                double pos = GetActPos(axis);
                double err = Math.Abs(pos - targetUnit);
                
                if (err <= tolUnit)
                {
                    if (++steady >= need) return;
                }
                else steady = 0;
                
                if (!moving && err > tolUnit)
                    throw new OperationCanceledException("Motion stopped before reaching target.");

                await Task.Delay(poll, ct).ConfigureAwait(false);
            }
        }
        /// <summary> 현재 위치를 기준으로 상대이동 </summary>
        public async Task MoveRelAsync(
            AjinAxis axis, double delta /*unit*/, double vel, double acc, double dec,
            double tolerance = 0.01, CancellationToken ct = default)
        {
            double cur = GetActPos(axis);
            await MoveAbsAsync(axis, cur + delta, vel, acc, dec, tolerance, ct);
        }
        // 속도 구동 + 조건 만족 시 정지하는 범용 유틸
        private async Task MoveVelUntil(
            AjinAxis axis, double velUnit, double accUnit, double decUnit,
            Func<bool> condition, Func<bool> guard, int timeoutMs, CancellationToken ct)
        {
            // 시작 전 리미트 가드
            GuardLimitBeforeMove(axis, Math.Sign(velUnit));

            double vPulse = VelUnitToPulse(axis, velUnit);
            double aPulse = AccUnitToPulse(axis, accUnit);
            double dPulse = AccUnitToPulse(axis, decUnit);

            CAXM.AxmMoveVel((int)axis, vPulse, aPulse, dPulse);
            var t0 = Environment.TickCount;

            try
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    if (UseExternalIoForHomeAndLimits && IoSenseProvider != null)
                    {
                        if (guard != null && guard())
                        {
                            CAXM.AxmMoveSStop((int)axis);
                            RaiseIoAlarm(axis, $"Home/Limit 시퀀스 가드 조건 위반 → 정지 ({axis})");
                            throw new OperationCanceledException("Guard violated");
                        }
                        if (condition())
                        {
                            CAXM.AxmMoveSStop((int)axis);
                            await WaitUntilNotMoving(axis, ct);
                            return;
                        }
                    }

                    if (timeoutMs >= 0 && Environment.TickCount - t0 > timeoutMs)
                    {
                        CAXM.AxmMoveSStop((int)axis);
                        throw new TimeoutException("MoveVelUntil timeout");
                    }
                    await Task.Delay(5, ct);
                }
            }
            finally
            {
                // 안전상 잔여 이동 멈춤
                try { CAXM.AxmMoveSStop((int)axis); } catch { }
            }
        }
        private async Task WaitInPositionOrStopped(AjinAxis axis, double targetUnit, double tolUnit, CancellationToken ct)
        {
            const int poll = 50;
            int steadyCount = 0;
            const int steadyNeed = 3;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                bool moving = IsInMotion(axis);
                double pos = GetActPos(axis);  // unit
                double err = Math.Abs(pos - targetUnit);

                if (err <= tolUnit)
                {
                    if (++steadyCount >= steadyNeed) return;
                }
                else steadyCount = 0;

                if (!moving && err > tolUnit)
                    throw new OperationCanceledException("Motion stopped before reaching target.");

                await Task.Delay(poll, ct).ConfigureAwait(false);
            }
        }

        private async Task WaitUntilNotMoving(AjinAxis axis, CancellationToken ct)
        {
            const int poll = 20;
            var timeout = Task.Delay(2000, ct);
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                if (!IsInMotion(axis)) return;
                var t = Task.Delay(poll, ct);
                var done = await Task.WhenAny(t, timeout).ConfigureAwait(false);
                if (done == timeout) return;
            }
        }

        private CancellationToken CreateLinkedAxisToken(AjinAxis axis, CancellationToken external)
        {
            var own = CancelAndReplaceAxisCts(axis, cancelOnly: false);
            return CancellationTokenSource.CreateLinkedTokenSource(own.Token, external).Token;
        }

        private CancellationTokenSource CancelAndReplaceAxisCts(AjinAxis axis, bool cancelOnly)
        {
            lock (_ctsLock)
            {
                var old = _axisMoveCts[(int)axis];
                try { old?.Cancel(); } catch { }
                if (cancelOnly) return old ?? new CancellationTokenSource();

                var next = new CancellationTokenSource();
                _axisMoveCts[(int)axis] = next;
                try { old?.Dispose(); } catch { }
                return next;
            }
        }

        /// <summary> 조그 시작 (positive=true면 +방향, false면 -방향) </summary>
        public void JogStart(AjinAxis axis, bool positive, double vel /*unit/s*/, double acc, double dec)
        {
            var snap = Sense(axis);
            if (snap.LimitMinus && snap.LimitPlus)
                throw new Exception($"{axis}: ±Limit 동시 감지(이상) → Jog 금지.");

            var cts = CancelAndReplaceAxisCts(axis, cancelOnly: false);
            int dir = positive ? +1 : -1;
            GuardLimitBeforeMove(axis, dir);

            double vPulse = VelUnitToPulse(axis, (positive ? vel : -vel));
            double aPulse = AccUnitToPulse(axis, acc);
            double dPulse = AccUnitToPulse(axis, dec);
            CAXM.AxmMoveVel((int)axis, vPulse, aPulse, dPulse);

            if (UseExternalIoForHomeAndLimits && IoSenseProvider != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            var s = Sense(axis);
                            if ((dir > 0 && s.LimitPlus) || (dir < 0 && s.LimitMinus))
                            {
                                CAXM.AxmMoveSStop((int)axis);
                                RaiseIoAlarm(axis, $"Jog 중 리미트 감지 → 정지 ({axis}, dir={(dir > 0 ? "+" : "-")})");
                                break;
                            }
                            await Task.Delay(10, cts.Token);
                        }
                    }
                    catch { /* ignore */ }
                }, cts.Token);
            }
        }
        public void Stop(AjinAxis axis)
        {
            // 1) 진행 중인 축 전용 CTS 취소(모든 대기/폴링 즉시 깸)
            CancelAndReplaceAxisCts(axis, cancelOnly: true);

            // 2) 이동은 감속정지
            CAXM.AxmMoveSStop((int)axis);

            // 3) (중요) 홈 중이었다면 홈 상태도 중단
            //    Ajin 보드는 HomeSetStart로 들어간 상태를 별도로 Stop 해줘야 다음 홈이 다시 시작됩니다.
            try { CAXM.AxmMoveEStop((int)axis); } catch { /* 장비/라이브러리별 구현 유무 고려 */ }
        }

        // Position Error Reset Home 진행 후 ,Error량 zeroset.현재 위치 Zeroset
        public void PosErrZeroSet(AjinAxis axis)
        {
            CAXM.AxmStatusSetPosMatch((int)axis, 0.000);
            CAXM.AxmStatusSetCmdPos((int)axis, 0.000);
            CAXM.AxmStatusSetActPos((int)axis, 0.000);
        }
        #endregion

        #region Repeat Motion (검증 코드 로직 충실)
        /// <summary>
        /// 시작↔종료 위치 반복 구동. 각 스텝마다 도달 확인: InMotion!=1 && |ACTPOS - target| <= posTolerance
        /// </summary>
        public async Task RepeatMotionAsync(
            AjinAxis axis, double startPos, double endPos, double speed, double acceleration,
            int dwellTimeMs, int repeatCount, double posTolerance = 1.0, IProgress<(int done, int total)> progress = null,
            CancellationToken ct = default)
        {
            EnsureOpened();
            if (!IsServoOn(axis)) throw new Exception($"반복구동 실패: Axis={axis} Servo가 OFF");
            if (!_homed[axis]) throw new Exception($"반복구동 실패: Axis={axis} Home 미완료");
            if (repeatCount <= 0) throw new ArgumentOutOfRangeException(nameof(repeatCount));
            if (speed <= 0 || acceleration <= 0) throw new ArgumentOutOfRangeException(nameof(speed), "속도/가속도는 0보다 커야 합니다.");

            await WithAxisLock(axis, async () =>
            {
                for (int i = 1; i <= repeatCount; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    // 시작 위치와 다르면 먼저 시작 위치로
                    double cur = GetActPos(axis);
                    if (Math.Abs(cur - startPos) > posTolerance)
                    {
                        uint rr = CAXM.AxmMoveStartPos((int)axis, startPos, speed, acceleration, acceleration);
                        if (rr != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            throw new Exception($"시작위치 이동 실패 Axis={axis} (ret={rr})");
                        await WaitInPosition(axis, startPos, posTolerance, ct);
                        await Task.Delay(dwellTimeMs, ct);
                    }

                    // 종료 위치로
                    {
                        uint rr = CAXM.AxmMoveStartPos((int)axis, endPos, speed, acceleration, acceleration);
                        if (rr != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            throw new Exception($"종료위치 이동 실패 Axis={axis} (ret={rr})");
                        await WaitInPosition(axis, endPos, posTolerance, ct);
                        await Task.Delay(dwellTimeMs, ct);
                    }

                    // 다시 시작 위치로
                    {
                        uint rr = CAXM.AxmMoveStartPos((int)axis, startPos, speed, acceleration, acceleration);
                        if (rr != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            throw new Exception($"복귀 이동 실패 Axis={axis} (ret={rr})");
                        await WaitInPosition(axis, startPos, posTolerance, ct);
                        await Task.Delay(dwellTimeMs, ct);
                    }

                    progress?.Report((i, repeatCount));
                }
            });
        }
        #endregion

        #region 모니터링(검증 코드 스타일)

        /// <summary> 명령 속도(CMDVEL) </summary>
        public double GetCmdVel(AjinAxis axis)
        {
            EnsureOpened();
            double vPulse = 0;
            try
            {
                uint r = CAXM.AxmStatusReadVel((int)axis, ref vPulse);
                if (r != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    throw new Exception($"AxmStatusReadVel 실패 Axis={axis} (ret={r})");
                return PulseToUnit(axis, vPulse);  // unit/s
            }
            catch
            {
                return 0.00;
            }
        }

        /// <summary> 진행중 여부(InMotion==1) </summary>
        public bool IsInMotion(AjinAxis axis)
        {
            uint b = 0;
            CAXM.AxmStatusReadInMotion((int)axis, ref b);
            return b == 1;
        }

        public bool IsServoOn(AjinAxis axis)
        {
            uint on = 0;
            CAXM.AxmSignalIsServoOn((int)axis, ref on);
            return on == 1;
        }
        public bool IsHomed(AjinAxis axis) => _homed.TryGetValue(axis, out var h) && h;
        // IAjinStatus 구현
        bool IAjinStatus.IsHomed(AjinAxis axis) => IsHomed(axis);

        bool IAjinStatus.IsServoOn(AjinAxis axis) => IsServoOn(axis);

        public bool IsInPosition(AjinAxis axis, double posTolerance = 0.01)
        {
            EnsureOpened();
            try
            {
                uint inpos = 0;
                uint r = CAXM.AxmSignalGetInpos((int)axis, ref inpos); // 있으면 사용
                if (r == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return inpos == 1;
            }
            catch { /* ignore */ }

            if (IsInMotion(axis)) return false;
            try
            {
                double cmd = GetCmdVel(axis);
                if (Math.Abs(cmd) > 1e-6) return false;
            }
            catch { }
            return true;
        }

        public bool IsAlarm(AjinAxis axis)
        {
            EnsureOpened();
            try
            {
                uint v = 0;
                uint r = CAXM.AxmSignalReadServoAlarm((int)axis, ref v);
                if (r == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return v == 1;
            }
            catch { }
            return false;
        }
        public bool ResetAlarm(AjinAxis axis, int pulseMs = 80, int settleMs = 150)
        {
            EnsureOpened();
            try
            {
                // 0) 즉시 감속정지(혹시 모를 이동 중 상태 대비)
                try { CAXM.AxmMoveSStop((int)axis); } catch { }

                // 1) 표준 AlarmReset 펄스
                uint r1 = CAXM.AxmSignalServoAlarmReset((int)axis, 1);
                System.Threading.Thread.Sleep(pulseMs);
                uint r2 = CAXM.AxmSignalServoAlarmReset((int)axis, 0);
                System.Threading.Thread.Sleep(settleMs);

                // 빠르게 해제되는 경우 바로 OK
                if (!IsAlarm(axis)) return true;

                // 2) 보조 루틴: Servo Off → AlarmReset → Servo On
                //    (일부 드라이버는 Off 상태에서 Reset이 안정적)
                CAXM.AxmSignalServoOn((int)axis, 0);
                System.Threading.Thread.Sleep(50);
                CAXM.AxmSignalServoAlarmReset((int)axis, 1);
                System.Threading.Thread.Sleep(pulseMs);
                CAXM.AxmSignalServoAlarmReset((int)axis, 0);
                System.Threading.Thread.Sleep(settleMs);
                CAXM.AxmSignalServoOn((int)axis, 1);
                System.Threading.Thread.Sleep(50);

                return !IsAlarm(axis);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 내부 유틸
        private void EnsureOpened()
        {
            try { if (!_opened) throw new Exception("Ajin 보드가 연결되지 않았습니다. Connect()를 먼저 호출하세요."); }
            catch { }
        }

        private async Task WithAxisLock(AjinAxis axis, Func<Task> body)
        {
            var sem = _axisLocks[(int)axis];
            var waitToken = GetAxisWaitToken(axis);
            await sem.WaitAsync(waitToken).ConfigureAwait(false);
            try { await body().ConfigureAwait(false); }
            finally { try { sem.Release(); } catch (SemaphoreFullException) { } }
        }

        private CancellationToken GetAxisWaitToken(AjinAxis axis)
        {
            lock (_ctsLock)
            {
                var t = _axisMoveCts[(int)axis]?.Token ?? CancellationToken.None;
                if (t.CanBeCanceled && t.IsCancellationRequested) return CancellationToken.None;
                return t;
            }
        }

        /// <summary> 검증 코드 패턴: InMotion 비활성 & 위치 오차 기준으로 도달 판정 </summary>
        private async Task WaitInPosition(AjinAxis axis, double target, double tolerance, CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                uint moving = 0;
                CAXM.AxmStatusReadInMotion((int)axis, ref moving);

                double pos = 0;
                CAXM.AxmStatusGetActPos((int)axis, ref pos);

                if (moving != 1 && Math.Abs(pos - target) <= tolerance)
                    break;

                await Task.Delay(50, ct);
            }
        }
        #endregion

    }
}
