using UnityEngine;

namespace Raft.Scripts
{
    [CreateAssetMenu(fileName = "New TowerStats", menuName = "Towers/TowerStats")]
    public class TowerStats : ScriptableObject
    {
        public GameObject projectilePrefab;
        public TowersTargetPriority priorityTarget;
        public Vector3 shootPositionOffset;
        
        public float baseDamage;
        public float damagePerLevel;
        public float baseAttackRange;
        public float attackRangePerLevel;
        [Tooltip("Attacks per minute")]
        public float baseAttackSpeed;
        [Tooltip("Attacks per minute")]
        public float attackSpeedPerLevel;
        public float baseProjectileSpeed;
        public float projectileSpeedPerLevel;
    }
}