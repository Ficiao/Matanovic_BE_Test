using BETest.Misc;
using BETest.UI.Controllers;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class LoginSceneContext : SceneContext
    {
        [field: SerializeField] public SessionBrowserController SessionBrowserController { get; private set; }
        [field: SerializeField] public LoginController LoginController { get; private set; }

        public override void Initialize(DependencyContainer container)
        {
            base.Initialize(container);

            container.LocalPlayerSession.ClearRoomData();

            SessionBrowserController.Initialize(container.RoomManager, container.LanDiscovery, container.WeaponData, container.LocalPlayerSession, LoginController);
            LoginController.Initialize(container.LocalPlayerSession, container.VolumeSettings);
            LoginController.TryAutoLogin();
        }
    }
}