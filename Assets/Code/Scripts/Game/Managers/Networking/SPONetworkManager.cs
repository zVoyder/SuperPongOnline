namespace SPO.Managers.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Mirror;
    using Mirror.FizzySteam;
    using kcp2k;
    using Steamworks;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main.Interfaces.Networking;
    using SPO.Player;
    using SPO.Managers.EventArgs;
    
    [RequireComponent(typeof(FizzySteamworks))]
    [RequireComponent(typeof(SteamManager))]
    [RequireComponent(typeof(FizzySteamworks))]
    public class SPONetworkManager : NetworkManager, INetworkManager, ICastGameManager<SPOGameManager>, ICastSceneManager<SPOSceneManager>
    {
        private SteamManager _steamManager;
        private FizzySteamworks _fizzySteamworks;
        private KcpTransport _kcpTransport;

        public List<NetPlayerController> NetPlayers { get; private set; } = new List<NetPlayerController>();

        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;
        public SPOSceneManager SceneManager => MainManager.Ins.SceneManager as SPOSceneManager;

        public static event Action<ConnectedPlayerEventArgs> OnPlayerAddedToServer;
        public static event Action<NetworkConnectionToClient> OnServerClientConnected;
        public static event Action<NetworkConnectionToClient> OnServerClientDisconnected;
        public static event Action OnServerStarted;
        public static event Action OnServerStopped;
        public static event Action<string> OnServerChangedScene;

        public override void Awake()
        {
            base.Awake();
            TryGetComponent(out _steamManager);
            TryGetComponent(out _fizzySteamworks);
            transport = _fizzySteamworks;
        }

        private void OnEnable()
        {
            NetPlayerData.OnPlayerServerUpdateReadyStatus += OnPlayerReadyStatusUpdate;
        }

        private void OnDisable()
        {
            NetPlayerData.OnPlayerServerUpdateReadyStatus -= OnPlayerReadyStatusUpdate;
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (IsServerFull()) return;
            
            Debug.Log($"Player {conn.connectionId} has been added to the server!");
            ConnectedPlayerEventArgs eventArgs = new ConnectedPlayerEventArgs(conn, numPlayers);
            AddPlayer(eventArgs);
        }

        private void AddPlayer(ConnectedPlayerEventArgs eventArgs)
        {
            GameObject goPlayer = Instantiate(playerPrefab);
            goPlayer.TryGetComponent(out NetPlayerController netPlayer);
            
            CSteamID lobbyID = SPOSteamLobbyManager.LobbyID;
            ulong playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, NetPlayers.Count);
            netPlayer.Init(eventArgs.Connection.connectionId, NetPlayers.Count + 1, playerSteamId);
            
            // NetPlayers.Add() and NetPlayers.Remove()
            // are called in the OnStartClient and OnStopClient methods respectively
            // NetPlayers.Add(netPlayer);
            
            NetworkServer.AddPlayerForConnection(eventArgs.Connection, goPlayer);
            OnPlayerAddedToServer?.Invoke(eventArgs);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"<color=green>Client {conn.connectionId} has connected to the server</color>");
            OnServerClientConnected?.Invoke(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"<color=green>Client {conn.connectionId} has disconnected from the server</color>");
            OnServerClientDisconnected?.Invoke(conn);
            
            if (SceneManager.NetSceneManager.IsGameScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path))
                StopConnection(); // Stop connection if a player disconnects during the game
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerStarted?.Invoke();
            Debug.Log("<color=yellow>Server has started</color>");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerStopped?.Invoke();
            Debug.Log("<color=red>Server has stopped</color>");
        }
        
        public bool ArePlayersReady()
        {
            // if (!IsServerFull()) return false; // Uncomment this line if you want to check if all players are ready only when the server is full
            Debug.Log("Checking if all players are ready...");
            Debug.Log("Number of players ready: " + NetPlayers.Count(player => player.NetData.IsReady));
            return NetPlayers.All(player => player.NetData.IsReady);
        }
        
        public bool IsServerFull()
        { 
            bool isFull = numPlayers >= maxConnections;
            return isFull;
        }
        
        public bool IsServerEmpty()
        {
            bool isEmpty = numPlayers <= 0;
            Debug.Log("Checking if server is empty: " + isEmpty);
            return isEmpty;
        }
        
        public static void StartClientConnection()
        {
            if (NetworkClient.isConnected) return;
            NetworkManager.singleton.StartClient();
        }
        
        public static void StartHostConnection()
        {
            if (NetworkServer.active && NetworkClient.isConnected) return;
            NetworkManager.singleton.StartHost();
        }
        
        public static void StartServerConnection()
        {
            if (NetworkServer.active) return;
            NetworkManager.singleton.StartServer();
        }
        
        public static void StopConnection()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Debug.Log("Requesting stop connection by host");
                NetworkManager.singleton.StopHost();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                Debug.Log("Requesting stop connection by client");
                NetworkManager.singleton.StopClient();
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                Debug.Log("Requesting stop connection by server");
                NetworkManager.singleton.StopServer();
            }
        }
        
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            OnServerChangedScene?.Invoke(sceneName);
        }
        
        public static NetPlayerController GetLocalNetPlayer()
        {
            NetworkClient.localPlayer.TryGetComponent(out NetPlayerController localPlayer);
            return localPlayer;
        }
        
        public static int GetRandomConnectionID()
        {
            if (NetworkServer.connections.Count <= 0) return -1;

            return NetworkServer.connections.ElementAt(UnityEngine.Random.Range(0, NetworkServer.connections.Count)).Key;
        }
        
        private void OnPlayerReadyStatusUpdate()
        {
            if (ArePlayersReady())
                LobbyReady();
            else
                LobbyUnready();
        }
        
        private void LobbyReady()
        {
            Debug.Log("All players are ready!");
            SceneManager.NetSceneManager.ChangeSceneToGame();
        }
        
        private void LobbyUnready()
        {
            Debug.Log("Not all players are ready!");
            SceneManager.NetSceneManager.StopChangingScene();
        }
    }
}