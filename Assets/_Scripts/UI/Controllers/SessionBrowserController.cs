using BETest.Config;
using BETest.Entities;
using BETest.Enum;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using BETest.UI.Views;
using System.Collections;
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
        private string _roomIDToReselect;
        private Coroutine _autoRefreshCoroutine;

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
            if (_autoRefreshCoroutine != null)
            {
                StopCoroutine(_autoRefreshCoroutine);
                _autoRefreshCoroutine = null;
            }
        }

        private void Refresh()
        {
            _roomIDToReselect = _selectedRoom?.Id;

            _rooms.Clear();
            _selectedRoom = null;
            _selectedEntry = null;

            _view.ClearSessions();
            _view.SetJoinInteractable(false);

            _roomManager.BrowseRooms();
        }

        private void OnRoomDiscovered(RoomInfo room)
        {
            if (room.PlayerCount == room.MaxPlayers) return;
            if (_rooms.Exists(existingRoom => existingRoom.Id == room.Id)) return;
            _rooms.Add(room);

            SessionEntryView entry = _view.AddSession(room);
            entry.SelectionChanged += OnEntrySelectionChanged;

            if (room.Id == _roomIDToReselect)
            {
                SelectEntry(entry);
                _roomIDToReselect = null;
            }
        }

        private void SelectEntry(SessionEntryView entry)
        {
            if (_selectedEntry != null && _selectedEntry != entry) _selectedEntry.SetSelected(false);

            _selectedEntry = entry;
            _selectedRoom = entry.Room;

            entry.SetSelected(true);
            _view.SetJoinInteractable(true);
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

            SelectEntry(entry);
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
            if (state == RoomStateType.InRoomHost || state == RoomStateType.InRoomClient) return;
            bool waiting = state == RoomStateType.Joining || state == RoomStateType.Creating;

            _view.SetInteractionEnabled(!waiting);
            if (!waiting) _view.SetJoinInteractable(_selectedRoom != null);
        }

        private void OnLoginSucceeded()
        {
            _view.Show();
            Refresh();

            if (_autoRefreshCoroutine == null) _autoRefreshCoroutine = StartCoroutine(AutoRefresh());
        }

        private IEnumerator AutoRefresh()
        {
            WaitForSecondsRealtime wait = new(GameConfig.ROOM_AUTO_REFRESH_INTERVAL);

            while (true)
            {
                yield return wait;

                if (_roomManager.State == RoomStateType.Idle) Refresh();
            }
        }
    }
}