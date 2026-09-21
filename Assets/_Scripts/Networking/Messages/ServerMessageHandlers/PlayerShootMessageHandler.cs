using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerShootMessageHandler
    {
        public static void ProcessMessage(PlayerShootMessage message, NetPeer peer, GameSceneContext context)
        {
            NetworkObjectStateManager objectStateManager = context != null ? context.ObjectStateManager : null;

            if (objectStateManager == null)
            {
                CustomLogger.Warning("disconnecting_peer", new() { ["ID"] = peer?.Id, ["reason"] = "game_scene_not_ready, unprocessed_shoot" });
                peer.Disconnect();
                return;
            }

            uint PID = (uint)peer.Id;
            objectStateManager.PlayerShoot(PID, message.Data);
        }
    }
}