namespace BETest.Networking.Messages
{
    public class NetworkEntityStatesMessage
    {
        public NetworkEntityStateDatas Data { get; set; }

        public override string ToString()
        {
            return $"network entity states with data: ({Data})";
        }
    }
}