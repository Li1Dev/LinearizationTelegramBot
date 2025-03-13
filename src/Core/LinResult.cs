namespace Lin.Core;

public record LinResult
{
    public required string Solving { get; init; }

    public required MemoryStream Graph { get; init; }
}