using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerScoreMessageHandler
    {
        public static void ProcessMessage(PlayerScoreMessage message, NetPeer peer)
        {
            GameSceneContext.Instance.NetworkObjectManager.HandlePlayerScore(message.Data);
        }
    }
}