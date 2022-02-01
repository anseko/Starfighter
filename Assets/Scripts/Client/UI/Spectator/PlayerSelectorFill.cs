using Client.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectorFill : MonoBehaviour
{

    [SerializeField] private GameObject _button;
    [SerializeField] private VerticalLayoutGroup _panel;

    private void Start()
    {
        var players = FindObjectsOfType<PlayerScript>();
        foreach (var ship in players)
        {
            var button = Instantiate(_button, _panel.transform);
            string buttonName = button.GetComponentInChildren<TMP_Text>().text;
            Debug.Log(ship.name);
            buttonName = ship.name.Substring(0, ship.name.Length-7);
        }
    }
}
