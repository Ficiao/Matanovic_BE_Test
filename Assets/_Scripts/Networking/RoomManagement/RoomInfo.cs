using System.Net;

namespace BETest.Networking.RoomManagement
{
    public class RoomInfo
    {
        public string Id;
        public string Name;

        public int PlayerCount;
        public int MaxPlayers;

        public string HostAddress;
        public int GamePort;

        public IPEndPoint EndPoint => new(IPAddress.Parse(HostAddress), GamePort);
    }
}