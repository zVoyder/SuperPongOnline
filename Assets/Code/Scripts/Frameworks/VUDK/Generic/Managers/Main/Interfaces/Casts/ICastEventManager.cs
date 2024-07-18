namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Features.Main.EventSystem;

    public interface ICastEventManager<T> where T : EventManager
    {
        public T EventManager { get; }
    }
}
