namespace VUDK.Features.Main.CharacterController
{
    using System;
    using UnityEngine;
    using VUDK.Generic.Serializable;

    public abstract class CharacterControllerBase : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Min(0f)]
        public float MoveSpeed = 1f;
        [Min(0f)]
        public float SprintMultiplier = 1.5f;
        [Min(0f)]
        public float JumpForce;
        public bool HasAirControl;
        [Min(0f)]
        public float AirSpeed = 1f;

        [SerializeField, Header("Ground Settings")]
        protected Vector3 GroundedOffset;
        [SerializeField]
        protected LayerMask GroundLayers;
        [SerializeField, Min(0f)]
        protected float GroundedRadius = 0.5f;
        [SerializeField, Min(0f)]
        protected Vector3 WallCheckOffset;
        [SerializeField, Min(0f)]
        protected float WallCheckRadius = 0.5f;
        [SerializeField, Range(0f, 360f)]
        protected float MaxSlopeAngle = 45f;

        protected bool IsRunning;
        protected bool IsSliding;
        protected float CurrentSpeed;

        public Vector2 InputMove { get; private set; }
        public abstract bool IsGrounded { get; }
        protected bool CanJump => IsGrounded;
        protected bool CanMove => IsGrounded || (!IsGrounded && HasAirControl);

        public event Action<Vector2> OnMovement;
        public event Action OnStopMovement;
        public event Action OnJump;

        protected virtual void Update()
        {
            CheckSlope();
            MoveUpdateVelocity();
        }

        public abstract void SetVelocity(Vector3 velocity);

        /// <summary>
        /// Moves the character in the specified direction at the set speed using rigidbody velocity.
        /// </summary>
        /// <param name="direction">Direction.</param>
        public virtual bool MoveCharacter(Vector2 direction)
        {
            InputMove = direction;
            OnMovement?.Invoke(direction);
            return true;
        }

        public virtual bool StopCharacter()
        {
            StopInputMovementCharacter();
            return true;
        }

        /// <summary>
        /// Makes the character jump in the specified direction at the set jump force using rigidbody addforce.
        /// </summary>
        /// <param name="direction">Direction.</param>
        public virtual void Jump(Vector3 direction)
        {
            OnJump?.Invoke();
        }

        /// <summary>
        /// Resets to zero its input movement.
        /// </summary>
        public void StopInputMovementCharacter()
        {
            InputMove = Vector2.zero;
            OnStopMovement?.Invoke();
            StopSprint();
        }

        public void Sprint()
        {
            IsRunning = true;
        }

        public void StopSprint()
        {
            IsRunning = false;
        }

        protected abstract void CheckSlope();

        protected abstract bool IsOnSlope(out float slopeAngle, out Vector3 slopeDirection);

        protected bool CanClimbSlope(float slopeAngle)
        {
            return slopeAngle <= MaxSlopeAngle;
        }

        private void MoveUpdateVelocity()
        {
            if (IsGrounded)
            {
                if(IsRunning)
                    CurrentSpeed = MoveSpeed * SprintMultiplier;
                else
                    CurrentSpeed = MoveSpeed;
            }
            else if (HasAirControl)
            {
                if(IsRunning)
                    CurrentSpeed = AirSpeed * SprintMultiplier;
                else
                    CurrentSpeed = AirSpeed;
            }
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            DrawCheckGroundSphere();
        }

        private void DrawCheckGroundSphere()
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Gizmos.DrawSphere(transform.position + GroundedOffset, GroundedRadius);
        }
#endif
    }
}