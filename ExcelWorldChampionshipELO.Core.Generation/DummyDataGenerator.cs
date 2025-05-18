using ExcelWorldChampionshipELO.Core.Common;
using ExcelWorldChampionshipELO.Core.Domain;

namespace ExcelWorldChampionshipELO.Core.Generation;

public sealed class DummyDataGenerator(DummyTourneyInputs? input = null)
{
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly Random _random = input?.Seed is int seedInt ? new Random(seedInt) : new Random();

    private readonly int _numberOfGames = input?.NumberOfGames ?? 500;

    private readonly int _numberOfPlayers = input?.NumberOfPlayers ?? 100;

    private readonly int _nameLength = input?.NameLength ?? 5;

    private readonly double _probabilityPlayerPlaysRound = input?.ProbabilityPlayerPlaysRound ?? 0.9;

    public Tourney GenerateTournament()
    {
        ValidateInputs();

        Game[] games = new Game[_numberOfGames];

        for (int i = 0; i < _numberOfGames; i++)
        {
            games[i] = GenerateGame();
        }

        DateTime startingEloDate = games.Min(x => x.DateTime).AddDays(-1);

        Player[] players = new Player[_numberOfPlayers];

        for (int i = 0; i < _numberOfPlayers; i++)
        {
            players[i] = GeneratePlayer(games, startingEloDate);
        }

        foreach (Player player in players)
        {
            foreach (GameResult gameResult in player.GameScores.Values)
            {
                gameResult.Player = player;
            }
        }

        return new()
        {
            Games = games,
            Players = players,
        };
    }


    private Player GeneratePlayer(Game[] games, DateTime startingEloDate)
    {
        Dictionary<Guid, GameResult> playerResults = [];
            
        double skillLevel = _random.NextDouble() * 0.5;

        foreach (Game game in games)
        {
            if (_random.NextDouble() > _probabilityPlayerPlaysRound)
            {
                continue;
            }

            double percentageScore = 0;

            while (percentageScore < skillLevel || percentageScore > skillLevel * 2)
            {
                percentageScore = _random.NextDouble();
            }

            playerResults[game.GameId] = new GameResult()
            {
                Score = (int)(percentageScore * game.MaxScore),
                SecondaryScore = 0,
                Game = game,
            };
        }

        return new()
        {
            PlayerId = Guid.NewGuid(),
            Name = GetRandomString(),
            GameScores = playerResults,
            EloScores = new() { { startingEloDate, Constants.DefaultElo } },
            SeededSkill = skillLevel,
        };
    }

    private Game GenerateGame()
    => new()
    {
        GameId = Guid.NewGuid(),
        Name = GetRandomString(),
        MaxScore = _random.Next(3, 6) * 250,
        DateTime = DateTime.UtcNow.AddDays(_random.Next(0, 366)).AddMinutes(_random.Next(0, 1440)),
    };

    private void ValidateInputs()
    {
        if (_numberOfGames <= 1)
        {
            throw new Exception("Not enough games.");
        }

        if (_nameLength < 3)
        {
            throw new Exception("Names are too short.");
        }

        if (_numberOfPlayers < 2)
        {
            throw new Exception("Not enough players.");
        }

        if (_probabilityPlayerPlaysRound < 0 || _probabilityPlayerPlaysRound > 1)
        {
            throw new Exception("Invalid probability players play round.");
        }
    }

    private string GetRandomString() => new(Enumerable.Repeat(_chars, _nameLength).Select(s => s[_random.Next(s.Length)]).ToArray());
}