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
        public const float TICK_DELTA = 1f / 60f;
        public const float SERVER_TICK_DELTA = 1f / 20f;
        public const int MAX_JUMPS = 2;
        public const float ROOM_AUTO_REFRESH_INTERVAL = 3f;
        public const int PLAYER_MAX_HEALTH = 100;
        public const float PLAYER_RESPAWN_DELAY = 5f;
        public const float PLAYER_DEATH_Y = -27.5f;
    }
}