using Client.Core;
using Net.Components;
using Unity.Netcode;
using UnityEngine;


namespace Core
{
    public class Grappler : NetworkBehaviour
    {
        public GameObject grappledObject { get; private set; }
        public NetworkVariable<ulong> grappledObjectId { get; private set; }
        
        private Joint _joint;
        private LineRenderer _lineRenderer;
        private GameObject _owner;
        public NetworkVariable<ulong> ownerObjectId;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _grappleVelocity;

    
        //Calls only on server side
        public void Init(PlayerScript playerScript, float maxLength)
        {
            ownerObjectId.Value = playerScript.NetworkObjectId; 
            _maxLength = maxLength;
            
            var forceVector = _owner.transform.Find("Back").position - _owner.transform.position;
            
            GetComponent<Rigidbody>().AddForce(forceVector.normalized * _grappleVelocity,
                ForceMode.Impulse);
        }
    
        private void Awake()
        {
            grappledObjectId = new NetworkVariable<ulong>((ulong)0);
            
            ownerObjectId = new NetworkVariable<ulong>();

            grappledObjectId.OnValueChanged += (value, newValue) =>
            {
                if (newValue == 0) return;
                grappledObject = GetNetworkObject(newValue).gameObject;
            };

            ownerObjectId.OnValueChanged += (value, newValue) =>
            {
                if (newValue == value) return;
                _owner = GetNetworkObject(newValue).gameObject;
                if(_owner == null) DestroyOnServer();
            };
            
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        private void Update()
        {
            if (_owner == null) return;
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _owner.transform.position);
            if (!IsOwner) return;
            
            var distance = (_owner.transform.position - transform.position).magnitude;
            if (grappledObject == null &&
                _maxLength < distance)
            {
                DestroyOnServer();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!IsServer) return;
            
            Destroy(GetComponent<Collider>());
            
            grappledObject = other.transform.root.gameObject;
            
            if(!grappledObject.TryGetComponent<NetworkObject>(out var netObject) 
               || !netObject.IsOwnedByServer)
            {
                DestroyOnServer();
                return;
            }
            
            grappledObjectId.Value = netObject.NetworkObjectId;

            var ownerClientId = GetNetworkObject(ownerObjectId.Value).OwnerClientId;
            
            NetworkObject.ChangeOwnership(ownerClientId);
            
            if (grappledObject.TryGetComponent<Rigidbody>(out _))
            {
                //Give away ownership to owner of grappler
                GetNetworkObject(netObject.NetworkObjectId).ChangeOwnership(ownerClientId);
            }

            var clientRpcParams = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] { ownerClientId }
                }
            };
            OnGrappleClientRpc(other.GetContact(0).point, grappledObjectId.Value, clientRpcParams);
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void OnGrappleClientRpc(Vector3 contactPoint, ulong grappledObjId, ClientRpcParams clientRpcParams = default)
        {
            grappledObject = GetNetworkObject(grappledObjId).gameObject;
            
            if (grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
            {
                moveComponent.enabled = false;
            }

            if (grappledObject.TryGetComponent<UnitScript>(out var unitScript))
            {
                unitScript.isGrappled.Value = true;
            }
            
            //Add joint to _grappledObject
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = grappledObject.GetComponent<Rigidbody>();
            _joint.connectedAnchor = contactPoint;
            _joint.enableCollision = false;
            _joint.autoConfigureConnectedAnchor = true;
            _joint.axis = Vector3.up;

            //Add joint to _owner
            var joint = _owner.AddComponent<FixedJoint>();
            joint.connectedMassScale = 0.5f;
            joint.axis = Vector3.up;
            joint.connectedBody = gameObject.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = true;
        }

        private void OnJointBreak(float breakForce)
        {
            Debug.unityLogger.Log($"Joint breaks with force: {breakForce}");
            DestroyOnServer();
        }

        public void DestroyOnServer()
        {
            if (IsServer)
            {
                Debug.unityLogger.Log($"DestroyOnServer destroy: {grappledObjectId.Value}");
                grappledObject = GetNetworkObject(grappledObjectId.Value)?.gameObject;

                if (grappledObject != null &&
                    grappledObject.TryGetComponent<UnitScript>(out var unitScript))
                {
                    unitScript.isGrappled.Value = false;
                    var networkObject = grappledObject.GetComponent<NetworkObject>();
                    if(!networkObject.IsOwnedByServer)
                        networkObject.RemoveOwnership();
                }
                if (grappledObject != null && 
                    grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
                {
                    moveComponent.enabled = true;
                }

                if (gameObject.GetComponent<NetworkObject>().IsSpawned)
                {
                    gameObject.GetComponent<NetworkObject>().Despawn(true);
                }
                
                _owner.GetComponent<GrappleComponent>().grapplerObjectId.Value = default;
            }
            else
            {
                if(!IsOwner) return;
                foreach (var component in _owner.GetComponents<Joint>())
                {
                    Destroy(component);
                }
                DestroyServerRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = true)]
        private void DestroyServerRpc()
        {
            Debug.unityLogger.Log($"DestroyServerRpc destroy: {grappledObjectId.Value}");
            grappledObject = GetNetworkObject(grappledObjectId.Value)?.gameObject;

            if (grappledObject != null &&
                grappledObject.TryGetComponent<UnitScript>(out var unitScript))
            {
                unitScript.isGrappled.Value = false;
                var networkObject = grappledObject.GetComponent<NetworkObject>();
                if(!networkObject.IsOwnedByServer)
                    networkObject.RemoveOwnership();
            }
            if (grappledObject != null && 
                grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
            {
                moveComponent.enabled = true;
            }

            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn(true);
            }
            _owner.GetComponent<GrappleComponent>().grapplerObjectId.Value = default;
        }
    }
}
