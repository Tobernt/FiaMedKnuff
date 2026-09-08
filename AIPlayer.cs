namespace FiaMedKnuff
{
    /// <summary>
    /// Computer player using the shared player movement rules.
    /// </summary>
    public class AIPlayer : Player
    {
        public AIPlayer(string name) : base(name) { }

        /// <summary>
        /// Uses the same movement rule as Player.
        /// </summary>
        /// <param name="steps">The number of steps to move forward.</param>
        public new void Move(int steps)
        {
            base.Move(steps);
        }
    }
}
