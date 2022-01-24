using System.Collections;
using System.Collections.Generic;
using Client.Core;
using UnityEngine;

public class OrdersStaticFrame : MonoBehaviour
{
    public PlayerScript ship;
    public string text;
    public Vector3 position;
    public Vector3 size;
    
    void Start()
    {
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
