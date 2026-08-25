namespace BETest.Networking.Messages
{
    public class NetworkEntitiesDespawnMessage
    {
        public NetworkEntityDespawnDatas DespawnDatas { get; set; }

        public override string ToString()
        {
            return $"network objects despawn with data: ({DespawnDatas})";
        }
    }
}