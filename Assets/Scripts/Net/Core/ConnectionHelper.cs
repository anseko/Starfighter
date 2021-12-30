using System;
using System.Linq;
using Client.Core;
using Client.UI;
using Core;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Net.Core
{
    public class ConnectionHelper: NetworkBehaviour
    {
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SelectSceneClientRpc(UserType type, ulong networkId, UnitState state, ClientRpcParams clientRpcParams = default)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            FindObjectOfType<MainMenu>().gameObject.SetActive(false);
            var ps = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.IsOwner || x.NetworkObjectId == networkId)?.GetComponent<PlayerScript>();
            switch (type)
            {
                case UserType.Admin:
                    break;
                case UserType.Pilot:
                    GetComponent<ClientInitManager>().InitPilot(ps);
                    break;
                case UserType.Navigator:
                    GetComponent<ClientInitManager>().InitNavigator(ps);
                    break;
                case UserType.Spectator:
                    GetComponent<ClientInitManager>().InitSpectator(ps);
                    break;
                case UserType.Moderator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            if(ps.GetState() != state) ps.unitStateMachine.ChangeState(state);
        }
    }
}