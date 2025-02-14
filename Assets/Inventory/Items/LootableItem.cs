using System.Collections;
using UnityEngine;

namespace Inventory.Items
{
    public class LootableItem : MonoBehaviour
    {
        private Rigidbody _rb;
        
        private bool _isMovingToTarget;
        private Collider[] _hitColliders;
        [SerializeField] private float lootRadius = 4f;
        [SerializeField] private float lootForce = 5f;
        
        public string itemName;
        [Min(1)] public int count = 1;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            TargetPlayer();
        }

        private IEnumerator MoveToItemLooter(ItemLooter target)
        {
            while (true)
            {
                var direction = (target.transform.position - transform.position).normalized;
                transform.Translate(direction * (Time.deltaTime * lootForce), Space.World);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void TargetPlayer()
        {
            if (!_isMovingToTarget)
            {
                // ReSharper disable once Unity.PreferNonAllocApi
                _hitColliders = Physics.OverlapSphere(transform.position, lootRadius);

                foreach (Collider targetCollider in _hitColliders)
                {
                    if (targetCollider.TryGetComponent(out ItemLooter target))
                    {
                        _rb.useGravity = false;
                        StartCoroutine(MoveToItemLooter(target));
                        _isMovingToTarget = true;
                    }
                }
            }
        }
    }
}
