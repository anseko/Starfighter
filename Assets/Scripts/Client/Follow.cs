using UnityEngine;

namespace Client
{
    public class Follow : MonoBehaviour
    {
        public GameObject player;
        private Vector3 _offset;

        private void Start()
        {
            _offset = Vector3.zero + Vector3.up*40;
        }
        
        private void LateUpdate()
        {
            transform.position = player.transform.position + _offset;
        }
    }
}
