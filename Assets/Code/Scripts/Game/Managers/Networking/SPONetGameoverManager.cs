namespace SPO.Managers.Networking
{
    using System;
    using Mirror;
    using UnityEngine;
    using SPO.Player;

    public class SPONetGameoverManager : NetworkBehaviour
    {
        public static event Action OnPlayerClientWin;
        public static event Action OnPlayerClientLose;
        public static event Action<int> OnServerGameover;
        public static event Action<int, bool> OnClientGameover;
        public static event Action OnServerResetGame;
        public static event Action OnClientResetGame;
        
        /// <summary>
        /// Triggers the game over event for every player.
        /// </summary>
        /// <param name="winnerID">The ID of the player that won the game.</param>
        [Server]
        public void ServerGameOver(int winnerID)
        {
            OnServerGameover?.Invoke(winnerID);
            RpcGameOver(winnerID);
        }

        /// <summary>
        /// RPC for triggering the game over.
        /// </summary>
        /// <param name="winnerID">The ID of the player that won the game.</param>
        [ClientRpc]
        public void RpcGameOver(int winnerID)
        {
            
            if (winnerID == NetPlayerController.GetLocalPlayerID())
            {
                Debug.Log("You win!");
                OnPlayerClientWin?.Invoke();
                OnClientGameover?.Invoke(winnerID, true);
                Camera.main.backgroundColor = Color.green;
            }
            else
            {
                Debug.Log("You lose!");
                OnPlayerClientLose?.Invoke();
                OnClientGameover?.Invoke(winnerID, false);
                Camera.main.backgroundColor = Color.red;
            }
        }

        /// <summary>
        /// Triggers the game reset event for every player.
        /// </summary>
        [Server]
        public void ServerResetGame()
        {
            OnServerResetGame?.Invoke();
            RpcResetGame();
        }

        /// <summary>
        /// RPC for triggering the game reset.
        /// </summary>
        [ClientRpc]
        public void RpcResetGame()
        {
            OnClientResetGame?.Invoke();
            Camera.main.backgroundColor = Color.black;
        }
    }
}