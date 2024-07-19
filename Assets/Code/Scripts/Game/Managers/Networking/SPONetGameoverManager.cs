namespace SPO.Managers.Networking
{
    using Mirror;
    using UnityEngine;
    using SPO.Player;
    using SPO.GameConstants;
    using VUDK.Features.Main.EventSystem;

    public class SPONetGameoverManager : NetworkBehaviour
    {
        [Server]
        public void ServerGameOver(int winnerID)
        {
            EventManager.Ins.TriggerEvent(SPOServerEvents.OnGameOver, winnerID);
            RpcGameOver(winnerID);
        }

        [ClientRpc]
        public void RpcGameOver(int winnerID)
        {
            if (winnerID == NetPlayerController.GetLocalPlayerID())
            {
                Camera.main.backgroundColor = Color.green; // TODO: Just for testing purposes
                Debug.Log("You win!");
            }
            else
            {
                Camera.main.backgroundColor = Color.red; // TODO: Just for testing purposes
                Debug.Log("You lose!");
            }
        }

        [Server]
        public void ServerResetGame()
        {
            EventManager.Ins.TriggerEvent(SPOServerEvents.OnGameReset);
            
            RpcResetGame();
        }

        [ClientRpc]
        public void RpcResetGame()
        {
            Camera.main.backgroundColor = Color.black; // TODO: Just for testing purposes
        }
    }
}