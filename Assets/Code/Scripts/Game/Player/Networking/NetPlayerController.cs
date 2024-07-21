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

        public void Init(int connectionID, int playerIdNumber, ulong playerSteamId)
        {
            NetData.Init(connectionID, playerIdNumber, playerSteamId);
        }

        public override void OnStartAuthority()
        {
            OnPlayerStartAuthority?.Invoke();
        }

        public override void OnStopAuthority()
        {
            OnPlayerStopAuthority?.Invoke();
        }

        public override void OnStartServer()
        {
            OnServerPlayerConnected?.Invoke();
        }

        public override void OnStopServer()
        {
            OnServerPlayerDisconnected?.Invoke();
        }

        public override void OnStartClient()
        {
            OnPlayerStartClient?.Invoke();
            NetworkManager.NetPlayers.Add(this);
        }

        public override void OnStopClient()
        {
            OnPlayerStopClient?.Invoke();
            NetworkManager.NetPlayers.Remove(this);
            DespawnPlayerRacket();
        }

        [Server]
        public void SpawnPlayerRacket(Vector2 position)
        {
            GameObject goPlayerRacket = Instantiate(_playerRacketPrefab, position, Quaternion.identity);
            goPlayerRacket.TryGetComponent(out PlayerRacketManager playerRacket);
            playerRacket.Init(NetData);
            PlayerRacket = playerRacket;
            NetworkServer.Spawn(goPlayerRacket, connectionToClient);
        }
        
        [Server]
        public void DespawnPlayerRacket()
        {
            if (PlayerRacket == null) return;
            NetworkServer.Destroy(PlayerRacket.gameObject);
            PlayerRacket = null;
        }

        public static int GetLocalPlayerID()
        {
            if (NetworkClient.localPlayer.TryGetComponent(out NetPlayerController netPlayer))
                return netPlayer.NetData.ConnectionID;

            return -1;
        }
        
        private void OnClientGameBegin()
        {
            OnResetStatus();
        }
        
        private void OnResetStatus()
        {
            NetData.SetReadyStatus(false);
        }
    }
}