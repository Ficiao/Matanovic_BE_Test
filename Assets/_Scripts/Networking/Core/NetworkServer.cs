using BETest.Config;
using BETest.Enum;
using BETest.Networking.Managers;
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
        private int _gamePort = ConnectionConfig.GAME_PORT;
        private string _connectionKey = ConnectionConfig.CONNECTION_KEY;
        private static NetManager _server;
        private static ServerMessageProcessor _messageProcessor;
        private short _currentTick;

        public short CurrentTick => _currentTick;

        public bool IsRunning => _server?.IsRunning ?? false;

        public event Action<NetPeer> ClientConnected;
        public event Action<NetPeer> ClientDisconnected;

        public bool StartServer()
        {
            if (IsRunning) return true;

            _messageProcessor = new();

            _server = new NetManager(this)
            {
                AutoRecycle = true
            };

            CustomLogger.Info("server_starting");

            if (!_server.Start(_gamePort))
            {
                CustomLogger.Error("server_start_failed", null, new()
                {
                    ["port"] = _gamePort
                });
                return false;
            }

            CustomLogger.Info("server_started", new()
            {
                ["port"] = _gamePort
            });

            return true;
        }

        public void StopServer()
        {
            _server?.Stop();
            _server = null;
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
            ClientConnected?.Invoke(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            CustomLogger.Info("peer_disconnected", new()
            {
                ["pid"] = (uint)peer.Id,
                ["reason"] = disconnectInfo.Reason.ToString()
            });
            ClientDisconnected?.Invoke(peer);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _messageProcessor.HandleAllPacketsForPeer(reader, peer);
        }

        public static void SendMessage<T>(T packet, NetPeer peer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            _messageProcessor.SendPacket(packet, peer, channel, deliveryMethod);
        }

        public static void SendMessageToAll<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            _messageProcessor.SendPacketToAll(packet, _server, channel, deliveryMethod);
        }

        public static void SendMessageToAllExcept<T>(T packet, NetPeer excludedPeer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
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