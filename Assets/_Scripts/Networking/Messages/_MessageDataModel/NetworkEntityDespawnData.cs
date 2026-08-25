using BETest.Enum;
using BETest.Extensions;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct NetworkEntityDespawnData : INetSerializable
    {
        public uint ObjectID;
        public EntityType EntityType; // koristi tvoj extension (byte)

        public NetworkEntityDespawnData(uint objectId, EntityType entityType)
        {
            ObjectID = objectId;
            EntityType = entityType;
        }

        public static int Size()
        {
            return sizeof(uint) + sizeof(byte);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ObjectID);
            writer.Put(EntityType);
        }

        public void Deserialize(NetDataReader reader)
        {
            ObjectID = reader.GetUInt();
            EntityType = reader.GetEntityType(); 
        }

        public override string ToString()
        { 
            return $"Despawn[{ObjectID}, {EntityType}]"; 
        }
    }
}
