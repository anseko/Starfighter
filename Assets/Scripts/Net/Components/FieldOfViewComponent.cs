using System;
using System.Linq;
using Client.Core;
using MLAPI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Net.Components
{
    public class FieldOfViewComponent: MonoBehaviour
    {
        [SerializeField] private GameObject _fovCollider;
        private GameObject _fovInstance = null;
        private void Awake()
        {
            enabled = false;
        }

        public void Init(PlayerScript ps)
        {
            _fovInstance = Instantiate(_fovCollider, gameObject.transform);
            _fovInstance.transform.localScale *= ps.FOVRadius;
            
            enabled = true;
            SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .Where(x => x.layer == LayerMask.NameToLayer("Units") || x.layer == LayerMask.NameToLayer("Ships") 
                && !x.Equals(gameObject))
                .ToList().ForEach(x=>x.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = false));
        }

        public bool IsIntersect(Collider other)
        {
            if (other.bounds.Intersects(_fovCollider.GetComponent<SphereCollider>().bounds))
            {
                other.gameObject.GetComponents<Renderer>().ToList().ForEach(x=>x.enabled = false);
                other.gameObject.GetComponentsInChildren<Renderer>().ToList().ForEach(x=>x.enabled = false);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.unityLogger.Log($"Enter:{_fovInstance}");
            if (_fovInstance == null) return;
            if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Units") ||
                other.transform.root.gameObject.layer == LayerMask.NameToLayer("Ships"))
                other.transform.root.gameObject.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = true);
        }

        private void OnTriggerExit(Collider other)
        {
            if(_fovInstance == null) return;
            if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Units") ||
                other.transform.root.gameObject.layer == LayerMask.NameToLayer("Ships"))
                other.transform.root.gameObject.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = false);
        }
    }
}