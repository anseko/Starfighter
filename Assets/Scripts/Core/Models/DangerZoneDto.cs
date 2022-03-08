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
        public Guid id;
        public ZoneType type;
        
        public DangerZoneDto() {}

        public DangerZoneDto(DangerZoneConfig config)
        {
            center = config.center;
            radius = config.radius;
            stressDamage = config.stressDamage;
            hpDamage = config.hpDamage;
            color = config.color;
            id = config.id;
            type = config.type;
        }
    }
}