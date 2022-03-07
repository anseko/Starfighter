using Client.Core;
using Core;
using Net.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Mechanic
{
    public class MechanicPlayerSelectorFill : MonoBehaviour
    {

        [SerializeField] private GameObject _button;
        [SerializeField] private VerticalLayoutGroup _panel;

        private void Start()
        {
            foreach (var ship in FindObjectsOfType<PlayerScript>())
            {
                if (ship.GetState() == UnitState.IsDocked && 
                    ship.GetComponent<DockComponent>().lastThingToDock.name.Contains("Station"))
                {
                    var button = Instantiate(_button, _panel.transform);
                    var buttonName = button.GetComponentInChildren<TextMeshProUGUI>();
                    buttonName.text = ship.NetworkUnitConfig.ShipId;
                    button.GetComponent<MechanicButtonScript>().ship = ship.GetComponent<PlayerScript>();
                }
            }
        }
    }
}
