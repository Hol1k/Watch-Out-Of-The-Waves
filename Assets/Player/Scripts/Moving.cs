using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Scripts
{
    public sealed class Moving : MonoBehaviour
    {
        private InputAction _movingAction;
        private InputAction _sprintingAction;
        private CharacterController _characterController;
    
        private Vector3 _movementVector;
        private bool _isSprinting;

        public float gravity = -9.81f;
        public float moveSpeed = 10f;
        public float sprintSpeed = 15f;

        private void Awake()
        {
            _movingAction = InputSystem.actions.FindAction("Move");
            _sprintingAction = InputSystem.actions.FindAction("Sprint");
            
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            MovingInput();
            SprintInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void MovingInput()
        {
            var movementVector = _movingAction.ReadValue<Vector2>();
            
            _movementVector.x = movementVector.x;
            _movementVector.z = movementVector.y;
        }

        private void SprintInput()
        {
            _isSprinting = _sprintingAction.IsPressed();
        }

        private void ApplyMovement()
        {
            _movementVector.y = 0;
            
            _movementVector = _isSprinting ?
                    _movementVector * sprintSpeed :
                    _movementVector * moveSpeed;

            _movementVector.y = gravity;
            
            _characterController.Move(_movementVector * Time.fixedDeltaTime);
        }
    }
}
