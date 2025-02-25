using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HUD.Scripts
{
    public class InventoryShow : MonoBehaviour
    {
        private List<RectTransform> _itemsList = new();

        [SerializeField] private TextMeshProUGUI itemPrefab;

        public void UpdateItemList(Dictionary<string, int> inventory)
        {
            foreach (var itemTransform in _itemsList)
            {
                Destroy(itemTransform.gameObject);
            }
            
            _itemsList.Clear();

            int i = 0;
            foreach (var item in inventory)
            {
                var itemRectTransform = Instantiate(itemPrefab.transform as RectTransform, transform);
                itemRectTransform.gameObject.name = $"{item.Key} count";
                
                itemRectTransform.GetComponent<TextMeshProUGUI>().text = $"{item.Key}: {item.Value}";
                itemRectTransform.position =
                    new Vector2(
                        itemRectTransform.position.x,
                        itemRectTransform.position.y - itemRectTransform.sizeDelta.y * i++);

                _itemsList.Add(itemRectTransform.GetComponent<RectTransform>());
            }
        }
    }
}
