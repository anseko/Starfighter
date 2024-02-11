using System.Linq;
using Client.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Client.Utils
{
    public class HpMarker : MonoBehaviour
    {
        private Volume _volume;
        private float _maxHp;

        public void Init(PlayerScript playerScript)
        {
            _volume = playerScript.volume;
            _maxHp = gameObject.GetComponent<PlayerScript>().networkUnitConfig.maxHp;
            
            var vignette = _volume.profile.components.First(x => x is Vignette);
            var vignetteParameter = new FloatParameter(Mathf.Lerp(0, 0.4f, (_maxHp - gameObject.GetComponent<PlayerScript>().networkUnitConfig.currentHp) / _maxHp));
                
            vignette.parameters[2].SetValue(vignetteParameter);
            
            gameObject.GetComponent<PlayerScript>().networkUnitConfig.currentHp.OnValueChanged += (value, newValue) =>
            {
                var vignette = _volume.profile.components.First(x => x is Vignette);
                var vignetteParameter = new FloatParameter(Mathf.Lerp(0, 0.4f, (_maxHp - newValue) / _maxHp));
                
                vignette.parameters[2].SetValue(vignetteParameter);
            };
        }
    }
}