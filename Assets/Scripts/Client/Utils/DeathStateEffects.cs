using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Client.Utils
{
    public class DeathStateEffects: MonoBehaviour
    {
        private Volume _volume;
        private Task _deathTask;

        
        private void Awake()
        {
            _volume = GetComponent<Volume>();
            var vignette = _volume.profile.components.First(x => x is Vignette);
            var blur = _volume.profile.components.First(x => x is DepthOfField);
            vignette.parameters[2].SetValue(new FloatParameter(0));
            blur.parameters[7].SetValue(new FloatParameter(1));
        }

        private void OnEnable()
        {
            _deathTask = GoToDeath(300);
        }

        private async Task GoToDeath(int timeLength)
        {
            var vignette = _volume.profile.components.First(x => x is Vignette);
            var blur = _volume.profile.components.First(x => x is DepthOfField);

            var vignetteParameter = new FloatParameter(0);
            var blurParameter = new FloatParameter(1);

            for (var i = 0; i < timeLength; i++)
            {
                var val = Mathf.Lerp(0, 1, (float)i / timeLength);
                blurParameter.value = i;
                vignetteParameter.value = val;

                vignette.parameters[2].SetValue(vignetteParameter);
                blur.parameters[7].SetValue(blurParameter);
                await Task.Delay(1);
            }
            
        }

        private void OnDisable()
        {
            var vignette = _volume.profile.components.First(x => x is Vignette);
            var blur = _volume.profile.components.First(x => x is DepthOfField);
            vignette.parameters[2].SetValue(new FloatParameter(0));
            blur.parameters[7].SetValue(new FloatParameter(1));
        }
    }
}