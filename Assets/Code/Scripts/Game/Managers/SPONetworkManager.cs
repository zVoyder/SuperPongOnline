namespace SPO.Managers
{
    using System.Linq;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main.Interfaces.Networking;
    using SPO.Patterns.Factories;
    using SPO.Level.Ball;
    using SPO.Managers.GameStats;
    using SPO.Player.Interfaces;
    
    public class SPONetworkManager : NetworkManager, INetworkManager, ICastGameManager<SPOGameManager>, ICastGameStats<SPOGameStats>
    {
        [field: Header("Game Spawns")]
        [field: SerializeField]
        public Transform BallSpawnPoint { get; private set; }
        [field: SerializeField]
        public Transform Player1SpawnPoint { get; private set; }
        [field: SerializeField]
        public Transform Player2SpawnPoint { get; private set; }
        
        public BallManager SpawnedBall { get; private set; }
        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Vector3 spawnPoint = numPlayers is 0 ? Player1SpawnPoint.position : Player2SpawnPoint.position;
            AddPlayer(conn, spawnPoint);
            CheckGameStart();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            GameManager.GameNetMachineController.ServerGameIdle();
            DespawnBall();
        }

        public override void OnClientDisconnect()
        {
            DisposeBall();
        }

        [Server]
        public void SpawnBall()
        {
            if (SpawnedBall) return;
            
            SpawnedBall = GameFactory.CreateBall(GetRandomConnectionID());
            NetworkServer.Spawn(SpawnedBall.gameObject);
            SpawnedBall.transform.position = BallSpawnPoint.position;
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

        private void AddPlayer(NetworkConnectionToClient conn, Vector3 spawnPoint)
        {
            GameObject goPlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, goPlayer);
            
            if (goPlayer.TryGetComponent(out INetPlayer player))
                player.AssignNetPlayer(conn);
            
            Debug.Log($"Player {conn.connectionId} has joined the game");
        }
        
        [Server]
        private void CheckGameStart()
        {
            if (numPlayers == 2)
                GameManager.GameNetMachineController.ServerGameBegin();
        }

        private int GetRandomConnectionID()
        {
            if (NetworkServer.connections.Count <= 0) return -1;
            
            return NetworkServer.connections.ElementAt(Random.Range(0, NetworkServer.connections.Count)).Key;
        }
    }
}