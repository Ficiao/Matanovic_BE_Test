using BETest.Networking.RoomManagement;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BETest.UI.Views
{
    public class SessionEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _playerCount;
        [SerializeField] private TMP_Text _maxPlayers;
        [SerializeField] private Toggle _toggle;

        private RoomInfo _room;
        public RoomInfo Room => _room;
        public event Action<SessionEntryView, bool> OnSelectionChanged;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener((value) => OnSelectionChanged?.Invoke(this, value));
        }

        public void SetRoom(RoomInfo room)
        {
            _room = room;
            _roomName.text = room.Name;
            _playerCount.text = room.PlayerCount.ToString();
            _maxPlayers.text = room.MaxPlayers.ToString();
        }

        public void SetSelected(bool selected)
        {
            _toggle.SetIsOnWithoutNotify(selected);
        }
    }
}