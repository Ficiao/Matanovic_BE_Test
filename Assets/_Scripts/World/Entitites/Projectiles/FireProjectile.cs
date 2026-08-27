using BETest.Enum;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

        protected override void HandleAuthorityCollision()
        {
            // TODO
        }

        public override void HandleProjectileEnd(ProjectileEndData data)
        {
            transform.position = data.Position;

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
            StopAllCoroutines();

            _damagedEnemyIDs.Clear();
            _projectileEffect.SetActive(true);
            _explosionEffect.SetActive(false);

            base.ResetForPool();
        }
    }
}