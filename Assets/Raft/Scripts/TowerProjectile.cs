using System;
using Enemies;
using GeneralScripts;
using UnityEngine;

namespace Raft.Scripts
{
    public class TowerProjectile : MonoBehaviour
    {
        [NonSerialized] public Transform Target;
        [NonSerialized] public float Damage;
        [NonSerialized] public float Speed = 10f;

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable damageable) || damageable is Building) return;
                
            damageable.TakeDamage(Damage);
            Destroy(gameObject);
        }

        private void MoveToTarget()
        {
            if (!Target)
            {
                Destroy(gameObject);
                return;
            }
            
            transform.LookAt(Target);

            Vector3 direction = (Target.position - transform.position).normalized;

            transform.position += direction * (Speed * Time.fixedDeltaTime);
        }
    }
}