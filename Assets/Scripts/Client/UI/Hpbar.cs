using Client.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class Hpbar: MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        public PlayerScript playerScript;

        public void Init(PlayerScript ps)
        {
            playerScript = ps;
            _slider.maxValue = playerScript.ShipConfig.maxHp;
            gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _slider.value = playerScript.ShipConfig.currentHp;
        }
    }
}