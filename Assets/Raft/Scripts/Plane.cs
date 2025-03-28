using System.Collections.Generic;
using UnityEngine;

namespace Raft.Scripts
{
    public class Plane : Building
    {
        public int xCoord;
        public int yCoord;

        public override void TakeDamage(float damage)
        {
            if (damage <= 0) return;
            currentHealth -= (int)damage;
            if (currentHealth <= 0) OnDeath();
        }

        public override void OnDeath()
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
                children.Add(child);
            foreach (Transform child in children)
                child.SetParent(null);
            
            BuildingManager.DestroyPlane(this);
        }
    }
}
