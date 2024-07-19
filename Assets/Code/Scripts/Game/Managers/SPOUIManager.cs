namespace SPO.Managers
{
    using UnityEngine;
    using VUDK.Generic.Managers.Main.Bases;
    using UI.Lobby;
    using UI.MainMenu;

    public class SPOUIManager : UIManagerBase
    {
        [field: Header("UI Managers")]
        [field: SerializeField]
        public UIMenuManager UIMenuManager { get; private set; }
        [field: SerializeField]
        public UILobbyManager UILobbyManager { get; private set; }
    }
}