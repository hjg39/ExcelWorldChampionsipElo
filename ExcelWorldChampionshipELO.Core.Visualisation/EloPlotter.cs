using ExcelWorldChampionshipELO.Core.Domain;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System.Collections.Concurrent;

namespace ExcelWorldChampionshipELO.Core.Visualisation;

public static class EloPlotter
{
    private static readonly ConcurrentDictionary<Guid, Dictionary<double, string>> _gameNames = new();

    public static void PlotFinalElos(Tourney tourney)
    {
        Plot finalElo = new();

        Player[] orderedPlayers = [.. tourney.Players.Where(x => x.GameScores.Count >= 5).OrderBy(x => x.EloLatest)];

        double[] xAxisValues = [.. Enumerable.Range(0, orderedPlayers.Length).Select(x => (double)x)];

        finalElo.Add.Bars(xAxisValues, orderedPlayers.Select(x => x.EloLatest));

        finalElo.YLabel("Elo");

        IXAxis xAxis = finalElo.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual(xAxisValues, orderedPlayers.Select(x => x.Name).ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        finalElo.Title($"{tourney.Name}-Final Elos-{tourney.TourneyId}");
        finalElo.SavePng(CreateDesktopPath(@$"{tourney.Name}-Final Elos-{tourney.TourneyId}.png"), 5000, 1200);
    }

    public static void PlotGameDifficulty(Tourney tourney)
    {
        Plot gameDifficultyPlot = new();

        Game[] orderedGames = [.. tourney.Games.Where(x => x.Difficulty is double d).OrderBy(x => x.Difficulty)];

        gameDifficultyPlot.Add.Bars(orderedGames.Select(x => (double)x.DifficultyRank).ToArray(), orderedGames.Select(x => x.Difficulty));

        gameDifficultyPlot.YLabel("Difficulty");

        IXAxis xAxis = gameDifficultyPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([.. orderedGames.Select(x => x.DifficultyRank)], orderedGames.Select(x => x.Name).ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        gameDifficultyPlot.Title($"{tourney.Name}-Game Difficulties-{tourney.TourneyId}");
        gameDifficultyPlot.SavePng(CreateDesktopPath(@$"{tourney.Name}-Game Difficulties-{tourney.TourneyId}.png"), 1800, 1200);
    }

    public static void PlotTopElos(Tourney tourney)
    {
        Plot eloPlot = new();

        Player[] orderedPlayers = [.. tourney.Players.OrderByDescending(x => x.EloLatest)];

        Dictionary<double, string> gameData = tourney.Games.ToDictionary(x => x.GameNumber, x => x.Name);
        double minTimelineValue = tourney.Players.SelectMany(x => x.EloScores.Keys).Min();
        gameData[minTimelineValue] = "Start";
        _gameNames[tourney.TourneyId] = new Dictionary<double, string>(gameData);

        foreach (Player player in orderedPlayers.Take(20))
        {
            Scatter scatter = eloPlot.Add.Scatter([.. player.EloScores.Keys], player.EloScores.Values.ToArray());
            scatter.LegendText = player.Name;
        }

        eloPlot.YLabel("Elo");

        IXAxis xAxis = eloPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([..gameData.Keys], gameData.Values.ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        eloPlot.Legend.Alignment = Alignment.UpperLeft;

        eloPlot.Title($"Top20-Elos-{tourney.Name}-{tourney.TourneyId}");
        eloPlot.SavePng(CreateDesktopPath(@$"Top20-Elos-{tourney.Name}-{tourney.TourneyId}.png"), 1800, 1200);
    }

    public static void PlotTopRankings(Tourney tourney)
    {
        Plot worldRankingPlot = new();

        Player[] orderedPlayers = [.. tourney.Players.OrderBy(x => x.WorldRankingLatest)];

        Dictionary<double, string> gameData = tourney.Games.ToDictionary(x => x.GameNumber, x => x.Name);
        double minTimelineValue = tourney.Players.SelectMany(x => x.WorldRankings.Keys).Min();
        gameData[minTimelineValue] = "Start";
        _gameNames[tourney.TourneyId] = new Dictionary<double, string>(gameData);

        foreach (Player player in orderedPlayers.Take(20))
        {
            Scatter scatter = worldRankingPlot.Add.Scatter([.. player.WorldRankings.Keys], player.WorldRankings.Values.ToArray());
            scatter.LegendText = player.Name;
        }

        worldRankingPlot.YLabel("World Ranking");

        worldRankingPlot.Axes.AutoScaler.InvertedY = true;
        IYAxis yAxis = worldRankingPlot.Axes.GetYAxes().First();

        yAxis.Max = 0;
        yAxis.Min = 30;

        IXAxis xAxis = worldRankingPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([.. gameData.Keys], gameData.Values.ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        worldRankingPlot.Legend.Alignment = Alignment.UpperLeft;

        worldRankingPlot.Title($"Top20-WorldRankings-{tourney.Name}-{tourney.TourneyId}");
        worldRankingPlot.SavePng(CreateDesktopPath(@$"Top20-WorldRankings-{tourney.Name}-{tourney.TourneyId}.png"), 1800, 1200);
    }

    public static void PlotPlayerElos(Player mainPlayer, Player[] players, Tourney tourney)
    {
        Plot eloPlot = new();

        Player[] orderedPlayers = [.. players.OrderByDescending(x => x.EloLatest)];

        Dictionary<double, string> gameData = tourney.Games.ToDictionary(x => x.GameNumber, x => x.Name);
        double minTimelineValue = players.SelectMany(x => x.EloScores.Keys).Min();
        gameData[minTimelineValue] = "Start";
        _gameNames[tourney.TourneyId] = new Dictionary<double, string>(gameData);

        foreach (Player player in orderedPlayers)
        {
            Scatter scatter = eloPlot.Add.Scatter([.. player.EloScores.Keys], player.EloScores.Values.ToArray());
            scatter.LegendText = player.Name;
        }

        eloPlot.YLabel("Elo");

        IXAxis xAxis = eloPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([.. gameData.Keys], gameData.Values.ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        eloPlot.Legend.Alignment = Alignment.UpperLeft;

        eloPlot.Title($"{mainPlayer.Name} and rivals-Elos-{tourney.Name}-{tourney.TourneyId}");
        eloPlot.SavePng(CreateDesktopPath(@$"{mainPlayer.Name} and rivals-Elos-{tourney.Name}-{tourney.TourneyId}.png"), 1800, 1200);
    }

    public static void PlotPlayerRankings(Player mainPlayer, Player[] players, Tourney tourney)
    {
        Plot worldRankingPlot = new();

        Player[] orderedPlayers = [.. players.OrderBy(x => x.WorldRankingLatest)];

        Dictionary<double, string> gameData = tourney.Games.ToDictionary(x => x.GameNumber, x => x.Name);
        double minTimelineValue = players.SelectMany(x => x.WorldRankings.Keys).Min();
        gameData[minTimelineValue] = "Start";
        _gameNames[tourney.TourneyId] = new Dictionary<double, string>(gameData);

        foreach (Player player in orderedPlayers)
        {
            Scatter scatter = worldRankingPlot.Add.Scatter([.. player.WorldRankings.Keys], player.WorldRankings.Values.ToArray());
            scatter.LegendText = player.Name;
        }

        IYAxis yAxis = worldRankingPlot.Axes.GetYAxes().First();

        yAxis.Max = mainPlayer.WorldRankingLatest - 20;
        yAxis.Min = mainPlayer.WorldRankingLatest + 20;

        worldRankingPlot.YLabel("World Ranking");

        worldRankingPlot.Axes.AutoScaler.InvertedY = true;

        IXAxis xAxis = worldRankingPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([.. gameData.Keys], gameData.Values.ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        worldRankingPlot.Legend.Alignment = Alignment.UpperLeft;

        worldRankingPlot.Title($"{mainPlayer.Name} and rivals-WorldRankings-{tourney.Name}-{tourney.TourneyId}");
        worldRankingPlot.SavePng(CreateDesktopPath(@$"{mainPlayer.Name} and rivals-WorldRankings-{tourney.Name}-{tourney.TourneyId}.png"), 1800, 1200);
    }

    private static string CreateDesktopPath(string name)
    => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), name);
}
