namespace SPO.Level.Goal.Interfaces
{
    public interface IGoal
    {
        /// <summary>
        /// Score a goal for the player.
        /// </summary>
        /// <param name="playerID">The player ID that scored the goal.</param>
        void Score(int playerID);
    }
}