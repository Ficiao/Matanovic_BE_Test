using BETest.Config;
using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;

namespace BETest.Networking.Services
{
    public class NetworkStateBroadcastService
    {
        private const int PACKET_RESERVE_BYTES = 64;

        private readonly List<NetworkEntityStateData> _states = new(256);
        private readonly List<NetworkEntityStateData> _batch = new(64);
        private readonly NetworkEntityStatesMessage _reusableStatesMessage = new();

        private NetworkObjectStateManager _objectStateManager;

        public void Initialize(NetworkObjectStateManager objectStateManager)
        {
            _objectStateManager = objectStateManager;
        }

        public void HandleTick()
        {
            _states.Clear();
            _objectStateManager.GetDirtyStates(_states);

            if (_states.Count == 0) return;

            _batch.Clear();

            int maxBatchSize = ConnectionConfig.MAX_PACKET_BYTES - PACKET_RESERVE_BYTES;
            int batchSize = sizeof(ushort);

            foreach (NetworkEntityStateData state in _states)
            {
                int stateSize = NetworkEntityStateData.Size(state.UpdateFlags);

                if (_batch.Count > 0 && batchSize + stateSize > maxBatchSize)
                {
                    SendBatch();
                    batchSize = sizeof(ushort);
                }

                _batch.Add(state);
                batchSize += stateSize;
            }

            if (_batch.Count > 0) SendBatch();
        }

        private void SendBatch()
        {
            _reusableStatesMessage.Data = new NetworkEntityStateDatas
            {
                NetworkEntityStates = _batch,
            };

            NetworkServer.SendMessageToAll(_reusableStatesMessage, TransmissionChannel.StateUpdate, DeliveryMethod.Unreliable);
            _batch.Clear();
        }

        public static void BroadcastPlayerHealth(PlayerHealthData data)
        {
            PlayerHealthMessage message = new()
            {
                Data = data,
            };

            NetworkServer.SendMessageToAll(message, TransmissionChannel.GenericRO, DeliveryMethod.ReliableOrdered);
        }
    }
}