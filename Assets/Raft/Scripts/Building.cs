using System.Collections.Generic;
using GeneralScripts;
using UnityEditor;
using UnityEngine;

namespace Raft.Scripts
{
    public class Building : MonoBehaviour, IDamageable
    {
        protected BuildingsManager BuildingManager;

        public List<ResourcesCostConfig> resources = new();
        public BuildingType buildingType;
        
        [SerializeField] [Min(0)] protected int currentHealth;

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
            else if (currentHealth == 0) EditorApplication.delayCall += OnDeath;
        }

        public virtual void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0) OnDeath();
        }

        public virtual void OnDeath()
        {
            BuildingManager.RemoveBuildingFromList(this);
            Destroy(gameObject);
        }

        public void SetBuildingManager(BuildingsManager buildingManager)
            => BuildingManager = buildingManager;
    }
}