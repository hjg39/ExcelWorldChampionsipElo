namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Tourney
{
    public required Guid TourneyId { get; init; }

    public required string Name { get; init; }

    public required Game[] Games { get; init; }

    public required Player[] Players { get; init; }
}