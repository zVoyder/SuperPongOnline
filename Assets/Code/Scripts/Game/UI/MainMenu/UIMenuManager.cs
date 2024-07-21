namespace SPO.UI.MainMenu
{
    using Managers.Networking;
    using UnityEngine;
    using SPO.Managers.Networking.Steam;
    
    public class UIMenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField]
        private GameObject _menuPanel;
        [SerializeField]
        private GameObject _lobbyPanel;
        [SerializeField]
        private GameObject _matchmakingPanel;

        private void OnEnable()
        {
            SPOSteamMatchmaker.OnStartMatchmaking += GoToMatchmaking;
            SPONetworkManager.OnClientConnected += GoToLobby;
            SPONetworkManager.OnClientDisconnected += GoToMenu;
        }

        private void OnDisable()
        {
            SPOSteamMatchmaker.OnStartMatchmaking -= GoToMatchmaking;
            SPONetworkManager.OnClientConnected -= GoToLobby;
            SPONetworkManager.OnClientDisconnected -= GoToMenu;
        }

        public void GoToMenu()
        {
            _menuPanel.SetActive(true);
            _matchmakingPanel.SetActive(false);
            _lobbyPanel.SetActive(false);
        }

        public void GoToLobby()
        {
            _lobbyPanel.SetActive(true);
            _menuPanel.SetActive(false);
            _matchmakingPanel.SetActive(false);
        }
        
        public void GoToMatchmaking()
        {
            _matchmakingPanel.SetActive(true);
            _menuPanel.SetActive(false);
            _lobbyPanel.SetActive(false);
        }
    }
}