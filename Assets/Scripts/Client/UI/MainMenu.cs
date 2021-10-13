using System;
using Client.Core;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    [RequireComponent(typeof(ConnectionHelper))]
    public class MainMenu : MonoBehaviour
    {
        public InputField loginField;
        public InputField passwordField;
        public InputField serverField;

        private ConnectionHelper _connectionHelper;

        private void Awake()
        {
            _connectionHelper = FindObjectOfType<ConnectionHelper>();
        }

        public void PlayGame()
        {
            _connectionHelper.TryToConnect(serverAddress:serverField.text, login:loginField.text, password:passwordField.text);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}