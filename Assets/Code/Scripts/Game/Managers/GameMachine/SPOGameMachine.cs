namespace SPO.Managers.GameMachine
{
    using VUDK.Generic.Managers.Main.Bases;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using SPO.Patterns.Factories;
    using SPO.Managers.GameMachine.States;
    using SPO.Managers.GameMachine.Contexts;
    using SPO.Managers.GameMachine.Data;
    using UnityEngine;

    public class SPOGameMachine : GameMachineBase, ICastGameManager<SPOGameManager>
    {
        [field: Header("Networking")]
        [field: SerializeField]
        public SPONetGameMachineController NetMachineController { get; private set; }
        
        public GameContext Context { get; private set; }
        
        public SPOGameManager GameManager => MainManager.Ins.GameManager as SPOGameManager;

        private void Awake()
        {
            Init();
        }
        
        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();

            NetMachineController.Init(this);
            Context = MachineFactory.Create(GameManager, NetMachineController);
            
            GameIdleState idleState = MachineFactory.Create<GameIdleState>(GameStateKeys.GameIdle, this, Context);
            GameBeginState beginState = MachineFactory.Create<GameBeginState>(GameStateKeys.GameBegin, this, Context);
            GamePlayingState playingState = MachineFactory.Create<GamePlayingState>(GameStateKeys.GamePlaying, this, Context);
            GameEndState endState = MachineFactory.Create<GameEndState>(GameStateKeys.GameEnd, this, Context);
            
            AddState(GameStateKeys.GameIdle, idleState);
            AddState(GameStateKeys.GameBegin, beginState);
            AddState(GameStateKeys.GamePlaying, playingState);
            AddState(GameStateKeys.GameEnd, endState);
        }
        
        /// <inheritdoc/>
        public override bool Check()
        {
            return Context != null;
        }
    }
}