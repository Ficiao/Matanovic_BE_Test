using BETest.Enum;
using BETest.Extensions;
using BETest.Flags;
using LiteNetLib.Utils;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public struct NetworkEntityStateData : INetSerializable
    {
        public uint ObjectID;
        public uint StateAuthorityPID;
        public EntityType EntityType;
        public EntityUpdateFlags UpdateFlags;
        public uint SeqAcc;

        public ushort X, Y;
        public MoveDirFlags Directions;
        public float MoveSpeed;

        public void Init(uint objectID, uint stateAuthorityPID, EntityType entityType, ushort x, ushort y, short rotation, float moveSpeed)
        {
            ObjectID = objectID;
            StateAuthorityPID = stateAuthorityPID;
            EntityType = entityType;
            SeqAcc = 0;
            X = x;
            Y = y;
            MoveSpeed = moveSpeed;
            Directions = MoveDirFlags.Grounded;
        }

        public static int Size(EntityUpdateFlags flags)
        {
            int size = sizeof(uint) * 3 + sizeof(byte) + sizeof(ushort);

            if ((flags & EntityUpdateFlags.Position) != 0) size += sizeof(ushort) * 2;
            if ((flags & EntityUpdateFlags.MoveDir) != 0) size += sizeof(ushort);
            if ((flags & EntityUpdateFlags.MoveSpeed) != 0) size += sizeof(float);

            return size;
        }

        public void UpdateValues(NetworkEntityStateData newData)
        {
            EntityUpdateFlags flags = newData.UpdateFlags;
            if ((flags & EntityUpdateFlags.Position) != 0)
            {
                X = newData.X;
                Y = newData.Y;
            }
            if ((flags & EntityUpdateFlags.MoveDir) != 0) Directions = newData.Directions;
            if ((flags & EntityUpdateFlags.MoveSpeed) != 0) MoveSpeed = newData.MoveSpeed;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ObjectID);
            writer.Put(StateAuthorityPID);
            writer.Put(EntityType);
            writer.Put((ushort)UpdateFlags); 
            writer.Put(SeqAcc);

            if ((UpdateFlags & EntityUpdateFlags.Position) != 0)
            {
                writer.Put(X);
                writer.Put(Y);
            }
            if ((UpdateFlags & EntityUpdateFlags.MoveDir) != 0) writer.Put(Directions);
            if ((UpdateFlags & EntityUpdateFlags.MoveSpeed) != 0) writer.Put(MoveSpeed);
        }

        public void Deserialize(NetDataReader reader)
        {
            ObjectID = reader.GetUInt();
            StateAuthorityPID = reader.GetUInt();
            EntityType = reader.GetEntityType();
            UpdateFlags = (EntityUpdateFlags)reader.GetUShort();
            SeqAcc = reader.GetUInt();

            if ((UpdateFlags & EntityUpdateFlags.Position) != 0)
            {
                X = reader.GetUShort();
                Y = reader.GetUShort();
            }
            if ((UpdateFlags & EntityUpdateFlags.MoveDir) != 0) Directions = reader.GetMoveDirFlags();
            if ((UpdateFlags & EntityUpdateFlags.MoveSpeed) != 0) MoveSpeed = reader.GetFloat();
        }

        public override string ToString()
        {
            string log = $"Id: {ObjectID}, StateAuthorityID: {StateAuthorityPID}, EntityType: {EntityType}, Seq: {SeqAcc}";
            if ((UpdateFlags & EntityUpdateFlags.Position) != 0) log += $", Position: ({Mathf.HalfToFloat(X)}, {Mathf.HalfToFloat(Y)}), ";
            if ((UpdateFlags & EntityUpdateFlags.MoveDir) != 0) log += $", MoveDir: ({Directions}), ";
            if ((UpdateFlags & EntityUpdateFlags.MoveSpeed) != 0) log += $", MoveSpeed: {MoveSpeed}";
            return log;
        }
    }
}
