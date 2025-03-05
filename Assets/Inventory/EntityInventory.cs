using System;
using System.Collections.Generic;
using HUD.Scripts;
using Raft.Scripts;
using UnityEngine;

namespace Inventory
{
    public sealed class EntityInventory : MonoBehaviour
    {
        public readonly Dictionary<string, int> Items = new Dictionary<string, int>();
        
        [Tooltip("If need show this inventory on HUD (only once can be showed!)")]
        [SerializeField] private InventoryShow inventoryShow;
        
        [Space]
        public List<ResourcesCostConfig> startResources;

        private void Start()
        {
            //Give start items
            foreach (var resource in startResources)
            {
                AddItem(resource.resourceName, resource.amount);
            }
        }

        public int GetItemCount(string itemName)
        {
            Items.TryGetValue(itemName, out int num);
            
            return num;
        }

        public void AddItem(string itemName, int count = 1)
        {
            if (Items.ContainsKey(itemName))
                Items[itemName] += count;
            else
                Items.Add(itemName, count);

            if (inventoryShow)
                inventoryShow.UpdateItemList(Items);
        }

        public void RemoveItem(string itemName, int count = 1)
        {
            if (Items.ContainsKey(itemName))
            {
                if (count >= Items[itemName])
                    Items.Remove(itemName);
                else
                    Items[itemName] -= count;

                if (inventoryShow)
                    inventoryShow.UpdateItemList(Items);
            }
        }
    }
}
