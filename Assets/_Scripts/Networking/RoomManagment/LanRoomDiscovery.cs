using BETest.Misc;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BETest.Networking.RoomManagment
{
    public class LanDiscovery : SingletonPersistent<LanDiscovery>, INetEventListener
    {
        private int _discoveryPort = ConnectionConfig.DISCOVERY_PORT;
        private string _protocol = ConnectionConfig.PROTOCOL;
        private NetManager _netManager;
        private RoomInfo _advertisedRoom;

        public event Action<RoomInfo> RoomDiscovered;

        protected override void Init()
        {
            base.Init();

            _netManager = new NetManager(this)
            {
                UnconnectedMessagesEnabled = true,
                BroadcastReceiveEnabled = true
            };
        }

        public void StartAdvertising(RoomInfo room)
        {
            _advertisedRoom = room;

            if (!_netManager.IsRunning) _netManager.Start(_discoveryPort);
        }

        public void StopAdvertising() => _advertisedRoom = null;

        public void StartBrowsing()
        {
            if (!_netManager.IsRunning) _netManager.Start();
        }

        public void Search()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(_protocol);

            _netManager.SendBroadcast(writer, _discoveryPort);
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

            RoomDiscovered?.Invoke(room);
        }

        public void OnPeerConnected(NetPeer peer) { }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) { }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnConnectionRequest(ConnectionRequest request) { }
    }
}