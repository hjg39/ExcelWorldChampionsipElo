namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Game
{
    public required Guid GameId { get; init; }

    public required string Name { get; init; }

    public required double MaxScore { get; init; }
}
