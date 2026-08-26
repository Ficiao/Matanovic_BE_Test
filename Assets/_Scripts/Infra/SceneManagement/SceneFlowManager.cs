using BETest.Misc;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace BETest.Infra.SceneManagement
{
    public class SceneFlowManager : MonoBehaviour
    {
        private bool _loading;

        public void EnterGame()
        {
            if (_loading) return;

            _loading = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync((int)SceneType.GameScene);
            operation.completed += _ => _loading = false;
        }
    }
}