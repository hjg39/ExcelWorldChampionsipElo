using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Parsing;
using ExcelWorldChampionshipELO.Core.Visualisation;
using System.Diagnostics;

namespace ExcelWorldChampionshipELO.Core.Logic;

public sealed class InputTourneyController
{
    public static void RunTourney(TourneyInputs input)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        Tourney tourney = DocumentParser.GetTourney(input);
        EloCalculator.CalculateElos(tourney, 64);
        EloPlotter.PlotTopElos(tourney);

        stopwatch.Stop();
        Console.WriteLine($"TimeTaken: {stopwatch.ElapsedMilliseconds}ms");
    }
}