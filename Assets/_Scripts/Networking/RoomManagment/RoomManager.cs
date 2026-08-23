using BETest.Networking.ConnectionHandling;
using BETest.Misc;
using System;

namespace BETest.Networking.RoomManagment
{
    public class RoomManager : SingletonPersistent<RoomManager>
    {
        public RoomInfo CurrentRoom { get; private set; }

        public void CreateRoom(string roomName, int gamePort)
        {
            NetworkServer.Instance.StartServer();
            NetworkClient.Instance.Connect("127.0.0.1");

            CurrentRoom = new RoomInfo
            {
                Id = Guid.NewGuid().ToString(),
                Name = roomName,
                PlayerCount = 1,
                MaxPlayers = 3,
                GamePort = gamePort
            };

            
            LanDiscovery.Instance.StartAdvertising(CurrentRoom);
        }

        public void BrowseRooms()
        {
            LanDiscovery.Instance.StartBrowsing();
            LanDiscovery.Instance.Search();
        }

        public void JoinRoom(RoomInfo room)
        {
            NetworkClient.Instance.Connect(room.HostAddress);
        }

        public void LeaveRoom()
        {
            LanDiscovery.Instance.StopAdvertising();
            CurrentRoom = null;
        }
    }
}