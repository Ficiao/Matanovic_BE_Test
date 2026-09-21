using BETest.Infra.DependacyHandling;
using BETest.Networking.Messages;

namespace BETest.Networking.Transport
{
    public class ServerMessageProcessor : MessageProcessor
    {
        public ServerMessageProcessor(DependencyContainer container) : base(container)
        {
            Subscribe<ConnectRequestMessage>((message, peer) => ConnectRequestMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<PlayerMoveMessage>((message, peer) => PlayerMoveMessageHandler.ProcessMessage(message, peer, _context));
            Subscribe<PlayerShootMessage>((message, peer) => PlayerShootMessageHandler.ProcessMessage(message, peer, _context));
        }
    }
}