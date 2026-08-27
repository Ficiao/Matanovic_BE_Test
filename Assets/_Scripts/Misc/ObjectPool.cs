using System.Collections.Generic;
using UnityEngine;

namespace BETest.Misc
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _available = new();

        public ObjectPool(T prefab, Transform container)
        {
            _prefab = prefab;
            _container = container;
        }

        public T Get()
        {
            if (_available.Count > 0) return _available.Pop();

            T instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);

            return instance;
        }

        public void Release(T instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container);
            _available.Push(instance);
        }
    }
}