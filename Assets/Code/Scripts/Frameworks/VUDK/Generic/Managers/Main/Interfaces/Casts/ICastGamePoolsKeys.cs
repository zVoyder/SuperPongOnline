namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Bases;

    public interface ICastGamePoolsKeys<T> where T : GamePoolsKeysBase
    {
        public T GamePoolsKeys { get; }
    }
}