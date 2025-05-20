using ExcelWorldChampionshipELO.Core.Domain;
using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Logic;
using ExcelWorldChampionshipELO.Core.Storage;

namespace ExcelWorldChampionshipELO
{
    internal class Program
    {
        static void Main()
        {
            bool exit = false;

            while (!exit)
            {
                if (TourneyStorage.LastRunTourney is null)
                {
                    exit = InterpretStartingCommand();
                }
                else
                {
                    exit = InterpretFollowUpCommand();
                }
            }
        }

        private static bool InterpretFollowUpCommand()
        {
            try
            {
                Console.WriteLine("Please enter a follow-up command, e.g. 'exit', 'print-results' or 'player-stats':");

                string? input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "exit":
                        return true;
                    case "print-results":
                        return false;
                    case "player-stats":
                        ExecutePlayerStatsCommand();
                        return false;
                    default:
                        Console.WriteLine("Input not recognised, please try again.");
                        return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException);

                return false;
            }
        }

        private static void ExecutePlayerStatsCommand()
        {
            Tourney tourney = TourneyStorage.LastRunTourney!;

            Console.WriteLine($"Please enter player name (e.g.) '{tourney.Players.MaxBy(x => x.EloLatest)!.Name}'");
            string? input = Console.ReadLine()?.ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (tourney.Players.FirstOrDefault(x => x.Name == input) is Player player)
            {
                PrintPlayerStats(player);
            }
            else
            {
                Console.WriteLine($"{input} not found in the data.");
                ExecutePlayerStatsCommand();
            }

            return;
        }

        private static void PrintPlayerStats(Player player)
        {

        }

        private static void RunTourney()
        {
            string name = GetStringInput("tourneyName");
            string gamesFilePath = GetStringInput("gameDataCsvFilePath");
            string playersFilePath = GetStringInput("playerDataCsvFilePath");

            TourneyInputs tourneyInputs = new()
            {
                Name = name,
                GameDataPath = gamesFilePath,
                PlayerDataPath = playersFilePath,
            };

            InputTourneyController.RunTourney(tourneyInputs);
        }

        private static void RunDefaultTourney()
        {
            TourneyInputs tourneyInputs = new()
            {
                Name = "Default Tourney Elo Data",
                GameDataPath = "C:\\Users\\harry\\source\\repos\\ExcelWorldChampionshipELO\\SampleResources\\GameData.csv",
                PlayerDataPath = "C:\\Users\\harry\\source\\repos\\ExcelWorldChampionshipELO\\SampleResources\\PlayerData.csv",
            };

            InputTourneyController.RunTourney(tourneyInputs);
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
                Console.WriteLine($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (int.TryParse(consoleInput, out input))
                {
                    isSet = true;
                }
                else
                {
                    Console.WriteLine("Could not interpret input.");
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
                Console.WriteLine($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (double.TryParse(consoleInput, out input))
                {
                    isSet = true;
                }
                else
                {
                    Console.WriteLine("Could not interpret input.");
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
                Console.WriteLine($"Please enter the {parameterName}:");
                string? consoleInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(consoleInput))
                {
                    input = consoleInput;
                    isSet = true;
                }
                else
                {
                    Console.WriteLine("Could not interpret input.");
                }
            }

            return input;
        }

        private static bool InterpretStartingCommand()
        {
            try
            {
                Console.WriteLine("Please enter command, e.g. 'exit', 'run-default-tourney' or 'run-tourney':");
                string? input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "exit":
                        return true;
                    case "run-default-tourney":
                        RunDefaultTourney();
                        return false;
                    case "run-tourney":
                        RunTourney();
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
                Console.WriteLine("Exception:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException);

                return false;
            }
        }
    }
}
