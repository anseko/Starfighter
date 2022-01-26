using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPanelOKClick : MonoBehaviour
{
    private OrdersScript _os;
    private POIScript _ps;
    
    // Start is called before the first frame update
    void Start()
    {
        _os = FindObjectOfType<OrdersScript>();
        _ps = FindObjectOfType<POIScript>();
    }

    // Update is called once per frame
    public void Create()
    {
        if (_os.isOrder)
        {
            _os.CreateOrder();
        }
        if (_ps.isPOI)
        {
            _ps.CreatePOI();
        }
    }
}
