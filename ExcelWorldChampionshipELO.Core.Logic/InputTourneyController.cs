using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Parsing;
using ExcelWorldChampionshipELO.Core.Storage;
using System.Diagnostics;

namespace ExcelWorldChampionshipELO.Core.Logic;

public sealed class InputTourneyController
{
    public static void RunTourney(TourneyInputs input)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Tourney tourney = DocumentParser.GetTourney(input);
            EloCalculator.CalculateElos(tourney, 64);

            TourneyStorage.LastRunTourney = tourney;
        }
        finally
        {
            Debug.WriteLine($"TimeTaken: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}