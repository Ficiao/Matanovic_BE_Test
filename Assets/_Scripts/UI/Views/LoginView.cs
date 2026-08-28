using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BETest.UI.Views
{
    public class LoginView : MonoBehaviour
    {
        [Header("Login")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private Button _loginButton;
        [SerializeField] private TMP_Text _notification;

        [Header("Options")]
        [SerializeField] private Button _optionsButton;
        [SerializeField] private GameObject _optionsMenu;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSFXVolumeChanged;
        public event Action<string> OnLoginRequested;

        private void Awake()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);

            _optionsButton.onClick.AddListener(OpenOptions);
            _returnButton.onClick.AddListener(CloseOptions);
            _quitButton.onClick.AddListener(OnQuitClicked);

            _musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(HandleSFXVolumeChanged);

            CloseOptions();
        }

        private void OnLoginClicked()
        {
            OnLoginRequested?.Invoke(_usernameInput.text);
        }

        private void OpenOptions()
        {
            _optionsMenu.SetActive(true);
        }

        private void HandleMusicVolumeChanged(float volume)
        {
            OnMusicVolumeChanged?.Invoke(volume);
        }

        private void HandleSFXVolumeChanged(float volume)
        {
            OnSFXVolumeChanged?.Invoke(volume);
        }

        private void CloseOptions()
        {
            _optionsMenu.SetActive(false);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetVolumes(float musicVolume, float SFXVolume)
        {
            _musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            _sfxVolumeSlider.SetValueWithoutNotify(SFXVolume);
        }

        public void ShowError(string message)
        {
            _notification.text = message;
            _notification.gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _loginButton.onClick.RemoveListener(OnLoginClicked);

            _optionsButton.onClick.RemoveListener(OpenOptions);
            _returnButton.onClick.RemoveListener(CloseOptions);
            _quitButton.onClick.RemoveListener(OnQuitClicked);

            _musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.RemoveListener(HandleSFXVolumeChanged);
        }
    }
}