using BETest.Config;
using BETest.Player;
using BETest.UI.Views;
using System;
using UnityEngine;

namespace BETest.UI.Controllers
{
    public class LoginController : MonoBehaviour
    {
        [SerializeField] private LoginView _view;

        public static event Action LoginSucceeded;

        private void OnEnable()
        {
            _view.LoginRequested += Login;
        }

        private void OnDisable()
        {
            _view.LoginRequested -= Login;
        }

        private void Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < GameConfig.MIN_USERNAME_LENGTH || username.Length > GameConfig.MAX_USERNAME_LENGTH)
            {
                _view.ShowError($"Username must be between {GameConfig.MIN_USERNAME_LENGTH} and {GameConfig.MAX_USERNAME_LENGTH} characters.");
                return;
            }

            PlayerSession.Instance.SetUsername(username);
            _view.Hide();

            LoginSucceeded?.Invoke();
        }
    }
}