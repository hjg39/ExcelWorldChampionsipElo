using CsvHelper;
using ExcelWorldChampionshipELO.Core.Common;
using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Domain.Parsing;
using ExcelWorldChampionshipELO.Core.Domain.Parsing.Mapping;
using System.Globalization;

namespace ExcelWorldChampionshipELO.Core.Parsing;

public static class DocumentParser
{
    public static Tourney GetTourney(TourneyInputs tourneyInputs)
    {
        GameInput[] gameInputs = GetTourneyGames(tourneyInputs.GameDataPath);
        Game[] games = GetGames(gameInputs);

        PlayerInput[] playerInputs = GetTourneyPlayers(tourneyInputs.PlayerDataPath);
        Player[] players = GetPlayers(playerInputs, games);

        return new()
        {
            TourneyId = Guid.NewGuid(),
            Name = tourneyInputs.Name,
            Games = games,
            Players = players,
        };
    }

    private static Player[] GetPlayers(PlayerInput[] playerInputs, Game[] games)
    {
        IGrouping<string, PlayerInput>[] playerInputsByPlayerName = playerInputs.GroupBy(x => x.PlayerName).ToArray();

        int numberOfPlayers = playerInputsByPlayerName.Length;

        Player[] result = new Player[numberOfPlayers];


        var v = games.ToDictionary(x => x.GameNumber, x => x);
        double startingInputTime = games.Min(x => x.GameNumber) - 1d;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            IGrouping<string, PlayerInput> playerInputGroup = playerInputsByPlayerName[i];
            Player player = GetPlayer([.. playerInputGroup], games, startingInputTime);

            foreach (GameResult gameResult in player.GameScores.Values)
            {
                gameResult.Player = player;
            }

            result[i] = player;
        }

        return result;
    }
    private static Game GetGame(GameInput gameInput)
    => new()
    {
        GameId = Guid.NewGuid(),
        Name = gameInput.GameName,
        GameNumber = gameInput.GameNumber,
        MaxScore = gameInput.MaxPoints,
        Author = gameInput.Author,
        Description = gameInput.GameDescription,
    };

    private static Player GetPlayer(PlayerInput[] playerInputs, Game[] games, double startingInputTime)
    {
        PlayerInput lastInput = playerInputs.Last();

        Dictionary<Guid, GameResult> gameScores = [];

        foreach (PlayerInput playerInput in playerInputs)
        {
            Game game = games.First(x => x.GameNumber == playerInput.GameNumber);

            gameScores[game.GameId] = new()
            {
                Game = game,
                Score = playerInput.Score,
                SecondaryScore = playerInput.SecondaryScore,
            };
        }

        return new()
        {
            PlayerId = Guid.NewGuid(),
            Name = lastInput.PlayerName,
            Country = lastInput.Country,
            EloScores = new() { { startingInputTime, Constants.DefaultElo } },
            GameScores = gameScores,
            WorldRankings = [],
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


    private static PlayerInput[] GetTourneyPlayers(string csvPath)
    {
        using StreamReader reader = new(csvPath);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<PlayerInputMap>();

        return [.. csv.GetRecords<PlayerInput>()];
    }

    private static GameInput[] GetTourneyGames(string csvPath)
    {
        using StreamReader reader = new(csvPath);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<GameInputMap>();

        return [.. csv.GetRecords<GameInput>()];
    }
}