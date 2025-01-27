﻿namespace VUDK.Generic.Managers.Main.Interfaces.Casts
{
    using VUDK.Generic.Managers.Main.Bases;

    public interface ICastGameManager<T> where T : GameManagerBase
    {
        public T GameManager { get; }
    }
}
