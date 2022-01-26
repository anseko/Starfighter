using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationControler : MonoBehaviour
{
    // Start is called before the first frame update
    private OrdersScript _ordersScript;
    private POIScript _poiScript;
    
    void Start()
    {
        _ordersScript = GetComponent<OrdersScript>();
        _ordersScript.isActive = false;
        _poiScript = GetComponent<POIScript>();
        _poiScript.isActive = false;
    }

    public void SetOrder()
    {
        _ordersScript.isActive = true;
        Cursor.SetCursor(GetComponent<CursorEngine>().cursorExclamation, new Vector2(0,0), CursorMode.Auto);
    }

    public void SetPOI()
    {
        _poiScript.isActive = true;
        Cursor.SetCursor(GetComponent<CursorEngine>().cursorQuestion,new Vector2(0,0), CursorMode.Auto);
    }
}
