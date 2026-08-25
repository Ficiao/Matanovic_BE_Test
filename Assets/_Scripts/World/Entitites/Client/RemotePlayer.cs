using BETest.Networking.Messages;
using BETest.World.Visuals;
using UnityEngine;

namespace BETest.Entities
{
    public class RemotePlayer : NetworkEntity
    {
        [SerializeField] private float _lerpSpeed = 15f;
        [SerializeField] private Transform _modelContainer;
        private Vector3 _targetPosition;
        private CharacterModelController _characterModel;

        public void Init(uint objectID, CharacterModelController modelPrefab)
        {
            base.Init(objectID);

            _characterModel = Instantiate(modelPrefab, _modelContainer);
        }

        public override void HandleServerStateUpdate(NetworkEntityStateData state)
        {
        }

        public override void HandleTick()
        {
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _lerpSpeed);
        }
    }
}