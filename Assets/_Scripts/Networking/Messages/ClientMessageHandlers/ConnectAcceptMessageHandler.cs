using BETest.Infra.DependacyHandling;
using LiteNetLib;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public static class ConnectAcceptMessageHandler
    {
        public static void ProcessMessage(ConnectAcceptMessage message, NetPeer peer, DependencyContainer container, GameSceneContext context)
        {
            if (context == null) return;

            context.TerrainGenerator.Initialize(message.WorldSeed, context.PlayerManager);

            container.LocalPlayerSession.SetLocalPID(message.PlayerData.PID);
            context.PlayerManager.SetLocalPID(message.PlayerData.PID);
        }
    }
}