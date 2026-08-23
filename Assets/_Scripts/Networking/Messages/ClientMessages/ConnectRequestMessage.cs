namespace BETest.Networking.Messages
{
    public class ConnectRequestMessage
    {
        public ConnectRequestData Data { get; set; }

        public override string ToString()
        {
            return $"player connect request with data: ({Data})";
        }
    }
}