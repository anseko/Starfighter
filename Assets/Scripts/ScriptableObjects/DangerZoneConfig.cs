using System;
using Core;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "DangerZoneConfig", menuName = "Configs/DangerZoneConfig", order = 0)]
    [Serializable]
    public class DangerZoneConfig : ScriptableObject {
        public float stressDamage;
        public float hpDamage;
        public Vector3 center;
        public float radius;
        public Color color;
        public Guid id;
        public ZoneType type;
    }
}
