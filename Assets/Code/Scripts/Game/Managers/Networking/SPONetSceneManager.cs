namespace SPO.Managers.Networking
{
    using UnityEngine;
    using Mirror;
    using UnityEngine.SceneManagement;

    public class SPONetSceneManager : NetworkBehaviour
    {
        [field: Header("Scenes")]
        [field: Scene, SerializeField]
        public string GameScene { get; private set; }
        [field: Scene, SerializeField]
        public string LobbyScene { get; private set; }
        
        [Server]
        public void ChangeSceneToGame()
        {
            NetworkManager.singleton.ServerChangeScene(GameScene);
        }
        
        [Server]
        public void ChangeSceneToLobby()
        {
            NetworkManager.singleton.ServerChangeScene(LobbyScene);
        }
        
        public bool IsLobbyScene()
        {
            return SceneManager.GetActiveScene().name == LobbyScene;
        }
        
        public bool IsGameScene()
        {
            return SceneManager.GetActiveScene().name == GameScene;
        }
    }
}