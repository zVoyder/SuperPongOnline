namespace SPO.Managers.GameMachine.States
{
    using System;
    using SPO.Managers.GameMachine.Contexts;
    using UnityEngine;
    using VUDK.Patterns.StateMachine;

    public class GameIdleState : State<GameContext>
    {
        public GameIdleState(Enum stateKey, StateMachine relatedStateMachine, StateContext context) : base(stateKey, relatedStateMachine, context)
        {
        }
        
        /// <inheritdoc/>
        public override void Enter()
        {
            Debug.Log("GameIdleState entered.");
            Context.MachineController.TriggerResetGame();
        }
        
        /// <inheritdoc/>
        public override void Exit()
        {
        }
        
        /// <inheritdoc/>
        public override void Process()
        {
        }
        
        /// <inheritdoc/>
        public override void FixedProcess()
        {
        }
    }
}