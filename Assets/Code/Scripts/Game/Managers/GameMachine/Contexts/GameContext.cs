namespace SPO.Managers.GameMachine.Contexts
{
    using VUDK.Patterns.StateMachine;

    public class GameContext : StateContext
    {
        public SPOGameManager GameManager { get; private set; }
        public SPOGameNetMachineController NetMachineController { get; private set; }
        
        public GameContext(SPOGameManager gameManager, SPOGameNetMachineController netMachineController)  : base()
        {
            // Do not use the NetworkManager, since Server methods can be called only from NetworkBehaviour classes
            GameManager = gameManager;
            NetMachineController = netMachineController;
        }
    }
}