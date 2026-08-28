using UnityEngine;

namespace BETest.World.Visuals
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField, Range(0f, 1f)] private float _parallaxFactor = 0.3f;
        [SerializeField] private RectTransform _tileA;
        [SerializeField] private RectTransform _tileB;
        [SerializeField] private RectTransform _tileC;
        [SerializeField] private RectTransform _tileD;

        private float _tileWidth;
        private float _startCameraX;
        private Vector3 _startPosition;

        private void Start()
        {
            _startCameraX = _camera.position.x;
            _startPosition = transform.position;

            _tileWidth = _tileA.rect.width;

            _tileA.anchoredPosition = Vector2.left * _tileWidth * 1.5f;
            _tileB.anchoredPosition = Vector2.left * _tileWidth * 0.5f;
            _tileC.anchoredPosition = Vector2.right * _tileWidth * 0.5f;
            _tileD.anchoredPosition = Vector2.right * _tileWidth * 1.5f;
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

            RectTransform leftmost = GetLeftmost();
            RectTransform rightmost = GetRightmost();

            if (cameraLocalX - leftmost.localPosition.x > _tileWidth * 2f)
                leftmost.localPosition = rightmost.localPosition + Vector3.right * _tileWidth;
            else if (rightmost.localPosition.x - cameraLocalX > _tileWidth * 2f)
                rightmost.localPosition = leftmost.localPosition - Vector3.right * _tileWidth;
        }

        private RectTransform GetLeftmost()
        {
            RectTransform result = _tileA;

            if (_tileB.localPosition.x < result.localPosition.x) result = _tileB;
            if (_tileC.localPosition.x < result.localPosition.x) result = _tileC;
            if (_tileD.localPosition.x < result.localPosition.x) result = _tileD;

            return result;
        }

        private RectTransform GetRightmost()
        {
            RectTransform result = _tileA;

            if (_tileB.localPosition.x > result.localPosition.x) result = _tileB;
            if (_tileC.localPosition.x > result.localPosition.x) result = _tileC;
            if (_tileD.localPosition.x > result.localPosition.x) result = _tileD;

            return result;
        }
    }
}