namespace SPO.Player
{
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Managers.GameStats;
    using SPO.Player.Interfaces;

    [RequireComponent(typeof(RacketMovement))]
    [RequireComponent(typeof(PlayerGraphicsController))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransformUnreliable))]
    public class PlayerManager : NetworkBehaviour, INetPlayer, ICastGameStats<SPOGameStats>
    {
        private RacketMovement _racketMovement;
        private PlayerGraphicsController _graphicsController;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private NetworkIdentity _networkIdentity;

        [field: SyncVar]
        public int PlayerID { get; private set; }
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        private void Awake()
        {
            TryGetComponent(out _networkIdentity);
            TryGetComponent(out _racketMovement);
            TryGetComponent(out _collider);
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _graphicsController);
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _racketMovement.Init(this, GameStats, _rigidbody);
            _graphicsController.Init(_spriteRenderer);
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
                OnLocalPlayer();
            else
                OnRemotePlayer();
        }

        public static int GetLocalPlayerID()
        {
            if (NetworkClient.localPlayer.TryGetComponent(out PlayerManager playerManager))
                return playerManager.PlayerID;
            
            return -1;
        }
        
        [Server]
        public void AssignNetPlayer(NetworkConnectionToClient connectionToClient)
        {
            PlayerID = connectionToClient.connectionId;
            // _networkIdentity.AssignClientAuthority(connectionToClient); // Already did by the NetworkManager
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