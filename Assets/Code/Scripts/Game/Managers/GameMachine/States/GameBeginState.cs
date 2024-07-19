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

        public override void Enter()
        {
            Debug.Log("GameBeginState entered.");
            Context.MachineController.BeginBall();
            Context.MachineController.ServerGamePlaying();
        }

        public override void Exit()
        {
        }

        public override void Process()
        {
        }

        public override void FixedProcess()
        {
        }
    }
}