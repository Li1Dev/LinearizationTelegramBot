using System.Drawing;

namespace Lin.Core;

public interface ILinService
{
    Task<LinResult> GetLinAsync(
        IReadOnlyList<Point> points,
        CancellationToken ct);
}

