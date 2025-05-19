namespace ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;

public sealed class TourneyInputs
{
    public required string Name { get; init; }

    public required string GameDataPath { get; init; }

    public required string PlayerDataPath { get; init; }
}
