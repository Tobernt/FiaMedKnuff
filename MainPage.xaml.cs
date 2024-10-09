using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        private int[] playerPositions; // Holds player positions on the track for each player
        private int currentPlayerIndex;
        private int totalPlayers = 4;
        private bool[] hasStarted;
        private Random random;

        // Paths for each player, defined as (row, column) positions on the grid.
        private readonly (int row, int col)[] RedPath = new (int row, int col)[]
        {
            // Starting from Red quadrant at (4, 0)
            (4, 0), 
            (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
            (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
            (0, 5), (0, 6),                          // Move Right in upper blue quadrant
            (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
            (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
            (5, 10), (6, 10),                        // Move Down into green quadrant
            (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
            (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
            (10, 5), (10, 4),                        // Move left into yellow quadrant
            (9, 4), (8, 4),(7, 4),                   // Move up in yellow quadrant
            (6, 4), (6, 3), (6, 2), (6, 1), (6, 0),  // Move left in yellow quadrant
            (5, 0), (5, 1),(5, 2),(5,3),(5,4),(5,5)  // Red finishing stretch
        };

        private readonly (int row, int col)[] BluePath = new (int row, int col)[]
        {
            // Starting from Blue quadrant at (0, 6), moving down
            (0, 6),
            (1, 6), (2, 6), (3, 6), (4, 6),   // Move Down in blue quadrant
            (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
            (5, 10), (6, 10),                        // Move Down into green quadrant
            (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
            (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
            (10, 5), (10, 4),                        // Move left into yellow quadrant
            (9, 4), (8, 4),(7, 4),                   // Move up in yellow quadrant
            (6, 4), (6, 3), (6, 2), (6, 1), (6, 0), (5, 0),  // Move left in yellow quadrant
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
            (3, 4), (2, 4), (1, 4), (0, 4), (0, 5),  // Move Up in Red quadrant
            (1,5), (2,5), (3,5), (4,5), (5,5)        // Blue finishing stretch
        };


        private readonly (int row, int col)[] YellowPath = new (int row, int col)[]
        {
            // Starting from Yellow quadrant at (10, 4)
            (10, 4),                                 // Move left into yellow quadrant
            (9, 4), (8, 4),(7, 4),                   // Move up in yellow quadrant
            (6, 4), (6, 3), (6, 2), (6, 1), (6, 0), (5, 0),  // Move left in yellow quadrant
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
            (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
            (0, 5), (0, 6),                          // Move Right in upper blue quadrant
            (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
            (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
            (5, 10), (6, 10),                        // Move Down into green quadrant
            (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
            (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
            (10,5), (9,5), (8,5), (6,5),(5,5)        // Yellow finishing stretch
        };



        private readonly (int row, int col)[] GreenPath = new (int row, int col)[]
        {
            // Starting from Green quadrant at (6, 10)
            (6, 10), 
            (6, 9), (6, 8), (6, 7),         // Move Left in green quadrant
            (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
            (10, 5), (10, 4),                        // Move left into yellow quadrant
            (9, 4), (8, 4),(7, 4),                   // Move up in yellow quadrant
            (6, 4), (6, 3), (6, 2), (6, 1), (6, 0), (5, 0), // Move left in yellow quadrant
            (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
            (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
            (0, 5), (0, 6),                          // Move Right in upper blue quadrant
            (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
            (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
            (5, 10),(5, 9), (5, 8), (5, 7),(5, 6), (5, 5) // Green finishing stretch
		};

        public MainPage()
        {
            this.InitializeComponent();
            playerPositions = new int[totalPlayers]; // Initialize positions for each player
            random = new Random();
            currentPlayerIndex = random.Next(0, 4); // Randomizes initial starting player
			DiceIsEnable(currentPlayerIndex);
			hasStarted = new bool[totalPlayers];
        }

        private void DiceIsEnable(int currentPlayerIndex)
        {
			//Enables only one dice at the time
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

        private void RollDice_Click(object sender, RoutedEventArgs e)
        {
            // Roll the dice and display the result
            int diceRoll = RollDice();
			RedDice.DiceVisualEffect(diceRoll);

			DiceRollResult.Text = $"Player {IndexToName(currentPlayerIndex)} rolled a {diceRoll}";

            //Checks if current player have the piece on the board or in the nest
            if (!hasStarted[currentPlayerIndex])
            {
                if(diceRoll == 1 || diceRoll == 6)
                {
                    hasStarted[currentPlayerIndex] = true;
					MovePlayer(currentPlayerIndex, diceRoll - 1);
				}
                else
                {
                    DiceRollResult.Text += " (Player must roll a 1 or 6 to start moving";
                }
            }
            else
            {
				// Move the current player based on the dice roll
				MovePlayer(currentPlayerIndex, diceRoll);
			}
            // Move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
			DiceIsEnable(currentPlayerIndex);
		}

        private int RollDice()
        {
            return random.Next(1, 7); // Roll a number between 1 and 6
        }

        private int PlayerPathToNum(int playerIndex)
        {
            return 0;
        }

        private void MovePlayer(int playerIndex, int steps)
        {

			// Get the current position of the player
            playerPositions[playerIndex] += steps;
            int position = playerPositions[playerIndex]; //Dice roll

			// Get the path based on the player index
			var path = GetPlayerPath(playerIndex);

            // Ensure the position is within the bounds of the path
            if (position >= path.Length)
            {
                position = path.Length - 1; // Stop at the last position
            }

            for (int i = 0; i < playerPositions.Length; i++)
            {
                if (playerIndex == i) // make sure we're not checking players own piece
                {
                    continue;
                }

                if (playerPositions[i] >= GetPlayerPath(i).Length)
                    playerPositions[i] = GetPlayerPath(i).Length - 1;

                var (playerR, playerC) = GetPlayerPath(playerIndex)[position]; // get players piece row & cell
                var (occupierR, occupierC) = GetPlayerPath(i)[playerPositions[i]]; // get other players piece row & cell
                Debug.WriteLine($"Player {IndexToName(i)} with step {playerPositions[i]} is at {occupierR} {occupierC} | Current move: {playerR} {playerC}");

                if (playerR == occupierR && playerC == occupierC)
                {
                    if (occupierC == 5 && occupierR == 5)
                    {
                        break;
                    }

                    Debug.WriteLine("Duplicate position found!");
                    Debug.WriteLine($"{playerR} {playerC} == {occupierR} {occupierC}");

                    // Set other player piece to 0
                    var p = GetPlayerPath(i);
                    var (nr, nc) = p[0];
                    SetTokenPosition(GetPlayerToken(i), nr, nc);
                    playerPositions[i] = 0;
                }
            }

            // Move the player token to the new position
            Ellipse playerToken = GetPlayerToken(playerIndex);
            var (newRow, newCol) = path[position];
            SetTokenPosition(playerToken, newRow, newCol);
            if (newRow == 5 && newCol == 5)
            {
                HandlePlayerGoal(playerIndex); // Call the goal function when reaching (5,5)
            }
        }
        private void HandlePlayerGoal(int playerIndex)
        {
            // You can customize this message or action based on the player's color.
            string playerColor = IndexToName(playerIndex);
            DiceRollResult.Text = $"Player {playerColor} has reached the goal!";

            // You can add additional actions like disabling the player's movements, ending the game, etc.
        }

        private string IndexToName(int index)
        {
            switch (index)
            {
                case 0: return "Red"; // Red player
                case 1: return "Blue"; // Blue player
                case 2: return "Green"; // Green player
                case 3: return "Yellow"; // Yellow player
                default: return null;
            }
        }

        private Ellipse GetPlayerToken(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0: return Player1Token; // Red player
                case 1: return Player2Token; // Blue player
                case 2: return Player3Token; // Green player
                case 3: return Player4Token; // Yellow player
                default: return null;
            }
        }

        private (int row, int col)[] GetPlayerPath(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0: return RedPath; // Red path
                case 1: return BluePath; // Blue path
                case 2: return GreenPath; // Green path
                case 3: return YellowPath; // Yellow path
                default: return null;
            }
        }

        // Method to move the player's token on the grid
        private void SetTokenPosition(Ellipse token, int row, int col)
        {
            // Set the player's token to the new row and column
            Grid.SetRow(token, row);
            Grid.SetColumn(token, col);
        }

        private void Back_to_MainMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }
    }
}
