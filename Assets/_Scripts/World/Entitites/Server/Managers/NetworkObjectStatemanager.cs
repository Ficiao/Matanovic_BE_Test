using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Entities
{
    public class NetworkObjectStateManager : MonoBehaviour
    {
        private NetworkServer _server;
        private SpawnManager _spawnManager;
        private readonly NetworkPlayerStateManager _playerStateManager = new();

        public IReadOnlyDictionary<uint, NetworkEntityStateData> PlayerStates => _playerStateManager.PlayerStates;

        public void Initialize(SpawnManager spawnManager, NetworkServer server)
        {
            _spawnManager = spawnManager;
            _server = server;
        }

        public void PlayerConnected(NetPeer peer, ConnectRequestData data)
        {
            _playerStateManager.PlayerConnected(peer, data, _server.CurrentTick, _spawnManager.PlayerSpawnPosition.position);
        }

        public void PlayerDisconnected(uint PID)
        {
            _playerStateManager.PlayerDisconnected(PID);
        }

        public bool HandlePlayerState(uint PID, NetworkEntityStateData state, out NetworkEntityStateData acceptedState)
        {
            return _playerStateManager.TryUpdateState(PID, state, out acceptedState);
        }

        public bool TryGetPlayerState(uint PID, out NetworkEntityStateData state)
        {
            return _playerStateManager.TryGetState(PID, out state);
        }
    }
}