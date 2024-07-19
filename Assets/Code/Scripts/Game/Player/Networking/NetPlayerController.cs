namespace SPO.Player
{
    using System;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Generic.Managers.Main;
    using SPO.Managers.Networking;

    [RequireComponent(typeof(NetPlayerData))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetPlayerController : NetworkBehaviour, ICastNetworkManager<SPONetworkManager>
    {
        private NetworkIdentity _networkIdentity;
        
        public NetPlayerData NetData { get; private set; }
        public SPONetworkManager NetworkManager => MainManager.Ins.NetworkManager as SPONetworkManager;
        
        public static event Action OnPlayerStartAuthority;
        public static event Action OnPlayerStartClient;
        public static event Action OnPlayerStopClient;
        
        private void Awake()
        {
            TryGetComponent(out _networkIdentity);
            TryGetComponent(out NetPlayerData netData);
            NetData = netData;
        }

        public void Init(int connectionID, int playerIdNumber, ulong playerSteamId)
        {
            NetData.Init(connectionID, playerIdNumber, playerSteamId);
        }
        
        public override void OnStartAuthority()
        {
            OnPlayerStartAuthority?.Invoke();
        }
        
        public override void OnStartClient()
        {
            OnPlayerStartClient?.Invoke();
            NetworkManager.NetPlayers.Add(this);
        }
        
        public override void OnStopClient()
        {
            OnPlayerStopClient?.Invoke();
            NetworkManager.NetPlayers.Remove(this);
        }
        
        public static int GetLocalPlayerID()
        {
            if (NetworkClient.localPlayer.TryGetComponent(out NetPlayerController netPlayer))
                return netPlayer.NetData.ConnectionID;
            
            return -1;
        }
    }
}