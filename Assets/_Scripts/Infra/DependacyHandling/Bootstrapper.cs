using BETest.Infra.DependacyHandling;
using UnityEngine;

namespace BETest.Infra
{
    [DefaultExecutionOrder(-1000)]
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private DependencyContainer _container;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _container.Initialize();
            _container.SceneFlowManager.EnterLogin();
        }
    }
}