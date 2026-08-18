using System.Collections.Generic;
using System.Linq;
using Mirror;
using Net;
using Net.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NetworkManager = Mirror.NetworkManager;

namespace Core
{
    public class StarfighterNetworkManager : NetworkManager
    {
        public new static StarfighterNetworkManager singleton => (StarfighterNetworkManager)NetworkManager.singleton;
        public Image indicator;

        #region ClientSide

        public ClientAccountObject AccountObject;
        
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
            Debug.LogWarning("=== OnStartServer ENTER ===");
            StarfighterSceneSpawnObjects();
            
            if (authenticator != null)
            {
                authenticator.OnServerAuthenticated.AddListener(OnPlayerAuthenticated);
                Debug.LogWarning($"=== Subscribed to OnServerAuthenticated. Authenticator: {authenticator.name} ===");
            }
            else
            {
                Debug.LogError("=== authenticator is NULL ===");
            }
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.LogWarning($"=== OnServerConnect ENTER. connId: {conn.connectionId} ===");
        }

        public override void OnStopServer()
        {
            if (authenticator != null)
                authenticator.OnServerAuthenticated.RemoveListener(OnPlayerAuthenticated);
            
            base.OnStopServer();
            FindFirstObjectByType<ServerInitializeHelper>().SaveServer();
        }

        private void OnPlayerAuthenticated(NetworkConnectionToClient conn)
        {
            Debug.LogWarning($"=== OnPlayerAuthenticated ENTER. connId: {conn.connectionId} ===");
            
            var auth = (StarfighterAuthenticator)authenticator;
            var account = auth.accountObjects.FirstOrDefault(x => x.connectionId == conn.connectionId);

            if (account == null)
            {
                Debug.LogError($"[OnPlayerAuthenticated] Account not found for connection {conn.connectionId}");
                return;
            }

            if (account.type == UserType.Spectator)
            {
                Debug.Log($"[OnPlayerAuthenticated] Connection {conn.connectionId} is Spectator — no player object.");
                conn.Send(new StarfighterAuthenticator.SceneSwitchMessage { type = UserType.Spectator, shipNetId = 0 });
                return;
            }

            uint shipNetId = 0;
            var ship = GameObject.Find($"{account.ship.prefabName}|{account.ship.shipId}");
            if (ship != null)
                shipNetId = ship.GetComponent<NetworkIdentity>().netId;
            else
                Debug.LogError($"[OnPlayerAuthenticated] Ship {account.ship.prefabName}|{account.ship.shipId} not found for account '{account.login}'!");

            if (account.type == UserType.Pilot)
            {
                if (ship != null)
                {
                    NetworkServer.AddPlayerForConnection(conn, ship);
                    Debug.Log($"[OnPlayerAuthenticated] Pilot '{account.login}' assigned to ship {account.ship.prefabName}|{account.ship.shipId}");
                }
            }
            else
            {
                Debug.Log($"[OnPlayerAuthenticated] Role {account.type} for connection {conn.connectionId} — no authority assigned at start.");
            }

            conn.Send(new StarfighterAuthenticator.SceneSwitchMessage { type = account.type, shipNetId = shipNetId });
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            var account = ((StarfighterAuthenticator)authenticator).accountObjects.FirstOrDefault(x => x.connectionId == conn.connectionId);

            if (account != null) account.connectionId = null;

            Debug.unityLogger.Log($"Disconnection: {conn.connectionId}");
            //TODO:
            // foreach (var grappler in FindObjectsOfType<Grappler>().Where(x => x.OwnerClientId == clientId))
            // {
            //     grappler.DestroyOnServer(); //передаст владение серверу
            // }

            NetworkServer.RemovePlayerForConnection(conn, RemovePlayerOptions.KeepActive);
        }

        public bool CheckForAccountId(int connectionId, string shipId)
        {
            return ((StarfighterAuthenticator)authenticator).accountObjects.FirstOrDefault(x => x.connectionId == connectionId && x.type == UserType.Pilot)?.ship.shipId == shipId;
        }

        public IEnumerable<int> GetClientsOfType(UserType type) => ((StarfighterAuthenticator)authenticator).accountObjects
            .Where(x => x.type == type && x.connectionId.HasValue).Select(x => x.connectionId.Value);
        
        
        private static bool StarfighterSceneSpawnObjects()
        {
            // find all NetworkIdentities in the scene.
            // all of them are disabled because of NetworkScenePostProcess.
            NetworkIdentity[] identities = FindObjectsByType<NetworkIdentity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            // second pass: spawn all scene objects
            foreach (NetworkIdentity identity in identities)
            {
                // scene objects may be children of inactive parents.
                // users would put them under disabled parents to 'deactivate' them.
                // those should not be used by Mirror at all.
                // fixes: https://github.com/MirrorNetworking/Mirror/issues/3330
                //        https://github.com/vis2k/Mirror/issues/2778
                if (identity.netId == 0 && identity.transform.parent == null ||
                    identity.transform.parent.gameObject.activeInHierarchy)
                {
                    // pass connection so that authority is not lost when server loads a scene
                    // https://github.com/vis2k/Mirror/pull/2987
                    NetworkServer.Spawn(identity.gameObject, identity.connectionToClient);
                }
            }

            return true;
        }
        
        #endregion
    }
}
