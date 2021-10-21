using Client.Core;
using UnityEngine;

namespace Client
{
    public class GPSView : MonoBehaviour
    { 
        public GameObject ship; 
        private GameObject _target;
        private Vector3 _lastPosition;

        
        public void Init(PlayerScript playerScript, GameObject target = null)
        {
            Debug.unityLogger.Log($"CourseView: {gameObject.name} , {playerScript.gameObject.name}");
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
            transform.position = ship.transform.position;
            transform.LookAt(_target.transform);
        }
    }
}