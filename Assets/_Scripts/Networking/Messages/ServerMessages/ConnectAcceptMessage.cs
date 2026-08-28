namespace BETest.Networking.Messages
{
    public class ConnectAcceptMessage
    {
        public ClientPlayerData PlayerData { get; set; }
        public int WorldSeed { get; set; }

        public override string ToString()
        {
            return $"connect accept with data: ({PlayerData})";
        }
    }
}