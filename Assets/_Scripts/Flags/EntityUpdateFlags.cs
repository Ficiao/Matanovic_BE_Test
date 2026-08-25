using System;

namespace BETest.Flags
{
    [Flags]
    public enum EntityUpdateFlags : ushort
    {
        Position = 1 << 0,
        MoveDir = 1 << 2,
        MoveSpeed = 1 << 4,

        All = Position | MoveDir | MoveSpeed,
        Self = Position,
    }
}