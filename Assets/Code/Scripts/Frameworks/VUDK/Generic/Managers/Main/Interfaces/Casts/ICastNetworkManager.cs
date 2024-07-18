namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Interfaces.Networking;

    public interface ICastNetworkManager<T> where T : INetworkManager
    {
        public T NetworkManager { get; }
    }
}