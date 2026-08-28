using BETest.Audio;
using BETest.Entities;
using BETest.Enum;
using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using BETest.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Transform _poolContainer;

        private readonly Dictionary<ObjectPrefabType, ObjectPool<Enemy>> _pools = new();
        private readonly Dictionary<uint, Enemy> _enemies = new();
        private readonly List<Enemy> _activeEnemies = new();
        private NetworkObjectStateManager _objectStateManager;
        private PlayerManager _playerManager;
        private GameAudioManager _audioManager;
        private bool _hasStateAuthority;

        public void Initialize(ObjectPrefabsScriptable objectPrefabs, PlayerManager playerManager, bool hasStateAuthority, NetworkObjectStateManager objectStateManager, GameAudioManager audioManager)
        {
            _playerManager = playerManager;
            _hasStateAuthority = hasStateAuthority;
            _objectStateManager = objectStateManager;
            _audioManager = audioManager;

            foreach (ObjectPrefabsScriptable.PrefabData prefabData in objectPrefabs.GetPrefabs<Enemy>())
            {
                Enemy enemyPrefab = prefabData.Prefab.GetComponent<Enemy>();

                _pools.Add(
                    prefabData.PrefabType,
                    new ObjectPool<Enemy>(enemyPrefab, _poolContainer)
                );
            }
        }

        public Enemy SpawnEnemy(NetworkEntitySpawnData data)
        {
            uint ObjectID = data.StateData.ObjectID;

            if (_enemies.TryGetValue(ObjectID, out Enemy existingEnemy)) return existingEnemy;
            
            Enemy enemy = _pools[data.PrefabType].Get();
            enemy.Init(data, _hasStateAuthority, _playerManager, this);
            enemy.gameObject.SetActive(true);

            _enemies.Add(ObjectID, enemy);
            _activeEnemies.Add(enemy);

            return enemy;
        }

        public void DespawnEnemy(uint ObjectID)
        {
            if (!_enemies.Remove(ObjectID, out Enemy enemy)) return;

            _activeEnemies.Remove(enemy);

            ObjectPrefabType prefabType = enemy.PrefabType;
            enemy.ResetForPool();
            _audioManager.PlayEnemyDeath();

            if (_pools.TryGetValue(prefabType, out ObjectPool<Enemy> pool))
                pool.Release(enemy);
        }

        public bool TryDamagePlayer(uint PID, int damage)
        {
            if (!_hasStateAuthority) return false;

            return _objectStateManager.DamagePlayer(PID, damage);
        }

        public void KillEnemy(Enemy enemy, uint KillerPID)
        {
            if (!_hasStateAuthority) return;

            _objectStateManager.RegisterKill(KillerPID);
            _objectStateManager.RemoveEnemy(enemy.ObjectID);

            NetworkSpawnService.BroadcastDespawn(
                new NetworkEntityDespawnData(enemy.ObjectID, EntityType.Mob)
            );
        }

        public void KillEnemy(Enemy enemy)
        {
            if (!_hasStateAuthority) return;

            _objectStateManager.RemoveEnemy(enemy.ObjectID);

            NetworkSpawnService.BroadcastDespawn(
                new NetworkEntityDespawnData(enemy.ObjectID, EntityType.Mob)
            );
        }

        public void HandleState(NetworkEntityStateData state)
        {
            if (!_enemies.TryGetValue(state.ObjectID, out Enemy enemy)) return;

            enemy.HandleServerStateUpdate(state);
        }

        public IEnumerable<Enemy> GetStateAuthorityEnemies()
        {
            if (!_hasStateAuthority) yield break;

            foreach (Enemy enemy in _activeEnemies)
            {
                if (enemy != null) yield return enemy;
            }
        }
    }
}