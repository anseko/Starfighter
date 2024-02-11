using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mirror;
using Net.Core;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NetworkManager = Mirror.NetworkManager;

namespace Core
{
    [RequireComponent(typeof(ServerInitializeHelper))]
    public class StarfighterNetworkManager : NetworkManager
    {
        public Image indicator;
        public TextMeshProUGUI clientCounter;
        public List<ClientAccountObject> accountObjects;
        [SerializeField] private ConnectionHelper _connector;

        public override void Awake()
        {
            NetEventStorage.GetInstance().WorldInit.AddListener(_ => StartServer());
            base.Awake();
        }

        public override void Start()
        {
            try
            {
                var spacefield = File.ReadAllText(Constants.PathToAsteroids);
                var field = Resources.Load<GameObject>(Constants.PathToPrefabs + spacefield);
                var fieldGO = Instantiate(field, Vector3.zero, new Quaternion(0, 180, 0, 1));
                StartCoroutine(GetComponent<ServerInitializeHelper>().InitServer());
            }
            catch (FileNotFoundException notFoundException)
            {
                var spacefield = File.ReadAllText(Constants.PathToAsteroids + "Spacefield_Test");
                var field = Resources.Load<GameObject>(Constants.PathToPrefabs + spacefield);
                var fieldGO = Instantiate(field, Vector3.zero, Quaternion.identity);
                StartCoroutine(GetComponent<ServerInitializeHelper>().InitServer());
            }
            finally
            {
                base.Start();
            }
        }
        
        
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Connection accepted: {conn.connectionId}");
            var account = accountObjects.FirstOrDefault(x => x.connectionId == conn.connectionId);

            if (account == null || account.type == UserType.Spectator)
            {
                _connector.userType = account?.type ?? UserType.Spectator;
                _connector.SelectSceneClientRpc(conn, account?.type ?? UserType.Spectator, 0);
                return;
            }

            if (account.type == UserType.Admin || account.type == UserType.Mechanic)
            {
                _connector.userType = account.type;
                _connector.SelectSceneClientRpc(conn, account.type, 0);
                return;
            }

            var goNetIdentity = GameObject.Find($"{account.ship.prefabName}|{account.ship.shipId}").GetComponent<NetworkIdentity>();
            var netId = goNetIdentity.netId;//NetworkObjectId;

            _connector.userType = account.type;
            _connector.SelectSceneClientRpc(conn, account.type, netId);
            //OtherConnectionStuff
            //Передача владения объектом корабля
            if (account.type < UserType.Pilot) return;

            goNetIdentity.AssignClientAuthority(conn); //ChangeOwnership(clientId);
            base.OnServerConnect(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            var account = accountObjects.FirstOrDefault(x => x.connectionId == conn.connectionId);

            if (account != null) account.connectionId = null;

            Debug.unityLogger.Log($"Disconnection: {conn.connectionId}");
            //TODO:
            // foreach (var grappler in FindObjectsOfType<Grappler>().Where(x => x.OwnerClientId == clientId))
            // {
            //     grappler.DestroyOnServer(); //передаст владение серверу
            // }

            base.OnServerDisconnect(conn);
        }

        public override void Update()
        {
            clientCounter.text = singleton.numPlayers.ToString();
            base.Update();
        }

        public bool CheckForAccountId(int connectionId, string shipId)
        {
            return accountObjects.FirstOrDefault(x => x.connectionId == connectionId && x.type == UserType.Pilot)?.ship.shipId == shipId;
        }

        public IEnumerable<int> GetClientsOfType(UserType type) => accountObjects
            .Where(x => x.type == type && x.connectionId.HasValue).Select(x => x.connectionId.Value);

        public override void OnApplicationQuit()
        {
            GetComponent<ServerInitializeHelper>().SaveServer();
            singleton.StopServer();
            base.OnApplicationQuit();
        }
        
        
        private void ApprovalCheck(byte[] connectionData, int connectionId, NetworkManager.ConnectionApprovedDelegate callback)
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
            if(account.connectionId != null)
            {
                Debug.unityLogger.Log($"Account already connected: {connectionString}");
                callback(false, null, false, null, null);
                return;
            }
            if(account.type != UserType.Spectator)
                account.connectionId = connectionId;
            //If approve is true, the connection gets added. If it's false. The client gets disconnected
            callback(false, null, account != null, null, null);
        }
        
    }
}
