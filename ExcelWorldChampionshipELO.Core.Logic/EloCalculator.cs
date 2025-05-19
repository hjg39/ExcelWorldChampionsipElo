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
            UpdateElos(game, tourney.Players, maxAdjustmentPerGame);
            Console.WriteLine($"Game {i++} completed, EloMax: {tourney.Players.Max(x => x.EloLatest)}, EloMin: {tourney.Players.Min(x => x.EloLatest)}");
        }
    }

    private static void UpdateElos(Game game, Player[] players, double maxAdjustmentPerGame)
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

            currentPlayer.EloScores[currentGameResult.Game.GameNumber] = currentPlayer.EloLatest + updateAmount;
        }
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