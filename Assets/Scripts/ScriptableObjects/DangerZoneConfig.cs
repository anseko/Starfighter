using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "DangerZoneConfig", menuName = "Configs/DangerZoneConfig", order = 0)]
    [Serializable]
    public class DangerZoneConfig : ScriptableObject {
        public float StressDamage;
        public float HpDamage;
        public Vector3 Center;
        public float Radius;
        public Color Color;
    }
}
