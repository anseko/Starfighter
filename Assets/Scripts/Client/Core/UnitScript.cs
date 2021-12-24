using System.Linq;
using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using ScriptableObjects;
using UnityEngine;

namespace Client.Core
{
    public class UnitScript : NetworkBehaviour
    {
        public SpaceUnitConfig unitConfig;

        public NetworkVariable<float> currentHp = new NetworkVariable<float>(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.ServerOnly
        });
        
        public NetworkVariable<bool> isGrappled = new NetworkVariable<bool>(new NetworkVariableSettings()
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission =  NetworkVariablePermission.Everyone
        });
        
        public virtual UnitState GetState() => UnitState.InFlight;
        
        public void RequestShipOwnership()
        {
            RequestShipOwnershipServerRpc(NetworkManager.LocalClientId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestShipOwnershipServerRpc(ulong clientId)
        {
            Debug.unityLogger.Log($"Ownership requestig for {clientId}");
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
        }
    }
}