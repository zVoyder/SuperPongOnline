namespace SPO.Managers.Networking
{
    using UnityEngine;
    using Mirror;
    using Player;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Level.Ball;
    using SPO.Patterns.Factories;

    public class SPONetGameManager : NetworkBehaviour, ICastSceneManager<SPOSceneManager>, ICastNetworkManager<SPONetworkManager>
    {
        [field: Header("Game Spawns")]
        [field: SerializeField]
        public Vector2 BallSpawnPoint { get; private set; } = Vector2.zero;
        [field: SerializeField]
        public Vector2 Player1SpawnPoint { get; private set; } = new Vector2(-15f, 0);
        [field: SerializeField]
        public Vector2 Player2SpawnPoint { get; private set; } = new Vector2(15f, 0);
        
        public BallManager SpawnedBall { get; private set; }
        
        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;
        public SPOSceneManager SceneManager => MainManager.Ins.SceneManager as SPOSceneManager;
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;
        
        private void OnEnable()
        {
            SPONetworkManager.OnServerClientDisconnected += OnServerClientDisconnected;
            SPONetworkManager.OnServerChangedScene += OnServerChangedScene;
            NetPlayerData.OnServerPlayerUpdateReadyStatus += OnServerPlayerUpdateReadyStatus;
        }

        private void OnDisable()
        {
            SPONetworkManager.OnServerClientDisconnected -= OnServerClientDisconnected;
            SPONetworkManager.OnServerChangedScene -= OnServerChangedScene;
            NetPlayerData.OnServerPlayerUpdateReadyStatus -= OnServerPlayerUpdateReadyStatus;
        }

        /// <summary>
        /// Event handler for when a client disconnects from the server.
        /// </summary>
        /// <param name="conn">The connection to the client that disconnected.</param>
        private void OnServerClientDisconnected(NetworkConnectionToClient conn)
        { 
            DespawnBall();
        }
        
        /// <summary>
        /// Event handler for when the server changes the scene.
        /// </summary>
        /// <param name="scenePath">The path of the scene that was loaded.</param>
        private void OnServerChangedScene(string scenePath)
        {
            if (SceneManager.NetSceneManager.IsGameScene(scenePath))
            {
                SpawnPlayerRackets(); // Do it only when once the game scene is loaded
                StartGame();
            }
        }
        
        /// <summary>
        /// Starts the game by setting the game state to GameBegin.
        /// </summary>
        [Server]
        public void StartGame()
        {
            // The reset status of each player client is done in the NetPlayerController when the game begins
            GameManager.GameMachine.NetMachineController.ServerGameBegin();
        }
        
        /// <summary>
        /// Spawns the player rackets for all connected players.
        /// </summary>
        [Server]
        public void SpawnPlayerRackets()
        {
            Debug.Log("Spawning player rackets in SPONetGameManager for all players, count: " + NetworkManager.NetPlayers.Count);
            for (int i = 0; i < NetworkManager.NetPlayers.Count; i++)
            {
                NetPlayerController netPlayer = NetworkManager.NetPlayers[i];
                netPlayer.SpawnPlayerRacket(i == 0 ? Player1SpawnPoint : Player2SpawnPoint);
                Debug.Log($"Spawning player racket for player {netPlayer.NetData.PlayerName}");
            }
        }
        
        /// <summary>
        /// Spawns the ball in the game scene if it has not been spawned yet.
        /// </summary>
        [Server]
        public void SpawnBall()
        {
            if (SpawnedBall) return;

            SpawnedBall = GameFactory.CreateBall(SPONetworkManager.GetRandomConnectionID());
            NetworkServer.Spawn(SpawnedBall.gameObject);
            SpawnedBall.transform.position = BallSpawnPoint;
            SpawnedBall.Begin();
        }

        /// <summary>
        /// Despawns the ball in the game scene if it has been spawned.
        /// </summary>
        [Server]
        public void DespawnBall()
        {
            if (!SpawnedBall) return;
            NetworkServer.UnSpawn(SpawnedBall.gameObject);
            DisposeBall();
        }

        /// <summary>
        /// Returns the ball in the pool.
        /// </summary>
        private void DisposeBall()
        {
            if (!SpawnedBall) return;

            SpawnedBall.Dispose();
            SpawnedBall = null;
        }
        
        /// <summary>
        /// Event handler for when a player updates their ready status.
        /// </summary>
        private void OnServerPlayerUpdateReadyStatus()
        {
            if (!AreAllPlayersReadyForRematch()) return;
            
            StartGame();
        }
        
        /// <summary>
        /// Checks if all players are ready for a rematch.
        /// </summary>
        /// <returns>True if all players are ready for a rematch, false otherwise.</returns>
        private bool AreAllPlayersReadyForRematch()
        {
            if (!NetworkManager.SceneManager.NetSceneManager.IsCurrentSceneGame()) return false;
            
            foreach (NetPlayerController netPlayer in NetworkManager.NetPlayers)
                if (!netPlayer.NetData.IsPlayerReady) return false;

            return true;
        }
    }
}