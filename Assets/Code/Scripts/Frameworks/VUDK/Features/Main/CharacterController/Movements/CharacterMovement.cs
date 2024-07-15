namespace VUDK.Features.Main.CharacterController.Movements
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using VUDK.Features.Main.InputSystem;
    using VUDK.Generic.Serializable;

    public class CharacterMovement : CharacterController3DBase
    {
        private void OnEnable()
        {
            InputsManager.Inputs.Player.Movement.canceled += StopMoveInput;
            InputsManager.Inputs.Player.Sprint.canceled += StopSprintInput;
            InputsManager.Inputs.Player.Jump.performed += JumpInput;
        }

        private void OnDisable()
        {
            InputsManager.Inputs.Player.Movement.canceled -= StopMoveInput;
            InputsManager.Inputs.Player.Sprint.canceled -= StopSprintInput;
            InputsManager.Inputs.Player.Jump.performed -= JumpInput;
        }

        protected virtual void FixedUpdate()
        {
            MoveInput();
        }

        private void MoveInput()
        {
            InputAction movement = InputsManager.Inputs.Player.Movement;
            InputAction sprint = InputsManager.Inputs.Player.Sprint;

            if (!movement.IsInProgress()) return;

            if (sprint.IsInProgress())
                Sprint();

            MoveCharacter(movement.ReadValue<Vector2>());
        }

        private void StopMoveInput(InputAction.CallbackContext context)
        {
            StopCharacter();
        }

        private void JumpInput(InputAction.CallbackContext context)
        {
            Jump(Vector3.up);
        }

        private void StopSprintInput(InputAction.CallbackContext context)
        {
            StopSprint();
        }
    }
}
