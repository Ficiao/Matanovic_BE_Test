using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class NetworkEntitiesDespawnMessageHandler
    {
        public static void ProcessMessage(NetworkEntitiesDespawnMessage message, NetPeer peer)
        {
            NetworkObjectManager objectManager = GameSceneContext.Instance.NetworkObjectManager;

            foreach (NetworkEntityDespawnData data in message.DespawnDatas.NetworkEntityDespawns)
            {
                objectManager.DespawnEntity(data.ObjectID, data.EntityType);
            }
        }
    }
}