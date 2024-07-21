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
        
        public bool Check()
        {
            return ConnectionID == -1;
        }

        public override void OnStartAuthority()
        { 
            SetPlayerName(SteamFriends.GetPersonaName());
        }
        
        [Client]
        public void SetPlayerName(string playerName)
        { 
            CmdSetPlayerName(playerName);
            gameObject.name = SPOConstants.LocalPlayerName;
        }
        
        [ContextMenu("Set Ready Status")]
        private void SetReadyStatus()
        {
            CmdSetReadyStatus(!IsPlayerReady);
        }
        
        [Client]
        public void SetReadyStatus(bool readyStatus)
        {
            CmdSetReadyStatus(readyStatus);
        }

        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.OnPlayerUpdateName(this.PlayerName, playerName);
        }
        
        [Command]
        private void CmdSetReadyStatus(bool readyStatus)
        {
            this.IsPlayerReady = readyStatus;
        }
        
        private void OnPlayerUpdateName(string oldValue, string newValue)
        {
            if (isServer)
                this.PlayerName = newValue;

            if (isClient)
                OnClientPlayerUpdatedName?.Invoke();
        }

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