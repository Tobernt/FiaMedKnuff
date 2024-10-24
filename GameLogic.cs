using System;

namespace FiaMedKnuff
{
    /// <summary>
    /// Handles the core game logic, such as dice rolling, player movement, and AI turns.
    /// </summary>
    public class GameLogic
    {
        private Player[] players;
        private AIPlayer[] aiPlayers;
        private int currentPlayerIndex;
        private Random random;

        public GameLogic(int totalPlayers, int totalAIPlayers)
        {
            // Initialize players and AI
            players = new Player[totalPlayers];
            aiPlayers = new AIPlayer[totalAIPlayers];
            for (int i = 0; i < totalPlayers; i++)
            {
                players[i] = new Player($"Player {i + 1}");
            }

            for (int i = 0; i < totalAIPlayers; i++)
            {
                aiPlayers[i] = new AIPlayer($"AI {i + 1}");
            }

            currentPlayerIndex = 0;
            random = new Random();
        }

        /// <summary>
        /// Rolls a six-sided dice.
        /// </summary>
        /// <returns>Returns a random number between 1 and 6.</returns>
        public int RollDice()
        {
            return random.Next(1, 7);
        }

        /// <summary>
        /// Moves the player by the given number of steps.
        /// </summary>
        /// <param name="playerIndex">The index of the player to move.</param>
        /// <param name="steps">The number of steps to move forward.</param>
        public void MovePlayer(int playerIndex, int steps)
        {
            if (playerIndex < players.Length)
            {
                players[playerIndex].Move(steps);
            }
            else
            {
                int aiIndex = playerIndex - players.Length;
                aiPlayers[aiIndex].Move(steps);
            }
        }

        /// <summary>
        /// Checks if the current player has won the game.
        /// </summary>
        public bool HasWon(int playerIndex)
        {
            return players[playerIndex].HasWon;
        }

        /// <summary>
        /// Gets the current player's index.
        /// </summary>
        public int GetCurrentPlayerIndex()
        {
            return currentPlayerIndex;
        }

        /// <summary>
        /// Moves to the next player's turn.
        /// </summary>
        public void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % (players.Length + aiPlayers.Length);
        }
    }
}
