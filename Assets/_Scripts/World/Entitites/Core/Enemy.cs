using BETest.Config;
using BETest.Enum;
using BETest.Flags;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using UnityEngine;

namespace BETest.Entities
{
    public class Enemy : NetworkEntity
    {
        [SerializeField] private ObjectPrefabType _prefabType;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _interpolationSpeed = 15f;
        [SerializeField] private int _spawnWeight = 1;
        [SerializeField] private int _maxHealth = 30;
        [SerializeField] private int _contactDamage = 20;
        [SerializeField] private BoxCollider _hitCollider;
        [SerializeField] private LayerMask _playerMask;
        private readonly Collider[] _playerHitBuffer = new Collider[GameConfig.MAX_PLAYERS_PER_ROOM];

        private PlayerManager _playerManager;
        private Vector3 _targetPosition;
        private EnemyManager _enemyManager;
        private int _health;
        private bool _dead;

        public int Health => _health;
        public int MaxHealth => _maxHealth;
        public bool IsDead => _dead;
        public int SpawnWeight => _spawnWeight;
        public ObjectPrefabType PrefabType => _prefabType;

        public void Init(NetworkEntitySpawnData data, bool hasStateAuthority, PlayerManager playerManager, EnemyManager enemyManager)
        {
            base.Init(data, hasStateAuthority);

            _playerManager = playerManager;
            _enemyManager = enemyManager;

            _targetPosition = transform.position;
            _health = _maxHealth;
            _dead = false;
        }

        public override void HandleTick()
        {
            if (!HasStateAuthority) return;

            Player target = GetNearestPlayer();
            if (target == null) return;

            Vector3 direction = target.transform.position - transform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude <= 0.001f) return;

            transform.position += direction.normalized * _moveSpeed * GameConfig.TICK_DELTA;
            UpdatePositionStateFromTransform();
            HandlePlayerContact();
        }

        public override void HandleServerStateUpdate(NetworkEntityStateData state)
        {
            base.HandleServerStateUpdate(state);

            if (HasStateAuthority) return;

            if ((state.UpdateFlags & EntityUpdateFlags.Position) != 0)
            {
                _targetPosition = new Vector3(
                    Mathf.HalfToFloat(_entityState.X),
                    Mathf.HalfToFloat(_entityState.Y),
                    GameConfig.OBJECT_Z_POSITION
                );
            }
        }

        private void Update()
        {
            if (HasStateAuthority) return;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _interpolationSpeed);
        }

        public void TakeDamage(int damage, uint SourcePID)
        {
            if (!HasStateAuthority || _dead || damage <= 0) return;

            _health = Mathf.Max(0, _health - damage);

            if (_health <= 0) Die(SourcePID);
        }

        private void HandlePlayerContact()
        {
            Bounds bounds = _hitCollider.bounds;

            int hitCount = Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents,
                _playerHitBuffer,
                Quaternion.identity,
                _playerMask,
                QueryTriggerInteraction.Collide
            );

            for (int i = 0; i < hitCount; i++)
            {
                Player player = _playerHitBuffer[i].GetComponentInParent<Player>();
                if (player == null) continue;

                if (_enemyManager.TryDamagePlayer(player.ObjectID, _contactDamage))
                {
                    Die();
                    return;
                }
            }
        }

        private void Die()
        {
            if (_dead) return;

            _dead = true;
            _enemyManager.KillEnemy(this);
        }

        private void Die(uint KillerPID)
        {
            if (_dead) return;

            _dead = true;
            _enemyManager.KillEnemy(this, KillerPID);
        }

        private Player GetNearestPlayer()
        {
            Player nearestPlayer = null;
            float nearestDistance = float.MaxValue;

            foreach (Player player in _playerManager.Players.Values)
            {
                if (player == null || !player.gameObject.activeInHierarchy) continue;

                Vector3 direction = player.transform.position - transform.position;
                direction.z = 0f;

                float distance = direction.sqrMagnitude;
                if (distance >= nearestDistance) continue;

                nearestDistance = distance;
                nearestPlayer = player;
            }

            return nearestPlayer;
        }

        public void ResetForPool()
        {
            _playerManager = null;
            _enemyManager = null;
            _targetPosition = Vector3.zero;
            _health = _maxHealth;
            _dead = false;
        }
    }
}