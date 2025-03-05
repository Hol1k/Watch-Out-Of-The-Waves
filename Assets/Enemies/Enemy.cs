using GeneralScripts;
using Inventory;
using Inventory.Items;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies
{
    public sealed class Enemy : MonoBehaviour, IDamageable
    {
        private const float WaterLevel = 0f;
        private bool _isInWater;
        
        private bool _isTargetOnRange;
        [SerializeField] private AttackType attackType;
        [Tooltip("Projectile for attack prefab with AttackProjectile.cs")]
        [SerializeField] private GameObject attackProjectilePrefab;
        [SerializeField] private Vector3 attackOffset;

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

        [Space]
        [SerializeField] private EntityInventory inventory;
        [SerializeField] private GameObject itemPrefab;
        
        private float _attackCooldown;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            _visualModelPosition = modelTransform.position;
            
            currentHealth = stats.maxHealth;
        }

        private void FixedUpdate()
        {
            MoveToTarget();
            AttackTarget();
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
            if (target)
            {
                Vector3 movingVector = Vector3.zero;

                _isTargetOnRange = false;
                Collider[] colliders = Physics.OverlapSphere(transform.position, stats.attackRange);
                foreach (Collider buildingCollider in colliders)
                {
                    if (buildingCollider.transform == target)
                    {
                        _isTargetOnRange = true;
                        break;
                    }
                }

                if (!_isTargetOnRange)
                {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    movingVector = (transform.rotation * Vector3.forward) * (stats.moveSpeed);
                }

                movingVector.y = _isInWater ? WaterLevel - transform.position.y : gravity;

                _characterController.Move(movingVector * Time.fixedDeltaTime);
            }
            else
            {
                _isTargetOnRange = false;
            }
        }

        private void AttackTarget()
        {
            if (_attackCooldown > 0f)
            {
                _attackCooldown -= Time.fixedDeltaTime;
                return;
            }
            if (_isTargetOnRange)
            {
                var attackObject = Instantiate(attackProjectilePrefab, transform.position + attackOffset,
                    Quaternion.identity);
                attackObject.GetComponent<EnemyAttackProjectile>().CastAttack(target, stats.damage, attackType);
                _attackCooldown = 60f / stats.attackSpeed;
            }
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
            foreach (var item in inventory.Items)
            {
                var itemObject = Instantiate(itemPrefab, transform.position, quaternion.identity);

                var itemData = itemObject.GetComponent<LootableItem>();
                itemData.itemName = item.Key;
                itemData.count = item.Value;
            }
            
            Destroy(gameObject);
        }
    }
}
