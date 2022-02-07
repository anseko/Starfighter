using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.Messaging;
using Net.Core;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
    [RequireComponent(typeof(ServerInitializeHelper))]
    public class MainServerLoop : MonoBehaviour
    {
        public Image indicator;
        public TextMeshProUGUI clientCounter;
        [SerializeField] private List<ClientAccountObject> accountObjects;
        [SerializeField] private ConnectionHelper _connector;

        private void Awake()
        {
            NetEventStorage.GetInstance().WorldInit.AddListener(BeginReceiving);
        }

        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectCallback;
            StartCoroutine(GetComponent<ServerInitializeHelper>().InitServer());
        }
        
        private void OnDisconnectCallback(ulong clientId)
        {
            var account = accountObjects.FirstOrDefault(x => x.clientId == clientId);
            
            if(account != null) account.clientId = null;
            
            Debug.unityLogger.Log($"Disconnection: {clientId}");
            foreach (var grappler in FindObjectsOfType<Grappler>().Where(x=>x.OwnerClientId == clientId))
            {
                grappler.DestroyOnServer(clientId); //передаст владение серверу
            }
        }

        private void OnConnectCallback(ulong clientId)
        {
            Debug.Log($"Connection accepted: {clientId}");
            var account = accountObjects.FirstOrDefault(x => x.clientId == clientId);
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[]{clientId}
                }
            };
            
            if (account == null || account.type == UserType.Spectator || account.type == UserType.SpaceStation)
            {
                _connector.SelectSceneClientRpc(account?.type ?? UserType.Spectator, 0, UnitState.InFlight, clientRpcParams);
                return;
            }
            
            var go = GameObject.Find($"{account.ship.prefabName}|{account.ship.shipId}");
            var netId = go.GetComponent<NetworkObject>().NetworkObjectId;
            if (account.type == UserType.Pilot && go.GetComponent<PlayerScript>().isGrappled.Value)
            {
                foreach (var grappler in FindObjectsOfType<Grappler>().Where(x=>x.grappledObjectId.Value == netId))
                {
                    grappler.DestroyOnServer(clientId, netId); //передаст владение серверу
                }
            }
            
            _connector.SelectSceneClientRpc(account.type, netId, go.GetComponent<UnitScript>().GetState(), clientRpcParams);
            //OtherConnectionStuff
            //Передача владения объектом корабля
            if (account.type < UserType.Pilot) return;
            
            go.GetComponent<NetworkObject>().ChangeOwnership(clientId);
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            var connectionString = Encoding.ASCII.GetString(connectionData);
            Debug.unityLogger.Log($"Connection approve: {connectionString}");
            var account = accountObjects.FirstOrDefault(acc => (acc.login + acc.password).GetHashCode() == connectionString.GetHashCode());
            if (account == null)
            {
                Debug.unityLogger.Log($"Wrong login\\password pair: {connectionString}");
                callback(false, null, false, null, null);
                return;
            }
            if(account.clientId != null)
            {
                Debug.unityLogger.Log($"Account already connected: {connectionString}");
                callback(false, null, false, null, null);
                return;
            }
            if(account.type != UserType.Spectator)
                account.clientId = clientId;
            //If approve is true, the connection gets added. If it's false. The client gets disconnected
            callback(false, null, account != null, null, null);
        }

        private void BeginReceiving(int _)
        {
            NetworkManager.Singleton.StartServer();
            foreach (var aiComponent in FindObjectsOfType<AIComponent>())
            {
                aiComponent.Init();
            }
        }
        
        private void Update()
        {
            clientCounter.text = NetworkManager.Singleton.ConnectedClients.Count.ToString();
        }

        public bool CheckForAccountId(ulong clientId, string shipId)
        {
            return accountObjects.FirstOrDefault(x => x.clientId == clientId)?.ship.shipId == shipId;
        }

        public ulong GetClientIdByShipName(string name)
        {
            var targetClientId = accountObjects.FirstOrDefault(x => x.ship.name == name 
                                                                    && x.type == UserType.Navigator).clientId.Value;
            return targetClientId;
        }
        
        private void OnApplicationQuit()
        {
            GetComponent<ServerInitializeHelper>().SaveServer();
            NetworkManager.Singleton.StopServer();
        }
    }

}
