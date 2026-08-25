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

        public virtual void HandleServerStateUpdate(NetworkEntityStateData state)
        {
            _entityState.UpdateValues(state);
        }
    }
}