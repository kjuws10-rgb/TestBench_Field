using System;
using System.Collections.Generic;
using StageWin.WagoIO;

namespace StageWin.Safety
{
    public interface ISafetyContext
    {
        ProgramMode Mode { get; }
        string CurrentProgram { get; }

        // === Motion과 무관한 순수 조회 API ===
        double GetXActualVelocity();
        double GetYActualVelocity();
        double GetMaintZActualVelocity();
        double GetThetaActualVelocity();

        // RawBit 호환
        bool GetInput(string ioName);
        bool GetOutput(string ioName);

        // 상태/라벨
        DigitalStatus GetInputStatus(string ioName);
        DigitalStatus GetOutputStatus(string ioName);
        string GetInputLabel(string ioName);
        string GetOutputLabel(string ioName);

        bool IsLaserOn();
    }

}
