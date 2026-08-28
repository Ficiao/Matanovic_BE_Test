namespace BETest.Networking.Messages
{
    public class PlayerScoreRemovedMessage
    {
        public uint PID { get; set; }

        public override string ToString()
        {
            return $"remove player score PID: {PID}";
        }
    }
}