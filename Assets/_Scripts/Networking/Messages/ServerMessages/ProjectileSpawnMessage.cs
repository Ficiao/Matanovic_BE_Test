namespace BETest.Networking.Messages
{
    public class ProjectileSpawnMessage
    {
        public ProjectileSpawnData Data { get; set; }

        public override string ToString() => $"projectile spawn with data: ({Data})";
    }
}