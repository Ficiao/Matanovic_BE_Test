using BETest.Enum;
using BETest.Networking.Messages;

namespace BETest.Entities
{
    public class TeslaProjectile : Projectile
    {
        protected override void HandleAuthorityCollision()
        {
            // TODO
        }

        public override void HandleProjectileEnd(ProjectileEndData data)
        {
            transform.position = data.Position;
            _projectileManager.ReleaseProjectile(this);
        }
    }
}