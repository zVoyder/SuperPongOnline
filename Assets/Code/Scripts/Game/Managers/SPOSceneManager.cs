namespace SPO.Managers
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using VUDK.Generic.Managers.Main.Bases;
    using SPO.Managers.Networking;

    public class SPOSceneManager : SceneManagerBase
    {
        [field: Header("Network Scene Managers")]
        [field: SerializeField]
        public SPONetSceneManager NetSceneManager { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            SPONetworkManager.OnServerStopped += OnServerStopped;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SPONetworkManager.OnServerStopped -= OnServerStopped;
        }

        private void OnServerStopped() // If the server is stopped, change to the lobby scene
        {
            if (NetSceneManager.IsGameScene(SceneManager.GetActiveScene().path))
                SceneManager.LoadScene(NetSceneManager.LobbyScene);
        }
    }
}