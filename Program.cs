namespace Sikanoppa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "games.json"; // Specify your file path here
            GameDatabase gameDatabase = new GameDatabase(filePath);

            while (true)
            {
                PrintMenu();

                switch (Console.ReadLine())
                {
                    case "1":
                        gameDatabase.StartNewGame();
                        break;
                    case "2":
                        gameDatabase.ContinueInterruptedGame();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void PrintMenu()
        {
            Console.Clear(); // Clear the console before displaying the menu

            // Menu text
            string title = "Game Menu";
            string option1 = "1. Start New Game";
            string option2 = "2. Continue Interrupted Game";
            string option3 = "3. Exit";
            string prompt = "Enter your choice: ";

            // Calculate padding for center alignment
            int consoleWidth = Console.WindowWidth;
            int titlePadding = (consoleWidth - title.Length) / 2;
            int option1Padding = (consoleWidth - option1.Length) / 2;
            int option2Padding = (consoleWidth - option2.Length) / 2;
            int option3Padding = (consoleWidth - option3.Length) / 2;
            int promptPadding = (consoleWidth - prompt.Length) / 2;

            // Display the centered menu
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('-', consoleWidth));
            Console.WriteLine(new string(' ', titlePadding) + title);
            Console.WriteLine(new string('-', consoleWidth));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', option1Padding) + option1);
            Console.WriteLine(new string(' ', option2Padding) + option2);
            Console.WriteLine(new string(' ', option3Padding) + option3);
            Console.WriteLine(new string('-', consoleWidth));

            Console.Write(new string(' ', promptPadding) + prompt);
            Console.ResetColor();
        }
    }
}
