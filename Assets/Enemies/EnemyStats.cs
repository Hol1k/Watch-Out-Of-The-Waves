using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
    public class EnemyStats : ScriptableObject
    {
        public TargetPriority targetPriority = TargetPriority.ClosestBuilding;
        public int maxHealth = 10;
        public float attackRange = 1.5f;
        public float moveSpeed = 3f;
    }
}