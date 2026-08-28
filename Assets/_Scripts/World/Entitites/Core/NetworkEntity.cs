using BETest.Flags;
using BETest.Networking.Messages;
using UnityEngine;

namespace BETest.Entities
{
    public abstract class NetworkEntity : NetworkObject
    {
        protected NetworkEntityStateData _entityState;

        public NetworkEntityStateData GetEntityStateForBroadcast()
        {
            _entityState.SeqAcc++;
            return _entityState;
        }

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

        protected void UpdateStateFromTransform(MoveDirFlags direction, ushort AimAngle)
        {
            _entityState.X = Mathf.FloatToHalf(transform.position.x);
            _entityState.Y = Mathf.FloatToHalf(transform.position.y);
            _entityState.Directions = direction;
            _entityState.AimAngle = AimAngle;
        }

        protected void UpdatePositionStateFromTransform()
        {
            _entityState.X = Mathf.FloatToHalf(transform.position.x);
            _entityState.Y = Mathf.FloatToHalf(transform.position.y);
        }
    }
}