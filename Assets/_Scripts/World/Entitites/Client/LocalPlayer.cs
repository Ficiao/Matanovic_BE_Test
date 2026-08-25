using BETest.Config;
using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using BETest.World.Visuals;
using LiteNetLib;
using UnityEngine;

namespace BETest.Entities
{
    public class LocalPlayer : NetworkEntity
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private Transform _modelContainer;
        private CharacterModelController _characterModel;
        private PlayerStateMessage _reusablePlayerStateMessage = new();

        public void Init(uint objectID, CharacterModelController modelPrefab)
        {
            base.Init(objectID);

            _characterModel = Instantiate(modelPrefab, _modelContainer);
        }

        public override void HandleTick()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector3 movement = Vector3.right * horizontal * _moveSpeed * GameConfig.TICK_DELTA;

            _characterController.Move(movement);

            _reusablePlayerStateMessage.Data = _entityState;

            NetworkClient.SendMessage(_reusablePlayerStateMessage, TransmissionChannel.StateUpdate, DeliveryMethod.Unreliable);
        }
    }
}