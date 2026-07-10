using System.Runtime.InteropServices;

namespace NetCommon
{
    // 문서 기준 상수(필요 시 조정)
    public static class VisionConst
    {
        public const int MAX_CAM = 3;          // 카메라 수 (문서상 '카메라 별 Pixel 위치' — 최대 3대로 가정)
        public const int MAX_LINE_FIND = 20;   // 라인 Find 응답 배열 최대
    }

    // 3000 Glass Align -----------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_GLASS_ALIGN_REQ { /* 문서상 Body 없음 */ }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_GLASS_ALIGN_RSP
    {
        public int nResult;        // 0:동작실패, 1:동작완료
        public int nJudgement;     // 0:NG, 1:OK
        public double dAlignErrorX; // mm 단위(문서 'AlignErrorX')
        public double dAlignErrorY; // mm 단위
        public double dAlignErrorT; // deg/urad 등 사양에 맞게
        // 카메라 별 Pixel 위치 (문서 표기: dCam_X[], dCam_Y[])
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_CAM)]
        public double[] dCam_X;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_CAM)]
        public double[] dCam_Y;
    }

    // 3001 2nd Align -------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_2ND_ALIGN_REQ { /* 문서상 Body 없음 */ }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_2ND_ALIGN_RSP
    {
        public int nResult;        // 0:동작실패, 1:동작완료
        public int nJudgement;     // 0:NG, 1:OK
        public double dAlignErrorX;
        public double dAlignErrorY;
        public double dAlignErrorT;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_CAM)]
        public double[] dCam_X;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_CAM)]
        public double[] dCam_Y;
    }

    // 3002 Mark Find (개별) ------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MARK_FIND_SINGLE_REQ
    {
        public int nLine;     // 현재 측정중인 라인 위치 (Stage 기준 line index)
        public int nGlobalR;  // globalR
        public int nGlobalC;  // globalC
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MARK_FIND_SINGLE_RSP
    {
        public int nResult;      // 0:실패, 1:성공
        public double dTargetX;  // Target mark
        public double dTargetY;
        public double dMarkX;    // Processed hole mark
        public double dMarkY;
    }

    // 3003 Mark Find (라인) ------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MARK_FIND_LINE_REQ
    {
        public int nDataCount; // 요청 개수 (#작성 Hole 개수)
    }

    // 문서 구조: { nDataCount; int nResult[20]; double dDiffX[20]; double dDiffY[20]; }
    // 가변 길이를 유연 파싱하기 위해 'Pack'형으로 분리
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MARK_FIND_LINE_RSP_PACK
    {
        public int nDataCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_LINE_FIND)]
        public int[] nResult;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_LINE_FIND)]
        public double[] dTargetX;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_LINE_FIND)]
        public double[] dTargetY;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_LINE_FIND)]
        public double[] dMarkX;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = VisionConst.MAX_LINE_FIND)]
        public double[] dMarkY;
    }

    // 3004 Flying Ready ----------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_FLYING_READY_REQ
    {
        public int nDataCount; // 가공 예정 Hole(=Row) 개수
        public int nLine;      // 라인 위치 (예: 2)
        public int nCol;       // 몇번째 열(vecC) (예: 1 -> 두번째 열)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_FLYING_READY_RSP
    {
        // 문서상 Body 없음 → 헤더 ErrorCode로 판정
    }

    // 3005 Motor Move (Vision→Stage) ---------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_MOVE_VISION_REQ
    {
        public int nMoveType;   // 0:REL, 1:ABS
        public double dMoveX;   // X
        public double dMoveY;   // Y
        public double dMoveT;   // T
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_MOVE_VISION_RSP
    {
        public int nResult;     // 4B echo 용(문서: 응답은 ack)
    }

    // 3006 Motor Current Pos (Vision→Stage) --------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_CUR_POS_REQ { /* 없음 */ }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MOTOR_CUR_POS_RSP
    {
        // 0:X, 1:Y, 2:T
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] dMotorCurPos;
    }

    // 3007 Flying Move Done ----------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_FLYING_MOVEDONE_REQ { /* 없음 */ }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_FLYING_MOVEDONE_RSP { /* 없음 */ }

    // 3008 Manual Mark Inspection -------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MANUAL_INSPECTION_REQ
    {
        public int nResult;      // 0:fail, 1:success (또는 FindResult로 사용)
        public double dTargetX;  // px
        public double dTargetY;  // px
        public double dMarkX;    // px
        public double dMarkY;    // px
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST_MANUAL_INSPECTION_RSP
    {
        
    }

}