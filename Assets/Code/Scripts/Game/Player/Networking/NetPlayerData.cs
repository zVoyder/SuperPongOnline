namespace SPO.Player
{
    using System;
    using Mirror;
    using Steamworks;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.GameConstants;

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
        public bool IsReady { get; private set; }

        public static event Action OnPlayerClientUpdatedName;
        public static event Action OnPlayerClientUpdatedReadyStatus;
        public static event Action OnPlayerServerUpdateReadyStatus;
        
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
        public void SetReadyStatus(bool readyStatus)
        {
            CmdSetReadyStatus(readyStatus);
        }

        [Client]
        public void SetPlayerName(string playerName)
        { 
            CmdSetPlayerName(playerName);
            gameObject.name = SPOConstants.LocalPlayerName;
        }
        
        [Command]
        private void CmdSetReadyStatus(bool readyStatus)
        {
            this.IsReady = readyStatus;
        }
        
        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.OnPlayerUpdateName(this.PlayerName, playerName);
        }

        private void OnPlayerUpdateName(string oldValue, string newValue)
        {
            if (isServer)
            {
                this.PlayerName = newValue;
            }

            if (isClient)
            {
                OnPlayerClientUpdatedName?.Invoke();
            }
        }

        private void OnChangedReadyStatus(bool oldStatus, bool newStatus)
        {
            if (isServer)
            {
                this.IsReady = newStatus;
                OnPlayerServerUpdateReadyStatus?.Invoke();
            }
            
            if (isClient)
            {
                OnPlayerClientUpdatedReadyStatus?.Invoke();
            }
        }
    }
}