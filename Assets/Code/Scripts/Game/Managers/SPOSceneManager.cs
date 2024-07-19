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
    }
}