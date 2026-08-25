using BETest.Entities;
using BETest.Misc;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class GameSceneContext : Singleton<GameSceneContext>
    {
        [SerializeField] private SpawnManager _spawnManager;
        [SerializeField] private NetworkObjectStateManager _networkObjectStateManager;
        [SerializeField] private LocalEntityManager _localEntityManager;

        private void Start()
        {
            DependencyContainer container = DependencyContainer.Instance;

            _networkObjectStateManager.Initialize(_spawnManager, container.Server);

        }

        public NetworkObjectStateManager GetNetworkObjectStateManager()
        {
            return _networkObjectStateManager;
        }
    }
}