using BETest.Config;
using BETest.Entities;
using BETest.Scriptables;
using BETest.UI.Views;
using System;
using UnityEngine;

namespace BETest.UI.Controllers
{
    public class LoginController : MonoBehaviour
    {
        [SerializeField] private LoginView _view;
        private VolumeSettingsScriptable _volumeSettings;
        private LocalPlayerSession _localPlayerSession;

        public event Action OnLoginSucceeded;

        public void Initialize(LocalPlayerSession localPlayerSession, VolumeSettingsScriptable volumeSettings)
        {
            _localPlayerSession = localPlayerSession;
            _volumeSettings = volumeSettings;

            _view.SetVolumes(_volumeSettings.MusicVolume, _volumeSettings.SFXVolume);
        }

        private void OnEnable()
        {
            _view.OnLoginRequested += Login;
            _view.OnMusicVolumeChanged += OnMusicVolumeChanged;
            _view.OnSFXVolumeChanged += OnSFXVolumeChanged;
        }        

        private void OnDisable()
        {
            _view.OnLoginRequested -= Login;
            _view.OnMusicVolumeChanged -= OnMusicVolumeChanged;
            _view.OnSFXVolumeChanged -= OnSFXVolumeChanged;
        }

        private void OnMusicVolumeChanged(float volume)
        {
            _volumeSettings.SetMusicVolume(volume);
        }

        private void OnSFXVolumeChanged(float volume)
        {
            _volumeSettings.SetSFXVolume(volume);
        }

        private void Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < GameConfig.MIN_USERNAME_LENGTH || username.Length > GameConfig.MAX_USERNAME_LENGTH)
            {
                _view.ShowError($"Username must be between {GameConfig.MIN_USERNAME_LENGTH} and {GameConfig.MAX_USERNAME_LENGTH} characters.");
                return;
            }

            _localPlayerSession.SetUsername(username);
            _view.Hide();

            OnLoginSucceeded?.Invoke();
        }

        public void TryAutoLogin()
        {
            if (string.IsNullOrWhiteSpace(_localPlayerSession.Username)) return;

            _view.Hide();
            OnLoginSucceeded?.Invoke();
        }
    }
}