using BETest.Config;
using LiteNetLib.Utils;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public struct PlayerShootData : INetSerializable
    {
        public float SourceX;
        public float SourceY;

        public float DirectionX;
        public float DirectionY;

        public Vector3 SourcePosition => new(SourceX, SourceY, GameConfig.OBJECT_Z_POSITION);
        public Vector3 Direction => new(DirectionX, DirectionY, 0f);

        public PlayerShootData(Vector3 sourcePosition, Vector3 direction)
        {
            SourceX = sourcePosition.x;
            SourceY = sourcePosition.y;

            DirectionX = direction.x;
            DirectionY = direction.y;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SourceX);
            writer.Put(SourceY);
            writer.Put(DirectionX);
            writer.Put(DirectionY);
        }

        public void Deserialize(NetDataReader reader)
        {
            SourceX = reader.GetFloat();
            SourceY = reader.GetFloat();
            DirectionX = reader.GetFloat();
            DirectionY = reader.GetFloat();
        }

        public override string ToString()
        {
            return $"Source: {SourceX}, {SourceY}, Direction: {DirectionX}, {DirectionY}";
        }
    }
}