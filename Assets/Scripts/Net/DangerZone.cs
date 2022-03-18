using System;
using Core;
using Core.Models;
using Net.Components;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Net
{
    public class DangerZone: NetworkBehaviour
    {
        public NetworkVariable<Color> zoneColor;
        public NetworkVariable<float> zoneStressDamage;
        public NetworkVariable<float> zoneHpDamage;
        public NetworkVariable<float> zoneRadius;
        public NetworkVariable<ZoneType> zoneType;
        private NetworkVariable<FixedString128Bytes> _id;

        public Guid Guid
        {
            get => Guid.Parse(_id.Value.ToString());
            set => _id.Value = new FixedString128Bytes(value.ToString());
        }


        public DangerZoneDto Export()
        {
            var dto = new DangerZoneDto()
            {
                center = transform.position,
                radius = zoneRadius.Value,
                color = zoneColor.Value,
                hpDamage = zoneHpDamage.Value,
                stressDamage = zoneStressDamage.Value,
                id = Guid,
                type = zoneType.Value
            };
            return dto;
        }
        
        private void Awake()
        {
            // var permissionDelegate =
            //     new Write(id =>
            //         IsOwner || IsServer || FindObjectOfType<ConnectionHelper>().userType.Value == UserType.Admin);
            
            zoneColor = new NetworkVariable<Color>();
            zoneRadius = new NetworkVariable<float>();
            zoneStressDamage = new NetworkVariable<float>();
            zoneHpDamage = new NetworkVariable<float>();
            _id = new NetworkVariable<FixedString128Bytes>();
            zoneType = new NetworkVariable<ZoneType>();

            zoneRadius.OnValueChanged += (value, newValue) => { transform.localScale = Vector3.one * 10 * newValue; };
            zoneColor.OnValueChanged += (value, newValue) =>
            {
                var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
                renderer.color = newValue;
            };
        }

        private void Start()
        {
            var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
            renderer.color = zoneColor.Value;
            transform.localScale = Vector3.one * 10 * zoneRadius.Value;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (IsClient) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value += zoneStressDamage.Value;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta.Value += zoneHpDamage.Value;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value -= zoneStressDamage.Value;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta.Value -= zoneHpDamage.Value;
            }
        }
    }
}