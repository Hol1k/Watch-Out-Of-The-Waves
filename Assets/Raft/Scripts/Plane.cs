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
            BuildingManager.DestroyPlane(this);
        }
    }
}
