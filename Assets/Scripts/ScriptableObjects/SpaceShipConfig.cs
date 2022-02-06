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
    }
}