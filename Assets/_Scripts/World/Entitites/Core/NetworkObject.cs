using BETest.Enum;
using UnityEngine;

namespace BETest.Entities
{
    public abstract class NetworkObject : MonoBehaviour
    {
        protected uint _objectID;
        protected EntityType _entityType;

        public uint ObjectID => _objectID;
        public EntityType EntityType => _entityType;
        public bool HasStateAuthority { get; private set; }

        public virtual void Init(uint objectID, EntityType entityType, bool hasStateAuthority)
        {
            _objectID = objectID;
            _entityType = entityType;
            HasStateAuthority = hasStateAuthority;
        }

        public abstract void HandleTick();
    }
}