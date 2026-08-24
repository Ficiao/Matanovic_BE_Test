using BETest.Networking.RoomManagement;
using BETest.UI.Controllers;
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

        public string RoomName => _roomNameInput.text;

        public event Action JoinRequested;
        public event Action<string> CreateRoomRequested;
        public event Action RefreshRequested;

        private void Awake()
        {
            _joinButton.onClick.AddListener(() => JoinRequested?.Invoke());
            _createButton.onClick.AddListener(() => CreateRoomRequested?.Invoke(_roomNameInput.text));
            _refreshButton.onClick.AddListener(() => RefreshRequested?.Invoke());

            _joinButton.interactable = false;
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