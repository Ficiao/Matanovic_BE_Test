using BETest.Enum;
using BETest.Flags;
using LiteNetLib.Utils;
using UnityEngine;

namespace BETest.Extensions
{
    public static class SerializingExtensions
    {
        //Vector2 serializer
        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }

        //EntityType serializer
        public static void Put(this NetDataWriter writer, EntityType entityType)
        {
            writer.Put((byte)entityType);
        }

        public static EntityType GetEntityType(this NetDataReader reader)
        {
            return (EntityType)reader.GetByte();
        }

        //PlayerCharacterType serializer 
        public static void Put(this NetDataWriter writer, PlayerCharacterType objectPrefabType)
        {
            writer.Put((ushort)objectPrefabType);
        }

        public static PlayerCharacterType GetPlayerCharacterType(this NetDataReader reader)
        {
            return (PlayerCharacterType)reader.GetUShort();
        }

        //ObjectPrefabType serializer
        public static ObjectPrefabType GetObjectPrefabType(this NetDataReader reader)
        {
            return (ObjectPrefabType)reader.GetUShort();
        }

        public static void Put(this NetDataWriter writer, ObjectPrefabType objectPrefabType)
        {
            writer.Put((ushort)objectPrefabType);
        }

        //WeaponType serializer
        public static void Put(this NetDataWriter writer, WeaponType weaponType)
        {
            writer.Put((ushort)weaponType);
        }

        public static WeaponType GetWeaponType(this NetDataReader reader)
        {
            return (WeaponType)reader.GetUShort();
        }

        //MoveDirFlags serializer 
        public static void Put(this NetDataWriter writer, MoveDirFlags moveDirFlags)
        {
            writer.Put((ushort)moveDirFlags);
        }

        public static MoveDirFlags GetMoveDirFlags(this NetDataReader reader)
        {
            return (MoveDirFlags)reader.GetUShort();
        }
    }
}