using BETest.Networking.Messages;
using LiteNetLib;

namespace BETest.Networking.Transport
{
    public class ServerMessageProcessor : MessageProcessor
    {
        public ServerMessageProcessor()
        {
            Subscribe<ConnectRequestMessage>(ConnectRequestMessageHandler.ProcessMessage);
        }
    }
}