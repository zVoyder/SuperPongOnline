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

        public override void Awake()
        {
            base.Awake();
            TryGetComponent(out _steamManager);
            TryGetComponent(out _fizzySteamworks);
            transport = _fizzySteamworks;
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (!SceneManager.NetSceneManager.IsLobbyScene())
            {
                Debug.LogWarning("A Player has tried to join the server from a scene that is not the lobby!");
                return;
            }
            
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
            // NetPlayers.Add(netPlayer);
            
            NetworkServer.AddPlayerForConnection(eventArgs.Connection, goPlayer);
            OnPlayerAddedToServer?.Invoke(eventArgs);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Client {conn.connectionId} has connected to the server");
            OnServerClientConnected?.Invoke(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"Client {conn.connectionId} has disconnected from the server");
            OnServerClientDisconnected?.Invoke(conn);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerStarted?.Invoke();
            Debug.Log("<color=green>Server has started</color>");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerStopped?.Invoke();
            Debug.Log("<color=red>Server has stopped</color>");
        }

        public static void StopConnection()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }
        }

        public static int GetRandomConnectionID()
        {
            if (NetworkServer.connections.Count <= 0) return -1;

            return NetworkServer.connections.ElementAt(UnityEngine.Random.Range(0, NetworkServer.connections.Count)).Key;
        }

        // TODO: Add method for spawn player brackets when on game scene,
        // an idea could be to use the NetPlayerController to spawn the playerManager
        // private void SpawnPlayer(NetworkConnectionToClient conn, Vector3 spawnPoint)
        // {
        //     GameObject goPlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        //     NetworkServer.AddPlayerForConnection(conn, goPlayer);
        //
        //     if (goPlayer.TryGetComponent(out INetPlayer player))
        //         player.AssignNetPlayer(conn);
        // }
    }
}