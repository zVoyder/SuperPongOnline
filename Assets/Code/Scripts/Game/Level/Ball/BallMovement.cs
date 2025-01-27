namespace SPO.Level.Ball
{
    using UnityEngine;
    using Mirror;
    using Player;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Level.Goal.Interfaces;
    using SPO.Managers.GameStats;

    public class BallMovement : NetworkBehaviour, IInit<BallManager, Collider2D, Rigidbody2D, SPOGameStats>
    {
        [SyncVar]
        private float _currentSpeedMultiplier;
        private BallManager _ballManager;
        private SPOGameStats _gameStats;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private Vector2 _currentVelocity;
        
        private float BallSpeed => _gameStats.SyncStats.BallSpeed * _currentSpeedMultiplier;
        private float BallAngleRange => _gameStats.SyncStats.BallStartAngleRange;
        private float RandomAngle => Random.Range(-BallAngleRange, BallAngleRange);

        /// <summary>
        /// Initializes the ball movement.
        /// </summary>
        /// <param name="arg1">The ball manager.</param>
        /// <param name="arg2">The collider.</param>
        /// <param name="arg3">The rigidbody.</param>
        /// <param name="arg4">The game stats.</param>
        public void Init(BallManager arg1, Collider2D arg2, Rigidbody2D arg3, SPOGameStats arg4)
        {
            _ballManager = arg1;
            _collider = arg2;
            _rigidbody = arg3;
            _gameStats = arg4;

            _collider.isTrigger = true;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.gravityScale = 0;
        }

        /// <summary>
        /// Checks if the ball movement is correctly initialized.
        /// </summary>
        /// <returns>True if the ball movement is valid, false otherwise.</returns>
        public bool Check()
        {
            return _rigidbody != null && _gameStats != null;
        }

        /// <summary>
        /// Begins the ball movement.
        /// </summary>
        [Server]
        public virtual void Begin()
        {
            _currentSpeedMultiplier = 1f;
            NetworkServer.connections.TryGetValue(_ballManager.AttackerPlayerID, out NetworkConnectionToClient playerConnection);
            Vector2 dir = Vector2.zero;
            if (playerConnection != null)
                dir = (_ballManager.transform.position - playerConnection.identity.transform.position).normalized;
            
            dir.y = RandomAngle;
            SetVelocity(dir * _gameStats.SyncStats.BallSpeed);
        }

        /// <summary>
        /// Stops the ball movement.
        /// </summary>
        [Server]
        public void Stop()
        {
            SetVelocity(Vector2.zero);
        }

        /// <summary>
        /// Sets the velocity of the ball.
        /// </summary>
        /// <param name="velocity">The velocity to set.</param>
        [Server]
        protected void SetVelocity(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
            _currentVelocity = velocity;
        }

        /// <summary>
        /// Bounces the ball off the wall.
        /// </summary>
        /// <param name="other">The collider of the wall.</param>
        [ServerCallback]
        protected virtual void WallBounce(Collider2D other)
        {
            Vector2 vel = _currentVelocity;
            vel.y = -vel.y;
            SetVelocity(vel);
        }

        /// <summary>
        /// Bounces the ball off the racket.
        /// </summary>
        /// <param name="playerID">The player ID of the racket.</param>
        /// <param name="other">The collider of the racket.</param>
        [ServerCallback]
        protected virtual void RacketBounce(int playerID, Collider2D other)
        {
            IncreaseSpeedMultiplier();
            Vector3 center = Vector2.zero;
            Vector3 closestPoint = other.ClosestPoint(center);
    
            Vector2 arenaDir = (center - closestPoint).normalized;
            Vector2 newDir = new Vector2(arenaDir.x, RandomAngle);

            if (Mathf.Sign(newDir.x) == Mathf.Sign(_currentVelocity.x))
                newDir.x = -newDir.x;
            
            _ballManager.AssignAttackerPlayerID(playerID);
            SetVelocity(newDir * BallSpeed);
        }
        
        /// <summary>
        /// Increases the speed multiplier of the ball.
        /// </summary>
        [Server]
        private void IncreaseSpeedMultiplier()
        {
            _currentSpeedMultiplier *= _gameStats.SyncStats.BallSpeedMultiplier;
        }
        
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerRacketManager player))
                RacketBounce(player.PlayerID, other);
            else
                if (other.TryGetComponent(out IGoal goal))
                    _ballManager.ScoreGoal(goal);
                else
                    WallBounce(other);
        }
    }
}