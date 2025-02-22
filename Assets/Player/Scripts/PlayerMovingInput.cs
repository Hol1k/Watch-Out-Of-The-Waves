using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Scripts
{
    public sealed class PlayerMovingInput : MonoBehaviour
    {
        private InputAction _movingAction;
        private InputAction _sprintingAction;
        private InputAction _jumpingAction;
        private CharacterController _characterController;
    
        private Vector3 _movementVector;
        private bool _isSprinting;

        private float _jumpForce;
        private bool _jumpRequested;
        
        private Vector3 _visualModelPosition;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float smoothingFactor = 0.05f;

        [Space]
        public float moveSpeed = 6f;
        public float sprintSpeed = 9f;

        [Space]
        public float gravity = -9.81f;
        public float jumpStrength = 8f;
        public float jumpAcceleration = 20f;

        private void Awake()
        {
            _movingAction = InputSystem.actions.FindAction("Move");
            _sprintingAction = InputSystem.actions.FindAction("Sprint");
            _jumpingAction = InputSystem.actions.FindAction("Jump");
            
            _characterController = GetComponent<CharacterController>();
            
            _visualModelPosition = modelTransform.position;
        }

        private void Update()
        {
            MovingInput();
            SprintInput();
            JumpInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void LateUpdate()
        {
            SmoothModel();
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

        private void JumpInput()
        {
            if (_jumpingAction.WasPressedThisFrame() && _jumpForce <= 0f && _characterController.isGrounded)
                _jumpRequested = true;
        }

        private void ApplyMovement()
        {
            _movementVector.y = 0;
            
            _movementVector = _isSprinting ?
                    _movementVector * sprintSpeed :
                    _movementVector * moveSpeed;

            if (_jumpRequested)
            {
                _jumpForce = jumpStrength - gravity;
                _jumpRequested = false;
            }
            
            _movementVector.y = gravity + _jumpForce;
            
            _characterController.Move(_movementVector * Time.fixedDeltaTime);
            
            if (_characterController.isGrounded)
                _jumpForce = 0f;
            if (_jumpForce >= 0f)
                _jumpForce -= jumpAcceleration * Time.fixedDeltaTime;
        }

        private void SmoothModel()
        {
            _visualModelPosition = Vector3.Lerp(_visualModelPosition, transform.position, 1 / smoothingFactor * Time.deltaTime);
            modelTransform.position = _visualModelPosition;
        }
    }
}
