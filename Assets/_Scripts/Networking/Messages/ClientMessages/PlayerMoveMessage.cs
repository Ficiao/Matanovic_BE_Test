namespace BETest.Networking.Messages
{
    public class PlayerMoveMessage
    {
        public PlayerMoveData Data { get; set; }

        public override string ToString()
        {
            return $"playerer state with data: ({Data})";
        }
    }
}