using BETest.Networking.Messages;
using LiteNetLib;

namespace BETest.Networking.Transport
{
    public class ServerMessageProcessor : MessageProcessor
    {
        public ServerMessageProcessor()
        {
            RegisterNestedType<ConnectRequestData>();

            Subscribe<ConnectRequestMessage>(ConnectRequestMessageHandler.ProcessMessage);
        }
    }
}