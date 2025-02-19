using UnityEngine;
using UnityEngine.Serialization;

namespace Raft.Scripts
{
    [CreateAssetMenu(fileName = "New TowerStats", menuName = "Towers/TowerStats")]
    public class TowerStats : ScriptableObject
    {
        public int damage;
        public TowersTargetPriority priorityTarget;
        public float attackRange;
        [Tooltip("Attacks per minute")]
        public int attackSpeed;

        public GameObject projectilePrefab;
        public Vector3 shootPositionOffset;
        public float projectileSpeed;
    }
}