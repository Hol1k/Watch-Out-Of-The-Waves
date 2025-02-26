using Enemies;
using Raft.TowerAbilities;
using UnityEngine;

namespace Raft.Scripts
{
    public class Tower : Building
    {
        private Transform _target;
        private float _attackCooldown;
        
        public TowerStats stats;
        public Ability towerAbility;

        private void FixedUpdate()
        {
            ChoseTarget();
            AttackTarget();
        }

        public void UseAbility()
        {
            towerAbility.Activate(this);
        }
        
        private void ChoseTarget()
        {
            switch (stats.priorityTarget)
            {
                case TowersTargetPriority.NearestEnemy:
                    ChoseTargetNearby();
                    break;
            }
        }

        private void AttackTarget()
        {
            if (_attackCooldown > 0 || !_target)
            {
                _attackCooldown -= Time.fixedDeltaTime;
                return;
            }

            Shoot();
            _attackCooldown = 60f / stats.GetCurrentAttackSpeed(level);
        }

        private void Shoot()
        {
            transform.LookAt(new Vector3(_target.position.x, transform.position.y, _target.position.z));
            
            var projectileObject = Instantiate(stats.projectilePrefab,
                transform.position + stats.shootPositionOffset,
                Quaternion.identity);
            projectileObject.transform.LookAt(_target);

            var projectile = projectileObject.GetComponent<TowerProjectile>();
            projectile.Target = _target;
            projectile.Damage = stats.GetCurrentDamage(level);
            projectile.Speed = stats.GetCurrentProjectileSpeed(level);
        }

        private void ChoseTargetNearby()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var hitColliders = Physics.OverlapSphere(transform.position, stats.GetCurrentAttackRange(level));
            
            Vector3 position2D = new Vector3(transform.position.x, 0, transform.position.z);
            Transform nearestEnemy = null;
            float minDistance = float.MaxValue;
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.gameObject.TryGetComponent(out Enemy enemy)) continue;

                float distance = Vector3.Distance(position2D,
                    new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            _target = nearestEnemy;
        }
    }
}