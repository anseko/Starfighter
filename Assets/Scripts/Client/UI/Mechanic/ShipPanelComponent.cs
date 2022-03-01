using System.Collections;
using System.Collections.Generic;
using Client.Core;
using Core.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipPanelComponent : MonoBehaviour
{
    [SerializeField] private Slider _speedSlider, _physResSlider, _radResSlider, _radarSlider;

    [SerializeField]
    private TMP_Text _freePointsText;
    private PlayerScript _ship;
    private float _totalPoints;
    private float _speed, _physRes, _radRes, _radar;
    private NetworkSpaceUnitDto _shipData;
    private Dictionary<float,float> _accConvert, _physResConvert, _radResConvert, _radRangeConvert;
    Dictionary<float, float>[] mass;

    void Awake()
    {
        
    }

    void Start()
    {
        _shipData = _ship.NetworkUnitConfig;
        _accConvert = new Dictionary<float, float>();
        _physResConvert = new Dictionary<float, float>();
        _radResConvert = new Dictionary<float, float>();
        _radRangeConvert = new Dictionary<float, float>();
        mass = new[] {_accConvert, _physResConvert, _radRangeConvert };
        foreach (var x in mass)
        {
            x.Add(1,0.75f);
            x.Add(2,1);
            x.Add(3,1.25f);
            x.Add(4,1.5f);
            x.Add(5,2);
        }
        _accConvert = mass[0];
        _physResConvert = mass[1];
        _radRangeConvert = mass[2];
        _radResConvert.Add(1,1);
        _radResConvert.Add(2,2);
        _radResConvert.Add(3,3);
        _radResConvert.Add(4,4);
        _radResConvert.Add(5,5);
    }

    public void AssignPoints()
    {
        if (!(_totalPoints < 0))
        {
            _shipData._accelerationCoefficient.Value = _shipData.Acceleration * _accConvert[_speedSlider.value];
            _shipData._physResistanceCoefficient.Value = _shipData.PhysResistance * _accConvert[_physResSlider.value];
            _shipData._radResistanceCoefficient.Value = _shipData.RadResistance * _accConvert[_radResSlider.value];
            _shipData._radarRangeCoefficient.Value = _shipData.RadarRange * _accConvert[_radarSlider.value];
            gameObject.SetActive(false);
        }
        
    }
    
    void Update()
    {
        _totalPoints = _speedSlider.value + _physResSlider.value + _radResSlider.value + _radarSlider.value;
        _freePointsText.text = $"Свободных очков: {8 - _totalPoints}";
    }
}
