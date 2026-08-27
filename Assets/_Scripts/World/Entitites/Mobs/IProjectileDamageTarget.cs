namespace BETest.Entities
{
    public interface IProjectileDamageTarget
    {
        uint ObjectID { get; }

        void DealDamage(int damage, uint sourcePID);
    }
}