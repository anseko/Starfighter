using Client.Core;
using UnityEngine;

namespace Client
{
    public class CourseView : MonoBehaviour
    {
        public PlayerScript ship;
        
        
        public void Init(PlayerScript playerScript)
        {
            ship = playerScript;
            ship.shipSpeed.OnValueChanged += CourseChange;
        }

        private void CourseChange(Vector3 previousvalue, Vector3 newvalue)
        {
            var shipPosition = ship.transform.position;
            transform.position = shipPosition + Vector3.up * 70;
            transform.LookAt(shipPosition + newvalue.normalized + Vector3.up * 70);
        }
    }
}
