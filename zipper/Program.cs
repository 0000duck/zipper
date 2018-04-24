using System;

namespace zipper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StartProgram(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error \"{0}\"", ex.Message);
                Console.WriteLine("Do you want to try again? y/n");

                if (Console.ReadKey().KeyChar == 'y' || Console.ReadKey().KeyChar == 'Y')
                {
                    StartProgram(new string[0]);
                }
            }
        }

        private static void StartProgram(string[] args)
        {
            var consoleManager = args?.Length == 0 ? new ConsoleManager() : new ConsoleManager(args);

            var zipManager = ManagerFactry.CreateZipManager(consoleManager.CompressionMode, consoleManager.InputFileName, consoleManager.OutputFileName);
            zipManager.Run();

            Console.WriteLine("File was seccessfully {0}ed, enter any button to exit", consoleManager.CompressionMode.ToString().ToLower());
            Console.ReadLine();
        }
    }
}
