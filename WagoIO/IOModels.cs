using System;
using System.Globalization;

namespace StageWin.WagoIO
{
    // 표준화된 상태 분류(옵션). 매핑되지 않으면 Unknown
    public enum StandardStatusKind
    {
        Unknown = 0,
        Normal, Warning, Alarm, On, Off, Lock, Unlock, Enable, None, Teach, Auto, NA
    }

    public sealed class IOData
    {
        public string Name { get; set; }            // 예: "PM Rack Fan Status - Front 1"
        public string StatusCode { get; set; }      // 예: "X100", "Y101", "AO01"
        public string SubAddressHex { get; set; }   // 예: "000A"
        public int Index { get; set; }              // 16(디지털), 1/2/…(아날로그 word 수)
        public string FalseLabel { get; set; }      // 예: "Warning"
        public string TrueLabel { get; set; }       // 예: "Normal"

        public bool IsInput { get { return StatusCode.StartsWith("X", StringComparison.OrdinalIgnoreCase); } }
        public bool IsOutput { get { return StatusCode.StartsWith("Y", StringComparison.OrdinalIgnoreCase); } }
        public bool IsAnalog { get { return StatusCode.StartsWith("AO", StringComparison.OrdinalIgnoreCase) || StatusCode.StartsWith("AI", StringComparison.OrdinalIgnoreCase); } }

        public int SubAddressInt
        {
            get
            {
                int v; int.TryParse(SubAddressHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v);
                return v;
            }
        }
    }

    public struct IoEvaluatedState
    {
        public bool RawBit;                 // Coil raw 값
        public string Label;                // Config 선언 라벨
        public StandardStatusKind Kind;     // 표준화 분류(선택)
        public IOData Entry;                // 해당 IO 엔트리
    }
}