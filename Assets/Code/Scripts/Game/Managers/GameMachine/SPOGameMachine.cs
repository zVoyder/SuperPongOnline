namespace SPO.Managers.GameMachine
{
    using Mirror;
    using Player.Interfaces;
    using VUDK.Generic.Managers.Main.Bases;
    using SPO.Patterns.Factories;
    using SPO.Managers.GameMachine.States;
    using SPO.Managers.GameMachine.Contexts;
    using SPO.Managers.GameMachine.Data;
    using UnityEngine;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Patterns.Initialization.Interfaces;

    public class SPOGameMachine : GameMachineBase, IInit<SPOGameNetMachineController>, ICastGameManager<SPOGameManager>
    {
        private SPOGameNetMachineController _gameNetMachineController;
        
        public GameContext Context { get; private set; }
        
        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;

        public void Init(SPOGameNetMachineController arg1)
        {
            base.Init();
            _gameNetMachineController = arg1;
            
            Context = MachineFactory.Create(GameManager, _gameNetMachineController);
            
            GameIdleState idleState = MachineFactory.Create<GameIdleState>(GameStateKeys.GameIdle, this, Context);
            GameBeginState beginState = MachineFactory.Create<GameBeginState>(GameStateKeys.GameBegin, this, Context);
            GamePlayingState playingState = MachineFactory.Create<GamePlayingState>(GameStateKeys.GamePlaying, this, Context);
            GameEndState endState = MachineFactory.Create<GameEndState>(GameStateKeys.GameEnd, this, Context);
            
            AddState(GameStateKeys.GameIdle, idleState);
            AddState(GameStateKeys.GameBegin, beginState);
            AddState(GameStateKeys.GamePlaying, playingState);
            AddState(GameStateKeys.GameEnd, endState);
        }
        
        public override bool Check()
        {
            return Context != null;
        }
    }
}