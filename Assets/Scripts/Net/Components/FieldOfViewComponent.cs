using System;
using Client.Core;
using UnityEngine;

namespace Net.Components
{
    public class FieldOfViewComponent: MonoBehaviour
    {
        private PlayerScript _unit;
        [SerializeField] private SphereCollider _fovCollider; 

        private void Awake()
        {
            _unit = GetComponent<PlayerScript>();
            _fovCollider = GetComponent<SphereCollider>() ?? gameObject.AddComponent<SphereCollider>();
            // _fovCollider.center = Vector3.zero;
            _fovCollider.radius = _unit.FOVRadius;
            _fovCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}