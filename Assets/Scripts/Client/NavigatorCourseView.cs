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
        public Vector3[] points;
        
        public void Init(PlayerScript playerScript)
        {
            ship = playerScript;
            points = new Vector3[2];
            renderer = GetComponent<LineRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void FixedUpdate()
        {
            var shipPosition = ship.transform.position;
            points[0] = shipPosition;
            points[1] = shipPosition + (ship.shipSpeed.Value)*speedOffset;
            renderer.startColor = lineColor;
            renderer.SetPositions(points);
        }
    }
}
