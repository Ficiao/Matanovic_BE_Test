using BETest.Entities;
using BETest.Misc;
using BETest.UI.Controllers;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public class LoginSceneContext : Singleton<LoginSceneContext>
    {
        [SerializeField] private SessionBrowserController _sessionBrowser;
        [SerializeField] private LoginController _loginController;

        private void Start()
        {
            DependencyContainer container = DependencyContainer.Instance;

            _sessionBrowser.Initialize(container.RoomManager, container.LanDiscovery, container.WeaponData, container.LocalPlayerSession, _loginController);
            _loginController.Initialize(container.LocalPlayerSession);
        }
    }
}