using BETest.Entities;
using BETest.Enum;
using BETest.Networking.ConnectionHandling;
using BETest.Networking.Managers;
using BETest.Networking.Messages;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;

namespace BETest.Networking.Services
{ 
    public class NetworkStateBroadcastService
    {
        private readonly List<NetworkEntityStateData> _dirtyStates = new(128);
        private readonly NetworkEntityStatesMessage _reusableStatesMessage = new();

        private NetworkObjectStateManager _objectStateManager;

        public void Initialize(NetworkObjectStateManager objectStateManager)
        {
            _objectStateManager = objectStateManager;
        }

        public void HandleTick()
        {
            _dirtyStates.Clear();
            _objectStateManager.GetDirtyStates(_dirtyStates);

            if (_dirtyStates.Count == 0) return;

            _reusableStatesMessage.Data = new NetworkEntityStateDatas
            {
                NetworkEntityStates = _dirtyStates,
            };

            NetworkServer.SendMessageToAll(_reusableStatesMessage, TransmissionChannel.StateUpdate, DeliveryMethod.Unreliable);
        }
    }
}