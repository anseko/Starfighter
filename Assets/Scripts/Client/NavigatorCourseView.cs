using Client.Core;
using UnityEngine;

namespace Client
{
    public class NavigatorCourseView : MonoBehaviour
    {
        [SerializeField] private PlayerScript _ship;
        [SerializeField] private float _radius = 8f;
        [Range(1, 10)] [SerializeField] private float _speedMarkerRatio = 1f;
        private LineRenderer _renderer;

        public void Init(PlayerScript playerScript)
        {
            _ship = playerScript;
            _renderer = GetComponent<LineRenderer>();
            _renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void Update()
        {
            var shipPosition = _ship.transform.position;
            var size = Mathf.LerpUnclamped(0,
                _ship.unitConfig.maxSpeed / _speedMarkerRatio,
                Mathf.Log(_ship.shipSpeed.Value.magnitude / Mathf.PI + 1)) ;
            var radiusVector = _ship.shipSpeed.Value.normalized * _radius;
            _renderer.SetPosition(0, shipPosition + _ship.shipSpeed.Value.normalized + radiusVector);
            _renderer.SetPosition(1, shipPosition + _ship.shipSpeed.Value.normalized * size + radiusVector);
        }
    }
}
