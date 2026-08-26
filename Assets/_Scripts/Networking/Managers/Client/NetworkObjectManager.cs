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

        public void Initialize(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void SpawnEntity(NetworkEntitySpawnData data)
        {
            switch (data.StateData.EntityType)
            {
                case EntityType.Player:
                    _playerManager.SpawnPlayer(data);
                    break;

                    // case EntityType.NPC:
                    //     _npcManager.SpawnNPC(data);
                    //     break;

                    // case EntityType.Projectile:
                    //     _projectileManager.SpawnProjectile(data);
                    //     break;
            }
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
            Vector3 position = player.transform.position;

            PlayerMoveMessage message = new()
            {
                Data = new PlayerMoveData
                {
                    Seq = player.EntityState.SeqAcc,
                    X = position.x,
                    Y = position.y,
                    Directions = player.EntityState.Directions
                }
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