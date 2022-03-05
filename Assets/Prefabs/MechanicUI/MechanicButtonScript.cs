using System.Collections;
using System.Collections.Generic;
using Client.Core;
using UnityEngine;
using UnityEngine.UI;

public class MechanicButtonScript : MonoBehaviour
{
    [SerializeField] private ShipPanelComponent _panel;
    [SerializeField] public PlayerScript ship;
    private float[] _sliders;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void SetParameters()
    {
        _panel.gameObject.SetActive(true);
        _panel.ship = ship;
        _panel.Init();
    }
}
