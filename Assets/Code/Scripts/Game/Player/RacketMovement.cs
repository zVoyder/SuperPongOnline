namespace SPO.Player
{
    using System;
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
        
        public void Init(PlayerRacketManager arg1, SPOGameStats arg2, Rigidbody2D arg3)
        {
            _playerRacketManager = arg1;
            _gameStats = arg2;
            _rigidbody = arg3;
            
            _rigidbody.simulated = true;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        
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
            if (!isLocalPlayer) return;
            
            Vector2 direction = InputsManager.Inputs.Player.Movement.ReadValue<Vector2>();
            float v = Mathf.RoundToInt(direction.y); // Round the value to 1, 0 or -1
            CmdSendInputMove(v);
        }
        
        [ClientCallback]
        private void OnStopMoveInput(InputAction.CallbackContext obj)
        {
            if (!isLocalPlayer) return;
            
            CmdStop();
        }
        
        [Command]
        private void CmdSendInputMove(float vertical)
        {
            ServerMove(vertical);
        }
        
        [Command]
        private void CmdStop()
        {
            ServerStop();
        }
        
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
        
        [Server]
        private void ServerStop()
        {
            _rigidbody.velocity = Vector2.zero;
        }
        
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