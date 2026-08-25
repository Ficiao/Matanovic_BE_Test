using UnityEngine;

namespace BETest.World.Visuals
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField, Range(0f, 1f)] private float _parallaxFactor = 0.3f;
        [SerializeField] private RectTransform _tileA;
        [SerializeField] private RectTransform _tileB;

        private float _tileWidth;
        private float _startCameraX;
        private Vector3 _startPosition;

        private void Start()
        {
            _startCameraX = _camera.position.x;
            _startPosition = transform.position;
            _tileWidth = _tileA.rect.width;
            _tileA.localPosition = Vector3.zero;
            _tileB.localPosition = Vector3.right * _tileWidth;
        }

        private void LateUpdate()
        {
            UpdateParallax();
            RecycleTiles();
        }

        private void UpdateParallax()
        {
            float cameraDelta = _camera.position.x - _startCameraX;
            transform.position = new Vector3(_startPosition.x + cameraDelta * _parallaxFactor, _startPosition.y, _startPosition.z);
        }

        private void RecycleTiles()
        {
            float cameraLocalX = transform.InverseTransformPoint(_camera.position).x;

            if (cameraLocalX - _tileA.localPosition.x > _tileWidth) _tileA.localPosition = _tileB.localPosition + Vector3.right * _tileWidth;
            if (cameraLocalX - _tileB.localPosition.x > _tileWidth) _tileB.localPosition = _tileA.localPosition + Vector3.right * _tileWidth;
            if (_tileA.localPosition.x - cameraLocalX > _tileWidth) _tileA.localPosition = _tileB.localPosition - Vector3.right * _tileWidth;
            if (_tileB.localPosition.x - cameraLocalX > _tileWidth) _tileB.localPosition = _tileA.localPosition - Vector3.right * _tileWidth;
        }
    }
}