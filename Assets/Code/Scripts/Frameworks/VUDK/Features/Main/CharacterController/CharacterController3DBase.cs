namespace VUDK.Features.Main.CharacterController
{
    using UnityEngine;
    using VUDK.Extensions;

    [RequireComponent(typeof(Rigidbody))]
    public abstract class CharacterController3DBase : CharacterControllerBase
    {
        protected Rigidbody Rigidbody { get; private set; }
        public override bool IsGrounded => Physics.CheckSphere(transform.position + GroundedOffset, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        protected virtual void Awake()
        {
            TryGetComponent(out Rigidbody rb);
            Rigidbody = rb;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void SetVelocity(Vector3 velocity)
        {
            Rigidbody.linearVelocity = velocity;
        }

        public override void Jump(Vector3 direction)
        {
            if (!CanJump) return;

            base.Jump(direction);
            Rigidbody.AddForce(direction * JumpForce, ForceMode.Impulse);
        }

        public override bool MoveCharacter(Vector2 direction)
        {
            if (!CanMove) return false;
            base.MoveCharacter(direction);

            if (!IsSliding)
            {
                Vector3 movementDirection = transform.forward * InputMove.y + transform.right * InputMove.x;

                if (!CheckWall(movementDirection))
                {
                    Vector3 velocity = new Vector3(movementDirection.x * CurrentSpeed, Rigidbody.linearVelocity.y, movementDirection.z * CurrentSpeed);
                    SetVelocity(velocity);
                }
            }

            return true;
        }

        protected override void CheckSlope()
        {
            if (IsOnSlope(out float slopeAngle, out Vector3 slopeDirection))
            {
                if (!CanClimbSlope(slopeAngle))
                {
                    IsSliding = true;
                    Rigidbody.AddForce(-slopeDirection * 10f, ForceMode.Acceleration);
                    return;
                }
                else
                {
                    Rigidbody.AddForce(-Vector3.up * 10f, ForceMode.Acceleration);
                }
            }

            IsSliding = false;
        }

        private bool CheckWall(Vector3 direction)
        {
            float rayLength = GroundedRadius * 1.5f;
            Debug.DrawRay(transform.position, direction * rayLength, Color.red);
            return Physics.Raycast(transform.position, direction, rayLength, GroundLayers) && !IsGrounded;

            //dir = transform.position + direction;
            //return Physics.CheckSphere(transform.position + direction, WallCheckRadius, GroundLayers);
        }

        public override bool StopCharacter()
        {
            if (!CanMove) return false;

            base.StopCharacter();
            SetVelocity(new Vector3(0f, Rigidbody.linearVelocity.y, 0f));
            return true;
        }

        public void MoveRotation(Quaternion rotation)
        {
            Rigidbody.MoveRotation(rotation);
        }

        protected override bool IsOnSlope(out float slopeAngle, out Vector3 slopeDirection)
        {
            if (Physics.Raycast(transform.position + GroundedOffset, Vector3.down, out RaycastHit hit, GroundedRadius, GroundLayers))
            {
                slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if (slopeAngle > 1f)
                {
                    slopeDirection = Vector3.up - hit.normal * Vector3.Dot(Vector3.up, hit.normal);
                    return true;
                }
            }

            slopeAngle = -1f;
            slopeDirection = Vector3.zero;
            return false;
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + GroundedOffset, 0.1f);
            Gizmos.DrawRay(transform.position + GroundedOffset, Vector3.down * GroundedRadius);
        }
#endif
    }
}