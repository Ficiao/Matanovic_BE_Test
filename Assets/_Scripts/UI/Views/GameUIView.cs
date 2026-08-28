using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BETest.UI.Views
{
    public class GameUIView : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Death")]
        [SerializeField] private GameObject _deathScreen;
        [SerializeField] private TextMeshProUGUI _respawnTimer;

        [Header("Scoreboard")]
        [SerializeField] private GameObject _scoreboard;
        [SerializeField] private Transform _scoreboardDataContainer;
        [SerializeField] private PlayerScoreboardDataView _playerScoreboardDataPrefab;

        [Header("Options")]
        [SerializeField] private Button _optionsButton;
        [SerializeField] private GameObject _optionsMenu;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        public Transform ScoreboardDataContainer => _scoreboardDataContainer;
        public PlayerScoreboardDataView PlayerScoreboardDataPrefab => _playerScoreboardDataPrefab;
        public bool IsOptionsOpen => _optionsMenu.activeSelf;

        public event Action<bool> OnOptionsVisibilityChanged;
        public event Action OnLeaveRequested;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSFXVolumeChanged;

        private void Awake()
        {
            _optionsButton.onClick.AddListener(OpenOptions);
            _returnButton.onClick.AddListener(CloseOptions);
            _leaveButton.onClick.AddListener(OnLeaveClicked);
            _musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(HandleSFXVolumeChanged);

            CloseOptions();
        }

        private void HandleMusicVolumeChanged(float volume)
        {
            OnMusicVolumeChanged?.Invoke(volume);
        }

        private void HandleSFXVolumeChanged(float volume)
        {
            OnSFXVolumeChanged?.Invoke(volume);
        }

        private void OpenOptions()
        {
            _optionsMenu.SetActive(true);
            OnOptionsVisibilityChanged?.Invoke(true);
        }

        private void CloseOptions()
        {
            _optionsMenu.SetActive(false);
            OnOptionsVisibilityChanged?.Invoke(false);
        }

        private void OnLeaveClicked()
        {
            OnLeaveRequested?.Invoke();
        }

        public void SetVolumes(float musicVolume, float SFXVolume)
        {
            _musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            _sfxVolumeSlider.SetValueWithoutNotify(SFXVolume);
        }

        public void SetScoreboardVisible(bool visible)
        {
            _scoreboard.SetActive(visible);
        }

        public void SetHealth(int health, int maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
            _healthSlider.value = health;
            _healthText.text = health.ToString();
        }

        public void ShowDeathScreen()
        {
            _deathScreen.SetActive(true);
        }

        public void HideDeathScreen()
        {
            _deathScreen.SetActive(false);
        }

        public void SetRespawnCountdown(int seconds)
        {
            _respawnTimer.text = seconds.ToString();
        }

        private void OnDestroy()
        {
            _optionsButton.onClick.RemoveListener(OpenOptions);
            _returnButton.onClick.RemoveListener(CloseOptions);
            _leaveButton.onClick.RemoveListener(OnLeaveClicked);
            _musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.RemoveListener(HandleSFXVolumeChanged);
        }
    }
}