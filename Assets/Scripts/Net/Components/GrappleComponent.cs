using System;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;


namespace Net.Components
{
    public class GrappleComponent: NetworkBehaviour
    {
        [SerializeField]
        private GameObject _grapplerPrefab;
        [SerializeField]
        private PlayerScript _unit;
        [SerializeField]
        private Transform _launcher;
        private Grappler _grappler;
        
        private void Update()
        {
            if (!IsOwner || _unit.isGrappled.Value) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                if (_grappler == null)
                {
                    InitGrappleServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    _grappler.DestroyOnServer(NetworkManager.LocalClientId, _grappler.grappledObject?.GetComponent<NetworkObject>().NetworkObjectId ?? default);
                }
            }
        }

        [ServerRpc]
        private void InitGrappleServerRpc(ulong clientId)
        {
            var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
            grapplerGo.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, destroyWithScene: true);
            _grappler = grapplerGo.GetComponent<Grappler>();
            _grappler?.Init(_unit, 20);
            InitGrappleClientRpc(_grappler.NetworkObjectId);
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void InitGrappleClientRpc(ulong objectId)
        {
            //BUG: fixed(?) by kostil'
            try
            {
                _grappler = GetNetworkObject(objectId)?.GetComponent<Grappler>();
                if (_grappler is null)
                {
                    FixByDestroyServerRpc();
                }
                _grappler?.Init(_unit, 20);
            }
            catch (Exception exception)
            {
                FixByDestroyServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void FixByDestroyServerRpc()
        {
            _grappler.DestroyOnServer(NetworkManager.LocalClientId, _grappler.grappledObject?.GetComponent<NetworkObject>().NetworkObjectId ?? default);
        }
    }
}