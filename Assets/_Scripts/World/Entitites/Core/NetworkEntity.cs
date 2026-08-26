using BETest.Networking.Messages;
using UnityEngine;

namespace BETest.Entities
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class NetworkEntity : NetworkObject
    {
        [SerializeField] protected CharacterController _characterController;
        protected NetworkEntityStateData _entityState;

        public NetworkEntityStateData EntityState => _entityState;

        public virtual void Init(NetworkEntitySpawnData data, bool hasStateAuthority)
        {
            base.Init(data.StateData.ObjectID, data.StateData.EntityType, hasStateAuthority);

            _entityState = data.StateData;
            transform.position = new Vector3(Mathf.HalfToFloat(_entityState.X), Mathf.HalfToFloat(_entityState.Y), transform.position.z);
        }

        public virtual void HandleServerStateUpdate(NetworkEntityStateData state)
        {
            _entityState.UpdateValues(state);
        }

        protected void UpdateStateFromTransform()
        {
            _entityState.X = Mathf.FloatToHalf(transform.position.x);
            _entityState.Y = Mathf.FloatToHalf(transform.position.y);
        }
    }
}