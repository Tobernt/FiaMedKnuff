﻿using System;
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
        private Random random;

        // Paths for each player, defined as (row, column) positions on the grid, not working properly, got lazy and tried to use AI to fill out the pathing.
        private readonly (int row, int col)[] RedPath = new (int row, int col)[]
        {
    // Starting from Red quadrant at (4, 0), moving right across the bottom row of the Red quadrant
    (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
    (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
    (0, 5), (0, 6),                          // Move Right in upper blue quadrant
    (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
    (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
    (5, 10), (6, 10),                        // Move Down into green quadrant
    (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
    (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
    (10, 5), (10, 4), (9, 4), (8, 4),(7, 4), // Move left into yellow quadrant
    (6, 4), (6, 3), (6, 2), (6, 1), (6, 0),  //
    (5, 0), (5, 1),(5, 2),(5,3),(5,4),(5,5)  //
        };

        private readonly (int row, int col)[] BluePath = new (int row, int col)[]
        {
    // Starting from Red quadrant at (4, 0), moving right across the bottom row of the Red quadrant
    (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
    (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
    (0, 5), (0, 6),                          // Move Right in upper blue quadrant
    (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
    (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
    (5, 10), (6, 10),                        // Move Down into green quadrant
    (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
    (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
    (10, 5), (10, 4), (9, 4), (8, 4),(7, 4), // Move left into yellow quadrant
    (6, 4), (6, 3), (6, 2), (6, 1), (6, 0),  //
    (5, 0), (5, 1),(5, 2),(5,3),(5,4),(5,5)  //
        };


        private readonly (int row, int col)[] YellowPath = new (int row, int col)[]
        {
    // Starting from Red quadrant at (4, 0), moving right across the bottom row of the Red quadrant
    (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
    (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
    (0, 5), (0, 6),                          // Move Right in upper blue quadrant
    (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
    (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
    (5, 10), (6, 10),                        // Move Down into green quadrant
    (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
    (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
    (10, 5), (10, 4), (9, 4), (8, 4),(7, 4), // Move left into yellow quadrant
    (6, 4), (6, 3), (6, 2), (6, 1), (6, 0),  //
    (5, 0), (5, 1),(5, 2),(5,3),(5,4),(5,5)  //
        };



        private readonly (int row, int col)[] GreenPath = new (int row, int col)[]
        {
               // Starting from Red quadrant at (4, 0), moving right across the bottom row of the Red quadrant
    (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),  // Move right in Red quadrant
    (3, 4), (2, 4), (1, 4), (0, 4),          // Move Up in Red quadrant
    (0, 5), (0, 6),                          // Move Right in upper blue quadrant
    (1, 6), (2, 6), (3, 6), (4, 6),          // Move Down in blue quadrant
    (4, 7), (4, 8), (4, 9), (4, 10),         // Move Right in blue quadrant
    (5, 10), (6, 10),                        // Move Down into green quadrant
    (6, 9), (6, 8), (6, 7),                  // Move Left in green quadrant
    (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), // Move Down in green quadrant
    (10, 5), (10, 4), (9, 4), (8, 4),(7, 4), // Move left into yellow quadrant
    (6, 4), (6, 3), (6, 2), (6, 1), (6, 0),  //
    (5, 0), (5, 1),(5, 2),(5,3),(5,4),(5,5)  //
        };



        public MainPage()
        {
            this.InitializeComponent();
            playerPositions = new int[totalPlayers]; // Initialize positions for each player
            currentPlayerIndex = 0;
            random = new Random();
        }

        private void RollDice_Click(object sender, RoutedEventArgs e)
        {
            // Roll the dice and display the result
            int diceRoll = RollDice();
            DiceRollResult.Text = $"Player {currentPlayerIndex + 1} rolled a {diceRoll}";

            // Move the current player based on the dice roll
            MovePlayer(currentPlayerIndex, diceRoll);

            // Move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        }

        private int RollDice()
        {
            return random.Next(1, 7); // Roll a number between 1 and 6
        }

        private void MovePlayer(int playerIndex, int steps)
        {
            // Get the current position of the player
            playerPositions[playerIndex] += steps;
            int position = playerPositions[playerIndex];

            // Get the path based on the player index
            var path = GetPlayerPath(playerIndex);

            // Ensure the position is within the bounds of the path
            if (position >= path.Length)
            {
                position = path.Length - 1; // Stop at the last position
            }

            // Move the player token to the new position
            Ellipse playerToken = GetPlayerToken(playerIndex);
            var (newRow, newCol) = path[position];
            SetTokenPosition(playerToken, newRow, newCol);
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
    }
}
