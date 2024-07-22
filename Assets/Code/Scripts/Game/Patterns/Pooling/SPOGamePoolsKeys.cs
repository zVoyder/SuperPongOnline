namespace SPO.Patterns.Pooling
{
    using UnityEngine;
    using VUDK.Features.Main.ScriptableKeys;
    using VUDK.Generic.Managers.Main.Bases;

    public class SPOGamePoolsKeys : GamePoolsKeysBase
    {
        [field: Header("Pool Keys")]
        [field: SerializeField]
        public ScriptableKey BallKey { get; private set; }
    }
}