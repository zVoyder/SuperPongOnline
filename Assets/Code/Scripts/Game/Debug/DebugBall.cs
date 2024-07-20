#if UNITY_EDITOR
namespace SPO.Debug
{
    using UnityEngine;
    using Mirror;
    using SPO.Level.Ball;

    [RequireComponent(typeof(BallManager))]
    public class DebugBall : NetworkBehaviour
    {
        private BallManager _ballManager;
        
        private void Awake()
        {
            TryGetComponent(out _ballManager);
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 200, 200));
            GUILayout.Label("Ball ID: " + _ballManager.AttackerPlayerID);
            GUILayout.EndArea();
        }
    }
}
#endif