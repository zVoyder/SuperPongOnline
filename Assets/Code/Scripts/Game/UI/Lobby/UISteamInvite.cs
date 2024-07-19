namespace SPO.UI.Lobby
{
    using UnityEngine;
    using Mirror;
    using UnityEngine.Serialization;

    public class UISteamInvite : NetworkBehaviour
    {
        [FormerlySerializedAs("_relatedPlayerLobbyInfo"),Header("Player Info")]
        [SerializeField]
        private UIPlayerLobbyTag relatedPlayerLobbyTag;

        [Header("UI Elements")]
        [SerializeField]
        private GameObject _playerInfoPanel;
        [SerializeField]
        private GameObject _inviteButton;
        
        // private void OnEnable()
        // {
        //     relatedPlayerLobbyTag.OnPlayerJoinedInfoPanel += OnPlayerJoinedInfoPanel;
        //     relatedPlayerLobbyTag.OnPlayerLeftInfoPanel += OnPlayerLeftInfoPanel;
        // }
        //
        // private void OnDisable()
        // {
        //     relatedPlayerLobbyTag.OnPlayerJoinedInfoPanel -= OnPlayerJoinedInfoPanel;
        //     relatedPlayerLobbyTag.OnPlayerLeftInfoPanel -= OnPlayerLeftInfoPanel;
        // }
        
        private void OnPlayerJoinedInfoPanel()
        {
            RpcDisableInviteButton();
        }

        private void OnPlayerLeftInfoPanel()
        {
            RpcEnableInviteButton();
        }
        
        [ClientRpc]
        public void RpcEnableInviteButton()
        {
            _inviteButton.SetActive(true);
            _playerInfoPanel.SetActive(true);
        }
        
        [ClientRpc]
        public void RpcDisableInviteButton()
        {
            _inviteButton.SetActive(false);
            _playerInfoPanel.SetActive(false);
        }
    }
}