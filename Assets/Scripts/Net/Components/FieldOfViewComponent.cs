using System.Linq;
using Client.Core;
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
            _fovInstance.transform.localScale *=
                ps.NetworkUnitConfig.RadarRange * ps.NetworkUnitConfig.RadarRangeCoefficient;
            
            enabled = true;
            SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .Where(x => x.layer == LayerMask.NameToLayer("Units") || x.layer == LayerMask.NameToLayer("Ships"))
                .ToList().ForEach(x=>x.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = false));
            gameObject.GetComponentsInChildren<Renderer>().ToList().ForEach(renderer => renderer.enabled = true);

            ps.NetworkUnitConfig._radarRange.OnValueChanged += (value, newValue) =>
            {
                _fovInstance.transform.localScale *= newValue * ps.NetworkUnitConfig.RadarRangeCoefficient;
            };
            
            ps.NetworkUnitConfig._radarRangeCoefficient.OnValueChanged += (value, newValue) =>
            {
                _fovInstance.transform.localScale *= newValue * ps.NetworkUnitConfig.RadarRange;
            };
        }

        private void OnTriggerEnter(Collider other)
        {
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