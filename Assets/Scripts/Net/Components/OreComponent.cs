using MLAPI;
using UnityEngine;

namespace Net.Components
{
    public class OreComponent: NetworkBehaviour
    {
        private Rigidbody _rigidbody;
        private Collider _collider;
        [SerializeField] private GameObject _asteroid;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (!IsServer) return;
            if (other.gameObject != _asteroid) return;
            transform.SetParent(null, true);
            gameObject.layer = LayerMask.NameToLayer("Units");
        }
    }
}