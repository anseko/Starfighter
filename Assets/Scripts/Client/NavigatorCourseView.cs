using Client.Core;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class NavigatorCourseView : MonoBehaviour
    {
        public PlayerScript ship;
        public float speedOffset = 5f;
        public Color lineColor = new Color(89,250,19,255);
        public LineRenderer renderer;
        [SerializeField] private Vector3[] _points;
        [SerializeField] private Vector3 shipPosition;

        public void Start()
        {
            _points = new Vector3[2];
        }
        public void Init(PlayerScript playerScript)
        {
            ship = playerScript;
            renderer = GetComponent<LineRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void FixedUpdate()
        {
            shipPosition = ship.transform.position;
            _points[0] = shipPosition;
            _points[1] = shipPosition + (ship.shipSpeed.Value)*speedOffset;
            renderer.startColor = lineColor;
            renderer.SetPositions(_points);
        }
    }
}
