using BETest.Config;
using BETest.Misc;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System;
using UnityEngine;

namespace BETest.Networking.RoomManagement
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private NetworkServer _networkServer;
        [SerializeField] private NetworkClient _networkClient;
        [SerializeField] private LanDiscovery _lanDiscovery;

        public RoomStateType State { get; private set; } = RoomStateType.Idle;
        public RoomInfo CurrentRoom { get; private set; } 
        public event Action<RoomStateType> StateChanged;
        public event Action RoomEntered;
        public event Action<string> RoomOperationFailed;

        public void CreateRoom(string roomName)
        {
            if (State != RoomStateType.Idle) return;

            SetState(RoomStateType.Creating);

            CurrentRoom = new RoomInfo
            {
                Id = Guid.NewGuid().ToString(),
                Name = roomName,
                PlayerCount = 0,
                MaxPlayers = GameConfig.MAX_PLAYERS_PER_ROOM,
                GamePort = ConnectionConfig.GAME_PORT,
            };

            _networkServer.ClientConnected += OnClientConnected;
            _networkServer.ClientDisconnected += OnClientDisconnected;
            _networkServer.StartServer();
            _networkClient.Connected += OnConnected;
            _networkClient.Connect("127.0.0.1");

            _lanDiscovery.StartAdvertising();
        }

        private void OnClientConnected(NetPeer peer)
        {
            if (CurrentRoom == null) return;

            CurrentRoom.PlayerCount++;
            CustomLogger.Info("room_player_joined", new()
            {
                ["players"] = CurrentRoom.PlayerCount
            });
        }

        private void OnClientDisconnected(NetPeer peer)
        {
            if (CurrentRoom == null) return;

            CurrentRoom.PlayerCount--;
        }

        public void BrowseRooms()
        {
            _lanDiscovery.StartBrowsing();
            _lanDiscovery.Search();
        }

        public void JoinRoom(RoomInfo room)
        {
            if (State != RoomStateType.Idle) return;

            SetState(RoomStateType.Joining);
            _networkClient.Connected += OnConnected;
            _networkClient.Connect(room.HostAddress);
        }

        private void OnConnected()
        {
            _networkClient.Connected -= OnConnected;
            SetState(State == RoomStateType.Creating ? RoomStateType.InRoomHost : RoomStateType.InRoomClient);
            RoomEntered?.Invoke();
        }

        public void LeaveRoom()
        {
            if (State == RoomStateType.InRoomHost)
            {
                _networkServer.ClientConnected -= OnClientConnected;
                _networkServer.ClientDisconnected -= OnClientDisconnected;
            }

            _lanDiscovery.StopAdvertising();
            CurrentRoom = null;
            SetState(RoomStateType.Idle);
        }

        private void SetState(RoomStateType state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}