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
        Destroy(gameObject);
    }
}
