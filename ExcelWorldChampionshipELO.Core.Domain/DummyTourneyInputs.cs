namespace ExcelWorldChampionshipELO.Core.Domain;

public readonly struct DummyTourneyInputs
{
    public int? Seed { get; init; }
    
    public int? NumberOfGames { get; init; }

    public int? NumberOfPlayers { get; init; }

    public int? NameLength { get; init; }

    public double? ProbabilityPlayerPlaysRound { get; init; }
}