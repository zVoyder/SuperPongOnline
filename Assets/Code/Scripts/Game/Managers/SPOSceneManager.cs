namespace SPO.Managers
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Bases;
    using SPO.Managers.Networking;

    public class SPOSceneManager : SceneManagerBase
    {
        [field: Header("Network Scene Managers")]
        [field: SerializeField]
        public SPONetSceneManager NetSceneManager { get; private set; }
        
        // No need to do it since mirror will switch to offline scene
        // private void OnServerStopped()
        // {
        //     if (!NetSceneManager.IsCurrentSceneGame()) return;
        //     
        //     int lobbySceneIndex = SceneManager.GetSceneByName(NetSceneManager.LobbyScene).buildIndex;
        //     ChangeScene(lobbySceneIndex);
        // }
    }
}