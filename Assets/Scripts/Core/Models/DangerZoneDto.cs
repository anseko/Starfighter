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
        public float damage;
        public Color color;

        public DangerZoneDto(DangerZoneConfig config)
        {
            center = config.Center;
            radius = config.Radius;
            damage = config.Damage;
            color = config.Color;
        }
    }
}