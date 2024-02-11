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
            ClientEventStorage.GetInstance().OnShipSpeedChange.AddListener(CourseChange);
        }

        private void CourseChange(Vector3 newValue)
        {
            var shipPosition = ship.transform.position;
            transform.position = shipPosition + Vector3.up * 70;
            transform.LookAt(shipPosition + newValue.normalized + Vector3.up * 70);
        }
    }
}
