using Client.Core;
using Client.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class OrderComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject _orderFramePrefab;
        private PlayerScript _playerScript;
        private GameObject _myOrder;
        public NetworkVariable<OrdersScript.OrderUnit> lastOrder;

        private void Awake()
        {
            lastOrder = new NetworkVariable<OrdersScript.OrderUnit>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Everyone
            });
        }

        public void Init()
        {
            _playerScript = GetComponent<PlayerScript>();
            lastOrder.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(OrdersScript.OrderUnit previousvalue, OrdersScript.OrderUnit newvalue)
        {
            if (IsServer || newvalue.shipName != _playerScript.ShipConfig.shipId) return;
            
            switch (newvalue.operation)
            {
                case OrdersScript.OrderOperation.Add:
                    _myOrder = GameObject.Find("OrderStaticFrame(Clone)") ?? Instantiate(_orderFramePrefab);
                    _myOrder.GetComponent<StaticFrameInit>().FrameInit(GetComponent<PlayerScript>(), newvalue.position, newvalue.size, newvalue.text, true);
                    break;
                case OrdersScript.OrderOperation.Remove:
                    Destroy(_myOrder);
                    break;
                case OrdersScript.OrderOperation.Edit:
                    _myOrder.GetComponent<StaticFrameInit>().FrameInit(GetComponent<PlayerScript>(), newvalue.position, newvalue.size, newvalue.text, true);
                    break;
            }
        }
    }
}