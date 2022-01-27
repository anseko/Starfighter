using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPanelOKClick : MonoBehaviour
{
    private OrdersScript _os;

    // Start is called before the first frame update
    void Start()
    {
        _os = FindObjectOfType<OrdersScript>();
    }

    // Update is called once per frame
    public void Create()
    {
        _os.CreateOrder();
    }
}
