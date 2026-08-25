using BETest.Enum;
using BETest.Extensions;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct ConnectRequestData : INetSerializable
    {
        public string PlayerName { get; set; }
        public WeaponType PlayerWeaponType { get; set; }
        public PlayerCharacterType PlayerCharacterType { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerName);
            writer.Put(PlayerWeaponType);
            writer.Put(PlayerCharacterType);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerName = reader.GetString();
            PlayerWeaponType = reader.GetWeaponType();
            PlayerCharacterType = reader.GetPlayerCharacterType();
        }   

        public override string ToString()
        {
            return $"player name: {PlayerName}";
        }
    }
}