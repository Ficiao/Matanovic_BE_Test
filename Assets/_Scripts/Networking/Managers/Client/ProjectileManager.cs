using BETest.Audio;
using BETest.Entities;
using BETest.Enum;
using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using static BETest.Scriptables.WeaponDataScriptable;

namespace BETest.Networking.Managers
{
    public class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private Transform _poolContainer;

        private readonly Dictionary<WeaponType, ObjectPool<Projectile>> _pools = new();
        private readonly Dictionary<uint, Projectile> _projectiles = new();
        private readonly List<Projectile> _activeProjectiles = new();
        private GameAudioManager _audioManager;

        private WeaponDataScriptable _weaponData;
        private bool _hasProjectileStateAuthority;

        public void Initialize(WeaponDataScriptable weaponData, bool hasProjectileStateAuthority, GameAudioManager audioManager)
        {
            _weaponData = weaponData;
            _hasProjectileStateAuthority = hasProjectileStateAuthority;
            _audioManager = audioManager;

            foreach (WeaponData weapon in weaponData.Weapons)
            {
                _pools.Add(weapon.WeaponType, new ObjectPool<Projectile>(weapon.WeaponPrefab, _poolContainer));
            }
        }

        public void SpawnProjectile(ProjectileSpawnData data)
        {
            ObjectPool<Projectile> pool = _pools[data.WeaponType];

            WeaponData weaponData = _weaponData.GetWeaponData(data.WeaponType);

            Projectile projectile = pool.Get();
            projectile.Init(data, _hasProjectileStateAuthority, weaponData.Damage, this);
            projectile.gameObject.SetActive(true);

            _projectiles.Add(data.ObjectID, projectile);
            _activeProjectiles.Add(projectile);
            _audioManager.PlayShoot();
        }

        public void HandleTick()
        {
            for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
            {
                _activeProjectiles[i].HandleTick();
            }
        }

        public void HandleProjectileEnd(ProjectileEndData data)
        {
            _projectiles[data.ObjectID].HandleProjectileEnd(data);
        }

        public void ReleaseProjectile(Projectile projectile)
        {
            uint ObjectID = projectile.ObjectID;
            WeaponType weaponType = projectile.WeaponType;

            _projectiles.Remove(ObjectID);
            _activeProjectiles.Remove(projectile);

            projectile.ResetForPool();
            _pools[weaponType].Release(projectile);
        }
    }
}