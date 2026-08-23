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
        [SerializeField] private Button _quitButton;
        [SerializeField] private TMP_Text _notification;

        public event Action<string> LoginRequested;

        private void Awake()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnLoginClicked()
        {
            LoginRequested?.Invoke(_usernameInput.text);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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