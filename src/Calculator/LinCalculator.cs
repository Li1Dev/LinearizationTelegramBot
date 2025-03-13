using Lin.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Lin.Calculator;

public class LinCalculator : ILinCalculator
{
    public Task<MemoryStream> DrowGraphAsync(DrowRequest drowRequest, CancellationToken ct)
    {
        int width = 500, height = 300;
        int size = 40;
        using var image = new Image<Rgba32>(width, height, Color.White);

        for (int i = 300 - 20 - size; i > 0; i -= size)
            for (int j = 0; j < width; j++) image[j, i] = Color.Gray;

        for (int i = 20 + size * 2; i < width; i += size)
            for (int j = 0; j < height; j++) image[i, j] = Color.Gray;

        for (int i = 0; i < width; i++) image[i, 300 - 20] = Color.Black;
        for (int i = 0; i < height; i++) image[20 + size, i] = Color.Black;

        for (int i = 0; i < drowRequest.Points.Count; i++)
        {
            for (int j = 0; j < drowRequest.Points[i].Count; j++)
            {
                for (int m = height - 20 - drowRequest.Points[i][j].Y * size - 1; m < height - 20 - drowRequest.Points[i][j].Y * size + 2; m++)
                    for (int n = 20 + size + drowRequest.Points[i][j].X * size - 3; n < 20 + size + drowRequest.Points[i][j].X * size + 4; n++)
                        image[n, m] = Color.Red;

                for (int n = 20 + size + drowRequest.Points[i][j].X * size - 1; n < 20 + size + drowRequest.Points[i][j].X * size + 2; n++)
                    for (int m = height - 20 - drowRequest.Points[i][j].Y * size - 3; m < height - 20 - drowRequest.Points[i][j].Y * size + 4; m++)
                        image[n, m] = Color.Red;

                image[20 + size + drowRequest.Points[i][j].X * size + 2, height - 20 - drowRequest.Points[i][j].Y * size + 2] = Color.Red;
                image[20 + size + drowRequest.Points[i][j].X * size - 2, height - 20 - drowRequest.Points[i][j].Y * size - 2] = Color.Red;
                image[20 + size + drowRequest.Points[i][j].X * size + 2, height - 20 - drowRequest.Points[i][j].Y * size - 2] = Color.Red;
                image[20 + size + drowRequest.Points[i][j].X * size - 2, height - 20 - drowRequest.Points[i][j].Y * size + 2] = Color.Red;
            }
        }

        var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult(memoryStream);
    }

    public async Task<CalculateResult> CalculateLinResultAsync(IReadOnlyList<System.Drawing.Point> points, CancellationToken ct)
    {
        var equations = GetEquations(points);
        var sum = GetEquationsSum(equations);
        var derivatives = GetDerivatives(sum);
        var result = GetResult(derivatives);

        var res = new CalculateResult
        {
            Points = points,
            Equations = equations,
            Sum = sum,
            Derivatives = derivatives,
            Result = result
        };

        return await Task.FromResult(res);
    }

    private static IReadOnlyList<IReadOnlyList<double>> GetEquations(IReadOnlyList<System.Drawing.Point> points)
    {
        var equations = new List<List<double>>(points.Count);

        for (int i = 0; i < points.Count; i++)
        {

            List<double> squareError =
            [
                Math.Pow(points[i].Y, 2),
                -(points[i].Y * 2),
                -(points[i].Y * 2 * points[i].X),
                1,
                points[i].X * 2,
                Math.Pow(points[i].X, 2),
            ];

            equations.Add(squareError);
        }

        return equations;
    }

    private static IReadOnlyList<double> GetEquationsSum(IReadOnlyList<IReadOnlyList<double>> equations)
    {
        var sum = new List<double>(new double[equations[0].Count]);

        for (int i = 0; i < equations[0].Count; i++)
        {
            for (int j = 0; j < equations.Count; j++)
            {
                sum[i] += equations[j][i];
            }
        }

        return sum;
    }

    private static IReadOnlyList<IReadOnlyList<double>> GetDerivatives(IReadOnlyList<double> sum)
    {
        List<double> firstDerivativ = [sum[1], sum[3] * 2, sum[4]];
        List<double> secondDerivativ = [sum[2], sum[4], sum[5] * 2];

        return [firstDerivativ, secondDerivativ];
    }

    private static (double k, double b) GetResult(IReadOnlyList<IReadOnlyList<double>> derivatives)
    {
        double k = -((derivatives[1][0] * derivatives[0][1] - derivatives[1][1] * derivatives[0][0])
                    / (derivatives[1][2] * derivatives[0][1] - derivatives[1][1] * derivatives[0][2]));

        double b = (-derivatives[0][0] - derivatives[0][2] * k) / derivatives[0][1];

        return (k, b);
    }
}