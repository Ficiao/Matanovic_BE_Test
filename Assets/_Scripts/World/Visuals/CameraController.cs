using BETest.Networking.Managers;
using UnityEngine;

namespace BETest.World.Visuals
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Vector2 _deadZone = new(2f, 1f);

        private Transform _target;

        public void Initialize(PlayerManager playerManager)
        {
            playerManager.OnLocalPlayerSpawned += player => SetTarget(player.transform);
        }

        public void SetTarget(Transform target)
        {
            _target = target;

            transform.position = new Vector3(_target.position.x + _offset.x, _target.position.y + _offset.y, transform.position.z);
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 cameraPosition = transform.position;
            Vector3 targetPosition = _target.position + (Vector3)_offset;
            float deltaX = targetPosition.x - cameraPosition.x;

            if (deltaX > _deadZone.x) cameraPosition.x = targetPosition.x - _deadZone.x;
            else if (deltaX < -_deadZone.x) cameraPosition.x = targetPosition.x + _deadZone.x;

            float deltaY = targetPosition.y - cameraPosition.y;

            if (deltaY > _deadZone.y) cameraPosition.y = targetPosition.y - _deadZone.y;
            else if (deltaY < -_deadZone.y) cameraPosition.y = targetPosition.y + _deadZone.y;

            transform.position = cameraPosition;
        }
    }
}