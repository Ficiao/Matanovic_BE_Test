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
            NetworkEntitiesSpawnMessage message = new()
            {
                SpawnDatas = new NetworkEntitySpawnDatas
                {
                    NetworkEntitySpawns = new List<NetworkEntitySpawnData>(spawnDatas)
                }
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