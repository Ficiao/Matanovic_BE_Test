using BETest.Flags;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct PlayerMoveData : INetSerializable
    {
        public uint Seq;

        public float X;
        public float Y;
        public float Z;

        public MoveDirFlags Directions;   
        public short Rotation;

        public void Init(float x, float y, float z, short rotation)
        {
            Seq = 0;
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
            Directions = MoveDirFlags.Grounded;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Seq);
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
            writer.Put(Rotation);

            byte packed = (byte)Directions;
            writer.Put(packed);
        }

        public void Deserialize(NetDataReader reader)
        {
            Seq = reader.GetUInt();
            X = reader.GetFloat();
            Y = reader.GetFloat();
            Z = reader.GetFloat();
            Rotation = reader.GetShort();

            byte packed = reader.GetByte();
            Directions = (MoveDirFlags)(packed); 
        }

        public override string ToString()
        {
            return $"Seq: {Seq}, Pos: ({X:F2},{Y:F2},{Z:F2}), Dirs: {Directions}";
        }
    }
}
