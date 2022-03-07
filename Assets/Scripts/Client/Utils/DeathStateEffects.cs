using System;
using System.Linq;
using System.Threading.Tasks;
using Client.Core;
using Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Client.Utils
{
    public class DeathStateEffects: MonoBehaviour
    {
        private Volume _volume;
        private PlayerScript _playerScript;

        public void Init(PlayerScript ps)
        {
            _playerScript = ps;

            _playerScript.NetworkUnitConfig._shipState.OnValueChanged += (value, newValue) =>
            {
                
                if (value == UnitState.IsDocked || value == newValue) return;
                switch (newValue)
                {
                    case UnitState.InFlight:
                        GoResurrect(300);
                        break;
                    case UnitState.IsDocked:
                        break;
                    case UnitState.IsDead:
                        GoToDeath(300);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
                }
            };
        }

        private void Awake()
        {
            _volume = GetComponent<Volume>();
            var blur = _volume.profile.components.First(x => x is DepthOfField);
            blur.parameters[7].SetValue(new FloatParameter(1));
        }

        // private void Start()
        // {
        //     switch (_playerScript.NetworkUnitConfig.ShipState)
        //     {
        //         case UnitState.InFlight:
        //             GoResurrect(1);
        //             break;
        //         case UnitState.IsDocked:
        //             break;
        //         case UnitState.IsDead:
        //             GoToDeath(1);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        // }
        
        public async Task GoToDeath(int timeLength)
        {
            var blur = _volume.profile.components.First(x => x is DepthOfField);
            var blurParameter = new FloatParameter(1);
            for (var i = 0; i < timeLength; i++)
            {
                blurParameter.value = i;
                blur.parameters[7].SetValue(blurParameter);
                await Task.Delay(1);
            }
        }
        
        public async Task GoResurrect(int timeLength)
        {
            var blur = _volume.profile.components.First(x => x is DepthOfField);
            var blurParameter = new FloatParameter(timeLength);
            for (var i = timeLength; i >= 1; i--)
            {
                blurParameter.value = i;
                blur.parameters[7].SetValue(blurParameter);
                await Task.Delay(1);
            }
            
        }
    }
}