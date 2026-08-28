using UnityEngine;

namespace BETest.Entities
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPosition;

        [Header("Enemy Spawning")]
        [SerializeField] private float _enemySpawnInterval = 0.5f;
        [SerializeField] private float _enemyMinSpawnDistance = 12f;
        [SerializeField] private float _enemyMaxSpawnDistance = 20f;
        [SerializeField] private int _enemySpawnPositionAttempts = 10;
        [SerializeField] private int _maxAliveEnemies = 100;

        public Transform PlayerSpawnPosition => _playerSpawnPosition;

        public float EnemySpawnInterval => _enemySpawnInterval;
        public float EnemyMinSpawnDistance => _enemyMinSpawnDistance;
        public float EnemyMaxSpawnDistance => _enemyMaxSpawnDistance;
        public int EnemySpawnPositionAttempts => _enemySpawnPositionAttempts;
        public int MaxAliveEnemies => _maxAliveEnemies;
    }
}