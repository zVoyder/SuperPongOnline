namespace SPO.Managers.EventArgs
{
    using System;
    using Mirror;

    public class ConnectedPlayerEventArgs : EventArgs
    {
        public NetworkConnectionToClient Connection { get; private set; }
        public int PlayerNumIndex { get; private set; }

        public ConnectedPlayerEventArgs(NetworkConnectionToClient connection, int playerNumIndex)
        {
            Connection = connection;
            PlayerNumIndex = playerNumIndex;
        }
    }
}