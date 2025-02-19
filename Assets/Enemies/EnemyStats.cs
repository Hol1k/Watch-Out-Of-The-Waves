using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
    public class EnemyStats : ScriptableObject
    {
        public EnemiesTargetPriority enemiesTargetPriority = EnemiesTargetPriority.ClosestBuilding;
        public int maxHealth = 10;
        public float attackRange = 1.5f;
        public float moveSpeed = 3f;
    }
}