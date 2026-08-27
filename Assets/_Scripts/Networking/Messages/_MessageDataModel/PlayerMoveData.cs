using BETest.Flags;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct PlayerMoveData : INetSerializable
    {
        public uint Seq;

        public ushort X;
        public ushort Y;
        public ushort AimAngle;

        public MoveDirFlags Directions;   

        public PlayerMoveData(NetworkEntityStateData stateData)
        {
            X = stateData.X;
            Y = stateData.Y;
            Directions = stateData.Directions;
            AimAngle = stateData.AimAngle;
            Seq = stateData.SeqAcc;
        }

        public PlayerMoveData(ushort x, ushort y, MoveDirFlags directions, ushort angle)
        {
            Seq = 0;
            X = x;
            Y = y;
            Directions = directions;
            AimAngle = angle;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Seq);
            writer.Put(X);
            writer.Put(Y);
            writer.Put(AimAngle);

            byte packed = (byte)Directions;
            writer.Put(packed);
        }

        public void Deserialize(NetDataReader reader)
        {
            Seq = reader.GetUInt();
            X = reader.GetUShort();
            Y = reader.GetUShort();
            AimAngle = reader.GetUShort();

            byte packed = reader.GetByte();
            Directions = (MoveDirFlags)(packed); 
        }

        public override string ToString()
        {
            return $"Seq: {Seq}, Pos: ({X:F2},{Y:F2}), Aim angle {AimAngle}, Dirs: {Directions}";
        }
    }
}
