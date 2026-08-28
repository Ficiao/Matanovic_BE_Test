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
        private EnemyManager _enemyManager;
        private NetworkObjectStateManager _objectStateManager;

        public void Initialize(PlayerManager playerManager, ProjectileManager projectileManager, EnemyManager enemyManager, NetworkObjectStateManager objectStateManager = null)
        {
            _playerManager = playerManager;
            _projectileManager = projectileManager;
            _enemyManager = enemyManager;
            _objectStateManager = objectStateManager;
        }

        public void SpawnEntity(NetworkEntitySpawnData data)
        {
            switch (data.StateData.EntityType)
            {
                case EntityType.Player:
                    _playerManager.SpawnPlayer(data);
                    break;

                case EntityType.Mob:
                    _enemyManager.SpawnEnemy(data);
                    break;
            }
        }

        public void SpawnEntity(ProjectileSpawnData data)
        {
            _projectileManager.SpawnProjectile(data);
        }

        public void HandlePlayerHealth(PlayerHealthData data)
        {
            _playerManager.HandleHealth(data);
        }

        public void HandleProjectileEnd(ProjectileEndData data)
        {
            _projectileManager.HandleProjectileEnd(data);
        }

        public void DespawnEntity(uint ObjectID, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    _playerManager.DespawnPlayer(ObjectID);
                    break;

                case EntityType.Mob:
                    _enemyManager.DespawnEnemy(ObjectID);
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

                case EntityType.Mob:
                    SendEnemyState(entity);
                    break;
            }
        }

        private void SendPlayerMove(Player player)
        {
            NetworkEntityStateData state = player.GetEntityStateForBroadcast();

            PlayerMoveMessage message = new()
            {
                Data = new PlayerMoveData(state),
            };

            NetworkClient.SendMessage(message, TransmissionChannel.StateUpdate, DeliveryMethod.Unreliable);
        }

        private void SendEnemyState(NetworkEntity enemy)
        {
            if (_objectStateManager == null) return;

            NetworkEntityStateData state = enemy.GetEntityStateForBroadcast();
            _objectStateManager.UpdateEnemyState(state);
        }

        public void HandleEntityState(NetworkEntityStateData state)
        {
            switch (state.EntityType)
            {
                case EntityType.Player:
                    _playerManager.HandleState(state);
                    break;

                case EntityType.Mob:
                    _enemyManager.HandleState(state);
                    break;
            }
        }

        public void HandlePlayerScore(PlayerScoreData data)
        {
            _playerManager.HandleScore(data);
        }

        public void HandlePlayerScoreRemoved(uint PID)
        {
            _playerManager.HandleScoreRemoved(PID);
        }

        public IEnumerable<NetworkEntity> GetStateAuthorityEntities()
        {
            if (_playerManager.LocalPlayer != null) yield return _playerManager.LocalPlayer;

            foreach (Enemy enemy in _enemyManager.GetStateAuthorityEnemies())
            {
                yield return enemy;
            }
        }
    }
}