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
        
        public void Init(PlayerScript playerScript)
        {
            ship = playerScript;
            ship.shipSpeed.OnValueChanged += CourseChange;
        }

        private void CourseChange(Vector3 previousvalue, Vector3 newvalue)
        {
            var shipPosition = ship.transform.position;
            Handles.color = lineColor;
            Handles.DrawLine(shipPosition, ship.shipSpeed.Value*speedOffset);
        }
    }
}
