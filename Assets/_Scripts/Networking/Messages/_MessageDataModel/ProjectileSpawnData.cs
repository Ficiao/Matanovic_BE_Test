using BETest.Enum;
using BETest.Extensions;
using LiteNetLib.Utils;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public struct ProjectileSpawnData : INetSerializable
    {
        public uint ObjectID;
        public uint OwnerPID;
        public WeaponType WeaponType;

        public float SourceX;
        public float SourceY;
        public float SourceZ;

        public float DirectionX;
        public float DirectionY;
        public float DirectionZ;

        public Vector3 SourcePosition => new(SourceX, SourceY, SourceZ);
        public Vector3 Direction => new(DirectionX, DirectionY, DirectionZ);

        public ProjectileSpawnData(uint ObjectID, uint OwnerPID, WeaponType weaponType, Vector3 sourcePosition, Vector3 direction)
        {
            this.ObjectID = ObjectID;
            this.OwnerPID = OwnerPID;
            WeaponType = weaponType;

            SourceX = sourcePosition.x;
            SourceY = sourcePosition.y;
            SourceZ = sourcePosition.z;

            DirectionX = direction.x;
            DirectionY = direction.y;
            DirectionZ = direction.z;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ObjectID);
            writer.Put(OwnerPID);
            writer.Put(WeaponType);

            writer.Put(SourceX);
            writer.Put(SourceY);
            writer.Put(SourceZ);

            writer.Put(DirectionX);
            writer.Put(DirectionY);
            writer.Put(DirectionZ);
        }

        public void Deserialize(NetDataReader reader)
        {
            ObjectID = reader.GetUInt();
            OwnerPID = reader.GetUInt();
            WeaponType = reader.GetWeaponType();

            SourceX = reader.GetFloat();
            SourceY = reader.GetFloat();
            SourceZ = reader.GetFloat();

            DirectionX = reader.GetFloat();
            DirectionY = reader.GetFloat();
            DirectionZ = reader.GetFloat();
        }

        public override string ToString()
        {
            return $"ObjectID: {ObjectID}, Owner: {OwnerPID}, Weapon type: {WeaponType}";
        }
    }
}