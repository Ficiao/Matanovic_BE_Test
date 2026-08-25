using BETest.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "ObjectPrefabs", menuName = "Scriptables/ObjectPrefabs")]
    public class ObjectPrefabsScriptable : ScriptableObject
    {
        [Serializable]
        public class PrefabData
        {
            public ObjectPrefabType PrefabType;
            public GameObject Prefab;
        }

        [SerializeField] private List<PrefabData> _prefabs;
        public List<PrefabData> Prefabs => _prefabs;

        public GameObject GetPrefab(ObjectPrefabType prefabType)
        {
            return _prefabs.First(prefab => prefab.PrefabType == prefabType).Prefab;
        }
    }
}