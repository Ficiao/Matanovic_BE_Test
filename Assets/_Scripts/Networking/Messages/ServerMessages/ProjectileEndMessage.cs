namespace BETest.Networking.Messages
{
    public class ProjectileEndMessage
    {
        public ProjectileEndData Data { get; set; }

        public override string ToString()
        {
            return $"projectile end with data: ({Data})";
        }
    }
}