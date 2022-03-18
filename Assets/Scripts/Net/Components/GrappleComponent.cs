using Client.Core;
using Core;
using Unity.Netcode;
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
        public NetworkVariable<ulong> grapplerObjectId;
        private bool _grappleOut;

        private void Awake()
        {
            // GrappleOut = new NetworkVariable<bool>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = id => IsOwner || IsServer 
            // });
            
            grapplerObjectId = new NetworkVariable<ulong>();

            _unit ??= gameObject.GetComponent<PlayerScript>();
        }

        private void Update()
        {
            if (!IsOwner || _unit.isGrappled.Value) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                if (grapplerObjectId.Value != default)
                {
                    var grappler = GetNetworkObject(grapplerObjectId.Value)?.GetComponent<Grappler>();
                    grappler?.DestroyOnServer();
                }
                else
                {
                    InitGrappleServerRpc();   
                }
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable)]
        private void InitGrappleServerRpc()
        {
            var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
            var netGrappler = grapplerGo.GetComponent<NetworkObject>();
            netGrappler.Spawn(destroyWithScene: true);
            grapplerObjectId.Value = netGrappler.NetworkObjectId;
            grapplerGo.GetComponent<Grappler>().Init(_unit, 20);
        }
    }
}