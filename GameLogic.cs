using System;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.UI.Xaml.Controls.Maps;

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

        public GameLogic()
        {
            // Initialize players and AI
            players = new Player[2]; // Example: 2 human players
            aiPlayers = new AIPlayer[2]; // Example: 2 AI players
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
        /// Handles the logic for processing a turn based on dice roll.
        /// </summary>
        /// <param name="diceRoll">The result of the dice roll.</param>
        public void ProcessTurn(int diceRoll)
        {
            if (currentPlayerIndex < players.Length)
            {
                // It's a human player's turn
                players[currentPlayerIndex].Move(diceRoll);
            }
            else
            {
                // It's an AI's turn
                int aiIndex = currentPlayerIndex - players.Length;
                aiPlayers[aiIndex].Move(diceRoll);
            }

            // Move to the next turn
            NextTurn();
        }

        /// <summary>
        /// Moves to the next player's turn.
        /// </summary>
        private void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % (players.Length + aiPlayers.Length);
        }
    }
}
