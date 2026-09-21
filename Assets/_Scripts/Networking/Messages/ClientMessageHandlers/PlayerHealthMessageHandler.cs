using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerHealthMessageHandler
    {
        public static void ProcessMessage(PlayerHealthMessage message, NetPeer peer, GameSceneContext context)
        {
            if (context == null) return;

            context.NetworkObjectManager.HandlePlayerHealth(message.Data);
        }
    }
}