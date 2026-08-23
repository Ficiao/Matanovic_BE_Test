using BETest.Enum;
using LiteNetLib;

namespace BETest.Networking.Transport
{
    public class ClientMessageProcessor : MessageProcessor
    {
        private NetPeer _server;

        public NetPeer ServerPeer { set => _server = value; }

        public ClientMessageProcessor()
        {
            //Processor.Subscribe<PlayerSpawnMessage>(
            //    PlayerSpawnMessageHandler.Process
            //);

            //Processor.Subscribe<WorldStateMessage>(
            //    WorldStateMessageHandler.Process
            //);
        }

        public void SendPacket<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            SendPacket(packet, _server, channel, deliveryMethod);
        }
    }
}