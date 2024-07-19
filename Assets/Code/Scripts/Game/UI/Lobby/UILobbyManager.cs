namespace SPO.UI.Lobby
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using SPO.Player;
    using SPO.Managers.Networking;

    public class UILobbyManager : MonoBehaviour, ICastNetworkManager<SPONetworkManager>
    {
        [Header("UI Elements")]
        [SerializeField]
        private RectTransform _playerTagsPanel;
        [SerializeField]
        private GameObject _playerTagPrefab;

        private bool _isPlayerTagCreated = false;
        private List<UIPlayerLobbyTag> _playerTags = new List<UIPlayerLobbyTag>();
        
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;

        private void OnEnable()
        {
            NetPlayerController.OnPlayerStartClient += UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStopClient += UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedName += UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStartAuthority += UpdatePlayerTagsList;
        }
        
        private void OnDisable()
        {
            NetPlayerController.OnPlayerStartClient -= UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStopClient -= UpdatePlayerTagsList;
            NetPlayerData.OnPlayerClientUpdatedName -= UpdatePlayerTagsList;
            NetPlayerController.OnPlayerStartAuthority -= UpdatePlayerTagsList;
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
    }
}