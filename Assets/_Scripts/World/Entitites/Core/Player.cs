using BETest.Config;
using BETest.Networking.Messages;
using BETest.World.Visuals;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BETest.Entities
{
    public class Player : NetworkEntity
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpHeight = 2.5f;
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _interpolationSpeed = 15f;
        [SerializeField] private Transform _modelContainer;

        private CharacterModelController _characterModel;
        private Vector3 _targetPosition;
        private float _verticalVelocity;
        private int _jumpsRemaining;
        private bool _jumpRequested;
        private float _horizontalInput;

        private const int MAX_JUMPS = 2;

        public void Init(NetworkEntitySpawnData data, bool hasStateAuthority, CharacterModelController modelPrefab)
        {
            base.Init(data, hasStateAuthority);

            _characterModel = Instantiate(modelPrefab, _modelContainer);
            _targetPosition = transform.position;
            _jumpsRemaining = MAX_JUMPS;
        }

        public override void HandleTick()
        {
            if (!HasStateAuthority) return;

            bool grounded = _characterController.isGrounded;

            if (grounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
                _jumpsRemaining = MAX_JUMPS;
            }

            if (_jumpRequested && _jumpsRemaining > 0)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _jumpsRemaining--;
            }

            _jumpRequested = false;
            _verticalVelocity += _gravity * GameConfig.TICK_DELTA;

            Vector3 movement = new Vector3(_horizontalInput * _moveSpeed, _verticalVelocity, 0f) * GameConfig.TICK_DELTA;
            _characterController.Move(movement);

            UpdateStateFromTransform();
        }

        public override void HandleServerStateUpdate(NetworkEntityStateData state)
        {
            base.HandleServerStateUpdate(state);

            if (HasStateAuthority) return;

            _targetPosition = new Vector3(Mathf.HalfToFloat(state.X), Mathf.HalfToFloat(state.Y), transform.position.z);
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                _horizontalInput = 0f;

                if (Keyboard.current.aKey.isPressed) _horizontalInput += 1f;
                if (Keyboard.current.dKey.isPressed) _horizontalInput -= 1f;
                if (Keyboard.current.spaceKey.wasPressedThisFrame) _jumpRequested = true;

                // TODO: Implement platform dropping
                return;
            }


            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _interpolationSpeed);
        }
    }
}