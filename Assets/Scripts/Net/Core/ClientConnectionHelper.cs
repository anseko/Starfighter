using System;
using System.Linq;
using Client.Core;
using Client.UI;
using Core;
using Mirror;
using UnityEngine;

namespace Net.Core
{
    public class ClientConnectionHelper: NetworkBehaviour
    {
        [SyncVar] public UserType userType; 
        //     = new NetworkVariable<UserType>(new NetworkVariableSettings()
        // {
        //     ReadPermission = NetworkVariablePermission.Everyone,
        //     WritePermission = NetworkVariablePermission.ServerOnly
        // });

        [TargetRpc]
        public void SelectSceneClientRpc(NetworkConnectionToClient target, UserType type, uint networkId)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            FindObjectOfType<MainMenu>().gameObject.SetActive(false);
            //BUG: почему при вызове в networkId передается 0?
            var ps = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.isOwned || x.netId == networkId)?.GetComponent<PlayerScript>(); 
            switch (type)
            {
                case UserType.Admin:
                    GetComponent<ClientInitManager>().InitAdmin();
                    break;
                case UserType.Pilot:
                    GetComponent<ClientInitManager>().InitPilot(ps);
                    break;
                case UserType.Navigator:
                    GetComponent<ClientInitManager>().InitNavigator(ps);
                    break;
                case UserType.Spectator:
                    GetComponent<ClientInitManager>().InitSpectator();
                    break;
                case UserType.SpaceStation:
                    GetComponent<ClientInitManager>().InitStation(ps);
                    break;
                case UserType.Mechanic:
                    GetComponent<ClientInitManager>().InitMechanic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}