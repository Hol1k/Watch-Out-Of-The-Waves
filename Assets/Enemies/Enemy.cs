using System;
using System.Collections.Generic;
using GeneralScripts;
using Inventory;
using Inventory.Items;
using Raft.Scripts;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Plane = Raft.Scripts.Plane;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Animator animator;
        
        private bool _isTargetOnRange;

        private Vector3 _visualModelPosition;
        
        [Space]
        [SerializeField] private Transform modelTransform;

        [SerializeField] private float smoothingFactor = 0.05f;

        [Space]
        [SerializeField] [Min(0)] private int currentHealth = 1;

        public int CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (value > stats.maxHealth) currentHealth = stats.maxHealth;
                else if (value <= 0) OnDeath();
                else currentHealth = value;
            }
        }


        public EnemyStats stats;

        [Space]
        [SerializeField] private EntityInventory inventory;
        [SerializeField] private GameObject itemPrefab;

        protected Transform Target;
        
        [NonSerialized] public BuildingsManager BuildingsManager = null;

        private void Awake()
        {
            _visualModelPosition = modelTransform.position;
            
            currentHealth = stats.maxHealth;
        }

        private void FixedUpdate()
        {
            ChoseTarget();
        }

        private void LateUpdate()
        {
            SmoothModel();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (currentHealth > stats.maxHealth) currentHealth = stats.maxHealth;
            else if (currentHealth == 0) OnDeath();
        }

        protected void ChoseTarget()
        {
            if (!Target.IsDestroyed())
                Target = null;
            
            if (!Target)
            {
                switch (stats.priorityTarget)
                {
                    case EnemiesTargetPriority.NearestPlane:
                        ChoseTargetNearbyPlane();
                        break;
                }
            }
        }

        private void ChoseTargetNearbyPlane()
        {
            var planes = new List<Plane>();
            
            planes.AddRange(BuildingsManager.Planes);
            
            Vector3 selfPosition2D = new Vector3(transform.position.x, 0, transform.position.z);
            Transform nearestTower = null;
            float minDistance = float.MaxValue;
            foreach (var plane in planes)
            {
                if (!plane)
                    continue;
                
                float distance = Vector3.Distance(selfPosition2D,
                    new Vector3(plane.transform.position.x, 0, plane.transform.position.z));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTower = plane.transform;
                }
            }

            Target = nearestTower;
        }

        protected void SmoothModel()
        {
            _visualModelPosition = Vector3.Lerp(_visualModelPosition, transform.position, 1 / smoothingFactor * Time.deltaTime);
            modelTransform.position = _visualModelPosition;
        }

        public virtual void TakeDamage(float damage)
        {
            if (damage <= 0) return;
            currentHealth -= (int)damage;
            if (currentHealth <= 0) OnDeath();
        }

        public void OnDeath()
        {
            if (inventory)
            {
                foreach (var item in inventory.Items)
                {
                    var itemObject = Instantiate(itemPrefab, transform.position, quaternion.identity);

                    var itemData = itemObject.GetComponent<LootableItem>();
                    itemData.itemName = item.Key;
                    itemData.count = item.Value;
                }
            }
            
            Destroy(gameObject);
        }
    }
}
