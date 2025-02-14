using UnityEngine;

namespace Inventory.Items
{
    public class ItemLooter : MonoBehaviour
    {
        private EntityInventory _inventory;

        private void Awake()
        {
            if (!TryGetComponent(out _inventory))
                Debug.LogError("No component of type EntityInventory");
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out LootableItem item))
            {
                _inventory.AddItem(item.itemName, item.count);
                Destroy(item.gameObject);
            }
        }
    }
}