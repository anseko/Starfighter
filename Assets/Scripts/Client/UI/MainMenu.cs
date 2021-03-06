﻿using Core;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class MainMenu : MonoBehaviour
    {
        public UnityEngine.UI.Toggle pilotToggle;
        public UnityEngine.UI.Toggle naviToggle;
        public UserType accType;
        public InputField loginField;
        public InputField passwordField;
        public InputField serverField;
        public void PlayGame()
        {
            if (pilotToggle.isOn)
            {
                accType = UserType.Pilot;
            }
            if (naviToggle.isOn)
            {
                accType = UserType.Navigator;
            }
            ClientConnectionHelper.instance.TryToConnect(serverAddress:serverField.text, login:loginField.text, password:passwordField.text, accType:accType);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}