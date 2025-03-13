using System.Text;
using Lin.Core;

namespace Lin.Logic;

public class CalculationMapper : ICalculationMapper
{
    private readonly static char _delta = '\u0394';

    private readonly static char[] _subscriptNumber = [
        '\u2080',
        '\u2081',
        '\u2082',
        '\u2083',
        '\u2084',
        '\u2085',
        '\u2086',
        '\u2087',
        '\u2088',
        '\u2089',
    ];

    private readonly static string[] _mask = [
        "",
        $"a{_subscriptNumber[0]}",
        $"a{_subscriptNumber[1]}",
        $"a{_subscriptNumber[0]}{_superscriptTwo}",
        $"a{_subscriptNumber[0]}a{_subscriptNumber[1]}",
        $"a{_subscriptNumber[1]}{_superscriptTwo}",
    ];

    private readonly static char _superscriptTwo = '\u00B2';

    private readonly static char _capLetterSigma = '\u03A3';

    private readonly static char _letterDelta = '\u03B4';

    /// <summary>
    /// 
    /// </summary>
    public string MapToText(CalculateResult calculateResult)
    {
        var builder = new StringBuilder(100);

        builder.Append("U  | ");
        foreach (var point in calculateResult.Points)
        {
            builder.Append($"{point.X} | ");
        }
        builder.Append("\n");

        for (int i = 0; i < calculateResult.Points.Count * 6; i++) builder.Append('-');
        builder.Append('\n');

        builder.Append("I   | ");
        foreach (var point in calculateResult.Points)
        {
            builder.Append($"{(point.X < 0 ? " " : "")}{point.Y} | ");
        }
        builder.Append("\n\n");

        for (int i = 0; i < calculateResult.Equations.Count; i++)
        {
            builder.Append($"{_delta}{_subscriptNumber[i + 1]}{_superscriptTwo} = ({calculateResult.Points[i].Y} - [a{_subscriptNumber[0]} + a{_subscriptNumber[1]}{calculateResult.Points[i].X}]){_superscriptTwo} = ");
            builder.Append('\n');
            builder.Append("    ");
            for (int j = 0; j < _mask.Length; j++)
            {
                builder.Append($"{(calculateResult.Equations[i][j] >= 0 && j != 0 ? "+" : "")} {calculateResult.Equations[i][j]}{_mask[j]} ");
            }
            builder.Append("\n\n");
        }

        builder.Append($"{_capLetterSigma}{_delta}{_superscriptTwo} = ");
        for (int i = 0; i < _mask.Length; i++)
        {
            builder.Append($"{(calculateResult.Sum[i] >= 0 && i != 0 ? "+" : "")} {calculateResult.Sum[i]}{_mask[i]} ");
        }
        builder.Append("\n\n");

        for (int i = 0; i < calculateResult.Derivatives.Count; i++)
        {
            builder.Append($"{_letterDelta}{_capLetterSigma}{_delta} / {_letterDelta}a{_subscriptNumber[i]} = ");
            builder.Append($"{calculateResult.Derivatives[i][0]} + {calculateResult.Derivatives[i][1]}a{_subscriptNumber[0]} + {calculateResult.Derivatives[i][2]}a{_subscriptNumber[1]}");
            builder.Append("\n\n");
        }

        builder.Append($"I = {calculateResult.Result.B} + {calculateResult.Result.K}U");

        return builder.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    public DrowRequest MapToDrowRequest(CalculateResult calculateResult)
    {
        double func(double x) => calculateResult.Result.K * x + calculateResult.Result.B;

        return new DrowRequest
        {
            Points = [calculateResult.Points],
            Functions = [func]
        };
    }
}