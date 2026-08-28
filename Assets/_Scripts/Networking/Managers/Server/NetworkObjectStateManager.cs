using BETest.Entities;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using BETest.Scriptables;
using LiteNetLib;
using System.Collections.Generic;

namespace BETest.Networking.Managers
{
    public class NetworkObjectStateManager
    {
        private uint _nextProjectileID = 1000;

        private NetworkPlayerStateManager _playerStateManager;
        private NetworkEnemyStateManager _enemyStateManager;

        public void Initialize(SpawnManager spawnManager, ObjectPrefabsScriptable objectPrefabs, int worldSeed)
        {
            _playerStateManager = new();
            _playerStateManager.Initialize(spawnManager, worldSeed);

            _enemyStateManager = new();
            _enemyStateManager.Initialize(spawnManager, _playerStateManager, objectPrefabs);
        }

        public void HandleTick()
        {
            _playerStateManager.HandleTick();
            _enemyStateManager.HandleTick();
        }

        public NetworkEntitySpawnData PlayerConnected(NetPeer peer, ConnectRequestData request)
        {
            NetworkEntitySpawnData playerSpawn = _playerStateManager.PlayerConnected(peer, request);

            List<NetworkEntitySpawnData> enemySpawns = new(_enemyStateManager.GetSpawnData());
            if (enemySpawns.Count > 0) NetworkSpawnService.SendSpawn(peer, enemySpawns);

            return playerSpawn;
        }

        public void PlayerDisconnected(uint PID)
        {
            _playerStateManager.PlayerDisconnected(PID);
        }

        public bool TryAcceptPlayerMove(uint PID, PlayerMoveData moveData)
        {
            return _playerStateManager.TryAcceptMove(PID, moveData);
        }

        public bool DamagePlayer(uint PID, int damage)
        {
            return _playerStateManager.DamagePlayer(PID, damage);
        }

        public void PlayerShoot(uint PID, PlayerShootData shootData)
        {
            if (!_playerStateManager.TryGetPlayerData(PID, out ClientPlayerData playerData)) return;

            uint ObjectID = _nextProjectileID++;
            _nextProjectileID %= 10000;
            if (_nextProjectileID < 1000) _nextProjectileID += 1000;

            ProjectileSpawnData spawnData = new(ObjectID, PID, playerData.PlayerWeaponType, shootData.SourcePosition, shootData.Direction.normalized);

            NetworkSpawnService.BroadcastProjectileSpawn(spawnData);
        }

        public void UpdateEnemyState(NetworkEntityStateData state)
        {
            _enemyStateManager.UpdateState(state);
        }

        public void RemoveEnemy(uint ObjectID)
        {
            _enemyStateManager.RemoveEnemy(ObjectID);
        }

        public void RegisterKill(uint PID)
        {
            _playerStateManager.AddKill(PID);
        }

        public void GetDirtyStates(List<NetworkEntityStateData> states)
        {
            _playerStateManager.GetDirtyStates(states);
            _enemyStateManager.GetStates(states);
        }

        public IEnumerable<NetworkEntitySpawnData> GetExistingPlayerSpawns()
        {
            return _playerStateManager.GetSpawnData();
        }
    }
}