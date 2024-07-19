namespace SPO.Patterns.Factories
{
    using VUDK.Patterns.StateMachine;
    using SPO.Managers;
    using SPO.Managers.GameMachine;
    using SPO.Managers.GameMachine.Contexts;
    using SPO.Managers.GameMachine.Data;
    using SPO.Managers.GameMachine.States;

    /// <summary>
    /// Responsible for the creation of all necessary objects related to state machines for the game.
    /// </summary>
    public static class MachineFactory
    {
        public static GameContext Create(SPOGameManager gameManager, SPONetGameMachineController machineController)
        {
            return new GameContext(gameManager, machineController);
        }
        
        public static T Create<T>(GameStateKeys stateKey, StateMachine relatedStateMachine, GameContext context) where T : State<GameContext>
        {
            switch (stateKey)
            {
                case GameStateKeys.GameIdle:
                    return new GameIdleState(stateKey, relatedStateMachine, context) as T;
                case GameStateKeys.GamePlaying:
                    return new GamePlayingState(stateKey, relatedStateMachine, context) as T;
                case GameStateKeys.GameBegin:
                    return new GameBeginState(stateKey, relatedStateMachine, context) as T;
                case GameStateKeys.GameEnd:
                    return new GameEndState(stateKey, relatedStateMachine, context) as T;
            }

            return null;
        }
    }
}