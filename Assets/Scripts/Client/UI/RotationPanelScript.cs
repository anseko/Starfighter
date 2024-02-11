using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class RotationPanelScript : BasePlayerUIElement
    {
        [SerializeField] private Image _image;


        private void Update()
        {
            _image.fillAmount = PlayerScript.shipRotation.magnitude*Mathf.Rad2Deg / 90;
        }
    }
}
