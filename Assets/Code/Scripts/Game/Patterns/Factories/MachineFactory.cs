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
        /// <summary>
        /// Creates a game context object.
        /// </summary>
        /// <param name="gameManager">The game manager.</param>
        /// <param name="machineController">The net game machine controller.</param>
        /// <returns>The created game context object.</returns>
        public static GameContext Create(SPOGameManager gameManager, SPONetGameMachineController machineController)
        {
            return new GameContext(gameManager, machineController);
        }
        
        /// <summary>
        /// Creates a state for the game state machine.
        /// </summary>
        /// <param name="stateKey">The key of the state to be created.</param>
        /// <param name="relatedStateMachine">The state machine that the state will be related to.</param>
        /// <param name="context">The game context object.</param>
        /// <typeparam name="T">The type of the state to be created.</typeparam>
        /// <returns>The created state object.</returns>
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