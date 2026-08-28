using BETest.Config;
using BETest.Networking.Managers;
using BETest.Networking.Services;
using UnityEngine;

namespace BETest.Networking
{
    public class GameTickRunner : MonoBehaviour
    {
        private NetworkObjectManager _objectManager;
        private NetworkObjectStateManager _objectStateManager;
        private NetworkStateBroadcastService _stateBroadcastService;

        private float _gameTimer;
        private float _serverTimer;
        private bool _running;

        public void Initialize(NetworkObjectManager objectManager, NetworkStateBroadcastService stateBroadcastService = null, NetworkObjectStateManager objectStateManager = null)
        {
            _objectManager = objectManager;
            _stateBroadcastService = stateBroadcastService;
            _objectStateManager = objectStateManager;
            _running = true;
        }


        private void Update()
        {
            if (!_running) return;

            float deltaTime = Time.unscaledDeltaTime;
            _gameTimer += deltaTime;

            while (_gameTimer >= GameConfig.TICK_DELTA)
            {
                _gameTimer -= GameConfig.TICK_DELTA;

                _objectStateManager?.HandleTick();
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

        public void Stop()
        {
            _running = false;
        }
    }
}