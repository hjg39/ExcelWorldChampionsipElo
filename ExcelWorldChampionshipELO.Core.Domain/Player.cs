using ExcelWorldChampionshipELO.Core.Common;

namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Player
{
    public required Guid PlayerId { get; init; }

    public required string Name { get; init; }

    public required string Country { get; init; }

    public required Dictionary<Guid, GameResult> GameScores { get; init; }

    public required Dictionary<double, double> EloScores { get; init; }

    public double SeededSkill { get; set; }

    public double ScoreAvg => GameScores.Average(x => x.Value.Score);

    public double ScoreMax => GameScores.Max(x => x.Value.Score);

    public double ScoreMin => GameScores.Min(x => x.Value.Score);

    public double EloMin => EloScores.Count != 0 ? EloScores.Min(x => x.Value) : Constants.DefaultElo;

    public double EloMax => EloScores.Count != 0 ? EloScores.Max(x => x.Value) : Constants.DefaultElo;

    public double EloLatest => EloScores.Count != 0 ? EloScores.MaxBy(x => x.Key).Value : Constants.DefaultElo;
}