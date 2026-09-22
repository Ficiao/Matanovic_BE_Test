using BETest.Entities;
using BETest.Infra.SceneManagement;
using BETest.Misc;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class DependencyContainer : MonoBehaviour
    {
        [field: SerializeField] public RoomManager RoomManager { get; private set; }
        [field: SerializeField] public LanRoomDiscovery LanDiscovery { get; private set; }
        [field: SerializeField] public NetworkClient Client { get; private set; }
        [field: SerializeField] public NetworkServer Server { get; private set; }
        [field: SerializeField] public SceneFlowManager SceneFlowManager { get; private set; }
        [field: SerializeField] public LocalPlayerSession LocalPlayerSession { get; private set; }
        [field: SerializeField] public ObjectPrefabsScriptable ObjectPrefabs { get; private set; }
        [field: SerializeField] public WeaponDataScriptable WeaponData { get; private set; }
        [field: SerializeField] public VolumeSettingsScriptable VolumeSettings { get; private set; }

        public void Initialize()
        {
            VolumeSettings.Initialize();
            Client.Initialize(this);
            Server.Initialize(this);
            RoomManager.Initialize(Server, Client, LanDiscovery, SceneFlowManager);
            LanDiscovery.Initialize(RoomManager);
            SceneFlowManager.Initialize(this);
        }
    }
}