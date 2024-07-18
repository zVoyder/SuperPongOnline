namespace SPO.Player.Interfaces
{
    using Mirror;
    using UnityEngine;
    using VUDK.Patterns.Initialization.Interfaces;

    public interface INetPlayer
    {
        public int PlayerID { get; }

        public void AssignNetPlayer(NetworkConnectionToClient connectionToClient);
    }
}