using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class SpeedPanelScript : BasePlayerUIElement
    {
        [SerializeField] private Image _image;
        [SerializeField]
        [Range(0, 200)]
        private float _changeRate = 100;

        private void Update()
        {
            _image.fillAmount = PlayerScript.shipSpeed.Value.magnitude / _changeRate;
        }
    }
}
