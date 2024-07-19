namespace VUDK.Features.Main.Camera.CameraViews
{
    using UnityEngine;
    using VUDK.Features.Main.CharacterController;

    public class FirstPersonCamera : CameraFreeLook
    {
        [SerializeField, Header("First Person Settings"), Tooltip("Target Character Controller")]
        private CharacterController3DBase _targetCharacter;

        [SerializeField]
        private Vector3 _targetPositionOffset;

        private Vector3 TargetPosition => _targetCharacter.transform.position + _targetPositionOffset;

        protected virtual void OnValidate()
        {
            if (_targetCharacter == null)
            {
                Debug.LogError($"Target Character Controller in {nameof(FirstPersonCamera)} is null.");
                return;
            }

            transform.position = TargetPosition;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            LockToTargetPosition();
        }

        protected override void LookRotate()
        {
            base.LookRotate();
            RotateTarget();
        }

        private void RotateTarget()
        {
            _targetCharacter.MoveRotation(Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        }

        private void LockToTargetPosition()
        {
            transform.position = TargetPosition;
        }
    }
}