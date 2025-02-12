using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public sealed class EntityInventory : MonoBehaviour
    {
        private readonly Dictionary<string, int> _items = new Dictionary<string, int>();

        public int GetItemCount(string itemName)
        {
            _items.TryGetValue(itemName, out int num);
            
            return num;
        }

        public void AddItem(string itemName, int count = 1)
        {
            if (_items.ContainsKey(itemName))
                _items[itemName] += count;
            else
                _items.Add(itemName, count);
        }

        public void RemoveItem(string itemName, int count = 1)
        {
            if (_items.ContainsKey(itemName))
            {
                if (count >= _items[itemName])
                    _items.Remove(itemName);
                else
                    _items[itemName] -= count;
            }
        }
    }
}
