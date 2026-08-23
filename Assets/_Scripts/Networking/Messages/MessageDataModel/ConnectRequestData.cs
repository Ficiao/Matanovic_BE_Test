using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct ConnectRequestData : INetSerializable
    {
        public string PlayerName { get; set; }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerName);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerName = reader.GetString();
        }   

        public override string ToString()
        {
            return $"player name: {PlayerName}";
        }
    }
}