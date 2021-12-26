using Client.Core;
using ScriptableObjects;
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
            _slider.maxValue = (playerScript.unitConfig as SpaceShipConfig).maxStress;
            gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            gameObject.SetActive(false);
            _slider = GetComponent<Slider>() ?? gameObject.AddComponent<Slider>();
            
        }

        private void Update()
        {
            _slider.value = playerScript.currentStress.Value;
        }
    }
}