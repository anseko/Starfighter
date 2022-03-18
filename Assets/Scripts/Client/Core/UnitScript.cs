using System.Linq;
using Core;
using Core.Models;
using Net;
using Unity.Netcode;
using UnityEngine;

namespace Client.Core
{
    public class UnitScript : NetworkBehaviour
    {
        public NetworkSpaceUnitDto NetworkUnitConfig;
        public SpaceUnitDto unitConfig;

        public NetworkVariable<bool> isGrappled = new NetworkVariable<bool>();
        
        public virtual UnitState GetState() => UnitState.InFlight;

        public void Awake()
        {
           NetworkUnitConfig = gameObject.AddComponent<NetworkSpaceUnitDto>();
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
            if (!FindObjectOfType<MainServerLoop>().CheckForAccountId(clientId, NetworkUnitConfig.ShipId)) return;
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