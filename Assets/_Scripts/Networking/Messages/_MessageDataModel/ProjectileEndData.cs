using BETest.Config;
using BETest.Enum;
using LiteNetLib.Utils;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public struct ProjectileEndData : INetSerializable
    {
        public uint ObjectID;

        public float X;
        public float Y;

        public Vector3 Position => new(X, Y, GameConfig.OBJECT_Z_POSITION);

        public ProjectileEndData(uint ObjectID, Vector3 position)
        {
            this.ObjectID = ObjectID;

            X = position.x;
            Y = position.y;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ObjectID);
            writer.Put(X);
            writer.Put(Y);
        }

        public void Deserialize(NetDataReader reader)
        {
            ObjectID = reader.GetUInt();
            X = reader.GetFloat();
            Y = reader.GetFloat();
        }

        public override string ToString()
        {
            return $"ObjectID: {ObjectID}, Position: {X}, {Y}";
        }
    }
}