using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class BeaconComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject sprite;
        
        private void Awake()
        {
            if (isOwned)
            {
                sprite.SetActive(false);
            }
        }

        public void ChangeState(bool state)
        {
            gameObject.SetActive(state);
            
            if (isServer)
            {
                ChangeStateClientRpc(state);
                return;
            }
            if (isOwned) 
                ChangeStateServerRpc(state);
        }
        
        [Command]
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