using BETest.Config;
using BETest.Infra.SceneManagement;
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
        private NetworkServer _networkServer;
        private NetworkClient _networkClient;
        private LanDiscovery _lanDiscovery;
        private SceneFlowManager _sceneFlowManager;

        public RoomStateType State { get; private set; } = RoomStateType.Idle;
        public RoomInfo CurrentRoom { get; private set; } 

        public event Action<RoomStateType> StateChanged;
        public event Action<string> RoomOperationFailed;

        public void Initialize(NetworkServer networkServer,  NetworkClient networkClient, LanDiscovery lanDiscovery, SceneFlowManager sceneFlowManager)
        {
            _networkServer = networkServer;
            _networkClient = networkClient;
            _lanDiscovery = lanDiscovery;
            _sceneFlowManager = sceneFlowManager;
        }

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

            if (!_networkServer.StartServer())
            {
                CurrentRoom = null;
                SetState(RoomStateType.Idle);
                RoomOperationFailed?.Invoke("Failed to start server.");
                CustomLogger.Error("server_start_failed");

                return;
            }

            _sceneFlowManager.EnterGame();
        }

        public void JoinRoom(RoomInfo room)
        {
            if (State != RoomStateType.Idle) return;

            CurrentRoom = room;
            SetState(RoomStateType.Joining);

            _sceneFlowManager.EnterGame();
        }

        public void GameSceneReady()
        {
            switch (State)
            {
                case RoomStateType.Creating:
                    _networkClient.Connect("127.0.0.1");
                    break;

                case RoomStateType.Joining:
                    _networkClient.Connect(CurrentRoom.HostAddress);
                    break;
            }
        }

        public void LocalPlayerReady()
        {
            switch (State)
            {
                case RoomStateType.Creating:
                    SetState(RoomStateType.InRoomHost);
                    _lanDiscovery.StartAdvertising();
                    break;

                case RoomStateType.Joining:
                    SetState(RoomStateType.InRoomClient);
                    break;
            }
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

        public void LeaveRoom()
        {
            bool wasHost = State == RoomStateType.Creating || State == RoomStateType.InRoomHost;

            if (wasHost)
            {
                _networkServer.ClientConnected -= OnClientConnected;
                _networkServer.ClientDisconnected -= OnClientDisconnected;
            }

            _lanDiscovery.StopAdvertising();
            _networkClient.Disconnect();

            if (wasHost) _networkServer.StopServer();

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