using BETest.Config;
using BETest.Entities;
using BETest.Enum;
using BETest.Flags;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using BETest.Networking.Services;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Managers
{
    public class NetworkPlayerStateManager
    {
        private readonly Dictionary<uint, NetworkEntityStateData> _states = new();
        private readonly Dictionary<uint, ClientPlayerData> _playerDatas = new();
        private readonly Dictionary<uint, EntityUpdateFlags> _dirtyFlags = new();
        private readonly Dictionary<uint, int> _healths = new();
        private readonly Dictionary<uint, float> _respawnTimers = new();
        private readonly List<uint> _respawnPIDs = new();
        private readonly Dictionary<uint, int> _kills = new();
        private SpawnManager _spawnManager;
        private int _worldSeed;

        public bool HasPlayers => _states.Count > 0;

        public void Initialize(SpawnManager spawnManager, int worldSeed)
        {
            _spawnManager = spawnManager;
            _worldSeed = worldSeed;
        }

        public void GetPlayerPositions(List<Vector2> positions)
        {
            foreach (NetworkEntityStateData state in _states.Values)
            {
                positions.Add(new Vector2(Mathf.HalfToFloat(state.X), Mathf.HalfToFloat(state.Y)
                ));
            }
        }

        public NetworkEntitySpawnData PlayerConnected(NetPeer peer, ConnectRequestData request)
        {
            uint PID = (uint)peer.Id;
            Vector3 spawnPosition = _spawnManager.PlayerSpawnPosition.position;
            
            NetworkEntityStateData state = new NetworkEntityStateData
            {
                ObjectID = PID,
                StateAuthorityPID = PID,
                EntityType = EntityType.Player,
                X = Mathf.FloatToHalf(spawnPosition.x),
                Y = Mathf.FloatToHalf(spawnPosition.y),
            };

            ClientPlayerData playerData = new ClientPlayerData
            {
                PID = PID,
                PlayerName = request.PlayerName,
                PlayerWeaponType = request.PlayerWeaponType,
                PlayerCharacterType = request.PlayerCharacterType,
            };

            _states.Add(PID, state);
            _playerDatas.Add(PID, playerData);
            _healths[PID] = GameConfig.PLAYER_MAX_HEALTH;
            _kills[PID] = 0;

            ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;

            List<NetworkEntitySpawnData> existingPlayers = new(GetSpawnData());

            NetworkEntitySpawnData newPlayerSpawn = new(prefabType, state, playerData);
            
            ConnectAcceptMessage acceptMessage = new ConnectAcceptMessage()
            {
                PlayerData = playerData,
                WorldSeed = _worldSeed,
            };

            NetworkServer.SendMessage(acceptMessage, peer, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
            NetworkSpawnService.SendSpawn(peer, existingPlayers);
            NetworkSpawnService.BroadcastSpawnExclude(peer, newPlayerSpawn);

            foreach ((uint ExistingPID, int kills) in _kills)
            {
                ClientPlayerData existingPlayerData = _playerDatas[ExistingPID];

                NetworkScoreService.SendScore(peer, new PlayerScoreData(ExistingPID, existingPlayerData.PlayerName, kills)
                );
            }
            NetworkScoreService.BroadcastScore(new PlayerScoreData(PID, playerData.PlayerName, 0));

            return newPlayerSpawn;
        }

        public void HandleTick()
        {
            _respawnPIDs.Clear();

            foreach (uint PID in _respawnTimers.Keys)
            {
                _respawnPIDs.Add(PID);
            }

            foreach (uint PID in _respawnPIDs)
            {
                float remaining = _respawnTimers[PID] - GameConfig.TICK_DELTA;

                if (remaining <= 0f)
                    RespawnPlayer(PID);
                else
                    _respawnTimers[PID] = remaining;
            }
        }

        private void RespawnPlayer(uint PID)
        {
            if (!_playerDatas.TryGetValue(PID, out ClientPlayerData playerData)) return;

            Vector3 spawnPosition = _spawnManager.PlayerSpawnPosition.position;

            NetworkEntityStateData state = new()
            {
                ObjectID = PID,
                StateAuthorityPID = PID,
                EntityType = EntityType.Player,
                X = Mathf.FloatToHalf(spawnPosition.x),
                Y = Mathf.FloatToHalf(spawnPosition.y),
            };

            _states.Add(PID, state);
            _healths[PID] = GameConfig.PLAYER_MAX_HEALTH;
            _respawnTimers.Remove(PID);

            ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male
                ? ObjectPrefabType.PlayerMale
                : ObjectPrefabType.PlayerFemale;

            NetworkSpawnService.BroadcastSpawn(new NetworkEntitySpawnData(prefabType, state, playerData));
        }

        public void PlayerDisconnected(uint PID)
        {
            _states.Remove(PID);
            _playerDatas.Remove(PID);
            _dirtyFlags.Remove(PID);
            _healths.Remove(PID);
            _respawnTimers.Remove(PID);
            _kills.Remove(PID);
            NetworkScoreService.BroadcastScoreRemoved(PID);
        }

        public bool DamagePlayer(uint PID, int damage)
        {
            if (damage <= 0) return false;
            if (!_states.ContainsKey(PID)) return false;
            if (!_healths.TryGetValue(PID, out int health)) return false;

            health = Mathf.Max(0, health - damage);
            _healths[PID] = health;

            NetworkStateBroadcastService.BroadcastPlayerHealth(new PlayerHealthData(PID, health));

            if (health <= 0) KillPlayer(PID, false);

            return true;
        }

        public void AddKill(uint PID)
        {
            if (!_kills.TryGetValue(PID, out int kills)) return;
            if (!_playerDatas.TryGetValue(PID, out ClientPlayerData playerData)) return;

            kills++;
            _kills[PID] = kills;

            NetworkScoreService.BroadcastScore(new PlayerScoreData(PID, playerData.PlayerName, kills));
        }

        private void KillPlayer(uint PID, bool broadcastHealth)
        {
            if (!_states.Remove(PID)) return;

            _dirtyFlags.Remove(PID);
            _healths[PID] = 0;
            _respawnTimers[PID] = GameConfig.PLAYER_RESPAWN_DELAY;

            if (broadcastHealth) NetworkStateBroadcastService.BroadcastPlayerHealth(new PlayerHealthData(PID, 0));

            NetworkSpawnService.BroadcastDespawn(new NetworkEntityDespawnData(PID, EntityType.Player));
        }

        public bool TryAcceptMove(uint PID, PlayerMoveData moveData)
        {
            if (!_states.TryGetValue(PID, out NetworkEntityStateData state)) return false;
            if (moveData.Seq <= state.SeqAcc) return false;
            if (!ValidateMove(state, moveData)) return false;

            EntityUpdateFlags flags = EntityUpdateFlags.None;

            //Debug.Log(moveData.Directions);

            if (state.X != moveData.X || state.Y != moveData.Y)
            {
                state.X = moveData.X;
                state.Y = moveData.Y;
                flags |= EntityUpdateFlags.Position;
            }

            if (state.Directions != moveData.Directions)
            {
                state.Directions = moveData.Directions;
                flags |= EntityUpdateFlags.MoveDir;
            }

            if (state.AimAngle != moveData.AimAngle)
            {
                state.AimAngle = moveData.AimAngle;
                flags |= EntityUpdateFlags.Aim;
            }

            state.SeqAcc = moveData.Seq;
            _states[PID] = state;

            if (Mathf.HalfToFloat(state.Y) < GameConfig.PLAYER_DEATH_Y)
            {
                KillPlayer(PID, true);
                return true;
            }

            if (flags == EntityUpdateFlags.None) return true;

            //Debug.Log($"New state: {state}, update flags: {flags}");

            if (_dirtyFlags.TryGetValue(PID, out EntityUpdateFlags dirtyFlags))
                _dirtyFlags[PID] = dirtyFlags | flags;
            else
                _dirtyFlags.Add(PID, flags);

            return true;
        }

        private bool ValidateMove(NetworkEntityStateData previousState, PlayerMoveData moveData)
        {
            // Ovdje bi obično isšla validacija legalnosti promjene pozicije po pravilima igre.
            // Za potrebe ovog demo projekta, state se uvjek smatra validnim.

            return true;
        }

        public void GetDirtyStates(List<NetworkEntityStateData> states)
        {
            foreach ((uint PID, EntityUpdateFlags flags) in _dirtyFlags)
            {
                if (!_states.TryGetValue(PID, out NetworkEntityStateData state)) continue;

                state.UpdateFlags = flags;
                states.Add(state);
            }

            _dirtyFlags.Clear();
        }

        public IEnumerable<NetworkEntitySpawnData> GetSpawnData()
        {
            foreach ((uint PID, NetworkEntityStateData state) in _states)
            {
                ClientPlayerData playerData = _playerDatas[PID];
                ObjectPrefabType prefabType = playerData.PlayerCharacterType == PlayerCharacterType.Male ? ObjectPrefabType.PlayerMale : ObjectPrefabType.PlayerFemale;

                yield return new NetworkEntitySpawnData(prefabType, state, playerData);
            }
        }

        public bool TryGetPlayerData(uint PID, out ClientPlayerData playerData)
        {
            return _playerDatas.TryGetValue(PID, out playerData);
        }
    }
}