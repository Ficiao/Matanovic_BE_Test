using BETest.Config;
using BETest.Entities;
using BETest.Enum;
using BETest.Flags;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using BETest.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkEnemyStateManager
    {
        private struct EnemySpawnEntry
        {
            public ObjectPrefabType PrefabType;
            public int Weight;

            public EnemySpawnEntry(ObjectPrefabType prefabType, int weight)
            {
                PrefabType = prefabType;
                Weight = weight;
            }
        }

        private readonly List<EnemySpawnEntry> _enemySpawnEntries = new();
        private readonly Dictionary<uint, NetworkEntityStateData> _states = new();
        private readonly Dictionary<uint, ObjectPrefabType> _prefabTypes = new();
        private readonly List<Vector2> _playerPositions = new(4);
        private readonly List<ObjectPrefabType> _enemyPrefabTypes = new();

        private NetworkPlayerStateManager _playerStateManager;
        private SpawnManager _spawnManager;

        private uint _nextEnemyID = 10000;
        private float _spawnTimer;

        public void Initialize(SpawnManager spawnManager, NetworkPlayerStateManager playerStateManager, ObjectPrefabsScriptable objectPrefabs)
        {
            _spawnManager = spawnManager;
            _playerStateManager = playerStateManager;

            foreach (ObjectPrefabsScriptable.PrefabData prefabData in objectPrefabs.GetPrefabs<Enemy>())
            {
                Enemy enemyPrefab = prefabData.Prefab.GetComponent<Enemy>();

                _enemySpawnEntries.Add(new EnemySpawnEntry(prefabData.PrefabType, enemyPrefab.SpawnWeight));
            }
        }

        public void HandleTick()
        {
            if (!_playerStateManager.HasPlayers)
            {
                _spawnTimer = 0f;
                return;
            }

            if (_states.Count >= _spawnManager.MaxAliveEnemies) return;
            if (_spawnManager.EnemySpawnInterval <= 0f) return;

            _spawnTimer += GameConfig.TICK_DELTA;

            if (_spawnTimer < _spawnManager.EnemySpawnInterval) return;

            _spawnTimer -= _spawnManager.EnemySpawnInterval;
            TrySpawnEnemy();
        }

        public void UpdateState(NetworkEntityStateData newState)
        {
            if (!_states.TryGetValue(newState.ObjectID, out NetworkEntityStateData state)) return;

            state.X = newState.X;
            state.Y = newState.Y;
            state.SeqAcc = newState.SeqAcc;

            _states[newState.ObjectID] = state;
        }

        public void GetStates(List<NetworkEntityStateData> states)
        {
            foreach (NetworkEntityStateData stateValue in _states.Values)
            {
                NetworkEntityStateData state = stateValue;
                state.UpdateFlags = EntityUpdateFlags.Position;

                states.Add(state);
            }
        }

        public IEnumerable<NetworkEntitySpawnData> GetSpawnData()
        {
            foreach ((uint ObjectID, NetworkEntityStateData state) in _states)
            {
                yield return new NetworkEntitySpawnData(_prefabTypes[ObjectID], state);
            }
        }

        public void RemoveEnemy(uint ObjectID)
        {
            _states.Remove(ObjectID);
            _prefabTypes.Remove(ObjectID);
        }

        private void TrySpawnEnemy()
        {
            if (!TryGetSpawnPosition(out Vector2 position)) return;

            ObjectPrefabType prefabType = GetRandomEnemyPrefabType();
            uint ObjectID = _nextEnemyID++;

            NetworkEntityStateData state = new()
            {
                ObjectID = ObjectID,
                StateAuthorityPID = 0,
                EntityType = EntityType.Mob,
                X = Mathf.FloatToHalf(position.x),
                Y = Mathf.FloatToHalf(position.y),
            };

            _states.Add(ObjectID, state);
            _prefabTypes.Add(ObjectID, prefabType);

            NetworkEntitySpawnData spawnData = new(prefabType, state);
            NetworkSpawnService.BroadcastSpawn(spawnData);
        }

        private ObjectPrefabType GetRandomEnemyPrefabType()
        {
            int totalWeight = 0;

            foreach (EnemySpawnEntry entry in _enemySpawnEntries)
            {
                totalWeight += entry.Weight;
            }

            int roll = Random.Range(0, totalWeight);

            foreach (EnemySpawnEntry entry in _enemySpawnEntries)
            {
                if (roll < entry.Weight) return entry.PrefabType;
                roll -= entry.Weight;
            }

            return _enemySpawnEntries[0].PrefabType;
        }

        private bool TryGetSpawnPosition(out Vector2 position)
        {
            position = default;

            _playerPositions.Clear();
            _playerStateManager.GetPlayerPositions(_playerPositions);

            if (_playerPositions.Count == 0) return false;

            float minDistanceSqr = _spawnManager.EnemyMinSpawnDistance * _spawnManager.EnemyMinSpawnDistance;

            for (int i = 0; i < _spawnManager.EnemySpawnPositionAttempts; i++)
            {
                Vector2 anchor = _playerPositions[Random.Range(0, _playerPositions.Count)];

                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(_spawnManager.EnemyMinSpawnDistance, _spawnManager.EnemyMaxSpawnDistance);

                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidate = anchor + direction * distance;

                bool tooClose = false;

                foreach (Vector2 playerPosition in _playerPositions)
                {
                    if ((candidate - playerPosition).sqrMagnitude >= minDistanceSqr) continue;

                    tooClose = true;
                    break;
                }

                if (tooClose) continue;

                position = candidate;
                return true;
            }

            return false;
        }
    }
}