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

        private void OnServerClientDisconnected(NetworkConnectionToClient conn)
        { 
            DespawnBall();
        }
        
        private void OnServerChangedScene(string scenePath)
        {
            if (SceneManager.NetSceneManager.IsGameScene(scenePath))
            {
                SpawnPlayerRackets(); // Do it only when once the game scene is loaded
                StartGame();
            }
        }
        
        [Server]
        public void StartGame()
        {
            // The reset status of each player client is done in the NetPlayerController when the game begins
            GameManager.GameMachine.NetMachineController.ServerGameBegin();
        }
        
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
        
        [Server]
        public void SpawnBall()
        {
            if (SpawnedBall) return;

            SpawnedBall = GameFactory.CreateBall(SPONetworkManager.GetRandomConnectionID());
            NetworkServer.Spawn(SpawnedBall.gameObject);
            SpawnedBall.transform.position = BallSpawnPoint;
            SpawnedBall.Begin();
        }

        [Server]
        public void DespawnBall()
        {
            if (!SpawnedBall) return;
            NetworkServer.UnSpawn(SpawnedBall.gameObject);
            DisposeBall();
        }

        private void DisposeBall()
        {
            if (!SpawnedBall) return;

            SpawnedBall.Dispose();
            SpawnedBall = null;
        }
        
        private void OnServerPlayerUpdateReadyStatus()
        {
            if (!AreAllPlayersReadyForRematch()) return;
            
            StartGame();
        }
        
        private bool AreAllPlayersReadyForRematch()
        {
            if (!NetworkManager.SceneManager.NetSceneManager.IsCurrentSceneGame()) return false;
            
            foreach (NetPlayerController netPlayer in NetworkManager.NetPlayers)
                if (!netPlayer.NetData.IsPlayerReady) return false;

            return true;
        }
    }
}