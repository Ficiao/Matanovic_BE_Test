using UnityEngine;

namespace BETest.Misc
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        public static T Instance { get => _instance; private set => _instance = value; }
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this as T;
            }

            Init();
        }

        protected virtual void Init()
        {
            if (Instance == null) Instance = this as T;
        }

        private void OnDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    public abstract class SingletonReplaceable<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        public static T Instance { get => _instance; private set => _instance = value; }
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }

            Instance = this as T;

            Init();
        }

        protected virtual void Init()
        {
            if (Instance == null) Instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                base.Awake();
            }

            Init();
        }
    }

    public static class SingletonNonMono<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception($"SingletonNonMono<{typeof(T).Name}> has no implementation registered.");
                return _instance;
            }
        }

        public static void Register(T implementation)
        {
            _instance = implementation;
        }

        public static void Clear()
        {
            _instance = default;
        }
    }
}
