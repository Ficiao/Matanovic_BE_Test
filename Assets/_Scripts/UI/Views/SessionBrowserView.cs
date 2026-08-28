using BETest.Enum;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BETest.UI.Views
{
    public class SessionBrowserView : MonoBehaviour
    {
        [SerializeField] private Transform _sessionContainer;
        [SerializeField] private SessionEntryView _sessionPrefab;

        [SerializeField] private TMP_InputField _roomNameInput;

        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _refreshButton;

        [SerializeField] private WeaponSelectionToggle _weaponSelectionTogglePrefab;
        [SerializeField] private ToggleGroup _weaponSelectionContainer;
        [SerializeField] private Toggle _maleCharacterToggle;
        [SerializeField] private Toggle _femaleCharacterToggle;

        public string RoomName => _roomNameInput.text;

        public event Action OnJoinRequested;
        public event Action<string> OnCreateRoomRequested;
        public event Action OnRefreshRequested;
        public event Action<WeaponType> OnWeaponSelected;
        public event Action<PlayerCharacterType> OnCharacterSelected;

        private void Awake()
        {
            _joinButton.onClick.AddListener(() => OnJoinRequested?.Invoke());
            _createButton.onClick.AddListener(() => OnCreateRoomRequested?.Invoke(_roomNameInput.text));
            _refreshButton.onClick.AddListener(() => OnRefreshRequested?.Invoke());

            _joinButton.interactable = false;
        }

        public void InitializeCharacterSelection(WeaponDataScriptable weaponData)
        {
            foreach (var data in weaponData.Weapons)
            {
                WeaponSelectionToggle toggle = Instantiate(_weaponSelectionTogglePrefab, _weaponSelectionContainer.transform);
                toggle.ShowWeapon(data.WeaponType, _weaponSelectionContainer, data.WeaponImage, weaponType => OnWeaponSelected?.Invoke(weaponType));
            }

            _maleCharacterToggle.onValueChanged.AddListener(isOn => { if (isOn) OnCharacterSelected?.Invoke(PlayerCharacterType.Male); });
            _femaleCharacterToggle.onValueChanged.AddListener(isOn => { if (isOn) OnCharacterSelected?.Invoke(PlayerCharacterType.Female); });

            _maleCharacterToggle.isOn = true;
        }

        public SessionEntryView AddSession(RoomInfo room)
        {
            var entry = Instantiate(_sessionPrefab, _sessionContainer);
            entry.SetRoom(room);
            return entry;
        }

        public void ClearSessions()
        {
            foreach (Transform child in _sessionContainer) Destroy(child.gameObject);
        }

        public void SetJoinInteractable(bool interactable)
        {
            _joinButton.interactable = interactable;
        }

        public void SetInteractionEnabled(bool enabled)
        {
            _createButton.interactable = enabled;
            _refreshButton.interactable = enabled;

            if (!enabled) _joinButton.interactable = false;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}