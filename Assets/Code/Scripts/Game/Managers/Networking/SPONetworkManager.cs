namespace SPO.Managers.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameConstants;
    using UnityEngine;
    using Mirror;
    using Mirror.FizzySteam;
    using kcp2k;
    using Steamworks;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main.Interfaces.Networking;
    using SPO.Managers.EventArgs;
    using SPO.Managers.Networking.Steam;
    using SPO.Player;

    [RequireComponent(typeof(FizzySteamworks))]
    [RequireComponent(typeof(FizzySteamworks))]
    public class SPONetworkManager : NetworkManager, INetworkManager, ICastGameManager<SPOGameManager>, ICastSceneManager<SPOSceneManager>
    {
        private FizzySteamworks _fizzySteamworks;
        private KcpTransport _kcpTransport;

        public List<NetPlayerController> NetPlayers { get; private set; } = new List<NetPlayerController>();

        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;
        public SPOSceneManager SceneManager => MainManager.Ins.SceneManager as SPOSceneManager;

        public static event Action<ConnectedPlayerEventArgs> OnPlayerAddedToServer;
        public static event Action<NetworkConnectionToClient> OnServerClientConnected;
        public static event Action<NetworkConnectionToClient> OnServerClientDisconnected;
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action OnServerStarted;
        public static event Action OnServerStopped;
        public static event Action<string> OnServerChangedScene;
        public static event Action OnLobbyPlayersReady;
        public static event Action OnLobbyPlayersUnready;

        public override void Awake()
        {
            base.Awake();
            TryGetComponent(out _fizzySteamworks);
            transport = _fizzySteamworks;
            maxConnections = SPOConstants.MaxConnections;
        }

        private void OnEnable()
        {
            NetPlayerController.OnServerPlayerDisconnected += OnServerPlayerDisconnected;
            NetPlayerData.OnServerPlayerUpdateReadyStatus += OnServerPlayerReadyStatusUpdate;
        }

        private void OnDisable()
        {
            NetPlayerController.OnServerPlayerDisconnected -= OnServerPlayerDisconnected;
            NetPlayerData.OnServerPlayerUpdateReadyStatus -= OnServerPlayerReadyStatusUpdate;
        }

        /// <inheritdoc/>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log("Adding player to the server...");
            
            if (IsServerFull())
            {
                Debug.Log("Server is full, cannot add more players!");
                return;
            }
            
            Debug.Log($"Player {conn.connectionId} has been added to the server!");
            ConnectedPlayerEventArgs eventArgs = new ConnectedPlayerEventArgs(conn, numPlayers);
            AddPlayer(eventArgs);
        }

        /// <inheritdoc/>
        public override void OnClientConnect()
        {
            base.OnClientConnect(); // Call the base otherwise the OnServerAddPlayer method will not be called
            Debug.Log("Client has connected to the server!");
            OnClientConnected?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnClientDisconnect()
        {
            Debug.Log("Client has disconnected from the server!");
            OnClientDisconnected?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Server has received a Client connection {conn.connectionId}");
            OnServerClientConnected?.Invoke(conn);
        }

        /// <inheritdoc/>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"Server has lost a Client connection {conn.connectionId}");
            OnServerClientDisconnected?.Invoke(conn);
            
            // DO NOT CALL StopConnection() here, it will go in a leak memory loop
        }

        /// <inheritdoc/>
        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerStarted?.Invoke();
            Debug.Log("<color=yellow>Server has started</color>");
        }

        /// <inheritdoc/>
        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerStopped?.Invoke();
            Debug.Log("<color=red>Server has stopped</color>");
        }
        
        /// <summary>
        /// Gets the number of players that are ready.
        /// </summary>
        /// <returns>The number of players that are ready.</returns>
        public int GetReadyPlayerCount()
        {
            return NetPlayers.Count(player => player.NetData.IsPlayerReady);
        }
        
        /// <summary>
        /// Checks if the server is full.
        /// </summary>
        /// <returns>True if the server is full, false otherwise.</returns>
        public bool IsServerFull()
        { 
            bool isFull = numPlayers >= maxConnections;
            return isFull;
        }
        
        /// <summary>
        /// Checks if the server is empty.
        /// </summary>
        /// <returns>True if the server is empty, false otherwise.</returns>
        public bool IsServerEmpty()
        {
            bool isEmpty = numPlayers <= 0;
            Debug.Log("Checking if server is empty: " + isEmpty);
            return isEmpty;
        }
        
        /// <summary>
        /// Starts a client connection to the server.
        /// </summary>
        /// <param name="hostAddress">The host address to connect to.</param>
        public static void StartClientConnection(string hostAddress)
        {
            if (NetworkClient.isConnected) return;
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
        }
        
        /// <summary>
        /// Starts a network "host" - a server and client in the same application with a host address.
        /// </summary>
        /// <param name="hostAddress">The host address to connect to.</param>
        public static void StartHostConnection(string hostAddress = "localhost")
        {
            if (NetworkServer.active && NetworkClient.isConnected) return;
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartHost();
        }
        
        /// <summary>
        /// Starts the server, listening for incoming connections if not already started.
        /// </summary>
        public static void StartServerConnection()
        {
            if (NetworkServer.active) return;
            NetworkManager.singleton.StartServer();
        }
        
        /// <summary>
        /// Stops the respectively the connection based on the current mode.
        /// </summary>
        public static void StopConnection()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Debug.Log("Requesting stop connection by host");
                NetworkManager.singleton.StopHost();
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                Debug.Log("Requesting stop connection by server");
                NetworkManager.singleton.StopServer();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                Debug.Log("Requesting stop connection by client");
                NetworkManager.singleton.StopClient();
            }
        }
        
        /// <summary>
        /// Event handler for when the server changes the scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene that was loaded.</param>
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            OnServerChangedScene?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Gets the local NetPlayerController.
        /// </summary>
        /// <returns>The local NetPlayerController.</returns>
        public static NetPlayerController GetLocalNetPlayer()
        {
            NetworkClient.localPlayer.TryGetComponent(out NetPlayerController localPlayer);
            return localPlayer;
        }
        
        /// <summary>
        /// Gets a random connection ID from the server.
        /// </summary>
        /// <returns>A random connection ID from the server.</returns>
        public static int GetRandomConnectionID()
        {
            if (NetworkServer.connections.Count <= 0) return -1;

            return NetworkServer.connections.ElementAt(UnityEngine.Random.Range(0, NetworkServer.connections.Count)).Key;
        }
        
        /// <summary>
        /// Adds a NetPlayerController to the server.
        /// </summary>
        /// <param name="eventArgs">The event arguments for the connected player.</param>
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
        
        /// <summary>
        /// Event handler for when a player disconnects from the server.
        /// </summary>
        private void OnServerPlayerDisconnected()
        {
            if (IsServerEmpty()) return; // Do it if server is empty
            // Do it if we are in the game scene
            if (SceneManager.NetSceneManager.IsCurrentSceneGame()) 
                StopConnection();
        }
        
        /// <summary>
        /// Event handler for when a player updates their ready status.
        /// </summary>
        private void OnServerPlayerReadyStatusUpdate()
        { 
            CheckLobby();
        }
        
        /// <summary>
        /// Checks if the lobby is ready and triggers the respective event.
        /// </summary>
        [Server]
        private void CheckLobby()
        {
            if (!SceneManager.NetSceneManager.IsCurrentSceneLobby()) return;
            
            if (ArePlayersLobbyReady())
                OnLobbyReady();
            else
                OnLobbyUnready();
        }
        
        /// <summary>
        /// Checks if all players in the lobby are ready.
        /// </summary>
        /// <returns>True if all players in the lobby are ready, false otherwise.</returns>
        private bool ArePlayersLobbyReady()
        {
            if (!IsServerFull()) return false;
            Debug.Log("Checking if all players are ready...");
            Debug.Log("Number of players ready: " + NetPlayers.Count(player => player.NetData.IsPlayerReady));
            return NetPlayers.All(player => player.NetData.IsPlayerReady);
        }

        /// <summary>
        /// Event handler for when all players in the lobby are ready.
        /// </summary>
        private void OnLobbyReady()
        {
            Debug.Log("All players are ready!");
            OnLobbyPlayersReady?.Invoke();
        }
        
        /// <summary>
        /// Event handler for when not all players in the lobby are ready.
        /// </summary>
        private void OnLobbyUnready()
        {
            Debug.Log("Not all players are ready!");
            OnLobbyPlayersUnready?.Invoke();
        }
    }
}