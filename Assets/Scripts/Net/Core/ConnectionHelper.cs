using System;
using System.Linq;
using Client.Core;
using Client.UI;
using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Core
{
    public class ConnectionHelper: NetworkBehaviour
    {
        public NetworkVariable<UserType> userType = new NetworkVariable<UserType>(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        });

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SelectSceneClientRpc(UserType type, ulong networkId, ClientRpcParams clientRpcParams = default)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            FindObjectOfType<MainMenu>().gameObject.SetActive(false);
            var ps = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.IsOwner || x.NetworkObjectId == networkId)?.GetComponent<PlayerScript>();
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
                case UserType.Moderator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}