using System;
using System.Linq;
using Client.Core;
using Client.UI;
using Core;
using Mirror;
using UnityEngine;

namespace Net.Core
{
    public class ClientConnectionHelper: MonoBehaviour
    {
        public void SelectScene(UserType type, uint networkId)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            FindFirstObjectByType<MainMenu>().gameObject.SetActive(false);
            var initManager = FindFirstObjectByType<ClientInitManager>();
            //BUG: почему при вызове в networkId передается 0?
            var ps = FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.isOwned || x.netId == networkId)?.GetComponent<PlayerScript>();
            switch (type)
            {
                case UserType.Admin:
                    initManager.InitAdmin();
                    break;
                case UserType.Pilot:
                    initManager.InitPilot(ps);
                    // GetComponent<ClientInitManager>().InitPilot(ps);
                    break;
                case UserType.Navigator:
                    initManager.InitNavigator(ps);
                    // GetComponent<ClientInitManager>().InitNavigator(ps);
                    break;
                case UserType.Spectator:
                    initManager.InitSpectator();
                    // GetComponent<ClientInitManager>().InitSpectator();
                    break;
                case UserType.SpaceStation:
                    initManager.InitStation(ps);
                    // GetComponent<ClientInitManager>().InitStation(ps);
                    break;
                case UserType.Mechanic:
                    initManager.InitMechanic();
                    // GetComponent<ClientInitManager>().InitMechanic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}