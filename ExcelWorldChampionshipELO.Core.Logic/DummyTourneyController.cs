using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Generation;
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

        stopwatch.Stop();
        Console.WriteLine($"TimeTaken: {stopwatch.ElapsedMilliseconds}ms");
    }
}
