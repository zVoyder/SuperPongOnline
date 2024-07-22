namespace SPO.Patterns.Factories
{
    using Player;
    using VUDK.Generic.Managers.Main;
    using SPO.Level.Ball;
    using SPO.Patterns.Pooling;
    using UnityEngine;

    /// <summary>
    /// Responsible for creating game objects.
    /// </summary>
    public static class GameFactory
    {
        private static SPOGamePoolsKeys GamePoolsKeys => MainManager.Ins.GamePoolsKeys as SPOGamePoolsKeys;
        
        /// <summary>
        /// Creates a ball object.
        /// </summary>
        /// <param name="playerID">The attacker player ID.</param>
        /// <returns>The created ball object.</returns>
        public static BallManager CreateBall(int playerID)
        {
            BallManager ball = MainManager.Ins.PoolsManager.Pools[GamePoolsKeys.BallKey].Get<BallManager>();
            ball.Init(playerID);
            return ball;
        }
    }
}