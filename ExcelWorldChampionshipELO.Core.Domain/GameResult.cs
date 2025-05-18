namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class GameResult
{
    public required double Score { get; init; }

    public required double SecondaryScore { get; init; }

    public required Game Game { get; init; }

    public Player? Player { get; set; }
}
