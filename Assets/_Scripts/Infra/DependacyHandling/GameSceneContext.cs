using BETest.Entities;
using BETest.Misc;
using BETest.Networking;
using BETest.Networking.Managers;
using BETest.Networking.Services;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class GameSceneContext : Singleton<GameSceneContext>
    {
        [field: SerializeField] public SpawnManager SpawnManager { get; private set; }
        [field: SerializeField] public NetworkObjectManager NetworkObjectManager { get; private set; }
        [field: SerializeField] public PlayerManager PlayerManager { get; private set; }
        [field: SerializeField] public GameTickRunner GameTickRunner { get; private set; }
        [field: SerializeField] public ProjectileManager ProjectileManager { get; private set; }
        public NetworkObjectStateManager ObjectStateManager { get; private set; }
        public NetworkStateBroadcastService NetworkStateBroadcastService { get; private set; }

        private void Start()
        {
            DependencyContainer container = DependencyContainer.Instance;

            ProjectileManager.Initialize(container.WeaponData, container.Server.IsRunning);
            NetworkObjectManager.Initialize(PlayerManager, ProjectileManager);
            PlayerManager.Initialize(container.ObjectPrefabs, container.RoomManager, container.WeaponData);

            if (container.Server.IsRunning)
            {
                ObjectStateManager = new NetworkObjectStateManager();
                ObjectStateManager.Initialize(SpawnManager);
                NetworkStateBroadcastService = new NetworkStateBroadcastService();
                NetworkStateBroadcastService.Initialize(ObjectStateManager);
            }

            GameTickRunner.Initialize(NetworkObjectManager, NetworkStateBroadcastService);

            container.RoomManager.GameSceneReady();
        }
    }
}