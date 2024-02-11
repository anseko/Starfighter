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
        [SyncVar]
        public OrdersScript.OrderUnit lastOrder;

        private void Awake()
        {
            //only can be writing on server
            // lastOrder = new NetworkVariable<OrdersScript.OrderUnit>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Everyone
            // });
        }

        public void Init()
        {
            _playerScript = GetComponent<PlayerScript>();
            lastOrder.OnValueChanged += OnValueChanged;
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