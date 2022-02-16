#nullable enable
using System.Linq;
using Core;
using Core.Models;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Net;
using UnityEngine;

namespace Client.Core
{
    public class UnitScript : NetworkBehaviour
    {
        public NetworkVariable<SpaceShipDto> unitConfig;

        public NetworkVariable<bool> isGrappled = new NetworkVariable<bool>(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission =  NetworkVariablePermission.Everyone
        });
        
        public virtual UnitState GetState() => UnitState.InFlight;

        private void Awake()
        {
            unitConfig = new NetworkVariable<SpaceShipDto>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer,
            });
        }
        
        public void RequestShipOwnership()
        {
            if (IsServer) return;
            
            RequestShipOwnershipServerRpc(NetworkManager.LocalClientId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RequestShipOwnershipServerRpc(ulong clientId)
        {
            Debug.unityLogger.Log($"Ownership requestig for {clientId}");
            if (unitConfig.Value is SpaceShipDto shipConfig && !FindObjectOfType<MainServerLoop>().CheckForAccountId(clientId, shipConfig.shipId)) return;
            if (GetComponent<UnitScript>().isGrappled.Value) // если подключается к схваченному кораблю - отпустить
            {
                foreach (var grappler in FindObjectsOfType<Grappler>().Where(x=>x.grappledObject == gameObject))
                {
                    Destroy(grappler); //передаст владение серверу
                }
            }
            
            NetworkObject.ChangeOwnership(clientId);
        }
        
        [ServerRpc(RequireOwnership = true)]
        public void GiveAwayShipOwnershipServerRpc()
        {
            NetworkObject.RemoveOwnership();
            var rigidbody = NetworkObject.gameObject.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            var force = rigidbody.gameObject.GetComponent<ConstantForce>();
            force.force = Vector3.zero;
            force.torque = Vector3.zero;

        }
    }
}