namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Player
{
    public required Guid PlayerId { get; init; }

    public required string Name { get; init; }

    public required Dictionary<Guid, GameResult> GameScores { get; init; }

    public double SeededSkill { get; set; }

    public double AverageScore => GameScores.Average(x => x.Value.Score);

    public double MaximumScore => GameScores.Max(x => x.Value.Score);

    public double MinimumScore => GameScores.Min(x => x.Value.Score);
}