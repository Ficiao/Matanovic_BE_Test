using BETest.Misc;

namespace BETest.Player
{
    public class PlayerSession : SingletonPersistent<PlayerSession>
    {
        public string Username { get; private set; }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void ResetData()
        {
            Username = null;
        }
    }
}