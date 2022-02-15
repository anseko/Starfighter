using Client.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class Stressbar: MonoBehaviour
    {
        private Slider _slider;
        public PlayerScript playerScript;

        public void Init(PlayerScript ps)
        {
            playerScript = ps;
            _slider.maxValue = playerScript.ShipConfig.maxStress;
            gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            gameObject.SetActive(false);
            _slider = GetComponent<Slider>() ?? gameObject.AddComponent<Slider>();
            
        }

        private void Update()
        {
            _slider.value = playerScript.ShipConfig.currentStress;
        }
    }
}