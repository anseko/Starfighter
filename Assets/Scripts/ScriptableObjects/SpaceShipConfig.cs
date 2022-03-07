using System;
using Core;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SpaceShipConfig", menuName = "Configs/SpaceShipConfig", order = 0)]
    [Serializable]
    public class SpaceShipConfig: SpaceUnitConfig
    {
        public float maxStress;
        public float currentStress;
        public string shipId;
        public UnitState shipState;
        [ColorUsage(true, true)]
        public Color baseColor;
        
        public float acceleration;
        public float radarRange;
        
        public float accelerationCoefficient;
        public float physResistanceCoefficient;
        public float radResistanceCoefficient;
        public float radarRangeCoefficient;
    }
}