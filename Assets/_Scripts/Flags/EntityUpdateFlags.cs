using System;

namespace BETest.Flags
{
    [Flags]
    public enum EntityUpdateFlags : ushort
    {
        Position = 1 << 0,
        MoveDir = 1 << 1,
        MoveSpeed = 1 << 2,
        Aim = 1 << 3,

        All = Position | MoveDir | MoveSpeed | Aim,
        None = 0,
    }
}