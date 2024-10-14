namespace FiaMedKnuff
{
    /// <summary>
    /// Class representing a player.
    /// </summary>
    public class Player
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public bool HasWon {  get; set; }
        public bool HasStarted { get; set; }
        public int Moves { get; set; }

        public Player(string name)
        {
            Name = name;
            Position = 0;
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
    }
}
