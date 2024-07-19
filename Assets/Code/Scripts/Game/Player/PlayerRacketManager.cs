namespace SPO.Player
{
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Managers.GameStats;
    using VUDK.Patterns.Initialization.Interfaces;

    [RequireComponent(typeof(RacketMovement))]
    [RequireComponent(typeof(RacketGraphicsController))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransformUnreliable))]
    public class PlayerRacketManager : NetworkBehaviour, ICastGameStats<SPOGameStats>, IInit<NetPlayerData>
    {
        private RacketMovement _racketMovement;
        private RacketGraphicsController _graphicsController;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private NetworkIdentity _networkIdentity;
        private NetworkTransformBase _networkTransform;
        
        public NetPlayerData PlayerData { get; private set; }
        public int PlayerID => PlayerData.ConnectionID;
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        private void Awake()
        {
            TryGetComponent(out _networkTransform);
            TryGetComponent(out _networkIdentity);
            TryGetComponent(out _racketMovement);
            TryGetComponent(out _collider);
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _graphicsController);
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _racketMovement.Init(this, GameStats, _rigidbody);
            _graphicsController.Init(_spriteRenderer);
            SetNetSyncs();
        }
        
        public void Init(NetPlayerData arg)
        {
            PlayerData = arg;
        }
        
        public bool Check()
        {
            return PlayerData != null;
        }
        
        private void SetNetSyncs()
        {
            syncDirection = SyncDirection.ServerToClient;
            syncMode = SyncMode.Observers; // Sync to all clients
            _networkTransform.syncDirection = SyncDirection.ServerToClient;
            _networkTransform.syncMode = SyncMode.Owner; // Sync position to the owner client
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
                OnLocalPlayer();
            else
                OnRemotePlayer();
        }
        
        private void OnLocalPlayer()
        {
            _graphicsController.AssignColor(GameStats.LocalPlayerColor);
        }
        
        private void OnRemotePlayer()
        {
            _graphicsController.AssignColor(GameStats.RemotePlayerColor);
        }
    }
}