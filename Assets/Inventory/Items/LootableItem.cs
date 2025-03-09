using System;
using System.Collections;
using UnityEngine;

namespace Inventory.Items
{
    public class LootableItem : MonoBehaviour, ISwimable
    {
        private Rigidbody _rb;
        
        private const float WaterLevel = 0f;
        private bool _isInWater;
        [SerializeField] [Min(1)] private float swimToWaterLevelSpeed = 4f;

        private Collider[] _hitColliders;
        [SerializeField] private float lootForce = 5f;

        [NonSerialized] public bool IsMovingToTarget;
        public Coroutine MovingCoroutine;

        public string itemName;
        [Min(1)] public int count = 1;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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

        private void FixedUpdate()
        {
            Swim();
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

        public void Swim()
        {
            if (!IsMovingToTarget)
            {
                float y;
                if (_isInWater)
                {
                    y = WaterLevel - transform.position.y;
                    _rb.useGravity = false;
                    _rb.linearVelocity = Vector3.zero;
                }
                else
                {
                    y = 0;
                    _rb.useGravity = true;
                }

                transform.Translate(new Vector3(0, y * swimToWaterLevelSpeed * Time.fixedDeltaTime, 0));
            }
        }
    }
}
