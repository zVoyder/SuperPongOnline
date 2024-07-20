namespace SPO.Managers.GameMachine
{
    using System;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Managers.GameMachine.Data;
    using SPO.Managers.GameStats;

    [RequireComponent(typeof(NetworkIdentity))]
    public class SPONetGameMachineController : NetworkBehaviour, IInit<SPOGameMachine>, ICastGameStats<SPOGameStats>, ICastGameManager<SPOGameManager>
    {
        [SyncVar (hook = nameof(OnWinnerIDChanged))]
        private int _winnerID = -1;
        private SPOGameMachine _gameMachine;
        
        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;
        
        public Action<int> OnWinnerIDChangedHookReceived;
        
        public void Init(SPOGameMachine arg)
        {
            _gameMachine = arg;
        }
        
        public bool Check()
        {
            return _gameMachine != null;
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
            GameManager.NetGameManager.SpawnBall();
        }
        
        [Server]
        public void EndBall()
        {
            GameManager.NetGameManager.DespawnBall();
        }
        
        [Server]
        public void TriggerGameOver()
        {
            GameManager.NetGameoverManager.ServerGameOver(GetWinnerID());
        }
        
        [Server]
        public void TriggerResetGame()
        {
            GameManager.NetGameoverManager.ServerResetGame();
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