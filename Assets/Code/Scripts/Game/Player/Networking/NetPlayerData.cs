namespace SPO.Player
{
    using System;
    using Mirror;
    using Steamworks;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.GameConstants;
    using UnityEngine;

    public class NetPlayerData : NetworkBehaviour, IInit<int, int, ulong>
    {
        [field: SyncVar]
        public int ConnectionID { get; private set; } = -1;
        [field: SyncVar]
        public int PlayerIdNumber { get; private set; }
        [field: SyncVar]
        public ulong PlayerSteamID { get; private set; }
        [field: SyncVar(hook = nameof(OnPlayerUpdateName))]
        public string PlayerName { get; private set; }
        [field: SyncVar(hook = nameof(OnChangedReadyStatus))]
        public bool IsPlayerReady { get; private set; }

        public static event Action OnClientPlayerUpdatedName;
        public static event Action OnClientPlayerUpdatedReadyStatus;
        public static event Action OnServerPlayerUpdateReadyStatus;
        
        /// <summary>
        /// Initializes the player network data.
        /// </summary>
        /// <param name="arg1">The connection ID.</param>
        /// <param name="arg2">The player ID number.</param>
        /// <param name="arg3">The player Steam ID.</param>
        public void Init(int arg1, int arg2, ulong arg3)
        {
            ConnectionID = arg1;
            PlayerIdNumber = arg2;
            PlayerSteamID = arg3;
        }
        
        /// <summary>
        /// Checks if the player is not connected.
        /// </summary>
        /// <returns>True if the player is not connected, false otherwise. </returns>
        public bool Check()
        {
            return ConnectionID == -1;
        }

        /// <inheritdoc/>
        public override void OnStartAuthority()
        { 
            SetPlayerName(SteamFriends.GetPersonaName());
        }
        
        /// <summary>
        /// Sets the player name.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        [Client]
        public void SetPlayerName(string playerName)
        { 
            CmdSetPlayerName(playerName);
            gameObject.name = SPOConstants.LocalPlayerName;
        }
        
        /// <summary>
        /// Sets the player ready status.
        /// </summary>
        /// <param name="readyStatus">The ready status.</param>
        [Client]
        public void SetReadyStatus(bool readyStatus)
        {
            CmdSetReadyStatus(readyStatus);
        }

        /// <summary>
        /// Command to set the player name.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.OnPlayerUpdateName(this.PlayerName, playerName);
        }
        
        /// <summary>
        /// Command to set the player ready status.
        /// </summary>
        /// <param name="readyStatus">The ready status.</param>
        [Command]
        private void CmdSetReadyStatus(bool readyStatus)
        {
            this.IsPlayerReady = readyStatus;
        }
        
        /// <summary>
        /// SyncVar event handler for when the player name is updated.
        /// </summary>
        /// <param name="oldValue">The old player name.</param>
        /// <param name="newValue">The new player name.</param>
        private void OnPlayerUpdateName(string oldValue, string newValue)
        {
            if (isServer)
                this.PlayerName = newValue;

            if (isClient)
                OnClientPlayerUpdatedName?.Invoke();
        }

        /// <summary>
        /// SyncVar event handler for when the player ready status is changed.
        /// </summary>
        /// <param name="oldStatus">The old ready status.</param>
        /// <param name="newStatus">The new ready status.</param>
        private void OnChangedReadyStatus(bool oldStatus, bool newStatus)
        {
            if (isServer)
            {
                this.IsPlayerReady = newStatus;
                OnServerPlayerUpdateReadyStatus?.Invoke();
            }
            
            if (isClient)
                OnClientPlayerUpdatedReadyStatus?.Invoke();
        }
    }
}