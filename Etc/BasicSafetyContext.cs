using System;
using StageWin.WagoIO;

namespace StageWin.Safety
{
    // 외부에서 실제 모드/속도 제공 안 해도, IO(도어/그립/레이저)는 VirtualBus로 읽어와서 안전판단 가능
    public sealed class BasicSafetyContext : ISafetyContext
    {
        private readonly Func<ProgramMode> _modeGetter;
        private readonly Func<string> _programGetter;
        private readonly Func<double> _vxGetter, _vyGetter, _vzGetter, _vtGetter;

        public BasicSafetyContext(
            Func<ProgramMode> modeGetter = null,
            Func<string> programGetter = null,
            Func<double> vxGetter = null,
            Func<double> vyGetter = null,
            Func<double> vzGetter = null,
            Func<double> vtGetter = null)
        {
            _modeGetter = modeGetter;
            _programGetter = programGetter;
            _vxGetter = vxGetter;
            _vyGetter = vyGetter;
            _vzGetter = vzGetter;
            _vtGetter = vtGetter;
        }

        public ProgramMode Mode => _modeGetter?.Invoke() ?? ProgramMode.Manual;
        public string CurrentProgram => _programGetter?.Invoke() ?? "-";

        public double GetXActualVelocity() => _vxGetter?.Invoke() ?? 0;
        public double GetYActualVelocity() => _vyGetter?.Invoke() ?? 0;
        public double GetMaintZActualVelocity() => _vzGetter?.Invoke() ?? 0;
        public double GetThetaActualVelocity() => _vtGetter?.Invoke() ?? 0;

        public bool GetInput(string ioName)
            => VirtualBus.DigitalInputs.TryGet(ioName, out var v) && v.RawBit;

        public bool GetOutput(string ioName)
            => VirtualBus.DigitalOutputs.TryGet(ioName, out var v) && v.RawBit;

        public DigitalStatus GetInputStatus(string ioName)
        {
            if (VirtualBus.DigitalInputs.TryGet(ioName, out var v))
            {
                // 레이블 기반 추정 -> 불가하면 bit로 추정
                var s = ParseStatus(v.Label);
                if (s != DigitalStatus.Unknown) return s;
                return v.RawBit ? DigitalStatus.On : DigitalStatus.Off;
            }
            return DigitalStatus.Unknown;
        }

        public DigitalStatus GetOutputStatus(string ioName)
        {
            if (VirtualBus.DigitalOutputs.TryGet(ioName, out var v))
            {
                var s = ParseStatus(v.Label);
                if (s != DigitalStatus.Unknown) return s;
                return v.RawBit ? DigitalStatus.On : DigitalStatus.Off;
            }
            return DigitalStatus.Unknown;
        }

        public string GetInputLabel(string ioName)
            => VirtualBus.DigitalInputs.TryGet(ioName, out var v) ? v.Label : null;

        public string GetOutputLabel(string ioName)
            => VirtualBus.DigitalOutputs.TryGet(ioName, out var v) ? v.Label : null;

        public bool IsLaserOn() => GetInput(SafetyPolicy.IN_LASER_STATUS);

        private static DigitalStatus ParseStatus(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return DigitalStatus.Unknown;
            var t = label.Trim().ToUpperInvariant();
            if (t.Contains("TEACH") || t.Contains("MANUAL") || t.Contains("HAND")) return DigitalStatus.Teach;
            if (t.Contains("AUTO") || t.Contains("AUTOMATIC")) return DigitalStatus.Auto;
            if (t.Contains("LOCK")) return DigitalStatus.Lock;
            if (t.Contains("UNLOCK")) return DigitalStatus.Unlock;
            if (t == "ON" || t.Contains("ENABLE")) return DigitalStatus.On;
            if (t == "OFF" || t.Contains("DISABLE")) return DigitalStatus.Off;
            if (t == "NA" || t == "N/A" || t == "NONE") return DigitalStatus.None;
            return DigitalStatus.Unknown;
        }
    }
}
