using System.Linq;
using Client.Core;
using MLAPI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Client.Utils
{
    public class HpMarker : MonoBehaviour
    {
        private Volume _volume;
        private float _maxHp;

        public void Init()
        {
            _volume = FindObjectOfType<Volume>(true);
            _maxHp = gameObject.GetComponent<PlayerScript>().unitConfig.Value.maxHp;
            
            var vignette = _volume.profile.components.First(x => x is Vignette);
            var vignetteParameter = new FloatParameter(Mathf.Lerp(0, 0.4f, (_maxHp - gameObject.GetComponent<PlayerScript>().ShipConfig.currentHp) / _maxHp));
                
            vignette.parameters[2].SetValue(vignetteParameter);
            
            gameObject.GetComponent<PlayerScript>().unitConfig.OnValueChanged += (value, newValue) =>
            {
                if (value.currentHp == newValue.currentHp) return;
                
                var vignette = _volume.profile.components.First(x => x is Vignette);
                var vignetteParameter = new FloatParameter(Mathf.Lerp(0, 0.4f, (_maxHp - newValue.currentHp) / _maxHp));
                
                vignette.parameters[2].SetValue(vignetteParameter);
            };
        }
    }
}