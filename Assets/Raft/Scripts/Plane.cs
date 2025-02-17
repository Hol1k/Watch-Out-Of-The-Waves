using UnityEngine;

namespace Raft.Scripts
{
    public class Plane : Building
    {
        public int xCoord;
        public int yCoord;

        public override void OnDeath()
        {
            BuildingManager.DestroyPlane(this);
        }
    }
}
