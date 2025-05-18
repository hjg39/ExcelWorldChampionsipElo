namespace ExcelWorldChampionshipELO.Core.Domain;

public readonly struct GameResult
{
    public required double Score { get; init; }

    public required double SecondaryScore { get; init; }
}
