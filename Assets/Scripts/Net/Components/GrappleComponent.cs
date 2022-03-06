using Client.Core;
using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
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
        private NetworkVariable<ulong> _grapplerObjectId;
        public NetworkVariable<bool> GrappleOut { get; private set; }

        private void Awake()
        {
            GrappleOut = new NetworkVariable<bool>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer 
            });
            
            _grapplerObjectId = new NetworkVariable<ulong>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });

            _unit ??= gameObject.GetComponent<PlayerScript>();
        }

        private void Update()
        {
            if (!IsOwner || _unit.isGrappled.Value) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                if (GrappleOut.Value)
                {
                    var grappler = GetNetworkObject(_grapplerObjectId.Value)?.GetComponent<Grappler>();
                    grappler?.DestroyOnServer();
                }
                else
                {
                    InitGrappleServerRpc();   
                }
                
                GrappleOut.Value = !GrappleOut.Value;
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable)]
        private void InitGrappleServerRpc()
        {
            var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
            var netGrappler = grapplerGo.GetComponent<NetworkObject>();
            netGrappler.Spawn(destroyWithScene: true);
            _grapplerObjectId.Value = netGrappler.NetworkObjectId;
            grapplerGo.GetComponent<Grappler>().Init(_unit, 20);
        }
    }
}