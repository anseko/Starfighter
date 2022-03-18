using System.Collections.Generic;
using Client.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Spectator
{
    public class PlayerSelectorFill : MonoBehaviour
    {

        [SerializeField] private GameObject _button;
        [SerializeField] private VerticalLayoutGroup _panel;
        private List<CenterView> buttons;
        public bool isSpectator = false;

        private void Start()
        {
            buttons = new List<CenterView>();
            foreach (var ship in FindObjectsOfType<PlayerScript>())
            {
                 var button = Instantiate(_button, _panel.transform);
                 var buttonName = button.GetComponentInChildren<TextMeshProUGUI>();
                 buttonName.text = ship.NetworkUnitConfig.ShipId;
                 buttons.Add(button.GetComponent<CenterView>());
            }
        }

        private void Update()
        {
            if (isSpectator) return;
            buttons.ForEach(x=>x.gameObject.SetActive(x.IsInRadarRange()));
        }
    }
}
