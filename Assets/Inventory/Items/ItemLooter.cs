using UnityEngine;

namespace Inventory.Items
{
    public class ItemLooter : MonoBehaviour
    {
        private EntityInventory _inventory;
        
        [SerializeField] private float lootRadius = 4f;

        private void Awake()
        {
            if (!TryGetComponent(out _inventory))
                Debug.LogError("No component of type EntityInventory");
        }

        private void Update()
        {
            TargetLoot();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out LootableItem item))
            {
                _inventory.AddItem(item.itemName, item.count);
                item.GetComponent<SphereCollider>().enabled = false; // Need for fix double loot item
                StopCoroutine(item.MovingCoroutine); //Need for fix using coroutine when item == null
                Destroy(item.gameObject);
            }
        }

        private void TargetLoot()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var hitColliders = Physics.OverlapSphere(transform.position, lootRadius);

            foreach (Collider targetCollider in hitColliders)
            {
                if (targetCollider.TryGetComponent(out LootableItem target) && !target.isMovingToTarget)
                {
                    target.MovingCoroutine = StartCoroutine(target.MoveToTarget(this));
                    target.isMovingToTarget = true;
                }
            }
        }
    }
}