namespace SPO.Managers.Networking
{
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Level.Ball;
    using SPO.Patterns.Factories;

    public class SPONetGameManager : NetworkBehaviour, ICastGameManager<SPOGameManager>
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

        private void OnEnable()
        {
            SPONetworkManager.OnServerClientDisconnected += OnServerClientDisconnected;
        }
        
        private void OnDisable()
        {
            SPONetworkManager.OnServerClientDisconnected -= OnServerClientDisconnected;
        }

        private void OnServerClientDisconnected(NetworkConnectionToClient obj)
        { 
            DespawnBall();
        }
        
        [Server]
        public void StartGame()
        {
            GameManager.NetGameMachineController.ServerGameBegin();
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
    }
}