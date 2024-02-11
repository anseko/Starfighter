using Mirror;
using UnityEngine;

namespace Client.UI
{
    public class Editor : NetworkBehaviour
    {
        private OrdersScript _ordersScript;

        private void Awake()
        {
            //BUG possible
            if (isServer)
            {
                enabled = false;
            }
        }

        private void Start()
        {
            _ordersScript = FindObjectOfType<OrdersScript>(true);
            _ordersScript.state = OrdersScript.EditorState.IsInactive;
        }
        
        public void Edit()
        {
            _ordersScript.EditOrder(GetComponent<OrderFrameInit>());
        }

        public void Destroy()
        {
            _ordersScript.CancelOrder();
        }
    }
}
