namespace ExcelWorldChampionshipELO.Core.Domain.Parsing;

public sealed record PlayerInput
{
    public required double GameNumber { get; init; }

    public required string PlayerName { get; init; }

    public required string Country { get; init; }

    public required double Score { get; init; }

    public required double SecondaryScore { get; init; }
}
