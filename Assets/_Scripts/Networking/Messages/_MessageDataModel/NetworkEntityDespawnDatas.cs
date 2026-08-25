using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BETest.Networking.Messages
{
    public struct NetworkEntityDespawnDatas : INetSerializable
    {
        public List<NetworkEntityDespawnData> NetworkEntityDespawns;

        public static int Size(int count)
        {
            return sizeof(ushort) + NetworkEntityDespawnData.Size() * count;
        }

        public void Serialize(NetDataWriter writer)
        {
            if (NetworkEntityDespawns == null) 
            {
                writer.Put((ushort)0);
                return;
            }

            writer.Put((ushort)(NetworkEntityDespawns.Count));

            foreach (NetworkEntityDespawnData spawn in NetworkEntityDespawns)
            {
                spawn.Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            int count = reader.GetUShort();
            NetworkEntityDespawns = new List<NetworkEntityDespawnData>(count);
            for (int i = 0; i < count; i++)
            {
                NetworkEntityDespawnData it = new NetworkEntityDespawnData();
                it.Deserialize(reader);
                NetworkEntityDespawns.Add(it);
            }
        }

        public override string ToString()
        {
            return $"DespawnCollection[{string.Join(", ", NetworkEntityDespawns.Select(i => i.ObjectID))}]";
        }
    }
}
