using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class ConnectAcceptMessageHandler
    {
        public static void ProcessMessage(ConnectAcceptMessage message, NetPeer peer)
        {
            DependencyContainer container = DependencyContainer.Instance;
            GameSceneContext sceneContext = GameSceneContext.Instance;

            sceneContext.TerrainGenerator.Initialize(message.WorldSeed, sceneContext.PlayerManager);
            container.LocalPlayerSession.SetLocalPID(message.PlayerData.PID);
            sceneContext.PlayerManager.SetLocalPID(message.PlayerData.PID);
        }
    }
}