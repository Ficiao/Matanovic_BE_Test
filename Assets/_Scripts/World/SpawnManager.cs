using UnityEngine;

namespace BETest.Entities
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPosition;

        public Transform PlayerSpawnPosition => _playerSpawnPosition;
    }
}