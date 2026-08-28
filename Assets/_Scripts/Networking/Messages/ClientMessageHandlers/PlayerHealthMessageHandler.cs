using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerHealthMessageHandler
    {
        public static void ProcessMessage(PlayerHealthMessage message, NetPeer peer)
        {
            GameSceneContext.Instance.NetworkObjectManager.HandlePlayerHealth(message.Data);
        }
    }
}