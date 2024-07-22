namespace SPO.Player
{
    using System;
    using Managers.GameMachine;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Mirror;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using SPO.Managers.Networking;

    [RequireComponent(typeof(NetPlayerData))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetPlayerController : NetworkBehaviour, ICastNetworkManager<SPONetworkManager>
    {
        [Header("Player Prefabs")]
        [SerializeField]
        private GameObject _playerRacketPrefab;

        private NetworkIdentity _networkIdentity;

        public PlayerRacketManager PlayerRacket { get; private set; }
        public NetPlayerData NetData { get; private set; }
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;

        public static event Action OnPlayerStartAuthority;
        public static event Action OnPlayerStopAuthority;
        public static event Action OnPlayerStartClient;
        public static event Action OnPlayerStopClient;
        public static event Action OnServerPlayerConnected;
        public static event Action OnServerPlayerDisconnected;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            TryGetComponent(out _networkIdentity);
            TryGetComponent(out NetPlayerData netData);
            NetData = netData;
        }

        private void OnEnable()
        {
            SPONetGameMachineController.OnClientGameBegin += OnClientGameBegin;
        }

        private void OnDisable()
        {
            SPONetGameMachineController.OnClientGameBegin -= OnClientGameBegin;
        }

        /// <summary>
        /// Initializes the net player controller with its data.
        /// </summary>
        /// <param name="connectionID">The connection ID of the player. </param>
        /// <param name="playerIdNumber">The player ID number.</param>
        /// <param name="playerSteamId">The Steam ID of the player.</param>
        public void Init(int connectionID, int playerIdNumber, ulong playerSteamId)
        {
            NetData.Init(connectionID, playerIdNumber, playerSteamId);
        }

        /// <inheritdoc/>
        public override void OnStartAuthority()
        {
            OnPlayerStartAuthority?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnStopAuthority()
        {
            OnPlayerStopAuthority?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnStartServer()
        {
            OnServerPlayerConnected?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnStopServer()
        {
            OnServerPlayerDisconnected?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnStartClient()
        {
            OnPlayerStartClient?.Invoke();
            NetworkManager.NetPlayers.Add(this);
        }

        /// <inheritdoc/>
        public override void OnStopClient()
        {
            OnPlayerStopClient?.Invoke();
            NetworkManager.NetPlayers.Remove(this);
            DespawnPlayerRacket();
        }

        /// <summary>
        /// Spawns the player racket for this net player controller.
        /// </summary>
        /// <param name="position">The position where the player racket will be spawned.</param>
        [Server]
        public void SpawnPlayerRacket(Vector2 position)
        {
            GameObject goPlayerRacket = Instantiate(_playerRacketPrefab, position, Quaternion.identity);
            goPlayerRacket.TryGetComponent(out PlayerRacketManager playerRacket);
            playerRacket.Init(NetData);
            PlayerRacket = playerRacket;
            NetworkServer.Spawn(goPlayerRacket, connectionToClient);
        }
        
        /// <summary>
        /// Despawns the player racket for this net player controller.
        /// </summary>
        [Server]
        public void DespawnPlayerRacket()
        {
            if (PlayerRacket == null) return;
            NetworkServer.Destroy(PlayerRacket.gameObject);
            PlayerRacket = null;
        }

        /// <summary>
        /// Returns the connection ID of the local player.
        /// </summary>
        /// <returns>The connection ID of the local player.</returns>
        public static int GetLocalPlayerID()
        {
            if (NetworkClient.localPlayer.TryGetComponent(out NetPlayerController netPlayer))
                return netPlayer.NetData.ConnectionID;

            return -1;
        }
        
        /// <summary>
        /// Event handler for when the client game begins.
        /// </summary>
        private void OnClientGameBegin()
        {
            OnResetStatus();
        }
        
        /// <summary>
        /// Event handler for when a player resets their status.
        /// </summary>
        private void OnResetStatus()
        {
            NetData.SetReadyStatus(false);
        }
    }
}