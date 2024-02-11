using System.Linq;
using Client.Core;
using TMPro;
using UnityEngine;

namespace Client.UI.Spectator
{
    public class CenterView : MonoBehaviour
    {
        private GameObject _ship;
        [SerializeField] private Camera _camera;
    
        private void Start()
        {
            _camera = transform.root.gameObject.GetComponentInChildren<Camera>();
            _ship = FindObjectsOfType<PlayerScript>().FirstOrDefault(x =>
                x.networkUnitConfig.shipId == GetComponentInChildren<TextMeshProUGUI>().text)?.gameObject;
        }
    
        public void Spectate()
        {
            if (_ship.GetComponentInChildren<MeshRenderer>().enabled)
            {
                var position = _ship.transform.position;
                _camera.transform.position = new Vector3(position.x, 100, position.z);
            }
        }

        public bool IsInRadarRange()
        {
            var check1 = _ship.gameObject.GetComponentsInChildren<Renderer>().All(x => x.enabled); 
            var check2 = _ship.gameObject.GetComponents<Renderer>().All(x => x.enabled);
            return check1 && check2;
        }
    }
}
