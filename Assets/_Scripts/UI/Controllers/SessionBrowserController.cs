using BETest.Config;
using BETest.Entities;
using BETest.Enum;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using BETest.UI.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BETest.UI.Controllers
{
    public class SessionBrowserController : MonoBehaviour
    {
        [SerializeField] private SessionBrowserView _view;
        private LoginController _loginController;
        private RoomManager _roomManager;
        private LanDiscovery _lanDiscovery;
        private readonly List<RoomInfo> _rooms = new();
        private SessionEntryView _selectedEntry;
        private RoomInfo _selectedRoom;
        private LocalPlayerSession _localPlayerSession;

        public void Initialize(RoomManager roomManager, LanDiscovery lanDiscovery, WeaponDataScriptable weaponData, LocalPlayerSession localPlayerSession, LoginController loginController)
        {
            _roomManager = roomManager;
            _lanDiscovery = lanDiscovery;
            _localPlayerSession = localPlayerSession;
            _loginController = loginController;

            _view.WeaponSelected += _localPlayerSession.SetWeapon;
            _view.CharacterSelected += _localPlayerSession.SetCharacter;
            _lanDiscovery.RoomDiscovered += OnRoomDiscovered;
            _roomManager.StateChanged += OnRoomStateChanged;
            _loginController.LoginSucceeded += OnLoginSucceeded;


            _view.InitializeCharacterSelection(weaponData);
        }

        private void OnEnable()
        {
            _view.RefreshRequested += Refresh;
            _view.JoinRequested += JoinRoom;
            _view.CreateRoomRequested += CreateRoom;
        }

        private void OnDisable()
        {
            _view.RefreshRequested -= Refresh;
            _view.JoinRequested -= JoinRoom;
            _view.CreateRoomRequested -= CreateRoom;
        }

        private void Refresh()
        {
            _rooms.Clear();
            _selectedRoom = null;
            _selectedEntry = null;

            _view.ClearSessions();
            _view.SetJoinInteractable(false);

            _roomManager.BrowseRooms();
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

            _roomManager.JoinRoom(_selectedRoom);
        }

        private void CreateRoom(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName) || roomName.Length < GameConfig.MIN_ROOM_NAME_LENGTH || roomName.Length > GameConfig.MAX_ROOM_NAME_LENGTH)
                return;

            _roomManager.CreateRoom(roomName);
        }

        private void OnRoomStateChanged(RoomStateType state)
        {
            bool waiting = state == RoomStateType.Joining || state == RoomStateType.Creating;

            _view.SetInteractionEnabled(!waiting);
            if (!waiting) _view.SetJoinInteractable(_selectedRoom != null);
        }

        private void OnLoginSucceeded()
        {
            _view.Show();
            Refresh();
        }
    }
}