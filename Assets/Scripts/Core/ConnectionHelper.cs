using System;
using System.Text;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class ConnectionHelper: NetworkBehaviour
    {
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SelectSceneClientRpc(UserType type, ClientRpcParams clientRpcParams = default)
        {
            Debug.unityLogger.Log($"I pick scene type: {type}");
            
            // if (NetworkManager.Singleton.LocalClientId != clientId) return;

            switch (type)
            {
                case UserType.Pilot:
                    SceneManager.LoadScene("Scenes/pilot_UI", LoadSceneMode.Additive);
                    break;
                case UserType.Navigator:
                    SceneManager.LoadScene("Scenes/navi_UI", LoadSceneMode.Additive);
                    break;
                case UserType.Admin:
                case UserType.Spectator:
                case UserType.Moderator:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}