using TMPro;
using UnityEngine;

namespace BETest.World.Visuals
{
    public class CharacterModelController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rightArmPivot;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _staffPoint;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private float _aimAngleOffset;

        public Transform StaffPoint => _staffPoint;
        public float AimAngle => _rightArmPivot.eulerAngles.z;
        public Vector3 AimDirection => _rightArmPivot.right;

        public float AimAt(Vector3 worldPosition)
        {
            Vector3 direction = worldPosition - _rightArmPivot.position;
            _rightArmPivot.right = Quaternion.AngleAxis(_aimAngleOffset, Vector3.forward) * direction;
            return _rightArmPivot.eulerAngles.z;
        }

        public void SetAimAngle(float angle)
        {
            _rightArmPivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void SetPlayerName(string playerName) => _playerName.text = playerName;
    }
}