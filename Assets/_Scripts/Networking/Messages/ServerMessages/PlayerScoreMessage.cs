namespace BETest.Networking.Messages
{
    public class PlayerScoreMessage
    {
        public PlayerScoreData Data { get; set; }

        public override string ToString()
        {
            return $"player score with data: ({Data})";
        }
    }
}