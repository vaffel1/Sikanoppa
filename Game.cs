using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sikanoppa
{
    public class Game
    {
        private Random random;
        private GameDatabase gameDatabase;

        private int WinScore = 100;

        public Game(GameDatabase gameDatabase)
        {
            random = new Random();
            this.gameDatabase = gameDatabase;
        }

        // Game play loop
        public void PlayGame(GameData gameData)
        {
            while (gameData.Player1Score < WinScore && gameData.Player2Score < WinScore)
            {
                string currentPlayerName = gameData.IsItPlayersTurn1 ? gameData.Player1Name : gameData.Player2Name;
                int currentPlayerScore = gameData.IsItPlayersTurn1 ? gameData.Player1Score : gameData.Player2Score;
                gameData.RoundScore = 0;  // Track score for this round


                PrintPlayerHeader(gameData);
                Console.WriteLine($"Press any key to roll the dice.");
                Console.ReadKey();

                while (true)
                {
                    int diceRoll = RollDice();
                    Console.SetCursorPosition(0, Console.CursorTop); // To overwrite the last line

                    // Print the result of the dice roll
                    PrintPlayerHeader(gameData);
                    Console.WriteLine($"{currentPlayerName} rolled: {diceRoll}");
                    Console.ReadKey();

                    if (diceRoll == 1)
                    {
                        // Check if the round score is zero
                        if (gameData.RoundScore == 0)
                        {
                            // Special logic when the round score is zero
                            PrintPlayerHeader(gameData);
                            Console.WriteLine($"{currentPlayerName} rolled a 1! No points to lose this round.");
                            Console.ReadKey();
                        }
                        else
                        {
                            // Logic for when the player loses the points for the round
                            PrintPlayerHeader(gameData);
                            Console.WriteLine($"{currentPlayerName} rolled a 1! Losing the points for this round.");
                            Console.ReadKey();

                            //if (gameData.IsItPlayersTurn1)
                            //{
                            //    gameData.Player1Score -= gameData.RoundScore;
                            //}
                            //else
                            //{
                            //    gameData.Player2Score -= gameData.RoundScore;
                            //}
                        }

                        // Reset the round score to zero after the roll of a 1
                        gameData.RoundScore = 0;

                        // End the player's turn if they roll a 1
                        break;
                    }
                    else
                    {
                        // Add points to the round score
                        gameData.RoundScore += diceRoll;

                        PrintPlayerHeader(gameData);
                        Console.WriteLine($"New round score: {gameData.RoundScore}. Current total: {currentPlayerScore + gameData.RoundScore}.");

                        string choice = "n";

                        while (true)
                        {
                            if (IsPlayerRobot(gameData))
                            {
                                choice = GetChoice(gameData.RoundScore);
                            }
                            else
                            {
                                // Ask the player if they want to continue or end their round
                                Console.WriteLine("Would you like to continue your turn? (y/n)");
                                choice = Console.ReadLine()?.ToLower();
                            }
                            if (choice == "n")
                            {
                                // End the round, update the total score
                                if (gameData.IsItPlayersTurn1)
                                {
                                    gameData.Player1Score += gameData.RoundScore;
                                }
                                else
                                {
                                    gameData.Player2Score += gameData.RoundScore;
                                }
                                gameData.RoundScore = 0;

                                // Display round end status
                                PrintPlayerHeader(gameData);
                                Console.WriteLine($"{currentPlayerName}'s round is over! Final score this round: {currentPlayerScore + gameData.RoundScore}.");
                                break;
                            }
                            else if (choice == "y")
                            {
                                // Continue turn
                                break;
                            }
                            else
                            {
                                // Invalid input handling
                                PrintPlayerHeader(gameData);
                                Console.WriteLine("Invalid input. Please enter 'y' to continue or 'n' to end your round.");
                                Console.ReadKey();
                            }
                        }

                        break;
                    }
                }

                // Check if the player has won
                if (CheckWinCondition(gameData))
                {
                    break;
                }

                // Save the game state after each turn
                gameDatabase.SaveOrUpdate(gameData, gameDatabase.Data);

                // Switch turns
                gameData.UpdateIsItPlayersTurn();
            }

            // Declare the winner at the end of the game
            DeclareWinner(gameData);

            // Save final game state
            gameDatabase.SaveOrUpdate(gameData, gameDatabase.Data);
        }

        // Print the player header in a neat format
        private void PrintPlayerHeader(GameData gameData)
        {
            Console.Clear();

            string currentPlayerName = gameData.IsItPlayersTurn1 ? gameData.Player1Name : gameData.Player2Name;
            int currentPlayerScore = gameData.IsItPlayersTurn1 ? gameData.Player1Score : gameData.Player2Score;

            // Centering the text based on the console width
            string title = $"{currentPlayerName}'s Turn";
            string roundScoreText = $"Current Round Score: {gameData.RoundScore}";
            string player1ScoreText = $"Player 1: {gameData.Player1Name} - Score: {gameData.Player1Score}";
            string player2ScoreText = $"Player 2: {gameData.Player2Name} - Score: {gameData.Player2Score}";

            // Calculate padding for centering text
            int consoleWidth = Console.WindowWidth;
            int titlePadding = (consoleWidth - title.Length) / 2;
            int roundScorePadding = (consoleWidth - roundScoreText.Length) / 2;
            int player1Padding = (consoleWidth - player1ScoreText.Length) / 2;
            int player2Padding = (consoleWidth - player2ScoreText.Length) / 2;

            // Print centered text
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string(' ', titlePadding) + title);
            Console.WriteLine(new string('-', consoleWidth));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', roundScorePadding) + roundScoreText);
            Console.WriteLine(new string('-', consoleWidth));
            Console.WriteLine(new string(' ', player1Padding) + player1ScoreText);
            Console.WriteLine(new string(' ', player2Padding) + player2ScoreText);
            Console.WriteLine(new string('-', consoleWidth));
            Console.ForegroundColor = ConsoleColor.White;
        }



        // Roll the dice (1-6)
        private int RollDice()
        {
            return random.Next(1, 7);
        }

        // Check if either player has won
        private bool CheckWinCondition(GameData gameData)
        {
            return gameData.Player1Score >= WinScore || gameData.Player2Score >= WinScore;
        }

        // Declare the winner at the end of the game
        private void DeclareWinner(GameData gameData)
        {
            Console.Clear();
            if (gameData.Player1Score >= WinScore)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{gameData.Player1Name} wins with a score of {gameData.Player1Score}!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{gameData.Player2Name} wins with a score of {gameData.Player2Score}!");
            }
            Console.ReadKey();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private bool IsPlayerRobot(GameData gameData)
        {
            return gameData.IsItPlayersTurn1 ? gameData.IsPlayer1Robot : gameData.IsPlayer2Robot;
        }

        private string GetChoice(int score)
        {
            // Simple decision-making algorithm
            if (score >= 20)
            {
                return "n";  // End the round if the round score is 20 or more
            }
            else if (score < 5)
            {
                return "y";  // Keep going if the round score is very low (less than 5)
            }
            else
            {
                // Randomly decide if the round score is between 5 and 20
                int riskFactor = random.Next(1, 101);  // Generate a number between 1 and 100
                if (riskFactor <= 70)
                {
                    return "y";  // 70% chance to keep going
                }
                else
                {
                    return "n";  // 30% chance to end the round
                }
            }
        }
    }
}
