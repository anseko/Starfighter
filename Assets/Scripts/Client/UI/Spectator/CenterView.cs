using System.Linq;
using Client.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                x.NetworkUnitConfig.ShipId == GetComponentInChildren<TextMeshProUGUI>().text)?.gameObject;
        }
    
        public void Spectate()
        {
            if (_ship.GetComponentInChildren<MeshRenderer>().enabled)
            {
                var position = _ship.transform.position;
                _camera.transform.position = new Vector3(position.x, 100, position.z);
            }
        }

        private void Update()
        {
            if (!_ship.gameObject.GetComponentInChildren<MeshRenderer>().enabled)
            {
                this.gameObject.GetComponent<Button>().interactable = false;
            }
            else this.gameObject.GetComponent<Button>().interactable = true;
        }
    }
}
