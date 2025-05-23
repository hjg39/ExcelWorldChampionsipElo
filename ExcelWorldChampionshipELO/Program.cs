using ExcelWorldChampionshipELO.Core.Common;
using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Exporting;
using ExcelWorldChampionshipELO.Core.Logic;
using ExcelWorldChampionshipELO.Core.Storage;
using System.Reflection;

namespace ExcelWorldChampionshipELO
{
    internal class Program
    {
        static bool _hasTourneyBeenSet = false;

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            bool exit = false;

            while (!exit)
            {
                if (TourneyStorage.LastRunTourney is not Tourney tourney)
                {
                    exit = InterpretStartingCommand();
                }
                else
                {
                    if (!_hasTourneyBeenSet)
                    {
                        WriteSystemPrompt($"Tourney of {tourney.Games.Length} games and {tourney.Players.Length} players ran successfully.");
                        _hasTourneyBeenSet = true;
                    }

                    exit = InterpretFollowUpCommand();
                }
            }
        }

        private static bool InterpretFollowUpCommand()
        {
            try
            {
                WriteSystemPrompt("Please enter a follow-up command, e.g. 'exit', 'get-csv-results', 'get-chart-results' or 'player-stats':");

                string? input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "exit":
                        return true;
                    case "get-csv-results":
                        CsvExporter.ExportPlayerResults(TourneyStorage.LastRunTourney!);
                        CsvExporter.ExportGameDifficulties(TourneyStorage.LastRunTourney!);
                        WriteSystemPrompt("Exported csv results to desktop");
                        return false;
                    case "get-chart-results":
                        InputTourneyController.PlotTourney(TourneyStorage.LastRunTourney!);
                        WriteSystemPrompt("Saved charts to desktop.");
                        return false;
                    case "player-stats":
                        ExecutePlayerStatsCommand();
                        
                        return false;
                    default:
                        WriteSystemPrompt("Input not recognised, please try again.");
                        return false;
                }

            }
            catch (Exception ex)
            {
                WriteException(ex);
                return false;
            }
        }

        private static void ExecutePlayerStatsCommand()
        {
            Tourney tourney = TourneyStorage.LastRunTourney!;
            WriteSystemPrompt($"Please enter player name (e.g.) '{tourney.Players.MaxBy(x => x.EloLatest)!.Name}'");

            string? input = Console.ReadLine()?.ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (tourney.Players.FirstOrDefault(x => x.Name.Equals(input, StringComparison.CurrentCultureIgnoreCase)) is Player player)
            {
                PrintPlayerStats(player, tourney);
                ExecutePrintPlayerChartsCommand(player, tourney);
            }
            else
            {
                WriteSystemPrompt($"{input} not found in the data, did you mean {tourney.Players.MinBy(x => LevenshteinDistance.CalculateDistance(input, x.Name))!.Name}?");
                ExecutePlayerStatsCommand();
            }

            return;
        }

        private static void ExecutePrintPlayerChartsCommand(Player player, Tourney tourney)
        {
            WriteSystemPrompt($"Enter 'Yes' to create charts for {player.Name} and their closest active rivals, enter anything else otherwise.");
            string? input = Console.ReadLine()?.ToLower();

            if (input != "yes")
            {
                return;
            }

            int targetPlayerFinalWorldRanking = player.WorldRankingLatest;

            Guid[] latest10GameIds = [.. tourney.Games.OrderByDescending(x => x.GameNumber).Take(10).Select(x => x.GameId)];

            List<Player> relevantPlayers = [..tourney.Players.Where(x => x.GameScores.Keys.Intersect(latest10GameIds).Count() >= 2)];
            relevantPlayers = [.. relevantPlayers.OrderBy(x => Math.Abs(player.EloLatest - x.EloLatest)).Take(21)];

            relevantPlayers.Add(player);

            InputTourneyController.PlotPlayers(player, [.. relevantPlayers.Distinct()], tourney);

            WriteSystemPrompt($"Saved {player.Name} charts to desktop.");
        }

        private static void PrintPlayerStats(Player player, Tourney tourney)
        {
            Guid[] playerGames = [..player.GameScores.Keys];

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Name: {player.Name}");
            Console.WriteLine($"Data on {player.GameScores.Count} games available: '{player.FirstGamePlayed(tourney).Name}' to '{player.LastGamePlayed(tourney).Name}'");
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Latest Elo: {player.EloLatest:0.00}, achieved on {player.GameWhereEloAchieved(player.EloLatest, tourney).Name}.");
            Console.WriteLine($"Min Elo: {player.EloMin:0.00}, achieved on {player.GameWhereEloAchieved(player.EloMin, tourney).Name}.");
            Console.WriteLine($"Max Elo: {player.EloMax:0.00}, achieved on {player.GameWhereEloAchieved(player.EloMax, tourney).Name}.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Latest world ranking: {player.WorldRankingLatest}, now that {player.GameWhereWorldRankingAchieved(player.WorldRankingLatest, tourney).Name} ran.");
            Console.WriteLine($"Highest world ranking: {player.WorldRankingMin}, most recently occurred when {player.GameWhereWorldRankingAchieved(player.WorldRankingMin, tourney, false).Name} ran.");
            Console.WriteLine($"Lowest world ranking: {player.WorldRankingMax}, first occured when {player.GameWhereWorldRankingAchieved(player.WorldRankingMax, tourney, true).Name} ran.");

            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void RunCustomTourney()
        {
            string name = GetStringInput("tourneyName");

            WriteSystemPrompt("See csv sample files in the exe folder for the input data format.");
            string gamesFilePath = GetFileInput("gameDataCsv");
            string playersFilePath = GetFileInput("playerDataCsvFilePath");

            TourneyInputs tourneyInputs = new()
            {
                Name = name,
                GameDataPath = gamesFilePath,
                PlayerDataPath = playersFilePath,
            };

            InputTourneyController.RunTourney(tourneyInputs);
        }

        private static void RunDefaultMEWCTourney()
        {
            string exePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            string exeDirectory = Path.GetDirectoryName(exePath)!;

            string resourcesDirectory = Path.Combine(exeDirectory, "SampleResources");

            TourneyInputs tourneyInputs = new()
            {
                Name = "MEWC",
                GameDataPath = Path.Combine(resourcesDirectory, "GameData.csv"),
                PlayerDataPath = Path.Combine(resourcesDirectory, "PlayerData.csv"),
            };

            InputTourneyController.RunTourney(tourneyInputs);
        }

        private static void RunDefaultFMWCTourney()
        {
            string exePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            string exeDirectory = Path.GetDirectoryName(exePath)!;

            string resourcesDirectory = Path.Combine(exeDirectory, "SampleResources");

            TourneyInputs tourneyInputs = new()
            {
                Name = "FMWC",
                GameDataPath = Path.Combine(resourcesDirectory, "GameDataFMWC.csv"),
                PlayerDataPath = Path.Combine(resourcesDirectory, "PlayerDataFMWC.csv"),
            };

            InputTourneyController.RunTourney(tourneyInputs);
        }

        private static void WriteSystemPrompt(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void RunRandomTourneyConfigured()
        {
            int seed = GetIntegerInput("seed");
            int numberOfGames = GetIntegerInput("numberOfGames");
            int numberOfPlayers = GetIntegerInput("numberOfPlayers");
            int nameLength = GetIntegerInput("nameLength");
            double probabilityPlayerPlaysRound = GetDoubleInput("probabilityPlayerPlaysRound");

            DummyTourneyInputs dummyTourneyInputs = new()
            {
                Seed = seed,
                NumberOfGames = numberOfGames,
                NumberOfPlayers = numberOfPlayers,
                NameLength = nameLength,
                ProbabilityPlayerPlaysRound = probabilityPlayerPlaysRound,
            };

            DummyTourneyController.RunTourney(dummyTourneyInputs);
        }

        private static int GetIntegerInput(string parameterName)
        {
            bool isSet = false;
            int input = 0;

            while (!isSet)
            {
                WriteSystemPrompt($"Please enter the {parameterName}:");

                string? consoleInput = Console.ReadLine();

                if (int.TryParse(consoleInput, out input))
                {
                    isSet = true;
                }
                else
                {
                    WriteSystemPrompt("Could not interpret input.");
                }
            }

            return input;
        }

        private static double GetDoubleInput(string parameterName)
        {
            bool isSet = false;
            double input = 0;

            while (!isSet)
            {
                WriteSystemPrompt($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (double.TryParse(consoleInput, out input))
                {
                    isSet = true;
                }
                else
                {
                    WriteSystemPrompt("Could not interpret input.");
                }
            }

            return input;
        }

        private static string GetStringInput(string parameterName)
        {
            bool isSet = false;
            string input = string.Empty;

            while (!isSet)
            {
                WriteSystemPrompt($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(consoleInput))
                {
                    input = consoleInput;
                    isSet = true;
                }
                else
                {
                    WriteSystemPrompt("Could not interpret input.");
                }
            }

            return input;
        }

        private static string GetFileInput(string parameterName)
        {
            bool isSet = false;
            string input = string.Empty;

            while (!isSet)
            {
                WriteSystemPrompt($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(consoleInput))
                {
                    WriteSystemPrompt("Could not interpret input.");
                }
                else if (!File.Exists(consoleInput))
                {
                    WriteSystemPrompt("File not found, please try again.");
                    return GetFileInput(parameterName);
                }
                else
                {
                    input = consoleInput;
                    isSet = true;
                }
            }

            return input;
        }

        private static bool InterpretStartingCommand()
        {
            try
            {
                WriteSystemPrompt("Please enter command, e.g. 'exit', 'run-default-mewc-tourney', 'run-default-fmwc-tourney' or 'run-custom-tourney':");
                string? input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "exit":
                        return true;
                    case "run-default-mewc-tourney":
                        RunDefaultMEWCTourney();
                        return false;
                    case "run-default-fmwc-tourney":
                        RunDefaultFMWCTourney();
                        return false;
                    case "run-custom-tourney":
                        RunCustomTourney();
                        return false;
                    case "run-random-tourney-auto":
                        DummyTourneyController.RunTourney();
                        return false;
                    case "run-random-tourney-configured":
                        RunRandomTourneyConfigured();
                        return false;
                    default:
                        Console.WriteLine("Input not recognised, please try again.");
                        return false;
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                return false;
            }
        }

        private static void WriteException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Exception:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.InnerException);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
