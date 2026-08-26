using BETest.Config;
using BETest.Networking.Managers;
using BETest.Networking.Services;
using UnityEngine;

namespace BETest.Networking
{
    public class GameTickRunner : MonoBehaviour
    {
        private NetworkObjectManager _objectManager;
        private NetworkStateBroadcastService _stateBroadcastService;

        private float _gameTimer;
        private float _serverTimer;

        public void Initialize(NetworkObjectManager objectManager, NetworkStateBroadcastService stateBroadcastService = null)
        {
            _objectManager = objectManager;
            _stateBroadcastService = stateBroadcastService;
        }

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;

            _gameTimer += deltaTime;

            while (_gameTimer >= GameConfig.TICK_DELTA)
            {
                _gameTimer -= GameConfig.TICK_DELTA;
                _objectManager.HandleTick();
            }

            if (_stateBroadcastService == null) return;

            _serverTimer += deltaTime;

            while (_serverTimer >= GameConfig.SERVER_TICK_DELTA)
            {
                _serverTimer -= GameConfig.SERVER_TICK_DELTA;
                _stateBroadcastService.HandleTick();
            }
        }
    }
}