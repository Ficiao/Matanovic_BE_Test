using BETest.Entities;
using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkObjectManager : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private ProjectileManager _projectileManager;

        public void Initialize(PlayerManager playerManager, ProjectileManager projectileManager)
        {
            _playerManager = playerManager;
            _projectileManager = projectileManager;
        }

        public void SpawnEntity(NetworkEntitySpawnData data)
        {
            switch (data.StateData.EntityType)
            {
                case EntityType.Player:
                    _playerManager.SpawnPlayer(data);
                    break;

                case EntityType.Mob:
                    break;
            }
        }

        public void SpawnEntity(ProjectileSpawnData data)
        {
            _projectileManager.SpawnProjectile(data);
        }

        public void HandleProjectileEnd(ProjectileEndData data)
        {
            _projectileManager.HandleProjectileEnd(data);
        }

        public void DespawnEntity(uint objectID, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    _playerManager.DespawnPlayer(objectID);
                    break;
            }
        }

        public void HandleTick()
        {
            foreach (NetworkEntity entity in GetStateAuthorityEntities())
            {
                entity.HandleTick();
                SendStateAuthorityUpdate(entity);
            }

            _projectileManager.HandleTick();
        }

        private void SendStateAuthorityUpdate(NetworkEntity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Player:
                    SendPlayerMove((Player)entity);
                    break;
            }
        }

        private void SendPlayerMove(Player player)
        {
            NetworkEntityStateData lastState = player.GetEntityStateForBroadcast();          

            PlayerMoveMessage message = new()
            {
                Data = new PlayerMoveData(lastState),
            };

            NetworkClient.SendMessage(message, TransmissionChannel.StateUpdate, DeliveryMethod.Unreliable);
        }

        public void HandleEntityState(NetworkEntityStateData state)
        {
            switch (state.EntityType)
            {
                case EntityType.Player:
                    _playerManager.HandleState(state);
                    break;
            }
        }

        public IEnumerable<NetworkEntity> GetStateAuthorityEntities()
        {
            if (_playerManager.LocalPlayer != null) yield return _playerManager.LocalPlayer;
        }
    }
}