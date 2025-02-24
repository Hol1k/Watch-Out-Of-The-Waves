using GeneralScripts;
using UnityEngine;

namespace Enemies
{
    public sealed class Enemy : MonoBehaviour, IDamageable
    {
        private const float WaterLevel = 0f;
        private bool _isInWater;
        
        private CharacterController _characterController;
        [SerializeField] private float gravity = -9.81f;
        
        private Vector3 _visualModelPosition;
        [Space]
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float smoothingFactor = 0.05f;
        
        public Transform target;
        
        [Space]
        [SerializeField] [Min(0)] private int currentHealth = 1;

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value > stats.maxHealth) currentHealth = stats.maxHealth;
                else if (value <= 0) OnDeath();
                else currentHealth = value;
            }
        }
        
        public EnemyStats stats;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            _visualModelPosition = modelTransform.position;
            
            currentHealth = stats.maxHealth;
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        private void LateUpdate()
        {
            SmoothModel();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (currentHealth > stats.maxHealth) currentHealth = stats.maxHealth;
            else if (currentHealth == 0) OnDeath();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ocean"))
                _isInWater = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ocean"))
                _isInWater = false;
        }

        private void MoveToTarget()
        {
            Vector3 movingVector = Vector3.zero;
            
            Vector3 transformPositionWithoutY = transform.position;
            transformPositionWithoutY.y = 0f;
            Vector3 targetPositionWithoutY = target.position;
            transformPositionWithoutY.y = 0f;
            if (Vector3.Distance(transformPositionWithoutY, targetPositionWithoutY) > stats.attackRange)
            {
                movingVector = (transform.rotation * Vector3.forward) * (stats.moveSpeed);
            }
            movingVector.y = _isInWater ? WaterLevel - transform.position.y : gravity;
                
            _characterController.Move(movingVector * Time.fixedDeltaTime);
            
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        }

        private void SmoothModel()
        {
            _visualModelPosition = Vector3.Lerp(_visualModelPosition, transform.position, 1 / smoothingFactor * Time.deltaTime);
            modelTransform.position = _visualModelPosition;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0) return;
            currentHealth -= (int)damage;
            if (currentHealth <= 0) OnDeath();
        }

        public void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}
