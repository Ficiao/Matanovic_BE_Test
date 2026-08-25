using BETest.Enum;
using UnityEngine;

namespace BETest.Entities
{
    public class LocalPlayerSession : MonoBehaviour
    {
        public string Username { get; private set; }
        public WeaponType WeaponType { get; private set; } 
        public PlayerCharacterType CharacterType { get; private set; }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void SetWeapon(WeaponType weaponType)
        {
            WeaponType = weaponType;
        }

        public void SetCharacter(PlayerCharacterType characterType)
        {
            CharacterType = characterType;
        }

        public void ResetData()
        {
            Username = null;
            WeaponType = default;
            CharacterType = default;
        }
    }
}