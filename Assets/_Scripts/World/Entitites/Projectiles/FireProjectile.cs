using BETest.Networking.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Entities
{
    public class FireProjectile : Projectile
    {
        [SerializeField] private float _explosionRadius = 2f;
        [SerializeField] private float _explosionDuration = 0.5f;
        [SerializeField] private GameObject _projectileEffect;
        [SerializeField] private GameObject _explosionEffect;

        private readonly HashSet<uint> _damagedEnemyIDs = new();

        protected override void OnInitialized()
        {
            _damagedEnemyIDs.Clear();
            _projectileEffect.SetActive(true);
            _explosionEffect.SetActive(false);
        }

        protected override void HandleCollisions()
        {
            if (GetEnemyHits(_hitRadius) > 0) EndProjectile();
        }

        protected override void OnResolve()
        {
            _damagedEnemyIDs.Clear();

            int hitCount = GetEnemyHits(_explosionRadius);

            for (int i = 0; i < hitCount; i++)
            {
                if (!TryGetEnemy(_enemyHitBuffer[i], out Enemy enemy)) continue;
                if (!_damagedEnemyIDs.Add(enemy.ObjectID)) continue;

                enemy.TakeDamage(_damage, OwnerPID);
            }
        }

        protected override void OnProjectileEnd(ProjectileEndData data)
        {
            _projectileEffect.SetActive(false);
            _explosionEffect.SetActive(true);

            StartCoroutine(FinishExplosion());
        }

        private IEnumerator FinishExplosion()
        {
            yield return new WaitForSeconds(_explosionDuration);
            _projectileManager.ReleaseProjectile(this);
        }

        public override void ResetForPool()
        {
            _damagedEnemyIDs.Clear();
            _projectileEffect.SetActive(false);
            _explosionEffect.SetActive(false);

            base.ResetForPool();
        }
    }
}