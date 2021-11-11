using System.Linq;
using Client;
using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using Net.Components;
using UnityEngine;


//TODO: Сделать, чтобы можно было вертеться будучи привязанным
public class Grappler : NetworkBehaviour
{
    [SerializeField] private GameObject _owner;
    [SerializeField] private GameObject _grappledObject;
    [SerializeField] private Vector3 _forceVector;
    [SerializeField] private FixedJoint _joint;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _maxLength;
    [SerializeField] private float _grappleVelocity;

    
    public void Init(PlayerScript playerScript, float maxLength)
    {
        // SpawnGrapplerServerRpc(NetworkManager.Singleton.LocalClientId);
        _owner = playerScript.gameObject;
        _maxLength = maxLength;
        _forceVector = _owner.transform.Find("Front").position - _owner.transform.position;
        GetComponent<Rigidbody>().AddForce(_forceVector.normalized * _grappleVelocity, ForceMode.Impulse);
    }
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    [ServerRpc]
    private void SpawnGrapplerServerRpc(ulong clientId)
    {
        GetComponent<NetworkObject>().SpawnWithOwnership(clientId, destroyWithScene: true);
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _owner.transform.position);
        if (_grappledObject == null &&
            _maxLength < (_lineRenderer.GetPosition(0) - _lineRenderer.GetPosition(1)).magnitude)
        {
            //превысили возможную длину, но так ничего не нашли.
            Destroy(gameObject);
        }
    }
    
    private void LateUpdate()
    {
        if (!_joint) return;
        Debug.unityLogger.Log($"{_joint.currentForce.magnitude}:{_joint.currentTorque.magnitude}");
    }

    //Это будет выполнено на всех клиентах и сервере или нет?
    private void OnCollisionEnter(Collision other)
    {
        Debug.unityLogger.Log(other.gameObject.name);
        if(GetComponents<Joint>().Any(x => x.connectedBody == other.gameObject.GetComponent<Rigidbody>())) return;
        //приклеиться к объекту, с которым столкнулся
        _grappledObject = other.transform.root.gameObject;

        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = _grappledObject.GetComponentInChildren<Rigidbody>();
        _joint.connectedAnchor = other.GetContact(0).point;
        _joint.enableCollision = false;
        _joint.breakForce = 100f;
        _joint.autoConfigureConnectedAnchor = true;
        _joint.axis = Vector3.up;
        
        Debug.unityLogger.Log(_joint);
        
        //установить соединение с кораблем
        var springJoint = GetComponent<SpringJoint>();
        springJoint = springJoint ? springJoint : gameObject.AddComponent<SpringJoint>();
        springJoint.connectedMassScale = 1;
        springJoint.maxDistance = _maxLength / 2;
        springJoint.axis = Vector3.up;
        springJoint.connectedBody = _owner.GetComponent<Rigidbody>();
        springJoint.autoConfigureConnectedAnchor = true;

        // var networkObjectId = _grappledObject.GetComponent<NetworkObject>()?.NetworkObjectId;
        // if(networkObjectId != null && _joint.connectedBody != null)
        //     _grappledObject.GetComponent<GrappleComponent>()?.ApplyGrappleServerRpc(networkObjectId.Value);
    }

    private void OnJointBreak(float breakForce)
    {
        Debug.unityLogger.Log($"Joint breaks with force: {breakForce}");
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        Destroy(_joint);
    }
}
