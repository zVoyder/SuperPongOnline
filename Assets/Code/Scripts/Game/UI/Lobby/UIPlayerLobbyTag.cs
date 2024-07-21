namespace SPO.UI.Lobby
{
    using UnityEngine;
    using UnityEngine.UI;
    using Steamworks;
    using TMPro;
    using SPO.Utility;
    using SPO.Player;
    using UnityEngine.Serialization;

    public class UIPlayerLobbyTag : MonoBehaviour
    {
        [Header("Ready Box")]
        [SerializeField]
        private Image _readyImage;
        [SerializeField]
        private Sprite _readyOnSprite;
        [SerializeField]
        private Sprite _readyOffSprite;
        
        [Header("UI Elements")]
        [SerializeField]
        private RawImage _imageAvatar;
        [SerializeField]
        private TMP_Text _textPlayerName;
        
        private bool _isAvatarReceived;
        private NetPlayerData _relatedNetPlayerData;
        
        public int ConnectionID => _relatedNetPlayerData.ConnectionID;
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
            _relatedNetPlayerData = netData;
        }
        
        public void SetPlayerValues()
        {
            _textPlayerName.text = _relatedNetPlayerData.PlayerName;
            UpdateReadyBox(_relatedNetPlayerData.IsPlayerReady);
            if (!_isAvatarReceived) UpdatePlayerAvatarImage();
        }
        
        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID != _relatedNetPlayerData.PlayerSteamID) return;
            _imageAvatar.texture = SPOSteamUtils.GetSteamAvatar(callback.m_iImage);
            _isAvatarReceived = true;
        }
        
        private void UpdatePlayerAvatarImage()
        {
            CSteamID steamID = (CSteamID)_relatedNetPlayerData.PlayerSteamID;
            int imageId = SteamFriends.GetLargeFriendAvatar(steamID);
            if (imageId == -1) return;
            _imageAvatar.texture = SPOSteamUtils.GetSteamAvatar(imageId);
        }
        
        private void UpdateReadyBox(bool isReady)
        {
            _readyImage.sprite = isReady ? _readyOnSprite : _readyOffSprite;
        }
    }
}