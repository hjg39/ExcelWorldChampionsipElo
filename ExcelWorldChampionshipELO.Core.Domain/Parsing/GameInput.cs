namespace ExcelWorldChampionshipELO.Core.Domain.Parsing;

public sealed record GameInput
{
    public required int GameNumber { get; init; }

    public required string? GameDescription { get; init; }

    public required string GameName { get; init; }

    public required string? Author { get; init; }

    public required double MaxPoints { get; init; }
}
