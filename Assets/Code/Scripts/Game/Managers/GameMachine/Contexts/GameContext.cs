namespace SPO.Managers.GameMachine.Contexts
{
    using VUDK.Patterns.StateMachine;

    public class GameContext : StateContext
    {
        public SPOGameManager GameManager { get; private set; }
        public SPONetGameMachineController MachineController { get; private set; }
        
        public GameContext(SPOGameManager gameManager, SPONetGameMachineController machineController)  : base()
        {
            // Do not use the NetworkManager, since Server methods can be called only from NetworkBehaviour classes
            GameManager = gameManager;
            MachineController = machineController;
        }
    }
}