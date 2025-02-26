using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Scripts
{
    public sealed class PlayerInput : MonoBehaviour
    {
        private InputAction _movingAction;
        private InputAction _sprintingAction;
        private InputAction _jumpingAction;
        private InputAction _mouseLeftClickAction;
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

        [Space]
        [SerializeField] private float attackRange;
        [SerializeField] [Tooltip("By degrees")] private float attackRadius;
        [SerializeField] [Tooltip("Attack thickness by OY")] private float attackSize = 2f;
        [SerializeField] private float damage;
        [SerializeField] [Tooltip("Attacks per minute")] private float attackSpeed;

        private float _attackCooldown;

        private void Awake()
        {
            _movingAction = InputSystem.actions.FindAction("Move");
            _sprintingAction = InputSystem.actions.FindAction("Sprint");
            _jumpingAction = InputSystem.actions.FindAction("Jump");
            _mouseLeftClickAction = InputSystem.actions.FindAction("Mouse LeftClick");
            
            _characterController = GetComponent<CharacterController>();
            
            _visualModelPosition = modelTransform.position;
        }

        private void Update()
        {
            MovingInput();
            SprintInput();
            JumpInput();
            AttackInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            CalculateAttackCooldown();
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

        private void AttackInput()
        {
            if (_mouseLeftClickAction.IsPressed() &&
                PlayerStateMachine.State == PlayerStateMachine.PlayerState.Default)
            {
                if (_attackCooldown > 0f)
                    return;
                
                // ReSharper disable once Unity.PreferNonAllocApi
                var sphereColliders = Physics.OverlapSphere(transform.position, attackRange);
                // ReSharper disable once Unity.PreferNonAllocApi
                var boxColliders = Physics.OverlapBox(transform.position,
                    new Vector3(attackRange, attackSize / 2f, attackRange));

                List<Enemy> enemiesToAttack = new();
                foreach (var currCollider in sphereColliders.Intersect(boxColliders))
                {
                    Vector3 directionToTarget = (currCollider.transform.position - transform.position).normalized;
                    directionToTarget.y = 0f;
                    float angle = Vector3.Angle(transform.forward, directionToTarget);
                    
                    if (angle <= attackRadius / 2 && currCollider.TryGetComponent(out Enemy enemy))
                        enemiesToAttack.Add(enemy);
                }

                foreach (var enemy in enemiesToAttack)
                {
                    enemy.TakeDamage(damage);
                }

                _attackCooldown = 60f / attackSpeed;
            }
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

        private void CalculateAttackCooldown()
        {
            if (_attackCooldown > 0f)
                _attackCooldown -= Time.fixedDeltaTime;
        }

        private void SmoothModel()
        {
            _visualModelPosition = Vector3.Lerp(_visualModelPosition, transform.position, 1 / smoothingFactor * Time.deltaTime);
            modelTransform.position = _visualModelPosition;
        }
    }
}
