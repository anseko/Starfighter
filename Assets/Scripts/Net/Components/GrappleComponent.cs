using System;
using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.Serialization;

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
            if (!IsOwner) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                Debug.unityLogger.Log($"KEY PRESSED: {gameObject.name}");
                if (_grappler == null)
                {
                    var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
                    _grappler = grapplerGo.GetComponent<Grappler>();
                    _grappler.Init(_unit, 20);
                }
                else
                {
                    Destroy(_grappler.gameObject);
                }
            }
        }

        [ServerRpc]
        public void ApplyGrappleServerRpc(ulong networkObjectId)
        {
            ApplyGrappleClientRpc(networkObjectId);
        }

        [ClientRpc]
        private void ApplyGrappleClientRpc(ulong networkObjectId)
        {
            if (NetworkObjectId == networkObjectId && IsOwner)
            {
                
            }
        }
    }
}