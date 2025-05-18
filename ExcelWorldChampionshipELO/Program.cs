using ExcelWorldChampionshipELO.Core.Domain;
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
                    case "run-random-tourney-auto":
                        DummyTourneyController.RunTourney();
                        return false;
                    case "run-random-tourney-configured":
                        RunTourneyConfigured();
                        return false;
                    default:
                        Console.WriteLine("Input not recognised, try 'run-random-tourney-auto'.");
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

        public static void RunTourneyConfigured()
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
                string? seedInput = Console.ReadLine();

                if (int.TryParse(seedInput, out input))
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
                string? seedInput = Console.ReadLine();

                if (double.TryParse(seedInput, out input))
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
    }
}
