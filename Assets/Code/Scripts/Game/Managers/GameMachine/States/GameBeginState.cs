namespace SPO.Managers.GameMachine.States
{
    using System;
    using UnityEngine;
    using VUDK.Patterns.StateMachine;
    using SPO.Managers.GameMachine.Contexts;

    public class GameBeginState : State<GameContext>
    {
        public GameBeginState(Enum stateKey, StateMachine relatedStateMachine, StateContext context) : base(stateKey, relatedStateMachine, context)
        {
        }

        /// <inheritdoc/>
        public override void Enter()
        {
            Debug.Log("GameBeginState entered.");
            Context.MachineController.BeginBall();
            Context.MachineController.ServerGamePlaying();
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