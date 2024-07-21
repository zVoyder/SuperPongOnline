namespace SPO.Managers.Networking.Steam
{
    using System;
    using Steamworks;
    using SPO.GameConstants;
    using UnityEngine;

    public static class SPOSteamMatchmaker
    {
        private static Callback<LobbyMatchList_t> s_LobbyMatchList;
        
        public static event Action OnStartMatchmaking;
        
        static SPOSteamMatchmaker()
        {
            if (!SteamManager.Initialized) return;
            
            s_LobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        }
        
        public static void StartMatchmaking()
        {
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1); // No need to find full lobbies
            SteamMatchmaking.AddRequestLobbyListStringFilter(SPOConstants.NetworkLobbyOwnerNameKey, SPOSteamLobbyManager.LobbyOwnerPersonaName, ELobbyComparison.k_ELobbyComparisonNotEqual);
            SteamMatchmaking.AddRequestLobbyListStringFilter(SPOConstants.NetworkLobbyGameIDKey, SPOConstants.NetworkGameID, ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
            SteamMatchmaking.RequestLobbyList();
            OnStartMatchmaking?.Invoke();
        }
        
        private static void OnLobbyMatchList(LobbyMatchList_t param)
        {
            if (param.m_nLobbiesMatching == 0)
            {
                Debug.Log("No lobbies found!, creating a public lobby...");
                SPOSteamLobbyManager.CreatePublicLobby();
                return;
            }
            
            Debug.Log("Lobbies found! Joining first lobby found...");
            SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
        }
    }
}