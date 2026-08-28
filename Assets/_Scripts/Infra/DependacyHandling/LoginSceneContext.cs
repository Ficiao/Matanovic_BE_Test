using BETest.Misc;
using BETest.UI.Controllers;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class LoginSceneContext : Singleton<LoginSceneContext>
    {
        [field: SerializeField] public SessionBrowserController SessionBrowserController { get; private set; }
        [field: SerializeField] public LoginController LoginController { get; private set; }

        private void Start()
        {
            DependencyContainer container = DependencyContainer.Instance;

            container.LocalPlayerSession.ClearRoomData();

            SessionBrowserController.Initialize(container.RoomManager, container.LanDiscovery, container.WeaponData, container.LocalPlayerSession, LoginController);
            LoginController.Initialize(container.LocalPlayerSession, container.VolumeSettings);
            LoginController.TryAutoLogin();
        }
    }
}