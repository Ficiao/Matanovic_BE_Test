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
        private int _discoveryPort = ConnectionConfig.DISCOVERY_PORT;
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

        public void StartAdvertising()
        {
            Stop();

            _netManager.BroadcastReceiveEnabled = true;
            _netManager.Start(_discoveryPort);
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