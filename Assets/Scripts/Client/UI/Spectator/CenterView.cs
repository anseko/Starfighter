using System.Collections;
using System.Collections.Generic;
using Client.Core;
using TMPro;
using UnityEngine;

public class CenterView : MonoBehaviour

{
    
    private GameObject _ship;
    [SerializeField] private Camera _camera;
    
    void Start()
    {
        _ship = GameObject.Find(GetComponent<TextMeshPro>().text);
    }


    public void Spectate()
    {
        _camera.transform.position = _ship.transform.position;
    }
}