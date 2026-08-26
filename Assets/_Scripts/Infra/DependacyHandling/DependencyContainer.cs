using BETest.Entities;
using BETest.Infra.SceneManagement;
using BETest.Misc;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class DependencyContainer : SingletonPersistent<DependencyContainer>
    {
        [field: SerializeField] public RoomManager RoomManager { get; private set; }
        [field: SerializeField] public LanDiscovery LanDiscovery { get; private set; }
        [field: SerializeField] public NetworkClient Client { get; private set; }
        [field: SerializeField] public NetworkServer Server { get; private set; }
        [field: SerializeField] public SceneFlowManager SceneFlowManager { get; private set; }
        [field: SerializeField] public LocalPlayerSession LocalPlayerSession { get; private set; }
        [field: SerializeField] public ObjectPrefabsScriptable ObjectPrefabs { get; private set; }
        [field: SerializeField] public WeaponDataScriptable WeaponData { get; private set; }

        private void Start()
        {
            RoomManager.Initialize(Server, Client, LanDiscovery, SceneFlowManager);
            LanDiscovery.Initialize(RoomManager);
        }
    }
}