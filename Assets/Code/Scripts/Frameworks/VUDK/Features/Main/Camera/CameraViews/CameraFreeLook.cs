namespace VUDK.Features.Main.Camera.CameraViews
{
    using System;
    using UnityEngine;
    using VUDK.Extensions;
    using VUDK.Features.Main.InputSystem;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CameraFreeLook : MonoBehaviour
    {
        [Header("Sensitivity Settings")]
        [SerializeField, Range(1f, 100f)]
        protected float Sensitivity = 2f;
        [SerializeField, Range(0f, 1f)]
        private float _sensitivityCoefficient = 0.1f;

        [Header("Smoothing Settings")]
        [SerializeField]
        private bool _useSmoothing;
        [SerializeField, Min(0f)]
        private float _smoothTime = 0.1f;

        [Header("Angles Settings")]
        [Tooltip("How far in degrees you can move the camera up")]
        [SerializeField]
        private float _topClamp = 90.0f;

        [Tooltip("How far in degrees you can move the camera down")]
        [SerializeField]
        private float _bottomClamp = -90.0f;

        protected float ClampSens => Sensitivity * _sensitivityCoefficient / 100f;
        protected Camera Camera { get; private set; }

        private bool _canLook = true;
        private Vector3 _targetRotation;

        protected virtual void Awake()
        {
            TryGetComponent(out Camera cam);
            Camera = cam;
        }

        protected virtual void Start()
        {
            SetTargetRotation(transform.root.rotation);
        }

        /// <summary>
        /// Enables the camera inputs.
        /// </summary>
        public virtual void Enable()
        {
            InputsManager.Inputs.Camera.Enable();
            _canLook = true;
        }

        /// <summary>
        /// Disables the camera inputs.
        /// </summary>
        public virtual void Disable()
        {
            InputsManager.Inputs.Camera.Disable();
            _canLook = false;
        }

        /// <summary>
        /// Sets the target rotation.
        /// </summary>
        /// <param name="rotation">Target rotation as a Quaternion.</param>
        public void SetTargetRotation(Quaternion rotation)
        {
            _targetRotation = rotation.SignedEulerAngles();
        }

        /// <summary>
        /// Resets the target rotation to Vector3.zero.
        /// </summary>
        public virtual void ResetTargetRotation()
        {
            _targetRotation = Vector3.zero;
        }

        protected virtual void LateUpdate()
        {
            LookRotate();
        }

        /// <summary>
        /// Rotates the camera.
        /// </summary>
        protected virtual void LookRotate()
        {
            if (!_canLook) return;

            Vector2 lookDirection = InputsManager.Inputs.Camera.Look.ReadValue<Vector2>();
            float mouseX = lookDirection.x * ClampSens;
            float mouseY = lookDirection.y * ClampSens;

            _targetRotation.y += mouseX;
            _targetRotation.x -= mouseY;
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, _bottomClamp, _topClamp);

            Quaternion eulerRot = Quaternion.Euler(_targetRotation);
            transform.root.rotation = _useSmoothing ? Quaternion.Slerp(transform.root.rotation, eulerRot, _smoothTime * Time.deltaTime) : eulerRot;
        }
    }
}