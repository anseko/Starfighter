using Client.Core;
using Mirror;
using Net.Components;
using UnityEngine;


namespace Core
{
    public class Grappler : NetworkBehaviour
    {
        public GameObject grappledObject { get; private set; }

        [SyncVar] public ulong grappledObjectId;
        
        private Joint _joint;
        private LineRenderer _lineRenderer;
        private GameObject _owner;
        [SyncVar] public ulong ownerObjectId;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _grappleVelocity;

    
        //Calls only on server side
        public void Init(PlayerScript playerScript, float maxLength)
        {
            ownerObjectId = playerScript.netId; 
            _maxLength = maxLength;
            
            var forceVector = _owner.transform.Find("Back").position - _owner.transform.position;
            
            GetComponent<Rigidbody>().AddForce(forceVector.normalized * _grappleVelocity,
                ForceMode.Impulse);
        }
    
        private void Awake()
        {
            // grappledObjectId = new NetworkVariable<ulong>(new NetworkVariableSettings(){
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission =  NetworkVariablePermission.OwnerOnly
            // }, 0);
            //
            // ownerObjectId = new NetworkVariable<ulong>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission =  NetworkVariablePermission.OwnerOnly
            // });

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
            if (!isOwned) return;
            
            var distance = (_owner.transform.position - transform.position).magnitude;
            if (grappledObject == null &&
                _maxLength < distance)
            {
                DestroyOnServer();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isServer) return;
            
            Destroy(GetComponent<Collider>());
            
            grappledObject = other.transform.root.gameObject;
            
            if(!grappledObject.TryGetComponent<NetworkIdentity>(out var netObject) 
               || !netObject.IsOwnedByServer)
            {
                DestroyOnServer();
                return;
            }
            
            grappledObjectId = netObject.netId;

            var ownerClientId = GetNetworkObject(ownerObjectId).OwnerClientId;
            
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
            OnGrappleClientRpc(connectionToClient, other.GetContact(0).point, grappledObjectId);
        }

        [TargetRpc]
        private void OnGrappleClientRpc(NetworkConnectionToClient connectionToClient, Vector3 contactPoint, ulong grappledObjId)
        {
            grappledObject = GetNetworkObject(grappledObjId).gameObject;
            
            if (grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
            {
                moveComponent.enabled = false;
            }

            if (grappledObject.TryGetComponent<UnitScript>(out var unitScript))
            {
                unitScript.isGrappled = true;
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
            if (isServer)
            {
                Debug.unityLogger.Log($"DestroyOnServer destroy: {grappledObjectId}");
                grappledObject = GetNetworkObject(grappledObjectId)?.gameObject;

                if (grappledObject != null &&
                    grappledObject.TryGetComponent<UnitScript>(out var unitScript))
                {
                    unitScript.isGrappled = false;
                    var networkObject = grappledObject.GetComponent<NetworkIdentity>();
                    if(!networkObject.IsOwnedByServer)
                        networkObject.RemoveClientAuthority();
                }
                if (grappledObject != null && 
                    grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
                {
                    moveComponent.enabled = true;
                }

                if (gameObject.GetComponent<NetworkIdentity>().IsSpawned)
                {
                    gameObject.GetComponent<NetworkIdentity>().Despawn(true);
                }
                
                _owner.GetComponent<GrappleComponent>().grapplerObjectId = default;
            }
            else
            {
                if(!isOwned) return;
                foreach (var component in _owner.GetComponents<Joint>())
                {
                    Destroy(component);
                }
                DestroyServerRpc();
            }
        }
        
        [Command]
        private void DestroyServerRpc()
        {
            Debug.unityLogger.Log($"DestroyServerRpc destroy: {grappledObjectId}");
            grappledObject = GetNetworkObject(grappledObjectId)?.gameObject;

            if (grappledObject != null &&
                grappledObject.TryGetComponent<UnitScript>(out var unitScript))
            {
                unitScript.isGrappled = false;
                var networkObject = grappledObject.GetComponent<NetworkIdentity>();
                if(!networkObject.IsOwnedByServer)
                    networkObject.RemoveOwnership();
            }
            if (grappledObject != null && 
                grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
            {
                moveComponent.enabled = true;
            }

            if (gameObject.GetComponent<NetworkIdentity>().IsSpawned)
            {
                gameObject.GetComponent<NetworkIdentity>().Despawn(true);
            }
            _owner.GetComponent<GrappleComponent>().grapplerObjectId = default;
        }
    }
}
