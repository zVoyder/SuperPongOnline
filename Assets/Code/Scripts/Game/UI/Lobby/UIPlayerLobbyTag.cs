namespace SPO.UI.Lobby
{
    using UnityEngine;
    using UnityEngine.UI;
    using Steamworks;
    using TMPro;
    using SPO.Utility;
    using SPO.Player;
    
    public class UIPlayerLobbyTag : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField]
        private RawImage _imageAvatar;
        [SerializeField]
        private TMP_Text _textPlayerName;
     
        private string _playerName;
        private ulong _playerSteamID;
        private bool _isAvatarReceived;
        
        public int ConnectionID { get; private set; }
        protected Callback<AvatarImageLoaded_t> AvatarImageLoaded;

        private void OnEnable()
        { 
            AvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        }

        private void OnDisable()
        {
            AvatarImageLoaded?.Dispose();
        }

        public void Init(NetPlayerData netData)
        {
            ConnectionID = netData.ConnectionID;
            _playerSteamID = netData.PlayerSteamID;
            _playerName = netData.PlayerName;
        }
        
        public void SetPlayerValues()
        {
            _textPlayerName.text = _playerName;
            if (!_isAvatarReceived) UpdatePlayerAvatarImage();
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID != _playerSteamID) return;
            _imageAvatar.texture = SPOSteamUtils.GetSteamAvatar(callback.m_iImage);
            _isAvatarReceived = true;
        }
        
        private void UpdatePlayerAvatarImage()
        {
            CSteamID steamID = (CSteamID)_playerSteamID;
            int imageId = SteamFriends.GetLargeFriendAvatar(steamID);
            if (imageId == -1) return;
            _imageAvatar.texture = SPOSteamUtils.GetSteamAvatar(imageId);
        }
    }
}