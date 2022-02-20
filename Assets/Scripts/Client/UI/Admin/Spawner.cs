using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Client.UI.Admin
{
    public class Spawner: NetworkBehaviour
    {
        public GameObject selectedPrefab;

        public void Spawn()
        {
            var cam = FindObjectOfType<Camera>().gameObject;
            var go = Instantiate(selectedPrefab, cam.transform.position, Quaternion.identity);
            if(go.TryGetComponent<NetworkObject>(out var netObj))
            {
                SpawnServerRpc(netObj.NetworkObjectId);
            }
        }

        public void Despawn()
        {
            if(selectedPrefab.TryGetComponent<NetworkObject>(out var netObj))
            {
                DespawnServerRpc(netObj.NetworkObjectId);
            }
        }

        [ServerRpc]
        public void SpawnServerRpc(ulong objectId)
        {
            GetNetworkObject(objectId).Spawn();
        }

        [ServerRpc]
        public void SpawnServerRpc(ulong objectId, ulong clientId)
        {
            GetNetworkObject(objectId).SpawnWithOwnership(clientId);
        }
        
        [ServerRpc]
        public void DespawnServerRpc(ulong objectId)
        {
            GetNetworkObject(objectId).Despawn(true);
        }
    }
}