using BETest.Config;
using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;

namespace BETest.Networking.Services
{
    public static class NetworkSpawnService
    {
        public static void SendSpawn(NetPeer peer, IEnumerable<NetworkEntitySpawnData> spawnDatas)
        {
            const int packetReserveBytes = 64;

            int maxBatchSize = ConnectionConfig.MAX_PACKET_BYTES - packetReserveBytes;
            int batchSize = sizeof(ushort);

            List<NetworkEntitySpawnData> batch = new();

            foreach (NetworkEntitySpawnData spawnData in spawnDatas)
            {
                int spawnSize = NetworkEntitySpawnData.Size(spawnData.StateData.EntityType);

                if (batch.Count > 0 && batchSize + spawnSize > maxBatchSize)
                {
                    SendSpawnBatch(peer, batch);
                    batch.Clear();
                    batchSize = sizeof(ushort);
                }

                batch.Add(spawnData);
                batchSize += spawnSize;
            }

            if (batch.Count > 0) SendSpawnBatch(peer, batch);
        }

        private static void SendSpawnBatch(NetPeer peer, List<NetworkEntitySpawnData> spawnDatas)
        {
            NetworkEntitiesSpawnMessage message = new()
            {
                SpawnDatas = new NetworkEntitySpawnDatas
                {
                    NetworkEntitySpawns = spawnDatas,
                },
            };

            NetworkServer.SendMessage(message, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastSpawn(NetworkEntitySpawnData spawnData)
        {
            NetworkEntitiesSpawnMessage message = new()
            {
                SpawnDatas = new NetworkEntitySpawnDatas
                {
                    NetworkEntitySpawns = new List<NetworkEntitySpawnData> { spawnData }
                }
            };

            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastSpawnExclude(NetPeer peer, NetworkEntitySpawnData spawnData)
        {
            NetworkEntitiesSpawnMessage message = new()
            {
                SpawnDatas = new NetworkEntitySpawnDatas
                {
                    NetworkEntitySpawns = new List<NetworkEntitySpawnData> { spawnData }
                }
            };

            NetworkServer.SendMessageToAllExcept(message, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastDespawn(NetworkEntityDespawnData data)
        {
            NetworkEntitiesDespawnMessage message = new()
            {
                DespawnDatas = new NetworkEntityDespawnDatas
                {
                    NetworkEntityDespawns = new List<NetworkEntityDespawnData> { data },
                }
            };

            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastProjectileSpawn(ProjectileSpawnData data)
        {
            ProjectileSpawnMessage message = new()
            {
                Data = data,
            };

            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastProjectileEnd(ProjectileEndData data)
        {
            ProjectileEndMessage message = new()
            {
                Data = data,
            };

            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }
    }
}