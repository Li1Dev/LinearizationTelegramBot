using System.Drawing;

namespace Lin.Core;

public record DrowRequest
{
    public required IReadOnlyList<IReadOnlyList<Point>> Points { get; init; }

    public required IReadOnlyList<Func<double, double>> Functions { get; set; }
}