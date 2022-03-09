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
    private TMP_Text _freePointsText, _noPointsText;
    public PlayerScript ship;
    private float _totalPoints;
    private float _speed, _physRes, _radRes, _radar;
    private NetworkSpaceUnitDto _shipData;
    private Dictionary<float,float> _accConvert, _physResConvert, _radResConvert, _radRangeConvert;
    [SerializeField] private Button _okButton;

    private void Awake()
    {
        _totalPoints = _speedSlider.value + _physResSlider.value + _radResSlider.value + _radarSlider.value;
    }

    private void Start()
    {
        _accConvert = new Dictionary<float, float>();
        _physResConvert = new Dictionary<float, float>();
        _radResConvert = new Dictionary<float, float>();
        _radRangeConvert = new Dictionary<float, float>();
        
        _accConvert.Add(1,0.75f);
        _accConvert.Add(2,1);
        _accConvert.Add(3,1.25f);
        _accConvert.Add(4,1.5f);
        _accConvert.Add(5,2f);
        
        _physResConvert.Add(1,1.25f);
        _physResConvert.Add(2,1f);
        _physResConvert.Add(3,0.75f);
        _physResConvert.Add(4,0.5f);
        _physResConvert.Add(5,0.25f);
        
        _radResConvert.Add(1,1.25f);
        _radResConvert.Add(2,1f);
        _radResConvert.Add(3,0.75f);
        _radResConvert.Add(4,0.5f);
        _radResConvert.Add(5,0.25f);
        
        _radRangeConvert.Add(1,0.75f);
        _radRangeConvert.Add(2,1f);
        _radRangeConvert.Add(3,1.25f);
        _radRangeConvert.Add(4,1.5f);
        _radRangeConvert.Add(5,2f);
    }
    
    public void AssignPoints()
    {
        _shipData.AccelerationCoefficient = _accConvert[_speedSlider.value];
        _shipData.PhysResistanceCoefficient = _physResConvert[_physResSlider.value];
        _shipData.RadResistanceCoefficient = _radResConvert[_radResSlider.value];
        _shipData.RadarRangeCoefficient = _radRangeConvert[_radarSlider.value];
        gameObject.SetActive(false);
    }

    public void Init()
    {
        _shipData = ship.NetworkUnitConfig;
        
        _speedSlider.value = ship.NetworkUnitConfig.AccelerationCoefficient switch
        {
            0.75f => 1,
            1f => 2,
            1.25f => 3,
            1.5f => 4,
            2f => 5,
            _ => _speedSlider.value
        };

        _physResSlider.value = ship.NetworkUnitConfig.PhysResistanceCoefficient switch
        {
            1.25f => 1,
            1f => 2,
            0.75f => 3,
            0.5f => 4,
            0.25f => 5,
            _ => _physResSlider.value
        };
        _radResSlider.value = ship.NetworkUnitConfig.RadResistanceCoefficient switch
        {
            1.25f => 1,
            1f => 2,
            0.75f => 3,
            0.5f => 4,
            0.25f => 5,
            _ => _radResSlider.value
        };
        _radarSlider.value = ship.NetworkUnitConfig.RadarRangeCoefficient switch
        {
            0.75f => 1,
            1f => 2,
            1.25f => 3,
            1.5f => 4,
            2f => 5,
            _ => _radarSlider.value
        };
    }
    
    private void Update()
    {
        _totalPoints = _speedSlider.value + _physResSlider.value + _radResSlider.value + _radarSlider.value;
        _freePointsText.text = $"Свободных очков: {8 - _totalPoints}";
        if (_totalPoints > 8)
        {
            _okButton.interactable = false;
            _noPointsText.gameObject.SetActive(true);
        }
        else if (_totalPoints <= 8)
        {
            _okButton.interactable = true;
            _noPointsText.gameObject.SetActive(false);
        }
    }
}
