namespace Raft.Scripts
{
    public sealed class PlaneBlueprint : Plane
    {
        public void BuildPlane()
        {
            BuildingManager.BuildPlane(this);
        }
    }
}