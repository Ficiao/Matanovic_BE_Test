using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Messages
{
    public static class ConnectRequestMessageHandler
    {
        public static void ProcessMessage(ConnectRequestMessage message, NetPeer peer, GameSceneContext context)
        {
            NetworkObjectStateManager objectManager = context != null ? context.ObjectStateManager : null;
            if (objectManager == null)
            {
                CustomLogger.Warning($"disconnecting_peer", new() { ["id"] = peer?.Id, ["reason"] = "game_scene_not_ready" });
                peer.Disconnect();
                return;
            }

            if (!objectManager.CanAcceptPlayer)
            {
                CustomLogger.Warning("disconnecting_peer", new() {["id"] = peer.Id, ["reason"] = "room_full"});

                peer.Disconnect();
                return;
            }

            uint PID = (uint)peer.Id;
            if (objectManager.IsPlayerConnected(PID)) return;

            objectManager.PlayerConnected(peer, message.Data);
        }
    }
}