using Client.Core;
using Core;
using Net.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Spectator
{
    public class PlayerSelectorFill : MonoBehaviour
    {

        [SerializeField] private GameObject _button;
        [SerializeField] private VerticalLayoutGroup _panel;

        private void Start()
        {
            foreach (var ship in FindObjectsOfType<PlayerScript>())
            {
                 var button = Instantiate(_button, _panel.transform);
                 var buttonName = button.GetComponentInChildren<TextMeshProUGUI>();
                 buttonName.text = ship.NetworkUnitConfig.ShipId;
            }
        }
    }
}
