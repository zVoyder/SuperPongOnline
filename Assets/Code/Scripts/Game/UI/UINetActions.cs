namespace SPO.UI
{
    using UnityEngine;
    using SPO.Player;
    using SPO.Managers.Networking;
    using SPO.Managers.Networking.Steam;

    public class UINetActions : MonoBehaviour
    {
        public void CasualMatchButton()
        {
            SPOSteamMatchmaker.StartMatchmaking();
        }
        
        public void PrivateMatchButton()
        {
            SPOSteamLobbyManager.CreatePrivateLobby();
        }
        
        public void PublicMatchButton()
        {
            SPOSteamLobbyManager.CreatePublicLobby();
        }
        
        public void InviteFriendsButton()
        {
            SPOSteamLobbyManager.OpenInviteOverlay();
        }
        
        public void LeaveLobbyButton()
        {
            SPOSteamLobbyManager.DisconnectFromLobby();
        }
        
        public void ReadyToggleButton()
        {
            NetPlayerData localNetData = SPONetworkManager.GetLocalNetPlayer().NetData;
            bool isReady = localNetData.IsPlayerReady;
            localNetData.SetReadyStatus(!isReady);
        }
        
        public void SetReadyStatusButton(bool readyStatus)
        {
            NetPlayerData localNetData = SPONetworkManager.GetLocalNetPlayer().NetData;
            localNetData.SetReadyStatus(readyStatus);
        }
    }
}
