using BETest.Entities;
using BETest.Enum;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkObjectStateManager
    {
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