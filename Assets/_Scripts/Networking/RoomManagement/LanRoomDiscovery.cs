using BETest.Config;
using BETest.Networking.Messages;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BETest.Networking.RoomManagement
{
    public class LanRoomDiscovery : MonoBehaviour, INetEventListener
    {
        private RoomManager _roomManager;
        private string _protocol = ConnectionConfig.PROTOCOL;
        private NetManager _netManager;

        public event Action<RoomInfo> OnRoomDiscovered;

        public void Initialize(RoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        private void Awake()
        {
            _netManager = new NetManager(this)
            {
                UnconnectedMessagesEnabled = true,
                BroadcastReceiveEnabled = true
            };
        }

        public bool StartAdvertising(int gamePort)
        {
            Stop();

            int discoveryPort = gamePort + ConnectionConfig.DISCOVERY_PORT_OFFSET;

            if (!_netManager.Start(discoveryPort))
            {
                CustomLogger.Error("discovery_start_failed", null, new()
                {
                    ["port"] = discoveryPort
                });

                return false;
            }

            CustomLogger.Info("discovery_started", new()
            {
                ["port"] = discoveryPort
            });

            return true;
        }

        public void StopAdvertising()
        {
            Stop();
        }

        public void Stop()
        {
            if (!_netManager.IsRunning) return;

            _netManager.Stop(false);
        }

        public void StartBrowsing()
        {
            if (_netManager.IsRunning) return;

            _netManager.BroadcastReceiveEnabled = true;
            _netManager.Start();
        }

        public void Search()
        {
            NetDataWriter writer = new();
            writer.Put(_protocol);

            for (int gamePort = ConnectionConfig.GAME_PORT; gamePort <= ConnectionConfig.MAX_GAME_PORT; gamePort++)
            {
                int discoveryPort = gamePort + ConnectionConfig.DISCOVERY_PORT_OFFSET;
                _netManager.SendBroadcast(writer, discoveryPort);
            }
        }

        private void Update()
        {
            _netManager?.PollEvents();
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            switch (messageType)
            {
                case UnconnectedMessageType.Broadcast:
                    HandleDiscoveryRequest(remoteEndPoint, reader);
                    break;

                case UnconnectedMessageType.BasicMessage:
                    HandleDiscoveryResponse(remoteEndPoint, reader);
                    break;
            }
        }

        private void HandleDiscoveryRequest(IPEndPoint remote, NetPacketReader reader)
        {
            RoomInfo _advertisedRoom = _roomManager.CurrentRoom;
            RoomStateType roomState = _roomManager.State;
            if(roomState != RoomStateType.InRoomHost) return;
            if (_advertisedRoom == null) return;
            if (reader.GetString() != _protocol) return;

            NetDataWriter writer = new NetDataWriter();

            writer.Put(_protocol);
            writer.Put(_advertisedRoom.Id);
            writer.Put(_advertisedRoom.Name);
            writer.Put(_advertisedRoom.PlayerCount);
            writer.Put(_advertisedRoom.MaxPlayers);
            writer.Put(_advertisedRoom.GamePort);

            _netManager.SendUnconnectedMessage(writer, remote);
        }

        private void HandleDiscoveryResponse(IPEndPoint remote, NetPacketReader reader)
        {
            if (reader.GetString() != _protocol) return;

            RoomInfo room = new RoomInfo
            {
                Id = reader.GetString(),
                Name = reader.GetString(),
                PlayerCount = reader.GetInt(),
                MaxPlayers = reader.GetInt(),
                GamePort = reader.GetInt(),
                HostAddress = remote.Address.ToString()
            };

            OnRoomDiscovered?.Invoke(room);
        }

        public void OnPeerConnected(NetPeer peer) { }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) { }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnConnectionRequest(ConnectionRequest request) { }
    }
}