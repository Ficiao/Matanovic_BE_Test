using BETest.Config;
using BETest.Entities;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using BETest.Scriptables;
using BETest.World.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BETest.Scriptables.WeaponDataScriptable;

namespace BETest.Networking.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Player _playerPrefab;

        private readonly Dictionary<uint, Player> _players = new();
        private ObjectPrefabsScriptable _objectPrefabs;
        private RoomManager _roomManager;
        private uint _localPID;
        private bool _hasLocalPID;
        private Player _localPlayer;
        private WeaponDataScriptable _weaponData;

        public IReadOnlyDictionary<uint, Player> Players => _players;
        public Player LocalPlayer => _localPlayer;

        public void Initialize(ObjectPrefabsScriptable objectPrefabs, RoomManager roomManager, WeaponDataScriptable waponData)
        {
            _objectPrefabs = objectPrefabs;
            _roomManager = roomManager;
            _weaponData = waponData;
        }

        public void SetLocalPID(uint localPID)
        {
            _localPID = localPID;
            _hasLocalPID = true;
        }

        public Player SpawnPlayer(NetworkEntitySpawnData data)
        {
            uint pid = data.StateData.ObjectID;
            if (_players.TryGetValue(pid, out Player existingPlayer)) return existingPlayer;

            bool hasStateAuthority = _hasLocalPID && data.StateData.StateAuthorityPID == _localPID;
            Vector3 position = new Vector3(Mathf.HalfToFloat(data.StateData.X), Mathf.HalfToFloat(data.StateData.Y), GameConfig.OBJECT_Z_POSITION);

            CharacterModelController modelPrefab = _objectPrefabs.GetPrefab(data.PrefabType).GetComponent<CharacterModelController>();

            WeaponData weaponData = _weaponData.GetWeaponData(data.ClientPlayerData.PlayerWeaponType);
            Player player = Instantiate(_playerPrefab, position, Quaternion.identity);
            player.Init(data, hasStateAuthority, modelPrefab, weaponData);

            _players.Add(pid, player);
            if (hasStateAuthority)
            {
                _localPlayer = player;
                _roomManager.LocalPlayerReady();
            }

            return player;
        }

        public void DespawnPlayer(uint pid)
        {
            if (!_players.Remove(pid, out Player player)) return;

            if (_localPlayer == player) _localPlayer = null;

            Destroy(player.gameObject);
        }

        public void HandleState(NetworkEntityStateData state)
        {
            if (!_players.TryGetValue(state.ObjectID, out Player player)) return;

            player.HandleServerStateUpdate(state);
        }
    }
}