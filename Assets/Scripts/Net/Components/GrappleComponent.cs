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
        private NetworkVariableULong _grapplerObjectId;
        public bool GrappleOut { get; private set; }

        private void Awake()
        {
            _grapplerObjectId = new NetworkVariableULong(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });

            _grapplerObjectId.OnValueChanged += InitGrapple;
            
            _unit ??= gameObject.GetComponent<PlayerScript>();
        }

        private void Update()
        {
            if (!IsOwner || _unit.isGrappled.Value) return;
            if (Input.GetKeyDown(_unit.keyConfig.grapple))
            {
                if (!GrappleOut)
                {
                    GrappleOut = true;
                    InitGrappleServerRpc(NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    GrappleOut = false;
                    var grappler = GetNetworkObject(_grapplerObjectId.Value)?.GetComponent<Grappler>();
                    grappler?.DestroyOnServer();
                }
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable)]
        private void InitGrappleServerRpc(ulong clientId)
        {
            var grapplerGo = Instantiate(_grapplerPrefab, _launcher.position, Quaternion.identity);
            var netGrappler = grapplerGo.GetComponent<NetworkObject>();
            netGrappler.SpawnWithOwnership(clientId, destroyWithScene: true);
            _grapplerObjectId.Value = netGrappler.NetworkObjectId;
            
            var clientIds = FindObjectOfType<MainServerLoop>().GetClientsOfType(UserType.Navigator);
            
        }
        
        private void InitGrapple(ulong oldValue, ulong value)
        {
            GetNetworkObject(value)?.GetComponent<Grappler>()?.Init(_unit, 20);
        }

        public void SetGrappleState(bool newState)
        {
            GrappleOut = newState;
        }
    }
}