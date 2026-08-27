using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class ProjectileEndMessageHandler
    {
        public static void ProcessMessage(ProjectileEndMessage message, NetPeer peer)
        {
            NetworkObjectManager objectManager = GameSceneContext.Instance.NetworkObjectManager;
            objectManager.HandleProjectileEnd(message.Data);
        }
    }
}