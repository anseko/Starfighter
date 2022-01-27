using UnityEngine;

public class Editor : MonoBehaviour
{
    [SerializeField] private OrdersScript _ordersScript;
    void Start()
    {
        _ordersScript = FindObjectOfType<OrdersScript>();
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
