using BETest.Networking.Messages;
using TMPro;
using UnityEngine;

namespace BETest.UI.Views
{
    public class PlayerScoreboardDataView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _killsText;

        public uint PID { get; private set; }

        public void SetData(PlayerScoreData data)
        {
            PID = data.PID;
            _playerNameText.text = data.PlayerName;
            _killsText.text = data.Kills.ToString();
        }
    }
}