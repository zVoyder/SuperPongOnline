namespace VUDK.Generic.Managers.Main
{
    using Extensions;
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Interfaces.Networking;
    using VUDK.Features.Main.AudioSystem;
    using VUDK.Generic.Managers.Main.Bases;
    using VUDK.Patterns.Pooling;
    using VUDK.Patterns.Singleton;

    /// <summary>
    /// Managers Structure:
    /// - MainManager: Serves as the central hub for primary managers.
    /// Extensible managers:
    /// - GameManager: Orchestrates game-specific managers for precise game control; ExecutionOrder(-900).
    /// - UIManager: Manages the game's UIs; ExecutionOrder(-895).
    /// - SceneManager: Manages the game's scenes;
    /// - GameMachine: Manages the game's state through a versatile state machine; ExecutionOrder(-990).
    /// - GameStats: Manages all the possible game's configs and statistics; ExecutionOrder(-800).
    /// - GamePoolsKeys: Manages all the possible game's pool keys;
    /// Not extensible managers:
    /// - AudioManager: Manages all the possible game's audio; ExecutionOrder(-890).
    /// - PoolsManager: Manages all the possible game's pools; ExecutionOrder(-100).
    /// Interfaces:
    /// - INetworkManager: Interface for the network manager suitable for all possible network managers of various frameworks.
    /// </summary>
    [DefaultExecutionOrder(-999)]
    public sealed class MainManager : Singleton<MainManager>
    {
        [SerializeField, Header("Network Settings")]
        [Tooltip("Use this option if you want to use networking in your game.\n" +
                 "Make sure to have an INetworkManager in the scene.")]
        private bool _useNetworking;

        [field: SerializeField, Header("Game Manager")]
        public GameManagerBase GameManager { get; private set; }

        [field: SerializeField, Header("UI Manager")]
        public UIManagerBase UIManager { get; private set; }

        [field: SerializeField, Header("Scene Manager")]
        public SceneManagerBase SceneManager { get; private set; }

        [field: SerializeField, Header("Audio Manager")]
        public AudioManager AudioManager { get; private set; }

        [field: SerializeField, Header("Game Stats")]
        public GameStats GameStats { get; private set; }

        [field: SerializeField, Header("Pooling")]
        public GamePoolsKeysBase GamePoolsKeys { get; private set; }
        [field: SerializeField]
        public PoolsManager PoolsManager { get; private set; }

        public INetworkManager NetworkManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            if (_useNetworking)
            {
                INetworkManager networkManager = FinderExtension.FindObjectOfInterface<INetworkManager>();

                if (networkManager == null)
                    Debug.LogError("No INetworkManager found in the scene. Please add an INetworkManager.");
                else
                    this.NetworkManager = networkManager;
            }
        }
    }
}
