namespace SPO.Level.Ball
{
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using VUDK.Patterns.Pooling;
    using VUDK.Patterns.Pooling.Interfaces;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Managers.GameStats;
    using SPO.Level.Goal.Interfaces;

    [RequireComponent(typeof(BallMovement))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(NetworkTransformUnreliable))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class BallManager : NetworkBehaviour, IInit<int>, ICastGameStats<SPOGameStats>, IPooledObject
    {
        private BallMovement _ballMovement;
        private NetworkTransformUnreliable _networkTransform;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        [field: SyncVar]
        public int AttackerPlayerID { get; private set; }
        public Pool RelatedPool { get; private set; }
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        private void Awake()
        {
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _ballMovement);
            TryGetComponent(out _networkTransform);
            TryGetComponent(out _collider);

            _ballMovement.Init(this, _collider, _rigidbody, GameStats);
            SetNetSyncs();
        }

        /// <summary>
        /// Initializes the ball with the attacker player ID.
        /// </summary>
        /// <param name="arg1">The attacker player ID.</param>
        public void Init(int arg1)
        {
            AssignAttackerPlayerID(arg1);
        }
        
        /// <summary>
        /// Checks if the attacker player ID is valid.
        /// </summary>
        /// <returns>True if the attacker player ID is valid, false otherwise.</returns>
        public bool Check()
        {
            return AttackerPlayerID >= 0;
        }

        /// <summary>
        /// Assigns the attacker player ID.
        /// </summary>
        /// <param name="playerID">The attacker player ID.</param>
        [Server]
        public void AssignAttackerPlayerID(int playerID)
        {
            AttackerPlayerID = playerID;
        }
        
        /// <summary>
        /// Sets the network syncs for the ball.
        /// </summary>
        private void SetNetSyncs()
        {
            // Making sure the ball is synced by the server, and the client only receives the updates
            _networkTransform.syncDirection = SyncDirection.ServerToClient;
            _networkTransform.syncMode = SyncMode.Observers;
            _ballMovement.syncDirection = SyncDirection.ServerToClient;
            _ballMovement.syncMode = SyncMode.Observers;
        }

        /// <summary>
        /// Scores a goal.
        /// </summary>
        /// <param name="goal">The receiver goal.</param>
        [Server]
        public void ScoreGoal(IGoal goal)
        {
            goal.Score(AttackerPlayerID);
        }
        
        /// <inheritdoc/>
        public void AssociatePool(Pool associatedPool)
        {
            RelatedPool = associatedPool;
        }
        
        /// <inheritdoc/>
        [Server]
        public void Dispose()
        {
            RelatedPool.Dispose(gameObject);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (!NetworkServer.active) return;
            
            _ballMovement.Stop();
        }

        /// <summary>
        /// Begins the ball behaviour.
        /// </summary>
        [Server]
        public void Begin()
        {
            _ballMovement.Begin();
        }
    }
}