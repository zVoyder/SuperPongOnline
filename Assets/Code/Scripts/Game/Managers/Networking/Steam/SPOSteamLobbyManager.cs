namespace SPO.Managers.Networking.Steam
{
    using System;
    using System.Collections.Generic;
    using Mirror;
    using Steamworks;
    using UnityEngine;
    using SPO.GameConstants;

    public static class SPOSteamLobbyManager
    { 
        private static Callback<LobbyCreated_t> s_LobbyCreated;
        private static Callback<LobbyEnter_t> s_LobbyEnter;
        private static Callback<GameLobbyJoinRequested_t> s_GameLobbyJoinRequested;
        private static Callback<LobbyMatchList_t> s_LobbyMatchList;
        private static Callback<LobbyChatUpdate_t> s_LobbyChatUpdate;

        public static CSteamID LobbyID { get; private set; }
        public static CSteamID LobbyOwnerID => SteamMatchmaking.GetLobbyOwner(LobbyID);
        public static string LobbyOwnerPersonaName => SteamFriends.GetFriendPersonaName(LobbyOwnerID);
        public static bool IsLobbyOwner => SteamUser.GetSteamID() == LobbyOwnerID;

        public static event Action<LobbyCreated_t> OnSteamLobbyCreated;
        public static event Action<LobbyEnter_t> OnSteamLobbyJoined;
        public static event Action<CSteamID> OnSteamLobbyLeft;
        public static event Action<LobbyChatUpdate_t> OnSteamLobbyUpdated;
        public static event Action<GameLobbyJoinRequested_t> OnSteamLobbyJoinRequested;
        
        static SPOSteamLobbyManager()
        {
            if (!SteamManager.Initialized) return;
            
            s_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            s_LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            s_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            // s_LobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
            s_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }
        
        public static List<CSteamID> GetAllPlayerIDsInLobby()
        {
            List<CSteamID> playerIDs = new List<CSteamID>();
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(LobbyID);

            for (int i = 0; i < memberCount; i++)
            {
                CSteamID playerID = SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, i);
                playerIDs.Add(playerID);
                Debug.Log("Player ID: " + playerID);
            }

            return playerIDs;
        }

        public static void CreatePrivateLobby()
        {
            Debug.Log("Creating Private Lobby");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, Mirror.NetworkManager.singleton.maxConnections);
        }
        
        public static void CreatePublicLobby()
        {
            Debug.Log("Creating Public Lobby");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,  Mirror.NetworkManager.singleton.maxConnections);
        }
        
        public static void DisconnectFromLobby()
        {
            SteamMatchmaking.LeaveLobby(LobbyID);
            SPONetworkManager.StopConnection();
            OnSteamLobbyLeft?.Invoke(LobbyID);
        }

        private static void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogWarning("Failed to create Steam Lobby!");
                return;
            }
            
            SetLobbyID((CSteamID)callback.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyGameIDKey, SPOConstants.NetworkGameID);
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyOwnerNameKey, LobbyOwnerPersonaName);
            
            string hostAddress = SteamUser.GetSteamID().ToString();
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyHostAddressKey, hostAddress);
            SPONetworkManager.StartHostConnection(hostAddress);
            
            Debug.Log($"Created Lobby by {LobbyOwnerPersonaName} with result {callback.m_eResult}.");
            OnSteamLobbyCreated?.Invoke(callback);
        }
        
        private static void OnLobbyEnter(LobbyEnter_t callback)
        {
            if (NetworkServer.active && !IsLobbyOwner)
            {
                Debug.Log("Trying to join a lobby while already in a lobby as a host.");
                return;
            }
            
            if (callback.m_EChatRoomEnterResponse != 1)
            {
                Debug.LogError("Failed to join Steam Lobby!");
                return;
            }
            
            SetLobbyID((CSteamID)callback.m_ulSteamIDLobby);
            string hostAddress = SteamMatchmaking.GetLobbyData(LobbyID, SPOConstants.NetworkLobbyHostAddressKey); 
            SPONetworkManager.StartClientConnection(hostAddress);
            
            Debug.Log($"Joined Lobby");
            OnSteamLobbyJoined?.Invoke(callback);
        }
        
        private static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
            OnSteamLobbyJoinRequested?.Invoke(callback);
        }

        private static void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            OnSteamLobbyUpdated?.Invoke(callback);
        }

        private static void SetLobbyID(CSteamID steamID)
        {
            LobbyID = steamID;
        }
    }
}