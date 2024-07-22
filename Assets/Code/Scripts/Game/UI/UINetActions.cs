namespace SPO.UI
{
    using UnityEngine;
    using SPO.Player;
    using SPO.Managers.Networking;
    using SPO.Managers.Networking.Steam;

    public class UINetActions : MonoBehaviour
    {
        /// <summary>
        /// Starts a casual match.
        /// </summary>
        public void CasualMatchButton()
        {
            SPOSteamMatchmaker.StartMatchmaking();
        }
        
        /// <summary>
        /// Starts a private match.
        /// </summary>
        public void PrivateMatchButton()
        {
            SPOSteamLobbyManager.CreatePrivateLobby();
        }
        
        /// <summary>
        /// Starts a public match.
        /// </summary>
        public void PublicMatchButton()
        {
            SPOSteamLobbyManager.CreatePublicLobby();
        }
        
        /// <summary>
        /// Opens the overlay to invite friends.
        /// </summary>
        public void InviteFriendsButton()
        {
            SPOSteamLobbyManager.OpenInviteOverlay();
        }
        
        /// <summary>
        /// Leaves the lobby.
        /// </summary>
        public void LeaveLobbyButton()
        {
            SPOSteamLobbyManager.DisconnectFromLobby();
        }
        
        /// <summary>
        /// Starts the game.
        /// </summary>
        public void ReadyToggleButton()
        {
            NetPlayerData localNetData = SPONetworkManager.GetLocalNetPlayer().NetData;
            bool isReady = localNetData.IsPlayerReady;
            localNetData.SetReadyStatus(!isReady);
        }
        
        /// <summary>
        /// Sets the ready status.
        /// </summary>
        /// <param name="readyStatus">The ready status.</param>
        public void SetReadyStatusButton(bool readyStatus)
        {
            NetPlayerData localNetData = SPONetworkManager.GetLocalNetPlayer().NetData;
            localNetData.SetReadyStatus(readyStatus);
        }
    }
}
