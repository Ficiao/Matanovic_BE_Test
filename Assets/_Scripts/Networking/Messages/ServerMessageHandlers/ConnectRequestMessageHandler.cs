using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public static class ConnectRequestMessageHandler
    {
        private const int MAX_ATTEMPTS = 5;
        private const float WINDOW = 60f;
        private static Dictionary<string, (int count, float resetTime)> _ipAttempts = new();

        public static void ProcessMessage(ConnectRequestMessage message, NetPeer peer)
        {
            if (!AllowConnection(peer.Address.ToString(), Time.unscaledTime))
            {
                CustomLogger.Warning($"disconnecting_peer", new() { ["id"] = peer?.Id, ["reason"] = "too_many_connection_attempts" });
                return;
            }


            ConnectAcceptMessage acceptMessage = new()
            {
                PlayerData = new ClientPlayerData
                {
                    PlayerName = message.Data.PlayerName,
                    PID = (uint)peer.Id,
                },
                TickIndex = NetworkServer.Instance.CurrentTick,
            };

            NetworkServer.SendMessage(acceptMessage, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }

        public static bool AllowConnection(string ip, float now)
        {
            if (!_ipAttempts.TryGetValue(ip, out (int count, float resetTime) entry))
            {
                _ipAttempts[ip] = (1, now + WINDOW);
                return true;
            }

            if (now > entry.resetTime)
            {
                _ipAttempts[ip] = (1, now + WINDOW);
                return true;
            }

            if (entry.count >= MAX_ATTEMPTS) return false;

            _ipAttempts[ip] = (entry.count + 1, entry.resetTime);
            return true;
        }
    }
}