using BETest.Entities;
using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class NetworkEntitiesSpawnMessageHandler
    {
        public static void ProcessMessage(NetworkEntitiesSpawnMessage message, NetPeer peer)
        {
            if(GameSceneContext.Instance == null)
            {
                CustomLogger.Warning($"disconnecting_peer", new() { ["id"] = peer?.Id, ["reason"] = "game_scene_not_ready, unprocessed_spawns" });
                peer.Disconnect();
                return;
            }

            NetworkObjectManager objectManager = GameSceneContext.Instance.NetworkObjectManager;

            foreach(NetworkEntitySpawnData spawnData in message.SpawnDatas.NetworkEntitySpawns)
            {
                objectManager.SpawnEntity(spawnData);
            }
        }
    }
}