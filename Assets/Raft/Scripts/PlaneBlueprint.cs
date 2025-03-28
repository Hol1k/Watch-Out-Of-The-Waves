using System.Collections.Generic;
using UnityEngine;

namespace Raft.Scripts
{
    public sealed class PlaneBlueprint : Plane
    {
        public void BuildPlane()
        {
            BuildingManager.BuildPlane(this);
        }

        public override void OnDeath()
        {
            BuildingManager.RemovePlaneBlueprintFromList(this);
            
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
                children.Add(child);
            foreach (Transform child in children)
                child.SetParent(null);
            
            Destroy(gameObject);
        }
    }
}