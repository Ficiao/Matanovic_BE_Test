using BETest.Infra.DependacyHandling;
using BETest.Networking.Messages;
using BETest.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BETest.Infra.SceneManagement
{
    public class SceneFlowManager : MonoBehaviour
    {
        [SerializeField] private SceneContextChannel _sceneContextChannel;
        private DependencyContainer _container;
        private bool _loading;

        public void Initialize(DependencyContainer container)
        {
            _container = container;
            _sceneContextChannel.ContextReady += InitializeContext;
            _sceneContextChannel.ContextReleased += ReleaseContext;
        }

        public void EnterLogin() => LoadScene(SceneType.LoginScreen);

        public void EnterGame() => LoadScene(SceneType.GameScene);

        private void LoadScene(SceneType sceneType)
        {
            if (_loading) return;

            _loading = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneType);
            operation.completed += _ => _loading = false;
        }

        private void InitializeContext(SceneContext context)
        {
            context.Initialize(_container);
            if (context is GameSceneContext gameContext)
            {
                _container.Client.MessageProcessor.BindContext(gameContext);
                _container.Server.MessageProcessor.BindContext(gameContext);

                _container.RoomManager.GameSceneReady();
            }
        }

        private void ReleaseContext(SceneContext context)
        {
            if (context is GameSceneContext gameContext)
            {
                _container.Client.MessageProcessor.UnbindContext(gameContext);
                _container.Server.MessageProcessor.UnbindContext(gameContext);
            }
        }

        private void OnDestroy()
        {
            if (_sceneContextChannel != null)
            {
                _sceneContextChannel.ContextReady -= InitializeContext;
                _sceneContextChannel.ContextReleased -= ReleaseContext;
            }
        }
    }
}