using BETest.Networking.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BETest.Infra.SceneManagement
{
    public class SceneFlowManager : MonoBehaviour
    {
        private bool _loading;

        public void EnterLogin()
        {
            LoadScene(SceneType.LoginScreen);
        }

        public void EnterGame()
        {
            LoadScene(SceneType.GameScene);
        }

        private void LoadScene(SceneType sceneType)
        {
            if (_loading) return;

            _loading = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneType);
            operation.completed += _ => _loading = false;
        }
    }
}