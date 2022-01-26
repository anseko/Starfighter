using UnityEngine;

public class Editor : MonoBehaviour
{
    private OrdersScript _ordersScript;
    private POIScript _poiScript;
    
    void Start()
    {
        _poiScript = FindObjectOfType<POIScript>();
        _ordersScript = FindObjectOfType<OrdersScript>();
    }
    public void Edit()
    {
        if (gameObject.name == "POIStaticFrame")
        {
            _poiScript.EditPOI(GetComponent<StaticFrameInit>());
        }
        else if (gameObject.name == "OrderStaticFrame")
        {
            _ordersScript.EditOrder(GetComponent<StaticFrameInit>());
        }
    }
}
