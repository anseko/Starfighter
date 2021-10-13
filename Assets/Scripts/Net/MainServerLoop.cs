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
// using UnityEngine.UIElements;
using Utils;

namespace Net
{
    [RequireComponent(typeof(HandlerManager))]
    [RequireComponent(typeof(ServerInitializeHelper))]
    [RequireComponent(typeof(InputManager))]
    public class MainServerLoop : NetworkBehaviour
    {
        public Image indicator;
        public TextMeshProUGUI clientCounter;
        [SerializeField] private List<ClientAccountObject> accountObjects;


        private void Awake()
        {
            NetEventStorage.GetInstance().worldInit.AddListener(BeginReceiving);
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 120;
        }

        private void OnDisconnectCallback(ulong clientId)
        {
            if (NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
            {
                Debug.unityLogger.Log($"Disconnection: {clientId}");
                //TODO: OtherDisconnectionStuff
            }
            else
            {
                Debug.unityLogger.Log("Server : There is no such client to Disconnect!");
            }
        }

        private void OnConnectCallback(ulong clientId)
        {
            Debug.Log($"Connection accepted: {clientId}");
            var account = accountObjects.First(x => x.clientId == clientId);
            FindObjectOfType<ConnectionHelper>().SelectSceneClientRpc(clientId, account.type);
            //Передача владения объектом корабля
            // var go = GameObject.Find($"{account.ship.prefabName}|{account.ship.shipId}");
            // go.GetComponent<NetworkObject>().ChangeOwnership(clientId);
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
        
        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectCallback;
            StartCoroutine(ServerInitializeHelper.instance.InitServer());
        }

        public void BeginReceiving(int _)
        {
            NetworkManager.Singleton.StartServer();
        }
        
        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
        }

        private IEnumerator CollectWorldObjects()
        {
            var worldObjects = new List<WorldObject>();
            var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(obj => obj.CompareTag(Constants.DynamicTag));
            foreach (var go in allGameObjects)
            {
                var ps = go.GetComponent<PlayerScript>();
                
                if (ps)
                {
                    var rb = go.GetComponent<Rigidbody>();
                    ps.shipConfig.shipState = ps.GetState();
                    worldObjects.Add(new SpaceShip(go.name,
                        go.transform,
                        rb.velocity,
                        rb.angularVelocity,
                        new SpaceShipDto(ps.shipConfig),
                        ps.GetState()));
                    yield return null;
                    continue;
                }

                worldObjects.Add(new WorldObject(go.name, go.transform));
                yield return null;
            }

            var statePackage = new StatePackage(new StateData()
            {
                worldState = worldObjects.ToArray()
            });
            
            foreach (var client in ClientManager.instance.ConnectedClients) 
            {
                Task.Run(() => client.SendPackage(statePackage)); 
            }
        }
        
        private void OnDestroy()
        {
            NetworkManager.Singleton.StopServer();
            HandlerManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            ServerInitializeHelper.instance.SaveServer();
            OnDestroy();
        }
    }

}
