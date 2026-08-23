using BETest.Config;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagment;
using BETest.UI.Views;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.UI.Controllers
{
    public class SessionBrowserController : MonoBehaviour
    {
        [SerializeField] private SessionBrowserView _view;

        private readonly List<RoomInfo> _rooms = new();

        private SessionEntryView _selectedEntry;
        private RoomInfo _selectedRoom;

        private void OnEnable()
        {
            _view.RefreshRequested += Refresh;
            _view.JoinRequested += JoinRoom;
            _view.CreateRoomRequested += CreateRoom;

            LanDiscovery.RoomDiscovered += OnRoomDiscovered;
            RoomManager.StateChanged += OnRoomStateChanged;

            LoginController.LoginSucceeded += _view.Show;
        }

        private void OnDisable()
        {
            _view.RefreshRequested -= Refresh;
            _view.JoinRequested -= JoinRoom;
            _view.CreateRoomRequested -= CreateRoom;

            LanDiscovery.RoomDiscovered -= OnRoomDiscovered;
            RoomManager.StateChanged -= OnRoomStateChanged;

            LoginController.LoginSucceeded -= _view.Show;
        }

        private void Refresh()
        {
            _rooms.Clear();
            _selectedRoom = null;
            _selectedEntry = null;

            _view.ClearSessions();
            _view.SetJoinInteractable(false);

            RoomManager.Instance.BrowseRooms();
        }

        private void OnRoomDiscovered(RoomInfo room)
        {
            _rooms.Add(room);

            SessionEntryView entry = _view.AddSession(room);
            entry.SelectionChanged += OnEntrySelectionChanged;
        }

        private void OnEntrySelectionChanged(SessionEntryView entry, bool selected)
        {
            if (!selected)
            {
                if (_selectedEntry == entry)
                {
                    _selectedEntry = null;
                    _selectedRoom = null;
                    _view.SetJoinInteractable(false);
                }

                return;
            }

            if (_selectedEntry != null && _selectedEntry != entry) _selectedEntry.SetSelected(false);

            _selectedEntry = entry;
            _selectedRoom = entry.Room;

            _view.SetJoinInteractable(true);
        }

        private void JoinRoom()
        {
            if (_selectedRoom == null) return;

            RoomManager.Instance.JoinRoom(_selectedRoom);
        }

        private void CreateRoom(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName) || roomName.Length < GameConfig.MIN_ROOM_NAME_LENGTH || roomName.Length > GameConfig.MAX_ROOM_NAME_LENGTH)
                return;

            RoomManager.Instance.CreateRoom(roomName);
        }

        private void OnRoomStateChanged(RoomStateType state)
        {
            bool waiting = state == RoomStateType.Joining || state == RoomStateType.Creating;

            _view.SetInteractionEnabled(!waiting);
            if (!waiting) _view.SetJoinInteractable(_selectedRoom != null);
        }
    }
}