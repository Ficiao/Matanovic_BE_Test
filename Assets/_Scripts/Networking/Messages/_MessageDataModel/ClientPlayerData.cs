using BETest.Config;
using BETest.Enum;
using BETest.Extensions;
using LiteNetLib.Utils;

namespace BETest.Networking.Messages
{
    public struct ClientPlayerData : INetSerializable
    {
        public uint PID;
        public string PlayerName { get; set; }
        public WeaponType PlayerWeaponType { get; set; }
        public PlayerCharacterType PlayerCharacterType { get; set; }

        public static int Size()
        {
            return sizeof(uint) + sizeof(char) * GameConfig.MAX_USERNAME_LENGTH + sizeof(ushort) * 2; 
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PID);
            writer.Put(PlayerName);
            writer.Put(PlayerWeaponType);
            writer.Put(PlayerCharacterType);
        }

        public void Deserialize(NetDataReader reader)
        {
            PID = reader.GetUInt();
            PlayerName = reader.GetString();
            PlayerWeaponType = reader.GetWeaponType();
            PlayerCharacterType = reader.GetPlayerCharacterType();
        }

        public override string ToString()
        {
            return $"player name: {PlayerName}, player ID: {PID}";
        }
    }
}