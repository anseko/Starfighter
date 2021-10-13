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
        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
        }

        private void OnConnected(ulong obj)
        {
            
        }

        private void OnDisconnected(ulong obj)
        {
            // SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        public void TryToConnect(string serverAddress, string login, string password)
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(login + password);
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = serverAddress;
            NetworkManager.Singleton.StartClient();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SelectSceneClientRpc(ulong clientId, UserType type)
        {
            Debug.unityLogger.Log($"I'm {clientId} select scene, {type}");
            
            if (NetworkManager.Singleton.LocalClientId != clientId) return;

            switch (type)
            {
                case UserType.Pilot:
                    SceneManager.LoadScene("Scenes/pilot_UI", LoadSceneMode.Single);
                    break;
                case UserType.Navigator:
                    SceneManager.LoadScene("Scenes/navi_UI", LoadSceneMode.Single);
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