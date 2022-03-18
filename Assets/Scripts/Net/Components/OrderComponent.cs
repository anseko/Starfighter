using Client.Core;
using Client.UI;
using Core.Models;
using Unity.Netcode;
using UnityEngine;

namespace Net.Components
{
    public class OrderComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject _orderFramePrefab;
        private PlayerScript _playerScript;
        private GameObject _myOrder;
        public NetworkVariable<OrderUnit> lastOrder;

        private void Awake()
        {
            lastOrder = new NetworkVariable<OrderUnit>();
        }

        public void Init()
        {
            _playerScript = GetComponent<PlayerScript>();
            lastOrder.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(OrderUnit previousvalue, OrderUnit newvalue)
        {
            if (IsServer || newvalue.shipName != _playerScript.NetworkUnitConfig.ShipId) return;
            
            switch (newvalue.operation)
            {
                case OrderOperation.Add:
                    _myOrder = GameObject.Find("OrderStaticFrame(Clone)") ?? Instantiate(_orderFramePrefab);
                    _myOrder.GetComponent<OrderFrameInit>().FrameInit(GetComponent<PlayerScript>(), newvalue.position, newvalue.size, newvalue.text.ToString(), true);
                    break;
                case OrderOperation.Remove:
                    Destroy(_myOrder);
                    break;
                case OrderOperation.Edit:
                    _myOrder.GetComponent<OrderFrameInit>().FrameInit(GetComponent<PlayerScript>(), newvalue.position, newvalue.size, newvalue.text.ToString(), true);
                    break;
            }
        }
    }
}