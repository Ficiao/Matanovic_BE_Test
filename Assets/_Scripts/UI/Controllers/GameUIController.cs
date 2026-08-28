using BETest.Config;
using BETest.Entities;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using BETest.Scriptables;
using BETest.UI.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BETest.UI.Controllers
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private GameUIView _view;
        private VolumeSettingsScriptable _volumeSettings;
        private readonly Dictionary<uint, PlayerScoreboardDataView> _scoreEntries = new();
        private PlayerManager _playerManager;
        private Coroutine _deathCountdown;

        public event Action OnLeaveRequested;

        public void Initialize(PlayerManager playerManager, VolumeSettingsScriptable volumeSettings)
        {
            _playerManager = playerManager;
            _volumeSettings = volumeSettings;

            _playerManager.OnLocalPlayerHealthChanged += OnLocalPlayerHealthChanged;
            _playerManager.OnLocalPlayerDied += OnLocalPlayerDied;
            _playerManager.OnLocalPlayerSpawned += OnLocalPlayerSpawned;

            _view.SetHealth(GameConfig.PLAYER_MAX_HEALTH, GameConfig.PLAYER_MAX_HEALTH);
            _view.HideDeathScreen();
            _view.OnLeaveRequested += OnGameLeaveRequested;
            _view.OnOptionsVisibilityChanged += OnOptionsVisibilityChanged;
            _view.SetVolumes(_volumeSettings.MusicVolume, _volumeSettings.SFXVolume);
            _view.OnMusicVolumeChanged += OnMusicVolumeChanged;
            _view.OnSFXVolumeChanged += OnSFXVolumeChanged;

            _playerManager.OnPlayerScoreChanged += OnPlayerScoreChanged;
            _playerManager.OnPlayerScoreRemoved += OnPlayerScoreRemoved;

            _view.SetScoreboardVisible(false);

            foreach (PlayerScoreData score in _playerManager.Scores.Values)
            {
                OnPlayerScoreChanged(score);
            }
        }

        private void Update()
        {
            bool scoreboardVisible = Keyboard.current != null && Keyboard.current.tabKey.isPressed;
            _view.SetScoreboardVisible(scoreboardVisible);
        }

        private void OnOptionsVisibilityChanged(bool visible)
        {
            _playerManager.LocalPlayer?.SetInputEnabled(!visible);
        }

        private void OnMusicVolumeChanged(float volume)
        {
            _volumeSettings.SetMusicVolume(volume);
        }

        private void OnSFXVolumeChanged(float volume)
        {
            _volumeSettings.SetSFXVolume(volume);
        }

        private void OnPlayerScoreChanged(PlayerScoreData data)
        {
            if (_scoreEntries.TryGetValue(data.PID, out PlayerScoreboardDataView existingEntry))
            {
                existingEntry.SetData(data);
                return;
            }

            PlayerScoreboardDataView entry = Instantiate(
                _view.PlayerScoreboardDataPrefab,
                _view.ScoreboardDataContainer
            );

            entry.SetData(data);
            _scoreEntries.Add(data.PID, entry);
        }

        private void OnPlayerScoreRemoved(uint PID)
        {
            if (!_scoreEntries.Remove(PID, out PlayerScoreboardDataView entry)) return;

            Destroy(entry.gameObject);
        }

        private void OnLocalPlayerHealthChanged(int health, int maxHealth)
        {
            _view.SetHealth(health, maxHealth);
        }

        private void OnLocalPlayerDied()
        {
            if (_deathCountdown != null) StopCoroutine(_deathCountdown);

            _view.ShowDeathScreen();
            _deathCountdown = StartCoroutine(DeathCountdown());
        }

        private void OnLocalPlayerSpawned(Player player)
        {
            if (_deathCountdown != null)
            {
                StopCoroutine(_deathCountdown);
                _deathCountdown = null;
            }

            _view.HideDeathScreen();
            _view.SetHealth(player.Health, player.MaxHealth);
        }

        private IEnumerator DeathCountdown()
        {
            float remaining = GameConfig.PLAYER_RESPAWN_DELAY;

            while (remaining > 0f)
            {
                _view.SetRespawnCountdown(Mathf.CeilToInt(remaining));
                yield return null;
                remaining -= Time.unscaledDeltaTime;
            }

            _view.SetRespawnCountdown(0);
        }

        private void OnGameLeaveRequested()
        {
            OnLeaveRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (_playerManager == null) return;

            _playerManager.OnLocalPlayerHealthChanged -= OnLocalPlayerHealthChanged;
            _playerManager.OnLocalPlayerDied -= OnLocalPlayerDied;
            _playerManager.OnLocalPlayerSpawned -= OnLocalPlayerSpawned;
            _view.OnLeaveRequested -= OnGameLeaveRequested;
            _playerManager.OnPlayerScoreChanged -= OnPlayerScoreChanged;
            _playerManager.OnPlayerScoreRemoved -= OnPlayerScoreRemoved;
            _view.OnOptionsVisibilityChanged -= OnOptionsVisibilityChanged;
            _view.OnMusicVolumeChanged -= OnMusicVolumeChanged;
            _view.OnSFXVolumeChanged -= OnSFXVolumeChanged;
        }
    }
}