using System;

namespace StageWin
{
    /// <summary>
    /// 설비 공정 시퀀스 ID
    /// (필요에 따라 이름/갯수는 자유롭게 수정 가능)
    /// </summary>
    public enum AutoSequenceId
    {
        Load,
        MoveToAlign,
        Align,
        PowerMeter,
        ProcessReady,
        Process,
        Inspection,
        MoveToAlignCheck,
        AlignCheck,
        MoveToUnload,
        Unload,
    }

    /// <summary>
    /// Auto 모드에서 실행할 시퀀스 묶음 요청
    /// </summary>
    public sealed class AutoProcessRequest
    {
        /// <summary>
        /// Dry Run 여부 (true면 물류만, 실제 가공/레이저 OFF)
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// 선택된 시퀀스 목록
        /// </summary>
        public AutoSequenceId[] EnabledSequences { get; set; } = Array.Empty<AutoSequenceId>();
    }
}