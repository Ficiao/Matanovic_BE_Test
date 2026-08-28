using System;
using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "VolumeSettingsScriptable", menuName = "Scriptables/Volume Settings Scriptable")]
    public class VolumeSettingsScriptable : ScriptableObject
    {
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";

        [SerializeField, Range(0f, 1f)] private float _defaultMusicVolume = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _defaultSFXVolume = 1f;

        public float MusicVolume { get; private set; }
        public float SFXVolume { get; private set; }

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SFXVolumeChanged;

        public void Initialize()
        {
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, _defaultMusicVolume);
            SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, _defaultSFXVolume);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);

            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, MusicVolume);
            PlayerPrefs.Save();

            MusicVolumeChanged?.Invoke(MusicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);

            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFXVolume);
            PlayerPrefs.Save();

            SFXVolumeChanged?.Invoke(SFXVolume);
        }
    }
}