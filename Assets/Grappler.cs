using System.Linq;
using Client;
using UnityEngine;


public class Grappler : MonoBehaviour
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
        _owner = playerScript.gameObject;
        _maxLength = maxLength;
        // _grappleVelocity = 1;
        _forceVector = _owner.transform.Find("Front").position - _owner.transform.position;
        enabled = true;
    }
    
    private void Awake()
    {
        enabled = false;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        Debug.unityLogger.Log(_forceVector);
        GetComponent<Rigidbody>().AddForce(_forceVector.normalized * _grappleVelocity, ForceMode.Impulse);
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _owner.transform.position);
        if (_grappledObject is null &&
            _maxLength < Vector3.Distance(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1)))
        {
            //превысили возможную длину, но так ничего не нашли.
            Destroy(this);
        }
    }
    
    private void LateUpdate()
    {
        if (!_joint) return;
        Debug.unityLogger.Log($"{_joint.currentForce.magnitude}:{_joint.currentTorque.magnitude}");
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponents<Joint>().Any(x => x.connectedBody == gameObject.GetComponent<Rigidbody>())) return;
        //приклеиться к объекту, с которым столкнулся
        _grappledObject = other.gameObject;

        _joint = _grappledObject.AddComponent<FixedJoint>();
        _joint.connectedBody = gameObject.GetComponent<Rigidbody>();
        _joint.connectedAnchor = other.GetContact(0).point;
        _joint.enableCollision = false;
        _joint.breakForce = 100f;
        _joint.autoConfigureConnectedAnchor = true;
        _joint.axis = Vector3.up;
        
        Debug.unityLogger.Log(_joint);
        
        //установить соединение с кораблем
        var hingeJoint = _owner.GetComponent<HingeJoint>();
        hingeJoint = hingeJoint ? hingeJoint : _owner.AddComponent<HingeJoint>();
        hingeJoint.connectedMassScale = 1;
        hingeJoint.useSpring = false;
        hingeJoint.axis = Vector3.up;
        hingeJoint.connectedBody = GetComponent<Rigidbody>();
        hingeJoint.autoConfigureConnectedAnchor = true;
        //TODO:...

    }

    private void OnDestroy()
    {
        Destroy(_joint);
    }
}
