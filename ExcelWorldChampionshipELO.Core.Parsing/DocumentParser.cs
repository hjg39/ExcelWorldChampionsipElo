using CsvHelper;
using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.Parsing;
using ExcelWorldChampionshipELO.Core.Domain.Parsing.Mapping;
using System.Globalization;

namespace ExcelWorldChampionshipELO.Core.Parsing;

public static class DocumentParser
{
    public static Tourney GetTourney(string tourneyName, string gamesCsvPath, string playersCsvPath)
    {
        GameInput[] gameInputs = GetTourneyGames(gamesCsvPath);
        Game[] games = GetGames(gameInputs);

        return new()
        {
            TourneyId = Guid.NewGuid(),
            Name = tourneyName,
            Games = games,
        };
    }

    private static Game[] GetGames(GameInput[] gameInputs)
    {
        int numberOfGames = gameInputs.Length;

        Game[] result = new Game[numberOfGames];

        for (int i = 0; i < numberOfGames; i++)
        {
            GameInput gameInput = gameInputs[i];

            result[i] = GetGame(gameInput);
        }

        return result;
    }

    private static Game GetGame(GameInput gameInput)
    => new()
    {
        GameId = Guid.NewGuid(),
        Name = gameInput.GameName,
        Order = gameInput.GameNumber,
        MaxScore = gameInput.MaxPoints,
        Author = gameInput.Author,
        Description = gameInput.GameDescription,
    };

    private static GameInput[] GetTourneyGames(string csvPath)
    {
        using StreamReader reader = new(csvPath);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<GameInputMap>();

        return [.. csv.GetRecords<GameInput>()];
    }
}