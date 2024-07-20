namespace SPO.UI.MainMenu
{
    using UnityEngine;
    using SPO.Managers.Networking;
    using SPO.Player;

    public class UINetActions : MonoBehaviour
    {
        public void CasualMatchButton()
        {
            SPOSteamLobbyManager.FindLobbies();
        }
        
        public void PrivateMatchButton()
        {
            SPOSteamLobbyManager.CreatePrivateLobby();
        }
        
        public void PublicMatchButton()
        {
            SPOSteamLobbyManager.CreatePublicLobby();
        }
        
        public void ReadyToggleButton()
        {
            NetPlayerData localNetData = SPONetworkManager.GetLocalNetPlayer().NetData;
            bool isReady = localNetData.IsReady;
            SPONetworkManager.GetLocalNetPlayer().NetData.SetReadyStatus(!isReady);
        }
        
        public void LeaveLobbyButton()
        {
            SPOSteamLobbyManager.DisconnectFromLobby();
        }
    }
}
