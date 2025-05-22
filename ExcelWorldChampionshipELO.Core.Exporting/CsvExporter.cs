using CsvHelper;
using ExcelWorldChampionshipELO.Core.Domain;
using System.Globalization;
using System;
using ExcelWorldChampionshipELO.Core.Common;

namespace ExcelWorldChampionshipELO.Core.Exporting;

public static class CsvExporter
{
    public static void ExportPlayerResults(Tourney tourney)
    {
        using StreamWriter writer = new(CreateDesktopPath($"Elo results-{tourney.Name}-{tourney.TourneyId}.csv"));
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

        csv.WriteField("Name");

        foreach (Game game in tourney.Games)
        {
            csv.WriteField(game.Name);
        }

        csv.NextRecord();

        foreach (Player player in tourney.Players)
        {
            csv.WriteField(player.Name);

            double lastElo = Constants.DefaultElo;

            foreach (Game game in tourney.Games)
            {
                if (player.EloScores.TryGetValue(game.GameNumber, out double newLastElo))
                {
                    lastElo = newLastElo;
                }

                csv.WriteField(lastElo);
            }
            csv.NextRecord();
        }
    }

    public static void ExportGameDifficulties(Tourney tourney)
    {
        using StreamWriter writer = new(CreateDesktopPath($"Game difficulties-{tourney.Name}-{tourney.TourneyId}.csv"));
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

        csv.WriteField("Name");

        foreach (Game game in tourney.Games)
        {
            csv.WriteField(game.Name);
        }

        csv.NextRecord();

        csv.WriteField("Score");

        foreach (Game game in tourney.Games)
        {
            csv.WriteField(game.Difficulty);
        }
    }

    private static string CreateDesktopPath(string name)
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), name);
}
