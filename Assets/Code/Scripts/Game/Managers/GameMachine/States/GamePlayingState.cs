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

        public override void Enter()
        {
            Debug.Log("GamePlayingState entered.");
            EventManager.Ins.AddListener<GoalEventArgs>(SPOServerEvents.OnGoalScored, OnGoalScored);
        }
        
        public override void Exit()
        {
            EventManager.Ins.RemoveListener<GoalEventArgs>(SPOServerEvents.OnGoalScored, OnGoalScored);
        }

        public override void Process()
        {
        }

        public override void FixedProcess()
        {
        }

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