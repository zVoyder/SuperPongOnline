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
        
        /// <summary>
        /// Enables the rematch panel.
        /// </summary>
        private void EnableRematchPanel()
        {
            _rematchPanel.SetActive(true);
        }
        
        /// <summary>
        /// Disables the rematch panel.
        /// </summary>
        private void DisableRematchPanel()
        {
            _rematchPanel.SetActive(false);
        }
        
        /// <summary>
        /// Event handler for when the client game begins.
        /// </summary>
        private void OnClientGameBegin()
        {
            DisableRematchPanel();
        }
        
        /// <summary>
        /// Event handler for when the client game is over.
        /// </summary>
        /// <param name="winnerId">The winner ID.</param>
        /// <param name="isWinner">True if the player is the winner, false otherwise.</param>
        private void OnClientGameover(int winnerId, bool isWinner)
        {
            SetRematchCountText(0);
            SetWinningStatusText(isWinner);
            EnableRematchPanel();
        }
        
        /// <summary>
        /// Event handler for when the client player updates the ready status.
        /// </summary>
        private void OnClientPlayerServerUpdateReadyStatus()
        { 
            SetRematchCountText(NetworkManager.GetReadyPlayerCount());
        }
        
        /// <summary>
        /// Sets the rematch count text.
        /// </summary>
        /// <param name="count">How many players are ready for a rematch.</param>
        private void SetRematchCountText(int count)
        {
            _rematchCountText.text = count.ToString() + " / " + NetworkManager.NetPlayers.Count;
        }
        
        /// <summary>
        /// Sets the winning status text.
        /// </summary>
        /// <param name="isWinner">True if the player is the winner, false otherwise.</param>
        private void SetWinningStatusText(bool isWinner)
        {
            _winStatusText.text = isWinner ? _winnerString : _loserString;
        }
    }
}