using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Net.Components
{
    public class BeaconComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject sprite;
        
        private void Awake()
        {
            if (IsOwner)
            {
                sprite.SetActive(false);
            }
        }

        public void ChangeState(bool state)
        {
            gameObject.SetActive(state);
            
            if (IsServer)
            {
                ChangeStateClientRpc(state);
                return;
            }
            if (IsOwner) 
                ChangeStateServerRpc(state);
        }
        
        [ServerRpc]
        private void ChangeStateServerRpc(bool state)
        {
            gameObject.SetActive(state);
            ChangeStateClientRpc(state);
        }

        [ClientRpc]
        private void ChangeStateClientRpc(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}