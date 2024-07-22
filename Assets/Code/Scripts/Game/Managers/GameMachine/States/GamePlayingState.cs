namespace SPO.Managers.GameMachine.States
{
    using System;
    using UnityEngine;
    using VUDK.Patterns.StateMachine;
    using VUDK.Features.Main.EventSystem;
    using SPO.Managers.GameMachine.Contexts;
    using SPO.GameConstants;
    using SPO.Level.Goal.Events;

    public class GamePlayingState : State<GameContext>
    {
        public GamePlayingState(Enum stateKey, StateMachine relatedStateMachine, StateContext context) : base(stateKey, relatedStateMachine, context)
        {
        }

        /// <inheritdoc/>
        public override void Enter()
        {
            Debug.Log("GamePlayingState entered.");
            EventManager.Ins.AddListener<GoalEventArgs>(SPOServerEvents.OnGoalScored, OnGoalScored);
        }
        
        /// <inheritdoc/>
        public override void Exit()
        {
            EventManager.Ins.RemoveListener<GoalEventArgs>(SPOServerEvents.OnGoalScored, OnGoalScored);
        }

        /// <inheritdoc/>
        public override void Process()
        {
        }

        /// <inheritdoc/>
        public override void FixedProcess()
        {
        }

        /// <summary>
        /// Event handler for when a goal is scored.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        private void OnGoalScored(GoalEventArgs args)
        {
            Context.MachineController.EndBall();
            
            // Check on the server if someone won
            if (Context.MachineController.CheckVictory(args.PlayerID, args.ScoreValue))
                Context.MachineController.ServerGameEndWinner(args.PlayerID);
            else
                Context.MachineController.ServerGameBegin();
        }
    }
}