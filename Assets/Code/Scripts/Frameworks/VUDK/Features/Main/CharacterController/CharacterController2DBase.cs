namespace VUDK.Features.Main.CharacterController
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterController2DBase : CharacterControllerBase
    {
        protected Rigidbody2D Rigidbody;
        public override bool IsGrounded => Physics2D.OverlapCircle(transform.position + GroundedOffset, GroundedRadius, GroundLayers);

        protected virtual void Awake()
        {
            TryGetComponent(out Rigidbody);
        }

        public override void Jump(Vector3 direction)
        {
            if (!CanJump) return;

            base.Jump(direction);
            Rigidbody.AddForce(direction * JumpForce, ForceMode2D.Impulse);
        }

        public override bool MoveCharacter(Vector2 direction)
        {
            if (!CanMove) return false;

            base.MoveCharacter(direction);
            Vector2 movementDirection = /* transform.forward * InputMove.y + */ transform.right * InputMove.x;
            Vector2 velocity = new Vector2(movementDirection.x * CurrentSpeed, Rigidbody.velocity.y);
            SetVelocity(velocity);

            return true;
        }

        public override bool StopCharacter()
        {
            if (!CanMove) return false;

            base.StopCharacter();
            SetVelocity(new Vector3(0f, Rigidbody.velocity.y, 0f));

            return true;
        }

        public override void SetVelocity(Vector3 velocity)
        {
            Rigidbody.velocity = velocity;
        }
    }
}
