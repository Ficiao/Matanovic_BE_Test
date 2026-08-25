namespace BETest.Networking.Messages
{
    public class PlayerStateMessage
    {
        public NetworkEntityStateData Data { get; set; }

        public override string ToString()
        {
            return $"playerer state with data: ({Data})";
        }
    }
}