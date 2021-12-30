using TMPro;
using UnityEngine;

namespace Client
{
    public class SwitchFollowMode : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private CameraMotion _cameraMotion;

        private void Start()
        {
            _buttonText = transform.GetComponentInChildren<TextMeshProUGUI>();
            _buttonText.text = _cameraMotion.GetFollowMode() switch
            {
                true => ">cam<",
                false => "<cam>"
            };
        }
        
        public void SwitchButtonText()
        {
            _buttonText.text = _cameraMotion.GetFollowMode() switch
            {
                true => ">cam<",
                false => "<cam>"
            };
        }
    }
}