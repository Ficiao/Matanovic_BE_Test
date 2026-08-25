using BETest.Enum;
using BETest.Flags;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Entities
{
    public class NetworkPlayerStateManager
    {
        private readonly Dictionary<uint, NetworkEntityStateData> _playerStates = new();
        private readonly Dictionary<uint, ClientPlayerData> _playerDatas = new();
        private readonly ConnectAcceptMessage _reusableConnectAcceptMessage = new();
        private readonly NetworkEntitiesSpawnMessage _reusableSpawnMessage = new();
        private readonly NetworkEntitiesDespawnMessage _reusableDespawnMessage = new();

        public IReadOnlyDictionary<uint, NetworkEntityStateData> PlayerStates => _playerStates;

        public void PlayerConnected(NetPeer peer, ConnectRequestData data, short tickIndex, Vector3 startPosition)
        {
            uint PID = (uint)peer.Id;

            NetworkEntityStateData stateData = new NetworkEntityStateData
            {
                ObjectID = PID,
                EntityType = EntityType.Player,
                UpdateFlags = EntityUpdateFlags.All,
                SeqAcc = 0,
                X = (ushort)startPosition.x,
                Y = (ushort)startPosition.y,
                Directions = MoveDirFlags.None,
            };

            ClientPlayerData playerData = new ClientPlayerData
            {
                PlayerName = data.PlayerName,
                PID = PID,
                PlayerWeaponType = data.PlayerWeaponType,
                PlayerCharacterType = data.PlayerCharacterType,
            };
            
            _playerDatas[PID] = playerData;
            _playerStates[PID] = stateData;

            SendConnectAccept(peer, playerData, tickIndex);
            SendExistingPlayers(peer);
            BroadcastNewPlayer(peer, playerData, stateData);          
        }

        private void SendConnectAccept(NetPeer peer, ClientPlayerData playerData, short tickIndex)
        {
            _reusableConnectAcceptMessage.PlayerData = playerData;
            _reusableConnectAcceptMessage.TickIndex = tickIndex;

            NetworkServer.SendMessage(_reusableConnectAcceptMessage, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        private void SendExistingPlayers(NetPeer peer)
        {
            List<NetworkEntitySpawnData> spawns = new(_playerStates.Count);

            foreach ((uint PID, NetworkEntityStateData state) in _playerStates)
            {
                ClientPlayerData playerData = _playerDatas[PID];
                ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;
                spawns.Add(new NetworkEntitySpawnData(prefabType, state, _playerDatas[PID]));
            }

            _reusableSpawnMessage.SpawnDatas = new NetworkEntitySpawnDatas
            {
                NetworkEntitySpawns = spawns
            };

            NetworkServer.SendMessage(_reusableSpawnMessage, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        private void BroadcastNewPlayer(NetPeer newPeer, ClientPlayerData playerData, NetworkEntityStateData stateData)
        {
            ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;
            _reusableSpawnMessage.SpawnDatas = new NetworkEntitySpawnDatas
            {
                NetworkEntitySpawns = new List<NetworkEntitySpawnData>
                {
                    new(prefabType, stateData, playerData)
                }
            };

            NetworkServer.SendMessageToAll(_reusableSpawnMessage, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }


        public void PlayerDisconnected(uint PID)
        {
            _playerStates.Remove(PID);
            _playerDatas.Remove(PID);
        }

        public bool TryUpdateState(uint PID, NetworkEntityStateData incomingState, out NetworkEntityStateData acceptedState)
        {
            acceptedState = default;

            if (!_playerStates.TryGetValue(PID, out NetworkEntityStateData previousState)) return false;

            incomingState.ObjectID = PID;

            if (!ValidateState(previousState, incomingState)) return false;

            _playerStates[PID] = incomingState;
            acceptedState = incomingState;

            return true;
        }

        public bool TryGetState(uint PID, out NetworkEntityStateData state)
        {
            return _playerStates.TryGetValue(PID, out state);
        }

        private bool ValidateState(NetworkEntityStateData previousState, NetworkEntityStateData incomingState)
        {
            //Ovdje bi obicno išla validacija nove pozcije, tj. je li ta promjena legalna po pravilima igre. 
            //Za potrebe demonstracije, ova metoda samo vra?a true i state klijenta se uvijek prihva?a.
            return true;
        }

    }
}