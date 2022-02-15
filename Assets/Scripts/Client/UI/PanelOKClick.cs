using UnityEngine;

namespace Client.UI
{
    public class PanelOKClick : MonoBehaviour
    {
        private OrdersScript _os;
        
        private void Start()
        {
            _os = FindObjectOfType<OrdersScript>();
        }
        
        public void Create()
        {
            _os.CreateOrder();
        }
    }
}
