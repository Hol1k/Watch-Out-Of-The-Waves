namespace Raft.Scripts
{
    public class PlaneBlueprint : Plane
    {
        public void BuildPlane()
        {
            _buildingManager.BuildPlane(this);
        }
    }
}