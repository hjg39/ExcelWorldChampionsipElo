using ExcelWorldChampionshipELO.Core.Domain.ConsoleInput;
using ExcelWorldChampionshipELO.Core.Logic;

namespace ExcelWorldChampionshipELO
{
    internal class Program
    {
        static void Main()
        {
            bool exit = false;

            while (!exit)
            {
                exit = InterpretCommand();
            }
        }

        public static bool InterpretCommand()
        {
            try
            {
                Console.WriteLine("Please enter command:");
                string? input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "exit":
                        return true;
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
                        Console.WriteLine("Input not recognised, try 'exit', 'run-tourney', 'run-random-tourney-auto' or 'run-random-tourney-configured'.");
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

        public static void RunTourney()
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

        public static void RunRandomTourneyConfigured()
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

        public static int GetIntegerInput(string parameterName)
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

        public static double GetDoubleInput(string parameterName)
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

        public static string GetStringInput(string parameterName)
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
    }
}
