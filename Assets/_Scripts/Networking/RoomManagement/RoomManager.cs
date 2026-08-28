using BETest.Config;
using BETest.Infra.SceneManagement;
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
        private LanRoomDiscovery _lanDiscovery;
        private SceneFlowManager _sceneFlowManager;
        private bool _leavingRoom;

        public int WorldSeed { get; private set; }
        public RoomStateType State { get; private set; } = RoomStateType.Idle;
        public RoomInfo CurrentRoom { get; private set; } 

        public event Action<RoomStateType> OnStateChanged;
        public event Action<string> OnRoomOperationFailed;

        public void Initialize(NetworkServer networkServer, NetworkClient networkClient, LanRoomDiscovery lanDiscovery, SceneFlowManager sceneFlowManager)
        {
            _networkServer = networkServer;
            _networkClient = networkClient;
            _lanDiscovery = lanDiscovery;
            _sceneFlowManager = sceneFlowManager;
            _networkClient.OnDisconnected += OnServerDisconnected;
        }

        public void CreateRoom(string roomName)
        {
            if (State != RoomStateType.Idle) return;

            SetState(RoomStateType.Creating);

            _networkServer.OnClientConnected += OnClientConnected;
            _networkServer.OnClientDisconnected += OnClientDisconnected;

            CustomLogger.Info("server_starting");
            int gamePort = FindAvailableGamePort();

            if (gamePort == -1)
            {
                _networkServer.OnClientConnected -= OnClientConnected;
                _networkServer.OnClientDisconnected -= OnClientDisconnected;

                SetState(RoomStateType.Idle);
                OnRoomOperationFailed?.Invoke("Failed to start server.");
                return;
            }

            CurrentRoom = new RoomInfo
            {
                Id = Guid.NewGuid().ToString(),
                Name = roomName,
                PlayerCount = 0,
                MaxPlayers = GameConfig.MAX_PLAYERS_PER_ROOM,
                GamePort = gamePort,
            };

            WorldSeed = Guid.NewGuid().GetHashCode();
            _sceneFlowManager.EnterGame();
        }

        private int FindAvailableGamePort()
        {
            for (int port = ConnectionConfig.GAME_PORT; port <= ConnectionConfig.MAX_GAME_PORT; port++)
            {
                if (_networkServer.StartServer(port)) return port;
            }

            return -1;
        }

        public void JoinRoom(RoomInfo room)
        {
            if (State != RoomStateType.Idle) return;

            _lanDiscovery.Stop();

            CurrentRoom = room;
            SetState(RoomStateType.Joining);

            _sceneFlowManager.EnterGame();
        }

        public void GameSceneReady()
        {
            switch (State)
            {
                case RoomStateType.Creating:
                    _networkClient.Connect("127.0.0.1", CurrentRoom.GamePort);
                    break;

                case RoomStateType.Joining:
                    _networkClient.Connect(CurrentRoom.HostAddress, CurrentRoom.GamePort);
                    break;
            }
        }

        public void LocalPlayerReady()
        {
            switch (State)
            {
                case RoomStateType.Creating:
                    SetState(RoomStateType.InRoomHost);
                    _lanDiscovery.StartAdvertising(CurrentRoom.GamePort);
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

            CurrentRoom.PlayerCount = Mathf.Max(0, CurrentRoom.PlayerCount - 1);
        }

        public void BrowseRooms()
        {
            _lanDiscovery.StartBrowsing();
            _lanDiscovery.Search();
        }

        public void LeaveRoom()
        {
            if (State == RoomStateType.Idle) return;

            _leavingRoom = true;

            bool wasHost = State == RoomStateType.Creating || State == RoomStateType.InRoomHost;

            if (wasHost)
            {
                _networkServer.OnClientConnected -= OnClientConnected;
                _networkServer.OnClientDisconnected -= OnClientDisconnected;
            }

            _lanDiscovery.Stop();
            if (wasHost) _networkServer.StopServer();
            _networkClient.Disconnect();

            CurrentRoom = null;
            SetState(RoomStateType.Idle);

            _sceneFlowManager.EnterLogin();

            _leavingRoom = false;
        }

        private void OnServerDisconnected()
        {
            if (_leavingRoom || State == RoomStateType.Idle) return;

            bool wasHost = State == RoomStateType.Creating || State == RoomStateType.InRoomHost;

            if (wasHost)
            {
                _networkServer.OnClientConnected -= OnClientConnected;
                _networkServer.OnClientDisconnected -= OnClientDisconnected;

                _networkServer.StopServer();
            }

            _lanDiscovery.Stop();
            _networkClient.Disconnect();

            CurrentRoom = null;
            SetState(RoomStateType.Idle);

            _sceneFlowManager.EnterLogin();
        }

        private void SetState(RoomStateType state)
        {
            State = state;
            OnStateChanged?.Invoke(state);
        }
    }
}