using BETest.Networking.Managers;
using UnityEngine;

namespace BETest.World
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _terrainContainer;
        [SerializeField] private GameObject _startTerrainPrefab;
        [SerializeField] private GameObject[] _terrainPrefabs;
        [SerializeField] private float _chunkWidth = 20f;
        [SerializeField] private int _initialChunksPerSide = 3;
        [SerializeField] private int _chunksAhead = 3;

        private System.Random _leftRandom;
        private System.Random _rightRandom;
        private Transform _target;

        private int _leftmostChunkIndex;
        private int _rightmostChunkIndex;
        private bool _initialized;

        public void Initialize(int seed, PlayerManager playerManager)
        {
            if (_initialized) return;

            playerManager.OnLocalPlayerSpawned += (player) => SetTarget(player.transform);

            _leftRandom = new System.Random(seed);
            _rightRandom = new System.Random(seed + 1);

            _leftmostChunkIndex = 0;
            _rightmostChunkIndex = 0;

            SpawnChunk(0, _startTerrainPrefab);

            for (int i = 0; i < _initialChunksPerSide; i++)
            {
                GenerateLeft();
                GenerateRight();
            }

            _initialized = true;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (!_initialized || _target == null) return;

            int targetChunkIndex = Mathf.FloorToInt((_target.position.x - _terrainContainer.position.x + _chunkWidth * 0.5f) / _chunkWidth);

            while (_rightmostChunkIndex < targetChunkIndex + _chunksAhead) GenerateRight();
            while (_leftmostChunkIndex > targetChunkIndex - _chunksAhead) GenerateLeft();
        }

        private void GenerateRight()
        {
            _rightmostChunkIndex++;
            int prefabIndex = _rightRandom.Next(_terrainPrefabs.Length);
            SpawnChunk(_rightmostChunkIndex, _terrainPrefabs[prefabIndex]);
        }

        private void GenerateLeft()
        {
            _leftmostChunkIndex--;
            int prefabIndex = _leftRandom.Next(_terrainPrefabs.Length);
            SpawnChunk(_leftmostChunkIndex, _terrainPrefabs[prefabIndex]);
        }

        private void SpawnChunk(int chunkIndex, GameObject prefab)
        {
            Vector3 position = _terrainContainer.position + Vector3.right * chunkIndex * _chunkWidth;
            Instantiate(prefab, position, Quaternion.identity, _terrainContainer);
        }
    }
}