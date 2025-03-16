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
            var connector = FindFirstObjectByType<ClientConnectionHelper>();
            
            base.OnClientConnect();

            return;
            
            if (AccountObject == null || AccountObject.type == UserType.Spectator)
            {
                connector.SelectScene(AccountObject?.type ?? UserType.Spectator, 0);
                return;
            }

            if (AccountObject.type == UserType.Admin || AccountObject.type == UserType.Mechanic)
            {
                connector.SelectScene(AccountObject.type, 0);
                return;
            }

            var goNetIdentity = GameObject.Find($"{AccountObject.ship.prefabName}|{AccountObject.ship.shipId}").GetComponent<NetworkIdentity>();
            var netId = goNetIdentity.netId;//NetworkObjectId;

            connector.SelectScene(AccountObject.type, netId);
            //OtherConnectionStuff
            //Передача владения объектом корабля
            if (AccountObject.type < UserType.Pilot) return;

            // goNetIdentity.AssignClientAuthority(conn); //ChangeOwnership(clientId);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }

        #endregion
        
        #region ServerSide

        public override void OnStartServer()
        {
            StarfighterSceneSpawnObjects();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Connection accepted: {conn.connectionId}");
            var player = GameObject.Find($"{AccountObject.ship.prefabName}|{AccountObject.ship.shipId}");
            NetworkServer.AddPlayerForConnection(conn, player);
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

        public override void OnStopServer()
        {
            base.OnStopServer();
            FindFirstObjectByType<ServerInitializeHelper>().SaveServer();
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
