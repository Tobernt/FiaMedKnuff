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
            (9,5), (8,5),(7, 5), (6,5),(5,5)
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
            if (currentPlayerIndex >= 0 && currentPlayerIndex < diceButtons.Length)
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

            bool hasPiecesOnBoard = players[currentPlayerIndex].HasPiecesOnBoard;
            bool hasPiecesInNest = players[currentPlayerIndex].HasPiecesInNest();
            bool allPiecesInNestOrGoal = players[currentPlayerIndex].AllPiecesInNestOrGoal();

            if (diceRoll == 1 && !hasPiecesOnBoard)
            {
                int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                MovePlayer(currentPlayerIndex, diceRoll == 1 ? 0 : 5, tokenToMoveOut);
            }
            else if (diceRoll == 6 && !hasPiecesOnBoard)
            {
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
                    int tokenToMoveOut = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(tokenToMoveOut);
                    MovePlayer(currentPlayerIndex, 5, tokenToMoveOut);
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    int firstToken = GetNextTokenInNest(currentPlayerIndex);
                    players[currentPlayerIndex].MoveOutOfNest(firstToken);
                    MovePlayer(currentPlayerIndex, 0, firstToken);

                    if (players[currentPlayerIndex].PiecesInNest > 0)
                    {
                        int secondToken = GetNextTokenInNest(currentPlayerIndex);
                        players[currentPlayerIndex].MoveOutOfNest(secondToken);
                        MovePlayer(currentPlayerIndex, 0, secondToken);
                    }
                }
            }
            else if (hasPiecesOnBoard)
            {
                MovePlayer(currentPlayerIndex, diceRoll, GetNextTokenOnBoard(currentPlayerIndex));
            }

            if (diceRoll != 6)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
                DiceIsEnable(currentPlayerIndex);
            }
            else
            {
                DiceRollResult.Text += " (Player gets to roll again!)";
            }
        }

        private int RollDice()
        {
            return random.Next(1, 7);
        }
        private void MovePlayer(int playerIndex, int steps, int tokenIndex)
        {
            int currentPosition = players[playerIndex].GetTokenPosition(tokenIndex);

            if (currentPosition == 99)
            {
                return;
            }

            if (currentPosition == -1)
            {
                players[playerIndex].SetTokenPosition(tokenIndex, 0);
                players[playerIndex].PiecesInNest--;
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
                int newPositionOnBoard = currentPosition + steps;

                var path = GetPlayerPath(playerIndex);
                if (newPositionOnBoard >= path.Length)
                {
                    newPositionOnBoard = path.Length - 1;
                }

                players[playerIndex].SetTokenPosition(tokenIndex, newPositionOnBoard);

                if (newPositionOnBoard == path.Length - 1)
                {
                    players[playerIndex].SetTokenPosition(tokenIndex, 99);
                }

                Grid playerToken = GetPlayerToken(playerIndex, tokenIndex);
                var (newRow, newCol) = path[newPositionOnBoard];
                SetTokenPosition(playerToken, newRow, newCol);
            }
        }

        private int GetNextTokenInNest(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[playerIndex].GetTokenPosition(i) == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetNextTokenOnBoard(int playerIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                int tokenPosition = players[playerIndex].GetTokenPosition(i);
                if (tokenPosition != -1 && tokenPosition != 99)
                {
                    return i;
                }
            }
            return -1;
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

            players[playerIndex].SetTokenPosition(tokenIndex, 99);

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
                case 0:
                    switch (tokenIndex)
                    {
                        case 0: return Player1Token;
                        case 1: return Player1Token2;
                        case 2: return Player1Token3;
                        case 3: return Player1Token4;
                    }
                    break;
                case 1:
                    switch (tokenIndex)
                    {
                        case 0: return Player2Token;
                        case 1: return Player2Token2;
                        case 2: return Player2Token3;
                        case 3: return Player2Token4;
                    }
                    break;
                case 2:
                    switch (tokenIndex)
                    {
                        case 0: return Player3Token;
                        case 1: return Player3Token2;
                        case 2: return Player3Token3;
                        case 3: return Player3Token4;
                    }
                    break;
                case 3:
                    switch (tokenIndex)
                    {
                        case 0: return Player4Token;
                        case 1: return Player4Token2;
                        case 2: return Player4Token3;
                        case 3: return Player4Token4;
                    }
                    break;
            }
            return null;
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
