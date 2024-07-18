namespace SPO.Managers
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Bases;
    using SPO.Managers.GameMachine;
    
    public class SPOGameManager : GameManagerBase
    {
        [field: Header("Managers")]
        [field: SerializeField]
        public SPOGameNetMachineController GameNetMachineController { get; private set; }
        [field: SerializeField]
        public SPOGameoverNetManager GameoverNetManager { get; private set; }
    }
}