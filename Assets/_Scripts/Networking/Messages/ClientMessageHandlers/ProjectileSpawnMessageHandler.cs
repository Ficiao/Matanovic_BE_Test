using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class ProjectileSpawnMessageHandler
    {
        public static void ProcessMessage(ProjectileSpawnMessage message, NetPeer peer, GameSceneContext context)
        {
            if (context == null) return;

            NetworkObjectManager objectManager = context.NetworkObjectManager;
            objectManager.SpawnEntity(message.Data);
        }
    }
}