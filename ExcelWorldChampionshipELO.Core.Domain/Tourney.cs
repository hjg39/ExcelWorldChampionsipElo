namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Tourney
{
    public required Game[] Games { get; init; }

    public required Player[] Players { get; init; }
}