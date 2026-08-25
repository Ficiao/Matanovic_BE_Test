using System;

namespace BETest.Flags
{
    [Flags]
    public enum MoveDirFlags : byte
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Grounded = 1 << 2,
    }
}