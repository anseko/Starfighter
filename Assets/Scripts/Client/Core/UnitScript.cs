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
        // public SpaceUnitDto unitConfig;

        [SyncVar] public bool isGrappled;
        
        public virtual UnitState GetState() => UnitState.InFlight;

        public void Awake()
        {
           networkUnitConfig = gameObject.AddComponent<NetworkSpaceUnitDto>();
        }
        
        public void RequestShipOwnership()
        {
            if (isServer) return;
            
            RequestShipOwnershipServerRpc(connectionToServer.connectionId); // NetworkManager.singleton.LocalClientId
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

            netIdentity.AssignClientAuthority(NetworkServer.connections[connectionId]);
        }
        
        [Command]
        public void GiveAwayShipOwnershipServerRpc()
        {
            netIdentity.RemoveClientAuthority();
            var rigidbody = netIdentity.gameObject.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            var force = rigidbody.gameObject.GetComponent<ConstantForce>();
            force.force = Vector3.zero;
            force.torque = Vector3.zero;
        }
    }
}