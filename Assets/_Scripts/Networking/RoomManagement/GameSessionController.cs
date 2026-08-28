using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using BETest.UI.Controllers;
using LiteNetLib;
using UnityEngine;

namespace BETest.Networking.RoomManagement
{
    public class GameSessionController : MonoBehaviour
    {
        private RoomManager _roomManager;
        private NetworkServer _networkServer;
        private NetworkObjectStateManager _objectStateManager;
        private GameUIController _gameUIController;
        private GameTickRunner _gameTickRunner;

        public void Initialize(RoomManager roomManager, NetworkServer networkServer, NetworkObjectStateManager objectStateManager, GameUIController gameUIController, GameTickRunner gameTickRunner)
        {
            _roomManager = roomManager;
            _networkServer = networkServer;
            _objectStateManager = objectStateManager;
            _gameUIController = gameUIController;
            _gameTickRunner = gameTickRunner;

            _gameUIController.OnLeaveRequested += OnLeaveRequested;
            _networkServer.OnClientDisconnected += OnClientDisconnected;
        }

        private void OnLeaveRequested()
        {
            _gameTickRunner.Stop();
            _roomManager.LeaveRoom();
        }

        private void OnClientDisconnected(NetPeer peer)
        {
            uint PID = (uint)peer.Id;

            _objectStateManager.PlayerDisconnected(PID);
            NetworkSpawnService.BroadcastDespawn(new NetworkEntityDespawnData(PID, EntityType.Player));
        }

        private void OnDestroy()
        {
            if (_gameUIController != null) _gameUIController.OnLeaveRequested -= OnLeaveRequested;
            if (_objectStateManager != null) _networkServer.OnClientDisconnected -= OnClientDisconnected;
        }
    }
}