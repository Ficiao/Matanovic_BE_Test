using BETest.Entities;
using BETest.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "WeaponDataScriptable", menuName = "Scriptables/Weapon Data Scriptable")]
    public class WeaponDataScriptable : ScriptableObject
    {
        [Serializable]
        public class WeaponData
        {
            public WeaponType WeaponType;
            public Sprite WeaponImage;
            public Projectile WeaponPrefab;
            public float FireRate;
            public int Damage;
        }

        [SerializeField] private List<WeaponData> _weapons;
        public List<WeaponData> Weapons => _weapons;

        public WeaponData GetWeaponData(WeaponType weaponType)
        {
            return _weapons.First(weapon => weapon.WeaponType == weaponType);
        }
    }
}