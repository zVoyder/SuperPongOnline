namespace SPO.Managers.GameStats
{
    using UnityEngine;
    using Mirror;
    using VUDK.Extensions;

    [RequireComponent(typeof(NetworkIdentity))]
    public class SPOGameSyncStats : NetworkBehaviour
    {
        // Avoid cheating by syncing these sensitive variables
        [field: Header("Gameplay Settings")]
        [field: SerializeField, SyncVar]
        public float BracketSpeed { get; private set; } = 5f;
        [field: SerializeField, SyncVar]
        public float BallSpeed { get; private set; } = 10f;
        [field: SerializeField, Range(0f, 1f), SyncVar]
        public float BallStartAngleRange { get; private set; } = 0.7f;
        [field: SerializeField, SyncVar]
        public float BallSpeedMultiplier { get; private set; } = 1.1f;
        [field: SerializeField, SyncVar]
        public int ScoreToWin { get; private set; } = 5;
        [field: SerializeField, SyncVar]
        public float TopBoundary { get; private set; } = 7.5f;
        [field: SerializeField, SyncVar]
        public float BottomBoundary { get; private set; } = -7.5f;

        private void Awake()
        {
            SetNetSyncs();
        }
        
        /// <summary>
        /// Set the network syncs for the SyncStats.
        /// </summary>
        private void SetNetSyncs()
        {
            // Only the server should be able to change these values
            syncDirection = SyncDirection.ServerToClient;
            syncMode = SyncMode.Observers;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 position = Vector3.zero;
            Vector3 top = position + Vector3.up * TopBoundary;
            Vector3 bottom = position + Vector3.up * BottomBoundary;
            GizmosExtension.DrawArrow(position, top, 1f);
            GizmosExtension.DrawArrow(position, bottom, 1f);

            Camera main = Camera.main;
            if (main != null)
            {
                float halfWidth = main.orthographicSize * main.aspect;
                Gizmos.DrawLine(top, top + Vector3.right * halfWidth);
                Gizmos.DrawLine(top, top + Vector3.left * halfWidth);
                Gizmos.DrawLine(bottom, bottom + Vector3.right * halfWidth);
                Gizmos.DrawLine(bottom, bottom + Vector3.left * halfWidth);
            }
        }
#endif
    }
}