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
            NetworkManager.singleton.authenticator.ConnectionData = Encoding.ASCII.GetBytes(loginField.text + passwordField.text);
            NetworkManager.singleton.GetComponent<KcpTransport>() = serverField.text;
            NetworkManager.singleton.StartClient();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}