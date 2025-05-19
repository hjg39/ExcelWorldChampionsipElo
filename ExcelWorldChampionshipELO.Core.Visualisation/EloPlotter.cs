using ExcelWorldChampionshipELO.Core.Domain;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System.Collections.Concurrent;

namespace ExcelWorldChampionshipELO.Core.Visualisation;

public static class EloPlotter
{
    private static ConcurrentDictionary<Guid, Dictionary<double, string>> _gameNames = new();

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

        // eloPlot.XLabel("Date");
        eloPlot.YLabel("Elo");

        IXAxis xAxis = eloPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new NumericManual([..gameData.Keys], gameData.Values.ToArray());
        xAxis.TickLabelStyle.Rotation = 90;
        xAxis.TickLabelStyle.Alignment = Alignment.LowerLeft;
        xAxis.MinimumSize = 300;

        eloPlot.Legend.Alignment = Alignment.UpperLeft;

        eloPlot.Title($"{tourney.Name}-{tourney.TourneyId}");

        eloPlot.SavePng(@$"C:\Users\harry\Downloads\{tourney.Name}-{tourney.TourneyId}.png", 1800, 1200);
    }

    public static string DoubleFormatter(double gameTime, Guid tourneyId)
    {
        Dictionary<double, string> caseNamesByTime = _gameNames[tourneyId];

        if (!(caseNamesByTime.TryGetValue(gameTime, out string? result)))
        {
            return string.Empty;
        }

        return result;
    }
}
