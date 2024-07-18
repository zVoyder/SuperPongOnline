namespace SPO.Level.Ball
{
    using System;
    using Goal.Interfaces;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using VUDK.Patterns.Pooling;
    using VUDK.Patterns.Pooling.Interfaces;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Managers.GameStats;

    [RequireComponent(typeof(BallMovement))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkTransformUnreliable))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class BallManager : NetworkBehaviour, IInit<int>, ICastGameStats<SPOGameStats>, IPooledObject
    {
        private BallMovement _ballMovement;
        private NetworkTransformUnreliable _networkTransform;
        private Rigidbody2D _rigidbody;

        [field: SyncVar (hook = nameof(OnPlayerIDChanged))]
        public int AssignedPlayerID { get; private set; }
        public Pool RelatedPool { get; private set; }
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;

        public Action<int> OnAssignedNewPlayerID;
        
        private void Awake()
        {
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _ballMovement);
            TryGetComponent(out _networkTransform);
            
            _ballMovement.Init(_rigidbody, GameStats, this);
            
            // Making sure the ball is synced by the server, and the client only receives the updates
            _networkTransform.syncDirection = SyncDirection.ServerToClient;
            _networkTransform.syncMode = SyncMode.Observers;
        }
        
        public void Init(int arg1)
        {
            AssignPlayer(arg1);
        }
        
        public bool Check()
        {
            return AssignedPlayerID < 0;
        }

        [Server]
        public void AssignPlayer(int playerID)
        {
            AssignedPlayerID = playerID;
        }
        
        private void OnPlayerIDChanged(int oldValue, int newValue)
        {
            OnAssignedNewPlayerID?.Invoke(newValue);
        }

        [Server]
        public void ScoreGoal(IGoal goal)
        {
            goal.Score(AssignedPlayerID);
        }
        
        public void AssociatePool(Pool associatedPool)
        {
            RelatedPool = associatedPool;
        }
        
        [Server]
        public void Dispose()
        {
            RelatedPool.Dispose(gameObject);
        }

        public void Clear()
        {
            if (!NetworkServer.active) return;
            
            _ballMovement.Stop();
        }

        [Server]
        public void Begin()
        {
            _ballMovement.Begin();
        }
    }
}