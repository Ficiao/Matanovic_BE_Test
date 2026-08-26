using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class NetworkEntityStatesMessageHandler
    {
        public static void ProcessMessage(NetworkEntityStatesMessage message, NetPeer peer)
        {
            NetworkObjectManager objectManager = GameSceneContext.Instance.NetworkObjectManager;

            foreach (NetworkEntityStateData state in message.Data.NetworkEntityStates)
            {
                objectManager.HandleEntityState(state);
            }
        }
    }
}