namespace BETest.Networking.Messages
{
    public class PlayerShootMessage
    {
        public PlayerShootData Data { get; set; }

        public override string ToString()
        {
            return $"player shoot with data: ({Data})";
        }
    }
}