namespace SPO.Level.Ball
{
    using UnityEngine;
    using Mirror;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Level.Goal.Interfaces;
    using SPO.Managers.GameStats;
    using SPO.Player.Interfaces;

    public class BallMovement : NetworkBehaviour, IInit<Rigidbody2D, SPOGameStats, BallManager>
    {
        private BallManager _ballManager;
        private SPOGameStats _gameStats;
        private Rigidbody2D _rigidbody;
        private Vector2 _currentVelocity;

        private float BallSpeed => _gameStats.SyncStats.BallSpeed;
        private float BallAngleRange => _gameStats.SyncStats.BallStartAngleRange;
        private float RandomAngle => Random.Range(-BallAngleRange, BallAngleRange);

        public void Init(Rigidbody2D arg1, SPOGameStats arg2, BallManager arg3)
        {
            _rigidbody = arg1;
            _gameStats = arg2;
            _ballManager = arg3;

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.gravityScale = 0;
        }

        public bool Check()
        {
            return _rigidbody != null && _gameStats != null;
        }

        [Server]
        public virtual void Begin()
        {
            NetworkServer.connections.TryGetValue(_ballManager.AssignedPlayerID, out NetworkConnectionToClient playerConnection);
            Vector2 dir = Vector2.zero;
            if (playerConnection != null)
                dir = (_ballManager.transform.position - playerConnection.identity.transform.position).normalized;
            
            dir.y = RandomAngle;
            SetVelocity(dir * _gameStats.SyncStats.BallSpeed);
        }

        [Server]
        public void Stop()
        {
            SetVelocity(Vector2.zero);
        }

        [ServerCallback]
        protected void SetVelocity(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
            _currentVelocity = velocity;
        }

        [ServerCallback]
        protected virtual void WallBounce(Collision2D other)
        {
            Vector2 vel = _currentVelocity;
            vel.y = -vel.y;
            SetVelocity(vel);
        }

        [ServerCallback]
        protected virtual void RacketBounce(INetPlayer netPlayer, Collision2D other)
        {
            Vector3 center = Vector2.zero;
            Vector3 contactPoint = other.contacts[0].point;
    
            Vector2 arenaDir = (center - contactPoint).normalized;
            Vector2 newDir = new Vector2(arenaDir.x, (contactPoint - other.transform.position).normalized.y);

            if (Mathf.Sign(newDir.x) == Mathf.Sign(_currentVelocity.x))
                newDir.x = -newDir.x;
            
            _ballManager.AssignPlayer(netPlayer.PlayerID);
            SetVelocity(newDir * BallSpeed);
        }

        [ServerCallback]
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.TryGetComponent(out INetPlayer player))
                RacketBounce(player, other);
            else
                WallBounce(other);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IGoal goal))
                _ballManager.ScoreGoal(goal);
        }
    }
}