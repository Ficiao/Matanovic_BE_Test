using BETest.Enum;
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

        protected override void HandleAuthorityCollision()
        {
            // TODO
        }

        public override void HandleProjectileEnd(ProjectileEndData data)
        {
            transform.position = data.Position;
            _projectileManager.ReleaseProjectile(this);
        }

        public override void ResetForPool()
        {
            _hitEnemyIDs.Clear();
            base.ResetForPool();
        }
    }
}