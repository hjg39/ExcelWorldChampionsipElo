namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Game
{
    public required Guid GameId { get; init; }

    public required string Name { get; init; }

    public required double GameNumber { get; init; } 

    public required double MaxScore { get; init; }

    public required string? Author { get; init; }

    public required string? Description { get; init; }

    public double? Difficulty { get; set; }

    public int DifficultyRank { get; set; }
}
