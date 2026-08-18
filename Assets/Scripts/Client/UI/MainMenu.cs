using Mirror;
using Net.Core;
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
            
            ((StarfighterAuthenticator)NetworkManager.singleton.authenticator).username = loginField.text;
            ((StarfighterAuthenticator)NetworkManager.singleton.authenticator).password = passwordField.text;

            NetworkManager.singleton.StartClient();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}