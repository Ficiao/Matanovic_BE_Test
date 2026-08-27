using BETest.Config;
using BETest.Enum;
using BETest.Flags;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using BETest.Scriptables;
using BETest.World.Visuals;
using LiteNetLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static BETest.Scriptables.WeaponDataScriptable;

namespace BETest.Entities
{
    public class Player : NetworkEntity
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpHeight = 2.5f;
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _interpolationSpeed = 15f;
        [SerializeField] private Transform _modelContainer;

        private Camera _camera;
        private CharacterModelController _characterModel;
        private Vector3 _targetPosition;
        private float _verticalVelocity;
        private int _jumpsRemaining;
        private bool _jumpRequested;
        private float _horizontalInput;
        private MoveDirFlags _direction;
        private float _aimAngle;
        private float _targetAimAngle;
        private WeaponData _weaponData;
        private float _nextFireTime;

        public void Init(NetworkEntitySpawnData data, bool hasStateAuthority, CharacterModelController modelPrefab, WeaponData weaponData)
        {
            base.Init(data, hasStateAuthority);

            _weaponData = weaponData;

            _characterModel = Instantiate(modelPrefab, _modelContainer);
            _characterModel.SetPlayerName(data.ClientPlayerData.PlayerName);
            float aimAngle = Mathf.HalfToFloat(data.StateData.AimAngle);
            _characterModel.SetAimAngle(aimAngle);
            _aimAngle = aimAngle;
            _targetAimAngle = aimAngle;

            _targetPosition = transform.position;
            _jumpsRemaining = GameConfig.MAX_JUMPS;
            _camera = Camera.main;
        }

        public override void HandleTick()
        {
            if (!HasStateAuthority) return;

            bool grounded = _characterController.isGrounded;

            _direction = 0;
            if (_horizontalInput == -1) _direction = MoveDirFlags.Right;
            if (_horizontalInput == 1) _direction = MoveDirFlags.Left;
            if (grounded) _direction |= MoveDirFlags.Grounded;

            if (grounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
                _jumpsRemaining = GameConfig.MAX_JUMPS;
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

            UpdateStateFromTransform(_direction, Mathf.FloatToHalf(_aimAngle));
        }

        public override void HandleServerStateUpdate(NetworkEntityStateData state)
        {
            base.HandleServerStateUpdate(state);

            if (HasStateAuthority) return;

            if ((state.UpdateFlags & EntityUpdateFlags.Position) != 0)
            {
                _targetPosition = new Vector3(Mathf.HalfToFloat(_entityState.X), Mathf.HalfToFloat(_entityState.Y), transform.position.z);
            }

            if((state.UpdateFlags & EntityUpdateFlags.MoveDir) != 0)
            {
                _direction = state.Directions;
            }

            if ((state.UpdateFlags & EntityUpdateFlags.Aim) != 0)
            {
                _targetAimAngle = Mathf.HalfToFloat(_entityState.AimAngle);
            }
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                _horizontalInput = 0f;

                if (Keyboard.current.aKey.isPressed) _horizontalInput += 1f;
                if (Keyboard.current.dKey.isPressed) _horizontalInput -= 1f;
                if (Keyboard.current.spaceKey.wasPressedThisFrame) _jumpRequested = true;

                UpdateAim();
                UpdateFire();

                return;
            }


            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _interpolationSpeed);
            float aimAngle = Mathf.LerpAngle(_characterModel.AimAngle, _targetAimAngle, Time.deltaTime * _interpolationSpeed * 2);
            _characterModel.SetAimAngle(aimAngle);
        }

        private void UpdateAim()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            Plane plane = new Plane(Vector3.forward, transform.position);

            if (!plane.Raycast(ray, out float distance)) return;

            Vector3 worldPosition = ray.GetPoint(distance);
            _aimAngle = _characterModel.AimAt(worldPosition);
        }

        private void UpdateFire()
        {
            if (!Mouse.current.leftButton.isPressed) return;
            if (Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + _weaponData.FireRate;

            Vector3 sourcePosition = _characterModel.StaffPoint.position;
            Vector3 direction = _characterModel.AimDirection.normalized;

            PlayerShootMessage message = new()
            {
                Data = new PlayerShootData(sourcePosition, direction),
            };

            NetworkClient.SendMessage(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }
    }
}