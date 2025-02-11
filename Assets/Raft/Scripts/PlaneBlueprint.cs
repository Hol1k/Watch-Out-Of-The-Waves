namespace Raft.Scripts
{
    public class PlaneBlueprint : Plane
    {
        public void BuildPlane(RaftBuildingsManager manager)
        {
            manager.BuildPlane(xCord, yCord);
            Destroy(gameObject);
        }
    }
}