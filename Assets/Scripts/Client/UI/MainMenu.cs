using System.Text;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class MainMenu : MonoBehaviour
    {
        public InputField loginField;
        public InputField passwordField;
        //IP address
        public InputField serverField;


        public void PlayGame()
        {
            NetworkManager.singleton.networkAddress = serverField.text;
            NetworkManager.singleton.StartClient();
            NetworkClient.connection.authenticationData = Encoding.ASCII.GetBytes(loginField.text + passwordField.text);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}