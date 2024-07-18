namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Bases;

    public interface ICastUIManager<T> where T : UIManagerBase
    {
        public T UIManager { get; }
    }
}