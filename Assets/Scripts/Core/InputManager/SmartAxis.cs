using System;
using UnityEngine;

namespace Core.InputManager
{
    [CreateAssetMenu(fileName = "SmartAxis", menuName = "Configs/SmartAxis", order = 0)]
    public class SmartAxis: ScriptableObject
    {
        private float _previousValue = 0;
        
        public float delta = 0.1f;
        public string axisName = "";

        public void Init(string axisName, float delta)
        {
            this.axisName = axisName;
            this.delta = delta;
        }
        
        public void Update()
        {
            var value = Input.GetAxis(axisName);
            if (Math.Abs(value - _previousValue) >= delta || (_previousValue != 0 && value == 0))
            {
                _previousValue = value;
                CoreEventStorage.GetInstance().AxisValueChanged.Invoke(axisName, value);
            }
        }
    }
}