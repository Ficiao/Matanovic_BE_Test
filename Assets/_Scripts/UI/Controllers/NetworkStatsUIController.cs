using BETest.Networking.ConnectionHandling;
using BETest.Networking.Messages;
using BETest.Networking.RoomManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BETest.UI.Controllers
{
    public class NetworkStatsUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _statsText;
        [SerializeField] private float _sampleInterval = 0.5f;
        [SerializeField] private float _averageWindow = 5f;

        private readonly Queue<BandwidthSample> _samples = new();

        private NetworkClient _client;
        private NetworkServer _server;
        private RoomManager _roomManager;

        private long _lastBytesReceived;
        private long _lastBytesSent;
        private float _lastSampleTime;
        private float _nextSampleTime;

        private struct BandwidthSample
        {
            public long IncomingBytes;
            public long OutgoingBytes;
            public float Duration;
        }

        public void Initialize(NetworkClient client, NetworkServer server, RoomManager roomManager)
        {
            _client = client;
            _server = server;
            _roomManager = roomManager;

            bool isHost = IsHost();

            _lastBytesReceived = GetBytesReceived(isHost);
            _lastBytesSent = GetBytesSent(isHost);
            _lastSampleTime = Time.unscaledTime;
            _nextSampleTime = Time.unscaledTime + _sampleInterval;
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextSampleTime) return;

            float now = Time.unscaledTime;
            float duration = now - _lastSampleTime;

            bool isHost = IsHost();

            long bytesReceived = GetBytesReceived(isHost);
            long bytesSent = GetBytesSent(isHost);

            _samples.Enqueue(new BandwidthSample
            {
                IncomingBytes = bytesReceived - _lastBytesReceived,
                OutgoingBytes = bytesSent - _lastBytesSent,
                Duration = duration
            });

            float totalDuration = 0f;
            long totalIncoming = 0;
            long totalOutgoing = 0;

            foreach (BandwidthSample sample in _samples)
            {
                totalDuration += sample.Duration;
                totalIncoming += sample.IncomingBytes;
                totalOutgoing += sample.OutgoingBytes;
            }

            while (totalDuration > _averageWindow && _samples.Count > 1)
            {
                BandwidthSample sample = _samples.Dequeue();
                totalDuration -= sample.Duration;
                totalIncoming -= sample.IncomingBytes;
                totalOutgoing -= sample.OutgoingBytes;
            }

            float incomingKB = totalIncoming / 1024f / totalDuration;
            float outgoingKB = totalOutgoing / 1024f / totalDuration;

            _statsText.text = $"{(isHost ? "HOST" : "CLIENT")} \nIN: {incomingKB:F1} KB/s\nOUT: {outgoingKB:F1} KB/s";

            _lastBytesReceived = bytesReceived;
            _lastBytesSent = bytesSent;
            _lastSampleTime = now;
            _nextSampleTime = now + _sampleInterval;
        }

        private bool IsHost()
        {
            return _roomManager.State == RoomStateType.Creating || _roomManager.State == RoomStateType.InRoomHost;
        }

        private long GetBytesReceived(bool isHost) => isHost ? _server.BytesReceived : _client.BytesReceived;
        private long GetBytesSent(bool isHost) => isHost ? _server.BytesSent : _client.BytesSent;
    }
}