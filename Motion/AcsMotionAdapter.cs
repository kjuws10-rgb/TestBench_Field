using ACS.SPiiPlusNET;
using System;
using System.Threading.Tasks;
// 사용자(IMotion) 축 타입 별칭
using MAxis = StageWin.Driver.Motion.Axis;
// 전역 ACS 라이브러리 별칭
using Spi = global::ACS.SPiiPlusNET;

namespace StageWin.Driver.Motion
{
    public class AcsController : IDisposable
    {
        private Spi.Api _acsApi;
        public bool IsConnected => _acsApi != null && _acsApi.IsConnected;
        private bool IsSimulation = false;
        private object m_objReadVar = null;
        public bool Connect(string ip, int port = 701, bool useSimulator = false)
        {
            try
            {
                _acsApi = new Spi.Api();
                if (useSimulator) { _acsApi.OpenCommSimulator(); IsSimulation = true; }
                else _acsApi.OpenCommEthernetTCP(ip, port);
                return _acsApi.IsConnected;
            }
            catch { _acsApi = null; return false; }
        }

        public void Disconnect()
        {
            if (_acsApi != null && _acsApi.IsConnected) _acsApi.CloseComm();
            _acsApi = null;
        }

        public void ServoOn(Spi.Axis axis) => _acsApi?.Enable(axis);
        public void ServoOff(Spi.Axis axis) => _acsApi?.Disable(axis);

        public double GetPosition(Spi.Axis axis) => _acsApi?.GetFPosition(axis) ?? double.NaN;
        public double GetVelocity(Spi.Axis axis) => _acsApi != null ? _acsApi.GetVelocity(axis) : double.NaN;
        public double GetAcceleration(Spi.Axis axis) => _acsApi != null ? _acsApi.GetAcceleration(axis) : double.NaN;
        public double GetAxisPosition(Spi.Axis axis) => _acsApi?.GetFPosition(axis) ?? double.NaN;

        public void MoveTo(Spi.Axis axis, double position, Spi.MotionFlags flags = Spi.MotionFlags.ACSC_NONE)
            => _acsApi?.ToPoint(flags, axis, position);

        public double GetAxisTargetPosition(Spi.Axis axis) => _acsApi?.GetTargetPosition(axis) ?? double.NaN;
        public void RunHome(int index, string program = "") => _acsApi?.RunBuffer((Spi.ProgramBuffer)index, null);
        public void RunBuffer(Spi.ProgramBuffer buffer, string program = "") => _acsApi?.RunBuffer(buffer, null);
        public bool IsBufferRunning(Spi.ProgramBuffer buffer) => _acsApi != null && _acsApi.GetProgramState(buffer) == Spi.ProgramStates.ACSC_PST_RUN;
        public void WaitProgramEndSafe(Spi.ProgramBuffer buffer, int timeoutMs)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");
            _acsApi.WaitProgramEnd(buffer, timeoutMs);
        }
        public void EnsureBufferStopped(Spi.ProgramBuffer buffer, int timeoutMs = 3000)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");

