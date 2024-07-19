namespace SPO.UI.MainMenu
{
    using UnityEngine;
    using UnityEngine.Serialization;
    using SPO.Managers.Networking;

    public class UIMenuManager : MonoBehaviour
    {
        [FormerlySerializedAs("_menu"), Header("UI")]
        [SerializeField]
        private GameObject _menuPanel;
        [SerializeField]
        private GameObject _lobbyPanel;

        private void OnEnable()
        {
            SPOSteamLobbyManager.OnJoinedLobby += OnJoinedLobby;
            SPOSteamLobbyManager.OnLeftLobby += OnLeftLobby;
        }
        
        private void OnDisable()
        {
            SPOSteamLobbyManager.OnJoinedLobby -= OnJoinedLobby;
            SPOSteamLobbyManager.OnLeftLobby -= OnLeftLobby;
        }

        private void OnJoinedLobby()
        {
            GoToLobby();
        }
        
        private void OnLeftLobby()
        {
            GoToMenu();
        }
        
        public void GoToMenu()
        {
            _menuPanel.SetActive(true);
            _lobbyPanel.SetActive(false);
        }

        public void GoToLobby()
        {
            _menuPanel.SetActive(false);
            _lobbyPanel.SetActive(true);
        }
    }
}