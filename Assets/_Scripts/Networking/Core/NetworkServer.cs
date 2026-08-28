using BETest.Config;
using BETest.Enum;
using BETest.Networking.Transport;
using LiteNetLib;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BETest.Networking.ConnectionHandling
{
    public class NetworkServer : MonoBehaviour, INetEventListener
    {
        private string _connectionKey = ConnectionConfig.CONNECTION_KEY;
        private static NetManager _server;
        private static ServerMessageProcessor _messageProcessor;
        private short _currentTick;

        public short CurrentTick => _currentTick;
        public static bool IsRunning => _server?.IsRunning ?? false;
        public long BytesReceived => _server?.Statistics.BytesReceived ?? 0;
        public long BytesSent => _server?.Statistics.BytesSent ?? 0;

        public event Action<NetPeer> OnClientConnected;
        public event Action<NetPeer> OnClientDisconnected;

        public bool StartServer(int port)
        {
            if (IsRunning) return true;

            _messageProcessor = new();

            _server = new NetManager(this)
            {
                AutoRecycle = true,
                ChannelsCount = 2,
                EnableStatistics = true,
            };

            if (!_server.Start(port))
            {
                _server.Stop(false);
                _server = null;
                _messageProcessor = null;
                return false;
            }

            CustomLogger.Info("server_started", new()
            {
                ["port"] = port
            });

            return true;
        }

        public void StopServer()
        {
            if (_server == null) return;

            _server.Stop();
            _server = null;
            _messageProcessor = null;

            CustomLogger.Info("server_stopped");
        }

        private void Update()
        {
            _server?.PollEvents();
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            CustomLogger.Info("incoming_connection", new() { ["remote"] = request.RemoteEndPoint?.ToString() });
            request.AcceptIfKey(_connectionKey);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            CustomLogger.Info("peer_connected", new()
            {
                ["pid"] = (uint)peer.Id,
                ["remote"] = peer.Address?.ToString()
            });
            OnClientConnected?.Invoke(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            CustomLogger.Info("peer_disconnected", new()
            {
                ["pid"] = (uint)peer.Id,
                ["reason"] = disconnectInfo.Reason.ToString()
            });
            OnClientDisconnected?.Invoke(peer);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _messageProcessor.HandleAllPacketsForPeer(reader, peer);
        }

        public static void SendMessage<T>(T packet, NetPeer peer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if (!IsRunning) return;

            _messageProcessor.SendPacket(packet, peer, channel, deliveryMethod);
        }

        public static void SendMessageToAll<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if (!IsRunning) return;

            _messageProcessor.SendPacketToAll(packet, _server, channel, deliveryMethod);
        }

        public static void SendMessageToAllExcept<T>(T packet, NetPeer excludedPeer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if (!IsRunning) return;

            _messageProcessor.SendPacketToAllExcept(packet, _server, excludedPeer, channel, deliveryMethod);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            CustomLogger.Error("network_error", null, new()
            {
                ["endpoint"] = endPoint?.ToString(),
                ["socket_error"] = socketError.ToString()
            });
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) 
        {
            CustomLogger.Debug("unconnected_message", new()
            {
                ["remote"] = remoteEndPoint?.ToString(),
                ["type"] = messageType.ToString()
            });
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }
    }
}