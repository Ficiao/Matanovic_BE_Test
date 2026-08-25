using UnityEngine;

namespace BETest.World.Visuals
{
    public class CharacterModelController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
    }
}