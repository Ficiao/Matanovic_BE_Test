using BETest.Flags;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct PlayerMoveData : INetSerializable
    {
        public uint Seq;

        public float X;
        public float Y;

        public MoveDirFlags Directions;   

        public void Init(float x, float y, MoveDirFlags directions)
        {
            Seq = 0;
            X = x;
            Y = y;
            Directions = directions;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Seq);
            writer.Put(X);
            writer.Put(Y);

            byte packed = (byte)Directions;
            writer.Put(packed);
        }

        public void Deserialize(NetDataReader reader)
        {
            Seq = reader.GetUInt();
            X = reader.GetFloat();
            Y = reader.GetFloat();

            byte packed = reader.GetByte();
            Directions = (MoveDirFlags)(packed); 
        }

        public override string ToString()
        {
            return $"Seq: {Seq}, Pos: ({X:F2},{Y:F2}), Dirs: {Directions}";
        }
    }
}
