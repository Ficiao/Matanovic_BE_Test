using BETest.Networking.Messages;

namespace BETest.Entities
{
    public class TeslaProjectile : Projectile
    {
        protected override void HandleCollisions()
        {
            int hitCount = GetEnemyHits(_hitRadius);

            for (int i = 0; i < hitCount; i++)
            {
                if (!TryGetEnemy(_enemyHitBuffer[i], out Enemy enemy)) continue;

                enemy.TakeDamage(_damage, OwnerPID);
                EndProjectile();

                return;
            }
        }
        protected override void OnProjectileEnd(ProjectileEndData data)
        {
            _projectileManager.ReleaseProjectile(this);
        }
    }
}