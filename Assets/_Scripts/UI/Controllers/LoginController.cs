using BETest.UI.Views;
using UnityEngine;

namespace BETest.UI.Controllers
{
    public class LoginController : MonoBehaviour
    {
        [SerializeField] private LoginView _view;

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
            if (string.IsNullOrWhiteSpace(username))
            {
                _view.ShowError("Username cannot be empty.");
                return;
            }

            PlayerSession.Instance.Username = username;

            _view.Hide();

            // otvori SessionExplorer
            // SessionSelectionController.Instance.Open();
        }
    }
}