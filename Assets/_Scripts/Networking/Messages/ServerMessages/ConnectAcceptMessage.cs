namespace BETest.Networking.Messages
{
    public class ConnectAcceptMessage
    {
        public ClientPlayerData PlayerData { get; set; }
        public short TickIndex;

        public override string ToString()
        {
            return $"connect accept with data: ({PlayerData})";
        }
    }
}