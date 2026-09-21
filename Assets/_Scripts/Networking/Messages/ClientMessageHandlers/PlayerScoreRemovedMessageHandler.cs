using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerScoreRemovedMessageHandler
    {
        public static void ProcessMessage(PlayerScoreRemovedMessage message, NetPeer peer, GameSceneContext context)
        {
            if (context == null) return;

            context.NetworkObjectManager.HandlePlayerScoreRemoved(message.PID);
        }
    }
}