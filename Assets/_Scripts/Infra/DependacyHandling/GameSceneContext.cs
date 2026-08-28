using BETest.Audio;
using BETest.Entities;
using BETest.Misc;
using BETest.Networking;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Managers;
using BETest.Networking.RoomManagement;
using BETest.Networking.Services;
using BETest.UI.Controllers;
using BETest.World;
using BETest.World.Visuals;
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
        [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
        [field: SerializeField] public GameUIController GameUIController { get; private set; }
        [field: SerializeField] public GameSessionController GameSessionController { get; private set; }
        [field: SerializeField] public CameraController CameraController { get; private set; }
        [field: SerializeField] public TerrainGenerator TerrainGenerator { get; private set; }
        [field: SerializeField] public GameAudioManager GameAudioManager { get; private set; }
        public NetworkObjectStateManager ObjectStateManager { get; private set; }
        public NetworkStateBroadcastService NetworkStateBroadcastService { get; private set; }

        private void Start()
        {
            DependencyContainer container = DependencyContainer.Instance;
            bool isServerRunning = NetworkServer.IsRunning;

            if (isServerRunning)
            {
                ObjectStateManager = new NetworkObjectStateManager();
                ObjectStateManager.Initialize(SpawnManager, container.ObjectPrefabs, container.RoomManager.WorldSeed);

                NetworkStateBroadcastService = new NetworkStateBroadcastService();
                NetworkStateBroadcastService.Initialize(ObjectStateManager);
            }

            GameAudioManager.Initialize(container.VolumeSettings);
            PlayerManager.Initialize(container.ObjectPrefabs, container.RoomManager, container.WeaponData);
            ProjectileManager.Initialize(container.WeaponData, isServerRunning, GameAudioManager);
            EnemyManager.Initialize(container.ObjectPrefabs, PlayerManager, isServerRunning, ObjectStateManager, GameAudioManager);
            GameUIController.Initialize(PlayerManager, container.VolumeSettings);
            GameSessionController.Initialize(container.RoomManager, container.Server, ObjectStateManager, GameUIController, GameTickRunner);
            NetworkObjectManager.Initialize(PlayerManager, ProjectileManager, EnemyManager, ObjectStateManager);
            GameTickRunner.Initialize(NetworkObjectManager, NetworkStateBroadcastService, ObjectStateManager);
            CameraController.Initialize(PlayerManager);

            container.RoomManager.GameSceneReady();
        }
    }
}