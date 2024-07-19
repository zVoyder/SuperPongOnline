namespace SPO.Managers.GameStats
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main;
    
    public class SPOGameStats : GameStats
    {
        [field: Header("Network Settings")]
        [field: SerializeField]
        public SPOGameSyncStats SyncStats { get; private set; }
        
        [field: Header("Player Colors")]
        [field: SerializeField]
        public Color LocalPlayerColor { get; private set; } = Color.blue;
        [field: SerializeField]
        public Color RemotePlayerColor { get; private set; } = Color.yellow;
    }
}