using BETest.Config;
using BETest.Enum;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using UnityEngine;

namespace BETest.Entities
{
    public abstract class Projectile : NetworkObject
    {
        [SerializeField] protected float _movementSpeed = 10f;
        [SerializeField] protected float _maxDistance = 20f;
        [SerializeField] protected float _hitRadius = 0.25f;
        [SerializeField] protected LayerMask _enemyMask;

        protected readonly Collider[] _enemyHitBuffer = new Collider[32];
        protected ProjectileManager _projectileManager;
        protected Vector3 _direction;
        protected int _damage;
        private float _travelledDistance;
        private bool _ended;

        public uint OwnerPID { get; private set; }
        public WeaponType WeaponType { get; private set; }

        public virtual void Init(ProjectileSpawnData data, bool hasStateAuthority, int damage, ProjectileManager projectileManager)
        {
            base.Init(data.ObjectID, EntityType.Projectile, hasStateAuthority);

            _projectileManager = projectileManager;
            OwnerPID = data.OwnerPID;
            WeaponType = data.WeaponType;
            _direction = data.Direction.normalized;
            _damage = damage;

            transform.position = data.SourcePosition;
            transform.right = _direction;

            _travelledDistance = 0f;
            _ended = false;

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        public override void HandleTick()
        {
            if (_ended) return;

            Vector3 movement = _direction * _movementSpeed * GameConfig.TICK_DELTA;

            transform.position += movement;
            _travelledDistance += movement.magnitude;

            if (!HasStateAuthority) return;

            HandleCollisions();

            if (!_ended && _travelledDistance >= _maxDistance) EndProjectile();
        }

        protected int GetEnemyHits(float radius)
        {
            return Physics.OverlapSphereNonAlloc(transform.position, radius, _enemyHitBuffer, _enemyMask, QueryTriggerInteraction.Collide);
        }

        protected bool TryGetEnemy(Collider collider, out Enemy enemy)
        {
            enemy = collider.GetComponentInParent<Enemy>();
            return enemy != null && !enemy.IsDead;
        }

        protected abstract void HandleCollisions();

        protected void EndProjectile()
        {
            if (!HasStateAuthority || _ended) return;

            _ended = true;
            OnResolve();
            ProjectileEndData data = new(ObjectID, transform.position);
            NetworkSpawnService.BroadcastProjectileEnd(data);
        }

        protected virtual void OnResolve() { }

        public void HandleProjectileEnd(ProjectileEndData data)
        {
            _ended = true;
            transform.position = data.Position;

            OnProjectileEnd(data);
        }

        protected abstract void OnProjectileEnd(ProjectileEndData data);

        public virtual void ResetForPool()
        {
            _projectileManager = null;
            _direction = Vector3.zero;
            OwnerPID = 0;
            _damage = 0;
            WeaponType = WeaponType.None;

            _travelledDistance = 0f;
            _ended = false;
        }
    }
}