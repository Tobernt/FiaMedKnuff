namespace FiaMedKnuff
{
    /// <summary>
    /// Class representing a player.
    /// </summary>
    public class Player
    {
        public enum PlayerType
        {
            None,
            Player,
            Computer
        }
        public PlayerType Type { get; set; }

        public string Name { get; set; }
        public int Position { get; set; }
        public bool HasWon { get; set; }
        public bool HasStarted { get; set; } // Indicates if at least one piece has started
        public int Moves { get; set; }
        public int PiecesInNest { get; set; } // Number of pieces still in the nest
        private int[] tokenPositions; // Array to store the positions of the player's 4 tokens

        /// <summary>
        /// Initializes a new player with the specified name.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public Player(string name)
        {
            Name = name;
            HasWon = false;
            HasStarted = false; // No pieces have started initially
            PiecesInNest = 4; // All 4 pieces start in the nest
            tokenPositions = new int[4]; // Initialize positions for all 4 tokens

            // Set all token positions to -1 (indicating they are in the nest)
            for (int i = 0; i < tokenPositions.Length; i++)
            {
                tokenPositions[i] = -1; // -1 indicates the token is in the nest
            }
        }

        /// <summary>
        /// Moves the player based on the dice roll.
        /// </summary>
        /// <param name="steps">The number of steps to move forward.</param>
        public void Move(int steps)
        {
            Position += steps;
            // Add logic to handle the game board, player knockout, and finishing the game
        }

        /// <summary>
        /// Moves a piece out of the nest if available.
        /// </summary>
        public void MoveOutOfNest(int tokenIndex)
        {
            if (PiecesInNest > 0)
            {
                PiecesInNest--; // Decrease the number of pieces in the nest
                SetTokenPosition(tokenIndex, 0); // Move the token to the start position
                HasStarted = true; // Mark that the player has started
            }
        }
        public bool HasPiecesOnBoard
        {
            get
            {
                // Loop through each token and check if any are on the board (not in the nest and not in the goal)
                for (int i = 0; i < tokenPositions.Length; i++)
                {
                    if (tokenPositions[i] >= 0 && tokenPositions[i] < 99) // On the board if position is between 0 and the goal
                    {
                        return true;
                    }
                }
                return false; // No pieces are on the board
            }
        }


        /// <summary>
        /// Checks if all the player's pieces are either in the goal or in the nest.
        /// </summary>
        public bool AllPiecesInNestOrGoal()
        {
            int piecesInGoalOrNest = 0;

            for (int i = 0; i < tokenPositions.Length; i++)
            {
                if (tokenPositions[i] == -1 || tokenPositions[i] == 99) // Token is in nest or goal
                {
                    piecesInGoalOrNest++;
                }
            }

            return piecesInGoalOrNest == 4; // If all 4 pieces are in nest or goal
        }
        public bool AllPiecesInGoal()
        {
            for (int i = 0; i < tokenPositions.Length; i++)
            {
                if (tokenPositions[i] != 99) // If any token is not in the goal
                {
                    return false;
                }
            }
            return true; // All pieces are in the goal
        }
        /// <summary>
        /// Gets the position of a specific token.
        /// </summary>
        /// <param name="tokenIndex">The index of the token (0-3).</param>
        /// <returns>The position of the token (-1 if it's in the nest).</returns>
        public int GetTokenPosition(int tokenIndex)
        {
            if (tokenIndex >= 0 && tokenIndex < 4)
            {
                return tokenPositions[tokenIndex];
            }
            return -1; // Invalid token index
        }

        /// <summary>
        /// Sets the position of a specific token.
        /// </summary>
        /// <param name="tokenIndex">The index of the token (0-3).</param>
        /// <param name="position">The new position of the token.</param>
        public void SetTokenPosition(int tokenIndex, int position)
        {
            if (tokenIndex >= 0 && tokenIndex < 4)
            {
                tokenPositions[tokenIndex] = position;
            }
        }

        /// <summary>
        /// Checks if the player has any pieces in the nest.
        /// </summary>
        /// <returns>True if there are pieces in the nest, false otherwise.</returns>
        public bool HasPiecesInNest()
        {
            return PiecesInNest > 0;
        }
    }
}
