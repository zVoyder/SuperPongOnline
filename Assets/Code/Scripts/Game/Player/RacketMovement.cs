namespace SPO.Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Mirror;
    using VUDK.Features.Main.InputSystem;
    using VUDK.Patterns.Initialization.Interfaces;
    using SPO.Managers.GameStats;

    public class RacketMovement : NetworkBehaviour, IInit<PlayerRacketManager, SPOGameStats, Rigidbody2D>
    {
        private SPOGameStats _gameStats;
        private Rigidbody2D _rigidbody;
        private PlayerRacketManager _playerRacketManager;
        private float _currentVertical;
        
        private float Speed => _gameStats.SyncStats.BracketSpeed * 10f;
        private float TopBoundary => _gameStats.SyncStats.TopBoundary;
        private float BottomBoundary => _gameStats.SyncStats.BottomBoundary;
        
        /// <summary>
        /// Initializes the racket movement.
        /// </summary>
        /// <param name="arg1">The player racket manager.</param>
        /// <param name="arg2">The game stats.</param>
        /// <param name="arg3">The rigidbody.</param>
        public void Init(PlayerRacketManager arg1, SPOGameStats arg2, Rigidbody2D arg3)
        {
            _playerRacketManager = arg1;
            _gameStats = arg2;
            _rigidbody = arg3;
            
            _rigidbody.simulated = true;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        
        /// <summary>
        /// Checks if the racket movement is correctly initialized.
        /// </summary>
        /// <returns>True if the racket movement is correctly initialized, false otherwise.</returns>
        public bool Check()
        {
            return _gameStats != null && _rigidbody != null;
        }
        
        [ClientCallback]
        private void OnEnable()
        {
            InputsManager.Inputs.Player.Movement.canceled += OnStopMoveInput;
        }

        [ClientCallback]
        private void OnDisable()
        {
            InputsManager.Inputs.Player.Movement.canceled -= OnStopMoveInput;
        }

        [ClientCallback]
        private void FixedUpdate()
        {
            // This control must be here since if we do this on the Command Method,
            // the command will be called on the server
            if (!isOwned) return; // Do not use isLocalPlayer since this is not the local player obeject
            
            Vector2 direction = InputsManager.Inputs.Player.Movement.ReadValue<Vector2>();
            float v = Mathf.RoundToInt(direction.y); // Round the value to 1, 0 or -1
            CmdSendInputMove(v);
        }
        
        /// <summary>
        /// Event handler for when the player stops moving.
        /// </summary>
        /// <param name="context">The input action callback context.</param>
        [ClientCallback]
        private void OnStopMoveInput(InputAction.CallbackContext context)
        {
            if (!isOwned) return;
            
            CmdStop();
        }
        
        /// <summary>
        /// Command to send the input move.
        /// </summary>
        /// <param name="vertical">The vertical input.</param>
        [Command]
        private void CmdSendInputMove(float vertical)
        {
            ServerMove(vertical);
        }
        
        /// <summary>
        /// Command to stop the racket movement.
        /// </summary>
        [Command]
        private void CmdStop()
        {
            ServerStop();
        }
        
        /// <summary>
        /// Moves the racket.
        /// </summary>
        /// <param name="vertical">The vertical input.</param>
        [Server]
        private void ServerMove(float vertical)
        {
            if (!CanMove(vertical))
            {
                ServerStop();
                return;
            }
            
            Vector2 direction = new Vector2(0, vertical);
            _rigidbody.velocity = direction * (Speed * Time.fixedDeltaTime);
        }
        
        /// <summary>
        /// Stops the racket.
        /// </summary>
        [Server]
        private void ServerStop()
        {
            _rigidbody.velocity = Vector2.zero;
        }
        
        /// <summary>
        /// Checks if the racket can move.
        /// </summary>
        /// <param name="vertical">The vertical input.</param>
        /// <returns>True if the racket can move, false otherwise.</returns>
        [Server]
        private bool CanMove(float vertical)
        {
            float h = transform.position.y;
            
            if (vertical > 0)
                return h < TopBoundary;
            
            if (vertical < 0)
                return h > BottomBoundary;
            
            return true;
        }
    }
}