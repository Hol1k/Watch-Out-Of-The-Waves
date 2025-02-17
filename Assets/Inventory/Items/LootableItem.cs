using System.Collections;
using UnityEngine;

namespace Inventory.Items
{
    public class LootableItem : MonoBehaviour
    {
        private Rigidbody _rb;
        
        private Collider[] _hitColliders;
        [SerializeField] private float lootForce = 5f;
        
        public bool isMovingToTarget;
        public Coroutine MovingCoroutine;
        
        public string itemName;
        [Min(1)] public int count = 1;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public IEnumerator MoveToTarget(ItemLooter target)
        {
            _rb.useGravity = false;
            while (true)
            {
                var direction = (target.transform.position - transform.position).normalized;
                transform.Translate(direction * (Time.deltaTime * lootForce), Space.World);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
