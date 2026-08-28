namespace BETest.Networking.Messages
{
    public class PlayerHealthMessage
    {
        public PlayerHealthData Data { get; set; }

        public override string ToString()
        {
            return $"player health with data: ({Data})";
        }
    }
}