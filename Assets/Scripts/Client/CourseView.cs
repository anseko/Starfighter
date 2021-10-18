using UnityEngine;

namespace Client
{
    public class CourseView : MonoBehaviour
    {
        public GameObject ship;
        private GameObject _target;
        private Vector3 _lastPosition;

        
        public void Init(PlayerScript playerScript, GameObject target = null)
        {
            ship = playerScript.gameObject;
            if (target is null)
                _target = ship;
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }
        
        // Update is called once per frame
        void Update()
        {
            var shipPosition = ship.transform.position;
            var delta = ship.GetComponent<Rigidbody>().velocity.normalized;
            transform.LookAt(shipPosition + delta + Vector3.up * 70);
            var cursorPosition = shipPosition + Vector3.up * 70;
            transform.position = cursorPosition;
            _lastPosition = _target.transform.position;
        }
    }
}
