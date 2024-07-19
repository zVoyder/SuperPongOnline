namespace SPO.Managers
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Bases;
    using SPO.Managers.GameMachine;
    using SPO.Managers.Networking;
    
    public class SPOGameManager : GameManagerBase
    {
        [field: Header("Network Game Managers")]
        [field: SerializeField]
        public SPONetGameManager NetGameManager { get; private set; }
        [field: SerializeField]
        public SPONetGameMachineController NetGameMachineController { get; private set; }
        [field: SerializeField]
        public SPONetGameoverManager NetGameoverManager { get; private set; }
    }
}