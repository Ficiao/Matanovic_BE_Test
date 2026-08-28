using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;

namespace BETest.Networking.Services
{
    public static class NetworkScoreService
    {
        public static void BroadcastScore(PlayerScoreData data)
        {
            PlayerScoreMessage message = new() { Data = data };
            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void SendScore(NetPeer peer, PlayerScoreData data)
        {
            PlayerScoreMessage message = new() { Data = data };
            NetworkServer.SendMessage(message, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static void BroadcastScoreRemoved(uint PID)
        {
            PlayerScoreRemovedMessage message = new() { PID = PID };
            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }
    }
}