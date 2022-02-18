using UnityEngine;

namespace Client.UI
{
    public class Editor : MonoBehaviour
    {
        private OrdersScript _ordersScript;

        private void Start()
        {
            _ordersScript = FindObjectOfType<OrdersScript>(true);
            _ordersScript.isActive = false;
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
