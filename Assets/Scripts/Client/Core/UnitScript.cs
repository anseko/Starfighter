using System.Linq;
using Core;
using Core.Models;
using Mirror;
using UnityEngine;

namespace Client.Core
{
    public class UnitScript : NetworkBehaviour
    {
        public NetworkSpaceUnitDto networkUnitConfig;
        public SpaceUnitDto unitConfig;
        private NetworkIdentity _networkIdentity;

        [SyncVar] public bool isGrappled;
        
        public virtual UnitState GetState() => UnitState.InFlight;

        public void Awake()
        {
           networkUnitConfig = gameObject.AddComponent<NetworkSpaceUnitDto>();
           _networkIdentity = gameObject.GetComponent<NetworkIdentity>();
        }
        
        public void RequestShipOwnership()
        {
            if (isServer) return;
            
            RequestShipOwnershipServerRpc(NetworkManager.singleton.LocalClientId);
        }
        
        [Command(requiresAuthority = false)]
        private void RequestShipOwnershipServerRpc(int connectionId)
        {
            Debug.unityLogger.Log($"Ownership requestig for {connectionId}");
            if (!FindObjectOfType<StarfighterNetworkManager>().CheckForAccountId(connectionId, networkUnitConfig.shipId)) return;
            if (GetComponent<UnitScript>().isGrappled) // если подключается к схваченному кораблю - отпустить
            {
                foreach (var grappler in FindObjectsOfType<Grappler>().Where(x=>x.grappledObject == gameObject))
                {
                    Destroy(grappler); //передаст владение серверу
                }
            }

            //BUG?? _networkIdentity.AssignClientAuthority(connectionToClient);
            NetworkObject.ChangeOwnership(connectionId);
        }
        
        [Command]
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