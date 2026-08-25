using BETest.Config;
using BETest.Enum;
using BETest.Networking.Messages;
using BETest.Scriptables;
using BETest.World.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Entities
{
    public class LocalEntityManager : MonoBehaviour
    {
        private const float OBJETC_Z_POSITION = GameConfig.OBJECT_Z_POSITION;

        [SerializeField] private LocalPlayer _localPlayerPrefab;
        [SerializeField] private RemotePlayer _remotePlayerPrefab;
        private readonly Dictionary<(uint, EntityType), NetworkEntity> _entities = new();
        private uint _localPID;
        private ObjectPrefabsScriptable _objectPrefabs;

        public LocalPlayer LocalPlayer { get; private set; }

        public void Initialize(ObjectPrefabsScriptable objectPrefabs)
        {
            _objectPrefabs = objectPrefabs;
        }

        public void SetLocalPID(uint localPID)
        {
            _localPID = localPID;
        }

        public void SpawnEntity(NetworkEntitySpawnData data)
        {
            EntityType entityType = data.StateData.EntityType;
            uint PID = data.ClientPlayerData.PID;
            if (_entities.ContainsKey((PID, entityType))) return;

            NetworkEntity entity;
            if (entityType == EntityType.Player)
            {
                entity = PID == _localPID ? SpawnLocalPlayer(data) : SpawnRemotePlayer(data);
                _entities.Add((PID, entityType), entity);
            }
            else
            {

            }
        }

        private LocalPlayer SpawnLocalPlayer(NetworkEntitySpawnData data)
        {
            Vector3 position = new Vector3(data.StateData.X, data.StateData.Y, OBJETC_Z_POSITION);

            CharacterModelController characterModelController = _objectPrefabs.GetPrefab(data.PrefabType).GetComponent<CharacterModelController>();
            LocalPlayer = Instantiate(_localPlayerPrefab, position, Quaternion.identity);
            LocalPlayer.Init(data.ClientPlayerData.PID, characterModelController);
            return LocalPlayer;
        }

        private RemotePlayer SpawnRemotePlayer(NetworkEntitySpawnData data)
        {
            Vector3 position = new Vector3(data.StateData.X, data.StateData.Y, OBJETC_Z_POSITION);

            CharacterModelController characterModelController = _objectPrefabs.GetPrefab(data.PrefabType).GetComponent<CharacterModelController>();
            RemotePlayer player = Instantiate(_remotePlayerPrefab, position, Quaternion.identity);
            player.Init(data.ClientPlayerData.PID, characterModelController);
            return player;
        }

        public void DespawnEntity(uint objectID, EntityType entityType)
        {
            if (!_entities.Remove((objectID, entityType), out NetworkEntity entity)) return;

            if (entity == LocalPlayer) LocalPlayer = null;

            Destroy(entity.gameObject);
        }

        public void HandleEntityState(NetworkEntityStateData state)
        {
            if (!_entities.TryGetValue((state.ObjectID, state.EntityType), out NetworkEntity entity)) return;

            entity.HandleServerStateUpdate(state);
        }

        public void HandleTick()
        {
            foreach (NetworkEntity entity in _entities.Values) entity.HandleTick();
        }
    }
}