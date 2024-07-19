namespace SPO.Player
{
    using System;
    using UnityEngine;
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
        [field: SyncVar(hook = nameof(UpdatePlayerName))]
        public string PlayerName { get; private set; }

        public static event Action OnPlayerClientUpdatedName;
        
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
            CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
            gameObject.name = SPOConstants.LocalPlayerName;
        }
        
        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.UpdatePlayerName(this.PlayerName, playerName);
        }

        private void UpdatePlayerName(string oldValue, string newValue)
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
    }
}