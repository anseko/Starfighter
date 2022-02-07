using System.Collections;
using System.Collections.Generic;
using Client.Core;
using Core;
using Net.Components;
using UnityEngine;
using UnityEngine.Animations;

public class StaticFrameInit : MonoBehaviour
{
    public PlayerScript ship;
    public Vector3 position;
    public Vector3 size;
    public string text;
    public float scaleX;
    public float scaleZ;

    void Awake()
    {
        GetComponent<OrderPanelComponent>().Init(ship,position,size,text);
    }
    
    public void FrameInit(PlayerScript shipInit, Vector3 positionInit, Vector3 sizeInit, string textInit)
    {
        ship = shipInit;
        position = positionInit;
        size = sizeInit;
        text = textInit;
        scaleX =  size.x - position.x;
        scaleZ =  size.z - position.z;
        transform.position = new Vector3(positionInit.x+scaleX/2, -180, positionInit.z+scaleZ/2);
        transform.localScale = new Vector3( Mathf.Abs(scaleX), Mathf.Abs(scaleZ), 1);
        transform.eulerAngles = new Vector3(90,0,0);
    }
    
    public void ClientFrameInit(Vector3 positionInit, Vector3 sizeInit, string textInit)
    {
        position = positionInit;
        size = sizeInit;
        text = textInit;
        scaleX =  size.x - position.x;
        scaleZ =  size.z - position.z;
        transform.position = new Vector3(positionInit.x+scaleX/2, -180, positionInit.z+scaleZ/2);
        transform.localScale = new Vector3( Mathf.Abs(scaleX), Mathf.Abs(scaleZ), 1);
        transform.eulerAngles = new Vector3(90,0,0);
    }
}
