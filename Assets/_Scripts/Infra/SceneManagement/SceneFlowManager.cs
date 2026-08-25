using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BETest.Infra.SceneManagement
{
    public class SceneFlowManager : MonoBehaviour
    {
        [SerializeField] private RoomManager _roomManager;

        private void OnEnable()
        {
            _roomManager.RoomEntered += EnterGame;
        }

        private void OnDisable()
        {
            _roomManager.RoomEntered -= EnterGame;
        }

        public void EnterGame()
        {
            SceneManager.LoadScene((int)SceneType.GameScene);
        }
    }
}