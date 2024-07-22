namespace SPO.Managers.GameMachine.States
{
    using System;
    using Mirror;
    using Player;
    using SPO.Managers.GameMachine.Contexts;
    using UnityEngine;
    using VUDK.Patterns.StateMachine;

    public class GameEndState : State<GameContext>
    {
        public GameEndState(Enum stateKey, StateMachine relatedStateMachine, StateContext context) : base(stateKey, relatedStateMachine, context)
        {
        }
        
        /// <inheritdoc/>
        public override void Enter()
        {
            Debug.Log("GameEndState entered.");
            
            if (Context.MachineController.isServer)
                TriggerWinStatus(Context.MachineController.GetWinnerID());
            else
                Context.MachineController.OnWinnerIDChangedHookReceived += TriggerWinStatus;
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            Context.MachineController.OnWinnerIDChangedHookReceived -= TriggerWinStatus;
            Context.MachineController.TriggerResetGame();
        }
        
        /// <inheritdoc/>
        public override void Process()
        {
        }
        
        /// <inheritdoc/>
        public override void FixedProcess()
        {
        }
        
        /// <inheritdoc/>
        private void TriggerWinStatus(int winnerID)
        {
            Context.MachineController.TriggerGameOver();
        }
    }
}