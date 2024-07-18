namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Bases;

    public interface ICastSceneManager<T> where T : SceneManagerBase
    {
        public T SceneManager { get; }
    }
}
