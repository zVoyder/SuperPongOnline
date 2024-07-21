namespace Code.Scripts.Game.Level.Goal
{
    using System;
    using UnityEngine;
    using Mirror;
    using VUDK.Generic.Managers.Main;
    using VUDK.Generic.Managers.Main.Interfaces.Casts;
    using VUDK.Features.Main.EventSystem;
    using SPO.GameConstants;
    using SPO.Level.Goal.Events;
    using SPO.Level.Goal.Interfaces;
    using SPO.Managers.GameStats;
    using SPO.Managers.Networking;
    using SPO.Player;

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class Goal : NetworkBehaviour, IGoal, ICastGameStats<SPOGameStats>
    {
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;

        [field: SyncVar(hook = nameof(OnScoreValueChanged))]
        public int ScoreValue { get; private set; } = 0;
        public SPOGameStats GameStats => MainManager.Ins.GameStats as SPOGameStats;

        public Action<int> OnScoreValueChangedHookReceived;
        
        private void Awake()
        {
            TryGetComponent(out _collider);
            TryGetComponent(out _rigidbody);
            
            _rigidbody.bodyType = RigidbodyType2D.Static;
        }

        private void OnEnable()
        {
            SPONetGameoverManager.OnServerResetGame += OnGameReset;
        }

        private void OnDisable()
        {
            SPONetGameoverManager.OnServerResetGame -= OnGameReset;
        }

        [Server]
        public void Score(int playerID)
        {
            Debug.Log("Goal triggered");
            ScoreValue++;
            EventManager.Ins.TriggerEvent(SPOServerEvents.OnGoalScored, new GoalEventArgs(playerID, ScoreValue));
        }
        
        [Server]
        public void SetScoreValue(int value)
        {
            ScoreValue = value;
        }
        
        private void OnGameReset()
        {
            SetScoreValue(0);
        }
        
        private void OnScoreValueChanged(int oldValue, int newValue)
        {
            OnScoreValueChangedHookReceived?.Invoke(newValue);
            // There is no need to trigger the static event,
            // since it is already triggered on the server that will call the RPC
        }
    }
}