using BETest.Config;
using BETest.Misc;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using LiteNetLib;
using System;

namespace BETest.Networking.RoomManagement
{
    public class RoomManager : SingletonPersistent<RoomManager>
    {
        public RoomStateType State { get; private set; } = RoomStateType.Idle;
        public RoomInfo CurrentRoom { get; private set; } 
        public static event Action<RoomStateType> StateChanged;
        public static event Action RoomEntered;
        public static event Action<string> RoomOperationFailed;

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

            NetworkServer.ClientConnected += OnClientConnected;
            NetworkServer.ClientDisconnected += OnClientDisconnected;
            NetworkServer.Instance.StartServer();
            NetworkClient.Connected += OnConnected;
            NetworkClient.Instance.Connect("127.0.0.1");

            LanDiscovery.Instance.StartAdvertising();
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
            LanDiscovery.Instance.StartBrowsing();
            LanDiscovery.Instance.Search();
        }

        public void JoinRoom(RoomInfo room)
        {
            if (State != RoomStateType.Idle) return;

            SetState(RoomStateType.Joining);
            NetworkClient.Connected += OnConnected;
            NetworkClient.Instance.Connect(room.HostAddress);
        }

        private void OnConnected()
        {
            NetworkClient.Connected -= OnConnected;
            SetState(State == RoomStateType.Creating ? RoomStateType.InRoomHost : RoomStateType.InRoomClient);
            RoomEntered?.Invoke();
        }

        public void LeaveRoom()
        {
            if (State == RoomStateType.InRoomHost)
            {
                NetworkServer.ClientConnected -= OnClientConnected;
                NetworkServer.ClientDisconnected -= OnClientDisconnected;
            }

            LanDiscovery.Instance.StopAdvertising();

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