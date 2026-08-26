using BETest.Infra.DependacyHandling;
using BETest.Networking.Managers;
using LiteNetLib;

namespace BETest.Networking.Messages
{
    public static class PlayerMoveMessageHandler
    {
        public static void ProcessMessage(PlayerMoveMessage message, NetPeer peer)
        {
            NetworkObjectStateManager objectStateManager = GameSceneContext.Instance?.ObjectStateManager;
            if (objectStateManager == null)
            {
                CustomLogger.Warning("disconnecting_peer", new() { ["ID"] = peer?.Id, ["reason"] = "game_scene_not_ready, unprocessed_player_move" });
                peer.Disconnect();
                return;
            }

            uint PID = (uint)peer.Id;
            objectStateManager.TryAcceptPlayerMove(PID, message.Data);
        }
    }
}