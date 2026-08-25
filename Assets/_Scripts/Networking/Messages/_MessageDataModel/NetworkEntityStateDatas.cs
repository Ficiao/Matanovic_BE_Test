using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BETest.Networking.Messages
{
    public struct NetworkEntityStateDatas : INetSerializable
    {
        public List<NetworkEntityStateData> NetworkEntityStates;

        public static int Size(List<NetworkEntityStateData> datas)
        {
            int total = sizeof(ushort);
            foreach (NetworkEntityStateData data in datas) total += NetworkEntityStateData.Size(data.UpdateFlags);
            return total;
        }

        public NetworkEntityStateDatas(int capacity = 100)
        {
            NetworkEntityStates = new List<NetworkEntityStateData>(capacity);
        }

        public void Clear()
        {
            NetworkEntityStates.Clear(); 
        }

        public void Add(ref NetworkEntityStateData state)
        {
            NetworkEntityStates.Add(state);
        }

        public int Count => NetworkEntityStates.Count;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((ushort)NetworkEntityStates.Count); 

            foreach (NetworkEntityStateData state in NetworkEntityStates)
            {
                state.Serialize(writer); 
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            int count = reader.GetUShort();
            if (NetworkEntityStates != null) NetworkEntityStates.Clear();
            else NetworkEntityStates = new();

                for (int i = 0; i < count; i++)
                {
                    NetworkEntityStateData state = new NetworkEntityStateData();
                    state.Deserialize(reader); 
                    NetworkEntityStates.Add(state);
                }
        }

        public override string ToString()
        {
            return $"Objects[{string.Join(", ", NetworkEntityStates.Select(s => s.ToString()))}]";
        }
    }
}
