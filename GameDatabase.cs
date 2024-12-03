using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sikanoppa
{
    public class GameDatabase
    {
        public List<GameData> Data;
        public string TiedostoPolku { get; }


        public GameDatabase(string tiedostoPolku)
        {
            Data = new List<GameData>();
            TiedostoPolku = tiedostoPolku;
            LoadGames();
        }

        public void StartNewGame()
        {
            Console.Clear();
            GameDatabase gameDatabase = new GameDatabase(TiedostoPolku);  // Assuming GameDatabase is already defined
            Game game = new Game(gameDatabase);  // Passing gameDatabase to the Game constructor

            // Determine the new game ID by checking the highest existing game ID and adding 1
            int gameId = Data.Count > 0 ? Data.Max(g => g.ID) + 1 : 1;

            // Get Player 1 details
            string player1Name;
            do
            {
                Console.WriteLine("Enter Player 1 name:");
                player1Name = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(player1Name))
                {
                    Console.WriteLine("Player name cannot be empty. Please try again.");
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (string.IsNullOrEmpty(player1Name));
            Console.Clear();

            bool isPlayer1Robot;
            do
            {
                Console.WriteLine("Is Player 1 a robot? (y/n):");
                string input = Console.ReadLine()?.ToLower();
                if (input == "y")
                {
                    isPlayer1Robot = true;
                    break;
                }
                else if (input == "n")
                {
                    isPlayer1Robot = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");
                }
            } while (true);
            Console.Clear();

            // Get Player 2 details
            string player2Name;
            do
            {
                Console.WriteLine("Enter Player 2 name:");
                player2Name = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(player2Name))
                {
                    Console.WriteLine("Player name cannot be empty. Please try again.");
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (string.IsNullOrEmpty(player2Name));
            Console.Clear();

            bool isPlayer2Robot;
            do
            {
                Console.WriteLine("Is Player 2 a robot? (y/n):");
                string input = Console.ReadLine()?.ToLower();
                if (input == "y")
                {
                    isPlayer2Robot = true;
                    break;
                }
                else if (input == "n")
                {
                    isPlayer2Robot = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");
                }
            } while (true);
            Console.Clear();

            // Initialize scores and turn tracking
            int player1Score = 0;
            int player2Score = 0;
            bool isItPlayersTurn1 = true; // Player 1 starts first

            // Create a new game with the gathered information
            var newGame = new GameData(gameId, player1Name, player2Name)
            {
                IsPlayer1Robot = isPlayer1Robot,
                IsPlayer2Robot = isPlayer2Robot,
                Player1Score = player1Score,
                Player2Score = player2Score,
                IsItPlayersTurn1 = isItPlayersTurn1
            };

            // Add the new game to the list of games and save it
            Data.Add(newGame);
            SaveGames(Data); // Save the list of games to persistent storage

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine(CenterText("New Game Started!"));
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.ResetColor();
            Console.WriteLine(CenterText($"Game ID: {gameId}"));
            Console.WriteLine(CenterText($"Player 1: {player1Name} (Robot: {isPlayer1Robot}) - Score: {player1Score}"));
            Console.WriteLine(CenterText($"Player 2: {player2Name} (Robot: {isPlayer2Robot}) - Score: {player2Score}"));
            Console.WriteLine();
            Console.WriteLine(CenterText("Press any key to begin the game..."));
            game.PlayGame(newGame);
        }


        public void ContinueInterruptedGame()
        {
            GameDatabase gameDatabase = new GameDatabase(TiedostoPolku);  // Assuming GameDatabase is already defined
            Game game = new Game(gameDatabase);  // Passing gameDatabase to the Game constructor
            LoadGames(); // Ensure the data is loaded
            ListGames();

            if (Data.Count == 0)
            {
                Console.WriteLine("No saved games found.");
                return;
            }

            Console.Write("Enter the number of the game you want to continue (e for exit): ");
            string userInput = Console.ReadLine()?.ToLower();  // Read input and convert to lowercase

            if (userInput == "e")
            {
                // Exit the program or handle accordingly if the user enters 'e'
                Console.WriteLine("Exiting...");
                Console.Clear();
                return;  // or other exit handling code
            }

            if (!int.TryParse(userInput, out int selectedGameId))
            {
                // Invalid input, handle it
                Console.WriteLine("Invalid input. Please enter a valid game number.");
                return;
            }

            GameData gameToContinue = Data.FirstOrDefault(g => g.ID == selectedGameId);

            if (gameToContinue == null)
            {
                Console.WriteLine("No game found with the provided ID.");
                return;
            }
            Console.Clear();
            Console.WriteLine($"Continuing game with ID: {gameToContinue.ID}");

            game.PlayGame(gameToContinue);
        }

        public void SaveOrUpdate(GameData uusiPeli, List<GameData> pelit)
        {
            var vanhaPeli = pelit.FirstOrDefault(p => p.ID == uusiPeli.ID);
            if (vanhaPeli != null)
            {
                int index = pelit.IndexOf(vanhaPeli);
                pelit[index] = uusiPeli;
            }
            else
            {
                pelit.Add(uusiPeli); // Add new game to the list
            }

            SaveGames(pelit); // Pass the updated list of games
        }

        private void LoadGames()
        {
            if (File.Exists(TiedostoPolku))
            {
                var json = File.ReadAllText(TiedostoPolku);
                Data = System.Text.Json.JsonSerializer.Deserialize<List<GameData>>(json) ?? new List<GameData>();
            }
        }

        public void SaveGames(List<GameData> games)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(games, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TiedostoPolku, json);
        }

        public void ListGames()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine(CenterText("Saved Games"));
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.ResetColor();

            // Create a header row with column names
            Console.WriteLine($"{new string('-', 4)} | {new string('-', 20)} | {new string('-', 8)} | {new string('-', 10)} | " +
                              $"{new string('-', 20)} | {new string('-', 8)} | {new string('-', 10)}");

            Console.WriteLine($"ID   | Player 1 Name        | Is Robot | Score      | Player 2 Name        | Is Robot | Score ");
            Console.WriteLine($"{new string('-', 4)} | {new string('-', 20)} | {new string('-', 8)} | {new string('-', 10)} | " +
                              $"{new string('-', 20)} | {new string('-', 8)} | {new string('-', 10)}");

            // List games with data
            for (int i = 0; i < Data.Count; i++)
            {
                GameData game = Data[i];

                Console.WriteLine($"{i + 1,-4} | {Shorten(game.Player1Name, 20),-20} | {game.IsPlayer1Robot,-8} | {game.Player1Score,-10} | " + $"{Shorten(game.Player2Name, 20),-20} | {game.IsPlayer2Robot,-8} | {game.Player2Score,-10}");

            }
        }
        static string Shorten(string text, int maxLength)
        {
            return text.Length > maxLength ? text.Substring(0, maxLength - 3) + "..." : text;
        }
        private string CenterText(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int padding = (consoleWidth - text.Length) / 2;
            return new string(' ', padding) + text;
        }
    }
}