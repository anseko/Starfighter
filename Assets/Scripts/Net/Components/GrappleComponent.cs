using Client.Core;
using Core;
using Mirror;
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
        [SyncVar]
        public ulong grapplerObjectId;
        private bool _grappleOut;

        private void Awake()
        {
            // GrappleOut = new NetworkVariable<bool>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = id => IsOwner || IsServer 
            // });
            
            // grapplerObjectId = new NetworkVariable<ulong>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.ServerOnly
            // });

            _unit ??= gameObject.GetComponent<PlayerScript>();
        }

        private void Update()
        {
            if (!isOwned || _unit.isGrappled) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                if (grapplerObjectId != default)
                {
                    var grappler = GetNetworkObject(grapplerObjectId)?.GetComponent<Grappler>();
                    grappler?.DestroyOnServer();
                }
                else
                {
                    InitGrappleServerRpc();   
                }
            }
        }

        [Command]
        private void InitGrappleServerRpc()
        {
            var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
            var netGrappler = grapplerGo.GetComponent<NetworkIdentity>();
            netGrappler.Spawn(destroyWithScene: true);
            grapplerObjectId = netGrappler.netId;
            grapplerGo.GetComponent<Grappler>().Init(_unit, 20);
        }
    }
}