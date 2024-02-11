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
    public class StarfighterNetworkManager : NetworkManager
    {
        public Image indicator;
        public List<ClientAccountObject> accountObjects;
        [SerializeField] private ClientConnectionHelper _connector;
        

        #region ClientSide

        public override void OnClientConnect()
        {
            base.OnClientConnect();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        #endregion
        
        #region ServerSide

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            AuthCheck(conn);

            if (!conn.isAuthenticated)
            {
                conn.Disconnect();
                return;
            }

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

        public bool CheckForAccountId(int connectionId, string shipId)
        {
            return accountObjects.FirstOrDefault(x => x.connectionId == connectionId && x.type == UserType.Pilot)?.ship.shipId == shipId;
        }

        public IEnumerable<int> GetClientsOfType(UserType type) => accountObjects
            .Where(x => x.type == type && x.connectionId.HasValue).Select(x => x.connectionId.Value);

        public override void OnApplicationQuit()
        {
            GetComponent<ServerInitializeHelper>().SaveServer();
            StopServer();
            base.OnApplicationQuit();
        }
        
        
        private void AuthCheck(NetworkConnectionToClient connectionData)
        {
            var authToken = connectionData.authenticationData.ToString();
            Debug.unityLogger.Log($"Connection approve: {authToken}");
            var account = accountObjects.FirstOrDefault(acc => (acc.login + acc.password).GetHashCode() == authToken.GetHashCode());
            if (account == null)
            {
                Debug.unityLogger.Log($"Wrong login\\password pair: {authToken}");
                connectionData.isAuthenticated = false;
                return;
            }
            if(account.connectionId != null)
            {
                Debug.unityLogger.Log($"Account already connected: {authToken}");
                connectionData.isAuthenticated = false;
                return;
            }
            if(account.type != UserType.Spectator)
                account.connectionId = connectionData.connectionId;
            connectionData.isAuthenticated = true;
        }
        
        #endregion
    }
}
