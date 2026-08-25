using UnityEngine;

namespace BETest.Entities
{
    public abstract class NetworkObject : MonoBehaviour
    {
        protected uint _objectID;

        public uint ObjectID => _objectID;

        public virtual void Init(uint objectID)
        {
            _objectID = objectID;
        }

        public abstract void HandleTick();
    }
}