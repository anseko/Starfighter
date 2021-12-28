using System.Collections;
using System.Collections.Generic;
using Client.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerSelectorFill : MonoBehaviour
{

    [SerializeField] private GameObject _button;
    [SerializeField] private GameObject _panel;
    
    void Start()
    {
        var players = FindObjectsOfType<PlayerScript>();
        foreach (var ship in players)
        {
            var button = Instantiate(_button, _panel.transform);
            button.GetComponent<TextMeshPro>().text = ship.name;
        }
    }
}
