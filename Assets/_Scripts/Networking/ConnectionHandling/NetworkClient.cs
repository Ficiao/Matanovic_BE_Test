using BETest.Config;
using BETest.Enum;
using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Networking.Transport;
using LiteNetLib;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BETest.Networking.ConnectionHandling
{
    public class NetworkClient : SingletonPersistent<NetworkClient>, INetEventListener
    {
        private string _connectionKey = ConnectionConfig.CONNECTION_KEY;
        private NetManager _client;
        private NetPeer _server;
        private ClientMessageProcessor _messageProcessor;
        private string _playerName; 

        public NetPeer ServerPeer => _server;
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;

        public static event Action Connected;
        public static event Action Disconnected;
        public static event Action<NetPacketReader, byte, DeliveryMethod> PacketReceived;

        public void Connect(string address)
        {
            Disconnect();

            _messageProcessor = new();

            _client = new NetManager(this)
            {
                AutoRecycle = true
            };

            _client.Start();

            int port = ConnectionConfig.GAME_PORT;
            CustomLogger.Info("client_connecting", new()
            {
                ["addr"] = address,
                ["port"] = port,
            });

            _client.Connect(address, port, _connectionKey);
        }

        public void Disconnect()
        {
            _client?.Stop();
            _client = null;
            _server = null;
        }

        private void Update()
        {
            _client?.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _server = peer;
            _messageProcessor.ServerPeer = peer;
            CustomLogger.Info("client_connected", new()
            {
                ["serverPeerId"] = (uint)peer.Id,
                ["remote"] = peer.Address?.ToString()
            });
            Connected?.Invoke();

            ConnectRequestMessage connectMessage = new() { Data = new() { PlayerName = _playerName} };
            SendMessage(connectMessage, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }        

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            CustomLogger.Info("client_disconnected", new()
            {
                ["serverPeerId"] = (uint)peer.Id,
                ["reason"] = disconnectInfo.Reason.ToString()
            });
            _server = null;
            Disconnected?.Invoke();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _messageProcessor.HandleAllPacketsForPeer(reader, peer);
        }

        public void SendMessage<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            _messageProcessor.SendPacket(packet, channel, deliveryMethod);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            CustomLogger.Error("client_network_error", null, new()
            {
                ["endpoint"] = endPoint?.ToString(),
                ["socket_error"] = socketError.ToString()
            });
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) 
        { 
            //CustomLogger.Debug("latency_update", new(){ ["ms"] = latency });
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) 
        {
            CustomLogger.Debug("client_unconnected_message", new()
            {
                ["remote"] = remoteEndPoint?.ToString(),
                ["type"] = messageType.ToString()
            });
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            CustomLogger.Debug("client_got_connection_request", new()
            {
                ["remote"] = request.RemoteEndPoint?.ToString()
            });
            request.Reject();
        }

        protected override void OnApplicationQuit()
        {
            Disconnect();
            base.OnApplicationQuit();
        }
    }
}