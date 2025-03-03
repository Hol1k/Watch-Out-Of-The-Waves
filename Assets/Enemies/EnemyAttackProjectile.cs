using System;
using Raft.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class EnemyAttackProjectile : MonoBehaviour
    {
        private Transform _target;
        private AttackType _projectileType;
        private float _damage;
        
        [Tooltip("If it's projectile for range attack")] public float projectileSpeed = 10f;

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_projectileType == AttackType.Range)
            {
                if (!other.gameObject.TryGetComponent(out Building building)) return;

                building.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
        
        public void CastAttack(Transform target, float damage, AttackType attackType)
        {
            _target = target;
            _damage = damage;
            _projectileType = attackType;
            if (attackType == AttackType.Melee)
            {
                target.GetComponent<Building>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        private void MoveToTarget()
        {
            if (_projectileType == AttackType.Range)
            {
                if (!_target)
                {
                    Destroy(gameObject);
                    return;
                }

                transform.LookAt(_target);

                Vector3 direction = (_target.position - transform.position).normalized;

                transform.position += direction * (projectileSpeed * Time.fixedDeltaTime);
            }
        }
    }
}