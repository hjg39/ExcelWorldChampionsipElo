﻿using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Parsing;
using ExcelWorldChampionshipELO.Core.Storage;
using ExcelWorldChampionshipELO.Core.Visualisation;
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

    public static void PlotTourney(Tourney tourney)
    {
        EloPlotter.PlotTopElos(tourney);
        EloPlotter.PlotTopRankings(tourney);
        EloPlotter.PlotGameDifficulty(tourney);
        EloPlotter.PlotFinalElos(tourney);
    }

    public static void PlotPlayers(Player mainPlayer, Player[] players, Tourney tourney)
    {
        EloPlotter.PlotPlayerElos(mainPlayer, players, tourney);
        EloPlotter.PlotPlayerRankings(mainPlayer, players, tourney);
    }
}