using System.Threading;
using System.Threading.Tasks;

namespace StageWin.Process
{
    /// <summary>
    /// 프로세스에서 외부(모션/비전/스캔/IO)를 호출하기 위해 필요한 최소 인터페이스.
    /// WinForms에서 구현체를 만들어 전달.
    /// </summary>
    public interface IPlantActions
    {
        Task MoveXAsync(double x, CancellationToken ct);
        Task MoveYAsync(double y, CancellationToken ct);
        Task<bool> VisionAlignAsync(CancellationToken ct);
        Task<bool> ScanProcessAsync(CancellationToken ct);
    }
}
