using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Mirror;
using ScriptableObjects;
using UnityEngine;

namespace Net.Core
{
    public class StarfighterAuthenticator : NetworkAuthenticator
    {
        readonly HashSet<NetworkConnectionToClient> connectionsPendingDisconnect = new HashSet<NetworkConnectionToClient>();
        
        [Header("Client Credentials")]
        public string username;
        public string password;
        
        public List<ClientAccountObject> accountObjects;
        
        #region Messages

        public struct AuthRequestMessage : NetworkMessage
        {
            // use whatever credentials make sense for your game
            // for example, you might want to pass the accessToken if using oauth
            public string authUsername;
            public string authPassword;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte code;
            public string message;
            public ClientAccountObject accountDetails;
            public uint shipNetId; // 0 для ролей без корабля
        }

        public struct SceneSwitchMessage : NetworkMessage
        {
            public UserType type;
            public uint shipNetId; // 0 для ролей без корабля
        }

        #endregion
        
        #region Server
        
        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        /// <summary>
        /// Called on server from StopServer to reset the Authenticator
        /// <para>Server message handlers should be unregistered in this method.</para>
        /// </summary>
        public override void OnStopServer()
        {
            // unregister the handler for the authentication request
            NetworkServer.UnregisterHandler<AuthRequestMessage>();
        }

        /// <summary>
        /// Called on server from OnServerConnectInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            // do nothing...wait for AuthRequestMessage from client
        }
        
        
        public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            if (connectionsPendingDisconnect.Contains(conn)) return;
            
            Debug.unityLogger.Log($"Connection approve: {msg.authUsername}:{msg.authPassword}");
            
            var account = accountObjects
                .FirstOrDefault(acc => acc.login == msg.authUsername 
                                       && acc.password == msg.authPassword);

            AuthResponseMessage responseMessage;
            
            if (account == null)
            {
                connectionsPendingDisconnect.Add(conn);
                Debug.unityLogger.Log($"Wrong login\\password pair: {msg.authUsername}:{msg.authPassword}");
                ServerReject(conn);

                responseMessage = new AuthResponseMessage()
                {
                    code = 200,
                    message = "Wrong login\\password pair",
                    accountDetails = null
                };
                conn.Send(responseMessage);
                conn.isAuthenticated = false;
                StartCoroutine(DelayedDisconnect(conn, 1f));
                return;
            }
            if(account.connectionId != null)
            {
                connectionsPendingDisconnect.Add(conn);
                Debug.unityLogger.Log($"Account already connected: {msg.authUsername}");
                ServerReject(conn);
                
                responseMessage = new AuthResponseMessage()
                {
                    code = 201,
                    message = "Already connected",
                    accountDetails = null
                };
                
                conn.Send(responseMessage);
                conn.isAuthenticated = false;
                StartCoroutine(DelayedDisconnect(conn, 1f));
                return;
            }

            account.connectionId = conn.connectionId;
            
            responseMessage = new AuthResponseMessage()
            {
                code = 100,
                message = "Success",
                accountDetails = account
            };

            conn.Send(responseMessage);
            // Note: Server no longer sets the global NetworkManager AccountObject.
            // Each client's account is resolved by connectionId in OnServerAuthenticated.
            ServerAccept(conn);
        }
        
        private IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            // Reject the unsuccessful authentication
            ServerReject(conn);

            yield return null;

            // remove conn from pending connections
            connectionsPendingDisconnect.Remove(conn);
        }
        
        #endregion

        #region Client

        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
            NetworkClient.RegisterHandler<SceneSwitchMessage>(OnSceneSwitchMessage, false);
        }

        /// <summary>
        /// Called on client from StopClient to reset the Authenticator
        /// <para>Client message handlers should be unregistered in this method.</para>
        /// </summary>
        public override void OnStopClient()
        {
            // unregister the handler for the authentication response
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
            NetworkClient.UnregisterHandler<SceneSwitchMessage>();
        }

        /// <summary>
        /// Called on client from OnClientConnectInternal when a client needs to authenticate
        /// </summary>
        public override void OnClientAuthenticate()
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = username,
                authPassword = password
            };

            NetworkClient.Send(authRequestMessage);
        }

        /// <summary>
        /// Called on client when the server's AuthResponseMessage arrives
        /// </summary>
        /// <param name="msg">The message payload</param>
        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                //Debug.Log($"Authentication Response: {msg.message}");

                StarfighterNetworkManager.singleton.AccountObject = msg.accountDetails;
                // Authentication has been accepted
                ClientAccept();
                NetworkClient.Ready();
            }
            else
            {
                Debug.LogError($"Authentication Response: {msg.message}");

                // Authentication has been rejected
                ClientReject();
            }
        }

        public void OnSceneSwitchMessage(SceneSwitchMessage msg)
        {
            StartCoroutine(SelectSceneWhenReady(msg));
        }
        
        private IEnumerator SelectSceneWhenReady(SceneSwitchMessage msg)
        {
            if (msg.shipNetId != 0)
            {
                float timeout = 30f;
                float elapsed = 0f;
                bool found = false;
                
                while (elapsed < timeout)
                {
                    if (NetworkClient.spawned.TryGetValue(msg.shipNetId, out _))
                    {
                        found = true;
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                    elapsed += 0.1f;
                }
                
                if (!found)
                {
                    Debug.LogError($"[SelectSceneWhenReady] Ship netId={msg.shipNetId} not found within {timeout}s!");
                }
            }
            
            var helper = FindFirstObjectByType<ClientConnectionHelper>();
            helper.SelectScene(msg.type, msg.shipNetId);
        }

        #endregion
    }
}