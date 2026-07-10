using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NetCommon
{
    //  [ADD] 새 파라미터 키 & 고정 ASCII 유틸 
    public static class ScanParamKeys
    {
        public const string LASER_POWER = "LASER_POWER";
        public const string ATTENUATOR_POS = "ATTENUATOR_POS";
    }

    public static class FixedAsciiUtil
    {
        public static byte[] ToFixed(string s, int len)
        {
            var src = Encoding.ASCII.GetBytes(s ?? string.Empty);
            var buf = new byte[len];
            Buffer.BlockCopy(src, 0, buf, 0, Math.Min(src.Length, len));
            return buf;
        }
        public static string FromFixed(byte[] block)
        {
            if (block == null) return string.Empty;
            int term = Array.IndexOf(block, (byte)0);
            if (term < 0) term = block.Length;
            return Encoding.ASCII.GetString(block, 0, term);
        }
    }

    //  Stage <-> Scan 전용 바디 정의 (REQ/RSP 규칙) 
    public static class ScanConst
    {
        public const int MAX_RECIPES = 200;
        public const int RECIPE_NAME_BYTES = 40; // ASCII 40바이트 고정
    }

    public static class ScanUtil
    {
        public static readonly int SIZEOF_DRAW = Marshal.SizeOf<ST_DRAW_DATA_LIST>();
        public static readonly int SIZEOF_POWER_TABLE = Marshal.SizeOf<ST_POWER_TABLE>();
        public static byte[] ToFixedName(string name)
        {
            var b = Encoding.ASCII.GetBytes(name ?? "");
            var buf = new byte[ScanConst.RECIPE_NAME_BYTES];
            Array.Copy(b, 0, buf, 0, Math.Min(b.Length, ScanConst.RECIPE_NAME_BYTES));
            return buf;
        }
        public static string FromFixedName(byte[] block)
        {
            if (block == null || block.Length < ScanConst.RECIPE_NAME_BYTES) return string.Empty;
            int term = Array.IndexOf(block, (byte)0);
            if (term < 0) term = ScanConst.RECIPE_NAME_BYTES;
            return Encoding.ASCII.GetString(block, 0, term);
        }
        public static ST_RECIPE_LIST_RSP FromStrings(string[] names)
        {
            var rl = new ST_RECIPE_LIST_RSP
            {
                nRecipeCount = Math.Min(names?.Length ?? 0, ScanConst.MAX_RECIPES),
                RecipeNameBlock = new byte[ScanConst.MAX_RECIPES * ScanConst.RECIPE_NAME_BYTES]
            };
            for (int i = 0; i < rl.nRecipeCount; i++)
            {
                var b = Encoding.ASCII.GetBytes(names[i] ?? "");
                int len = Math.Min(b.Length, ScanConst.RECIPE_NAME_BYTES);
                Array.Copy(b, 0, rl.RecipeNameBlock, i * ScanConst.RECIPE_NAME_BYTES, len);
            }
            return rl;
        }

        public static string[] ToStrings(in ST_RECIPE_LIST_RSP rl)
        {
            int n = Math.Max(0, Math.Min(rl.nRecipeCount, ScanConst.MAX_RECIPES));
            var arr = new string[n];
            for (int i = 0; i < n; i++)
            {
                var slice = new byte[ScanConst.RECIPE_NAME_BYTES];
                Array.Copy(rl.RecipeNameBlock, i * ScanConst.RECIPE_NAME_BYTES, slice, 0, ScanConst.RECIPE_NAME_BYTES);
                int term = Array.IndexOf(slice, (byte)0);
                if (term < 0) term = ScanConst.RECIPE_NAME_BYTES;
                arr[i] = Encoding.ASCII.GetString(slice, 0, term);
            }
            return arr;
        }

        // = Serialize: 헤더 struct 바이트 뒤에 배열 원소들을 그대로 붙임 =
        public static byte[] BuildBody(in ST_PROCESS_SCANNING_REQ header, ST_DRAW_DATA_LIST[] list)
        {
            if (list == null) list = Array.Empty<ST_DRAW_DATA_LIST>();

            var head = BinMarshal.ToBytes(header);
            var total = new byte[head.Length + list.Length * SIZEOF_DRAW];

            Buffer.BlockCopy(head, 0, total, 0, head.Length);
            for (int i = 0; i < list.Length; i++)
            {
                var item = BinMarshal.ToBytes(list[i]);
                Buffer.BlockCopy(item, 0, total, head.Length + i * SIZEOF_DRAW, SIZEOF_DRAW);
            }
            return total;
        }

        // 요청 body 조립(헤더 + 테이블[n])
        public static byte[] BuildMeas1Body(in ST_POWER_MEAS1_REQ header, ST_POWER_TABLE[] list)
        {
            if (list == null) list = Array.Empty<ST_POWER_TABLE>();

            var head = BinMarshal.ToBytes(header);
            var total = new byte[head.Length + list.Length * SIZEOF_POWER_TABLE];

            Buffer.BlockCopy(head, 0, total, 0, head.Length);
            for (int i = 0; i < list.Length; i++)
            {
                var item = BinMarshal.ToBytes(list[i]);
                Buffer.BlockCopy(item, 0, total, head.Length + i * SIZEOF_POWER_TABLE, SIZEOF_POWER_TABLE);
            }
            return total;
        }

        // 요청 body 파싱(유효성 체크)
        public static bool TryParsePowerMeas1Body(
            byte[] body,
            out ST_POWER_MEAS1_REQ reqHeader,
            out ST_POWER_TABLE[] tableList,
            out string error)
        {
            reqHeader = default;
            tableList = Array.Empty<ST_POWER_TABLE>();
            error = null;

            if (body == null || body.Length < Marshal.SizeOf<ST_POWER_MEAS1_REQ>())
            { error = "body too small"; return false; }

            int headSize = Marshal.SizeOf<ST_POWER_MEAS1_REQ>();
            var headBuf = new byte[headSize];
            Buffer.BlockCopy(body, 0, headBuf, 0, headSize);
            reqHeader = BinMarshal.FromBytesStrict<ST_POWER_MEAS1_REQ>(headBuf);

            int n = reqHeader.nDataCount;
            if (n < 0 || n > 1_000_000) { error = "invalid nDataCount"; return false; }

            int expect = headSize + n * SIZEOF_POWER_TABLE;
            if (body.Length != expect) { error = $"size mismatch {body.Length}!={expect}"; return false; }

            var arr = new ST_POWER_TABLE[n];
            for (int i = 0; i < n; i++)
            {
                var itemBuf = new byte[SIZEOF_POWER_TABLE];
                Buffer.BlockCopy(body, headSize + i * SIZEOF_POWER_TABLE, itemBuf, 0, SIZEOF_POWER_TABLE);
                arr[i] = BinMarshal.FromBytesStrict<ST_POWER_TABLE>(itemBuf);
            }
            tableList = arr;
            return true;
        }

        public static ST_POWER_MEAS1_RSP ParsePowerMeas1Rsp(byte[] body)
        {
            if (body == null || body.Length < Marshal.SizeOf<ST_POWER_MEAS1_RSP_HEAD>())
                throw new ArgumentException("Meas1 RSP body too small");

            int headSize = Marshal.SizeOf<ST_POWER_MEAS1_RSP_HEAD>();
            var headBuf = new byte[headSize];
            Buffer.BlockCopy(body, 0, headBuf, 0, headSize);
            var head = BinMarshal.FromBytesStrict<ST_POWER_MEAS1_RSP_HEAD>(headBuf);

            int remain = body.Length - headSize;
            if (remain % 8 != 0) throw new ArgumentException("Meas1 RSP invalid size (double array broken)");
            int n = remain / 8;

            var arr = new double[n];
            for (int i = 0; i < n; i++)
            {
                var dblBuf = new byte[8];
                Buffer.BlockCopy(body, headSize + i * 8, dblBuf, 0, 8);
                arr[i] = BitConverter.ToDouble(dblBuf, 0);
            }

            return new ST_POWER_MEAS1_RSP
            {
                nScanNo = head.nScanNo,
                bResult = head.bResult,
                dResultAttPos = arr
            };
        }
    }

    // 1000 Recipe List
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_LIST_REQ { /* empty (0B) */ }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_LIST_RSP
    {
        public int nRecipeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.MAX_RECIPES * ScanConst.RECIPE_NAME_BYTES)]
        public byte[] RecipeNameBlock; // 200*40 bytes, 0 padding
    }

    // 1001 Recipe Add
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_ADD_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName; // 40 bytes ASCII
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_ADD_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName; // echo
    }

    // 1002 Recipe Delete
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_DELETE_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName; // 40 bytes ASCII
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_DELETE_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName; // echo
    }

    // 1003 Recipe Select
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_SELECT_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_SELECT_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName; // echo
    }

    // 1004 Recipe Info Request
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PARAM_DATA_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] chDataName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PARAM_DATA_VAL
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] chDataName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] chDataValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PARAM_DATA_VAL_RES
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] chDataName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] chDataValue;
        public int nResult; // 0/1
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PARAM_DATA_RESULT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] chDataName;
        public int nResult; // 0/1
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_INFO_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ST_PARAM_DATA_REQ[] Items;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_INFO_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName;
        public int nDataCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ST_PARAM_DATA_VAL_RES[] Items;
    }

    // 1004 Recipe Data Change
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_DATA_CHANGE_REQ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ST_PARAM_DATA_VAL[] Items;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_RECIPE_DATA_CHANGE_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ScanConst.RECIPE_NAME_BYTES)]
        public byte[] chArrRecipeName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ST_PARAM_DATA_RESULT[] Items;
    }

    // 1006 Scan Stop
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_SCAN_STOP_REQ
    {
        public int nScanNo;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_SCAN_STOP_RSP
    {
        public int nScanNo;
    }

    // 1007 Laser Control
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_LASER_CONTROL_REQ
    {
        public int nScanNo;        // 4
        public long nTime;         // 8 (출사 시간)
        public double dPower;      // 8
        public double dFrequency;  // 8
        public int bLaserOn;       // 4 (BOOL 0/1)
        public double dXOffset;    // 8
        public double dYOffset;    // 8
                                    // 추후 항목 합의 시 뒤에 필드 추가
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_LASER_CONTROL_RSP
    {
        public int nScanNo;
    }

    // 1008 All Scan Status
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_SCAN_STATUS_REQ { /* empty (0B) */ }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_SCAN_STATUS_RSP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public int[] nArrScanStatus;
    }

    // 1009 Process Scanning
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_TOOL_LASER_PARAM
    {
        public double dPower;
        public double dAttPos;
        public double dFreq;
        public double dProcessSpeed;
        public int iLaserShotCount;
        public double dJumpSpeed;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_DRAW_DATA_LIST
    {
        public int nRowNo;
        public int nColNo;
        public double dMarkX;
        public double dMarkY;
        public double dOffsetX;
        public double dOffsetY;
        public ST_TOOL_LASER_PARAM toolParam;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PROCESS_SCANNING_REQ
    {
        public int nScanNo;
        public int nDataCount;
        public double dMotorX;
        public double dMotorY;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_PROCESS_SCANNING_RSP
    {
        public int nScanNo; // echo
    }

    // 1010 PowerMeter#1
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_POWER_TABLE
    {
        public double dFrequency;   // Laser Freq
        public double dTargetPower; // Target Power
        public double dAttMinPos;   // Attenuator Min Pos
        public double dAttMaxPos;   // Attenuator Max Pos
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_POWER_MEAS1_REQ
    {
        public int nScanNo;     // Scan Head 번호
        public int nDataCount;  // 측정 개수 (= 테이블 원소 수)
        public double dRange;      // ResultPower의 Set ±% (허용 편차)
    }

    //  #1: RESPONSE 헤더(+가변 double 배열) 
    // 본문: [ST_POWER_MEAS1_RSP_HEAD(8B)] + [double dResultAttPos[n]]
    //  - n 은 바디 길이에서 역산합니다. (프로토콜 상 별도 n 없음)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_POWER_MEAS1_RSP_HEAD
    {
        public int nScanNo;
        public int bResult; // 0:NG 1:OK
    }

    public sealed class ST_POWER_MEAS1_RSP
    {
        public int nScanNo;
        public int bResult;          // 0/1
        public double[] dResultAttPos; // 보정된 Att 위치 배열 (길이 = (len-8)/8)
    }

    // 1011 PowerMeter#2
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_POWER_MEAS2_REQ
    {
        public int nScanNo;    // Scan Head 번호
        public double dRange;     // 허용 편차(%)
        public double dFrequency; // Freq
        public double dAttPos;    // 측정할 Att 위치
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_POWER_MEAS2_RSP
    {
        public int nScanNo;             // echo
        public int bResult;             // 0:NG 1:OK
        public double dResultLaserPower;   // 측정된 레이저 출력
    }

    // 1021 Motor Move (Scan -> Stage)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_MOVE_REQ
    {
        public int nAxisNo;
        public int nMoveType;    // 0:REL, 1:ABS
        public double dMoveValue;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_MOVE_RSP
    {
        public int nAxisNo; // echo
    }

    // 1022 Motor Position (Scan -> Stage)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_POS_REQ
    {
        public int nAxisNo; // 4B
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_POS_RSP
    {
        public int nAxisNo;   // echo (4B)
        public double nCurPos; // mm (8B)
    }
}
