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
            EventManager.Ins.AddListener<GoalEventArgs>(SPOEvents.OnGoalScored, OnGoalScored);
        }
        
        public override void Exit()
        {
            EventManager.Ins.RemoveListener<GoalEventArgs>(SPOEvents.OnGoalScored, OnGoalScored);
        }

        public override void Process()
        {
        }

        public override void FixedProcess()
        {
        }

        private void OnGoalScored(GoalEventArgs args)
        {
            Context.NetMachineController.EndBall();
            
            // Check on the server if someone won
            if (Context.NetMachineController.CheckVictory(args.PlayerID, args.ScoreValue))
                Context.NetMachineController.ServerGameEndWinner(args.PlayerID);
            else
                Context.NetMachineController.ServerGameBegin();
        }
    }
}