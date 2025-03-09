using GeneralScripts;
using Inventory;
using Inventory.Items;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OtherObjects.Scripts
{
    public sealed class LootBox : MonoBehaviour, IDamageable, ISwimable
    {
        private const float WaterLevel = 0f;
        private bool _isInWater;
        [SerializeField] [Min(1)] private float swimToWaterLevelSpeed = 2f;
        
        [Space]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private EntityInventory inventory;
        [SerializeField] private GameObject itemPrefab;

        [Space]
        [SerializeField] [Min(0)]
        private int currentHealth;

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value > maxHealth) currentHealth = maxHealth;
                else if (value <= 0) OnDeath();
                else currentHealth = value;
            }
        }

        public int maxHealth;

        private void Start()
        {
            transform.rotation = Random.rotation;
        }

        private void Update()
        {
            Swim();
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

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            else if (currentHealth == 0) EditorApplication.delayCall += OnDeath;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0) return;
            currentHealth -= (int)damage;
            if (currentHealth <= 0) OnDeath();
        }

        public void OnDeath()
        {
            if (inventory)
            {
                foreach (var item in inventory.Items)
                {
                    var itemObject = Instantiate(itemPrefab, transform.position, quaternion.identity);

                    var itemData = itemObject.GetComponent<LootableItem>();
                    itemData.itemName = item.Key;
                    itemData.count = item.Value;
                }
            }

            Destroy(gameObject);
        }

        public void Swim()
        {
            float y;
            if (_isInWater)
            {
                y = WaterLevel - transform.position.y;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                y = 0;
                rb.useGravity = true;
            }

            transform.Translate(new Vector3(0, y * swimToWaterLevelSpeed * Time.fixedDeltaTime, 0), Space.World);
        }
    }
}
