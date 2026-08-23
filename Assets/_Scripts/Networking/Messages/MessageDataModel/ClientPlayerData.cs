using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct ClientPlayerData : INetSerializable
    {
        public string PlayerName { get; set; }
        public uint PID;


        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerName);
            writer.Put(PID);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerName = reader.GetString();
            PID = reader.GetUInt();
        }

        public override string ToString()
        {
            return $"player name: {PlayerName}, player ID: {PID}";
        }
    }
}