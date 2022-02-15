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
                x.ShipConfig.shipId == GetComponentInChildren<TextMeshProUGUI>().text)?.gameObject;
        }
    
        public void Spectate()
        {
            var position = _ship.transform.position;
            _camera.transform.position = new Vector3(position.x, 100, position.z);
        }
    }
}
