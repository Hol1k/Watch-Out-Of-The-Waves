namespace GeneralScripts
{
    public interface IDamageable
    {
        public void TakeDamage(int damage);
        
        public void OnDeath();
    }
}