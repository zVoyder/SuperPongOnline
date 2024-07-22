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
        [Header("Network Scenes Delay")]
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
        
        /// <summary>
        /// Starts the scene change process to the game scene.
        /// </summary>
        [Server]
        public void ChangeSceneToGame()
        {
            RpcStartChangingScene((int)_gameSceneDelay);
            _delayTask.Start(_gameSceneDelay);
            _delayTask.OnTaskCompleted += OnChangingToGameScene;
        }

        /// <summary>
        /// Starts the scene change process to the lobby scene.
        /// </summary>
        [Server]
        public void ChangeSceneToLobby()
        {
            RpcStartChangingScene((int)_lobbySceneDelay);
            _delayTask.Start(_lobbySceneDelay);
            _delayTask.OnTaskCompleted += OnChangingToLobbyScene;
        }
        
        /// <summary>
        /// Stops the scene change process.
        /// </summary>
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
        
        /// <summary>
        /// Checks if the current scene is the game scene.
        /// </summary>
        /// <returns>True if the current scene is the game scene, false otherwise.</returns>
        public bool IsCurrentSceneGame()
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            return IsGameScene(currentScenePath);
        }
        
        /// <summary>
        /// Checks if the current scene is the lobby scene.
        /// </summary>
        /// <returns>True if the current scene is the lobby scene, false otherwise.</returns>
        public bool IsCurrentSceneLobby()
        {
            string currentScenePath = SceneManager.GetActiveScene().path;
            return IsLobbyScene(currentScenePath);
        }

        /// <summary>
        /// Checks if the scene path is the lobby scene.
        /// </summary>
        /// <param name="scenePath">The scene path to check.</param>
        /// <returns>True if the scene path is the lobby scene, false otherwise.</returns>
        public bool IsLobbyScene(string scenePath)
        {
            return scenePath == LobbyScene;
        }

        /// <summary>
        /// Checks if the scene path is the game scene.
        /// </summary>
        /// <param name="scenePath">The scene path to check.</param>
        /// <returns>True if the scene path is the game scene, false otherwise.</returns>
        public bool IsGameScene(string scenePath)
        {
            return scenePath == GameScene;
        }

        /// <summary>
        /// Event handler for when the scene starts changing to the game scene.
        /// </summary>
        private void OnChangingToGameScene()
        {
            Debug.Log("Changing to Game Scene...");
            _currentSecondsDelay = 0;
            NetworkManager.singleton.ServerChangeScene(GameScene);
        }
        
        /// <summary>
        /// Event handler for when the scene starts changing to the lobby scene.
        /// </summary>
        private void OnChangingToLobbyScene()
        {
            Debug.Log("Changing to Lobby Scene...");
            _currentSecondsDelay = 0;
            NetworkManager.singleton.ServerChangeScene(LobbyScene);
        }

        /// <summary>
        /// RPC for starting the scene change process.
        /// </summary>
        /// <param name="totalSecondsDelay">The total seconds delay for the scene change.</param>
        [ClientRpc]
        private void RpcStartChangingScene(int totalSecondsDelay)
        {
            OnRPCStartChangingScene?.Invoke(totalSecondsDelay);
        }
        
        /// <summary>
        /// RPC for stopping the scene change process.
        /// </summary>
        [ClientRpc]
        private void RpcStopChangingScene()
        {
            OnRPCStopChangingScene?.Invoke();
        }
        
        /// <summary>
        /// RPC for updating the scene change delay time.
        /// </summary>
        /// <param name="currentSecondsDelay">The current seconds delay for the scene change.</param>
        [ClientRpc]
        private void RpcUpdateSceneDelayTime(int currentSecondsDelay)
        {
            int remainingSeconds = (int)_gameSceneDelay - currentSecondsDelay;
            OnRPCChangingSceneDelayUpdate?.Invoke(remainingSeconds);
        }
    }
}