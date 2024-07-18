namespace SPO.Patterns.Factories
{
    using VUDK.Generic.Managers.Main;
    using SPO.Level.Ball;
    using SPO.Patterns.Pooling;
    using UnityEngine;

    public static class GameFactory
    {
        private static SPOGamePoolsKeys GamePoolsKeys => MainManager.Ins.GamePoolsKeys as SPOGamePoolsKeys;
        
        public static BallManager CreateBall(int playerID)
        {
            BallManager ball = MainManager.Ins.PoolsManager.Pools[GamePoolsKeys.BallKey].Get<BallManager>();
            ball.Init(playerID);
            return ball;
        }
    }
}