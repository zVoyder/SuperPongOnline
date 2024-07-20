namespace SPO.UI.Lobby
{
    using System.Linq;
    using System.Collections.Generic;
    using Mirror;
    using UnityEngine;
    using TMPro;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using SPO.Player;
    using SPO.Managers.Networking;

    public class UILobbyManager : MonoBehaviour, ICastNetworkManager<SPONetworkManager>
    {
        [Header("UI Settings")]
        private string _defaultTimeText = "VS";
        
        [Header("UI Prefabs")]
        [SerializeField]
        private GameObject _playerTagPrefab;
        
        [Header("UI Elements")]
        [SerializeField]
        private RectTransform _playerTagsPanel;
        [SerializeField]
        private TMP_Text _lobbyTimeText;

        private bool _isPlayerTagCreated = false;
        private List<UIPlayerLobbyTag> _playerTags = new List<UIPlayerLobbyTag>();
        
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;

        private void OnEnable()
        {
            SPONetSceneManager.OnRPCStartChangingScene += OnStartChangingScene;
            SPONetSceneManager.OnRPCStopChangingScene += OnStopChangingScene;
            SPONetSceneManager.OnRPCChangingSceneDelayUpdate += UpdateTimeLeftText;
            SPONetworkManager.OnServerClientConnected += OnServerClientConnected;
            SPONetworkManager.OnServerClientDisconnected += OnServerClientDisconnected;
            NetPlayerController.OnPlayerStartClient += UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStopClient += OnPlayerStopClient;
            NetPlayerController.OnPlayerStartAuthority += UpdatePlayerTagsList;
            SPOSteamLobbyManager.OnLobbyUpdate += UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedReadyStatus += UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedName += UpdatePlayerTagsList;
        }

        private void OnDisable()
        {
            SPONetSceneManager.OnRPCStartChangingScene -= OnStartChangingScene;
            SPONetSceneManager.OnRPCStopChangingScene -= OnStopChangingScene;
            SPONetSceneManager.OnRPCChangingSceneDelayUpdate -= UpdateTimeLeftText;
            SPONetworkManager.OnServerClientConnected -= OnServerClientConnected;
            SPONetworkManager.OnServerClientDisconnected -= OnServerClientDisconnected;
            NetPlayerController.OnPlayerStartClient -= UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStopClient -= OnPlayerStopClient;
            NetPlayerController.OnPlayerStartAuthority -= UpdatePlayerTagsList;
            SPOSteamLobbyManager.OnLobbyUpdate -= UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedReadyStatus -= UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedName -= UpdatePlayerTagsList;
        }

        private void OnServerClientConnected(NetworkConnectionToClient conn)
        {
            UpdatePlayerTagsList();
        }
        
        private void OnServerClientDisconnected(NetworkConnectionToClient conn)
        { 
            UpdatePlayerTagsList();
        }

        private void OnPlayerStopClient()
        {
            UpdatePlayerTagsList();
            SetDefaultTimeText();
        }
        
        private void OnStopChangingScene()
        {
            SetDefaultTimeText();
        }
        
        private void OnStartChangingScene(int delay)
        { 
            UpdateTimeLeftText(delay);
        }

        public void UpdatePlayerTagsList()
        {
            if (!_isPlayerTagCreated) 
                CreateHostPlayerTag();
            
            if (_playerTags.Count < NetworkManager.NetPlayers.Count)
                CreateClientPlayerTag();
            
            if (_playerTags.Count > NetworkManager.NetPlayers.Count)
                RemovePlayerTag();
            
            if (_playerTags.Count == NetworkManager.NetPlayers.Count)
                UpdatePlayerTag();
        }
        
        public void CreateHostPlayerTag()
        {
            if (_playerTagPrefab == null) return;
            
            foreach (NetPlayerController player in NetworkManager.NetPlayers)
            {
                GameObject playerTag = Instantiate(_playerTagPrefab, _playerTagsPanel);
                playerTag.TryGetComponent(out UIPlayerLobbyTag newPlayerLobbyTag);
                newPlayerLobbyTag.Init(player.NetData);
                newPlayerLobbyTag.SetPlayerValues();
                _playerTags.Add(newPlayerLobbyTag);
            }
            
            _isPlayerTagCreated = true;
        }
        
        public void CreateClientPlayerTag()
        {
            if (_playerTagPrefab == null) return;
            
            foreach (NetPlayerController player in NetworkManager.NetPlayers)
            {
                if (!_playerTags.Any(playerLobbyTag => playerLobbyTag.ConnectionID == player.NetData.ConnectionID))
                {
                    GameObject playerTag = Instantiate(_playerTagPrefab, _playerTagsPanel);
                    playerTag.TryGetComponent(out UIPlayerLobbyTag newPlayerLobbyTag);
                    newPlayerLobbyTag.Init(player.NetData);
                    newPlayerLobbyTag.SetPlayerValues();
                    _playerTags.Add(newPlayerLobbyTag);
                }
            }
        }

        public void UpdatePlayerTag()
        {
            foreach (NetPlayerController player in NetworkManager.NetPlayers)
            {
                foreach (UIPlayerLobbyTag playerTag in _playerTags)
                {
                    if (playerTag.ConnectionID == player.NetData.ConnectionID)
                    {
                        playerTag.Init(player.NetData);
                        playerTag.SetPlayerValues();
                    }
                }
            }
        }
        
        public void RemovePlayerTag()
        {
            Debug.Log("Removing unnecessary tags...");
            
            List<UIPlayerLobbyTag> playerTagsToRemove = new List<UIPlayerLobbyTag>();
            
            foreach (UIPlayerLobbyTag playerTag in _playerTags)
            {
                if (!NetworkManager.NetPlayers.Any(netPlayer => netPlayer.NetData.ConnectionID == playerTag.ConnectionID))
                    playerTagsToRemove.Add(playerTag);
            }

            if (playerTagsToRemove.Count > 0)
            {
                foreach (UIPlayerLobbyTag playerTag in playerTagsToRemove)
                {
                    GameObject playerTagToRemove = playerTag.gameObject;
                    _playerTags.Remove(playerTag);
                    Destroy(playerTagToRemove);
                    playerTagToRemove = null;
                }
            }
        }
        
        private void SetDefaultTimeText()
        {
            Debug.Log("Setting default time text...");
            _lobbyTimeText.text = _defaultTimeText;
        }
        
        private void UpdateTimeLeftText(int time)
        {
            _lobbyTimeText.text = time.ToString();
        }
    }
}