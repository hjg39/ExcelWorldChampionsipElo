using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Generation;
using ExcelWorldChampionshipELO.Core.Visualisation;
using System.Diagnostics;

namespace ExcelWorldChampionshipELO.Core.Logic;

public sealed class DummyTourneyController
{
    public static void RunTourney(DummyTourneyInputs? input = null)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        DummyDataGenerator dummyDataGenerator = new(input);
        Tourney tourney = dummyDataGenerator.GenerateTournament();
        EloCalculator.CalculateElos(tourney, 32);
        EloPlotter.PlotTopElos(tourney);
        EloPlotter.PlotGameDifficulty(tourney);
        EloPlotter.PlotFinalElos(tourney);

        stopwatch.Stop();
        Debug.WriteLine($"TimeTaken: {stopwatch.ElapsedMilliseconds}ms");
    }
}
