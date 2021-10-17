using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Core;
using Core.InputManager;
using Core.Models;
using MLAPI;
using MLAPI.Configuration;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using MLAPI.Spawning;
using MLAPI.Transports.UNET;
using Net.Core;
using Net.PackageData;
using Net.PackageHandlers.ServerHandlers;
using Net.Packages;
using Net.Utils;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Net
{
    [RequireComponent(typeof(HandlerManager))]
    [RequireComponent(typeof(ServerInitializeHelper))]
    public class MainServerLoop : MonoBehaviour
    {
        public Image indicator;
        public TextMeshProUGUI clientCounter;
        [SerializeField] private List<ClientAccountObject> accountObjects;
        [SerializeField] private ConnectionHelper _connector;

        private void Awake()
        {
            NetEventStorage.GetInstance().worldInit.AddListener(BeginReceiving);
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 120;
        }

        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectCallback;
            StartCoroutine(ServerInitializeHelper.instance.InitServer());
        }
        
        private void OnDisconnectCallback(ulong clientId)
        {
            var account = accountObjects.First(x => x.clientId == clientId);
            account.clientId = null;
            Debug.unityLogger.Log($"Disconnection: {clientId}");
        }

        private void OnConnectCallback(ulong clientId)
        {
            Debug.Log($"Connection accepted: {clientId}");
            var account = accountObjects.First(x => x.clientId == clientId);
            
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[]{clientId}
                }
            };
            
            _connector.SelectSceneClientRpc(account.type, clientRpcParams);
            //Передача владения объектом корабля
            var go = GameObject.Find($"{account.ship.prefabName}|{account.ship.shipId}");
            go.GetComponent<NetworkObject>().ChangeOwnership(clientId);
            //TODO: OtherConnectionStuff
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            var connectionString = Encoding.ASCII.GetString(connectionData);
            Debug.unityLogger.Log($"Connection approve: {connectionString}");
            var account = accountObjects.FirstOrDefault(acc => (acc.login + acc.password).GetHashCode() == connectionString.GetHashCode());
            account.clientId = clientId;
            //If approve is true, the connection gets added. If it's false. The client gets disconnected
            callback(false, null, account != null, null, null);
        }

        private void BeginReceiving(int _)
        {
            NetworkManager.Singleton.StartServer();
        }
        
        private void Update()
        {
            clientCounter.text = NetworkManager.Singleton.ConnectedClients.Count.ToString();
        }

        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
        }

        private void OnDestroy()
        {
            HandlerManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            ServerInitializeHelper.instance.SaveServer();
            NetworkManager.Singleton.StopServer();
            OnDestroy();
        }
    }

}
