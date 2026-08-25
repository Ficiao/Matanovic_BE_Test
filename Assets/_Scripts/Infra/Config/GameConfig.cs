using UnityEngine;

namespace BETest.Config
{
    public static class GameConfig
    {
        public const int MIN_USERNAME_LENGTH = 3;
        public const int MAX_USERNAME_LENGTH = 12;
        public const int MIN_ROOM_NAME_LENGTH = 3;
        public const int MAX_ROOM_NAME_LENGTH = 12;
        public const int MAX_PLAYERS_PER_ROOM = 4;
        public const float OBJECT_Z_POSITION = 2;
        public const float TICK_DELTA = 0.02f;  
    }
}