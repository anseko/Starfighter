using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class RotationPanelScript : BasePlayerUIElement
    {
        [SerializeField] private Image _image;


        void Update()
        {
            _image.fillAmount = PlayerScript.shipRotation.Value.magnitude*Mathf.Rad2Deg / 90;
        }
    }
}
