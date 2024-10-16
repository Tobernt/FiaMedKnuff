using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using FiaMedKnuff;
using Windows.Media.PlayTo;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        private Player[] players; // use gamelogic class later
        private int currentPlayerIndex;
        private int totalPlayers = 4;
        private Random random;
        DateTime startTime = DateTime.Now;

        // Paths for each player, defined as (row, column) positions on the grid.
        private readonly (int row, int col)[] RedPath = new (int row, int col)[]
        {
            // Starting from Red quadrant at (4, 0)
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), 
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5), (0, 6), 
            (1, 6), (2, 6), (3, 6), (4, 6), (4, 7), (4, 8), 
            (4, 9), (4, 10), (5, 10), (6, 10), (6, 9), (6, 8),
            (6, 7), (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), 
            (10, 5), (10, 4), (9, 4), (8, 4), (7, 4), (6, 4), 
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), (5, 1),
            (5, 2), (5,3),(5,4),(5,5)
        };

        private readonly (int row, int col)[] BluePath = new (int row, int col)[]
        {
            // Starting from Blue quadrant at (0, 6), moving down
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6), 
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10), 
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6), 
            (7, 6), (8, 6), (9, 6), (10, 6), (10, 5),
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4), 
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), 
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), 
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5), 
            (1,5), (2,5), (3,5), (4,5), (5,5)
        };

        private readonly (int row, int col)[] YellowPath = new (int row, int col)[]
        {
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4), 
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), 
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), 
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5), 
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6), 
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10), 
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6), 
            (7, 6), (8, 6), (9, 6), (10, 6), (10,5), 
            (9,5), (8,5), (6,5),(5,5)
        };

        private readonly (int row, int col)[] GreenPath = new (int row, int col)[]
        {
            (6, 10), (6, 9), (6, 8), (6, 7), (6, 6), 
            (7, 6), (8, 6), (9, 6), (10, 6), (10, 5), 
            (10, 4), (9, 4), (8, 4), (7, 4), (6, 4), 
            (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), 
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), 
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5), 
            (0, 6), (1, 6), (2, 6), (3, 6), (4, 6), 
            (4, 7), (4, 8), (4, 9), (4, 10), (5, 10),
            (5, 9), (5, 8), (5, 7),(5, 6), (5, 5)
        };

        public MainPage()
        {
            this.InitializeComponent();

            Player red = new Player("red"); 
            Player blue = new Player("blue");
            Player green = new Player("green");
            Player yellow = new Player("yellow");
            players = new Player[] { red, blue, green, yellow };
            random = new Random();
            currentPlayerIndex = random.Next(0, 4); 
			DiceIsEnable(currentPlayerIndex);
        }

        private void DiceIsEnable(int currentPlayerIndex)
        {
			Button[] diceButtons = { RedDiceBtn, BlueDiceBtn, GreenDiceBtn, YellowDiceBtn };
            foreach (Button button in diceButtons)
            {
                button.IsEnabled = false;
            }
            if(currentPlayerIndex >= 0 && currentPlayerIndex < diceButtons.Length) 
            {
                diceButtons[currentPlayerIndex].IsEnabled = true;
            }
        }

        private async void RollDice_Click(object sender, RoutedEventArgs e)
        {
            int diceRoll = RollDice();

            // Display the result of the dice roll
            Button clickedButton = sender as Button;
            if (clickedButton == RedDiceBtn) RedDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == BlueDiceBtn) BlueDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == GreenDiceBtn) GreenDice.ThrowDiceVisual(diceRoll);
            if (clickedButton == YellowDiceBtn) YellowDice.ThrowDiceVisual(diceRoll);

            DiceRollResult.Text = $"{IndexToName(currentPlayerIndex)} rolled a {diceRoll}";

            bool hasPiecesOnBoard = players[currentPlayerIndex].HasPiecesOnBoard; // Are any pieces on the board?
            bool hasPiecesInNest = players[currentPlayerIndex].HasPiecesInNest(); // Are there still pieces in the nest?
            bool allPiecesInNestOrGoal = players[currentPlayerIndex].AllPiecesInNestOrGoal(); // Check if all pieces are in nest or goal

            // Case 1: If all pieces are either in the nest or goal and player rolls a 1 or 6, move a piece out of the nest
            if (diceRoll == 1 && !hasPiecesOnBoard)
            {
                // Move a piece out of the nest
                int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                MovePlayer(currentPlayerIndex, diceRoll == 1 ? 0 : 5, tokenToMoveOut); // Move the piece to the start or 6 steps based on the dice roll

            }
            // Case 2: If the player rolls a 6, still has pieces in the nest, and none on the board
            else if (diceRoll == 6 && !hasPiecesOnBoard)
            {
                // Prompt the player to choose between moving one token 6 steps or moving two tokens 1 step each
                ContentDialog choiceDialog = new ContentDialog
                {
                    Title = "Move Choice",
                    Content = "Would you like to move one token 6 steps or move two tokens 1 step each?",
                    PrimaryButtonText = "Move 1 token 6 steps",
                    SecondaryButtonText = "Move 2 tokens 1 step each"
                };

                ContentDialogResult result = await choiceDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Move one token out by 6 steps
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                    MovePlayer(currentPlayerIndex, 5, tokenToMoveOut); // Move out and move by 6 steps
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    // Move two tokens 1 step each from the nest

                    // First token
                    int firstToken = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(firstToken);
                    MovePlayer(currentPlayerIndex, 0, firstToken);

                    // Second token
                    if (players[currentPlayerIndex].PiecesInNest > 0) // Only if there are more pieces in the nest
                    {
                        int secondToken = GetNextTokenInNest(currentPlayerIndex);
                        players[currentPlayerIndex].MoveOutOfNest(secondToken);
                        MovePlayer(currentPlayerIndex, 0, secondToken);
                    }
                }
            }
            // Case 3: If the player has pieces on the board, move an existing piece based on the dice roll
            else if (hasPiecesOnBoard)
            {
                // Normal movement for a piece already on the board
                MovePlayer(currentPlayerIndex, diceRoll, GetNextTokenOnBoard(currentPlayerIndex));
            }

            // Move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            DiceIsEnable(currentPlayerIndex);
        }

        private int RollDice()
        {
            return random.Next(1, 7); 
        }
        private void MovePlayer(int playerIndex, int steps, int tokenIndex)
        {
            // Get the current position of the specific token
            int currentPosition = players[playerIndex].GetTokenPosition(tokenIndex);

            // If the token is already in the goal (position 99), skip movement
            if (currentPosition == 99)
            {
                return; // Skip any movement for pieces already in the goal
            }

            // If the token is still in the nest, move it to the start
            if (currentPosition == -1)
            {
                players[playerIndex].SetTokenPosition(tokenIndex, 0); // Move the token to the start of the path
                players[playerIndex].PiecesInNest--; // Decrease the number of pieces in the nest
            }
            // If the throw does not match the paces remaining to goal, moves back excess paces
            else if (currentPosition + steps > GetPlayerPath(playerIndex).Length - 1)
            {
                var path = GetPlayerPath(playerIndex);
                int moveBack = PacesToMoveBack(currentPosition, path.Length, steps);
                int newPositionOnBoard = currentPosition - moveBack;
				players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);
				Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);
				var (newRow, newCol) = path[newPositionOnBoard];
				SetTokenPosition(playerToken, newRow, newCol);
			}
            else
            {
                // Move the token forward by the number of steps
                int newPositionOnBoard = currentPosition + steps;

                // Ensure the token does not go beyond the last position on the path
                var path = GetPlayerPath(playerIndex);
                if (newPositionOnBoard >= path.Length)
                {
                    //newPositionOnBoard = PacesToMoveBack(newPositionOnBoard, path);
                    newPositionOnBoard = path.Length - 1; // Cap the position to the end of the path
                }

                // Set the new position
                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);

                // Check if the piece has reached the goal
                if (newPositionOnBoard == path.Length - 1)
                {
                    // Move the piece into the goal (mark it as 99)

                    players[playerIndex].SetTokenPosition(tokenIndex, 99);
                }

                // Move the player token visually on the board
                Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
            }
        }

        // Get the index of the next token in the nest
        private int GetNextTokenInNest(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[playerIndex].GetTokenPosition(i) == -1) // If the token is in the nest
                {
                    return i;
                }
            }
            return -1; // No tokens in the nest
        }
        // Get the index of the next token already on the board
        private int GetNextTokenOnBoard(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                int tokenPosition = players[playerIndex].GetTokenPosition(i);
                if (tokenPosition != -1 && tokenPosition != 99) // Ensure the token is not in the nest or goal
                {
                    return i;
                }
            }
            return -1; // No tokens on the board (this shouldn't happen if HasStarted is true)
        }

		private int PacesToMoveBack(int position, int pathLength, int steps)
		{
			int pacesToGoal = (pathLength - 1) - position;
			int moveBackPaces = (position + steps) - (pathLength - 1);
			return moveBackPaces - pacesToGoal; // Returns the amount of paces to go back if larger than paces to goal
		}

		private void HandlePlayerGoal(int playerIndex, int tokenIndex)
        {
            string playerColor = IndexToName(playerIndex);
            DiceRollResult.Text = $"Player {playerColor} has reached the goal with one of their pieces!";

            // Mark the token as having reached the goal
            players[playerIndex].SetTokenPosition(tokenIndex, 99); // Use 99 to mark the token in the goal

            // Check if all pieces have reached the goal
            if (players[playerIndex].AllPiecesInGoal())
            {
                players[playerIndex].HasWon = true;
                DiceRollResult.Text += $" {playerColor} has won the game!";
            }
        }

        private string IndexToName(int index)
        {
            switch (index)
            {
                case 0: return "Red"; 
                case 1: return "Blue"; 
                case 2: return "Green"; 
                case 3: return "Yellow"; 
                default: return null;
            }
        }

        private Grid GetPlayerToken(int playerIndex, int tokenIndex)
        {
            switch (playerIndex)
            {
                case 0: // Red player
                    switch (tokenIndex)
                    {
                        case 0: return Player1Token;
                        case 1: return Player1Token2;
                        case 2: return Player1Token3;
                        case 3: return Player1Token4;
                    }
                    break;
                case 1: // Blue player
                    switch (tokenIndex)
                    {
                        case 0: return Player2Token;
                        case 1: return Player2Token2;
                        case 2: return Player2Token3;
                        case 3: return Player2Token4;
                    }
                    break;
                case 2: // Green player
                    switch (tokenIndex)
                    {
                        case 0: return Player3Token;
                        case 1: return Player3Token2;
                        case 2: return Player3Token3;
                        case 3: return Player3Token4;
                    }
                    break;
                case 3: // Yellow player
                    switch (tokenIndex)
                    {
                        case 0: return Player4Token;
                        case 1: return Player4Token2;
                        case 2: return Player4Token3;
                        case 3: return Player4Token4;
                    }
                    break;
            }
            return null; // Default case if no valid token is found
        }

        private (int row, int col)[] GetPlayerPath(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0: return RedPath; 
                case 1: return BluePath; 
                case 2: return GreenPath; 
                case 3: return YellowPath; 
                default: return null;
            }
        }

        private void SetTokenPosition(Grid token, int row, int col)
        {
            Grid.SetRow(token, row);
            Grid.SetColumn(token, col);
        }

        private void Back_to_MainMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }
    }
}
