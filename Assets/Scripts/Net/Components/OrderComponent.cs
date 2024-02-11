using Client.Core;
using Client.UI;
using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class OrderComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject _orderFramePrefab;
        private PlayerScript _playerScript;
        private GameObject _myOrder;
        
        [SyncVar(hook = nameof(OnValueChanged))]
        public OrdersScript.OrderUnit lastOrder;

        public void Init()
        {
            _playerScript = GetComponent<PlayerScript>();
        }

        private void OnValueChanged(OrdersScript.OrderUnit previousValue, OrdersScript.OrderUnit newValue)
        {
            if (isServer || newValue.shipName != _playerScript.networkUnitConfig.shipId) return;
            switch (newValue.operation)
            {
                case OrdersScript.OrderOperation.Add:
                    _myOrder = GameObject.Find("OrderStaticFrame(Clone)") ?? Instantiate(_orderFramePrefab);
                    _myOrder.GetComponent<OrderFrameInit>().FrameInit(GetComponent<PlayerScript>(), newValue.position, newValue.size, newValue.text, true);
                    break;
                case OrdersScript.OrderOperation.Remove:
                    Destroy(_myOrder);
                    break;
                case OrdersScript.OrderOperation.Edit:
                    _myOrder.GetComponent<OrderFrameInit>().FrameInit(GetComponent<PlayerScript>(), newValue.position, newValue.size, newValue.text, true);
                    break;
            }
        }
    }
}