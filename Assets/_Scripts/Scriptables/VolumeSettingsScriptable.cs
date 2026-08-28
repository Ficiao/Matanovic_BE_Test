using System;
using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "VolumeSettingsScriptable", menuName = "Scriptables/Volume Settings Scriptable")]
    public class VolumeSettingsScriptable : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _defaultMusicVolume = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _defaultSFXVolume = 1f;

        public float MusicVolume { get; private set; }
        public float SFXVolume { get; private set; }

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SFXVolumeChanged;

        public void Initialize()
        {
            MusicVolume = _defaultMusicVolume;
            SFXVolume = _defaultSFXVolume;
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            MusicVolumeChanged?.Invoke(MusicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);
            SFXVolumeChanged?.Invoke(SFXVolume);
        }
    }
}