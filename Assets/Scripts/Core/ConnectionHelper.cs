﻿using System;
using System.Linq;
using Client;
using Client.Core;
using Client.UI;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Core
{
    public class ConnectionHelper: NetworkBehaviour
    {
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SelectSceneClientRpc(UserType type, ulong networkId, ClientRpcParams clientRpcParams = default)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            FindObjectOfType<MainMenu>().gameObject.SetActive(false);
            // if (NetworkManager.Singleton.LocalClientId != clientId) return;
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
                    break;
                case UserType.Moderator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}