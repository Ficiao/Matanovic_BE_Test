using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BETest.UI.Views
{
    public class LoginView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private Button _loginButton;
        [SerializeField] private TMP_Text _notification;

        public event Action<string> LoginRequested;

        private void Awake()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
        }

        private void OnLoginClicked()
        {
            LoginRequested?.Invoke(_usernameInput.text);
        }

        public void ShowError(string message)
        {
            _notification.text = message;
            _notification.gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}