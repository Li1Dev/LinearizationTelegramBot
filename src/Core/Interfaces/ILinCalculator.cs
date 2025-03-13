using System.Drawing;

namespace Lin.Core;

public interface ILinCalculator
{
    Task<CalculateResult> CalculateLinResultAsync(IReadOnlyList<Point> points, CancellationToken ct);

    Task<MemoryStream> DrowGraphAsync(DrowRequest drowRequest, CancellationToken ct);
}

