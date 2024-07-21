namespace Code.Scripts.Game.UI.Gameplay
{
    using TMPro;
    using UnityEngine;
    using SPO.Managers.GameMachine;
    using SPO.Managers.Networking;
    using SPO.Player;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;

    public class UIRematchManager : MonoBehaviour, ICastNetworkManager<SPONetworkManager>
    {
        [Header("Gameover Strings")]
        [SerializeField]
        private string _winnerString = "You Win";
        [SerializeField]
        private string _loserString = "You Lose";
        
        [Header("UI Panels")]
        [SerializeField]
        private GameObject _rematchPanel;
        
        [Header("UI Elements")]
        [SerializeField]
        private TMP_Text _winStatusText;
        [SerializeField]
        private TMP_Text _rematchCountText;

        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;
        
        private void OnEnable()
        {
            SPONetGameMachineController.OnClientGameBegin += OnClientGameBegin;
            SPONetGameoverManager.OnClientGameover += OnClientGameover;
            NetPlayerData.OnClientPlayerUpdatedReadyStatus += OnClientPlayerServerUpdateReadyStatus;
        }

        private void OnDisable()
        {
            SPONetGameoverManager.OnClientGameover -= OnClientGameover;
            SPONetGameMachineController.OnClientGameBegin -= OnClientGameBegin;
            NetPlayerData.OnClientPlayerUpdatedReadyStatus -= OnClientPlayerServerUpdateReadyStatus;
        }
        
        private void EnableRematchPanel()
        {
            _rematchPanel.SetActive(true);
        }
        
        private void DisableRematchPanel()
        {
            _rematchPanel.SetActive(false);
        }
        
        private void OnClientGameBegin()
        {
            DisableRematchPanel();
        }
        
        private void OnClientGameover(int winnerId, bool isWinner)
        {
            SetRematchCountText(0);
            SetWinningStatusText(isWinner);
            EnableRematchPanel();
        }
        
        private void OnClientPlayerServerUpdateReadyStatus()
        { 
            SetRematchCountText(NetworkManager.GetReadyPlayerCount());
        }
        
        private void SetRematchCountText(int count)
        {
            _rematchCountText.text = count.ToString() + " / " + NetworkManager.NetPlayers.Count;
        }
        
        private void SetWinningStatusText(bool isWinner)
        {
            _winStatusText.text = isWinner ? _winnerString : _loserString;
        }
    }
}