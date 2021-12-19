using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Net.Components;
using UnityEngine;


namespace Core
{
    public class Grappler : NetworkBehaviour
    {
        public GameObject grappledObject { get; private set; }
        public NetworkVariable<ulong> grappledObjectId { get; private set; }
        
        private FixedJoint _joint;
        private LineRenderer _lineRenderer;
        private GameObject _owner;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _grappleVelocity;

    
        public void Init(PlayerScript playerScript, float maxLength)
        {
            _owner = playerScript.gameObject;
            _maxLength = maxLength;
            if (IsOwner)
            {
                var forceVector = _owner.transform.Find("Back").position - _owner.transform.position;
                GetComponent<Rigidbody>().AddForce(forceVector.normalized * _grappleVelocity,
                    ForceMode.Impulse);
            }
            enabled = true;
        }
    
        private void Awake()
        {
            enabled = false;
            grappledObjectId = new NetworkVariable<ulong>(new NetworkVariableSettings(){
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission =  NetworkVariablePermission.OwnerOnly
            });
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        private void Update()
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _owner.transform.position);
            if (!IsOwner) return;
            var distance = (_owner.transform.position - transform.position).magnitude;
            if (grappledObject == null &&
                _maxLength < distance)
            {
                //превысили возможную длину, но так ничего не нашли.
                DestroyOnServer(NetworkManager.LocalClientId);
            }
        }

        private void OnCollisionEnter(Collision other)
        {

            Destroy(GetComponent<Collider>());
            
            if(!IsOwner) return;
            grappledObject = other.transform.root.gameObject;
            
            if(!grappledObject.TryGetComponent<NetworkObject>(out var net) 
               || !net.IsOwnedByServer)
            {
                DestroyOnServer(NetworkManager.LocalClientId);
                return;
            }
            
            Destroy(GetComponent<Collider>());

            var netObject = grappledObject.GetComponent<NetworkObject>();
            grappledObjectId.Value = netObject.NetworkObjectId;
            
            if (grappledObject.TryGetComponent<Rigidbody>(out _))
            {
                
                RequestOwnershipServerRpc(netObject.NetworkObjectId, NetworkManager.LocalClientId);
                if (grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
                {
                    moveComponent.enabled = false;
                }

                if (grappledObject.TryGetComponent<UnitScript>(out var unitScript))
                {
                    unitScript.isGrappled.Value = true;
                }
            }
            
            //Add joint to _grappledObject
            _joint = gameObject.AddComponent<FixedJoint>();
            _joint.connectedBody = grappledObject.GetComponent<Rigidbody>();
            _joint.connectedAnchor = other.GetContact(0).point;
            _joint.enableCollision = false;
            // _joint.breakForce = 100f;
            _joint.autoConfigureConnectedAnchor = true;
            _joint.axis = Vector3.up;

            //Add joint to _owner
            var joint = _owner.gameObject.AddComponent<FixedJoint>();
            joint.connectedMassScale = 0.5f;
            joint.axis = Vector3.up;
            joint.connectedBody = gameObject.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = true;
        }

        private void OnJointBreak(float breakForce)
        {
            Debug.unityLogger.Log($"Joint breaks with force: {breakForce}");
            DestroyOnServer(grappledObject.GetComponent<NetworkObject>().NetworkObjectId);
        }

        public void DestroyOnServer(ulong clientId, ulong grappledObjectId = default)
        {
            if (IsServer)
            {
                Debug.unityLogger.Log($"Grappler server destroy: {grappledObjectId}");
                grappledObject = GetNetworkObject(grappledObjectId)?.gameObject;

                Debug.unityLogger.Log($"grappled object {grappledObject?.name}");
                if (grappledObject != null &&
                    grappledObject.TryGetComponent<UnitScript>(out var unitScript) &&
                    unitScript.OwnerClientId == clientId)
                {
                    GetNetworkObject(grappledObjectId).RemoveOwnership();
                    unitScript.isGrappled.Value = false;
                }
                if (grappledObject != null && 
                    grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
                {
                    moveComponent.enabled = true;
                }
                if(gameObject.GetComponent<NetworkObject>().IsSpawned)
                    gameObject.GetComponent<NetworkObject>().Despawn(true);
            }
            else
            {
                DestroyServerRpc(clientId, grappledObjectId);
            }
        }
        
        [ServerRpc(RequireOwnership = true)]
        private void DestroyServerRpc(ulong clientId, ulong grappledObjectId = default)
        {
            Debug.unityLogger.Log($"Grappler server destroy: {grappledObjectId}");
            grappledObject = GetNetworkObject(grappledObjectId)?.gameObject;

            if (grappledObject != null &&
                grappledObject.TryGetComponent<UnitScript>(out var unitScript) &&
                unitScript.OwnerClientId == clientId)
            {
                grappledObject.GetComponent<NetworkObject>().RemoveOwnership();
                unitScript.isGrappled.Value = false;
            }
            if (grappledObject != null && 
                grappledObject.TryGetComponent<MoveComponent>(out var moveComponent))
            {
                moveComponent.enabled = true;
            }
            if(gameObject.GetComponent<NetworkObject>().IsSpawned)
                gameObject.GetComponent<NetworkObject>().Despawn(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestOwnershipServerRpc(ulong objectId, ulong newOwnerId)
        {
            GetNetworkObject(objectId).ChangeOwnership(newOwnerId);
        }
    }
}
