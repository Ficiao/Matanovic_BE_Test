using BETest.Enum;
using BETest.Networking.Messages;
using LiteNetLib;

namespace BETest.Networking.Transport
{
    public class ClientMessageProcessor : MessageProcessor
    {
        private NetPeer _server;

        public NetPeer ServerPeer { set => _server = value; }

        public ClientMessageProcessor()
        {
            Subscribe<ConnectAcceptMessage>(ConnectAcceptMessageHandler.ProcessMessage);
        }

        public void SendPacket<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            SendPacket(packet, _server, channel, deliveryMethod);
        }
    }
}