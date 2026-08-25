namespace BETest.Networking.Messages
{
    public class NetworkEntitiesSpawnMessage
    {
        public NetworkEntitySpawnDatas SpawnDatas { get; set; }

        public override string ToString()
        {
            return $"network objects spawn with data: ({SpawnDatas})";
        }
    }
}