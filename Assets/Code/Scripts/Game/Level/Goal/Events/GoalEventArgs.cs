namespace SPO.Level.Goal.Events
{
    using System;

    public class GoalEventArgs : EventArgs
    {
        public int PlayerID { get; private set; }
        public int ScoreValue { get; private set; }

        public GoalEventArgs(int playerID, int scoreValue)
        {
            PlayerID = playerID;
            ScoreValue = scoreValue;
        }
    }
}