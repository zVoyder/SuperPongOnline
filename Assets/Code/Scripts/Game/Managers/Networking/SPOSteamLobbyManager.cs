namespace SPO.Managers.Networking
{
    using System;
    using System.Collections.Generic;
    using Mirror;
    using Player;
    using Steamworks;
    using UnityEngine;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.GameConstants;

    public class SPOSteamLobbyManager : MonoBehaviour, ICastNetworkManager<SPONetworkManager>
    { 
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<LobbyEnter_t> LobbyEnter;
        protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
        protected Callback<LobbyMatchList_t> LobbyMatchList;
        protected Callback<LobbyChatUpdate_t> LobbyChatUpdate;

        public static CSteamID LobbyID { get; private set; }
        public static CSteamID LobbyOwnerID => SteamMatchmaking.GetLobbyOwner(LobbyID);
        public static string LobbyOwnerPersonaName => SteamFriends.GetFriendPersonaName(LobbyOwnerID);
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;

        public static event Action OnJoinedLobby;
        public static event Action OnLeftLobby;
        public static event Action OnLobbyUpdate;
        
        private void OnEnable()
        {
            if (!SteamManager.Initialized) return;
            
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            LobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
            LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        private void OnDisable()
        {
            if (!SteamManager.Initialized) return;
            
            LobbyCreated?.Dispose();
            LobbyEnter?.Dispose();
            GameLobbyJoinRequested?.Dispose();
            LobbyMatchList?.Dispose();
            LobbyChatUpdate?.Dispose();
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
        
        public static void FindLobbies()
        {
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1); // No need to find full lobbies
            SteamMatchmaking.AddRequestLobbyListStringFilter(SPOConstants.NetworkLobbyOwnerNameKey, LobbyOwnerPersonaName, ELobbyComparison.k_ELobbyComparisonNotEqual);
            SteamMatchmaking.AddRequestLobbyListStringFilter(SPOConstants.NetworkLobbyGameIDKey, SPOConstants.NetworkGameID, ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
            SteamMatchmaking.RequestLobbyList();
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
            OnLeftLobby?.Invoke();
        }

        private void OnLobbyMatchList(LobbyMatchList_t param)
        {
            for (int i = 0; i < param.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, SPOConstants.NetworkLobbyOwnerNameKey);
            }
            
            // TODO: DEBUG ONLY
            if (param.m_nLobbiesMatching > 0)
            {
                // TODO: Just for testing purposes, join the first lobby found
                string lobbyName = SteamMatchmaking.GetLobbyData(SteamMatchmaking.GetLobbyByIndex(0), SPOConstants.NetworkLobbyOwnerNameKey);
                Debug.Log("Lobbies Found! Joining first lobby found " + lobbyName);
                SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
            }
            else
            {
                Debug.LogError("No lobbies found!");
            }
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogWarning("Failed to create Steam Lobby!");
                return;
            }
            
            SetLobbyID(new CSteamID(callback.m_ulSteamIDLobby));
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyGameIDKey, SPOConstants.NetworkGameID);
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyHostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(LobbyID, SPOConstants.NetworkLobbyOwnerNameKey, LobbyOwnerPersonaName);
            SPONetworkManager.StartHostConnection();
            
            Debug.Log($"Created Lobby by {LobbyOwnerPersonaName} with result {callback.m_eResult}");
            OnJoinedLobby?.Invoke();
        }
        
        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            if (NetworkServer.active)
                return;
            
            if (callback.m_EChatRoomEnterResponse != 1)
            {
                Debug.LogError("Failed to join Steam Lobby!");
                return;
            }
            
            SetLobbyID(new CSteamID(callback.m_ulSteamIDLobby));
            string hostAddress = SteamMatchmaking.GetLobbyData(LobbyID, SPOConstants.NetworkLobbyHostAddressKey); 
            NetworkManager.networkAddress = hostAddress;
            SPONetworkManager.StartClientConnection();
            Debug.Log($"Joined Lobby");
            OnJoinedLobby?.Invoke();
        }
        
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            // SPONetworkManager.StartClientConnection();
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
            OnJoinedLobby?.Invoke();
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            OnLobbyUpdate?.Invoke();
        }

        private void SetLobbyID(CSteamID steamID)
        {
            LobbyID = steamID;
        }
    }
}