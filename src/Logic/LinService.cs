using System.Drawing;
using Lin.Core;

namespace Lin.Logic;

public class LinService : ILinService
{
    private readonly ILinCalculator _linCalculator;

    private readonly ICalculationMapper _calculationMapper;

    public LinService(
        ILinCalculator linCalculator,
        ICalculationMapper calculationMapper)
    {
        _linCalculator = linCalculator ?? throw new ArgumentNullException(nameof(linCalculator));
        _calculationMapper = calculationMapper ?? throw new ArgumentNullException(nameof(calculationMapper));
    }

    public async Task<LinResult> GetLinAsync(
        IReadOnlyList<Point> points,
        CancellationToken ct)
    {
        var calculateRes = await _linCalculator.CalculateLinResultAsync(points, ct);
        var graph = await _linCalculator.DrowGraphAsync(_calculationMapper.MapToDrowRequest(calculateRes), ct);

        var res = new LinResult
        {
            Solving = _calculationMapper.MapToText(calculateRes),
            Graph = graph
        };

        return res;
    }
}