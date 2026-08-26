using BETest.Entities;
using BETest.Enum;
using BETest.Flags;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkPlayerStateManager
    {
        private readonly Dictionary<uint, NetworkEntityStateData> _states = new();
        private readonly Dictionary<uint, ClientPlayerData> _playerDatas = new();
        private readonly Dictionary<uint, EntityUpdateFlags> _dirtyFlags = new();
        private SpawnManager _spawnManager;

        public void Initialize(SpawnManager spawnManager)
        {
            _spawnManager = spawnManager;
        }

        public NetworkEntitySpawnData PlayerConnected(NetPeer peer, ConnectRequestData request)
        {
            uint PID = (uint)peer.Id;
            Vector3 spawnPosition = _spawnManager.PlayerSpawnPosition.position;
            
            NetworkEntityStateData state = new NetworkEntityStateData
            {
                ObjectID = PID,
                StateAuthorityPID = PID,
                EntityType = EntityType.Player,
                X = Mathf.FloatToHalf(spawnPosition.x),
                Y = Mathf.FloatToHalf(spawnPosition.y),
            };

            ClientPlayerData playerData = new ClientPlayerData
            {
                PID = PID,
                PlayerName = request.PlayerName,
                PlayerWeaponType = request.PlayerWeaponType,
                PlayerCharacterType = request.PlayerCharacterType,
            };

            _states.Add(PID, state);
            _playerDatas.Add(PID, playerData);

            ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;

            List<NetworkEntitySpawnData> existingPlayers = new(GetSpawnData());

            NetworkEntitySpawnData newPlayerSpawn = new(prefabType, state, playerData);
            
            ConnectAcceptMessage acceptMessage = new ConnectAcceptMessage()
            {
                PlayerData = playerData
            };

            NetworkServer.SendMessage(acceptMessage, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
            NetworkSpawnService.SendSpawn(peer, existingPlayers);
            NetworkSpawnService.BroadcastSpawnExclude(peer, newPlayerSpawn);

            return newPlayerSpawn;
        }

        public void PlayerDisconnected(uint PID)
        {
            _states.Remove(PID);
            _playerDatas.Remove(PID);
            _dirtyFlags.Remove(PID);
        }

        public bool TryAcceptMove(uint PID, PlayerMoveData moveData)
        {
            if (!_states.TryGetValue(PID, out NetworkEntityStateData state)) return false;
            if (moveData.Seq <= state.SeqAcc) return false;
            if (!ValidateMove(state, moveData)) return false;

            state.X = Mathf.FloatToHalf(moveData.X);
            state.Y = Mathf.FloatToHalf(moveData.Y);
            state.Directions = moveData.Directions;
            state.SeqAcc = moveData.Seq;

            _states[PID] = state;

            EntityUpdateFlags flags = EntityUpdateFlags.Position | EntityUpdateFlags.MoveDir;

            if (_dirtyFlags.TryGetValue(PID, out EntityUpdateFlags dirtyFlags))
                _dirtyFlags[PID] = dirtyFlags | flags;
            else
                _dirtyFlags.Add(PID, flags);

            return true;
        }

        private bool ValidateMove(NetworkEntityStateData previousState, PlayerMoveData moveData)
        {
            // Ovdje bi obično isšla validacija legalnosti promjene pozicije po pravilima igre.
            // Za potrebe ovog demo projekta, state se uvjek smatra validnim.

            return true;
        }

        public void GetDirtyStates(List<NetworkEntityStateData> states)
        {
            foreach ((uint PID, EntityUpdateFlags flags) in _dirtyFlags)
            {
                if (!_states.TryGetValue(PID, out NetworkEntityStateData state)) continue;

                state.UpdateFlags = flags;
                states.Add(state);
            }

            _dirtyFlags.Clear();
        }

        public IEnumerable<NetworkEntitySpawnData> GetSpawnData()
        {
            foreach ((uint PID, NetworkEntityStateData state) in _states)
            {
                ClientPlayerData playerData = _playerDatas[PID];
                ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;

                yield return new NetworkEntitySpawnData(prefabType, state, playerData);
            }
        }
    }
}