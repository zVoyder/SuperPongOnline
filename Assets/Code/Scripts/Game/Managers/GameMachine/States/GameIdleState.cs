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
        
        public override void Enter()
        {
            Debug.Log("GameIdleState entered.");
            Context.NetMachineController.TriggerResetGame();
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