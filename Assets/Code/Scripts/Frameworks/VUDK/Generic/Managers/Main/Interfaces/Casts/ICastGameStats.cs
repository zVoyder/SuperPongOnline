namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Bases;

    public interface ICastGameStats<T> where T : GameStats
    {
        public T GameStats { get; }
    }
}
