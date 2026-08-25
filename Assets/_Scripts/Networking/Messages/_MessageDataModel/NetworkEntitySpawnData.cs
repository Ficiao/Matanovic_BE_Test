using BETest.Enum;
using BETest.Flags;
using BETest.Extensions;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct NetworkEntitySpawnData : INetSerializable
    {
        public ObjectPrefabType PrefabType;
        public ClientPlayerData ClientPlayerData;
        public NetworkEntityStateData StateData;


        public NetworkEntitySpawnData(ObjectPrefabType prefabType, NetworkEntityStateData stateData, ClientPlayerData playerData = default)
        {
            PrefabType = prefabType;
            StateData = stateData;
            StateData.UpdateFlags = EntityUpdateFlags.All;
            ClientPlayerData = playerData;
        }

        public static int Size(EntityType entityType)
        {
            if(entityType == EntityType.Player)
                return sizeof(ushort) + NetworkEntityStateData.Size(EntityUpdateFlags.All) + ClientPlayerData.Size();
            else 
                return sizeof(ushort) + NetworkEntityStateData.Size(EntityUpdateFlags.All);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PrefabType);
            StateData.Serialize(writer);
            if(StateData.EntityType == EntityType.Player) ClientPlayerData.Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            PrefabType = reader.GetObjectPrefabType();
            StateData = new();
            StateData.Deserialize(reader);
            if(StateData.EntityType == EntityType.Player) ClientPlayerData.Deserialize(reader);
        }

        public override string ToString()
        {
            return $"Prefab: {PrefabType}, State data: {StateData}]";
        }
    }
}
