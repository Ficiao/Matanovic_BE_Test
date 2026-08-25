using BETest.Enum;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace BETest.Networking.Messages
{
    public struct NetworkEntitySpawnDatas : INetSerializable
    {
        public List<NetworkEntitySpawnData> NetworkEntitySpawns;

        public static int Size(int count, EntityType type)
        {
            return sizeof(ushort) + NetworkEntitySpawnData.Size(type) * count;
        }

        public void Serialize(NetDataWriter writer)
        {
            if (NetworkEntitySpawns == null)
            {
                writer.Put((ushort)0);
                return;
            }

            writer.Put((ushort)NetworkEntitySpawns.Count); 

            foreach (NetworkEntitySpawnData spawn in NetworkEntitySpawns)
            {
                spawn.Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            int count = reader.GetUShort();
            NetworkEntitySpawns = new List<NetworkEntitySpawnData>(count);

            for (int i = 0; i < count; i++)
            {
                NetworkEntitySpawnData spawn = new NetworkEntitySpawnData();
                spawn.Deserialize(reader);
                NetworkEntitySpawns.Add(spawn);
            }
        }

        public override string ToString()
        {
            return $"SpawnCollection[{string.Join(", ", NetworkEntitySpawns.Select(s => s.StateData))}]";
        }
    }
}
