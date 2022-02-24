using System.Collections;
using System.Collections.Generic;
using Client.Core;
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
    
    void Awake()
    {
        
    }

    void Update()
    {
        _totalPoints = _speedSlider.value + _physResSlider.value + _radResSlider.value + _radarSlider.value;
        _freePointsText.text = $"Свободных очков {8 - _totalPoints}";
    }
}
