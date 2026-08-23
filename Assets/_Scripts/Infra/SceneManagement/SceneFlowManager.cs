using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagment;
using UnityEngine.SceneManagement;

namespace BETest.Infra.SceneManagement
{
    public class SceneFlowManager : SingletonPersistent<SceneFlowManager>
    {
        private void OnEnable()
        {
            RoomManager.RoomEntered += EnterGame;
        }

        private void OnDisable()
        {
            RoomManager.RoomEntered -= EnterGame;
        }

        public void EnterGame()
        {
            SceneManager.LoadScene((int)SceneType.GameScene);
        }
    }
}