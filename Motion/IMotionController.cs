using System;
using System.Threading.Tasks;
using StageWin.WagoIO;
using StageWin.Safety;
using ACS.SPiiPlusNET;

namespace StageWin.Driver.Motion
{
    // 사용 축(폼/시뮬/ACS 어댑터에서 X/Y만 씀)
    public enum Axis
    {
        Y = 0,
        X = 4, // 기존 값 유지
    }

    public struct MotionProfile
    {
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public double Deceleration { get; set; }

        public MotionProfile(double velocity, double acceleration, double deceleration)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Deceleration = deceleration;
        }
    }

    // 스테이지 공통 인터페이스
    public interface IMotionController : IDisposable
    {
        bool IsConnected { get; }
        void Connect();
        void Disconnect();

        Task ServoOnAsync(Axis axis);
        Task ServoOffAsync(Axis axis);

        Task HomeAsync(Axis axis);
        Task StopAsync(Axis axis);

        Task MoveAbsAsync(Axis axis, double position, double? vel = null, double? acc = null, double? dec = null);
        Task MoveRelAsync(Axis axis, double delta, double? vel = null, double? acc = null, double? dec = null);

        Task JogStartAsync(Axis axis, bool positive, double velocity);
        Task JogStopAsync(Axis axis);
        
        double GetPosition(Axis axis);
        double GetTargetPosition(Axis axis);
        double GetAPosition(Axis axis);

        double GetAcceleration(Axis axis);
        double GetVelocity(Axis axis);

        bool IsBusy(Axis axis);

        MotionProfile GetProfile(Axis axis);

        // 기존 시그니처 유지 (호출부 영향 최소화)
        void SetProfile(Axis axis, double vel, double acc, double dec);
    }

    // ACS에서만 제공되는 추가 상태 조회(폼에서 캐스팅해 사용)
    public interface IAcsStatus
    {
        bool IsServoOn(Axis axis);
        bool IsHomeDone(Axis axis);
        bool IsInPosition(Axis axis);
        double GetRms(Axis axis);
    }
    public interface IAcsVariables
    {
        // SPiiPlus .NET의 WriteVariable 시그니처 그대로 노출
        void WriteVariable(object value, string variable, ProgramBuffer nBuf,
                            int from1 = Api.ACSC_NONE, int to1 = Api.ACSC_NONE,
                            int from2 = Api.ACSC_NONE, int to2 = Api.ACSC_NONE);
        Task WriteVariableAsync(object value, string variable, ProgramBuffer nBuf,
                                int from1 = Api.ACSC_NONE, int to1 = Api.ACSC_NONE,
                                int from2 = Api.ACSC_NONE, int to2 = Api.ACSC_NONE);
    }

    public interface IAcsPrograms
    {
        void RunBuffer(int bufferNo);
        Task RunBufferAsync(int bufferNo);
        void StopBuffer(int bufferNo);
        Task StopBufferAsync(int bufferNo);
    }
    /// 모든 ServoOn(X/Y)에 SafetyPolicy를 강제 적용하는 래퍼
    public sealed class SafeMotionController : IMotionController, IAcsStatus, IAcsVariables, IAcsPrograms
    {
        private readonly IMotionController _inner;
        private readonly IAlarmSink _alarm;
        private readonly Func<ISafetyContext> _ctxProvider; // 선택 (없으면 라이트 컨텍스트 사용)

        public SafeMotionController(IMotionController inner,
                                    IAlarmSink alarm = null,
                                    Func<ISafetyContext> ctxProvider = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _alarm = alarm;
            _ctxProvider = ctxProvider;
        }
        // ACS 변수 쓰기(동기)
        void IAcsVariables.WriteVariable(object value, string variable, ProgramBuffer nBuf,
                                         int from1, int to1, int from2, int to2)
        {
            if (_inner is IAcsVariables v)
                v.WriteVariable(value, variable, nBuf, from1, to1, from2, to2);
            else
                throw new NotSupportedException("Underlying motion adapter does not support ACS variable access.");
        }

        // ACS 변수 쓰기(비동기)
        Task IAcsVariables.WriteVariableAsync(object value, string variable, ProgramBuffer nBuf,
                                              int from1, int to1, int from2, int to2)
        {
            if (_inner is IAcsVariables v)
                return v.WriteVariableAsync(value, variable, nBuf, from1, to1, from2, to2);

            throw new NotSupportedException("Underlying motion adapter does not support ACS variable access.");
        }

        // ACS 버퍼 실행(동기)
        void IAcsPrograms.RunBuffer(int bufferNo)
        {
            if (_inner is IAcsPrograms p)
                p.RunBuffer(bufferNo);
            else
                throw new NotSupportedException("Underlying motion adapter does not support ACS program buffers.");
        }

        // ACS 버퍼 실행(비동기)
        Task IAcsPrograms.RunBufferAsync(int bufferNo)
        {
            if (_inner is IAcsPrograms p)
                return p.RunBufferAsync(bufferNo);

            throw new NotSupportedException("Underlying motion adapter does not support ACS program buffers.");
        }
        void IAcsPrograms.StopBuffer(int bufferNo)
        {
            if (_inner is IAcsPrograms p)
            {
                // 어댑터가 명시 구현한 경우 바로 위임
                p.StopBuffer(bufferNo);
                return;
            }

            // 어댑터에 public StopBuffer(int) 메서드가 존재하면 reflection으로 호출
            var mSync = _inner.GetType().GetMethod("StopBuffer", new[] { typeof(int) });
            if (mSync != null)
            {
                mSync.Invoke(_inner, new object[] { bufferNo });
                return;
            }

            // 동기가 없고 비동기만 있으면 비동기 호출 후 완료 대기
            var mAsync = _inner.GetType().GetMethod("StopBufferAsync", new[] { typeof(int) });
            if (mAsync != null)
            {
                var task = (Task)mAsync.Invoke(_inner, new object[] { bufferNo });
                task.GetAwaiter().GetResult();
                return;
            }

            throw new NotSupportedException("Underlying motion adapter does not support StopBuffer/StopBufferAsync.");
        }

        // ACS 버퍼 정지(비동기)
        async Task IAcsPrograms.StopBufferAsync(int bufferNo)
        {
            if (_inner is IAcsPrograms p)
            {
                // 어댑터가 명시 구현한 경우 바로 위임
                await p.StopBufferAsync(bufferNo).ConfigureAwait(false);
                return;
            }

            // 어댑터에 public StopBufferAsync(int) 메서드가 존재하면 reflection으로 호출
            var mAsync = _inner.GetType().GetMethod("StopBufferAsync", new[] { typeof(int) });
            if (mAsync != null)
            {
                var task = (Task)mAsync.Invoke(_inner, new object[] { bufferNo });
                await task.ConfigureAwait(false);
                return;
            }

            // 비동기가 없고 동기만 있으면 동기 호출
            var mSync = _inner.GetType().GetMethod("StopBuffer", new[] { typeof(int) });
            if (mSync != null)
            {
                mSync.Invoke(_inner, new object[] { bufferNo });
                return;
            }

            throw new NotSupportedException("Underlying motion adapter does not support StopBuffer/StopBufferAsync.");
        }
        // ---- IMotionController 위임/차단 ----
        public bool IsConnected => _inner.IsConnected;
        public void Connect() => _inner.Connect();
        public void Disconnect() => _inner.Disconnect();
        public void Dispose() => _inner.Dispose();

        public async Task ServoOnAsync(Axis axis)
        {
            if (axis == Axis.X || axis == Axis.Y)
            {
                var ctx = _ctxProvider?.Invoke() ?? ServoLiteContext.Shared;
                if (!SafetyPolicy.CheckAcsXYServoOnAllowed(ctx, out var why))
                {
                    _alarm?.Notify("Safety Interlock", why);
                    return;
                }
            }
            await _inner.ServoOnAsync(axis);
        }
        public Task ServoOffAsync(Axis axis) => _inner.ServoOffAsync(axis);
        public Task HomeAsync(Axis axis) => _inner.HomeAsync(axis);
        public Task StopAsync(Axis axis) => _inner.StopAsync(axis);
        public Task MoveAbsAsync(Axis axis, double position, double? vel = null, double? acc = null, double? dec = null)
            => _inner.MoveAbsAsync(axis, position, vel, acc, dec);
        public Task MoveRelAsync(Axis axis, double delta, double? vel = null, double? acc = null, double? dec = null)
            => _inner.MoveRelAsync(axis, delta, vel, acc, dec);
        public Task JogStartAsync(Axis axis, bool positive, double velocity)
            => _inner.JogStartAsync(axis, positive, velocity);
        public Task JogStopAsync(Axis axis) => _inner.JogStopAsync(axis);
        public double GetPosition(Axis axis) => _inner.GetPosition(axis);

        public double GetTargetPosition(Axis axis) => _inner.GetTargetPosition(axis);

        public double GetAPosition(Axis axis) => _inner.GetAPosition(axis);

        public double GetVelocity(Axis axis) => _inner.GetVelocity(axis);
        public double GetAcceleration(Axis axis) => _inner.GetAcceleration(axis);

        public bool IsBusy(Axis axis) => _inner.IsBusy(axis);
        public MotionProfile GetProfile(Axis axis) => _inner.GetProfile(axis);
        public void SetProfile(Axis axis, double vel, double acc, double dec) => _inner.SetProfile(axis, vel, acc, dec);

        // ---- IAcsStatus 위임 (내부가 지원할 때만) ----
        private IAcsStatus InnerStatus => _inner as IAcsStatus;
        public bool IsServoOn(Axis axis) => InnerStatus?.IsServoOn(axis) ?? false;
        public bool IsHomeDone(Axis axis) => InnerStatus?.IsHomeDone(axis) ?? false;
        public bool IsInPosition(Axis axis) => InnerStatus?.IsInPosition(axis) ?? false;
        public double GetRms(Axis axis) => InnerStatus?.GetRms(axis) ?? 0.0;

        /// 최소 요건만 구현한 라이트 컨텍스트:
        /// - CheckAcsXYServoOnAllowed가 쓰는 건 Y100F(OUT_MC_ALLOK) 출력뿐
        private sealed class ServoLiteContext : ISafetyContext
        {
            public static readonly ServoLiteContext Shared = new ServoLiteContext();
            private ServoLiteContext() { }
            public ProgramMode Mode => ProgramMode.Manual;
            public string CurrentProgram => "MANUAL";
            public double GetXActualVelocity() => 0.0;
            public double GetYActualVelocity() => 0.0;
            public double GetMaintZActualVelocity() => 0.0;
            public double GetThetaActualVelocity() => 0.0;
            public bool GetInput(string ioName)
                => VirtualBus.DigitalInputs.TryGet(ioName, out var v) && v.RawBit;
            public bool GetOutput(string ioName)
                => VirtualBus.DigitalOutputs.TryGet(ioName, out var v) && v.RawBit;
            public DigitalStatus GetInputStatus(string ioName) => DigitalStatus.Unknown;
            public DigitalStatus GetOutputStatus(string ioName) => DigitalStatus.Unknown;
            public string GetInputLabel(string ioName) => "N/A";
            public string GetOutputLabel(string ioName) => "N/A";
            public bool IsLaserOn()
                => VirtualBus.DigitalInputs.TryGet("Laser ON / OFF Status", out var v) && v.RawBit;
        }
    }
}