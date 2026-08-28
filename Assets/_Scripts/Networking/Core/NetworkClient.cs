using BETest.Config;
using BETest.Entities;
using BETest.Enum;
using BETest.Infra.DependacyHandling;
using BETest.Networking.Messages;
using BETest.Networking.Transport;
using LiteNetLib;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BETest.Networking.ConnectionHandling
{
    public class NetworkClient : MonoBehaviour, INetEventListener
    {
        private string _connectionKey = ConnectionConfig.CONNECTION_KEY;
        private NetManager _client;
        private NetPeer _server;
        private static ClientMessageProcessor _messageProcessor;
        private LocalPlayerSession _localPlayerSession;

        public NetPeer ServerPeer => _server;
        public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
        public long BytesReceived => _client?.Statistics.BytesReceived ?? 0;
        public long BytesSent => _client?.Statistics.BytesSent ?? 0;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<NetPacketReader, byte, DeliveryMethod> OnPacketReceived;

        private void Start()
        {
            _localPlayerSession = DependencyContainer.Instance.LocalPlayerSession;
        }

        public void Connect(string address, int port)
        {
            Disconnect();

            _messageProcessor = new();

            _client = new NetManager(this)
            {
                AutoRecycle = true,
                ChannelsCount = 2,
                EnableStatistics = true,
            };

            _client.Start();
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
            OnConnected?.Invoke();

            ConnectRequestMessage connectMessage = new()
            {
                Data = new()
                {
                    PlayerName = _localPlayerSession.Username,
                    PlayerWeaponType = _localPlayerSession.WeaponType,
                    PlayerCharacterType = _localPlayerSession.CharacterType
                }
            };
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
            OnDisconnected?.Invoke();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _messageProcessor.HandleAllPacketsForPeer(reader, peer);
        }

        public static void SendMessage<T>(T packet, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
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

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}