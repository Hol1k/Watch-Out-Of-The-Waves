using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
    public class EnemyStats : ScriptableObject
    {
        public EnemiesTargetPriority targetPriority = EnemiesTargetPriority.ClosestBuilding;
        public int maxHealth = 10;
        public float attackRange = 1.5f;
        public float moveSpeed = 3f;
        public float damage;
        [Tooltip("Attack per minute")]
        public float attackSpeed;
    }
}