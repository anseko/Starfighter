using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class SpeedPanelScript : BasePlayerUIElement
    {
        [SerializeField] private Image _image;
    
    
        void Update()
        {
            _image.fillAmount = PlayerScript.shipSpeed.Value.magnitude / 100;
        }
    }
}
