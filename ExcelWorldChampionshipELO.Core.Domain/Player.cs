using ExcelWorldChampionshipELO.Core.Common;
using System.Linq;

namespace ExcelWorldChampionshipELO.Core.Domain;

public sealed class Player
{
    public required Guid PlayerId { get; init; }

    public required string Name { get; init; }

    public required string Country { get; init; }

    public required Dictionary<Guid, GameResult> GameScores { get; init; }

    public required Dictionary<double, double> EloScores { get; init; }

    public double SeededSkill { get; set; }

    public required Dictionary<double, int> WorldRankings { get; set; }

    public double ScoreAvg => GameScores.Average(x => x.Value.Score);

    public double ScoreMax => GameScores.Max(x => x.Value.Score);

    public double ScoreMin => GameScores.Min(x => x.Value.Score);

    public double EloMin => EloScores.Count != 0 ? EloScores.Where(x => x.Value != Constants.DefaultElo).Min(x => x.Value) : Constants.DefaultElo;

    public double EloMax => EloScores.Count != 0 ? EloScores.Where(x => x.Value != Constants.DefaultElo).Max(x => x.Value) : Constants.DefaultElo;

    public double EloLatest => EloScores.Count != 0 ? EloScores.MaxBy(x => x.Key).Value : Constants.DefaultElo;

    public int WorldRankingMin => WorldRankings.Count != 0 ? WorldRankings.Min(x => x.Value) : 0;

    public int WorldRankingMax => WorldRankings.Count != 0 ? WorldRankings.Max(x => x.Value) : 0;

    public int WorldRankingLatest => WorldRankings.Count != 0 ? WorldRankings.MaxBy(x => x.Key).Value : 0;

    public double LastEloBeforeGameNumber(double time)
    {
        IEnumerable<KeyValuePair<double, double>> releveantScores = EloScores.Where(x => x.Key < time);

        if (!releveantScores.Any())
        {
            return Constants.DefaultElo;
        }

        return releveantScores.MaxBy(x => x.Key).Value;
    }

    public Game FirstGamePlayed(Tourney tourney)
        => tourney.Games.Where(x => GameScores.ContainsKey(x.GameId)).MinBy(x => x.GameNumber)!;

    public Game LastGamePlayed(Tourney tourney)
        => tourney.Games.Where(x => GameScores.ContainsKey(x.GameId)).MaxBy(x => x.GameNumber)!;

    public Game GameWhereEloAchieved(double elo, Tourney tourney)
        => tourney.Games.Where(predicate: x => x.GameNumber == EloScores.Where(x => x.Value == elo).Last().Key).OrderBy(x => x.GameNumber).Last();


    public Game GameWhereWorldRankingAchieved(int ranking, Tourney tourney, bool preferFirst = false)
    {
        if (preferFirst)
        {
            return tourney.Games.Where(predicate: x => x.GameNumber == WorldRankings.Where(x => x.Value == ranking).First().Key).OrderBy(x => x.GameNumber).Last();
        }
        else
        {
            return tourney.Games.Where(predicate: x => x.GameNumber == WorldRankings.Where(x => x.Value == ranking).Last().Key).OrderBy(x => x.GameNumber).Last();
        }
    }
}