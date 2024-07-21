namespace SPO.Managers.Networking
{
    using System;
    using UnityEngine;
    using Mirror;
    using UnityEngine.SceneManagement;
    using UnityEngine.Serialization;
    using VUDK.Generic.Serializable;

    public class SPONetSceneManager : NetworkBehaviour
    {
        [FormerlySerializedAs("delay"),Header("Scene Delay")]
        [SerializeField]
        private float _gameSceneDelay = 3f;
        [SerializeField]
        private float _lobbySceneDelay = 3f;
        
        [field: Header("Scenes")]
        [field: Scene, SerializeField]
        public string GameScene { get; private set; }
        [field: Scene, SerializeField]
        public string LobbyScene { get; private set; }

        private DelayTask _delayTask;
        private int _currentSecondsDelay;

        public bool IsChangingScene => _delayTask.IsRunning;

        public static event Action<int> OnRPCStartChangingScene;
        public static event Action OnRPCStopChangingScene;
        public static event Action<int> OnRPCChangingSceneDelayUpdate;

        private void Awake()
        {
            _delayTask = new DelayTask();
        }

        private void OnEnable()
        {
            SPONetworkManager.OnLobbyPlayersReady += ChangeSceneToGame;
            SPONetworkManager.OnLobbyPlayersUnready += StopChangingScene;
        }
        
        private void OnDisable()
        {
            SPONetworkManager.OnLobbyPlayersReady -= ChangeSceneToGame;
            SPONetworkManager.OnLobbyPlayersUnready -= StopChangingScene;
        }

        [ServerCallback]
        private void Update()
        {
            if (!_delayTask.Process()) return;

            if (_currentSecondsDelay != (int)_delayTask.ElapsedTime)
            {
                _currentSecondsDelay = (int)_delayTask.ElapsedTime;
                RpcUpdateSceneDelayTime(_currentSecondsDelay);
            }
        }
        
        [Server]
        public void ChangeSceneToGame()
        {
            RpcStartChangingScene((int)_gameSceneDelay);
            _delayTask.Start(_gameSceneDelay);
            _delayTask.OnTaskCompleted += OnChangingToGameScene;
        }

        [Server]
        public void ChangeSceneToLobby()
        {
            RpcStartChangingScene((int)_lobbySceneDelay);
            _delayTask.Start(_lobbySceneDelay);
            _delayTask.OnTaskCompleted += OnChangingToLobbyScene;
        }
        
        [Server]
        public void StopChangingScene()
        {
            if (!IsChangingScene) return;
            
            _delayTask.Stop();
            _delayTask.Reset();
            _delayTask.OnTaskCompleted -= OnChangingToGameScene;
            _delayTask.OnTaskCompleted -= OnChangingToLobbyScene;
            RpcStopChangingScene();
        }
        
        public bool IsCurrentSceneGame()
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            return IsGameScene(currentScenePath);
        }
        
        public bool IsCurrentSceneLobby()
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            return IsLobbyScene(currentScenePath);
        }

        public bool IsLobbyScene(string scenePath)
        {
            return scenePath == LobbyScene;
        }

        public bool IsGameScene(string scenePath)
        {
            return scenePath == GameScene;
        }

        private void OnChangingToGameScene()
        {
            Debug.Log("Changing to Game Scene...");
            _currentSecondsDelay = 0;
            NetworkManager.singleton.ServerChangeScene(GameScene);
        }
        
        private void OnChangingToLobbyScene()
        {
            Debug.Log("Changing to Lobby Scene...");
            _currentSecondsDelay = 0;
            NetworkManager.singleton.ServerChangeScene(LobbyScene);
        }

        [ClientRpc]
        private void RpcStartChangingScene(int totalSecondsDelay)
        {
            OnRPCStartChangingScene?.Invoke(totalSecondsDelay);
        }
        
        [ClientRpc]
        private void RpcStopChangingScene()
        {
            OnRPCStopChangingScene?.Invoke();
        }
        
        [ClientRpc]
        private void RpcUpdateSceneDelayTime(int currentSecondsDelay)
        {
            int remainingSeconds = (int)_gameSceneDelay - currentSecondsDelay;
            OnRPCChangingSceneDelayUpdate?.Invoke(remainingSeconds);
        }
    }
}