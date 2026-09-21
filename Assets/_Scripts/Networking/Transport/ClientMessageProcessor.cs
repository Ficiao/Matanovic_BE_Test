using BETest.Enum;
using BETest.Infra.DependacyHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Runtime.CompilerServices;

namespace BETest.Networking.Transport
{
    public class ClientMessageProcessor : MessageProcessor
    {
        private NetPeer _server;

        public NetPeer ServerPeer { set => _server = value; }

        public ClientMessageProcessor(DependencyContainer container) : base(container)
        {
            Subscribe<ConnectAcceptMessage>((message, peer) => ConnectAcceptMessageHandler.ProcessMessage(message, peer, _container, _context));
            Subscribe<NetworkEntitiesSpawnMessage>((message, peer) => NetworkEntitiesSpawnMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<NetworkEntitiesDespawnMessage>((message, peer) => NetworkEntitiesDespawnMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<NetworkEntityStatesMessage>((message, peer) => NetworkEntityStatesMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<ProjectileSpawnMessage>((message, peer) => ProjectileSpawnMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<ProjectileEndMessage>((message, peer) => ProjectileEndMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<PlayerHealthMessage>((message, peer) => PlayerHealthMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<PlayerScoreMessage>((message, peer) => PlayerScoreMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<PlayerScoreRemovedMessage>((message, peer) => PlayerScoreRemovedMessageHandler.ProcessMessage(message, peer, _context));
        }

        public void SendPacket<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            SendPacket(packet, _server, channel, deliveryMethod);
        }
    }
}