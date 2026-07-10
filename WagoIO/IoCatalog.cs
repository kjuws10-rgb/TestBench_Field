using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StageWin.WagoIO
{
    public static class IoCatalog
    {
        // Bank(StatusCode)별 nibble index(0~15) → IOData
        private static readonly Dictionary<string, Dictionary<int, IOData>> _byBankNibble =
            new Dictionary<string, Dictionary<int, IOData>>(StringComparer.OrdinalIgnoreCase);

        // 빠른 조회: (StatusCode, SubAddressHex) → IOData
        private static readonly Dictionary<string, IOData> _byBankAndSub =
            new Dictionary<string, IOData>(StringComparer.OrdinalIgnoreCase);

        // 문자열 라벨 → 표준 분류 매핑(대소문자 무시)
        private static readonly Dictionary<string, StandardStatusKind> _label2Kind =
            new Dictionary<string, StandardStatusKind>(StringComparer.OrdinalIgnoreCase)
            {
                { "NORMAL",  StandardStatusKind.Normal },
                { "WARNING", StandardStatusKind.Warning },
                { "ALARM",   StandardStatusKind.Alarm },
                { "ON",      StandardStatusKind.On },
                { "OFF",     StandardStatusKind.Off },
                { "LOCK",    StandardStatusKind.Lock },
                { "UNLOCK",  StandardStatusKind.Unlock },
                { "ENABLE",  StandardStatusKind.Enable },
                { "NONE",    StandardStatusKind.None },
                { "TEACH",   StandardStatusKind.Teach },
                { "AUTO",    StandardStatusKind.Auto },
                { "N/A",     StandardStatusKind.NA },
            };

        public static void BuildFrom(IEnumerable<IOData> entries)
        {
            _byBankNibble.Clear();
            _byBankAndSub.Clear();

            foreach (var g in entries.GroupBy(e => e.StatusCode))
            {
                var bank = g.Key;
                var map = new Dictionary<int, IOData>();

                foreach (var e in g)
                {
                    if (!string.IsNullOrEmpty(e.SubAddressHex))
                    {
                        // SubAddress의 마지막 nibble(0~F) → 행과 매칭
                        int nibble = 0;
                        try
                        {
                            var lastHex = e.SubAddressHex.Substring(e.SubAddressHex.Length - 1);
                            nibble = int.Parse(lastHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        }
                        catch { nibble = 0; }

                        map[nibble] = e;
                        _byBankAndSub[bank + "|" + e.SubAddressHex] = e;
                    }
                }
                _byBankNibble[bank] = map;
            }
        }

        public static IOData Find(string statusCode, string subAddressHex)
        {
            IOData e;
            if (_byBankAndSub.TryGetValue(statusCode + "|" + subAddressHex, out e)) return e;
            return null;
        }

        public static IOData FindByNibble(string statusCode, int nibble)
        {
            Dictionary<int, IOData> m;
            if (_byBankNibble.TryGetValue(statusCode, out m))
            {
                IOData e;
                if (m.TryGetValue(nibble, out e)) return e;
            }
            return null;
        }

        public static IoEvaluatedState Evaluate(IOData entry, bool coilValue)
        {
            string label = null;
            if (entry != null && !entry.IsAnalog)
                label = coilValue ? entry.TrueLabel : entry.FalseLabel;

            var kind = StandardStatusKind.Unknown;
            if (!string.IsNullOrEmpty(label))
            {
                StandardStatusKind k;
                if (_label2Kind.TryGetValue(label, out k)) kind = k;
            }

            return new IoEvaluatedState
            {
                RawBit = coilValue,
                Label = label ?? "",
                Kind = kind,
                Entry = entry
            };
        }

        // 편의: 입력코일 시작주소(0,16,32,...) → "X100","X101"… 로 환산
        public static string InputBankFromStartAddress(ushort startAddress)
        {
            // 0→X100, 16→X101, 32→X102 ...
            var idx = startAddress / 16;
            return "X10" + idx.ToString(CultureInfo.InvariantCulture);
        }

        // 편의: 출력상태 읽기 주소(512,513,514,...) → "Y100","Y101"…
        public static string OutputBankFromStartAddress(ushort startAddress)
        {
            var idx = startAddress - 512;
            return "Y10" + idx.ToString(CultureInfo.InvariantCulture);
        }
    }
}