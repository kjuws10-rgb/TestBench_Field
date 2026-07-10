using System;
using System.Collections.Generic;

namespace StageWin.WagoIO
{
    public enum DigitalStatus
    {
        Unknown = 0,
        Normal, Warning, Alarm, On, Off, Lock, Unlock, Enable, None, Teach, Auto, NA
    }

    internal static class DigitalStatusMap
    {
        public static DigitalStatus From(StandardStatusKind k)
        {
            switch (k)
            {
                case StandardStatusKind.Normal: return DigitalStatus.Normal;
                case StandardStatusKind.Warning: return DigitalStatus.Warning;
                case StandardStatusKind.Alarm: return DigitalStatus.Alarm;
                case StandardStatusKind.On: return DigitalStatus.On;
                case StandardStatusKind.Off: return DigitalStatus.Off;
                case StandardStatusKind.Lock: return DigitalStatus.Lock;
                case StandardStatusKind.Unlock: return DigitalStatus.Unlock;
                case StandardStatusKind.Enable: return DigitalStatus.Enable;
                case StandardStatusKind.None: return DigitalStatus.None;
                case StandardStatusKind.Teach: return DigitalStatus.Teach;
                case StandardStatusKind.Auto: return DigitalStatus.Auto;
                case StandardStatusKind.NA: return DigitalStatus.NA;
                default: return DigitalStatus.Unknown;
            }
        }
    }

    public sealed class VirtualDigitalInput
    {
        public string Name { get; }
        public bool RawBit { get; private set; }
        public string Label { get; private set; }
        public StandardStatusKind Kind { get; private set; }
        public DigitalStatus ReadValue => DigitalStatusMap.From(Kind);
        public DateTime UpdatedAt { get; private set; }

        internal VirtualDigitalInput(string name)
        {
            Name = name ?? "(unnamed)";
            Label = "N/A";
            Kind = StandardStatusKind.Unknown;
        }
        internal void UpdateFrom(IoEvaluatedState s)
        {
            RawBit = s.RawBit;
            Label = string.IsNullOrWhiteSpace(s.Label) ? "N/A" : s.Label;
            Kind = s.Kind;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public sealed class VirtualVariableDigitalInputs
    {
        private readonly Dictionary<string, VirtualDigitalInput> _map =
            new Dictionary<string, VirtualDigitalInput>(StringComparer.OrdinalIgnoreCase);

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "(null)";
            s = s.Trim();

            // 흔한 접미/공백 차이를 흡수
            if (s.EndsWith(" IO", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(0, s.Length - 3).Trim();

            // 연속 공백처리
            while (s.Contains("  ")) s = s.Replace("  ", " ");
            return s;
        }

        public bool TryGet(string name, out VirtualDigitalInput v)
        {
            v = null;
            if (string.IsNullOrWhiteSpace(name)) return false;

            if (_map.TryGetValue(name, out v)) return true;

            // 정규화된 키로 2차 탐색
            string target = Normalize(name);
            foreach (var kv in _map)
            {
                if (Normalize(kv.Key).Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    v = kv.Value;
                    return true;
                }
            }
            return false;
        }

        // 안전한 접근: 있으면 반환, 없으면 새로 생성
        public VirtualDigitalInput Resolve(string name)
        {
            if (TryGet(name, out var v)) return v;
            return this[name];
        }

        public VirtualDigitalInput this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name)) name = "(null)";
                if (!_map.TryGetValue(name, out var v))
                {
                    v = new VirtualDigitalInput(name);
                    _map[name] = v;
                }
                return v;
            }
        }

        public void Upsert(string name, IoEvaluatedState state)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "(null)";

            // 같은 IO를 다른 철자로 여러 번 올리지 않도록 정규화 우선 반영
            if (TryGet(name, out var v)) { v.UpdateFrom(state); return; }
            this[name].UpdateFrom(state);
        }

        public void UpsertRange(IEnumerable<(string name, IoEvaluatedState state)> items)
        {
            foreach (var (name, st) in items) Upsert(name, st);
        }
    }

    public static class VirtualBus
    {
        public static readonly VirtualVariableDigitalInputs DigitalInputs = new VirtualVariableDigitalInputs();
        public static readonly VirtualVariableDigitalInputs DigitalOutputs = new VirtualVariableDigitalInputs();
        public static readonly VirtualVariableAnalogs AnalogInputs = new VirtualVariableAnalogs();
        public static readonly VirtualVariableAnalogs AnalogOutputs = new VirtualVariableAnalogs();


    }

    // 아날로그 컨테이너/채널
    public sealed class VirtualAnalogChannel
    {
        public string Name { get; }
        public short Raw { get; private set; }           // 원 데이터(Int16)
        public double Voltage { get; private set; }      // -5~+5V 스케일 기본
        public DateTime UpdatedAt { get; private set; }

        internal VirtualAnalogChannel(string name) { Name = name ?? "(unnamed)"; }

        internal void UpdateFrom(short raw)
        {
            Raw = raw;
            Voltage = (Raw / 32767.0) * 5.0; // 기본 스케일(-5~+5V)
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public sealed class VirtualVariableAnalogs
    {
        private readonly Dictionary<string, VirtualAnalogChannel> _map =
            new Dictionary<string, VirtualAnalogChannel>(StringComparer.OrdinalIgnoreCase);

        public VirtualAnalogChannel this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name)) name = "(null)";
                if (!_map.TryGetValue(name, out var v))
                {
                    v = new VirtualAnalogChannel(name);
                    _map[name] = v;
                }
                return v;
            }
        }

        public void Upsert(string name, short raw) => this[name].UpdateFrom(raw);
    }

}
