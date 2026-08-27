using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "Settings Scriptable", menuName = "Settings Scriptable")]
    class SettingsScriptable : ScriptableObject
    {
        [SerializeField] private float _upDownSensitivity = 0f;
        [SerializeField] private float _leftRightSensitivity = 0f;
        private float _upDownMultiplier = 1f;
        private float _leftRightMultiplier = 1f;

        public float UpDownSensitivity => _upDownSensitivity * _upDownMultiplier;
        public float LeftRightSensitivity => _leftRightSensitivity * _leftRightMultiplier;
        public float UpDownMultiplier => _upDownMultiplier;
        public float LeftRightMultiplier => _leftRightMultiplier;

        public void SetUpDownMultiplier(float multiplier) => _upDownMultiplier = multiplier;
        public void SetLeftRightMultiplier(float multiplier) => _leftRightMultiplier = multiplier;
    }
}