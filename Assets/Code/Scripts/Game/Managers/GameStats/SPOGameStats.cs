namespace SPO.Managers.GameStats
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main;

    [RequireComponent(typeof(SPOGameSyncStats))]
    public class SPOGameStats : GameStats
    {
        [field: Header("Player Colors")]
        [field: SerializeField]
        public Color LocalPlayerColor { get; private set; } = Color.blue;
        [field: SerializeField]
        public Color RemotePlayerColor { get; private set; } = Color.yellow;
        
        public SPOGameSyncStats SyncStats { get; private set; }
        
        private void Awake()
        {
            TryGetComponent(out SPOGameSyncStats syncStats);
            SyncStats = syncStats;
        }
    }
}