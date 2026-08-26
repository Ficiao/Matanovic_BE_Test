using BETest.Config;
using BETest.Enum;
using BETest.Extensions;
using BETest.Networking.Messages;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace BETest.Networking.Transport
{
    public class MessageProcessor
    {
        private readonly NetPacketProcessor _packetProcessor;
        private readonly NetDataWriter _writer;

        public MessageProcessor()
        {
            _packetProcessor = new NetPacketProcessor();
            _writer = new NetDataWriter();

            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetEntityType());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetPlayerCharacterType());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetMoveDirFlags());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetWeaponType());

            RegisterNestedType<ConnectRequestData>();
            RegisterNestedType<ClientPlayerData>();
            RegisterNestedType<NetworkEntityDespawnData>();
            RegisterNestedType<NetworkEntityDespawnDatas>();
            RegisterNestedType<NetworkEntitySpawnData>();
            RegisterNestedType<NetworkEntitySpawnDatas>();
            RegisterNestedType<NetworkEntityStateData>();
            RegisterNestedType<NetworkEntityStateDatas>();
            RegisterNestedType<PlayerMoveData>();
        }

        public void RegisterNestedType<T>() where T : struct, INetSerializable
        {
            _packetProcessor.RegisterNestedType<T>();
        }

        public void Subscribe<T>(Action<T, NetPeer> handler) where T : class, new()
        {
            _packetProcessor.SubscribeReusable(handler);
        }

        public void HandleAllPacketsForPeer(NetDataReader reader, NetPeer peer)
        {
            _packetProcessor.ReadAllPackets(reader, peer);
        }

        public void SendPacket<T>(T packet, NetPeer peer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            if (peer == null) return;

            _writer.Reset();
            _packetProcessor.Write(_writer, packet);

            if (_writer.Length > ConnectionConfig.MAX_PACKET_BYTES)
                CustomLogger.Warning("packet_oversize", new() { ["len"] = _writer.Length, ["type"] = typeof(T).Name });

            peer.Send(_writer, (byte)channel, deliveryMethod);
        }

        public void SendPacketToAll<T>(T packet, NetManager server, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            _writer.Reset();
            _packetProcessor.Write(_writer, packet);

            if (_writer.Length > ConnectionConfig.MAX_PACKET_BYTES)
                CustomLogger.Warning("packet_oversize", new() { ["len"] = _writer.Length, ["type"] = typeof(T).Name });

            server.SendToAll(_writer, (byte)channel, deliveryMethod);
        }

        public void SendPacketToAllExcept<T>(T packet, NetManager server, NetPeer excludedPeer, TransmissionChannel channel, DeliveryMethod deliveryMethod) where T : class, new()
        {
            _writer.Reset();
            _packetProcessor.Write(_writer, packet);

            if (_writer.Length > ConnectionConfig.MAX_PACKET_BYTES)
                CustomLogger.Warning("packet_oversize", new() { ["len"] = _writer.Length, ["type"] = typeof(T).Name });

            server.SendToAll(_writer, (byte)channel, deliveryMethod, excludedPeer);
        }
    }
}