namespace FiaMedKnuff
{
    /// <summary>
    /// Class representing an AI player.
    /// </summary>
    public class AIPlayer : Player
    {
        public AIPlayer(string name) : base(name) { }

        /// <summary>
        /// Moves the AI player based on the dice roll.
        /// The AI can have its own logic for decision making.
        /// </summary>
        /// <param name="steps">The number of steps to move forward.</param>
        public new void Move(int steps)
        {
            //AI-specific logic can be added here (e.g., smarter movement strategy)
            base.Move(steps);
        }
    }
}
