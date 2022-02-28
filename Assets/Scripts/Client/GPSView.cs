using Client.Core;
using UnityEngine;

namespace Client
{
    public class GPSView : MonoBehaviour
    { 
        public GameObject ship; 
        private GameObject _target;


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
        private void Update()
        {
            var shipPosition = ship.transform.position;
            transform.position = shipPosition + Vector3.up * 70;
            transform.LookAt(_target.transform.position + Vector3.up * 70);
        }
    }
}