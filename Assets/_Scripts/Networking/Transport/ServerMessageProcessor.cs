using BETest.Networking.Messages;

namespace BETest.Networking.Transport
{
    public class ServerMessageProcessor : MessageProcessor
    {
        public ServerMessageProcessor()
        {
            Subscribe<ConnectRequestMessage>(ConnectRequestMessageHandler.ProcessMessage);
            Subscribe<ConnectRequestMessage>(ConnectRequestMessageHandler.ProcessMessage);
            Subscribe<PlayerMoveMessage>(PlayerMoveMessageHandler.ProcessMessage);
            Subscribe<PlayerShootMessage>(PlayerShootMessageHandler.ProcessMessage);
        }
    }
}