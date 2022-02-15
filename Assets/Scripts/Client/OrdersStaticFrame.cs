using Client.Core;
using UnityEngine;

namespace Client
{
    public class OrdersStaticFrame : MonoBehaviour
    {
        public PlayerScript ship;
        public string text;
        public Vector3 position;
        public Vector3 size;

        private void Start()
        {
            transform.position = position;
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}