            // RUN이면 Stop 시도
            if (IsBufferRunning(buffer))
            {
                try { _acsApi.StopBuffer(buffer); } catch { /* ignore */ }
                try { _acsApi.WaitProgramEnd(buffer, timeoutMs); } catch { /* ignore */ }
            }
        }
        public void WaitProgramEnd(Spi.ProgramBuffer buffer, int timeout) => _acsApi?.WaitProgramEnd(buffer, timeout);

        public void SetVelocity(Spi.Axis axis, double velocity) => _acsApi?.SetVelocity(axis, velocity);
        public void SetAcceleration(Spi.Axis axis, double accel) => _acsApi?.SetAcceleration(axis, accel);
        public void SetDeceleration(Spi.Axis axis, double decel) => _acsApi?.SetDeceleration(axis, decel);
        public void SetPosition(Spi.Axis axis, double fposition) => _acsApi?.SetFPosition(axis, fposition);
        public void WaitMotionEnd(Spi.Axis axis, int timeout) => _acsApi?.WaitMotionEnd(axis, timeout);

        public void Jog(Spi.Axis axis, bool positive, double velocity = 10)
        {
            if (_acsApi == null) return;
            double vel = Math.Abs(velocity) * (positive ? 1 : -1);
            _acsApi.Jog(Spi.MotionFlags.ACSC_AMF_VELOCITY, axis, vel);
        }

        public void Halt(Spi.Axis axis) => _acsApi?.Halt(axis);

        public bool IsAxisMoving(Spi.Axis axis)
        {
            try
            {
                var st = _acsApi.GetMotorState(axis);
                return (st & Spi.MotorStates.ACSC_MST_MOVE) == Spi.MotorStates.ACSC_MST_MOVE;
            }
            catch { return false; }
        }

        public void WaitAxisIdle(Spi.Axis axis, int timeoutMs = 10000)
        {
            var t0 = Environment.TickCount;
            while (IsAxisMoving(axis))
            {
                if (timeoutMs >= 0 && Environment.TickCount - t0 > timeoutMs)
                    throw new TimeoutException($"Axis {axis} is still moving.");
                System.Threading.Thread.Sleep(10);
            }
        }

        // ========== 여기부터 PTP/PTP-R + Dwell 전용 구현 ==========

        /// <summary>PTP 절대이동 + 모션완료 대기 + Dwell(ms)</summary>
        public async Task PtpAbsAsync(
            Spi.Axis axis, double target,
            double vel, double acc, double dec,
            int dwellMs = 30, int timeoutMs = 600_000)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");

            // 선행 동작 종료 대기 (다음 명령 충돌 방지)
            WaitAxisIdle(axis, 15_000);

            // 프로파일 적용
            _acsApi.SetVelocity(axis, vel);
            _acsApi.SetAcceleration(axis, acc);
            _acsApi.SetDeceleration(axis, dec);

            // PTP (절대)
            _acsApi.ToPoint(Spi.MotionFlags.ACSC_NONE, axis, target);

            // 컨트롤러 내부 종료 대기
            _acsApi.WaitMotionEnd(axis, timeoutMs);

            // 안정 대기 (사용자 요청: INPOS 제거, Dwell만 사용)
            if (dwellMs > 0) await Task.Delay(dwellMs).ConfigureAwait(false);
        }

        /// <summary>PTP/R 상대이동 + 모션완료 대기 + Dwell(ms)</summary>
        public async Task PtpRelAsync(
            Spi.Axis axis, double delta,
            double vel, double acc, double dec,
            int dwellMs = 30, int timeoutMs = 600_000)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");

            WaitAxisIdle(axis, 15_000);

            _acsApi.SetVelocity(axis, vel);
            _acsApi.SetAcceleration(axis, acc);
            _acsApi.SetDeceleration(axis, dec);

            // PTP/R (상대)
            _acsApi.ToPoint(Spi.MotionFlags.ACSC_AMF_RELATIVE, axis, delta);
            _acsApi.WaitMotionEnd(axis, timeoutMs);
            if (dwellMs > 0) await Task.Delay(dwellMs).ConfigureAwait(false);
        }

        // ========== INPOS 관련은 남겨도 되지만 "이동 판정"엔 쓰지 않음 ==========
        public bool IsHomeDoneStatus(int axis)
        {
            try
            {
                if (_acsApi == null) return false;
                double[] homeFlags = (double[])_acsApi.ReadVariable("Home_Flag", Spi.ProgramBuffer.ACSC_NONE);
                return homeFlags[axis] == 1.0;
            }
            catch { return IsSimulation ? true : false; }
        }
        public bool IsServoOnStatus(int axis)
        {
            try
            {
                var state = _acsApi.GetMotorState((Spi.Axis)axis);
                return (state & Spi.MotorStates.ACSC_MST_ENABLE) == Spi.MotorStates.ACSC_MST_ENABLE;
            }
            catch { return false; }
        }

        public bool IsInPositionStatus(int axisIndex)
        {
            try
            {
                var state = _acsApi.GetMotorState((Spi.Axis)axisIndex);
                return (state & Spi.MotorStates.ACSC_MST_INPOS) == Spi.MotorStates.ACSC_MST_INPOS;
            }
            catch { return false; }
        }

        //RMS 17번 버퍼에서 값 읽어오기
        public double GetAxisRMS(MAxis axis)
        {
            string tempName;
            double value = 0;

            try
            {
                if (axis == MAxis.Y)
                    tempName = "RMS_Y1";
                else if (axis == MAxis.X)
                    tempName = "RMS_X1";
                else tempName = "NULL";

                //m_objReadVar = _ACS.ReadVariable(tempName, ProgramBuffer.ACSC_BUFFER_17, -1, -1, -1, -1);
                m_objReadVar = _acsApi?.ReadVariable(tempName, ProgramBuffer.ACSC_BUFFER_17);
                if (m_objReadVar != null)
                {
                    //m_objReadVar = m_objReadVar as Array;
                    if (m_objReadVar != null)
                    {
                        double.TryParse(m_objReadVar.ToString(), out value);
                        return value;
                    }
                }
            }
            catch
            {
                return 0.000;
            }
            return value;
            //try
            //{
            //    var rmsArray = (double[])_acsApi.ReadVariable("RMS", Spi.ProgramBuffer.ACSC_NONE);
            //    int idx = (axis == MAxis.Y) ? 0 : 4;
            //    return (rmsArray != null && idx >= 0 && idx < rmsArray.Length) ? rmsArray[idx] : double.NaN;
            //}
            //catch { return double.NaN; }
        }

        public double GetAPosition(MAxis axis)//FeedBack Position
        {
            try
            {
                string tempName;
                double value = 0;

                if (axis == MAxis.Y)
                    tempName = "APOS_Y1";
                else if (axis == MAxis.X)
                    tempName = "APOS_X1";
                else tempName = "NULL";

                //m_objReadVar = _ACS.ReadVariable(tempName, ProgramBuffer.ACSC_BUFFER_17, -1, -1, -1, -1);
                m_objReadVar = _acsApi?.ReadVariable(tempName, ProgramBuffer.ACSC_BUFFER_17);
                if (m_objReadVar != null)
                {
                    //m_objReadVar = m_objReadVar as Array;
                    if (m_objReadVar != null)
                    {
                        double.TryParse(m_objReadVar.ToString(), out value);
                        return value;
                    }
                }
                return value;
            }
            catch
            {

            }
            if (axis == MAxis.Y) return _acsApi.GetFPosition(Spi.Axis.ACSC_AXIS_0);
            if (axis == MAxis.X) return _acsApi.GetFPosition(Spi.Axis.ACSC_AXIS_4);
            return double.NaN;
        }
        public void WriteVariable(object value, string variable, Spi.ProgramBuffer nBuf,
                              int from1 = Spi.Api.ACSC_NONE, int to1 = Spi.Api.ACSC_NONE,
                              int from2 = Spi.Api.ACSC_NONE, int to2 = Spi.Api.ACSC_NONE)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");
            _acsApi.WriteVariable(value, variable, nBuf, from1, to1, from2, to2);
        }
        public Task WriteVariableAsync(object value, string variable, Spi.ProgramBuffer nBuf,
                                       int from1 = Spi.Api.ACSC_NONE, int to1 = Spi.Api.ACSC_NONE,
                                       int from2 = Spi.Api.ACSC_NONE, int to2 = Spi.Api.ACSC_NONE)
            => Task.Run(() => WriteVariable(value, variable, nBuf, from1, to1, from2, to2));

        public Task RunBufferAsync(Spi.ProgramBuffer buffer)
            => Task.Run(() =>
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");

            try
            {
                // 1) 첫 실행 전: 혹시 RUN이면 정리
                EnsureBufferStopped(buffer, timeoutMs: 3000);

                // 2) 실행
                _acsApi.RunBuffer(buffer, null);
            }
            catch (Exception ex)
            {
                // "program is running"류면 1회 더 정리 후 재시도
                if ((ex.Message ?? "").IndexOf("program is running", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (ex.Message ?? "").IndexOf("Command cannot be executed while the program is running", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    EnsureBufferStopped(buffer, timeoutMs: 5000);
                    _acsApi.RunBuffer(buffer, null);
                    return;
                }
                throw;
            }
        });
        public void Dispose() => Disconnect();

        public void StopBuffer(Spi.ProgramBuffer buffer)
        {
            if (_acsApi == null) throw new InvalidOperationException("ACS not connected.");

            // 1st: StopBuffer
            try { _acsApi.StopBuffer(buffer); return; }
            catch (MissingMethodException) { /* fallthrough */ }
            catch (NotImplementedException) { /* fallthrough */ }
        }

        public Task StopBufferAsync(Spi.ProgramBuffer buffer)
            => Task.Run(() => StopBuffer(buffer));
    }

    // IMotion 어댑터
    public sealed class AcsMotionAdapter : Motion.IMotionController, Motion.IAcsStatus, Motion.IAcsVariables, Motion.IAcsPrograms
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly Action<string> _log;
        private readonly bool _useSimulator;
        private AcsController _acs;
        private StageWin.Driver.Motion.MotionProfile _px = new StageWin.Driver.Motion.MotionProfile(50, 100, 100); // X(Review)
        private StageWin.Driver.Motion.MotionProfile _py = new StageWin.Driver.Motion.MotionProfile(50, 100, 100); // Y(Main)

        // 축 매핑: User(MAxis) -> ACS(Spi.Axis)
        private static int GetAcsIndex(MAxis a) => (a == MAxis.Y) ? 0 : 4;
        private static Spi.Axis ToAcs(MAxis a)
            => (a == MAxis.Y) ? Spi.Axis.ACSC_AXIS_0 : Spi.Axis.ACSC_AXIS_4;
        private int _postMoveDwellMs = 300; // 모션 완료 후 안정 대기(ms). 필요 시 SetPostMoveDwell로 변경
        public void SetPostMoveDwell(int ms) => _postMoveDwellMs = Math.Max(0, ms);
        public AcsMotionAdapter(string ip, int port, Action<string> log = null, bool useSimulator = false)
        {
            _ip = ip; _port = port; _log = log ?? (_ => { });
            _useSimulator = useSimulator;
        }

        public bool IsConnected => _acs != null && _acs.IsConnected;

        public void Connect()
        {
            _acs = new AcsController();
            if (!_acs.Connect(_ip, _port, _useSimulator))
                throw new Exception($"ACS connect failed: {(_useSimulator ? "Simulator" : $"{_ip}:{_port}")}");
            _log($"ACS connected {(_useSimulator ? "(Simulator)" : $"{_ip}:{_port}")}");
        }

        public void Disconnect()
        {
            try { _acs?.Dispose(); } catch { }
            _acs = null;
            _log("ACS disconnected");
        }

        public void Dispose() => Disconnect();

        //  Servo / Home / Stop 
        public Task ServoOnAsync(MAxis axis) => Task.Run(() => _acs.ServoOn(ToAcs(axis)));
        public Task ServoOffAsync(MAxis axis) => Task.Run(() => _acs.ServoOff(ToAcs(axis)));
        public Task HomeAsync(MAxis axis)
        {
            int idx = GetAcsIndex(axis);
            return Task.Run(() =>
            {
                _acs.RunHome(idx, string.Empty);
                var t0 = Environment.TickCount;
                while (!_acs.IsHomeDoneStatus(idx))
                {
                    if (Environment.TickCount - t0 > 300000)
                        throw new TimeoutException("Home timeout");
                    System.Threading.Thread.Sleep(50);
                }
            });
        }
        public Task StopAsync(MAxis axis) => Task.Run(() => _acs.Halt(ToAcs(axis)));

        // 절대 이동 = PTP
        public Task MoveAbsAsync(MAxis axis, double position, double? vel = null, double? acc = null, double? dec = null)
        {
            var p = (axis == MAxis.X) ? _px : _py;
            var v = vel ?? p.Velocity;
            var a = acc ?? p.Acceleration;
            var d = dec ?? p.Deceleration;

            return Task.Run(async () =>
            {
                var ax = ToAcs(axis);
                try
                {
                    await _acs.PtpAbsAsync(ax, position, v, a, d, dwellMs: _postMoveDwellMs, timeoutMs: 600_000);
                }
                catch (Exception ex)
                {
                    throw new Exception($"[{axis}] PTP failed: {ex.Message}", ex);
                }
            });
        }

        // 상대 이동 = PTP/R (이제 더 이상 현재 위치 읽어 + delta 절대이동으로 바꾸지 않음)
        public Task MoveRelAsync(MAxis axis, double delta, double? vel = null, double? acc = null, double? dec = null)
        {
            var p = (axis == MAxis.X) ? _px : _py;
            var v = vel ?? p.Velocity;
            var a = acc ?? p.Acceleration;
            var d = dec ?? p.Deceleration;

            return Task.Run(async () =>
            {
                var ax = ToAcs(axis);
                try
                {
                    await _acs.PtpRelAsync(ax, delta, v, a, d, dwellMs: _postMoveDwellMs, timeoutMs: 600_000);
                }
                catch (Exception ex)
                {
                    throw new Exception($"[{axis}] PTP/R failed: {ex.Message}", ex);
                }
            });
        }
        //  JOG 
        public Task JogStartAsync(MAxis axis, bool positive, double velocity)
            => Task.Run(() => _acs.Jog(ToAcs(axis), positive, Math.Abs(velocity)));
        public Task JogStopAsync(MAxis axis) => StopAsync(axis);

        //  Status 
        public double GetPosition(MAxis axis) => _acs.GetAxisPosition(ToAcs(axis));


        public double GetTargetPosition(MAxis axis) => _acs.GetAxisTargetPosition(ToAcs(axis));

        public double GetAPosition(MAxis axis) => _acs.GetAPosition(axis);

        public double GetVelocity(MAxis axis) => _acs.GetVelocity(ToAcs(axis));

        public double GetAcceleration(MAxis axis) => _acs.GetAcceleration(ToAcs(axis));

        public bool IsBusy(MAxis axis) => _acs.IsAxisMoving(ToAcs(axis));
        public StageWin.Driver.Motion.MotionProfile GetProfile(MAxis axis)
            => (axis == MAxis.X) ? _px : _py;

        public void SetProfile(MAxis axis, double vel, double acc, double dec)
        {
            if (axis == MAxis.X) _px = new StageWin.Driver.Motion.MotionProfile(vel, acc, dec);
            else _py = new StageWin.Driver.Motion.MotionProfile(vel, acc, dec);
        }

        //  IAcsStatus 구현 
        public bool IsServoOn(MAxis axis) => _acs.IsServoOnStatus(GetAcsIndex(axis));
        public bool IsHomeDone(MAxis axis) => _acs.IsHomeDoneStatus(GetAcsIndex(axis));
        public bool IsInPosition(MAxis axis) => _acs.IsInPositionStatus(GetAcsIndex(axis));
        public double GetRms(MAxis axis) => _acs.GetAxisRMS(axis);
        public void WriteVariable(object value, string variable, ProgramBuffer nBuf,
                              int from1 = Api.ACSC_NONE, int to1 = Api.ACSC_NONE,
                              int from2 = Api.ACSC_NONE, int to2 = Api.ACSC_NONE)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            _acs.WriteVariable(value, variable, nBuf, from1, to1, from2, to2);
        }

        public Task WriteVariableAsync(object value, string variable, ProgramBuffer nBuf,
                                       int from1 = Api.ACSC_NONE, int to1 = Api.ACSC_NONE,
                                       int from2 = Api.ACSC_NONE, int to2 = Api.ACSC_NONE)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            return _acs.WriteVariableAsync(value, variable, nBuf, from1, to1, from2, to2);
        }

        // === IAcsPrograms 구현 ===
        public void RunBuffer(int bufferNo)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            _acs.RunBuffer((Spi.ProgramBuffer)bufferNo, string.Empty);
        }

        public Task RunBufferAsync(int bufferNo)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            return _acs.RunBufferAsync((Spi.ProgramBuffer)bufferNo);
        }
        public void StopBuffer(int bufferNo)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            _acs.StopBuffer((Spi.ProgramBuffer)bufferNo);
        }

        public Task StopBufferAsync(int bufferNo)
        {
            if (_acs == null) throw new InvalidOperationException("ACS not connected.");
            return _acs.StopBufferAsync((Spi.ProgramBuffer)bufferNo);
        }
    }
}