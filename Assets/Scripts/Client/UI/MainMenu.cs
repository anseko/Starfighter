using System;
using System.Text;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class MainMenu : MonoBehaviour
    {
        public InputField loginField;
        public InputField passwordField;
        public InputField serverField;


        public void PlayGame()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(loginField.text + passwordField.text);
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = serverField.text;
            NetworkManager.Singleton.StartClient();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}