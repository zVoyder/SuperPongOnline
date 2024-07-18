namespace SPO.Managers.GameMachine
{
    using System;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Managers.GameMachine.Data;
    using SPO.Managers.GameStats;

    [RequireComponent(typeof(SPOGameMachine))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class SPOGameNetMachineController : NetworkBehaviour, ICastNetworkManager<SPONetworkManager>, ICastGameStats<SPOGameStats>
    {
        [SyncVar (hook = nameof(OnWinnerIDChanged))]
        private int _winnerID = -1;
        private SPOGameMachine _gameMachine;
        
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        public Action<int> OnWinnerIDChangedHookReceived;
        
        private void Awake()
        {
            TryGetComponent(out _gameMachine);
            _gameMachine.Init(this);
        }

        public override void OnStartClient()
        {
            ServerGameIdle();
        }
        
        #region States

        [Server]
        public void ServerGameIdle()
        {
            _gameMachine.ChangeState(GameStateKeys.GameIdle);
            RpcGameIdle();
        }

        [Server]
        public void ServerGameBegin()
        {
            _gameMachine.ChangeState(GameStateKeys.GameBegin);
            RpcGameBegin();
        }

        [Server]
        public void ServerGamePlaying()
        {
            _gameMachine.ChangeState(GameStateKeys.GamePlaying);
            RpcGamePlaying();
        }

        [Server]
        public void ServerGameEndWinner(int winnerID)
        {
            _gameMachine.ChangeState(GameStateKeys.GameEnd);
            RpcGameEnd();
            SetWinner(winnerID);
        }

        [ClientRpc]
        public void RpcGameIdle()
        {
            _gameMachine.ChangeState(GameStateKeys.GameIdle);
        }

        [ClientRpc]
        public void RpcGameBegin()
        {
            _gameMachine.ChangeState(GameStateKeys.GameBegin);
        }

        [ClientRpc]
        public void RpcGamePlaying()
        {
            _gameMachine.ChangeState(GameStateKeys.GamePlaying);
        }
        
        [ClientRpc]
        public void RpcGameEnd()
        {
            _gameMachine.ChangeState(GameStateKeys.GameEnd);
        }
        
        #endregion

        #region Gameplay
        
        [Server]
        public void BeginBall()
        {
            NetworkManager.SpawnBall();
        }
        
        [Server]
        public void EndBall()
        {
            NetworkManager.DespawnBall();
        }
        
        [Server]
        public void TriggerGameOver()
        {
            NetworkManager.GameManager.GameoverNetManager.ServerGameOver(GetWinnerID());
        }
        
        [Server]
        public void TriggerResetGame()
        {
            NetworkManager.GameManager.GameoverNetManager.ServerResetGame();
        }
        
        [Server]
        public bool CheckVictory(int playerID, int ScoreValue)
        {
            if (ScoreValue >= GameStats.SyncStats.ScoreToWin)
            {
                Debug.Log($"<color=green>Player {playerID} has won the game!</color>");
                SetWinner(playerID);
                return true;
            }
            
            return false;
        }
        
        [Server]
        public void SetWinner(int winnerId)
        {
            _winnerID = winnerId;
        }
        
        public int GetWinnerID()
        {
            return _winnerID;
        }
        
        private void OnWinnerIDChanged(int oldValue, int newValue)
        {
            OnWinnerIDChangedHookReceived?.Invoke(newValue);
        }
        
        #endregion
    }
}