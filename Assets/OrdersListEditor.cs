using MLAPI;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;

public class OrdersListEditor : NetworkBehaviour
{
    public NetworkDictionary<string, OrderUnit> ordersList;
    
    public class OrderUnit
    {
        public string shipName;
        public Vector3 position;
        public Vector3 size;
        public string text;
        
        public void SetOrder(string _shipName, Vector3 _position, Vector3 _size, string _text)
        {
            shipName = _shipName;
            position = _position;
            size = _size;
            text = _text;
        }
    }

    void Start()
        {
            var ordersList = new NetworkDictionary<string, OrderUnit>();
        }
    
    public bool AddOrderInList(OrderUnit _order)
    {
        var unit = new OrderUnit();
        unit.SetOrder(_order.shipName, _order.position, _order.size, _order.text);
        var result = true;
        var _isSucces = true;
        foreach (var x in ordersList)
        {
            if (x.Key == unit.shipName)
            {
                Debug.Log("Got an order for this ship already");
                _isSucces = false;
                result = false;
                break;
            }
        }
        
        if (_isSucces)
        {
            ordersList.Add(unit.shipName,unit);
        }
        return result;
    }

    public bool RemoveOrderFromList(OrderUnit _order)
    {
        var unit = new OrderUnit();
        unit.SetOrder(_order.shipName, _order.position, _order.size, _order.text);
        var result = true;
        var _isSucces = false;
        foreach (var x in ordersList)
        {
            if (x.Value.shipName == unit.shipName)
            {
                ordersList.Remove(x.Key);
                _isSucces = true;
                result = true;
                break;
            }

            if (!_isSucces)
            {
                Debug.Log("No order item to remove");
                result = false;
            }
        }
        return result;
    }
}
