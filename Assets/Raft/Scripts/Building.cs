using GeneralScripts;
using UnityEngine;

namespace Raft.Scripts
{
    public class Building : MonoBehaviour, IDamageable
    {
        protected RaftBuildingsManager BuildingManager;
        
        public BuildingType buildingType;
        
        [SerializeField] [Min(0)] private int currentHealth;

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value > maxHealth) currentHealth = maxHealth;
                else if (value <= 0) OnDeath();
                else currentHealth = value;
            }
        }
        public int maxHealth;

        protected void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            else if (currentHealth == 0) OnDeath();
        }

        public virtual void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0) OnDeath();
        }

        public void OnDeath()
        {
            Destroy(gameObject);
        }
        
        public void SetBuildingManager(RaftBuildingsManager buildingManager)
            => BuildingManager = buildingManager;
    }
}