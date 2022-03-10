using Client.Core;
using Core;
using Net.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.UI.Mechanic
{
    public class MechanicPlayerSelectorFill : MonoBehaviour
    {
        [SerializeField] private GameObject _button;
        [SerializeField] private VerticalLayoutGroup _panel;
        [SerializeField] private MechanicPanelComponent mechanicPanelComponent;

        public void Init()
        {
            foreach (var go in _panel.GetComponentsInChildren<MechanicButtonScript>())
            {
                Destroy(go.gameObject);
            }

            foreach (var ship in FindObjectsOfType<PlayerScript>())
            {
                if (ship.GetState() == UnitState.IsDocked && 
                    ship.TryGetComponent<DockComponent>(out var dockComponent) &&
                    dockComponent.lastThingToDock != null &&
                    dockComponent.lastThingToDock.name.Contains("Station"))
                {
                    var button = Instantiate(_button, _panel.transform);
                    var buttonName = button.GetComponentInChildren<TextMeshProUGUI>();
                    buttonName.text = ship.NetworkUnitConfig.ShipId;
                    button.GetComponent<MechanicButtonScript>().ship = ship.GetComponent<PlayerScript>();
                    button.GetComponent<MechanicButtonScript>().panel = mechanicPanelComponent;
                }
            }
        }
    }
}
