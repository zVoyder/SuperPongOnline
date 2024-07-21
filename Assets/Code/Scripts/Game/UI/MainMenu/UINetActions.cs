namespace SPO.UI.MainMenu
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
