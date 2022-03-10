using Client.Core;
using UnityEngine;

namespace Client.UI.Mechanic
{
    public class MechanicButtonScript : MonoBehaviour
    {
        public MechanicPanelComponent panel;
        public PlayerScript ship;

        public void SetParameters()
        {
            panel.gameObject.SetActive(true);
            panel.ship = ship;
            panel.Init();
        }
    }
}
