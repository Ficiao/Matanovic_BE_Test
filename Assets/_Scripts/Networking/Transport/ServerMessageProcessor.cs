using BETest.Entities;
using BETest.Infra.DependacyHandling;
using BETest.Networking.Messages;

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