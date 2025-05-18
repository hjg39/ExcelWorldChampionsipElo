using ExcelWorldChampionshipELO.Core.Domain;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;

namespace ExcelWorldChampionshipELO.Core.Visualisation;

public static class EloPlotter
{
    public static void PlotTopElos(Tourney tourney)
    {
        Plot eloPlot = new();

        Player[] orderedPlayers = [.. tourney.Players.OrderByDescending(x => x.EloLatest)];


        foreach (Player player in orderedPlayers)
        {
            Scatter scatter = eloPlot.Add.Scatter(player.EloScores.Keys.ToArray(), player.EloScores.Values.ToArray());
            scatter.LegendText = player.Name;
        }

        eloPlot.XLabel("Date");
        eloPlot.YLabel("Elo");

        IXAxis xAxis = eloPlot.Axes.GetXAxes().First();
        xAxis.TickGenerator = new DateTimeAutomatic()
        {
            LabelFormatter = DateTimeFormatter,
        };


        eloPlot.Title($"{tourney.Name}-{tourney.TourneyId}");

        eloPlot.SavePng(@$"C:\Users\harry\Downloads\RandomTourney-{tourney.TourneyId}.png", 1200, 1200);
    }

    public static string DateTimeFormatter(DateTime dateTime)
        => dateTime.ToString("d MMM yyyy");
}
