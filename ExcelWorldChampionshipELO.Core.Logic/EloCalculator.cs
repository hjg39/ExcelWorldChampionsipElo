using ExcelWorldChampionshipELO.Core.Domain;

namespace ExcelWorldChampionshipELO.Core.Logic;

public static class EloCalculator
{
    public static void CalculateElos(Tourney tourney, double maxAdjustmentPerGame)
    {
        Game[] gamesInOrder = [.. tourney.Games.OrderBy(x => x.GameNumber)];

        int i = 1;
        foreach (Game game in gamesInOrder)
        {
            UpdatePlayerElos(game, tourney.Players, maxAdjustmentPerGame);
            CalculateGameDifficulty(game, tourney.Players);
            Console.WriteLine($"Game {i++} of {gamesInOrder.Length} ({game.Name}) completed, EloMax: {tourney.Players.Max(x => x.EloLatest)}, EloMin: {tourney.Players.Min(x => x.EloLatest)}");
        }

        Game[] gamesRankedByDifficulty = [.. gamesInOrder.Where(x => x.Difficulty is not null).OrderBy(x => x.Difficulty)];

        for (int j = 0; j < gamesRankedByDifficulty.Length; j++)
        {
            gamesRankedByDifficulty[j].DifficultyRank = j;
        }
    }

    private static void CalculateGameDifficulty(Game game, Player[] players)
    {
        Player[] relevantPlayers = [.. players.Where(x => x.EloScores.ContainsKey(game.GameNumber) && x.EloScores.Count >= 5)];

        List<double> estimatedEloBasedOnSinglePlayer = [];

        foreach (Player player in relevantPlayers)
        {
            double elo = player.LastEloBeforeGameNumber(game.GameNumber);
            double percentScore = player.GameScores[game.GameId].Score / game.MaxScore;

            estimatedEloBasedOnSinglePlayer.Add(elo * 2 * (1 - percentScore));
        }

        game.Difficulty = estimatedEloBasedOnSinglePlayer.Count != 0 ? estimatedEloBasedOnSinglePlayer.Average() : null;
    }

    private static void UpdatePlayerElos(Game game, Player[] players, double maxAdjustmentPerGame)
    {
        Guid gameId = game.GameId;

        List<GameResult> gameResults = [];

        foreach (Player player in players)
        {
            if (player.GameScores.TryGetValue(gameId, out GameResult? result))
            {
                gameResults.Add(result);
            }
        }

        UpdateElos([.. gameResults], maxAdjustmentPerGame);
    }

    private static void UpdateElos(GameResult[] gameResults, double maxAdjustmentPerGame)
    {
        int numberOfGameResults = gameResults.Length;

        if (numberOfGameResults <= 1)
        {
            return;
        }

        double[,] expectedWinChances = new double[numberOfGameResults, numberOfGameResults];
        double[,] actualWins = new double[numberOfGameResults, numberOfGameResults];

        for (int i = 0; i < numberOfGameResults; i++)
        {
            for (int j = 0; j < numberOfGameResults; j++)
            {
                GameResult result1 = gameResults[i];
                GameResult result2 = gameResults[j];

                expectedWinChances[i, j] = ExpectedWinChance(result1, result2);
                actualWins[i, j] = ActualWinChance(result1, result2);
            }
        }

        for (int i = 0; i < numberOfGameResults; i++)
        {
            double totalActualScoreForPlayer = 0;
            double totalExpectedScoreForPlayer = 0;

            for (int j = 0; j < numberOfGameResults; j++)
            {
                if (i == j)
                {
                    continue;
                }

                totalActualScoreForPlayer += actualWins[i, j];
                totalExpectedScoreForPlayer += expectedWinChances[i, j];
            }

            double updateAmount = maxAdjustmentPerGame * ((totalActualScoreForPlayer - totalExpectedScoreForPlayer) / (numberOfGameResults - 1));

            GameResult currentGameResult = gameResults[i];
            Player currentPlayer = currentGameResult.Player!;

            currentPlayer.EloScores[currentGameResult.Game.GameNumber] = currentPlayer.EloLatest + GetBoostFactor(currentPlayer.EloScores.Count) * (updateAmount);
        }
    }

    private static double GetBoostFactor(int gameNumber)
    {
        if (gameNumber < 8)
        {
            return 3d;
        }

        if (gameNumber < 16)
        {
            return 2d;
        }

        return 1d;
    }

    private static double ExpectedWinChance(GameResult gameResult1, GameResult gameResult2)
    {
        double player1LatestElo = gameResult1.Player!.EloLatest;
        double player2LatestElo = gameResult2.Player!.EloLatest;

        double deltaOver400 = (player2LatestElo - player1LatestElo) / 400;

        double exponentiatedDeltaOver400 = Math.Pow(10d, deltaOver400);

        return 1 / (1 + exponentiatedDeltaOver400);
    }

    public static double ActualWinChance(GameResult gameResult1, GameResult gameResult2)
    {
        if (gameResult1.Score > gameResult2.Score)
        {
            return 1;
        }

        if (gameResult1.Score == gameResult2.Score)
        {
            if (gameResult1.SecondaryScore > gameResult2.SecondaryScore)
            {
                return 1;
            }

            if (gameResult1.SecondaryScore == gameResult2.SecondaryScore)
            {
                return 0.5;
            }

            return 0;
        }

        return 0;
    }
}