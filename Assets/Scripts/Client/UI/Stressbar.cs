using Client.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class Stressbar: MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        public PlayerScript playerScript;

        public void Init(PlayerScript ps)
        {
            playerScript = ps;
            Debug.unityLogger.Log($"stress {_slider}");
            _slider.maxValue = (playerScript.unitConfig as SpaceShipConfig).maxStress;
            gameObject.SetActive(true);
        }
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _slider.value = playerScript.currentStress.Value;
        }
    }
}