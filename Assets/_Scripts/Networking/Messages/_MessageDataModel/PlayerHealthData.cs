using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct PlayerHealthData : INetSerializable
    {
        public uint PID;
        public int Health;

        public PlayerHealthData(uint PID, int health)
        {
            this.PID = PID;
            Health = health;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PID);
            writer.Put(Health);
        }

        public void Deserialize(NetDataReader reader)
        {
            PID = reader.GetUInt();
            Health = reader.GetInt();
        }

        public override string ToString()
        {
            return $"PID: {PID}, Health: {Health}";
        }
    }
}