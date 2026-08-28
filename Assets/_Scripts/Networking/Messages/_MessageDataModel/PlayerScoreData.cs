using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct PlayerScoreData : INetSerializable
    {
        public uint PID;
        public string PlayerName;
        public int Kills;

        public PlayerScoreData(uint PID, string playerName, int kills)
        {
            this.PID = PID;
            PlayerName = playerName;
            Kills = kills;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PID);
            writer.Put(PlayerName);
            writer.Put(Kills);
        }

        public void Deserialize(NetDataReader reader)
        {
            PID = reader.GetUInt();
            PlayerName = reader.GetString();
            Kills = reader.GetInt();
        }

        public override string ToString()
        {
            return  $"Player {PID}, {Kills}";
        }
    }
}