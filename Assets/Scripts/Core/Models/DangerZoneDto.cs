using System;
using ScriptableObjects;
using UnityEngine;

namespace Core.Models
{
    [Serializable]
    public class DangerZoneDto
    {
        public Vector3 center;
        public float radius;
        public float stressDamage;
        public float hpDamage;
        public Color color;

        public DangerZoneDto(DangerZoneConfig config)
        {
            center = config.Center;
            radius = config.Radius;
            stressDamage = config.StressDamage;
            hpDamage = config.HpDamage;
            color = config.Color;
        }
    }
}