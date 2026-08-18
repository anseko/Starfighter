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

            PlayerScript ps = null;
            
            if (networkId != 0 && NetworkClient.spawned.TryGetValue(networkId, out var identity))
            {
                ps = identity.GetComponent<PlayerScript>();
            }
            
            if (ps == null)
            {
                ps = NetworkClient.spawned.Values.FirstOrDefault(x => x.isOwned)?.GetComponent<PlayerScript>();
            }
            switch (type)
            {
                case UserType.Admin:
                    initManager.InitAdmin();
                    break;
                case UserType.Pilot:
                    initManager.InitPilot(ps);
                    break;
                case UserType.Navigator:
                    initManager.InitNavigator(ps);
                    break;
                case UserType.Spectator:
                    initManager.InitSpectator();
                    break;
                case UserType.SpaceStation:
                    initManager.InitStation(ps);
                    break;
                case UserType.Mechanic:
                    initManager.InitMechanic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}