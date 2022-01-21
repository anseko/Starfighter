using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationControler : MonoBehaviour
{
    // Start is called before the first frame update
    public OrdersScript ordersScript;
    
    void Start()
    {
        ordersScript = GetComponent<OrdersScript>();
        ordersScript.isActive = false;
    }

    public void SetOrder()
    {
        ordersScript.isActive = true;
    }
}
