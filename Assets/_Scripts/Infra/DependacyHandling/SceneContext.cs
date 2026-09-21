using BETest.Scriptables;
using UnityEngine;

namespace BETest.Infra.DependacyHandling
{
    public abstract class SceneContext : MonoBehaviour
    {
        [SerializeField] private SceneContextChannel _sceneContextChannel;
        private bool _initialized;
        protected SceneContextChannel ContextChannel => _sceneContextChannel;

        protected virtual void Start()
        {
            _sceneContextChannel.Register(this);
        }

        public virtual void Initialize(DependencyContainer container)
        {
            if (_initialized) return;
            _initialized = true;
        }

        protected virtual void OnDestroy()
        {
            _sceneContextChannel.Unregister(this);
        }
    }
}