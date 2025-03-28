using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
    public class EnemyStats : ScriptableObject
    {
        public EnemiesTargetPriority priorityTarget = EnemiesTargetPriority.NearestPlane;
        
        [Space]
        public int maxHealth = 20;
        public float attackRange = 1.5f;
        public float moveSpeed = 3f;
        public float damage = 5f;
        [Tooltip("Attack per minute")]
        public float attackSpeed = 60f;
    }
}