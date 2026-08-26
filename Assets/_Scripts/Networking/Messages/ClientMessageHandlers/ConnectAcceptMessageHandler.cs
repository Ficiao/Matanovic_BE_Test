using BETest.Infra.DependacyHandling;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class ConnectAcceptMessageHandler
    {
        public static void ProcessMessage(ConnectAcceptMessage message, NetPeer peer)
        {
            DependencyContainer.Instance.LocalPlayerSession.SetLocalPID(message.PlayerData.PID);
            GameSceneContext.Instance.PlayerManager.SetLocalPID(message.PlayerData.PID);
        }
    }
}