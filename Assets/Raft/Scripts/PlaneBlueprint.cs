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
            Destroy(gameObject);
        }
    }
}