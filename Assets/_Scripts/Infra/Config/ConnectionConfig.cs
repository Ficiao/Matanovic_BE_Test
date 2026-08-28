namespace BETest.Config
{
    public static class ConnectionConfig
    {
        public const int GAME_PORT = 9051;
        public const int MAX_GAME_PORT = 9060;
        public const int DISCOVERY_PORT_OFFSET = 1000;
        public const string CONNECTION_KEY = "GAME_V1";
        public const string PROTOCOL = "GAME_DISCOVERY_V1";
        public const int MAX_PACKET_BYTES = 950;
    }
}