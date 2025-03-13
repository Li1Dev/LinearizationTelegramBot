using System.Drawing;

namespace Lin.Core;

public record CalculateResult
{
    public required IReadOnlyList<Point> Points { get; init; }

    public required IReadOnlyList<IReadOnlyList<double>> Equations { get; init; }

    public required IReadOnlyList<double> Sum { get; init; }

    public required IReadOnlyList<IReadOnlyList<double>> Derivatives { get; init; }

    public required (double K, double B) Result { get; init; }
}