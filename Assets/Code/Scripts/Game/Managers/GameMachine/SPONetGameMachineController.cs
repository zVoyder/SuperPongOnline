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

        public static event Action OnServerGameIdle;
        public static event Action OnServerGameBegin;
        public static event Action OnServerGamePlaying;
        public static event Action OnServerGameEnd;
        public static event Action OnClientGameIdle;
        public static event Action OnClientGameBegin;
        public static event Action OnClientGamePlaying;
        public static event Action OnClientGameEnd;
        
        public Action<int> OnWinnerIDChangedHookReceived;
        
        /// <summary>
        /// Initializes the net game machine controller.
        /// </summary>
        /// <param name="arg">The game machine.</param>
        public void Init(SPOGameMachine arg)
        {
            _gameMachine = arg;
        }
        
        /// <summary>
        /// Checks if the net game machine controller is correctly initialized.
        /// </summary>
        /// <returns>True if the net game machine controller is valid, false otherwise.</returns>
        public bool Check()
        {
            return _gameMachine != null;
        }
        
        #region States

        /// <summary>
        /// Sets the game state to idle.
        /// </summary>
        [Server]
        public void ServerGameIdle()
        {
            OnServerGameIdle?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameIdle);
            RpcGameIdle();
        }

        /// <summary>
        /// Sets the game state to begin.
        /// </summary>
        [Server]
        public void ServerGameBegin()
        {
            OnServerGameBegin?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameBegin);
            RpcGameBegin();
        }

        /// <summary>
        /// Sets the game state to playing.
        /// </summary>
        [Server]
        public void ServerGamePlaying()
        {
            OnServerGamePlaying?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GamePlaying);
            RpcGamePlaying();
        }

        /// <summary>
        /// Sets the game state to end with a winner.
        /// </summary>
        /// <param name="winnerID">The player ID of the winner.</param>
        [Server]
        public void ServerGameEndWinner(int winnerID)
        {
            OnServerGameEnd?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameEnd);
            RpcGameEnd();
            SetWinner(winnerID); // Change the winner after because the sync var hook will be called in game end state
        }

        /// <summary>
        /// RPC for setting the game state to idle.
        /// </summary>
        [ClientRpc]
        public void RpcGameIdle()
        {
            OnClientGameIdle?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameIdle);
        }

        /// <summary>
        /// RPC for setting the game state to begin.
        /// </summary>
        [ClientRpc]
        public void RpcGameBegin()
        {
            OnClientGameBegin?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameBegin);
        }

        /// <summary>
        /// RPC for setting the game state to playing.
        /// </summary>
        [ClientRpc]
        public void RpcGamePlaying()
        {
            OnClientGamePlaying?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GamePlaying);
        }
        
        /// <summary>
        /// RPC for setting the game state to end.
        /// </summary>
        [ClientRpc]
        public void RpcGameEnd()
        {
            OnClientGameEnd?.Invoke();
            _gameMachine.ChangeState(GameStateKeys.GameEnd);
        }
        
        #endregion

        #region Gameplay
        
        /// <summary>
        /// Spawns the ball and begins its behaviour.
        /// </summary>
        [Server]
        public void BeginBall()
        {
            GameManager.NetGameManager.SpawnBall();
        }
        
        /// <summary>
        /// Despawns the ball and stops its behaviour.
        /// </summary>
        [Server]
        public void EndBall()
        {
            GameManager.NetGameManager.DespawnBall();
        }
        
        /// <summary>
        /// Triggers the game over.
        /// </summary>
        [Server]
        public void TriggerGameOver()
        {
            GameManager.NetGameoverManager.ServerGameOver(GetWinnerID());
        }
        
        /// <summary>
        /// Triggers the game reset for a new round.
        /// </summary>
        [Server]
        public void TriggerResetGame()
        {
            GameManager.NetGameoverManager.ServerResetGame();
        }
        
        /// <summary>
        /// Checks if a player has won the game.
        /// </summary>
        /// <param name="playerID">The player ID to check.</param>
        /// <param name="ScoreValue">The score value to check.</param>
        /// <returns>True if the player has won the game, false otherwise.</returns>
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
        
        /// <summary>
        /// Sets the winner of the game.
        /// </summary>
        /// <param name="winnerId">The player ID of the winner.</param>
        [Server]
        public void SetWinner(int winnerId)
        {
            _winnerID = winnerId;
        }
        
        /// <summary>
        /// Gets the winner of the game.
        /// </summary>
        /// <returns>The player ID of the winner.</returns>
        public int GetWinnerID()
        {
            return _winnerID;
        }
        
        /// <summary>
        /// SyncVar event handler for when the winner ID is changed.
        /// </summary>
        /// <param name="oldValue">The old value of the winner ID.</param>
        /// <param name="newValue">The new value of the winner ID.</param>
        private void OnWinnerIDChanged(int oldValue, int newValue)
        {
            OnWinnerIDChangedHookReceived?.Invoke(newValue);
        }
        
        #endregion
    }
}