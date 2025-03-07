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
        
        public float damageBonus;
        public float attackRangeBonus;
        public float attackSpeedBonus;
        public float projectileSpeedBonus;
        
        [SerializeField] private Transform barrel;

        private void FixedUpdate()
        {
            ChoseTarget();
            AttackTarget();
        }

        private void LateUpdate()
        {
            LookAtTarget();
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

        private void ChoseTargetNearby()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, GetCurrentAttackRange(true));
            
            Vector3 selfPosition2D = new Vector3(transform.position.x, 0, transform.position.z);
            Transform nearestEnemy = null;
            float minDistance = float.MaxValue;
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.gameObject.TryGetComponent(out Enemy enemy)) continue;

                float distance = Vector3.Distance(selfPosition2D,
                    new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            _target = nearestEnemy;
        }

        private void AttackTarget()
        {
            if (_attackCooldown > 0 || !_target)
            {
                _attackCooldown -= Time.fixedDeltaTime;
                return;
            }

            Shoot();
            _attackCooldown = 60f / GetCurrentAttackSpeed(true);
        }

        private void LookAtTarget()
        {
            if (_target)
            {
                barrel.LookAt(_target);
                barrel.Rotate(Vector3.right, -90);
            }
        }

        private void Shoot()
        {
            var projectileObject = Instantiate(stats.projectilePrefab,
                transform.position + stats.shootPositionOffset,
                Quaternion.identity);
            projectileObject.transform.LookAt(_target);

            var projectile = projectileObject.GetComponent<TowerProjectile>();
            projectile.Target = _target;
            projectile.Damage = GetCurrentDamage(true);
            projectile.Speed = GetCurrentProjectileSpeed(true);
        }

        public float GetCurrentDamage(bool withBonus)
        {
            if (!withBonus)
                return stats.baseDamage + stats.damagePerLevel * level;
            return stats.baseDamage + stats.damagePerLevel * level + damageBonus;
        }

        public float GetCurrentAttackRange(bool withBonus)
        {
            if (!withBonus)
                return stats.baseAttackRange + stats.attackRangePerLevel * level;
            return stats.baseAttackRange + stats.attackRangePerLevel * level + attackRangeBonus;
        }

        public float GetCurrentAttackSpeed(bool withBonus)
        {
            if (!withBonus)
                return stats.baseAttackSpeed + stats.attackSpeedPerLevel * level;
            return stats.baseAttackSpeed + stats.attackSpeedPerLevel * level + attackSpeedBonus;
        }

        public float GetCurrentProjectileSpeed(bool withBonus)
        {
            if (!withBonus)
                return stats.baseProjectileSpeed + stats.projectileSpeedPerLevel * level;
            return stats.baseProjectileSpeed + stats.projectileSpeedPerLevel * level + projectileSpeedBonus;
        }
    }
}