using BETest.Entities;
using BETest.Enum;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkObjectStateManager
    {
        private uint _nextProjectileID = 1000;
        private NetworkPlayerStateManager _playerStateManager;

        public void Initialize(SpawnManager spawnManager)
        {
            _playerStateManager = new();
            _playerStateManager.Initialize(spawnManager);
        }

        public NetworkEntitySpawnData PlayerConnected(NetPeer peer, ConnectRequestData request)
        {
            return _playerStateManager.PlayerConnected(peer, request);
        }

        public void PlayerDisconnected(uint pid)
        {
            _playerStateManager.PlayerDisconnected(pid);
        }

        public bool TryAcceptPlayerMove(uint PID, PlayerMoveData moveData)
        {
            return _playerStateManager.TryAcceptMove(PID, moveData);
        }

        public void PlayerShoot(uint PID, PlayerShootData shootData)
        {
            if (!_playerStateManager.TryGetPlayerData(PID, out ClientPlayerData playerData)) return;

            uint ObjectID = _nextProjectileID++;
            _nextProjectileID %= 10000;
            if(_nextProjectileID < 1000) _nextProjectileID += 1000;

            ProjectileSpawnData spawnData = new(ObjectID, PID, playerData.PlayerWeaponType, shootData.SourcePosition, shootData.Direction.normalized);

            NetworkSpawnService.BroadcastProjectileSpawn(spawnData);
        }

        public void GetDirtyStates(List<NetworkEntityStateData> states)
        {
            _playerStateManager.GetDirtyStates(states);
        }

        public IEnumerable<NetworkEntitySpawnData> GetExistingPlayerSpawns()
        {
            return _playerStateManager.GetSpawnData();
        }
    }
}