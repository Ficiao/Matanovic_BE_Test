using BETest.Scriptables;
using UnityEngine;

namespace BETest.Audio
{
    public class GameAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private AudioClip _shootSound;
        [SerializeField] private AudioClip _enemyDeathSound;
        [SerializeField] private float _shootSoundCooldown = 0.1f;

        private VolumeSettingsScriptable _volumeSettings;
        private float _nextShootSoundTime;
        private bool _shootSoundQueued;

        public void Initialize(VolumeSettingsScriptable volumeSettings)
        {
            _volumeSettings = volumeSettings;

            _volumeSettings.MusicVolumeChanged += SetMusicVolume;
            _volumeSettings.SFXVolumeChanged += SetSFXVolume;

            SetMusicVolume(_volumeSettings.MusicVolume);
            SetSFXVolume(_volumeSettings.SFXVolume);

            _musicSource.clip = _backgroundMusic;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        private void Update()
        {
            if (!_shootSoundQueued || Time.time < _nextShootSoundTime) return;

            _shootSoundQueued = false;
            PlayShootSound();
        }

        private void PlayShootSound()
        {
            _nextShootSoundTime = Time.time + _shootSoundCooldown;
            _sfxSource.PlayOneShot(_shootSound);
        }

        public void PlayShoot()
        {
            if (Time.time >= _nextShootSoundTime)
            {
                PlayShootSound();
                return;
            }

            _shootSoundQueued = true;
        }

        public void PlayEnemyDeath()
        {
            _sfxSource.PlayOneShot(_enemyDeathSound);
        }

        private void SetMusicVolume(float volume)
        {
            _musicSource.volume = volume;
        }

        private void SetSFXVolume(float volume)
        {
            _sfxSource.volume = volume;
        }

        private void OnDestroy()
        {
            if (_volumeSettings == null) return;

            _volumeSettings.MusicVolumeChanged -= SetMusicVolume;
            _volumeSettings.SFXVolumeChanged -= SetSFXVolume;
        }
    }
}