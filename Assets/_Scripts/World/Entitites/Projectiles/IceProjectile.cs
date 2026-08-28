using BETest.Networking.Messages;
using System.Collections.Generic;

namespace BETest.Entities
{
    public class IceProjectile : Projectile
    {
        private readonly HashSet<uint> _hitEnemyIDs = new();

        protected override void OnInitialized()
        {
            _hitEnemyIDs.Clear();
        }

        protected override void HandleCollisions()
        {
            int hitCount = GetEnemyHits(_hitRadius);

            for (int i = 0; i < hitCount; i++)
            {
                if (!TryGetEnemy(_enemyHitBuffer[i], out Enemy enemy)) continue;
                if (!_hitEnemyIDs.Add(enemy.ObjectID)) continue;

                enemy.TakeDamage(_damage, OwnerPID);
            }
        }

        protected override void OnProjectileEnd(ProjectileEndData data)
        {
            _projectileManager.ReleaseProjectile(this);
        }

        public override void ResetForPool()
        {
            _hitEnemyIDs.Clear();
            base.ResetForPool();
        }
    }
}