using Net.Components;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private OrdersScript _ordersScript;
    void Start()
    {
        _ordersScript = FindObjectOfType<OrdersScript>(true);
        _ordersScript.isActive = false;
    }
    public void Edit()
    {
        _ordersScript.EditOrder(GetComponent<StaticFrameInit>());
    }
    
    public void Destroy()
    {
        var unit = new OrdersListEditor.OrderUnit();
        var panel = gameObject.GetComponent<OrderPanelComponent>();
        unit.shipName = panel.name;
        unit.position = panel.position;
        unit.size = panel.size;
        unit.text = panel.text;
        GetComponent<OrdersListEditor>().RemoveOrderFromList(unit);
        Destroy(gameObject);
    }
}
